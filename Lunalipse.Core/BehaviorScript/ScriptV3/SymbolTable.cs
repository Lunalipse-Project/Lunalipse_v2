using Antlr4.Runtime.Tree;
using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions;
using Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements;
using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System.Collections;
using System.Collections.Generic;

namespace Lunalipse.Core.BehaviorScript.ScriptV3
{
    public class SymbolTable : IEnumerable<KeyValuePair<string,LetterValue>>
    {
        Dictionary<string, LetterValue> table;

        public SymbolTable()
        {
            table = new Dictionary<string, LetterValue>();
            
        }

        public bool HasSymbol(string terminal)
        {
            return table.ContainsKey(terminal);
        }

        public LetterValue GetSymbol(ITerminalNode terminal, bool with_create = false)
        {
            string name = terminal.Symbol.Text;
            if (HasSymbol(name))
            {
                return table[name];
            }
            else if (!with_create)
            {
                throw new GeneralSemanticException(
                    TokenInfo.CreateTokenInfo(terminal.Symbol),
                    "CORE_LBS_SE_NOT_DEFINE");
            }
            else
            {
                // By default, a brand-new symbol will be created as variable
                table.Add(name, new LetterVariable(name));
                return table[name];
            }
        }

        public void AddSymbol(string identifier, LetterValue body)
        {
            table.Add(identifier, body);
        }

        public void RemoveSymbol(string identifier)
        {
            table.Remove(identifier);
        }

        public void Merge(SymbolTable table)
        {
            foreach (var kv in table)
            {
                if(!HasSymbol(kv.Key))
                {
                    AddSymbol(kv.Key, kv.Value);
                }
            }
        }

        public IEnumerator<KeyValuePair<string, LetterValue>> GetEnumerator()
        {
            return table.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return table.GetEnumerator();
        }

        public LetterValue this[string index]
        {
            get => table[index];
        }
    }
}
