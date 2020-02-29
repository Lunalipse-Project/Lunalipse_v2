using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public class LetterToPrincessLuna : LetterValue
    {
        public LetterParagraph mainProgram;
        
        public LetterToPrincessLuna(LetterString Writer, LetterString Title) : base(ElementType.TO_PRINCESS_LUNA)
        {
            this.Writer = Writer;
            this.Title = Title;
        }
        public LetterParagraph MainProgram
        {
            get => mainProgram;
            set
            {
                mainProgram = value;
            }
        }

        public SymbolTable SymbolTable { get; set; }
        public LetterString Writer { get; private set; }
        public LetterString Title { get; private set; }
    }
}
