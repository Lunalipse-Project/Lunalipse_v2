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

        public static CacheHub Instance(string Basedir = "")
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

        private List<CacheFileInfo> CacheWraps;
        private Dictionary<CacheType, ICacheOperator> Operators;

        public string baseDir = "";

        public CacheHub(string dir)
        {
            Operators = new Dictionary<CacheType, ICacheOperator>();
            CacheWraps = new List<CacheFileInfo>();
            baseDir = dir == "" ? baseDir : dir;
            ReflushCaches();
        }

        /// <summary>
        /// 注册一个缓存生成器
        /// </summary>
        /// <param name="type">缓存类型</param>
        /// <param name="op">缓存生成器实例</param>
        /// <returns></returns>
        public bool RegisterOperator(CacheType type ,ICacheOperator op)
        {
            if (Operators.ContainsKey(type)) return false;
            op.SetCacheDir(baseDir);
            Operators.Add(type, op);
            return true;
        }

        /// <summary>
        /// 缓存一个对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="obj">对象的实例</param>
        /// <param name="type">要使用的缓存类型</param>
        /// <returns></returns>
        public bool CacheObject<T>(T obj, CacheType type, params object[] aux_data)
        {
            if (!Operators.ContainsKey(type)) return false;
            Operators[type].InvokeOperator(CacheResponseType.SINGLE_CACHE, obj, aux_data);
            return true;
        }

        /// <summary>
        /// 缓存一堆对象
        /// </summary>
        /// <typeparam name="T">这堆对象的类型</typeparam>
        /// <param name="obj">这堆对象的集合</param>
        /// <param name="type">要使用的缓存类型</param>
        /// <returns></returns>
        public bool CacheObjects<T>(List<T> obj, CacheType type, params object[] aux_data)
        {
            if (!Operators.ContainsKey(type)) return false;
            Operators[type].InvokeOperator(CacheResponseType.BULK_CACHE, obj, aux_data);
            return true;
        }

        /// <summary>
        /// 缓存一个字段
        /// </summary>
        /// <typeparam name="T">字段所属的类</typeparam>
        /// <param name="ancestor">字段所属的类的实例</param>
        /// <param name="type">要使用的缓存类型</param>
        /// <param name="FieldName">字段的名称</param>
        /// <returns></returns>
        public bool CacheField<T>(T ancestor, CacheType type, string FieldName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 查找缓存记录，并从一个缓存恢复一个实例对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="Conditions">匹配条件</param>
        /// <param name="type">缓存使用的缓存类型</param>
        /// <returns></returns>
        public T RestoreObject<T>(string id, CacheType type)
        {
            if (!Operators.ContainsKey(type)) return default(T);
            try
            {
                return (T)Operators[type].InvokeOperator(CacheResponseType.SINGLE_RESTORE, null,
                new CacheFileInfo()
                {
                    id = id,
                    cacheType = type
                });
            }
            catch(Exception ex)
            {
                LunalipseLogger.GetLogger().Error("Unable to perform cache restoring. ");
                LunalipseLogger.GetLogger().Error($"Error occurs: {ex.Message}");
                LunalipseLogger.GetLogger().Error($"Stack trace:\n {ex.StackTrace}");
                return default(T);
            }
        }

        /// <summary>
        /// 查找缓存记录，恢复同一缓存类别中的所有类型相同的对象
        /// </summary>
        /// <typeparam name="T">这多个对象的共同所属类型</typeparam>
        /// <param name="type">缓存使用的缓存类型</param>
        /// <returns></returns>
        public IEnumerable<T> RestoreObjects<T>(CacheType type)
        {
            if (!Operators.ContainsKey(type)) yield break;
            List<T> ts = (List<T>)Operators[type].InvokeOperator(CacheResponseType.BULK_RESTORE, null);
            foreach(T t in ts)
            {
                yield return t;
            }
        }

        /// <summary>
        /// 查找缓存记录，并从一个缓存恢复一个字段
        /// </summary>
        /// <param name="Conditions">匹配条件</param>
        /// <param name="type">缓存使用的缓存类型</param>
        /// <param name="FieldName">字段名称</param>
        /// <returns></returns>
        public object RestoreField(Func<WinterWrapUp, bool> Conditions, CacheType type, string FieldName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 刷新缓存记录
        /// </summary>
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

        /// <summary>
        /// 检测使用给定缓存类型的缓存是否存在
        /// </summary>
        /// <param name="ctype">使用的缓存类型</param>
        /// <param name="suffies">额外的附加信息</param>
        /// <returns></returns>
        public bool ComponentCacheExists(CacheType ctype, string id)
        {
            return (bool)Operators[ctype].InvokeOperator(CacheResponseType.CACHE_EXIST, null, id, ctype);
        }

        /// <summary>
        /// 删除所有<seealso cref="WinterWrapUp.deletable"/>为<see cref="false"/>的缓存
        /// </summary>
        public void DeleteCaches(CacheType cacheType)
        {
            Operators[cacheType].InvokeOperator(CacheResponseType.DELETE_ALL_CACHE, null, cacheType);
        }

        /// <summary>
        /// 删除所有<seealso cref="WinterWrapUp.deletable"/>为<see cref="false"/>且有<seealso cref="CacheType"/>条件约束的缓存
        /// </summary>
        /// <param name="FilterType">条件约束</param>
        /// <param name="forced">使用强制模式，在此模式下，无视<seealso cref="WinterWrapUp.deletable"/>约束</param>
        public void DeleteCache(CacheType cacheType, string id)
        {
            Operators[cacheType].InvokeOperator(CacheResponseType.DELETE_CACHE, null, id, cacheType);
        }
    }
}
