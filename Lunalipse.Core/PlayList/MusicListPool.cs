using System.Collections.Generic;
using Lunalipse.Common.Data;
using System.IO;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Common.Interfaces.IMetadata;
using Lunalipse.Core.Console;
using Lunalipse.Common.Interfaces.IConsole;
using Lunalipse.Common.Interfaces.ICache;

namespace Lunalipse.Core.PlayList
{
    internal delegate bool MusicDeleted(string uuid);
    public class MusicListPool : ComponentHandler, IMusicListPool , ICachable
    {
        static volatile MusicListPool mlpInstance;
        static readonly object mlpLock = new object();
        internal static event MusicDeleted OnMusicDeleted;

        public static MusicListPool INSATNCE
        {
            get
            {
                if(mlpInstance == null)
                {
                    lock(mlpLock)
                    {
                        mlpInstance = mlpInstance ?? new MusicListPool();
                    }
                }
                return mlpInstance;
            }
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

        private MusicListPool()
        {
            CPool = CataloguePool.INSATNCE;
            if (!CPool.Exists(x => x.MainCatalogue == true || x.Name.Equals("CORE_CATALOGUE_AllMusic")))
                CPool.AddCatalogue(new Catalogue("CORE_CATALOGUE_AllMusic", true));
            AllMusic = CPool.MainCatalogue;
            ConsoleAdapter.INSTANCE.RegisterComponent("lpslist", this);
        }

        public void AddToPool(string dirpath, IMediaMetadataReader immr)
        {
            if (CPool.Exists(x => x.isLocationClassified == true && x.Name.Equals(dirpath)))
                return;
            Catalogue pathCatalogue = new Catalogue(dirpath)
            {
                isLocationClassified = true
            };
            foreach(string fi in Directory.GetFiles(dirpath))
            {
                if(SupportFormat.AllQualified(Path.GetExtension(fi)))
                {
                    MusicEntity me = immr.CreateEntity(fi);
                    AllMusic.AddMusic(me);
                    pathCatalogue.AddMusic(me);
                }
            }
            CPool.AddCatalogue(pathCatalogue);
        }

        public void AddToPool(string[] pathes, IMediaMetadataReader immr)
        {
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
                        MusicEntity me = immr.CreateEntity(fi);
                        AllMusic.AddMusic(me);
                        pathCatalogue.AddMusic(me);
                    }
                }
                CPool.AddCatalogue(pathCatalogue);
            }
        }

        public void CreateAlbumClasses()
        {
            if (AllMusic.MusicList.Count != 0)
            {
                List<string> searchedAlb = new List<string>();
                foreach(MusicEntity me in AllMusic.MusicList)
                {
                    Catalogue cat;
                    string alb = me.Album.Trim();
                    if (!searchedAlb.Exists((x) => alb == x.Trim()))
                    {
                        searchedAlb.Add(alb);
                        if (CPool.Exists(x => x.isAlbumClassified == true && x.Name.Equals(alb)))
                            continue;
                        cat = new Catalogue(alb)
                        {
                            isAlbumClassified = true
                        };
                    }
                    else continue;
                    foreach(MusicEntity me_ in AllMusic.MusicList)
                    {
                        if(me_.Album.Trim() == alb)
                        {
                            cat.AddMusic(me_);
                        }
                    }
                    CPool.AddCatalogue(cat);
                }
            }
        }
        public void CreateArtistClasses()
        {
            if (CPool.Exists(x => x.isArtistClassified == true)) return;
            if (AllMusic.MusicList.Count != 0)
            {
                List<string> searchedArt = new List<string>();
                foreach (MusicEntity me in AllMusic.MusicList)
                {
                    Catalogue cat;
                    string alb = me.ArtistFrist.Trim();
                    if (!searchedArt.Exists((x) => alb == x.Trim()))
                    {
                        searchedArt.Add(alb);
                        if (CPool.Exists(x => x.isAlbumClassified == true && x.Name.Equals(alb)))
                            continue;
                        cat = new Catalogue(alb)
                        {
                            isArtistClassified = true
                        };
                    }
                    else continue;
                    foreach (MusicEntity me_ in AllMusic.MusicList)
                    {
                        if (me_.ArtistFrist.Trim() == alb)
                        {
                            cat.AddMusic(me_);
                        }
                    }
                    CPool.AddCatalogue(cat);
                }
            }
        }

        public void DeleteMusic(MusicEntity entity, bool complete)
        {
            if (complete) File.Delete(entity.Path);
            OnMusicDeleted?.Invoke(entity.Name);
            AllMusic.DeleteMusic(entity);
        }

        public bool AddFileToPool(string MediaPath, IMediaMetadataReader immr)
        {
            if (SupportFormat.AllQualified(Path.GetExtension(MediaPath)))
            {
                AllMusic.AddMusic(immr.CreateEntity(MediaPath));
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
    }
}
