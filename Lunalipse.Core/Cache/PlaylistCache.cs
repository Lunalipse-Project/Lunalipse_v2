using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.Cache;
using Lunalipse.Common.Interfaces.ICache;
using Lunalipse.Core.PlayList;
using Lunalipse.Core.PlayList.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Lunalipse.Common.Generic.Cache.SerializeInfo;

namespace Lunalipse.Core.Cache
{
    public class PlaylistCache : ICacheOperator
    {
        public string CacheDir { get; private set; }
        const string cacheFolder = "UserData";
        public object InvokeOperator(CacheResponseType crt, object data_to_cache, params object[] args)
        {
            switch (crt)
            {
                case CacheResponseType.DELETE_CACHE:
                    CacheFileInfo fileInfo = new CacheFileInfo()
                    {
                        id = args[0] as string,
                        cacheType = CacheType.PlayList
                    };
                    string path = $"{CacheDir}/{fileInfo.GenerateName()}";
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    break;
                case CacheResponseType.SINGLE_CACHE:
                    CachePlayList(data_to_cache as Catalogue);
                    break;
                case CacheResponseType.BULK_RESTORE:
                    return RestoreAllPlaylistMetaData();
            }
            return null;
        }


        public void WriteToFile(byte[] cacheContent, string id)
        {
            CacheFileInfo cacheFileInfo = new CacheFileInfo();
            cacheFileInfo.cacheType = CacheType.PlayList;
            cacheFileInfo.id = id;
            Compression.Compress(cacheContent, $"{CacheDir}/{cacheFileInfo.GenerateName()}", false);
        }

        public void CachePlayList(Catalogue catalogue)
        {
            string content = PlayListSerializer.PlaylistSerializer(catalogue);
            WriteToFile(Encoding.UTF8.GetBytes(content), catalogue.Uid());
        }

        public List<CatalogueMetadata> RestoreAllPlaylistMetaData()
        {
            List<CatalogueMetadata> catalogues = new List<CatalogueMetadata>();
            foreach(string path in CacheUtils.FindAllCaches(CacheDir, CacheType.PlayList))
            {
                CacheFileInfo cfi = CacheUtils.ConvertToWWU(Path.GetFileNameWithoutExtension(path));
                string json = Encoding.UTF8.GetString(Compression.Decompress(path, false));
                CatalogueMetadata catalogue = PlayListSerializer.CatalogueDeserializer(json);
                catalogue.Uuid = cfi.id;
                catalogues.Add(catalogue);
            }
            return catalogues;
        }

        public void SetCacheDir(string BaseDir)
        {
            CacheDir = $"{BaseDir}/{cacheFolder}";
            if (!Directory.Exists(CacheDir))
            {
                Directory.CreateDirectory(CacheDir);
            }
        }
    }
}