using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions.Runtime
{

    [Serializable]
    public class RuntimeException : Exception
    {
        public TokenInfo Location { get; private set; }
        public string[] Arguements { get; private set; }
        public RuntimeException() : base() { }
        public RuntimeException(string message, params string[] args) : base(message)
        {
            Arguements = args;
        }
        public RuntimeException(string message, TokenInfo location) : base(message)
        {
            this.Location = location;
        }
        public RuntimeException(string message, Exception inner) : base(message, inner) { }
        protected RuntimeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
