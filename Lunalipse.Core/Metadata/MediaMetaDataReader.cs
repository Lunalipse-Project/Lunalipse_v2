using Lunalipse.Utilities;
using Lunalipse.Common.Data;
using TL = TagLib;
using System.IO;
using Lunalipse.Common;
using System.Windows.Media.Imaging;
using Lunalipse.Common.Interfaces.IMetadata;
using Lunalipse.Common.Interfaces.II18N;

namespace Lunalipse.Core.Metadata
{
    public class MediaMetaDataReader : IMediaMetadataReader
    {
        II18NConvertor Converter;
        public MediaMetaDataReader(II18NConvertor Converter)
        {
            this.Converter = Converter;
        }
        public MusicEntity CreateEntity(string path)
        {
            TL.File media = TL.File.Create(path);
            MusicEntity me = new MusicEntity()
            {
                Album = media.Tag.Album,
                Artist = media.Tag.Performers,
                Extension = Path.GetExtension(path),
                Name = string.IsNullOrEmpty(media.Tag.Title) ? Path.GetFileNameWithoutExtension(path) : media.Tag.Title,
                Year = media.Tag.Year.ToString(),
                Path = path,
            };
            if(me.Artist==null || me.Artist.Length == 0)
            {
                me.Artist = new string[] { Converter.ConvertTo("CORE_FUNC", "CORE_PRESENTOR_UNKNOW_ARTIST") };
            }
            if(string.IsNullOrEmpty(me.Album))
            {
                me.Album = Converter.ConvertTo("CORE_FUNC", "CORE_PRESENTOR_UNKNOW_ALBUM");
            }
            media.Dispose();
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
