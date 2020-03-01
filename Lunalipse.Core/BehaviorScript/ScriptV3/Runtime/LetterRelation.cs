using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public class LetterRelation : LetterValue
    {
        RelationType type;
        public bool isUnary { get; private set; }
        public LetterRelation(RelationType type, bool isUnary = false) : base(ElementType.RELATION, null)
        {
            this.type = type;
            this.isUnary = isUnary;
        }

        public void SetUnary()
        {
            isUnary = true;
        }

        public RelationType GetRelationType()
        {
            return type;
        }

        public static LetterRelation Create(int optr_type, bool isUnary)
        {
            switch(optr_type)
            {
                case LpsScriptLexer.ADD:
                    return new LetterRelation(RelationType.ADD, isUnary);
                case LpsScriptLexer.MINUS:
                    return new LetterRelation(RelationType.MINUS, isUnary);
                case LpsScriptLexer.MULT:
                    return new LetterRelation(RelationType.MULT, isUnary);
                case LpsScriptLexer.DIV:
                    return new LetterRelation(RelationType.DIV, isUnary);
                case LpsScriptLexer.GE:
                    return new LetterRelation(RelationType.CP_GE, isUnary);
                case LpsScriptLexer.LE:
                    return new LetterRelation(RelationType.CP_LE, isUnary);
                case LpsScriptLexer.GR:
                    return new LetterRelation(RelationType.CP_G, isUnary);
                case LpsScriptLexer.LS:
                    return new LetterRelation(RelationType.CP_L, isUnary);
                case LpsScriptLexer.NOT:
                    return new LetterRelation(RelationType.LG_NOT, isUnary);
                case LpsScriptLexer.EQ:
                    return new LetterRelation(RelationType.CP_EQ, isUnary);
                case LpsScriptLexer.NEQ:
                    return new LetterRelation(RelationType.CP_NEQ, isUnary);
                case LpsScriptLexer.AND:
                    return new LetterRelation(RelationType.LG_AND, isUnary);
                case LpsScriptLexer.OR:
                    return new LetterRelation(RelationType.LG_OR, isUnary);
            }
            return null;
        }
    }
}
