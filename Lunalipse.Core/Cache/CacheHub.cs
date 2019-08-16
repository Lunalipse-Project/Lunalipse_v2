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

        /// <summary>
        /// 注册一个缓存生成器
        /// </summary>
        /// <param name="type">缓存类型</param>
        /// <param name="op">缓存生成器实例</param>
        /// <returns></returns>
        public bool RegisterOperator(CacheType type ,ICacheOperator op)
        {
            if (Operators.ContainsKey(type)) return false;
            op.SetCacheDir(baseDir + "//mcdata");
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
        public bool CacheObject<T>(T obj, CacheType type) where T : ICachable
        {
            Operators[type].InvokeOperator(CacheResponseType.SINGLE_CACHE, obj);
            return true;
        }

        /// <summary>
        /// 缓存一堆对象
        /// </summary>
        /// <typeparam name="T">这堆对象的类型</typeparam>
        /// <param name="obj">这堆对象的集合</param>
        /// <param name="type">要使用的缓存类型</param>
        /// <returns></returns>
        public bool CacheObjects<T>(List<T> obj, CacheType type) where T : ICachable
        {
            //string markName = CacheUtils.GenerateMarkName(Operators[type].OperatorUID());
            //List<WinterWrapUp> ExistedCache = CacheWraps.FindAll(x => x.markName == markName);
            Operators[type].InvokeOperator(CacheResponseType.BULK_CACHE, obj);
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
            Operators[type].InvokeOperator(CacheResponseType.FIELD_CACHE, ancestor, FieldName);
            return true;
        }

        /// <summary>
        /// 查找缓存记录，并从一个缓存恢复一个实例对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="Conditions">匹配条件</param>
        /// <param name="type">缓存使用的缓存类型</param>
        /// <returns></returns>
        public T RestoreObject<T>(Func<WinterWrapUp, bool> Conditions, CacheType type)
        {
            return (T)Operators[type].InvokeOperator(CacheResponseType.SINGLE_RESTORE,
                CacheWraps.Find(x => Conditions(x)));
        }

        /// <summary>
        /// 查找缓存记录，并从多个个缓存恢复多个实例对象
        /// </summary>
        /// <typeparam name="T">这多个对象的共同所属类型</typeparam>
        /// <param name="Conditions">匹配条件</param>
        /// <param name="type">缓存使用的缓存类型</param>
        /// <returns></returns>
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

        /// <summary>
        /// 查找缓存记录，并从一个缓存恢复一个字段
        /// </summary>
        /// <param name="Conditions">匹配条件</param>
        /// <param name="type">缓存使用的缓存类型</param>
        /// <param name="FieldName">字段名称</param>
        /// <returns></returns>
        public object RestoreField(Func<WinterWrapUp, bool> Conditions, CacheType type, string FieldName)
        {
            return Operators[type].InvokeOperator(CacheResponseType.FIELD_RESTORE,
                CacheWraps.Find(x => Conditions(x)));
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
        public bool ComponentCacheExists(CacheType ctype,params string[] suffies)
        {
            if (!Operators.ContainsKey(ctype)) return false;
            string markName = CacheUtils.GenerateMarkName(Operators[ctype].OperatorUID(), suffies);
            return CacheWraps.FindIndex(x => x.markName.StartsWith(markName)) != -1;
        }

        /// <summary>
        /// 删除所有<seealso cref="WinterWrapUp.deletable"/>为<see cref="false"/>的缓存
        /// </summary>
        /// <param name="forced">使用强制模式，在此模式下，无视<seealso cref="WinterWrapUp.deletable"/>约束</param>
        public void DeleteCaches(bool forced = false)
        {
            foreach (WinterWrapUp WWU in CacheWraps.FindAll(x => forced ? true : !x.deletable))
            {
                File.Delete("{0}//mcdata//{1}".FormateEx(baseDir, CacheUtils.GenerateName(WWU)));
            }
            CacheWraps.RemoveAll(x => forced ? true : !x.deletable);
        }

        /// <summary>
        /// 删除所有<seealso cref="WinterWrapUp.deletable"/>为<see cref="false"/>且有<seealso cref="CacheType"/>条件约束的缓存
        /// </summary>
        /// <param name="FilterType">条件约束</param>
        /// <param name="forced">使用强制模式，在此模式下，无视<seealso cref="WinterWrapUp.deletable"/>约束</param>
        public void DeleteCaches(CacheType FilterType,bool forced = false)
        {
            string markName = CacheUtils.GenerateMarkName(Operators[FilterType].OperatorUID());
            foreach (WinterWrapUp WWU in CacheWraps.FindAll(x => forced ? true : !x.deletable || x.markName == markName)) 
            {
                File.Delete("{0}//mcdata//{1}".FormateEx(baseDir, CacheUtils.GenerateName(WWU)));
            }
            CacheWraps.RemoveAll(x => forced ? true : !x.deletable);
        }
        
    }
}
