using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LunalipseInstaller.Pages
{
    /// <summary>
    /// InstallCompleted.xaml 的交互逻辑
    /// </summary>
    public partial class InstallCompleted : Page, IPresentPage
    {
        int exitcode = 0;
        string targetPath;
        public InstallCompleted(int exitcode,string targetPath)
        {
            InitializeComponent();
            this.exitcode = exitcode;
            this.targetPath = targetPath;
            if(exitcode==0)
            {
                InstallationStatus.Content = "Lunalipse已完成安装";
                OperationButton.Content = "运行Lunalipse";
                Completed.Content = "完成";
            }
            else if (exitcode == 1641 || exitcode == 3010)
            {
                InstallationStatus.Content = "安装几乎完成，需要重启电脑以完成安装";
                OperationButton.Content = "立即重启";
                Completed.Content = "稍后重启";
            }
            else
            {
                InstallationStatus.Content = "安装完成，但未能安装.NET Framework，请重新运行本程序或尝试手动安装";
                OperationButton.Content = "手动安装.NET Framework";
                Completed.Content = "完成";
            }
        }

        public void setWidth(double width)
        {
            this.Width = width;
        }

        private void Completed_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void OperationButton_Click(object sender, RoutedEventArgs e)
        {
            if (exitcode == 0)
            {
                Process.Start(targetPath + @"\Lunalipse.exe");
            }
            else if(exitcode == 1641 || exitcode==3010)
            {
                Process.Start("shutdown -r -f -t 0");
            }
            else
            {
                Process.Start("https://www.microsoft.com/en-us/download/details.aspx?id=30653");
            }
            Environment.Exit(0);
        }
    }
}
