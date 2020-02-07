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
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.ICommunicator;
using Lunalipse.Presentation.LpsWindow;
using Lunalipse.Utilities;

namespace Lunalipse.Presentation.BasicUI
{
    /// <summary>
    /// ProgressDialogue.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressDialogue : LunalipseDialogue, IProgressIndicator
    {
        Action<IProgressIndicator> task;
        public ProgressDialogue(Action<IProgressIndicator> task)
        {
            InitializeComponent();
            Loaded += ProgressDialogue_Loaded;
            this.task = task;
        }

        private void ProgressDialogue_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() => task(this));
        }

        public void ChangeCurrentVal(double current, string message = null)
        {
            Dispatcher.Invoke(() =>
            {
                if (current >= 0)
                    Progress.CurrentValue = current;
                Message.Content = message;
            });
        }

        public void Complete()
        {
            Dispatcher.Invoke(() => this.Close());
        }

        public void SetRange(double min, double max)
        {
            Dispatcher.Invoke(() =>
            {
                Progress.MaximumValue = max;
                if (max <= 0)
                {
                    Progress.Wait();
                }
            });
        }

        protected override void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            base.ThemeManagerBase_OnThemeApplying(obj);
            Progress.TrackBackgroundBrush = obj.Primary.SetOpacity(0.8).ToLuna();
            Progress.ProgressBackgroundBrush = obj.Secondary;
        }

        

        public void UpdateCaption(string caption)
        {
            Dispatcher.Invoke(() => Title = $"Loading: {caption}");
        }
    }
}
