using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public class LetterSuffixActions : LetterValue
    {
        List<SuffixAction> suffixActions = new List<SuffixAction>();

        public LetterSuffixActions() : base(ElementType.SUFX_ACT_COLLECTION)
        {

        }

        public void AddSuffixAction(SuffixAction suffixAction)
        {
            suffixActions.Add(suffixAction);
        }

        public List<SuffixAction> GetSuffixActions()
        {
            return suffixActions;
        }
    }

    public class SuffixAction : LetterValue
    {
        LetterActionType actionType;
        LetterValue action_parameter;
        public SuffixAction(LetterActionType actionType, LetterValue action_parameter) : base(ElementType.SUFX_ACT)
        {
            this.actionType = actionType;
            this.action_parameter = action_parameter;
        }

        public LetterActionType GetSuffixType { get => actionType; }
        public LetterValue GetSuffixActionParams { get => action_parameter; }
    }
}
