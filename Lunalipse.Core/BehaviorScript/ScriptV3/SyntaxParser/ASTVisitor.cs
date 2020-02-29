using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions;
using Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser
{
    public class ASTVisitor : LpsScriptParserBaseVisitor<LetterValue>
    {
        SymbolTable symbolTable;

        public ASTVisitor()
        {
            symbolTable = new SymbolTable();
        }

        public ASTVisitor(SymbolTable AnotherTable) : this()
        {
            symbolTable.Merge(AnotherTable);
        }

        public void ResetVisitor()
        {
            //SymbolTable.Clear();
        }

        public override LetterValue VisitPrg([NotNull] LpsScriptParser.PrgContext context)
        {
            LetterString title = Visit(context.prg_start()) as LetterString;
            LetterString writer = Visit(context.prg_end()) as LetterString;
            LetterToPrincessLuna toPrincessLuna = new LetterToPrincessLuna(writer, title);
            symbolTable.AddSymbol(title.ToString(), toPrincessLuna);
            toPrincessLuna.MainProgram = Visit(context.prg_content()) as LetterParagraph;
            toPrincessLuna.MainProgram.SetElementTokenInfo(new TokenInfo(0, 0, title.ToString()));
            toPrincessLuna.SymbolTable = symbolTable;
            return toPrincessLuna;
        }

        public override LetterValue VisitPrg_start([NotNull] LpsScriptParser.Prg_startContext context)
        {
            // Program name is a valid symbol
            //      + planned feature: allow jump between letter and access the stuff in other letter
            return LetterString.Create(context.ID());
        }

        public override LetterValue VisitPrg_end([NotNull] LpsScriptParser.Prg_endContext context)
        {
            return LetterString.Create(context.STRING());
        }

        public override LetterValue VisitStatements([NotNull] LpsScriptParser.StatementsContext context)
        {
            if (context.ChildCount == 0)
            {
                return new LetterParagraph();
            }
            else if(context.ChildCount == 1)
            {
                LetterParagraph paragraph = new LetterParagraph();
                paragraph.addStatement(Visit(context.statement()));
                return paragraph;
            }
            LetterParagraph lparagraph = Visit(context.statements()) as LetterParagraph;
            lparagraph.addStatement(Visit(context.statement()));
            return lparagraph;
        }



        public override LetterValue VisitGroup_statemnt([NotNull] LpsScriptParser.Group_statemntContext context)
        {
            LetterValue letterValue = Visit(context.statements());
            letterValue.SetElementTokenInfo(TokenInfo.CreateTokenInfo(context.ID().Symbol));
            return AddSymbol(context.ID(), letterValue);
        }

        public override LetterValue VisitStatement([NotNull] LpsScriptParser.StatementContext context)
        {
            LetterValue statement;
            if (context.cata_choose() != null)
            {
                statement = Visit(context.cata_choose());
            }
            else if (context.play_actions() != null)
            {
                statement = Visit(context.play_actions());
            }
            else if (context.set_eqzr() != null)
            {
                statement = Visit(context.set_eqzr());
            }
            else if (context.do_action() != null)
            {
                statement = Visit(context.do_action());
            }
            else if (context.assign() != null)
            {
                statement = Visit(context.assign());
            }
            else
            {
                statement = Visit(context.func_call());
            }
            if (context.conditions() != null)
            {
                (statement as ISuffixable).SetSuffixActions(Visit(context.conditions()) as LetterSuffixActions);
            }
            return statement;
        }

        public override LetterValue VisitFunc_call([NotNull] LpsScriptParser.Func_callContext context)
        {
            LpsScriptParser.ArrayContext parameterContext = context.array();
            ITerminalNode node = context.ID();
            if (!symbolTable.HasSymbol(node.Symbol.Text))
            {
                throw new GeneralSemanticException(
                        TokenInfo.CreateTokenInfo(node.Symbol),
                        "CORE_LBS_SE_SPELL_NOT_FOUND");

            }
            LetterValue spell = symbolTable[node.Symbol.Text];
            if (spell.GetLetterElementType() != ElementType.SPELL_DELEGATION)
            {
                throw new GeneralSemanticException(TokenInfo.CreateTokenInfo(node.Symbol),
                                                    "CORE_LBS_SE_NOT_A_SPELL");
            }
            LetterFunctionCall functionCall = new LetterFunctionCall(context.ID().Symbol.Text,spell as LetterStarSwirlSpell,
                                                                    TokenInfo.CreateTokenInfo(context.ID().Symbol));
            if (parameterContext != null)
            {
                functionCall.SetParameter(Visit(parameterContext) as LetterArrayList);
            }
            functionCall.ValidateParameter();
            return functionCall;
        }

        public override LetterValue VisitArray_indexing([NotNull] LpsScriptParser.Array_indexingContext context)
        {
            LetterValue index;
            if (context.ID(1) != null)
            {
                index = symbolTable.GetSymbol(context.ID(1));
            }
            else
            {
                index = LetterNumber.Create(context.INT());
            }
            return new LetterIndexing(symbolTable.GetSymbol(context.ID(0)) as LetterVariable, index,
                                      TokenInfo.CreateTokenInfo(context.ID(0).Symbol));
        }

        public override LetterValue VisitArrayDeclr([NotNull] LpsScriptParser.ArrayDeclrContext context)
        {
            return Visit(context.array_content());
        }

        public override LetterValue VisitArray_content([NotNull] LpsScriptParser.Array_contentContext context)
        {
            if (context.ChildCount == 0)
            {
                return new LetterArrayList();
            }
            if (context.ChildCount == 1)
            {
                return new LetterArrayList(Visit(context.expr_wrap()));
            }
            LetterArrayList array = Visit(context.array_content(0)) as LetterArrayList;
            array.AddToElementList(Visit(context.array_content(1).expr_wrap()));
            return array;
        }

        public override LetterValue VisitAssign([NotNull] LpsScriptParser.AssignContext context)
        {
            LetterValue letterSymbolBase = symbolTable.GetSymbol(context.ID(),true);
            if (letterSymbolBase.GetLetterElementType() != ElementType.VAR)
            {
                throw new GeneralSemanticException(
                    TokenInfo.CreateTokenInfo(context.ID().Symbol),
                    "CORE_LBS_SE_INVALID_ASSIGN");
            }
            if (context.MAKE_CONSTANT() != null)
            {
                (letterSymbolBase as LetterVariable).MakeReadOnly();
            }
            return new LetterAssign(letterSymbolBase as LetterVariable, Visit(context.expr_wrap()), TokenInfo.CreateTokenInfo(context.ID().Symbol));
        }

        public override LetterValue VisitCata_choose([NotNull] LpsScriptParser.Cata_chooseContext context)
        {
            LetterFunctionCall functionCall = WrapActionAsFunctionCall(LetterActionType.ACT_CATA_CHOOSE, context.Start.Line, context.Start.Column);
            functionCall.SetParameter(new LetterArrayList(Visit(context.any_id())));
            return functionCall;
        }

        public override LetterValue VisitPlay_actions([NotNull] LpsScriptParser.Play_actionsContext context)
        {
            LetterFunctionCall functionCall;
            if (context.PLAY() != null)
            {
                functionCall = WrapActionAsFunctionCall(LetterActionType.ACT_PLAY, context.Start.Line, context.Start.Column);
                functionCall.SetParameter(new LetterArrayList(LetterString.Create(context.STRING())));
            }
            else
            {
                functionCall = WrapActionAsFunctionCall(LetterActionType.ACT_PLAY_BY_NUM, context.Start.Line, context.Start.Column);
                functionCall.SetParameter(new LetterArrayList(Visit(context.expr_wrap())));
            }
            return functionCall;
        }

        public override LetterValue VisitExpr_wrap([NotNull] LpsScriptParser.Expr_wrapContext context)
        {
            return new LetterExpression(Visit(context.expr()) as LetterRPN,
                                        TokenInfo.CreateTokenInfo(context.Start));
        }

        public override LetterValue VisitExprP1([NotNull] LpsScriptParser.ExprP1Context context)
        {
            return new LetterRPN(Visit(context.expr(0)) as LetterRPN,
                                 Visit(context.optr_P1()) as LetterRelation,
                                 Visit(context.expr(1)) as LetterRPN);
        }

        public override LetterValue VisitExprP2([NotNull] LpsScriptParser.ExprP2Context context)
        {
            return new LetterRPN(Visit(context.expr(0)) as LetterRPN,
                                 Visit(context.optr_P2()) as LetterRelation,
                                 Visit(context.expr(1)) as LetterRPN);
        }

        public override LetterValue VisitExprUnary([NotNull] LpsScriptParser.ExprUnaryContext context)
        {
            LetterRelation letterRelation = Visit(context.optr_P2()) as LetterRelation;
            letterRelation.SetUnary();
            return new LetterRPN(letterRelation,
                                 Visit(context.expr()) as LetterRPN);
        }

        public override LetterValue VisitExprParen([NotNull] LpsScriptParser.ExprParenContext context)
        {
            return Visit(context.expr());
        }

        public override LetterValue VisitExprInd([NotNull] LpsScriptParser.ExprIndContext context)
        {
            return new LetterRPN(Visit(context.GetChild(0)));
        }

        public override LetterValue VisitOptr_P1([NotNull] LpsScriptParser.Optr_P1Context context)
        {
            if (context.MULT() != null)
            {
                return LetterRelation.Create(context.MULT().Symbol.Type, false);
            }
            else
            {
                return LetterRelation.Create(context.DIV().Symbol.Type, false);
            }
        }

        public override LetterValue VisitOptr_P2([NotNull] LpsScriptParser.Optr_P2Context context)
        {
            if (context.ADD() != null)
            {
                return LetterRelation.Create(context.ADD().Symbol.Type, false);
            }
            else
            {
                return LetterRelation.Create(context.MINUS().Symbol.Type, false);
            }
        }

        public override LetterValue VisitSet_eqzr([NotNull] LpsScriptParser.Set_eqzrContext context)
        {
            LetterFunctionCall functionCall = WrapActionAsFunctionCall(LetterActionType.ACT_SET_EQZR, context.Start.Line, context.Start.Column);
            if (context.array()!=null)
            {
                functionCall.SetParameter(new LetterArrayList(Visit(context.array())));
            }
            else
            {
                functionCall.SetParameter(new LetterArrayList(symbolTable.GetSymbol(context.ID())));
            }
            return functionCall;
        }

        public override LetterValue VisitDo_action([NotNull] LpsScriptParser.Do_actionContext context)
        {
            LetterFunctionCall functionCall = WrapActionAsFunctionCall(LetterActionType.ACT_DO_CHECKLIST, context.Start.Line, context.Start.Column);
            functionCall.SetParameter(new LetterArrayList(symbolTable.GetSymbol(context.ID())));
            return functionCall;
        }

        public override LetterValue VisitConditions([NotNull] LpsScriptParser.ConditionsContext context)
        {
            if (context.ChildCount == 1)
            {
                LetterSuffixActions lsa = new LetterSuffixActions();
                lsa.AddSuffixAction(Visit(context.condition()) as SuffixAction);
                return lsa;
            }
            LetterSuffixActions letterSuffixActions = Visit(context.conditions(0)) as LetterSuffixActions;
            letterSuffixActions.AddSuffixAction(Visit(context.conditions(1).condition()) as SuffixAction);
            return letterSuffixActions;
        }

        public override LetterValue VisitSuffixSetVol([NotNull] LpsScriptParser.SuffixSetVolContext context)
        {
            return new SuffixAction(LetterActionType.ACT_SET_VOL, Visit(context.expr_wrap()));
        }

        public override LetterValue VisitSuffixLoop([NotNull] LpsScriptParser.SuffixLoopContext context)
        {
            return new SuffixAction(LetterActionType.ACT_REPEAT_TIME, Visit(context.expr_wrap()));
        }

        public override LetterValue VisitAny_number([NotNull] LpsScriptParser.Any_numberContext context)
        {
            string num = context.INT() != null ? context.INT().Symbol.Text : context.REAL().Symbol.Text;
            return new LetterNumber(num);
        }

        public override LetterValue VisitAny_id([NotNull] LpsScriptParser.Any_idContext context)
        {
            if (context.ID() != null)
            {
                return symbolTable.GetSymbol(context.ID(), true);
            }
            else
            {
                return new LetterString(context.STRING().Symbol.Text);
            }
        }

        private LetterValue AddSymbol(ITerminalNode id_node, LetterValue symbol)
        {
            string name = id_node.Symbol.Text;
            if (symbolTable.HasSymbol(name))
            {
                throw new DuplicateSymbolException(symbolTable.GetSymbol(id_node).GetLetterElementType(), 
                    TokenInfo.CreateTokenInfo(id_node.Symbol),
                    "CORE_LBS_SE_DUPL_DECLARE");
            }
            symbolTable.AddSymbol(name, symbol);
            return symbolTable.GetSymbol(id_node);
        }

        private LetterFunctionCall WrapActionAsFunctionCall(LetterActionType actionType,int line, int col)
        {
            string funcall = actionType.ToString();
            if (!symbolTable.HasSymbol(actionType.ToString()))
            {
                throw new GeneralSemanticException(new TokenInfo(line,col,funcall),
                    "CORE_LBS_SE_SPELL_NOT_FOUND");
            }
            TokenInfo tokenInfo = new TokenInfo(line, col, funcall);
            LetterStarSwirlSpell starSwirlSpell = symbolTable[funcall] as LetterStarSwirlSpell;
            LetterFunctionCall functionCall = new LetterFunctionCall(funcall, starSwirlSpell, tokenInfo);
            return functionCall;
        }
    }
}
