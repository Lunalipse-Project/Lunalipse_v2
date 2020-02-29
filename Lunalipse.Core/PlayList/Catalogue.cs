using Lunalipse.Common.Bus.Event;
using Lunalipse.Common.Data;
using Lunalipse.Common.Data.Attribute;
using Lunalipse.Common.Generic.Cache;
using Lunalipse.Common.Interfaces.ICache;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Core.Cache;
using Lunalipse.Core.Metadata;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Lunalipse.Core.PlayList
{
    [Serializable]
    public class Catalogue : ICatalogue, ICachable
    {
        [NonSerialized]
        private int Currently = 0;

        private string name;

        private string uid;

        private bool AlbumClassified, ArtistClassified, LocationClassified, mainCatalogue, UserDefined;
        [NonSerialized]
        private int ImageIndex = -1;

        [NonSerialized]
        private int CatalogueChanges = 0;

        public bool IsModified
        {
            get => CatalogueChanges == 0;
        }

        /// <summary>
        /// Store the name of catalogue
        /// </summary>

        public string Name { get => name;
            set {
                name = value;
                CatalogueChanges++;
            }
        }

        /// <summary>
        /// The Unique ID of the catalogue (MUST UNIQUE!)
        /// </summary>
        public string UUID { get => uid; }

        /// <summary>
        /// Show whether this catalogue is a particular album.
        /// Which means the catalogue is not created by user but by software base on the album atrribute of the songs
        /// in <see cref="MusicListPool"/>.
        /// </summary>
        public bool isAlbumClassified { get => AlbumClassified; set => AlbumClassified = value; }

        public bool isArtistClassified { get => ArtistClassified; set => ArtistClassified = value; }

        /// <summary>
        /// Indicate that this catalogue is classified by path on disk
        /// </summary>
        public bool isLocationClassified { get => LocationClassified; set => LocationClassified = value; }

        /// <summary>
        /// Indicate that this catalogue is created by user
        /// </summary>
        public bool isUserDefined { get => UserDefined; set => UserDefined = value; }

        /// <summary>
        /// Show whether this catalogue is the "Mother Catalogue" of all songs (inherit from <see cref="MusicListPool.Musics"/>). Each invidual catalogue or "Son Catalogue" inherit the songs from "Mother"
        /// </summary>
        public bool MainCatalogue { get=> mainCatalogue; }

        public int CurrentIndex { get => Currently; }

        /// <summary>
        /// 父对象的UUID。
        /// 该对象是从另一个对象中派生过来的。
        /// </summary>
        public string ParentUUID { get; set; }

        public List<MusicEntity> MusicList { get; set; }

        public Catalogue()
        {

        }

        public Catalogue(bool isMainUses = false)
        {
            uid = Guid.NewGuid().ToString();
            mainCatalogue = isMainUses;
        }
        /// <summary>
        /// Create a new catalogue instance
        /// </summary>
        /// <param name="Name">Name of Catalogue, if <see cref="isLocationClassified"/> set to <see cref="true"/>, then it represent the path</param>
        /// <param name="isMainUses"></param>
        public Catalogue(string Name, bool isMainUses = false)
            : this(isMainUses)
        {
            name = Name;
            MusicList = new List<MusicEntity>();
            if (!isMainUses)
            {
                MusicListPool.OnMusicDeleted += DeleteMusic;
                EventBus.OnMulticastRecieved += EventBus_OnMulticastRecieved;
            }
        }

        public Catalogue(string Name,string Uid, bool isMainUses = false)
            : this(isMainUses)
        {
            name = Name;
            MusicList = new List<MusicEntity>();
            if (!isMainUses)
            {
                MusicListPool.OnMusicDeleted += DeleteMusic;
                EventBus.OnMulticastRecieved += EventBus_OnMulticastRecieved;
            }
            this.uid = Uid;
        }

        public Catalogue(List<MusicEntity> list, string Name, bool isMainUses = false)
            : this(isMainUses)
        {
            name = Name;
            MusicList = list;
            if (!isMainUses)
            {
                MusicListPool.OnMusicDeleted += DeleteMusic;
                EventBus.OnMulticastRecieved += EventBus_OnMulticastRecieved;
            }
        }
        private void EventBus_OnMulticastRecieved(EventBusTypes MsgType, object Msg, object MsgReceiver)
        {
            if (!(MsgReceiver is string)) return;
            if (MsgReceiver as string != uid) return;
            if (MsgType== EventBusTypes.ON_ACTION_REQ_DELETE)
            {
                DeleteMusic(Msg as MusicEntity);
            }
            else if(MsgType == EventBusTypes.ON_ACTION_UPDATE)
            {
                //oldIndex, newIndex
                Tuple<int, int> MsgTuple = Msg as Tuple<int, int>;
                MusicEntity ME_Temp = MusicList[MsgTuple.Item1];
                DeleteMusic(MsgTuple.Item1);
                MusicList.Insert(MsgTuple.Item2, ME_Temp);
            }
        }

        public bool AddMusic(MusicEntity ME)
        {
            if (MusicList.Exists(x => x.MusicID == ME.MusicID)) return false;
            MusicList.Add(ME);
            // '2' for entry added or removed.
            CatalogueChanges += 2;
            return true;
        }

        public bool AddMusicCollection(List<MusicEntity> MusicCollection)
        {
            bool allSuccess = true;
            foreach(MusicEntity me in MusicCollection)
            {
                allSuccess = allSuccess && AddMusic(me);
            }
            return allSuccess;
        }

        public void SortByAlbum()
        {
            if(!isAlbumClassified)
            {
                MusicList.Sort((a, b) => a.Album.CompareTo(b.Album));
            }
        }

        public TimeSpan getTotalElapse()
        {
            TimeSpan total = new TimeSpan();
            foreach(MusicEntity me in MusicList)
            {
                total += me.EstimateDurSecond;
            }
            return total;
        }

        public void SortByName()
        {
            MusicList.Sort((a, b) => a.Name.CompareTo(b.Name));
        }

        /// <summary>
        /// Delete Music when a music is deleted from "Mother" catalogue
        /// </summary>
        /// <param name="name">Name of Deleted Music</param>
        /// <returns></returns>
        public bool DeleteMusic(string name)
        {
            return DeleteMusic(
                MusicList.Find(
                    e => e.Name == name
                    )
                );
        }

        public bool DeleteMusic(int index)
        {
            if (index > MusicList.Count - 1) return false;
            return DeleteMusic(MusicList[index]);
        }

        public bool DeleteMusic(MusicEntity ME)
        {
            CatalogueChanges -= 2;
            return MusicList.Remove(ME);
        }

        public bool DeleteMusic(int start, int count)
        {
            if (start < 0 || start + count > MusicList.Count) return false;
            CatalogueChanges -= 2;
            MusicList.RemoveRange(start, count);
            return true;
        }

        public int GetCount()
        {
            return MusicList.Count;
        }

        public MusicEntity getMusic(int index)
        {
            if (index > MusicList.Count - 1) return null;
            Currently = index;
            return MusicList[index];
        }

        public List<MusicEntity> SearchMusic(string name)
        {
            return MusicList.FindAll(e => e.Name.Contains(name));
        }

        public MusicEntity getMusic(string name)
        {
            MusicEntity me = MusicList.Find(e => e.Name.Equals(name) || e.Name.Equals(name));
            Currently = MusicList.IndexOf(me);
            return me;
        }

        public void SetMusic(MusicEntity entity)
        {
            int index = MusicList.IndexOf(entity);
            if (index == -1) throw new InvalidOperationException("Entity '{0}' does not contains in '{1}'".FormateEx(entity.MusicName, UUID));
            Currently = index;
        }

        public MusicEntity getNext()
        {
            Currently++;
            if (Currently >= MusicList.Count)
            {
                Currently = 0;
            }
            return MusicList[Currently];
        }

        public MusicEntity getPrevious()
        {
            Currently--;
            if (Currently < 0)
            {
                Currently = 0;
            }
            return MusicList[Currently];
        }

        public BitmapSource GetCatalogueCover()
        {
            if (ImageIndex != -1)
            {
                MediaMetaDataReader.RetrievePictureFromCache(MusicList[ImageIndex]);
                BitmapSource btmap = MediaMetaDataReader.GetPicture(MusicList[ImageIndex]);
                MusicList[ImageIndex].DisposePicture();
                return btmap;
            }
            List<MusicEntity> PictureHolder = MusicList.FindAll(x => x.HasImage);
            if (PictureHolder.Count == 0) return null;
            Random random = new Random();
            MusicEntity holder = PictureHolder[random.Next(PictureHolder.Count)];
            MediaMetaDataReader.RetrievePictureFromCache(holder);
            BitmapSource bitmap = MediaMetaDataReader.GetPicture(holder);
            holder.DisposePicture();
            return bitmap;
        }

        public IEnumerable<MusicEntity> GetAll()
        {
            foreach(MusicEntity me in MusicList)
            {
                yield return me;
            }
        }

        public string Uid() => UUID;

        

        string ICatalogue.Name() => name;

        public bool IsUserDefined()
        {
            return isUserDefined;
        }

        public MusicEntity getCurrent()
        {
            return MusicList[Currently];
        }

        public bool IsLocationClassification()
        {
            return isLocationClassified;
        }
    }
}
