using Lunalipse.Common;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.IConsole;
using Lunalipse.Core.Console;
using Lunalipse.Presentation.LpsWindow;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lunalipse.Windows
{
    /// <summary>
    /// Interaction logic for LpsConsole.xaml
    /// </summary>
    public partial class LpsConsole : LunalipseDialogue
    {
        bool isTaskComplete = false;
        ConsoleAdapter adapter;
        public LpsConsole()
        {
            InitializeComponent();
            adapter = ConsoleAdapter.Instance;
            adapter.OnEnvironmentChanged += Adapter_OnEnvironmentChanged;
            adapter.RequireMother();
            CommandInput.AddHandler(KeyDownEvent, new KeyEventHandler(CommandInput_KeyDown), true);
        }

        private void Adapter_OnEnvironmentChanged(ConsoleEnvironment obj)
        {
            obj.TaskComplete = LunaConsole_OnTaskCompleted;
            DataContext = obj;
            Prompt.Content = obj.GetPromptFormated();
        }

        private void LunaConsole_OnTaskCompleted()
        {
            isTaskComplete = true;
            InputArea.Visibility = Visibility.Visible;
        }

        protected override void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            base.ThemeManagerBase_OnThemeApplying(obj);
        }

        private void CommandInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                contextScroll.ScrollToBottom();
                InputArea.Visibility = Visibility.Hidden;
                isTaskComplete = false;
                adapter.runCommand(CommandInput.Text);
                CommandInput.Text = "";
            }
            else if(e.Key == Key.Up)
            {
                CommandInput.Text = ((ConsoleEnvironment)DataContext).HistoryNavigateBackward();
            }
            else if(e.Key == Key.Down)
            {
                CommandInput.Text = ((ConsoleEnvironment)DataContext).HistoryNavigateForward();
            }
            CommandInput.CaretIndex = CommandInput.Text.Length;
        }

        private void LpsConsoleWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CommandInput.Focus();
        }
    }
}
