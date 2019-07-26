using IWshRuntimeLibrary;
using Lunalipse.Resource;
using Lunalipse.Resource.Generic.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace LunalipseInstaller.Procedure
{
    public class ExtractLunalipse : IProcedure
    {
        const string InstallPack = "LunalipseInstaller.InstallPack.package_Lunalipse.clrss";

        string targetPath;

        public ExtractLunalipse(string targetPath)
        {
            this.targetPath = targetPath;
        }
        public string GetModuleName()
        {
            return GetType().Name;
        }

        public object GetResult()
        {
            return null;
        }

        public void Main()
        {
            ProcedureManager.UpdateProgress("正在安装：安装Lunalipse", string.Empty, 0);
            Thread.Sleep(100);
            LrssReader lrssReader = new LrssReader();
            lrssReader.LoadLrssRawCompressed(readInstallPackage());
            List<LrssIndex> indices = lrssReader.GetIndex();
            double total = indices.Count;
            double current = 1.0;
            foreach (LrssIndex index in indices)
            {
                LrssResource resource = lrssReader.ReadResource(index);
                ProcedureManager.UpdateProgress("正在安装：安装Lunalipse", "提取：" + resource.Name + resource.Type, current / total);
                using (FileStream fileStream = new FileStream(targetPath + "\\" + resource.Name + resource.Type, FileMode.Create)) 
                {
                    fileStream.Write(resource.Data, 0, resource.Data.Length);
                }
                current++;
            }
            CreateShortcut("Lunalipse", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), targetPath + "\\Lunalipse.exe");
        }

        byte[] readInstallPackage()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(InstallPack))
            {
                byte[] b = new byte[stream.Length];
                stream.Read(b, 0, b.Length);
                return b;
            }
        }

        public void CreateShortcut(string shortcutName, string shortcutPath, string targetFileLocation)
        {
            string shortcutLocation = System.IO.Path.Combine(shortcutPath, shortcutName + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = "Lunalipse Music Player";   // The description of the shortcut
            shortcut.IconLocation = targetFileLocation;           // The icon of the shortcut
            shortcut.TargetPath = targetFileLocation;                 // The path of the file that will launch when the shortcut is run
            shortcut.Save();                                    // Save the shortcut
        }
    }
}
