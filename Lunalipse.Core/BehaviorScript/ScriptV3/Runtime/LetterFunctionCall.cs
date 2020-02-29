using System;
using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions;
using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public class LetterFunctionCall:LetterValue, ISuffixable
    {
        LetterArrayList Parameters;
        LetterStarSwirlSpell ActualSpell;
        string identifier;
        LetterSuffixActions suffixActions = null;
        public LetterFunctionCall(string functionName, LetterStarSwirlSpell ActualSpell, TokenInfo tokenInfo) : base(ElementType.FUNCTION_CALL, tokenInfo)
        {
            identifier = functionName;
            this.ActualSpell = ActualSpell;
        }

        public void SetParameter(LetterArrayList Parameters)
        {
            this.Parameters = Parameters;
        }

        public void ValidateParameter()
        {
            int reuslt = ActualSpell.parameterCheck(Parameters);
            if (reuslt != 0)
            {
                throw new UnmatchedParamter(ActualSpell.ParameterNumber, ParameterSize(), ElementTokenInfo,
                    "CORE_LBS_SE_PARAMETER_MISMATCH_" + (reuslt > 0 ? "M" : "L"));
            }
        }

        public int ParameterSize()
        {
            return Parameters == null ? 0 : Parameters.GetSize();
        }

        public void SetSuffixActions(LetterSuffixActions suffixActions)
        {
            this.suffixActions = suffixActions;
        }

        public LetterSuffixActions GetSuffixActions()
        {
            return suffixActions ?? new LetterSuffixActions();
        }

        public override void Evaluate()
        {
            ActualSpell.CastTheSpell(Parameters);
        }

        public override LetterValue EvaluateWith(LetterValue operand, RelationType relationType)
        {
            return ActualSpell.CastTheSpell(Parameters).EvaluateWith(operand, relationType);
        }

        public override T EvaluateAs<T>()
        {
            return ActualSpell.CastTheSpell(Parameters).EvaluateAs<T>();
        }

        public override object EvaluateByType(Type type)
        {
            return ActualSpell.CastTheSpell(Parameters).EvaluateByType(type);
        }

    }
}
