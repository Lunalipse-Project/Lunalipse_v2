using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForm = System.Windows.Forms;

namespace UpdatePacker
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Packer packer = new Packer();

        public MainWindow()
        {
            InitializeComponent();
            FilesInfo.DataContext = packer;
            FilesInfo.ItemsSource = packer.WatchedFiles;
            packer.parseCache();
            SolutionPath.Content = packer.BaseDirectory;
            packer.MarkChanged();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch(button.Name)
            {
                case "ChooseDirectory":
                    if (packer.BaseDirectory == string.Empty)
                    {
                        WinForm.FolderBrowserDialog folderBrowserDialog = new WinForm.FolderBrowserDialog();
                        if(folderBrowserDialog.ShowDialog() == WinForm.DialogResult.OK)
                        {
                            packer.setFileRoot(folderBrowserDialog.SelectedPath);
                            SolutionPath.Content = packer.BaseDirectory;
                        }
                    }
                    break;
            }
        }

        private void FilesInfo_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(Directory.Exists("temp"))
            {
                Directory.Delete("temp", true);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ModifiedState operations = (ModifiedState)Enum.Parse(typeof(ModifiedState), item.Tag as string, true);
            packer.WatchedFiles[FilesInfo.SelectedIndex].State = operations;
        }

        private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            packer.WatchedFiles.RemoveAt(FilesInfo.SelectedIndex);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            packer.WatchedFiles.Clear();
            packer.setFileRoot(packer.BaseDirectory);
            packer.ExportTreeCache();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            switch((sender as Button).Tag as string)
            {
                case "Apply":
                    packer.ExportTreeCache();
                    break;
                case "Discard":
                    Close();
                    break;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            WinForm.FolderBrowserDialog folderBrowserDialog = new WinForm.FolderBrowserDialog();
            folderBrowserDialog.Description = "选择导出路径";
            if (folderBrowserDialog.ShowDialog() == WinForm.DialogResult.OK)
            {
                packer.PreparePack();
                packer.ExportUpdatePack(folderBrowserDialog.SelectedPath);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ContentViewer contentViewer = new ContentViewer(packer.ManifestContent);
            contentViewer.ShowDialog();
        }
    }
}
