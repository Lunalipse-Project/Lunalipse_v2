using Lunalipse.Common.Generic.Cache;
using Lunalipse.Common.Interfaces.ICache;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lunalipse.Common.Generic.Cache.SerializeInfo;

namespace Lunalipse.Core.Cache
{
    public class WebAudioFileCacher : ICacheOperator
    {
        public bool UseLZ78Compress { get; set; }
        public string CacheDir { get; private set; }

        const string cacheFolder = "mcdata/wm_cache";
        CacheSerializor caches;
        public WebAudioFileCacher()
        {
            caches = new CacheSerializor();
        }
        public object InvokeOperator(CacheResponseType crt, object data_to_cache, params object[] args)
        {
            switch(crt)
            {
                case CacheResponseType.CACHE_EXIST:
                    return checkCacheExist(args[0] as string, CacheType.WebAudioStuff);
                case CacheResponseType.SINGLE_CACHE:

                    // Aux data: args[0] is id of the audio
                    // data_to_cache is the bytes of audio data
                    CacheAudioStuff((WebAudioStuffs)data_to_cache, args[0] as string);

                    break;
                case CacheResponseType.DELETE_ALL_CACHE:
                    foreach (string path in CacheUtils.ListAllCaches(CacheDir, CacheType.WebAudioStuff))
                    {
                        File.Delete(path);
                    }
                    break;
                case CacheResponseType.DELETE_CACHE:
                    removeCache(args[0] as string, CacheType.WebAudioStuff);
                    break;
                case CacheResponseType.SINGLE_RESTORE:
                    return RestoreWebAudioStuff((CacheFileInfo)args[0]);
            }
            return null;
        }

        private void CacheAudioStuff(WebAudioStuffs stuff, string id)
        {
            byte[] lyric = Encoding.UTF8.GetBytes(stuff.lyric);
            byte[] url = Encoding.ASCII.GetBytes(stuff.downloadURL);
            byte[] file_type = Encoding.ASCII.GetBytes(stuff.fileType);
            WinterWrapUp winterWrapUp = new WinterWrapUp();
            winterWrapUp.createDate = (uint)DateTime.Now.ToLunalipseTimeStamp();
            winterWrapUp.num_of_sealed = 4;

            // audio data is right after the lyric. So offset is the length of lyric
            winterWrapUp.offsets = new int[4];
            winterWrapUp.offsets[0] = 0;                                                        // Lyric start
            winterWrapUp.offsets[1] = lyric.Length;                                             // Audio data start
            winterWrapUp.offsets[2] = winterWrapUp.offsets[1] + stuff.audioData.Length;         // download url start
            winterWrapUp.offsets[3] = winterWrapUp.offsets[2] + url.Length;                     // file type start

            // Compose the WebAudioStuffs into array
            byte[] content = new byte[winterWrapUp.offsets[3] + file_type.Length];
            Array.Copy(lyric, 0, content, winterWrapUp.offsets[0], lyric.Length);
            Array.Copy(stuff.audioData, 0, content, winterWrapUp.offsets[1], stuff.audioData.Length);
            Array.Copy(url, 0, content, winterWrapUp.offsets[2], url.Length);
            Array.Copy(file_type, 0, content, winterWrapUp.offsets[3], file_type.Length);

            WriteToFile(caches.CachePureByteArray(content, winterWrapUp), CacheType.WebAudioStuff, id);
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

        private void WriteToFile(byte[] cacheContent, CacheType cacheType, string id)
        {
            CacheFileInfo cacheFileInfo = new CacheFileInfo();
            cacheFileInfo.cacheType = cacheType;
            cacheFileInfo.id = id;
            Compression.Compress(cacheContent, $"{CacheDir}/{cacheFileInfo.GenerateName()}", UseLZ78Compress);
        }

        private WebAudioStuffs RestoreWebAudioStuff(CacheFileInfo cacheFileInfo)
        {
            WinterWrapUp winterWrapUp;
            byte[] content = Compression.Decompress($"{CacheDir}/{cacheFileInfo.GenerateName()}", UseLZ78Compress);
            byte[] raw = caches.GetByteArrayContent(content, out winterWrapUp);
            WebAudioStuffs stuff = new WebAudioStuffs();
            int audio_offset = winterWrapUp.offsets[1];
            int url_offset = winterWrapUp.offsets[2];
            int ftype_offset = winterWrapUp.offsets[3];

            // That's not our WinterWrapUp
            if (winterWrapUp.num_of_sealed != 4) return null;

            //lyric.
            byte[] str_data = new byte[audio_offset];
            Array.Copy(raw, 0, str_data, 0, str_data.Length);
            stuff.lyric = Encoding.UTF8.GetString(str_data);

            //audio
            stuff.audioData = new byte[url_offset - audio_offset];
            Array.Copy(raw, audio_offset, stuff.audioData, 0, stuff.audioData.Length);

            //url
            str_data = new byte[ftype_offset - url_offset];
            Array.Copy(raw, url_offset, str_data, 0, str_data.Length);
            stuff.downloadURL = Encoding.ASCII.GetString(str_data);

            //file type
            str_data = new byte[raw.Length - ftype_offset];
            Array.Copy(raw, ftype_offset, str_data, 0, str_data.Length);
            stuff.fileType = Encoding.ASCII.GetString(str_data);

            return stuff;
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
