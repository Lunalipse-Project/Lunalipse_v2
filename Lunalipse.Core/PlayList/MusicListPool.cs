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
using System.Linq;
using Lunalipse.Utilities.Misc;

namespace Lunalipse.Core.PlayList
{
    internal delegate bool MusicDeleted(string uuid);
    public class MusicListPool : IConsoleComponent, IMusicListPool , ICachable
    {
        static volatile MusicListPool mlpInstance;
        static readonly object mlpLock = new object();
        internal static event MusicDeleted OnMusicDeleted;

        CacheHub cacheSystem = CacheHub.Instance();
        CommandRegistry commandRegistry = new CommandRegistry();

        LunalipseLogger Log;

        IMediaMetadataReader immdr = null;

        public static MusicListPool Instance(IMediaMetadataReader immdr = null)
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
            CPool = CataloguePool.Instance;
            this.immdr = immdr ?? this.immdr;
            if (!CPool.Exists(x => x.MainCatalogue == true || x.Name.Equals("CORE_CATALOGUE_AllMusic")))
                CPool.AddCatalogue(new Catalogue("CORE_CATALOGUE_AllMusic", true));
            AllMusic = CPool.MainCatalogue;
            ConsoleAdapter.Instance.RegisterComponent(GetType().Name, this);
            Log = LunalipseLogger.GetLogger();
        }

        public string AddToPool(string dirpath, IProgressIndicator indicator = null, bool indicatorManuallyClose = false)
        {
            if (CPool.Exists(x => x.isLocationClassified == true && x.Name.Equals(dirpath)))
                return string.Empty;
            if (!Directory.Exists(dirpath))
            {
                Log.Error("Path {0} not exist locally.", dirpath);
                return string.Empty;
            }
            bool cacheAvailable = cacheSystem.ComponentCacheExists(CacheType.MusicList, dirpath);
            Catalogue pathCatalogue = new Catalogue(dirpath)
            {
                isLocationClassified = true
            };
            Log.Info("Loading path \"{0}\"".FormateEx(dirpath));
            indicator?.UpdateCaption(dirpath);
            List<string> fileOnDisk = Directory.GetFiles(dirpath).ToList();
            if (cacheAvailable)
            {
                indicator?.SetRange(0, 0);
                indicator?.ChangeCurrentVal(-1, $"Retrieving caches for {dirpath}");
                LunalipseLogger.GetLogger().Info($"Retriving cache from cache. Component: {dirpath}");
                List<MusicEntity> entities = cacheSystem.RestoreObject<List<MusicEntity>>(dirpath, CacheType.MusicList);
                if (entities == null)
                {
                    LunalipseLogger.GetLogger().Warning($"Unable to retrieve cache. Component: {dirpath}");
                    ReadMusicFromDisk(fileOnDisk, indicator, pathCatalogue);
                }
                else
                {
                    applyToPool(entities, fileOnDisk, pathCatalogue, indicator);
                }
            }
            else
            {
                LunalipseLogger.GetLogger().Warning($"Retrieving music from disk. Component: {dirpath}");
                ReadMusicFromDisk(fileOnDisk, indicator, pathCatalogue);
                LunalipseLogger.GetLogger().Warning($"Building cache. Component: {dirpath}");
                cacheSystem.CacheObject(pathCatalogue.MusicList, CacheType.MusicList, dirpath);
            }
            if(!indicatorManuallyClose)
            {
                indicator?.Complete();
            }
            Log.Debug("{0} musics loaded".FormateEx(pathCatalogue.GetCount()));
            CPool.AddCatalogue(pathCatalogue);
            return pathCatalogue.UUID;
        }

        /// <summary>
        /// Apply the music restored from cache to music pool.
        /// And compare the cached with the musics in current location on disk.
        /// If there are any mismatch, then solve it.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="files"></param>
        private void applyToPool(List<MusicEntity> entities, List<string> files, Catalogue catalogue, IProgressIndicator indicator)
        {
            List<string> filesInCache = new List<string>();
            entities.ForEach(x =>
            {
                if (File.Exists(x.Path))
                {
                    AllMusic.AddMusic(x);
                    catalogue.AddMusic(x);
                    filesInCache.Add(x.Path);
                }
                else
                {
                    //In this case, entry (music) recorded by cache but not exist in physical location (disk)
                    //So we need not to add this entry to pool and catalogue. And delete the album cover cache if it has.
                    if (x.HasImage)
                    {
                        cacheSystem.DeleteCache(CacheType.ALBUM_PIC, x.MusicID);
                    }
                }
            });
            // User may modify their music library without using Lunalipse.
            // In this case, user may:
            //      add or remove music only
            //      remove N musics but also add another M musics (where N,M>=0)

            // Extra: Extra music in the physical location.
            List<string> Extra = files.Except(filesInCache).ToList();
            if (Extra.Count != 0)
            {
                // Add extra musics to pool and catalogue from disk
                ReadMusicFromDisk(Extra, indicator, catalogue);
            }
        }

        private void ReadMusicFromDisk(List<string> pathes, IProgressIndicator indicator, Catalogue pathCatalogue)
        {
            indicator?.SetRange(0, pathes.Count);
            int counter = 0;
            foreach (string fi in pathes)
            {
                if (SupportFormat.AllQualified(Path.GetExtension(fi)))
                {
                    MusicEntity me = immdr.CreateEntity(fi);
                    AllMusic.AddMusic(me);
                    pathCatalogue.AddMusic(me);
                    counter++;
                    indicator?.ChangeCurrentVal(counter, me.Name);
                }
            }
        }

        [Obsolete]
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

        [AttrConsoleSupportable]
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

        [AttrConsoleSupportable]
        public void LoadAllMusics()
        {
            //if (cacheSystem.ComponentCacheExists(CacheType.MUSIC_CATALOGUE_CACHE))
            //{
            //    foreach (Catalogue cat in cacheSystem.RestoreObjects<Catalogue>(
            //        x => x.markName == "CATALOGUE",
            //        CacheType.MUSIC_CATALOGUE_CACHE))
            //    {
            //        CPool.AddCatalogue(cat);
            //    }
            //}
            //else
            //{
            //    //查看是否存在用户设置
            //    //if(cacheSystem.ComponentCacheExists(CacheType.))
            //}
        }

        public void OnEnvironmentLoaded(ILunaConsole console)
        {
            console.WriteLine("Welcome to MusicListPool.\nUse \"help\" for more information.");
        }

        public bool OnCommand(ILunaConsole console, params string[] args)
        {
            
            return true;
        }

        public ICommandRegistry GetCommandRegistry()
        {
            return commandRegistry;
        }

        public string GetContextDescription()
        {
            return "Manage all musics that are loaded by Lunalipse.";
        }
    }
}
