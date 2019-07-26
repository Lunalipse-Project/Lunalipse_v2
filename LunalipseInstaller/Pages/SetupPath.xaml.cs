using System;
using System.IO;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using WINFORM = System.Windows.Forms;

namespace LunalipseInstaller.Pages
{
    /// <summary>
    /// Welcome.xaml 的交互逻辑
    /// </summary>
    public partial class SetupPath : Page, IPresentPage
    {
        Storyboard showSequential;
        DoubleAnimation FirstRowAnim, SecondRowAnim;

        string defaultPath_x32 = "C:\\Program Files (x86)\\Lunalipse Music Player";
        string defaultPath_x64 = "C:\\Program Files\\Lunalipse Music Player";

        string selectedDirectory;
        public SetupPath()
        {
            InitializeComponent();
            FirstRowAnim = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(400)));
            SecondRowAnim = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(400)));
            showSequential = new Storyboard();
            SecondRowAnim.BeginTime = TimeSpan.FromMilliseconds(410);

            Storyboard.SetTarget(FirstRowAnim, FirstRow);
            Storyboard.SetTargetProperty(FirstRowAnim,new PropertyPath("(Grid.Opacity)"));
            Storyboard.SetTarget(SecondRowAnim, SecondRow);
            Storyboard.SetTargetProperty(SecondRowAnim, new PropertyPath("(Grid.Opacity)"));

            showSequential.Children.Add(FirstRowAnim);
            showSequential.Children.Add(SecondRowAnim);

            InstallPath.Text = (selectedDirectory = defaultPath_x64);
        }

        public void setWidth(double width)
        {
            this.Width = width;
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            WINFORM.FolderBrowserDialog browserDialog = new WINFORM.FolderBrowserDialog();
            browserDialog.Description = "选择Lunalipse的安装位置";
            browserDialog.SelectedPath = selectedDirectory;
            if (browserDialog.ShowDialog() == WINFORM.DialogResult.OK)
            {
                InstallPath.Text = selectedDirectory = (browserDialog.SelectedPath + @"\Lunalipse Music Player");
            }
            
        }

        private void Install_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(selectedDirectory)) Directory.CreateDirectory(selectedDirectory);
            ClearDirectorySecurity();
            MainWindow.SwitchPage(new InstallProcess(selectedDirectory));
        }

        void ClearDirectorySecurity()
        {
            FileSystemAccessRule alluser = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
            DirectoryInfo directoryInfo = new DirectoryInfo(selectedDirectory);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
            directorySecurity.SetAccessRule(alluser);
            directoryInfo.SetAccessControl(directorySecurity);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            showSequential.Begin();
        }
    }
}
