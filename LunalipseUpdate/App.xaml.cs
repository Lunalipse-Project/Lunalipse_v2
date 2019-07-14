using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LunalipseUpdate
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string[] args = e.Args;
            if (args != null || args.Length > 0)
            {
                string url = "", version = "";
                for(int i = 0; i < args.Length - 1;)
                {
                    switch(args[i])
                    {
                        case "-s:u":
                            url = args[i + 1];
                            break;
                        case "-s:v":
                            version = args[i + 1];
                            break;
                    }
                    i += 2;
                }
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    MainWindow mainwindow = new MainWindow(url,version);
                    mainwindow.Show();
                }
            }
        }
    }
}
