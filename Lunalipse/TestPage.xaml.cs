using Lunalipse.Presentation.LpsWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lunalipse
{
    /// <summary>
    /// TestPage.xaml 的交互逻辑
    /// </summary>
    public partial class TestPage : LunalipseDialogue
    {
        public TestPage()
        {
            InitializeComponent();
            HintL.ContentLabel = "Test Hint lable";
            HintL.ContentHint = "This is the test Hint lable descriptor";
        }
    }
}
