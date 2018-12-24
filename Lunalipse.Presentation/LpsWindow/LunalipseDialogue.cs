using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.ITheme;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lunalipse.Presentation.LpsWindow
{
    public class LunalipseDialogue : Window
    {
        const string UI_COMPONENT_THEME_UID = "PR_WND_LunalipseDialogue";

        Border TITLE_BAR;
        public LunalipseDialogue()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Style = (Style)Application.Current.Resources["LunalipseDialogue"];
            Loaded += DialogueLoaded;

            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());

        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            Background = obj.Primary.SetOpacity(1);
            Foreground = obj.Foreground;
            BorderBrush = (Background as SolidColorBrush).ToLuna();
        }

        protected virtual void DialogueLoaded(object sender, EventArgs args)
        {
            ControlTemplate ct = (ControlTemplate)Application.Current.Resources["LunalipseDialogueBaseTemplate"];
            (TITLE_BAR = ct.FindName("TitleBar", this) as Border).MouseDown += TitleBarMove;
            (ct.FindName("DialogueClose", this) as Button).Click += ClosePressed;
        }

        protected void TitleBarMove(object sender, EventArgs args)
        {
            this.DragMove();
        }

        protected void ClosePressed(object sender, EventArgs args)
        {
            this.Close();
        }
    }
}
