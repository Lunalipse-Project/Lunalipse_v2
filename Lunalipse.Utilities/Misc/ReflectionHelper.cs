using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Utilities.Misc
{
    public class ReflectionHelper
    {
        public static string GetFormatedCommandList(Type type, bool showNonPublic = false, Type attrFilter = null)
        {
            string str = "";
            foreach (MethodInfo mi in type.GetMethods())
            {
                if(!mi.IsPublic && !showNonPublic)
                {
                    continue;
                }
                if(attrFilter != null)
                {
                    if (mi.GetCustomAttribute(attrFilter) == null)
                    {
                        continue;
                    }
                }
                str += "\t{0} {1}({2})\n".FormateEx(mi.ReturnType.Name, mi.Name, GetFormatedParameter(mi.GetParameters()));
            }
            return str;
        }

        public static string GetFormatedParameter(ParameterInfo[] parameterInfos)
        {
            string paras = "";
            foreach(ParameterInfo parameter in parameterInfos)
            {
                paras += "{0} {1}, ".FormateEx(parameter.ParameterType.Name, parameter.Name);
            }
            return paras.Length == 0 ? paras : paras.Substring(0, paras.Length - 2);
        }

        public static object[] StringArrToParamters(ParameterInfo[] parameterInfos, string[] strargs)
        {
            if (strargs.Length != parameterInfos.Length)
            {
                return null;
            }
            List<object> arglist = new List<object>();
            try
            {
                for (int i=0;i<strargs.Length;i++)
                {
                    arglist.Add(Convert.ChangeType(strargs[i], parameterInfos[i].ParameterType));
                }
                return arglist.ToArray();
            }
            catch
            {
                return null;
            }
        }

        private static Tuple<bool,object,Type> __InvokeMethod(object ctx, object[] parameters, MethodInfo methodInfo)
        {
            if (methodInfo != null)
            {
                object obj = methodInfo.Invoke(ctx, parameters);
                return new Tuple<bool, object, Type>(true, obj, methodInfo.ReturnType);
            }
            return new Tuple<bool, object, Type>(false, null, null);
        }
        public static Tuple<bool, object, Type> InvokeMethod(Type classobj, object ctx, string name, string[] parameters)
        {
            MethodInfo methodInfo = classobj.GetMethod(name);
            return __InvokeMethod(ctx, methodInfo == null ? null : StringArrToParamters(methodInfo.GetParameters(), parameters), methodInfo);
        }
        public static Tuple<bool, object, Type> InvokeMethod(Type classobj, object ctx, string name, object[] parameters)
        {
            MethodInfo methodInfo = classobj.GetMethod(name);
            return __InvokeMethod(ctx, parameters, methodInfo);
        }
    }
}
