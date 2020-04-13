using Lunalipse.Common.Data;
using Lunalipse.Common.Data.BehaviorScript;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.IBehaviorScript;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core.BehaviorScript;
using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions;
using Lunalipse.Core.LpsAudio;
using Lunalipse.Pages.ConfigPage.Structures;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Presentation.LpsWindow;
using Lunalipse.Utilities;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WinForm = System.Windows.Forms;

namespace Lunalipse.Windows
{
    /// <summary>
    /// Interaction logic for LpsScriptLoader.xaml
    /// </summary>
    public partial class LpsScriptLoader : LunalipseDialogue
    {
        BehaviorScriptManager bScriptManager;
        SequenceControllerManager controllerManager;
        II18NConvertor translator;
        IInterpreter currentExecutor;
        string loaded;
        string[] status = new string[3];

        string run, terminate, openfile_ext_desc;

        string unsupportControlerT, unsupportControlerC;
        string deleteT, deleteC;
        public LpsScriptLoader()
        {
            InitializeComponent();
            bScriptManager = BehaviorScriptManager.Instance();
            controllerManager = SequenceControllerManager.Instance;
            currentExecutor = bScriptManager.CurrentLoader.ScriptInterpreter;
            Loaded += LpsScriptLoader_Loaded;
            Unloaded += LpsScriptLoader_Unloaded;
        }

        private void LpsScriptLoader_Unloaded(object sender, RoutedEventArgs e)
        {
            bScriptManager.CurrentLoader.ScriptInterpreter.OnInstructionFinished -= ScriptInterpreter_StatusUpdate;
            TranslationManagerBase.OnI18NEnvironmentChanged -= TranslationManagerBase_OnI18NEnvironmentChanged;
            bScriptManager.CurrentLoader.OnSemanticErrorArised -= CurrentLoader_OnSemanticErrorArised;
            bScriptManager.CurrentLoader.OnSyntaxErrorArised -= CurrentLoader_OnSyntaxErrorArised;
            currentExecutor.OnProgramComplete -= ScriptInterpreter_StatusUpdate;
            Loaded -= LpsScriptLoader_Loaded;
            Unloaded -= LpsScriptLoader_Unloaded;
        }

        private void LpsScriptLoader_Loaded(object sender, RoutedEventArgs e)
        {
            TranslationManagerBase.OnI18NEnvironmentChanged += TranslationManagerBase_OnI18NEnvironmentChanged;
            bScriptManager.CurrentLoader.OnSemanticErrorArised += CurrentLoader_OnSemanticErrorArised;
            bScriptManager.CurrentLoader.OnSyntaxErrorArised += CurrentLoader_OnSyntaxErrorArised;
            foreach (BScriptLocation script in bScriptManager.ScriptCollection)
            {
                if(bScriptManager.LoadedScript!=null)
                {
                    if (script.ScriptName == bScriptManager.LoadedScript.ScriptName)
                    {
                        ScriptLocations.Items.Insert(0, new BScriptLocationStruc(script));
                        continue;
                    }
                }
                ScriptLocations.Add(new BScriptLocationStruc(script));
            }
            TranslationManagerBase_OnI18NEnvironmentChanged(TranslationManagerBase.AquireConverter());
            currentExecutor.OnInstructionFinished += ScriptInterpreter_StatusUpdate;
            currentExecutor.OnProgramComplete += ScriptInterpreter_StatusUpdate;
            if (bScriptManager.CurrentLoader.isScriptLoaded)
            {
                ScriptInterpreter_StatusUpdate();
                Load.Content = terminate;
            }
        }

        private void CurrentLoader_OnSyntaxErrorArised(System.Exception obj)
        {
            GeneralSyntaxErrorException syntaxErrorException = obj as GeneralSyntaxErrorException;
            string title = translator.ConvertTo(SupportedPages.CORE_FUNC, "CORE_LBS_SY");
            string content = translator.ConvertTo(SupportedPages.CORE_FUNC, "CORE_LBS_SY_MISMATCH");
            content = content.FormateEx(syntaxErrorException.ErrorLine, syntaxErrorException.ErrorColumn, syntaxErrorException.MismatchedToken);
            new CommonDialog(title, content, MessageBoxButton.OK).ShowDialog();
        }

        private void CurrentLoader_OnSemanticErrorArised(System.Exception obj)
        {
            string title, content;
            title = translator.ConvertTo(SupportedPages.CORE_FUNC, "CORE_LBS_SE");
            content = translator.ConvertTo(SupportedPages.CORE_FUNC, obj.Message);
            if (obj is DuplicateSymbolException)
            {
                DuplicateSymbolException duplicateSymbol = obj as DuplicateSymbolException;
                content = content.FormateEx(duplicateSymbol.OffendingToken.Line,
                                            duplicateSymbol.OffendingToken.Column,
                                            duplicateSymbol.OffendingToken.TokenText,
                                            duplicateSymbol.LetterValueType.ToString());
            }
            else if (obj is UnmatchedParamter)
            {
                UnmatchedParamter unmatchedParamter = obj as UnmatchedParamter;
                content = content.FormateEx(unmatchedParamter.OffendingToken.Line,
                                            unmatchedParamter.OffendingToken.Column,
                                            unmatchedParamter.OffendingToken.TokenText,
                                            unmatchedParamter.Data["expected"],
                                            unmatchedParamter.Data["actual"]);
            }
            else
            {
                GeneralSemanticException semanticException = obj as GeneralSemanticException;
                content = content.FormateEx(semanticException.OffendingToken.Line,
                                            semanticException.OffendingToken.Column,
                                            semanticException.OffendingToken.TokenText);
            }
            new CommonDialog(title, content, MessageBoxButton.OK).ShowDialog();
        }

        private void ScriptInterpreter_StatusUpdate()
        {
            Dispatcher.Invoke(() =>
            {
                ctxID.Content = currentExecutor.CurrentContextIdentifier;
                ctxPtr.Content = currentExecutor.CurrentStackPointer;
                IntrpStatus.Content = status[(int)currentExecutor.GetInterpreterStatus];
                stackDepth.Content = currentExecutor.ExecutionStackDepth;
                if (!bScriptManager.CurrentLoader.isScriptLoaded)
                {
                    Load.Content = run;
                }
            });
        }

        private void TranslationManagerBase_OnI18NEnvironmentChanged(Common.Interfaces.II18N.II18NConvertor obj)
        {
            translator = obj;
            foreach (var i in Utils.FindVisualChildren<ContentControl>(this))
            {
                if (i.Tag == null) continue;
                i.Content = obj.ConvertTo(SupportedPages.CORE_FUNC, i.Tag as string);
            }
            Title = obj.ConvertTo(SupportedPages.CORE_FUNC, Tag as string);
            status[0] = obj.ConvertTo(SupportedPages.CORE_FUNC, "CORE_BSLOADER_INTRP_RUNNING");
            status[1] = obj.ConvertTo(SupportedPages.CORE_FUNC, "CORE_BSLOADER_INTRP_HALTED");
            status[2] = obj.ConvertTo(SupportedPages.CORE_FUNC, "CORE_BSLOADER_INTRP_STOPPED");
            run = obj.ConvertTo(SupportedPages.CORE_FUNC, "CORE_BSLOADER_LOAD");
            terminate = obj.ConvertTo(SupportedPages.CORE_FUNC, "CORE_BSLOADER_UNLOAD");
            unsupportControlerT = obj.ConvertTo(SupportedPages.CORE_FUNC, "CORE_BSLOADER_FUNC_CTRLLER_UNLOAD_TITLE");
            unsupportControlerC = obj.ConvertTo(SupportedPages.CORE_FUNC, "CORE_BSLOADER_FUNC_CTRLLER_UNLOAD_CONTENT");
            openfile_ext_desc = obj.ConvertTo(SupportedPages.CORE_FUNC, "CORE_BSLOADER_OPENFILE_LETTER_EXT");
            deleteT = obj.ConvertTo(SupportedPages.CORE_FUNC, "CORE_BSLOADER_OPENFILE_DELETE_TITLE");
            deleteC = obj.ConvertTo(SupportedPages.CORE_FUNC, "CORE_BSLOADER_OPENFILE_DELETE_CONTENT");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BScriptLocationStruc bScriptLocationStruc = ScriptLocations.SelectedItem as BScriptLocationStruc;
            Button button = sender as Button;
            switch(button.Name)
            {
                case "Load":
                    if (!controllerManager.CurrentController.SupportScipt)
                    {
                        (new CommonDialog(unsupportControlerT, unsupportControlerC, MessageBoxButton.OK)).ShowDialog();
                    }
                    else
                    {
                        if(currentExecutor.GetInterpreterStatus == InterpreterStatus.STOPPED)
                        {
                            bScriptManager.LoadScript(bScriptLocationStruc.DetailedDescription);
                            if(bScriptManager.CurrentLoader.isScriptLoaded)
                            {
                                button.Content = terminate;
                            }
                        }
                        else
                        {
                            currentExecutor.StopExecution();
                            button.Content = run;
                        }
                    }
                    break;
                case "Delete":
                    bool? result = (new CommonDialog(deleteT, deleteC.FormateEx(bScriptLocationStruc.bScriptLocation.ScriptName), MessageBoxButton.YesNo)).ShowDialog();
                    if (result.HasValue && result.Value)
                    {
                        File.Delete(bScriptLocationStruc.bScriptLocation.ScriptLocation);
                        ScriptLocations.Remove(ScriptLocations.SelectedItem);
                    }
                    break;
                case "Add":
                    WinForm.OpenFileDialog openFile = new WinForm.OpenFileDialog();
                    openFile.Filter = $"{openfile_ext_desc} (*.letter)|*.letter";
                    if(openFile.ShowDialog() == WinForm.DialogResult.OK)
                    {
                        BScriptLocation location = bScriptManager.AddScript(openFile.FileName);
                        ScriptLocations.Add(new BScriptLocationStruc(location));
                    }
                    break;
            }
        }

        protected override void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            base.ThemeManagerBase_OnThemeApplying(obj);
            Brush colorBrush = obj.Secondary.ToLuna();
            Add.Background = colorBrush;
            Delete.Background = colorBrush;
            Load.Background = colorBrush;
        }
    }
}
