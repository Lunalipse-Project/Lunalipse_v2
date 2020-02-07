using Lunalipse.Common.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.PlayList.Serialization
{
    public class PlayListSerializer
    {
        /*
         * NOTES:
         *      We can store the entire catalogue of each directory of musics resp.
         *      So we will cache these music and read them out directly without using TagLib to retrieve the information again!
         *      Store the following metadata in MusicEntity : 
         *          Path of music.
         *          ID of music.
         *          Flag Property*
         *          Artists array.
         *          Duration.
         *          ID3Name.
         *      And others properties can be calculate from the metadata shown above.
         *      
         *      Flag Property：
         *          An byte value, where
         *              upper 4 bits is HasImage Property (True = 0xf, False = 0x0)
         *              lower 4 bits is IsInternetLocation (True = 0xf, False = 0x0)
         */

        public static string PlaylistSerializer(Catalogue catalogue)
        {
            StringWriter sw = new StringWriter();
            JsonTextWriter jsonTextWriter = new JsonTextWriter(sw);
            jsonTextWriter.WriteStartObject();
            jsonTextWriter.WritePropertyName("name");
            jsonTextWriter.WriteValue(catalogue.Name);
            jsonTextWriter.WritePropertyName("entries");
            jsonTextWriter.WriteStartArray();
            foreach (MusicEntity musicEntity in catalogue.GetAll())
            {
                jsonTextWriter.WriteStartObject();
                jsonTextWriter.WritePropertyName("id");
                jsonTextWriter.WriteValue(musicEntity.MusicID);
                jsonTextWriter.WritePropertyName("name");
                jsonTextWriter.WriteValue(musicEntity.Name);
                jsonTextWriter.WriteEndObject();
            }
            jsonTextWriter.WriteEndArray();
            jsonTextWriter.WriteEndObject();
            return sw.ToString();
        }

        public static string MusicListSerializer(List<MusicEntity> musicEntities)
        {
            StringWriter sw = new StringWriter();
            JsonTextWriter jsonTextWriter = new JsonTextWriter(sw);
            jsonTextWriter.WriteStartObject();
            jsonTextWriter.WritePropertyName("entries");
            jsonTextWriter.WriteStartArray();
            foreach (MusicEntity musicEntity in musicEntities)
            {
                WriteEntry(musicEntity, jsonTextWriter);
            }
            jsonTextWriter.WriteEndArray();
            jsonTextWriter.WriteEndObject();
            return sw.ToString();
        }

        public static CatalogueMetadata CatalogueDeserializer(string jsonObject)
        {
            JObject jobj = JObject.Parse(jsonObject);
            CatalogueMetadata metadata = new CatalogueMetadata();
            metadata.Musics = new List<Tuple<string, string>>();
            metadata.Name = jobj["name"].Value<string>();
            foreach (JObject o in jobj["entries"].Children<JObject>())
            {
                metadata.Musics.Add(new Tuple<string, string>
                (
                    o["id"].Value<string>(),
                    o["name"].Value<string>()
                ));
            }
            return metadata;
        }

        public static List<MusicEntity> MusicListDeserializer(string jsonObject)
        {
            JObject jobj = JObject.Parse(jsonObject);
            List<MusicEntity> musicEntities = new List<MusicEntity>();
            foreach(JObject token in jobj["entries"].Children<JObject>())
            {
                musicEntities.Add(restoreEntity(token));
            }
            return musicEntities;
        }

        private static void WriteEntry(MusicEntity musicEntity, JsonTextWriter jsonTextWriter)
        {
            byte flag = 0x00;
            flag += (byte)(musicEntity.HasImage ? 0xf0 : 0x00);
            flag += (byte)(musicEntity.IsInternetLocation ? 0x0f : 0x00);
            jsonTextWriter.WriteStartObject();
            jsonTextWriter.WritePropertyName("id");
            jsonTextWriter.WriteValue(musicEntity.MusicID);
            jsonTextWriter.WritePropertyName("path");
            jsonTextWriter.WriteValue(musicEntity.Path);
            jsonTextWriter.WritePropertyName("flag");
            jsonTextWriter.WriteValue(flag);
            jsonTextWriter.WritePropertyName("aritists");
            jsonTextWriter.WriteStartArray();
            for (int i = 0; i < musicEntity.Artist.Length; i++)
            {
                jsonTextWriter.WriteValue(musicEntity.Artist[i]);
            }
            jsonTextWriter.WriteEndArray();
            jsonTextWriter.WritePropertyName("duration");
            jsonTextWriter.WriteValue((int)musicEntity.EstDuration.TotalMilliseconds);
            jsonTextWriter.WritePropertyName("id3n");
            jsonTextWriter.WriteValue(musicEntity.ID3Name);
            jsonTextWriter.WritePropertyName("album");
            jsonTextWriter.WriteValue(musicEntity.Album);
            jsonTextWriter.WriteEndObject();
        }

        private static MusicEntity restoreEntity(JObject obj)
        {
            MusicEntity musicEntity = new MusicEntity();
            byte flag = obj["flag"].Value<byte>();
            musicEntity.MusicID = obj["id"].Value<string>();
            musicEntity.Path = obj["path"].Value<string>();
            musicEntity.Artist = obj["aritists"].Select(x => (string)x).ToArray();
            musicEntity.EstDuration = TimeSpan.FromMilliseconds(obj["duration"].Value<int>());
            musicEntity.ID3Name = obj["id3n"].Value<string>();
            musicEntity.Album = obj["album"].Value<string>();
            musicEntity.IsInternetLocation = (flag & 0x0f) == 0x0f;
            musicEntity.HasImage = (flag & 0xf0) == 0xf0;

            musicEntity.Name = Path.GetFileNameWithoutExtension(musicEntity.Path);
            musicEntity.Extension = Path.GetExtension(musicEntity.Path);
            musicEntity.LyricPath = $"{Path.GetDirectoryName(musicEntity.Path)}\\Lyrics\\{musicEntity.Name}.lrc";
            if (string.IsNullOrEmpty(musicEntity.Artist[0]))
            {
                musicEntity.DefaultArtist = "CORE_PRESENTOR_UNKNOW_ARTIST";
            }
            if (string.IsNullOrEmpty(musicEntity.Album))
            {
                musicEntity.DefaultAlbum = "CORE_PRESENTOR_UNKNOW_ALBUM";
            }
            return musicEntity;
        }
    }
}
