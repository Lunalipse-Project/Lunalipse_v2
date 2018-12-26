using Lunalipse.Common.Data;
using Lunalipse.Common.Data.Attribute;
using Lunalipse.Common.Interfaces.ICache;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Core.Metadata;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Lunalipse.Core.PlayList
{
    public class Catalogue : ICatalogue, ICachable
    {
        [Cachable]
        private List<MusicEntity> Entities;

        [Cachable]
        private int Currently = 0;

        [Cachable]
        private string name;
        [Cachable]
        private string uid;
        [Cachable]
        private bool AlbumClassified, ArtistClassified, LocationClassified, mainCatalogue;
        [Cachable]
        private int ImageIndex = 0;

        /// <summary>
        /// Store the name of catalogue
        /// </summary>

        public string Name { get => name; }

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
        /// Show whether this catalogue is the "Mother Catalogue" of all songs (inherit from <see cref="MusicListPool.Musics"/>). Each invidual catalogue or "Son Catalogue" inherit the songs from "Mother"
        /// </summary>
        public bool MainCatalogue { get=> mainCatalogue; }

        public int CurrentIndex { get => Currently; }

        public List<MusicEntity> MusicList
        {
            get
            {
                return Entities;
            }
        }

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
            Entities = new List<MusicEntity>();
            if (!isMainUses) MusicListPool.OnMusicDeleted += DeleteMusic;
        }
        public Catalogue(List<MusicEntity> list, string Name, bool isMainUses = false)
            : this(isMainUses)
        {
            name = Name;
            Entities = list;
            if (!isMainUses) MusicListPool.OnMusicDeleted += DeleteMusic;
        }

        public bool AddMusic(MusicEntity ME)
        {
            foreach(MusicEntity me in Entities)
            {
                if (me.Name.Equals(ME.Name)) return false;
            }
            Entities.Add(ME);
            return true;
        }

        public void SortByAlbum()
        {
            if(!isAlbumClassified)
            {
                Entities.Sort((a, b) => a.Album.CompareTo(b.Album));
            }
        }

        public TimeSpan getTotalElapse()
        {
            TimeSpan total = new TimeSpan();
            foreach(MusicEntity me in Entities)
            {
                total += me.EstimateDurSecond;
            }
            return total;
        }

        public void SortByName()
        {
            Entities.Sort((a, b) => a.Name.CompareTo(b.Name));
        }

        public void SortByYear()
        {
            Entities.Sort((a, b) => a.Year.CompareTo(b.Year));
        }

        /// <summary>
        /// Delete Music when a music is deleted from "Mother" catalogue
        /// </summary>
        /// <param name="name">Name of Deleted Music</param>
        /// <returns></returns>
        public bool DeleteMusic(string name)
        {
            return Entities.Remove(Entities.Find(e => e.Name == name));
        }

        public bool DeleteMusic(int index)
        {
            if (index > Entities.Count - 1) return false;
            Entities.RemoveAt(index);
            return true;
        }

        public bool DeleteMusic(MusicEntity ME)
        {
            return Entities.Remove(ME);
        }

        public bool DeleteMusic(int start, int count)
        {
            if (start < 0 || start + count > Entities.Count) return false;
            Entities.RemoveRange(start, count);
            return true;
        }

        public int GetCount()
        {
            return Entities.Count;
        }

        public MusicEntity getMusic(int index)
        {
            if (index > Entities.Count - 1) return null;
            Currently = index;
            return Entities[index];
        }

        public List<MusicEntity> SearchMusic(string name)
        {
            return Entities.FindAll(e => e.Name.Contains(name));
        }

        public MusicEntity getMusic(string name)
        {
            MusicEntity me = Entities.Find(e => e.Name == name);
            Currently = Entities.IndexOf(me);
            return me;
        }

        public void SetMusic(MusicEntity entity)
        {
            int index = MusicList.IndexOf(entity);
            if (index == -1) throw new InvalidOperationException("Entity {0} does not contains in {1}".FormateEx(entity.MusicName, UUID));
            Currently = index;
        }

        public MusicEntity getNext()
        {
            Currently++;
            if (Currently >= Entities.Count)
            {
                Currently = 0;
            }
            return Entities[Currently];
        }

        public MusicEntity getPrevious()
        {
            Currently--;
            if (Currently < 0)
            {
                Currently = 0;
            }
            return Entities[Currently];
        }

        public BitmapSource GetCatalogueCover()
        {
            Random r = new Random();
            MusicEntity randomed = Entities[0];
            BitmapSource bs = null;
            for (int i = 0; i < Entities.Count && (bs = MediaMetaDataReader.GetPicture(randomed.Path)) == null; i++)
            {
                randomed = Entities[ImageIndex = i];
            }
            return bs;
        }

        public IEnumerable<MusicEntity> GetAll()
        {
            foreach(MusicEntity me in Entities)
            {
                yield return me;
            }
        }

        public string Uid() => UUID;

        string ICatalogue.Name() => name;
    }
}
