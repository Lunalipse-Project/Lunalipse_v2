using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Lunalipse.Core.BehaviorScript.ScriptV2
{
    public class LexicalLibs
    {
        static volatile LexicalLibs lexicalLibs = null;
        static readonly object object_lock = new object();

        public static LexicalLibs Instance
        {
            get
            {
                if (lexicalLibs == null)
                {
                    lock (object_lock)
                    {
                        lexicalLibs = lexicalLibs ?? new LexicalLibs();
                    }
                }
                return lexicalLibs;
            }
        }

        public Dictionary<string, Function> FunctionsDef { get; private set; } = new Dictionary<string, Function>();
        
        public LexicalLibs()
        {
            Parameter CataloguePara = new Parameter("catalogue", ReturnTypes.String, true);
            Parameter VolumeParas = new Parameter("volume", ReturnTypes.Double, true);
            DefineFunction("playInx", FunctionType.PLAY_INDEX, ReturnTypes.Void, FunctionProc.PLAY_PROC, new Parameter("index", ReturnTypes.Int), CataloguePara, VolumeParas);

            DefineFunction("play", FunctionType.PLAY, ReturnTypes.Void, FunctionProc.PLAY_PROC, new Parameter("musicName", ReturnTypes.String), CataloguePara, VolumeParas);

            DefineFunction("setEqzr", FunctionType.SET_EQUALIZER | FunctionType.NON_PLAYABLE_FUNCTION, ReturnTypes.Void, FunctionProc.SET_EQZR_PROC, new Parameter("eqzrData", ReturnTypes.Double | ReturnTypes.ARRAY));

            DefineFunction("getEqzrUserDef", FunctionType.GET_USER_DEF_EQZR, ReturnTypes.Double | ReturnTypes.ARRAY, FunctionProc.GET_EQZR_PROC);

            DefineFunction("getEqzrDefault", FunctionType.GET_DEFAULT_EQZR, ReturnTypes.Double | ReturnTypes.ARRAY, FunctionProc.GET_EQZR_PROC);

            DefineFunction("Rand", FunctionType.RANDOM_NUMBER, ReturnTypes.Int, FunctionProc.RANDOM_PROC,new Parameter("max",ReturnTypes.Int), new Parameter("min", ReturnTypes.Int));

            DefineFunction("Next", FunctionType.NEXT, ReturnTypes.Void, FunctionProc.NEXT_PROC);

            DefineFunction("setCatalogue", FunctionType.RANDOM_NUMBER | FunctionType.NON_PLAYABLE_FUNCTION, ReturnTypes.Int, FunctionProc.SET_CATALOGUE_PROC, CataloguePara);

            DefineFunction("loop", FunctionType.LOOP | FunctionType.NON_PLAYABLE_FUNCTION, ReturnTypes.Void, FunctionProc.LOOP_PROC);

            DefineFunction("getMusicCount", FunctionType.GET_MUSIC_COUNT, ReturnTypes.Int, FunctionProc.GET_MUSIC_COUNT_PROC, CataloguePara);

            DefineFunction("auto", FunctionType.PLACEHOLDER, ReturnTypes.Void, null);
        }

        public void DefineFunction(string functionName, FunctionType functionType, ReturnTypes returnTypes, FunctionProcDelegation funcHandler, params Parameter[] parameters)
        {
            List<Parameter> paras = new List<Parameter>();
            foreach(Parameter p in parameters)
            {
                paras.Add(p);
            }
            if (!FunctionsDef.ContainsKey(functionName))
            {
                FunctionsDef.Add(functionName, new Function()
                {
                    functionType = functionType,
                    functionName = functionName,
                    functionProc = funcHandler,
                    paras = paras,
                    data_type = returnTypes
                });
            }
        }

        public Function GetFunction(string functionName)
        {
            if (FunctionsDef.ContainsKey(functionName))
            {
                return FunctionsDef[functionName];
            }
            return null;
        }
    }

    public class Parameter
    {
        public Parameter()
        {
        }
        public Parameter(string name, ReturnTypes data_type, bool ignorable = false)
        {
            this.name = name;
            this.data_type = data_type;
            this.ignorable = ignorable;
        }
        public Parameter(string name, ReturnTypes data_type, object p_value)
            : this(name, data_type)
        {
            this.p_value = p_value;
        }
        public string name;
        public ReturnTypes data_type;
        public object p_value;
        public bool isFunction = false;
        public bool ignorable = false;
    }

    public class Function : Parameter
    {
        public FunctionType functionType;
        public string functionName;
        public FunctionProcDelegation functionProc;
        public List<Parameter> paras;
    }

    public class CodeBlock
    {
        public List<Function> functions;
        public Parameter repeatTimes;
    }

    public enum ReturnTypes : byte
    {
        Void = 0x0,
        String = 0x1,
        Int = 0x2,
        Double = 0x3,
        ARRAY = 0x1 << 7,
        UNCERTAIN = 0xff
    }

    public enum FunctionType : byte
    {
        PLAY_INDEX = 0x1,
        PLAY = 0x2,
        SET_EQUALIZER = 0x3,
        GET_DEFAULT_EQZR = 0x4,
        GET_USER_DEF_EQZR = 0x5,
        RANDOM_NUMBER = 0x6,
        NEXT = 0x7,
        SET_CATALOGUES = 0x8,
        LOOP = 0x9,
        GET_MUSIC_COUNT = 0x0a,

        OTHER = 0x3f,
        PLACEHOLDER = 0x7f,
        NON_PLAYABLE_FUNCTION = 0x80,
    }
}
