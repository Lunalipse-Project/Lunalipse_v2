using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.Cache;
using Lunalipse.Common.Interfaces.ICache;
using Lunalipse.Core.PlayList.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Lunalipse.Common.Generic.Cache.SerializeInfo;

namespace Lunalipse.Core.Cache
{
    public class MusicPoolCache : ICacheOperator
    {
        // An universal id for all cache created via this operator
        const string ID = "allmusic";
        const string PathRelative = "/plcache/";
        string cacheName = "";

        public MusicPoolCache()
        {
            CacheFileInfo cacheFileInfo = new CacheFileInfo();
            cacheFileInfo.cacheType = CacheType.MusicList;
            cacheFileInfo.id = ID;
            cacheName = cacheFileInfo.GenerateName();
        }
        public object InvokeOperator(CacheResponseType crt, object data_to_cache, params object[] args)
        {
            switch(crt)
            {
                case CacheResponseType.SINGLE_CACHE:
                    SaveMusicCache(args[0] as string, data_to_cache as List<MusicEntity>);
                    break;
                case CacheResponseType.SINGLE_RESTORE:
                    CacheFileInfo cacheFileInfo = (CacheFileInfo)args[0];
                    return RestoreMusicCache(cacheFileInfo.id);
                case CacheResponseType.CACHE_EXIST:
                    return File.Exists($"{args[0] as string}{PathRelative}{cacheName}");
            }
            return null;
        }

        public void SaveMusicCache(string locationPath, List<MusicEntity> musicEntities)
        {
            string content = PlayListSerializer.MusicListSerializer(musicEntities);
            WriteToFile(Encoding.UTF8.GetBytes(content),locationPath);
        }

        public List<MusicEntity> RestoreMusicCache(string locationPath)
        {
            string content = Encoding.UTF8.GetString(
                    Compression.Decompress($"{locationPath}{PathRelative}{cacheName}", false)
                );
            return PlayListSerializer.MusicListDeserializer(content);
        }

        public void WriteToFile(byte[] cacheContent, string location)
        {
            string path = $"{location}{PathRelative}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Compression.Compress(cacheContent, $"{path}{cacheName}", false);
        }

        public void SetCacheDir(string BaseDir)
        {
            // do nothing
        }
    }
}
