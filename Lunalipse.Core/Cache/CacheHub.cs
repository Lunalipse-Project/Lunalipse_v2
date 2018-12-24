using Lunalipse.Common.Generic.Cache;
using Lunalipse.Common.Interfaces.ICache;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using static Lunalipse.Common.Generic.Cache.SerializeInfo;

namespace Lunalipse.Core.Cache
{
    public class CacheHub : ICacheHub
    {
        static volatile CacheHub CHH_INSTANCE;
        static readonly object CHH_LOCK = new object();

        public static CacheHub INSTANCE(string Basedir = "")
        {
            if (CHH_INSTANCE == null)
            {
                lock (CHH_LOCK)
                {
                    CHH_INSTANCE = CHH_INSTANCE ?? new CacheHub(Basedir);
                }
            }
            return CHH_INSTANCE;
        }

        private List<WinterWrapUp> CacheWraps;
        private Dictionary<CacheType, ICacheOperator> Operators;

        public string baseDir = "";

        public CacheHub(string dir)
        {
            Operators = new Dictionary<CacheType, ICacheOperator>();
            CacheWraps = new List<WinterWrapUp>();
            baseDir = dir == "" ? baseDir : dir;
            ReflushCaches();
        }

        public bool RegisterOperator(CacheType type ,ICacheOperator op)
        {
            if (Operators.ContainsKey(type)) return false;
            op.SetCacheDir(baseDir + "//mcdata");
            Operators.Add(type, op);
            return true;
        }

        public bool CacheObject<T>(T obj, CacheType type) where T : ICachable
        {
            Operators[type].InvokeOperator(CacheResponseType.SINGLE_CACHE, obj);
            return true;
        }

        public bool CacheObjects<T>(List<T> obj, CacheType type) where T : ICachable
        {
            Operators[type].InvokeOperator(CacheResponseType.BULK_CACHE, obj);
            return true;
        }

        public bool CacheField<T>(T ancestor, CacheType type, string FieldName)
        {
            Operators[type].InvokeOperator(CacheResponseType.FIELD_CACHE, ancestor, FieldName);
            return true;
        }

        public T RestoreObject<T>(Func<WinterWrapUp, bool> Conditions, CacheType type)
        {
            return (T)Operators[type].InvokeOperator(CacheResponseType.SINGLE_RESTORE,
                CacheWraps.Find(x => Conditions(x)));
        }

        public IEnumerable<T> RestoreObjects<T>(Func<WinterWrapUp, bool> Conditions, CacheType type)
        {
            List<T> returns = (List<T>)Operators[type]
                .InvokeOperator(CacheResponseType.BULK_RESTORE, 
                    CacheWraps.FindAll(
                        x => Conditions(x)
                    )
                );
            foreach (object o in returns)
            {
                yield return (T)o;
            }
        }

        public object RestoreField<T>(Func<WinterWrapUp, bool> Conditions, CacheType type, string FieldName)
        {
            return Operators[type].InvokeOperator(CacheResponseType.FIELD_RESTORE,
                CacheWraps.Find(x => Conditions(x)));
        }

        private void ReflushCaches()
        {
            CacheWraps.Clear();
            if(!Directory.Exists(baseDir + "//mcdata"))
            {
                Directory.CreateDirectory(baseDir + "//mcdata");
                return;
            }
            foreach (string path in Directory.GetFiles(baseDir + "//mcdata"))
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                CacheWraps.Add(CacheUtils.ConvertToWWU(fileName));
            }
        }

        public bool ComponentCacheExists(CacheType ctype,params string[] suffies)
        {
            if (!Operators.ContainsKey(ctype)) return false;
            string markName = CacheUtils.GenerateMarkName(Operators[ctype].OperatorUID(), suffies);
            return CacheWraps.FindIndex(x => x.markName.StartsWith(markName)) != -1;
        }

        public void DeleteCaches(bool forced = false)
        {
            foreach (WinterWrapUp WWU in CacheWraps.FindAll(x => forced ? true : !x.deletable))
            {
                File.Delete("{0}//mcdata//{1}".FormateEx(baseDir, CacheUtils.GenerateName(WWU)));
            }
            CacheWraps.RemoveAll(x => forced ? true : !x.deletable);
        }

        
    }
}
