using System;
using System.IO;
using System.Windows.Media.Imaging;
using Lunalipse.Utilities;
using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.IMetadata;
using Lunalipse.Common.Generic.Cache;
using Lunalipse.Core.Cache;

using TL = TagLib;

namespace Lunalipse.Core.Metadata
{
    public class MediaMetaDataReader : IMediaMetadataReader
    {
        const string LyricPathFormat = "{0}\\Lyrics\\{1}.lrc";
        CacheHub cacheHub;

        public MediaMetaDataReader()
        {
            cacheHub = CacheHub.Instance();
        }

        public MusicEntity CreateEntity(string path)
        {
            MusicEntity me = new MusicEntity()
            {
                Extension = Path.GetExtension(path),
                Name = Path.GetFileNameWithoutExtension(path),
                Path = path,
                MusicID = Utils.getRandomID()
            };
            string lyricPath = string.Format(LyricPathFormat, Path.GetDirectoryName(path), me.Name);
            try
            {
                TL.File media = TL.File.Create(path);
                me.Album = media.Tag.Album;
                me.Artist = media.Tag.Performers;
                me.ID3Name = string.IsNullOrEmpty(media.Tag.Title) ? "" : media.Tag.Title;
                me.EstDuration = media.Properties.Duration;
                me.LyricPath = File.Exists(lyricPath) ? lyricPath : string.Empty;
                if (media.Tag.Pictures != null)
                {
                    me.HasImage = media.Tag.Pictures.Length != 0;
                    if(me.HasImage)
                    {
                        cacheHub.CacheObject(media.Tag.Pictures[0].Data.Data, CacheType.ALBUM_PIC, me.MusicID);
                    }
                }
                media.Dispose();
            }
            catch (Exception)
            {
                LunalipseLogger.GetLogger().Warning("Unable to load IDv3 tag, skipping...");
                me.EstDuration = TimeSpan.Zero;
                me.HasImage = false;
            }
            if (me.Artist==null || me.Artist.Length == 0)
            {
                me.Artist = new string[1] { "" };
                me.DefaultArtist = "CORE_PRESENTOR_UNKNOW_ARTIST";
            }
            if(string.IsNullOrEmpty(me.Album))
            {
                me.DefaultAlbum = "CORE_PRESENTOR_UNKNOW_ALBUM";
                me.Album = "";
            }
            return me;
        }

        public TL.File CreateRaw(string path)
        {
            return TL.File.Create(path);
        }
        
        /// <summary>
        /// Cache the album cover of music represent by <see cref="MusicEntity"/>
        /// </summary>
        /// <param name="musicEntity">Music</param>
        /// <returns><see cref="byte[]"/> of cached cover</returns>
        public static byte[] CacheCover(MusicEntity musicEntity)
        {
            TL.File media = TL.File.Create(musicEntity.Path);
            if (media.Tag.Pictures != null)
            {
                musicEntity.HasImage = media.Tag.Pictures.Length != 0;
                if (musicEntity.HasImage)
                {
                    CacheHub.Instance().CacheObject(media.Tag.Pictures[0].Data.Data, CacheType.ALBUM_PIC, musicEntity.MusicID);
                    return media.Tag.Pictures[0].Data.Data;
                }
            }
            return null;
        }

        public static BitmapSource GetPicture(MusicEntity entity)
        {
            if (!entity.HasImage) return null;
            return Graphic.Byte2BitmapSource(entity.AlbumPicture);
        }

        public static void RetrievePictureFromCache(MusicEntity entity)
        {
            CacheType cacheType = entity.IsInternetLocation ? CacheType.WebAlbumPic : CacheType.ALBUM_PIC;
            byte[] cover = null;
            if (!CacheHub.Instance().ComponentCacheExists(
                cacheType,
                entity.MusicID))
            {
                if (cacheType == CacheType.ALBUM_PIC)
                {
                    cover = CacheCover(entity);
                }
                else
                {
                    return;
                }
            }
            else
            {
                cover = CacheHub.Instance().RestoreObject<byte[]>(entity.MusicID, cacheType);
            }
            entity.InitializePicture(cover);
        }
    }
}
