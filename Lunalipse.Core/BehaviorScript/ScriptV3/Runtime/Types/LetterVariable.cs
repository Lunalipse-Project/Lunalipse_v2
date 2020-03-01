using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions.Runtime;
using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public class LetterVariable : LetterValue
    {
        string id;
        LetterValue id_value = new LetterUndefined();
        public bool isReadOnly { get; private set; } = false;
        public LetterVariable(string id, bool isReadOnly = false) : base(ElementType.VAR, null)
        {
            this.id = id;
            this.isReadOnly = isReadOnly;
        }

        public void assign(LetterValue value)
        {
            id_value = value;
        }

        public void MakeReadOnly()
        {
            isReadOnly = true;
        }

        public string GetIDName()
        {
            return id;
        }

        public bool CanAssign
        {
            get=> !(isReadOnly && id_value.GetLetterElementType() != ElementType.GAMMY);
        }

        public ElementType GetValueType()
        {
            return id_value.GetLetterElementType();
        }

        public override T EvaluateAs<T>()
        {
            return id_value.EvaluateAs<T>();
        }

        public override LetterValue EvaluateWith(LetterValue operand, RelationType relationType)
        {
            return id_value.EvaluateWith(operand, relationType);
        }

        public override object EvaluateByType(Type type)
        {
            if(type == GetType() || type == typeof(LetterValue))
            {
                return this;
            }
            return id_value.EvaluateByType(type);
        }

        public override T getValueAt<T>(int i)
        {
            return (id_value as LetterArrayList).getValueAt<T>(i);
        }
    }
}
