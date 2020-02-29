using Antlr4.Runtime.Tree;
using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions.Runtime;
using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public abstract class LetterValue
    {
        ElementType elementType = ElementType.GAMMY;
        public TokenInfo ElementTokenInfo { get; private set; } = null;
        public LetterValue(ElementType elementType, TokenInfo ElementTokenInfo = null)
        {
            this.elementType = elementType;
            this.ElementTokenInfo = ElementTokenInfo;
        }

        public virtual void AddToElementList(LetterValue letterElement)
        {
            throw new NotImplementedException();
        }

        public virtual ElementType GetLetterElementType()
        {
            return elementType;
        }

        public void SetElementTokenInfo(TokenInfo tokenInfo)
        {
            if(ElementTokenInfo == null)
            {
                ElementTokenInfo = tokenInfo;
            }
        }

        public virtual LetterValue EvaluateWith(LetterValue operand, RelationType relationType)
        {
            throw new Exceptions.Runtime.RTInvalidOperationException();
        }

        public virtual void Evaluate()
        {
            throw new NotImplementedException();
        }

        public virtual T EvaluateAs<T>()
        {
            throw new NotImplementedException();
        }

        public virtual object EvaluateByType(Type type)
        {
            throw new NotImplementedException();
        }

        public virtual T getValueAt<T>(int i)
        {
            throw new NotImplementedException();
        }

        public virtual object getValueByTypeAt(int i,Type t)
        {
            throw new NotImplementedException();
        }

        public virtual bool EqualTo(LetterValue elementBase)
        {
            return false;
        }


        public static LetterValue CreateByValue<T>(T value)
        {
            return CreateLetterValue(value, typeof(T));
        }

        public static LetterValue CreateLetterValue(object value, Type t)
        {
            switch (Utils.GetLetterType(t))
            {
                case ElementType.NUMBER:
                    return new LetterNumber((double)Convert.ChangeType(value, TypeCode.Double));
                case ElementType.STRING:
                    return new LetterString((string)Convert.ChangeType(value, t));
                case ElementType.BOOL:
                    return new LetterBool((bool)Convert.ChangeType(value, TypeCode.Boolean));
            }
            if (t.IsArray)
            {
                Type array_element_type = t.GetElementType();
                Array array = (Array)value;
                LetterArrayList larray = new LetterArrayList();
                for (int i = 0; i < array.Length; i++)
                {
                    larray.AddToElementList(CreateLetterValue(array.GetValue(i), array_element_type));
                }
                return larray;
            }
            else
            {
                return new LetterUndefined();
            }
        }
    }
}
