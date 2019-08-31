using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV2
{
    public static class LexicalUtils
    {
        public static ReturnTypes getType(ref string value)
        {
            ReturnTypes returnTypes;
            if (Regex.IsMatch(value, "(?<!\\\")[+-]?(\\d*[.])\\d+(?!\\\")"))
            {
                returnTypes = ReturnTypes.Double;
            }
            else if (Regex.IsMatch(value, "(?<!\\\")[+-]?\\d+(?!\\\")"))
            {
                returnTypes = ReturnTypes.Int;
            }
            else if (Regex.IsMatch(value, "\\\".*\\\""))
            {
                returnTypes = ReturnTypes.String;
                value = value.Trim('"');
            }
            else
            {
                returnTypes = ReturnTypes.UNCERTAIN;
            }
            if (Regex.IsMatch(value, @"\[.*\]"))
            {
                returnTypes |= ReturnTypes.ARRAY;
            }
            return returnTypes;
        }

        public static object ConvertTo(ReturnTypes returnTypes, string value)
        {
            if (returnTypes != ReturnTypes.UNCERTAIN && returnTypes != ReturnTypes.Void)
            {
                Type objectType = GetDotNetType(returnTypes & (~ReturnTypes.ARRAY));
                if ((returnTypes & ReturnTypes.ARRAY) == ReturnTypes.ARRAY)
                {
                    string[] array = value.Trim('[', ']').Split('|');
                    Array objs = Array.CreateInstance(objectType, array.Length);
                    for (int i = 0; i < array.Length; i++)
                    {
                        object element = Convert.ChangeType(array[i], objectType, CultureInfo.InvariantCulture);
                        objs.SetValue(element, i);
                    }
                    return objs;
                }
                return Convert.ChangeType(value, objectType, CultureInfo.InvariantCulture);
            }
            else
            {
                return null;
            }
        }

        public static string ToExpressionString(this Function function)
        {
            string func;
            if (!function.isFunction)
            {
                Type type = GetDotNetType(function.data_type & (~ReturnTypes.ARRAY));
                if((function.data_type & ReturnTypes.ARRAY) == ReturnTypes.ARRAY)
                {
                    string temp = "";
                    Array array = (Array)function.p_value;
                    for(int i = 0; i < array.Length; i++)
                    {
                        temp += Convert.ChangeType(array.GetValue(i), type).ToString() + "|";
                    }
                    temp = temp.Remove(temp.Length - 1, 1);
                    func = "[" + temp + "]";
                }
                else
                {
                    func = Convert.ChangeType(function.p_value, type).ToString();
                }
            }
            else
            {
                func = function.functionName + "(";
                foreach(Parameter parameter in function.paras)
                {
                    func += ToExpressionString((Function)parameter) + ", ";
                }
                if (function.paras.Count > 0)
                {
                    func = func.Remove(func.Length - 2, 2);
                }
                func += ")";
            }
            return func;
        }
        public static string ToParsedFuncString(this Function function)
        {
            string func = function.functionName + "(";
            foreach(Parameter parameter in function.paras)
            {
                Type type = GetDotNetType(parameter.data_type & (~ReturnTypes.ARRAY));
                if ((parameter.data_type & ReturnTypes.ARRAY) == ReturnTypes.ARRAY)
                {
                    string temp = "";
                    Array array = (Array)parameter.p_value;
                    for (int i = 0; i < array.Length; i++)
                    {
                        temp += Convert.ChangeType(array.GetValue(i), type).ToString() + ",";
                    }
                    temp = temp.Remove(temp.Length - 1, 1);
                    func += string.Format("new {0}[{1}]{{ {2} }}", type.Name, array.Length, temp);
                }
                else
                {
                    if(((Function)parameter).functionType == FunctionType.PLACEHOLDER)
                    {
                        func += "[Default]";
                    }
                    else
                    {
                        func += Convert.ChangeType(parameter.p_value, type).ToString();
                    }
                }
                func += ", ";
            }
            if (function.paras.Count > 0)
            {
                func = func.Remove(func.Length - 2, 2);
            }
            func += ")";
            return func;
        }

        public static Type GetDotNetType(ReturnTypes returnTypes)
        {
            switch (returnTypes)
            {
                case ReturnTypes.Double:
                    return typeof(double);
                case ReturnTypes.Int:
                    return typeof(int);
                case ReturnTypes.String:
                    return typeof(string);
                default:
                    return null;
            }
        }
    }
}
