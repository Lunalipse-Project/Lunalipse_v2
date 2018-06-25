//using Lunalipse.Common.Generic.Cache;
using Lunalipse.Common.Generic.Cache;
using Lunalipse.Common.Interfaces.ICache;
using Lunalipse.Core.PlayList;
using Lunalipse.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lunalipse.Common.Generic.Cache.SerializeInfo;

namespace Lunalipse.Core.Cache
{
    public class MusicCacheIndexer: ICacheOperator
    {
        const string OP_UID = "ml";
        public bool UseLZ78Compress { get; set; }
        public string CacheDir { get; private set; }

        CacheSerializor caches;
        public MusicCacheIndexer()
        {
            caches = new CacheSerializor();
        }

        public void CacheMusicCatalogue(Catalogue cata)
        {
            WinterWrapUp cw = new WinterWrapUp()
            {
                createDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),
                deletable = false,
                markName = CacheUtils.GenerateMarkName(OP_UID, "ctlg"),
                uid = Guid.NewGuid().ToString()
            };
            byte[] cstring = Encoding.UTF8.GetBytes(caches.CacheTo(cata, cw));
            Compressed.writeCompressed(cstring, "{0}//{1}".FormateEx(CacheDir, cw.GenerateName()), UseLZ78Compress);
        }

        private Catalogue RestoreMusicCataloge(object obj)
        {
            return caches.RestoreTo<Catalogue>(obj);
        }

        public void CacheCatalogues(List<Catalogue> catas)
        {
            foreach(Catalogue c in catas)
            {
                CacheMusicCatalogue(c);
            }
        }

        public void CacheCataloguesObject(CataloguePool cpool)
        {
            foreach (Catalogue c in cpool.All)
            {
                CacheMusicCatalogue(c);
            }
        }

        public List<Catalogue> RestoreCatalogues(List<WinterWrapUp> wwu)
        {
            List<Catalogue> catas = new List<Catalogue>();
            foreach(WinterWrapUp cw in wwu)
            {
                catas.Add(
                    RestoreMusicCataloge(
                        JObject.Parse(
                            Compressed.readCompressed("{0}//{1}".FormateEx(CacheDir, cw.GenerateName()),UseLZ78Compress)
                        )["ctx"]
                    )
                );
            }
            return catas;
        }

        public Catalogue RestoreCatalogue(WinterWrapUp wwu)
        {
            return RestoreMusicCataloge(
                        JObject.Parse(
                            Compressed.readCompressed("{0}//{1}".FormateEx(CacheDir, wwu.GenerateName()), UseLZ78Compress)
                        )["ctx"]
                    );
        }

        public object InvokeOperator(CacheResponseType crt, params object[] args)
        {
            switch (crt)
            {
                case CacheResponseType.SINGLE_CACHE:
                    CacheCataloguesObject((CataloguePool)args[0]);
                    break;
                case CacheResponseType.BULK_CACHE:
                    CacheCatalogues((List<Catalogue>)args[0]);
                    break;
                case CacheResponseType.SINGLE_RESTORE:
                    return RestoreCatalogue((WinterWrapUp)args[0]);
                case CacheResponseType.BULK_RESTORE:
                    return RestoreCatalogues((List<WinterWrapUp>)args[0]);
            }
            return null;
        }

        public void SetCacheDir(string BaseDir)
        {
            CacheDir = BaseDir;
        }

        public string OperatorUID()
        {
            return OP_UID;
        }
    }
}
