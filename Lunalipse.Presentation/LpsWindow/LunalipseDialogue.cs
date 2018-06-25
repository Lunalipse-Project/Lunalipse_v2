using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.Themes;
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
    public class LunalipseDialogue : Window, IThemeCustomizable
    {
        const string UI_COMPONENT_THEME_UID = "UICOMP_LPS_DAILOGUE";

        Border TITLE_BAR;
        public LunalipseDialogue()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Style = (Style)Application.Current.Resources["LunalipseDialogue"];
            Loaded += DialogueLoaded;
        }

        protected virtual void DialogueLoaded(object sender, EventArgs args)
        {
            ControlTemplate ct = (ControlTemplate)Application.Current.Resources["LunalipseDialogueBaseTemplate"];
            (TITLE_BAR = ct.FindName("TitleBar", this) as Border).MouseDown += TitleBarMove;
            (ct.FindName("BtnClose", this) as Ellipse).MouseDown += ClosePressed;
        }

        protected void TitleBarMove(object sender, EventArgs args)
        {
            this.DragMove();
        }

        protected void ClosePressed(object sender, EventArgs args)
        {
            this.Close();
        }

        public virtual void ThemeOverriding(ThemeTuple themeTuple)
        {
            throw new NotImplementedException();
        }

        public string ComponentUID()
        {
            return UI_COMPONENT_THEME_UID;
        }
    }
}
