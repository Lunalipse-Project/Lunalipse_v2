using Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3
{
    public class InterpreterConfig
    {
        public SymbolTable GlobalSymbolTable { get; private set; }

        /// <summary>
        /// The maximum depth of nested checklist call allowed
        /// </summary>
        public int MaximumContextDepth { get; set; }

        /// <summary>
        /// Frequency of interpreter (Unit: Hz)
        /// </summary>
        public int InstructionsPreSecond { get; set; }

        /// <summary>
        /// LpsInterpreter will try to recover from runtime error with this enabled.
        /// </summary>
        public bool AutoRuntimeErrorRecovery { get; set; }
        public InterpreterConfig()
        {
            GlobalSymbolTable = new SymbolTable();
        }

        public static InterpreterConfig CreateDefaultConfig()
        {
            InterpreterConfig config = new InterpreterConfig();
            config.AutoRuntimeErrorRecovery = false;
            config.InstructionsPreSecond = 100;
            config.MaximumContextDepth = 50;
            config.GlobalSymbolTable.AddSymbol("DefaultEqualizerSetting",
                LetterValue.CreateLetterValue(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, typeof(int[])));
            config.GlobalSymbolTable.AddSymbol("DefaultVolume",
                LetterValue.CreateLetterValue(70, typeof(double)));
            config.GlobalSymbolTable.AddSymbol("DefaultCatalogue",
                LetterValue.CreateLetterValue("CORE_CATALOGUE_AllMusic", typeof(string)));
            return config;
        }
    }
}
