using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions.Runtime;
using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    class LetterIndexing : LetterValue
    {
        LetterValue identifier;
        LetterValue index;
        public LetterIndexing(LetterValue identifier, LetterValue index, TokenInfo tokenInfo) : base(ElementType.INDEXING, tokenInfo)
        {
            this.identifier = identifier;
            this.index = index;
        }

        public override T EvaluateAs<T>()
        {
            return (T)EvaluateByType(typeof(T));
        }

        public override object EvaluateByType(Type type)
        {
            ReslovePending();
            TypeCheck();
            return identifier.getValueByTypeAt(IndexCheck(), type);
        }

        public override LetterValue EvaluateWith(LetterValue operand, RelationType relationType)
        {
            ReslovePending();
            TypeCheck();
            return identifier.getValueAt<LetterValue>(IndexCheck()).EvaluateWith(operand, relationType);
        }

        private void TypeCheck()
        {
            ElementType type = identifier.GetLetterElementType();
            if (type == ElementType.VAR)
            {
                type = (identifier as LetterVariable).GetValueType();
                identifier = identifier.EvaluateAs<LetterValue>();
            }
            if (type != ElementType.ARRAY)
            {
                throw new RTInvalidOperationException("CORE_LBS_RT_INDEXING_NOT_SUPPORT", ElementTokenInfo);
            }
        }

        private int IndexCheck()
        {
            int i = index.EvaluateAs<int>();
            int size = (identifier as LetterArrayList).GetSize();
            if (i < 0)  // Support negative indexing
            {
                i = size + i;
            }
            if (i >= size || i < 0)
            {
                //TODO Using token info to decorate error msg
                throw new RTInvalidOperationException("CORE_LBS_RT_INDEX_OUT_OF_RANGE", ElementTokenInfo);
            }
            return i;
        }

        private void ReslovePending()
        {
            if(index.GetLetterElementType() == ElementType.PENDING)
            {
                index = (index as LetterPendingSymbol).ResolvePending();
            }
            if (identifier.GetLetterElementType() == ElementType.PENDING)
            {
                identifier = (identifier as LetterPendingSymbol).ResolvePending();
            }
        }
    }
}
