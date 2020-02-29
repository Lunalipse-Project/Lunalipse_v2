using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions;
using Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements;
using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;

namespace Lunalipse.Core.BehaviorScript.ScriptV3
{
    public class LpsFrontEnd
    {
        ICharStream charStream;
        LpsScriptLexer lps_ScriptLexer;
        CommonTokenStream tokenStream;
        LpsScriptParser lpsScriptParser;
        SymbolTable GST;

        public LpsFrontEnd(SymbolTable GlobalSymbolTable)
        {
            GST = GlobalSymbolTable;
        }

        public void InitializeLexer(string path)
        {
            charStream = CharStreams.fromstring(File.ReadAllText(path, Encoding.UTF8));
            lps_ScriptLexer = new LpsScriptLexer(charStream);
            tokenStream = new CommonTokenStream(lps_ScriptLexer);
        }

        public void InitializeParser()
        {
            if(tokenStream == null)
            {
                throw new NullReferenceException("You must only initialize parser after lexer is fully loaded.");
            }
            lpsScriptParser = new LpsScriptParser(tokenStream);
            lpsScriptParser.ErrorHandler = new ParseExceptionStrategy();
        }

        public List<IToken> LexingOnly()
        {
            return tokenStream.GetTokens().ToList();
        }

        public LetterToPrincessLuna Parse()
        {
            if(lpsScriptParser==null || lps_ScriptLexer == null)
            {
                throw new NullReferenceException("Parsing require both parser and lexer ready.");
            }
            ASTVisitor visitor = new ASTVisitor(GST);
            IParseTree tree = lpsScriptParser.prg();
            LetterToPrincessLuna letterToPrincessLuna = null;
            letterToPrincessLuna = visitor.Visit(tree) as LetterToPrincessLuna;
            return letterToPrincessLuna;
        }
    }
}
