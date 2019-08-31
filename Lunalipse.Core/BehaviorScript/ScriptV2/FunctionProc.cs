using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Core.PlayList;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lunalipse.Core.BehaviorScript.ScriptV2
{
    public delegate object FunctionProcDelegation(Function function, ref ICatalogue MusicCatalogue, ref int scriptPtr, ref MusicEntity musicEntity);
    public class FunctionProc
    {
        public static CataloguePool CataloguePool { get; set; }
        public static object PLAY_PROC(Function function, ref ICatalogue MusicCatalogue, ref int scriptPtr, ref MusicEntity musicEntity)
        {
            ICatalogue catalogue = MusicCatalogue;
            List<Parameter> paras = function.paras;
            if (paras.Count == 3 && paras[2].p_value != null)
            {
                LpsAudio.AudioDelegations.ChangeVolume((float)(double)paras[2].p_value);
            }
            if (paras.Count == 2 && paras[1].p_value != null)
            {
                catalogue = CataloguePool.GetCatalogueFirst(paras[1].p_value as string)
                    ?? throw new ScriptException("CORE_BSCRIPTV2_ERROR_RT_CATANFOUND", ScriptExceptionType.RUNTIME, paras[1].p_value as string);
            }
            switch (function.functionType)
            {
                case FunctionType.PLAY:
                    musicEntity = catalogue.getMusic((string)paras[0].p_value)
                        ?? throw new ScriptException("CORE_BSCRIPTV2_ERROR_RT_MUSICNFPUND", ScriptExceptionType.RUNTIME, paras[0].p_value, MusicCatalogue.Name());
                    break;
                case FunctionType.PLAY_INDEX:
                    musicEntity = catalogue.getMusic((int)paras[0].p_value) 
                        ?? throw new ScriptException("CORE_BSCRIPTV2_ERROR_RT_INDEXOFR", ScriptExceptionType.RUNTIME);
                    break;
            }
            return null;
        }

        public static object SET_EQZR_PROC(Function function, ref ICatalogue MusicCatalogue, ref int scriptPtr, ref MusicEntity musicEntity)
        {
            double[] data = (double[])function.paras[0].p_value;
            for (int i = 0; i < 10; i++)
            {
                double d = data[i];
                if (d > 12)
                {
                    d = 12;
                }
                else if (d < -12)
                {
                    d = -12;
                }
                data[i] = d;
            }
            LpsAudio.AudioDelegations.ChangeEqualizerSetting?.Invoke(data);
            return null;
        }

        public static object GET_EQZR_PROC(Function function, ref ICatalogue MusicCatalogue, ref int scriptPtr, ref MusicEntity musicEntity)
        {
            switch (function.functionType)
            {
                case FunctionType.GET_DEFAULT_EQZR:
                    return new double[10];
                case FunctionType.GET_USER_DEF_EQZR:
                    return LpsAudio.AudioDelegations.GetUserDefinedEqualizerSetting?.Invoke();
                default:
                    return null;
            }
        }

        public static object RANDOM_PROC(Function function, ref ICatalogue MusicCatalogue, ref int scriptPtr, ref MusicEntity musicEntity)
        {
            List<Parameter> paras = function.paras;
            Random random = new Random();
            if (paras.Count == 1)
            {
                if (paras[0].p_value == null)
                {
                    throw new ScriptException("CORE_BSCRIPTV2_ERROR_RT_MAXVALNDEF", ScriptExceptionType.RUNTIME);
                }
                return random.Next((int)paras[0].p_value);
            }
            else
            {
                if (paras[1].p_value == null)
                {
                    throw new ScriptException("CORE_BSCRIPTV2_ERROR_RT_MINVALNDEF", ScriptExceptionType.RUNTIME);
                }
                if (paras[0].p_value == null)
                {
                    throw new ScriptException("CORE_BSCRIPTV2_ERROR_RT_MAXVALNDEF", ScriptExceptionType.RUNTIME);
                }
                return random.Next((int)paras[1].p_value, (int)paras[0].p_value);
            }
        }
        public static object NEXT_PROC(Function function, ref ICatalogue MusicCatalogue, ref int scriptPtr, ref MusicEntity musicEntity)
        {
            musicEntity = MusicCatalogue.getNext();
            return null;
        }

        public static object SET_CATALOGUE_PROC(Function function, ref ICatalogue MusicCatalogue, ref int scriptPtr, ref MusicEntity musicEntity)
        {
            ICatalogue catalogue = CataloguePool.GetCatalogueFirst(function.paras[1].p_value as string);
            MusicCatalogue = catalogue ?? throw new ScriptException("CORE_BSCRIPTV2_ERROR_RT_CATANFOUND", ScriptExceptionType.RUNTIME, function.paras[1].p_value as string);
            return null;
        }

        public static object GET_MUSIC_COUNT_PROC(Function function, ref ICatalogue MusicCatalogue, ref int scriptPtr, ref MusicEntity musicEntity)
        {
            List<Parameter> paras = function.paras;
            ICatalogue catalogue = MusicCatalogue;
            if (function.paras.Count > 0)
            {
                catalogue = CataloguePool.GetCatalogueFirst(function.paras[0].p_value as string);
            }
            return catalogue.GetCount();
        }

        public static object LOOP_PROC(Function function, ref ICatalogue MusicCatalogue, ref int scriptPtr, ref MusicEntity musicEntity)
        {
            scriptPtr = 0;
            return null;
        }
    }
}
