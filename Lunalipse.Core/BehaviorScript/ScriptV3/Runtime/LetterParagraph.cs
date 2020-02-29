using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public class LetterParagraph : LetterValue
    {
        List<LetterValue> LetterStatements;
        public LetterParagraph() : base(ElementType.PARAGRAPH,null)
        {
            LetterStatements = new List<LetterValue>();
        }

        public void addStatement(LetterValue statement)
        {
            if(statement!=null)
            {
                LetterStatements.Add(statement);
            }
        }

        public List<LetterValue> GetStatements()
        {
            return LetterStatements;
        }

        public int GetStatementsCount()
        {
            return LetterStatements.Count;
        }

        public LetterValue GetStatementAt(int i)
        {
            return LetterStatements[i];
        }

        public override T EvaluateAs<T>()
        {
            return (T)EvaluateByType(typeof(T));
        }

        public override object EvaluateByType(Type type)
        {
            if (type == GetType() || type == typeof(LetterValue))
            {
                return this;
            }
            throw new RTNotSupportedException();
        }
    }
}
