using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions.Runtime;
using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public class LetterArrayList : LetterValue, IEnumerable<LetterValue>, ICloneable
    {
        List<LetterValue> arrayContent;
        public LetterArrayList(LetterValue firstElement = null) : base(ElementType.ARRAY)
        {
            arrayContent = new List<LetterValue>();
            if (firstElement != null)
            {
                arrayContent.Add(firstElement);
            }
        }

        public int GetSize()
        {
            return arrayContent.Count;
        }

        public override void AddToElementList(LetterValue letterElement)
        {
            if(letterElement!=null)
            {
                arrayContent.Add(letterElement);
            }
        }

        public List<LetterValue> getContent()
        {
            return arrayContent;
        }

        public override bool Equals(object elementBase)
        {
            if (elementBase is LetterArrayList)
            {
                LetterArrayList array = elementBase as LetterArrayList;
                foreach (LetterValue e in array)
                {
                    if(!arrayContent.Contains(e))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public override T EvaluateAs<T>()
        {
            Type t = typeof(T);
            return (T)EvaluateByType(t);
        }

        public override LetterValue EvaluateWith(LetterValue operand, RelationType relationType)
        {
            if (relationType == RelationType.ADD)
            {
                if (operand is LetterArrayList)
                {
                    LetterArrayList array = new LetterArrayList();
                    LetterArrayList op = operand as LetterArrayList;
                    array.arrayContent.AddRange(arrayContent);
                    array.arrayContent.AddRange(op.arrayContent);
                    return array;
                }
                else if (operand is LetterValue)
                {
                    arrayContent.Add(operand);
                    return this;
                }
            }
            throw new RTInvalidOperationException();
        }

        public override object EvaluateByType(Type type)
        {
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                Array array = Array.CreateInstance(elementType, arrayContent.Count);
                for (int i = 0; i < arrayContent.Count; i++)
                {
                    array.SetValue(Convert.ChangeType(arrayContent[i].EvaluateAs<object>(), elementType), i);
                }
                return Convert.ChangeType(array, type);
            }
            if (type == GetType() || type == typeof(LetterValue))
            {
                return Clone();
            }
            throw new InvalidCastException();
        }

        public override T getValueAt<T>(int i)
        {
            ResolvePendingAt(i);
            return (T)getValueByTypeAt(i, typeof(T));
        }

        public override object getValueByTypeAt(int i, Type t)
        {
            if (i >= arrayContent.Count)
            {
                return null;
            }
            ResolvePendingAt(i);
            return arrayContent[i].EvaluateByType(t);
        }

        public IEnumerator<LetterValue> GetEnumerator()
        {
            return arrayContent.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return arrayContent.GetEnumerator();
        }

        public object Clone()
        {
            LetterArrayList array = new LetterArrayList();
            array.arrayContent.AddRange(arrayContent);
            return array;
        }

        private void ResolvePendingAt(int i)
        {
            if (arrayContent[i].GetLetterElementType() == ElementType.PENDING)
            {
                LetterValue value = (arrayContent[i] as LetterPendingSymbol).ResolvePending();
                arrayContent[i] = value;
            }
        }
    }
}
