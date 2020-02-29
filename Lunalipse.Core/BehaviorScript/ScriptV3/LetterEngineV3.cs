using Lunalipse.Common.Data;
using Lunalipse.Common.Data.BehaviorScript;
using Lunalipse.Common.Interfaces.IAudio;
using Lunalipse.Common.Interfaces.IBehaviorScript;
using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions;
using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions.Runtime;
using Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements;
using Lunalipse.Core.PlayList;
using System;

namespace Lunalipse.Core.BehaviorScript.ScriptV3
{
    public class LetterEngineV3 : IScriptEngine
    {
        LpsInterpreter interpreter;
        IAudioContext AudioCoreContext;
        CataloguePool cataloguePool;
        Random random;
        public LetterEngineV3(IAudioContext AudioCoreContext)
        {
            interpreter = new LpsInterpreter(InterpreterConfig.CreateDefaultConfig());
            cataloguePool = CataloguePool.Instance;
            this.AudioCoreContext = AudioCoreContext;
            interpreter.OnProgramComplete += () => OnScriptCompleted?.Invoke();
            interpreter.OnRuntimeExceptionThrown += (e) => OnRuntimeErrorArised?.Invoke(e);
            RegisterAction();
            RegisterMagicSpell();
        }

        public bool isScriptLoaded => interpreter.GetInterpreterStatus != InterpreterStatus.STOPPED;

        public IInterpreter ScriptInterpreter => interpreter;

        public event Action<Exception> OnRuntimeErrorArised;
        public event Action<Exception> OnSyntaxErrorArised;
        public event Action<Exception> OnSemanticErrorArised;
        public event Action OnScriptCompleted;

        public void LoadScript(BScriptLocation bScriptLocation)
        {
            try
            {
                random = new Random();
                interpreter.Prepare(bScriptLocation.ScriptLocation);
                interpreter.Execute();
            }
            catch(GeneralSyntaxErrorException e)
            {
                OnSyntaxErrorArised?.Invoke(e);
            }
            catch(FrontEndExceptionBase e)
            {
                OnSemanticErrorArised?.Invoke(e);
            }
        }

        public void SetAudioContext(IAudioContext AudioCoreContext)
        {
            this.AudioCoreContext = AudioCoreContext;
        }

        public void RegisterAction()
        {
            interpreter.RegisterAction(LetterActionType.ACT_CATA_CHOOSE,
                new Action<string>(cata_name =>
                {
                    Catalogue catalogue = cataloguePool.GetCatalogueFirst(cata_name, true);
                    if (catalogue == null)
                    {
                        throw new RuntimeException("CORE_LBS_RT_CATA_NFOUND", cata_name);
                    }
                    AudioCoreContext.SetCatalogue(catalogue);
                }));
            interpreter.RegisterAction(LetterActionType.ACT_PLAY,
                new Action<string>(music_name =>
                {
                    MusicEntity musicEntity = AudioCoreContext.GetCurrentCatalogue().getMusic(music_name);
                    if(musicEntity == null)
                    {
                        throw new RuntimeException("CORE_LBS_RT_ME_NFOUND", music_name);
                    }
                    interpreter.HaltExecution();
                    AudioCoreContext.PrepareMusic(musicEntity);
                }));
            interpreter.RegisterAction(LetterActionType.ACT_PLAY_BY_NUM,
                new Action<int>(music_index =>
                {
                    MusicEntity musicEntity = AudioCoreContext.GetCurrentCatalogue().getMusic(music_index);
                    if (musicEntity == null)
                    {
                        throw new RuntimeException("CORE_LBS_RT_MEI_NFOUND", music_index.ToString());
                    }
                    interpreter.HaltExecution();
                    AudioCoreContext.PrepareMusic(musicEntity);
                }));
            interpreter.RegisterAction(LetterActionType.ACT_SET_VOL,
                new Action<double>(volume =>
                {
                    if (volume > 100 || volume < 0)
                    {
                        throw new RuntimeException();
                    }
                    AudioCoreContext.SetVolume(volume);
                }));
            interpreter.RegisterAction(LetterActionType.ACT_SET_EQZR,
                new Action<double[]>(equalizer_values =>
                {
                    if (equalizer_values == null || equalizer_values.Length >= 10)
                    {
                        throw new RuntimeException();
                    }
                    AudioCoreContext.ApplyEqualizerSetting(equalizer_values);
                }));
        }

        public void RegisterMagicSpell()
        {
            interpreter.RegisterSpell("randm", new Func<int, int>(max =>
             {
                 return random.Next(max);
             }));
            interpreter.RegisterSpell("randim", new Func<int,int, int>((min,max) =>
            {
                return random.Next(min, max);
            }));
            interpreter.RegisterSpell("randd", new Func<double>(() =>
            {
                return random.NextDouble();
            }));
            interpreter.RegisterSpell("getMusicCount", new Func<int>(() =>
            {
                if (AudioCoreContext.GetCurrentCatalogue() == null)
                {
                    return 0;
                }
                return AudioCoreContext.GetCurrentCatalogue().GetCount();
            }));
        }

        public void Resume()
        {
            interpreter.ResumeExecution();
        }

        public void UnloadScript()
        {
            interpreter.StopExecution();
        }
    }
}
