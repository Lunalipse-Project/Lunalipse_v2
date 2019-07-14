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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Threading;
using System.Windows.Shapes;

using LunalipseUpdate.Procedures;

namespace LunalipseUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : LpsWindow
    {
        DoubleAnimation r_animation;
        ProcedureHelper procedureHelper;
        Thread UpdationThread;

        public MainWindow(string url,string version)
        {
            InitializeComponent();
            procedureHelper = new ProcedureHelper(UpdateOnUIElements);
            r_animation = new DoubleAnimation(0, 720, new Duration(TimeSpan.FromHours(5)));
            Celestia_CM.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "Resource/celestia_cm.png")));
            this.Background = new LinearGradientBrush(new GradientStopCollection()
            {
                new GradientStop((Color)ColorConverter.ConvertFromString("#F5ADFF"),1),
                new GradientStop((Color)ColorConverter.ConvertFromString("#93B9FF"),0.5),
                new GradientStop((Color)ColorConverter.ConvertFromString("#64DCB7"),0),
            }, 45);           
            UpdateProgress.TrackBackgroundBrush = new SolidColorBrush(Color.FromArgb(25, 51, 51, 51));
            UpdateProgress.ProgressBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 252, 196, 140));
            UpdateProgress.MaximumValue = 1;

            procedureHelper.AddProcedure(new Checking());
            procedureHelper.AddProcedure(new Downloading(url));
            procedureHelper.AddProcedure(new Unpacking());
            procedureHelper.AddProcedure(new Applying());
            procedureHelper.AddProcedure(new CleanUp());

            r_animation.RepeatBehavior = RepeatBehavior.Forever;
            UpdateProgress.Wait();

            UpgradeHint.Content = string.Format("正在升级Lunalipse到版本{0}", version);
        }

        private void LpsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Celestia_CM.BeginAnimation(RotateTransform.AngleProperty, r_animation);
            UpdationThread = new Thread(new ThreadStart(DoUpdateProcedures));
            UpdationThread.Start();
        }

        private void DoUpdateProcedures()
        {
            IEnumerator<IProcedure> procedures = procedureHelper.GetProcedures();
            try
            {
                while (procedures.MoveNext())
                {
                    procedures.Current.Main();
                }
                ProcedureHelper.UpdateProgress("", 0);
                Dispatcher.Invoke(() => UpgradeHint.Content = "Lunalipse已完成升级");
                Thread.Sleep(3000);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            Environment.Exit(0);
        }

        private void UpdateOnUIElements(string currentTask, double currentProgressPercentage)
        {
            Dispatcher.Invoke(() =>
            {
                if(!ProcessDetail.Content.Equals(currentTask))
                {
                    ProcessDetail.Content = currentTask;
                }
                if (currentProgressPercentage != -1)
                {
                    UpdateProgress.CurrentValue = currentProgressPercentage;
                }
                else
                {
                    UpdateProgress.Wait();
                }
            });
        }
    }
}
