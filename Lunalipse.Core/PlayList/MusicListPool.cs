using System.Collections.Generic;
using Lunalipse.Common.Data;
using System.IO;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Common.Interfaces.IMetadata;
using Lunalipse.Core.Console;
using Lunalipse.Common.Interfaces.IConsole;
using Lunalipse.Common.Interfaces.ICache;
using Lunalipse.Core.Cache;
using System;
using Lunalipse.Core.Metadata;
using Lunalipse.Core.I18N;
using Lunalipse.Common.Generic.Cache;
using Lunalipse.Utilities;
using Lunalipse.Common.Interfaces.ICommunicator;
using System.Windows.Threading;

namespace Lunalipse.Core.PlayList
{
    internal delegate bool MusicDeleted(string uuid);
    public class MusicListPool : ComponentHandler, IMusicListPool , ICachable
    {
        static volatile MusicListPool mlpInstance;
        static readonly object mlpLock = new object();
        internal static event MusicDeleted OnMusicDeleted;

        CacheHub cacheSystem = CacheHub.INSTANCE();

        LunalipseLogger Log;

        IMediaMetadataReader immdr = null;

        public static MusicListPool INSATNCE(IMediaMetadataReader immdr = null)
        {
            if (mlpInstance == null)
            {
                lock (mlpLock)
                {
                    mlpInstance = mlpInstance ?? new MusicListPool(immdr);
                }
            }
            return mlpInstance;
        }

        private CataloguePool CPool;
        private Catalogue AllMusic;
        public List<MusicEntity> Musics
        {
            get
            {
                return AllMusic.MusicList;
            }
        }

        private MusicListPool(IMediaMetadataReader immdr)
        {
            CPool = CataloguePool.INSATNCE;
            this.immdr = immdr ?? this.immdr;
            if (!CPool.Exists(x => x.MainCatalogue == true || x.Name.Equals("CORE_CATALOGUE_AllMusic")))
                CPool.AddCatalogue(new Catalogue("CORE_CATALOGUE_AllMusic", true));
            AllMusic = CPool.MainCatalogue;
            ConsoleAdapter.INSTANCE.RegisterComponent("lpslist", this);
            Log = LunalipseLogger.GetLogger();
        }

        public string AddToPool(string dirpath, IProgressIndicator indicator = null)
        {
            if (CPool.Exists(x => x.isLocationClassified == true && x.Name.Equals(dirpath)))
                return "";
            if (!Directory.Exists(dirpath))
            {
                Log.Error("Path {0} not exist locally.", dirpath);
                return "";
            }
            Catalogue pathCatalogue = new Catalogue(dirpath)
            {
                isLocationClassified = true
            };
            Log.Info("Loading path \"{0}\"".FormateEx(dirpath));
            string[] files = Directory.GetFiles(dirpath);
            indicator?.SetRange(0, files.Length);
            int counter = 0;
            foreach (string fi in files)
            {
                counter++;
                if(SupportFormat.AllQualified(Path.GetExtension(fi)))
                {
                    MusicEntity me = immdr.CreateEntity(fi);
                    AllMusic.AddMusic(me);
                    pathCatalogue.AddMusic(me);
                    indicator?.ChangeCurrentVal(counter, me.Name);
                }
            }
            indicator?.Complete();
            Log.Debug("{0} musics loaded".FormateEx(pathCatalogue.GetCount()));
            CPool.AddCatalogue(pathCatalogue);
            return pathCatalogue.UUID;
        }

        public List<string> AddToPool(string[] pathes)
        {
            List<string> uuids = new List<string>();
            foreach(string s in pathes)
            {
                if (CPool.Exists(x => x.isLocationClassified == true && x.Name.Equals(s)))
                    continue;
                Catalogue pathCatalogue = new Catalogue(s)
                {
                    isLocationClassified = true
                };
                foreach (string fi in Directory.GetFiles(s))
                {
                    if (SupportFormat.AllQualified(Path.GetExtension(fi)))
                    {
                        MusicEntity me = immdr.CreateEntity(fi);
                        AllMusic.AddMusic(me);
                        pathCatalogue.AddMusic(me);
                    }
                }
                uuids.Add(pathCatalogue.UUID);
                CPool.AddCatalogue(pathCatalogue);
            }
            return uuids;
        }

        public void CreateAlbumClasses()
        {
            if (AllMusic.MusicList.Count == 0) return;
            List<MusicEntity> TemporaryList;
            foreach (MusicEntity me in TemporaryList = AllMusic.MusicList.FindAll(x => !x.AlbumClassfied))
            {
                Catalogue cat;
                bool isNewCatalogue = false;
                string alb = me.Album.Trim();
                string location = Path.GetDirectoryName(me.Path);
                if (!CPool.All.Exists(x => x.Name.Trim().Equals(alb) && x.isAlbumClassified))
                {
                    cat = new Catalogue(alb)
                    {
                        isAlbumClassified = true,
                        ParentUUID = CPool.getUuidByLocation(location)
                    };
                    isNewCatalogue = true;
                }
                else
                {
                    cat = CPool.All.Find((c) => c.isAlbumClassified && c.Name == alb);
                }
                foreach (MusicEntity me_ in TemporaryList)
                {
                    if (me_.Album.Trim() == alb)
                    {
                        cat.AddMusic(me_);
                        me_.AlbumClassfied = true;
                    }
                }
                if (isNewCatalogue)
                    CPool.AddCatalogue(cat);
            }
        }
        public void CreateArtistClasses()
        {
            //if (CPool.Exists(x => x.isArtistClassified == true)) return;
            if (AllMusic.MusicList.Count == 0) return;
            List<MusicEntity> TemporaryList;
            foreach (MusicEntity me in TemporaryList = AllMusic.MusicList.FindAll(x => !x.ArtistClassfied))
            {
                Catalogue cat;
                string art = me.ArtistFrist.Trim();
                bool isNewCatalogue = false;
                string location = Path.GetDirectoryName(me.Path);
                if (!CPool.All.Exists(x => x.Name.Trim().Equals(art) && x.isArtistClassified))
                {
                    cat = new Catalogue(art)
                    {
                        isArtistClassified = true,
                        ParentUUID = CPool.getUuidByLocation(location)
                    };
                    isNewCatalogue = true;
                }
                else
                {
                    cat = CPool.All.Find((c) => c.Name.Equals(art) && c.isArtistClassified);
                }
                foreach (MusicEntity me_ in TemporaryList)
                {
                    if (me_.ArtistFrist.Trim() == art)
                    {
                        cat.AddMusic(me_);
                        me_.ArtistClassfied = true;
                    }
                }
                if (isNewCatalogue)
                    CPool.AddCatalogue(cat);
            }
        }

        public void DeleteMusic(MusicEntity entity, bool complete)
        {
            if (complete) File.Delete(entity.Path);
            OnMusicDeleted?.Invoke(entity.Name);
            AllMusic.DeleteMusic(entity);
        }

        public bool AddFileToPool(string MediaPath)
        {
            if (SupportFormat.AllQualified(Path.GetExtension(MediaPath)))
            {
                AllMusic.AddMusic(immdr.CreateEntity(MediaPath));
                return true;
            }
            return false;
        }

        public List<MusicEntity> GetMusics(string any, MusicEntityType mety)
        {
            return AllMusic.MusicList.FindAll(delegate (MusicEntity e)
            {
                switch (mety)
                {
                    case MusicEntityType.ARTIST:
                        return e.Artist[0].Equals(any);
                    case MusicEntityType.ALBUM:
                        return e.Album.Equals(any);
                    case MusicEntityType.NAME:
                        return e.Name.Equals(any);
                    default:
                        return false;
                }
            });
        }

        public MusicEntity GetMusic(int index)
        {
            return index > AllMusic.MusicList.Count - 1 ? null : AllMusic.MusicList[index];
        }

        public ICatalogue ToCatalogue()
        {
            return AllMusic;
        }

        public List<MusicEntity> getListObject()
        {
            return Musics;
        }

        public void LoadAllMusics()
        {
            if (cacheSystem.ComponentCacheExists(CacheType.MUSIC_CATALOGUE_CACHE))
            {
                foreach (Catalogue cat in cacheSystem.RestoreObjects<Catalogue>(
                    x => x.markName == "CATALOGUE",
                    CacheType.MUSIC_CATALOGUE_CACHE))
                {
                    CPool.AddCatalogue(cat);
                }
            }
            else
            {
                //查看是否存在用户设置
                //if(cacheSystem.ComponentCacheExists(CacheType.))
            }
        }
    }
}
