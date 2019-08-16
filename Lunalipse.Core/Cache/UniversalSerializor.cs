using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.Cache
{
    public class UniversalSerializor<IFilter,IInterface> where IFilter:Attribute
    {
        string interfaceName;
        const BindingFlags FieldFilter = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public UniversalSerializor()
        {
            interfaceName = typeof(IInterface).Name;
        }
        private List<FieldInfo> GetCachableFields(Type type)
        {
            List<FieldInfo> vars = new List<FieldInfo>();
            foreach (FieldInfo fi in type.GetFields(FieldFilter))
            {
                if (fi.GetCustomAttribute<IFilter>() == null)
                {
                    vars.Add(fi);
                }
            }
            return vars;
        }

        private List<object> getListItem(FieldInfo list, object father)
        {
            List<object> items = new List<object>();
            object value = list.GetValue(father);
            int count = (int)list.FieldType.GetProperty("Count").GetValue(value);
            for (int i = 0; i < count; i++)
            {
                object[] index = new object[] { i };
                object item = list.FieldType.GetProperty("Item").GetValue(value, index);
                items.Add(item);
            }
            return items;
        }

        private IDictionary GetDictionary(FieldInfo dictionary, object father)
        {
            return dictionary.GetValue(father) as IDictionary;
        }

        private IEnumerable<object> GetDictionaryKeys(IDictionary dic)
        {
            foreach (object obj in dic.Keys) yield return obj;
        }

        private JProperty WriteToNode(FieldInfo variable, object major)
        {
            return new JProperty(variable.Name, variable.GetValue(major));
        }

        public JObject WriteNested(object obj)
        {
            Type type = obj.GetType();
            JObject level = new JObject();
            if (type.GetInterface(interfaceName) == null) return level;
            foreach (FieldInfo fi in GetCachableFields(type))
            {
                encapsulateField(ref level, fi, obj);
            }
            return level;
        }

        public string WriteSingleField(object ancestor, string FieldName)
        {
            JObject jObject = new JObject();
            FieldInfo field = ancestor.GetType().GetField(FieldName, FieldFilter);
            if (field == null) return jObject.ToString();
            if (field.GetCustomAttribute<IFilter>() == null) return jObject.ToString();
            encapsulateField(ref jObject, field, ancestor);
            return jObject.ToString();
        }

        private void encapsulateField(ref JObject level, FieldInfo fi, object ancestor)
        {
            if (fi.FieldType.IsGenericType)
            {
                JArray ja = new JArray();
                if (fi.FieldType.GetGenericTypeDefinition().Name.Equals("List`1"))
                {
                    foreach (object t in getListItem(fi, ancestor))
                    {
                        Type ty = t.GetType();
                        if (ty.IsNonValueType())
                            ja.Add(WriteNested(t));
                        else
                            ja.Add(t);
                    }
                }
                else if (fi.FieldType.GetGenericTypeDefinition().Name.Equals("Dictionary`2"))
                {
                    IDictionary dic = GetDictionary(fi, ancestor);
                    foreach (object key in GetDictionaryKeys(dic))
                    {
                        JObject jo = new JObject();
                        object value = dic[key];
                        Type tkeys = key.GetType();
                        Type tvalue = value.GetType();
                        if (tkeys.IsNonValueType())
                            jo["DKey"] = WriteNested(key);
                        else
                            jo.Add(new JProperty("DKey", key));
                        if (tvalue.IsNonValueType())
                            jo["DVal"] = WriteNested(value);
                        else
                            jo.Add(new JProperty("DValue", value));
                        ja.Add(jo);
                    }
                }
                level.Add(fi.Name, ja);
            }
            else if (fi.FieldType.IsArray)
            {
                Array arr = (Array)fi.GetValue(ancestor);
                JArray ja = new JArray();
                for (int i = 0; i < arr.Length; i++)
                {
                    ja.Add(arr.GetValue(i));
                }
                level.Add(fi.Name, ja);
            }
            else if (!fi.FieldType.IsValueType && !fi.FieldType.Equals(typeof(String)))
                level.Add(new JProperty(fi.Name, WriteNested(fi.FieldType)));
            else
                level.Add(WriteToNode(fi, ancestor));
        }



        public object ReadNested(Type insType, JObject layer)
        {
            object instance = insType.GetConstructors(FieldFilter)[0].Invoke(null);
            foreach (JProperty jp in layer.Properties())
            {
                FieldInfo fi = insType.GetField(jp.Name, FieldFilter);
                fi.SetValue(instance, decapuslateField(jp.Value, insType, fi.FieldType));
            }
            return instance;
        }

        public object ReadSingleField(JObject layer, string fieldName,object ancestor)
        {
            Type ancestorType = ancestor.GetType();
            FieldInfo fi = ancestorType.GetField(fieldName, FieldFilter);
            return decapuslateField(layer[fieldName], ancestorType, fi.FieldType);
        }

        private object decapuslateField(JToken jp, Type insType, Type FieldType)
        {
            if (jp.Type == JTokenType.Array)
            {
                Type vartype = FieldType;
                if (!vartype.GetGenericTypeDefinition().Name.Equals("Dictionary`2"))
                {
                    ArrayList varo = null;
                    IList list = null;
                    bool isGeneric = vartype.IsGenericType && (vartype.GetGenericArguments()[0].IsCachable(interfaceName) || vartype.GetGenericArguments()[0].Equals(typeof(String)));
                    Type elementType = isGeneric ? vartype.GetGenericArguments()[0] : vartype.GetElementType();
                    if (!isGeneric) varo = new ArrayList();
                    else list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                    foreach (JToken jo in jp.Children())
                    {
                        if (isGeneric)
                        {
                            if(!elementType.IsNonValueType())
                            {
                                list.Add(AppliedValue(jo));
                            }
                            else
                            {
                                list.Add(ReadNested(elementType, (JObject)jo));
                            }
                        }
                        else
                        {
                            varo.Add(AppliedValue(jo));
                        }
                    }
                    if (vartype.IsArray)
                        return varo.ToArray(elementType);
                    else
                        return list;
                }
                else
                {
                    Type[] targs = vartype.GetGenericArguments();   //n=0:Key ; n=1:Value
                    bool KeyGeneric = targs[0].IsCachable(interfaceName);
                    bool ValGeneric = targs[1].IsCachable(interfaceName);
                    IDictionary dictionary = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(targs));
                    foreach (JToken jt in jp.Children())
                    {
                        dictionary.Add(KeyGeneric ? ReadNested(targs[0], jt["DKey"] as JObject) : AppliedValue(jt["DKey"]),
                                       ValGeneric ? ReadNested(targs[1], jt["DVal"] as JObject) : AppliedValue(jt["DVal"]));
                    }
                    return dictionary;
                }
            }
            else if (!FieldType.IsValueType && !FieldType.Equals(typeof(String)))
            {
                return ReadNested(FieldType, (JObject)jp);
            }
            else
            {
                return AppliedValue(jp);
            }
        }

        private object AppliedValue(JToken jp)
        {
            switch (jp.Type)
            {
                case JTokenType.String:
                    return jp.Value<string>();
                case JTokenType.Integer:
                    return jp.Value<int>();
                case JTokenType.Boolean:
                    return jp.Value<bool>();
                case JTokenType.Float:
                    return jp.Value<float>();
                case JTokenType.Date:
                    return jp.Value<DateTime>();
                case JTokenType.Null:
                    return null;
                default:
                    return null;
            }
        }
    }
}
