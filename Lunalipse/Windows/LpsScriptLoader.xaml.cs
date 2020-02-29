using Lunalipse.Common.Data;
using Lunalipse.Common.Data.BehaviorScript;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.IBehaviorScript;
using Lunalipse.Core.BehaviorScript;
using Lunalipse.Core.LpsAudio;
using Lunalipse.Pages.ConfigPage.Structures;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Presentation.LpsWindow;
using Lunalipse.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lunalipse.Windows
{
    /// <summary>
    /// Interaction logic for LpsScriptLoader.xaml
    /// </summary>
    public partial class LpsScriptLoader : LunalipseDialogue
    {
        BehaviorScriptManager bScriptManager;
        SequenceControllerManager controllerManager;
        IInterpreter currentExecutor;
        string loaded;

        string unsupportControlerT, unsupportControlerC;
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
            bScriptManager.CurrentLoader.ScriptInterpreter.OnInstructionFinished -= ScriptExecutor_OnMovingNext;
            TranslationManagerBase.OnI18NEnvironmentChanged -= TranslationManagerBase_OnI18NEnvironmentChanged;
            bScriptManager.CurrentLoader.OnSemanticErrorArised -= CurrentLoader_OnSemanticErrorArised;
            bScriptManager.CurrentLoader.OnSyntaxErrorArised -= CurrentLoader_OnSyntaxErrorArised;
            Loaded -= LpsScriptLoader_Loaded;
            Unloaded -= LpsScriptLoader_Unloaded;
        }

        private void LpsScriptLoader_Loaded(object sender, RoutedEventArgs e)
        {
            TranslationManagerBase.OnI18NEnvironmentChanged += TranslationManagerBase_OnI18NEnvironmentChanged;
            bScriptManager.CurrentLoader.OnSemanticErrorArised += CurrentLoader_OnSemanticErrorArised;
            bScriptManager.CurrentLoader.OnSyntaxErrorArised += CurrentLoader_OnSyntaxErrorArised;
            foreach(BScriptLocation script in bScriptManager.ScriptCollection)
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
            currentExecutor.OnInstructionFinished += ScriptExecutor_OnMovingNext;
            if(bScriptManager.CurrentLoader.isScriptLoaded)
            {
                ScriptExecutor_OnMovingNext();
            }
        }

        private void CurrentLoader_OnSyntaxErrorArised(System.Exception obj)
        {
            throw new System.NotImplementedException();
        }

        private void CurrentLoader_OnSemanticErrorArised(System.Exception obj)
        {
            throw new System.NotImplementedException();
        }

        private void ScriptExecutor_OnMovingNext()
        {
            ScriptStatus.Visibility = Visibility.Visible;
            Pointers.Visibility = Visibility.Visible;
            Status.Content = loaded.FormateEx(bScriptManager.LoadedScript.ScriptName);
            // TODO UI rework needed
            // Building blocks --
            // currentExecutor.CurrentContextIdentifier;
            // currentExecutor.CurrentStackPointer;
            // currentExecutor.ExecutionStackDepth;
            // currentExecutor.GetInterpreterStatus;
            innerPtr.Content = currentExecutor.currentInnerPointer;
            outerPtr.Content = currentExecutor.currentPointer;
            currentFunc.Content = currentExecutor.CurrentCode;
            currentFuncParsed.Content = currentExecutor.CurrentCodeParsed;
        }

        private void TranslationManagerBase_OnI18NEnvironmentChanged(Common.Interfaces.II18N.II18NConvertor obj)
        {
            foreach(var i in Utils.FindVisualChildren<ContentControl>(this))
            {
                if (i.Tag == null) continue;
                i.Content = obj.ConvertTo(SupportedPages.CORE_FUNC, i.Tag as string);
            }
            Title = obj.ConvertTo(SupportedPages.CORE_FUNC, Tag as string);
            loaded = obj.ConvertTo(SupportedPages.CORE_FUNC, "CORE_BSLOADER_LOADED");
            unsupportControlerT = obj.ConvertTo(SupportedPages.CORE_FUNC, "CORE_BSLOADER_FUNC_CTRLLER_UNLOAD_TITLE");
            unsupportControlerC = obj.ConvertTo(SupportedPages.CORE_FUNC, "CORE_BSLOADER_FUNC_CTRLLER_UNLOAD_CONTENT");
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
                        bScriptManager.LoadScript(bScriptLocationStruc.DetailedDescription);
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
