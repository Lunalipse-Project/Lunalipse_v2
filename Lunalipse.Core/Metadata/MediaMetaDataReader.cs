using Lunalipse.Utilities;
using Lunalipse.Common.Data;
using TL = TagLib;
using System.IO;
using Lunalipse.Common;
using System.Windows.Media.Imaging;
using Lunalipse.Common.Interfaces.IMetadata;
using Lunalipse.Common.Interfaces.II18N;
using System;

namespace Lunalipse.Core.Metadata
{
    public class MediaMetaDataReader : IMediaMetadataReader
    {
        public MusicEntity CreateEntity(string path)
        {
            MusicEntity me = new MusicEntity()
            {
                Extension = Path.GetExtension(path),
                Name = Path.GetFileNameWithoutExtension(path),
                Path = path,
            };
            try
            {
                TL.File media = TL.File.Create(path);
                me.Album = media.Tag.Album;
                me.Artist = media.Tag.Performers;
                me.ID3Name = string.IsNullOrEmpty(media.Tag.Title) ? "" : media.Tag.Title;
                me.Year = media.Tag.Year.ToString();
                me.EstDuration = media.Properties.Duration;
                if (media.Tag.Pictures != null)
                    me.HasImage = media.Tag.Pictures.Length != 0;
                media.Dispose();
            }
            catch (Exception)
            {
                LunalipseLogger.GetLogger().Warning("Unable to load IDv3 tag, skipping...");
                me.Year = "1900";
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

        public static BitmapSource GetPicture(string path)
        {
            TL.File media = TL.File.Create(path);
            BitmapSource bs = getPic(media.Tag);
            media.Dispose();
            return bs;
        }

        private static BitmapSource getPic(TL.Tag tag)
        {
            if (tag.Pictures == null || tag.Pictures.Length == 0) return null;
            return Graphic.Byte2BitmapSource(tag.Pictures[0].Data.Data);
        }

        
    }
}
