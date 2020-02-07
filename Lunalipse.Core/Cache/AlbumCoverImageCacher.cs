using Lunalipse.Common.Generic.Cache;
using Lunalipse.Common.Interfaces.ICache;
using Lunalipse.Utilities;
using System;
using System.IO;
using static Lunalipse.Common.Generic.Cache.SerializeInfo;

namespace Lunalipse.Core.Cache
{
    public class AlbumCoverImageCacher: ICacheOperator
    {
        public bool UseLZ78Compress { get; set; }
        public CacheType Responsiblity { get; set; }
        public string CacheDir { get; private set; }

        const string cacheFolder = "mcdata";
        CacheSerializor caches;
        public AlbumCoverImageCacher()
        {
            caches = new CacheSerializor();
            
        }

        public void WriteToFile(byte[] cacheContent, CacheType cacheType, string id)
        {
            CacheFileInfo cacheFileInfo = new CacheFileInfo();
            cacheFileInfo.cacheType = cacheType;
            cacheFileInfo.id = id;
            Compression.Compress(cacheContent, $"{CacheDir}/{cacheFileInfo.GenerateName()}", UseLZ78Compress);
        }

        public byte[] ReadCache(CacheFileInfo cacheFileInfo)
        {
            WinterWrapUp winterWrapUp;
            byte[] content = Compression.Decompress($"{CacheDir}/{cacheFileInfo.GenerateName()}", UseLZ78Compress);
            return caches.BinRestoreTo<byte[]>(content, out winterWrapUp);
        }

        public void CacheAlbumCoverImg(string id, byte[] image)
        {
            WinterWrapUp winterWrapUp = new WinterWrapUp();
            winterWrapUp.createDate = (uint)DateTime.Now.ToLunalipseTimeStamp();
            WriteToFile(caches.CacheToBin(image, winterWrapUp), Responsiblity, id);
        }

        public byte[] RestoreCatalogue(CacheFileInfo wwu)
        {
            return ReadCache(wwu);
        }

        // This should get two args to cache a album cover image:
        //          1. UID of MusicEntity in string!
        //          2. byte array of image
        public object InvokeOperator(CacheResponseType crt, object data_to_cache, params object[] args)
        {
            switch (crt)
            {
                case CacheResponseType.SINGLE_CACHE:
                    CacheAlbumCoverImg(args[0] as string, data_to_cache as byte[]);
                    break;
                case CacheResponseType.SINGLE_RESTORE:
                    return RestoreCatalogue((CacheFileInfo)args[0]);
                case CacheResponseType.CACHE_EXIST:
                    return checkCacheExist(args[0] as string, (CacheType)args[1]);
                case CacheResponseType.DELETE_ALL_CACHE:
                    foreach(string path in CacheUtils.ListAllCaches(CacheDir, Responsiblity))
                    {
                        File.Delete(path);
                    }
                    break;
                case CacheResponseType.DELETE_CACHE:
                    removeCache(args[0] as string, (CacheType)args[1]);
                    break;
            }
            return null;
        }

        public bool checkCacheExist(string id, CacheType cacheType)
        {
            CacheFileInfo cacheFileInfo = new CacheFileInfo()
            {
                id = id,
                cacheType = cacheType
            };
            return File.Exists($"{CacheDir}/{cacheFileInfo.GenerateName()}");
        }

        public void removeCache(string id, CacheType cacheType)
        {
            CacheFileInfo cacheFileInfo = new CacheFileInfo()
            {
                id = id,
                cacheType = cacheType
            };
            string path = $"{CacheDir}/{cacheFileInfo.GenerateName()}";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
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
