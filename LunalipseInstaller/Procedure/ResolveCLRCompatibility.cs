using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace LunalipseInstaller.Procedure
{
    public class ResolveCLRCompatibility : IProcedure
    {
        const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
        const string dotnetPack = "LunalipseInstaller.InstallPack.dotNetFx45_Full_setup.exe";
        string dotNetInstaller = Path.GetTempPath() + "dotNetFx45_Full_setup.exe";

        int exitcode = 0;
        
        public void Main()
        {
            ProcedureManager.UpdateProgress("正在安装：检查框架版本", string.Empty, -1);
            if(!checkDotNetVersion_v45())
            {
                ProcedureManager.UpdateProgress("正在安装：检查框架版本", "安装受支持的.Net Framework版本", -1);
                ExtractDotnetInstaller();
                StartInstall();
            }
            Thread.Sleep(1000);
        }

        public bool checkDotNetVersion_v45()
        {
            using (var ndpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine,string.Empty).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    return (int)ndpKey.GetValue("Release") >= 378389;
                }
                else
                {
                    return false;
                }
            }
        }

        public void ExtractDotnetInstaller()
        {
            MessageBox.Show("检测到不支持的.Net Framwork版本。现在即将安装Lunalipse支持的.Net Framework版本，请确保网络连接正常且通畅", "安装.Net Framework", MessageBoxButton.OK, MessageBoxImage.Information);
            using (FileStream fs = new FileStream(dotNetInstaller, FileMode.Create))
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(dotnetPack))
                {
                    byte[] b = new byte[stream.Length];
                    stream.Read(b, 0, b.Length);
                    fs.Write(b, 0, b.Length);
                }
            }
        }
        public void StartInstall()
        {
            Process process = Process.Start(dotNetInstaller,"/passive /norestart");
            process.WaitForExit();
            exitcode = process.ExitCode;
        }

        public object GetResult()
        {
            return exitcode;
        }

        public string GetModuleName()
        {
            return this.GetType().Name;
        }
    }
}
