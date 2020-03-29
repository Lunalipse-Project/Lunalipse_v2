using Lunalipse.Common.Data;
using MinJSON.JSON;
using MinJSON.Writer;
using System;
using System.Collections.Generic;
using System.IO;

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
            ISeqentialWriter jsonTextWriter = new JsonTextWriter();
            jsonTextWriter.WriteObjectBegin();
            jsonTextWriter.WriteProperty("name");
            jsonTextWriter.WriteString(catalogue.Name);
            jsonTextWriter.WriteProperty("entries");
            jsonTextWriter.WriteArrayBegin();
            foreach (MusicEntity musicEntity in catalogue.GetAll())
            {
                jsonTextWriter.WriteObjectBegin();
                jsonTextWriter.WriteProperty("id");
                jsonTextWriter.WriteString(musicEntity.MusicID);
                jsonTextWriter.WriteProperty("name");
                jsonTextWriter.WriteString(musicEntity.Name);
                jsonTextWriter.WriteObjectEnd();
            }
            jsonTextWriter.WriteArrayEnd();
            jsonTextWriter.WriteObjectEnd();
            return jsonTextWriter.ToString();
        }

        public static string MusicListSerializer(List<MusicEntity> musicEntities)
        {
            ISeqentialWriter jsonTextWriter = new JsonTextWriter();
            jsonTextWriter.WriteObjectBegin();
            jsonTextWriter.WriteProperty("entries");
            jsonTextWriter.WriteArrayBegin();
            foreach (MusicEntity musicEntity in musicEntities)
            {
                WriteEntry(musicEntity, jsonTextWriter);
            }
            jsonTextWriter.WriteArrayEnd();
            jsonTextWriter.WriteObjectEnd();
            return jsonTextWriter.ToString();
        }

        public static CatalogueMetadata CatalogueDeserializer(string jsonObject)
        {
            JsonObject jobj = JsonObject.Parse(jsonObject);
            CatalogueMetadata metadata = new CatalogueMetadata();
            metadata.Musics = new List<Tuple<string, string>>();
            metadata.Name = jobj["name"].As<string>();
            foreach (JsonObject o in (jobj["entries"] as JsonArray))
            {
                metadata.Musics.Add(new Tuple<string, string>
                (
                    o["id"].As<string>(),
                    o["name"].As<string>()
                ));
            }
            return metadata;
        }

        public static List<MusicEntity> MusicListDeserializer(string jsonObject)
        {
            JsonObject jobj = JsonObject.Parse(jsonObject);
            List<MusicEntity> musicEntities = new List<MusicEntity>();
            foreach(JsonObject token in jobj["entries"] as JsonArray)
            {
                musicEntities.Add(restoreEntity(token));
            }
            return musicEntities;
        }

        private static void WriteEntry(MusicEntity musicEntity, ISeqentialWriter jsonTextWriter)
        {
            byte flag = 0x00;
            flag += (byte)(musicEntity.HasImage ? 0xf0 : 0x00);
            flag += (byte)(musicEntity.IsInternetLocation ? 0x0f : 0x00);
            jsonTextWriter.WriteObjectBegin();
            jsonTextWriter.WriteProperty("id");
            jsonTextWriter.WriteString(musicEntity.MusicID);
            jsonTextWriter.WriteProperty("path");
            jsonTextWriter.WriteString(musicEntity.Path);
            jsonTextWriter.WriteProperty("flag");
            jsonTextWriter.WriteByte(flag);
            jsonTextWriter.WriteProperty("aritists");
            jsonTextWriter.WriteArrayBegin();
            for (int i = 0; i < musicEntity.Artist.Length; i++)
            {
                jsonTextWriter.WriteString(musicEntity.Artist[i]);
            }
            jsonTextWriter.WriteArrayEnd();
            jsonTextWriter.WriteProperty("duration");
            jsonTextWriter.WriteInteger((int)musicEntity.EstDuration.TotalMilliseconds);
            jsonTextWriter.WriteProperty("id3n");
            jsonTextWriter.WriteString(musicEntity.ID3Name);
            jsonTextWriter.WriteProperty("album");
            jsonTextWriter.WriteString(musicEntity.Album);
            jsonTextWriter.WriteObjectEnd();
        }

        private static MusicEntity restoreEntity(JsonObject obj)
        {
            MusicEntity musicEntity = new MusicEntity();
            byte flag = obj["flag"].As<byte>();
            musicEntity.MusicID = obj["id"].As<string>();
            musicEntity.Path = obj["path"].As<string>();
            musicEntity.Artist = obj["aritists"].As<string[]>();
            musicEntity.EstDuration = TimeSpan.FromMilliseconds(obj["duration"].As<int>());
            musicEntity.ID3Name = obj["id3n"].As<string>();
            musicEntity.Album = obj["album"].As<string>();
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
