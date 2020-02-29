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
            TypeCheck();
            return identifier.getValueAt<T>(IndexCheck());
        }

        public override LetterValue EvaluateWith(LetterValue operand, RelationType relationType)
        {
            TypeCheck();
            return identifier.getValueAt<LetterValue>(IndexCheck()).EvaluateWith(operand, relationType);
        }

        private void TypeCheck()
        {
            if (identifier.GetLetterElementType() != ElementType.ARRAY)
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
    }
}
