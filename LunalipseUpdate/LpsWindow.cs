using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LunalipseUpdate
{
    public class LpsWindow : Window
    {
        public LpsWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowState = WindowState.Normal;
            this.Style = (Style)Application.Current.Resources["LpsWindowFace"];
        }
    }
}
