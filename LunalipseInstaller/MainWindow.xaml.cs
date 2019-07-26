using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using LunalipseInstaller.Pages;

namespace LunalipseInstaller
{


    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public static event Action<IPresentPage> OnPageShowNext;

        DoubleAnimation FadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(700)));
        DoubleAnimation FadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(700)));

        Storyboard OpeningIntro;
        DoubleAnimation LunalipseIntro, InstallationIntro, LogoIntro, OperationAreaIntro;
        ThicknessAnimation VersionIntro;
        public MainWindow()
        {         
            InitializeComponent();
            LogoIntro = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(1.5)));
            LogoIntro.BeginTime = TimeSpan.FromSeconds(1);

            LunalipseIntro = new DoubleAnimation(0, 310, new Duration(TimeSpan.FromSeconds(1.5)));
            LunalipseIntro.BeginTime = TimeSpan.FromSeconds(2.5);
            LunalipseIntro.AccelerationRatio = 0.4;
            LunalipseIntro.DecelerationRatio = 0.5;

            VersionIntro = new ThicknessAnimation(new Thickness(0), new Duration(TimeSpan.FromMilliseconds(500)));
            VersionIntro.BeginTime = TimeSpan.FromSeconds(4.1);

            OperationAreaIntro = new DoubleAnimation(0, 180, new Duration(TimeSpan.FromMilliseconds(500)));
            OperationAreaIntro.AccelerationRatio = 0.4;
            OperationAreaIntro.DecelerationRatio = 0.5;
            OperationAreaIntro.BeginTime = TimeSpan.FromSeconds(4.7);

            Storyboard.SetTarget(LogoIntro, Logo);
            Storyboard.SetTargetProperty(LogoIntro, new PropertyPath("(Viewbox.Opacity)"));
            Storyboard.SetTarget(LunalipseIntro, Lunalipse);
            Storyboard.SetTargetProperty(LunalipseIntro, new PropertyPath("(Label.Width)"));
            Storyboard.SetTarget(VersionIntro, Version);
            Storyboard.SetTargetProperty(VersionIntro, new PropertyPath("(Label.Padding)"));
            Storyboard.SetTarget(OperationAreaIntro, Operation);
            Storyboard.SetTargetProperty(OperationAreaIntro, new PropertyPath("(Frame.Height)"));

            OpeningIntro = new Storyboard();
            OpeningIntro.Children.Add(LogoIntro);
            OpeningIntro.Children.Add(LunalipseIntro);
            OpeningIntro.Children.Add(VersionIntro);
            OpeningIntro.Children.Add(OperationAreaIntro);

            OpeningIntro.Completed += (sender, arg) =>
            {
                BtnClose.BeginAnimation(OpacityProperty, FadeIn);
                SwitchPage(new SetupPath());
            };

            OnPageShowNext += MainWindow_OnPageShowNext;

            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void MainWindow_OnPageShowNext(IPresentPage obj)
        {
            obj.setWidth(Operation.ActualWidth);
            if(Operation.Content!=null)
            {
                Operation.BeginAnimation(OpacityProperty, FadeOut);
            }
            Operation.Content = obj;
            Operation.BeginAnimation(OpacityProperty, FadeIn);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }
            }
            catch(InvalidOperationException)
            {

            }
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        public static void SwitchPage(IPresentPage page)
        {
            OnPageShowNext?.Invoke(page);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpeningIntro.Begin();
        }
    }
}
