using System;
using System.Reflection;
using System.Windows;

namespace LunalipseInstaller
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            LoadAsm();
            string[] temp = this.GetType().Assembly.GetManifestResourceNames();
            MainWindow window = new MainWindow();
            window.Show();
        }

        void LoadAsm()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                //LunalipseInstaller.lib.Lunalipse.Resource.dll   ---> lib/Lunalipse.Resource.dll
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));
                if (resource == null) return null;
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    byte[] assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
        }
    }
}
