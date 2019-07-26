using LunalipseInstaller.Procedure;
using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;

namespace LunalipseInstaller.Pages
{
    /// <summary>
    /// InstallProcess.xaml 的交互逻辑
    /// </summary>
    public partial class InstallProcess : Page, IPresentPage
    {
        ProcedureManager procedureManager;
        int exitCodeDotNetIns = 0;
        Thread thread;

        string targetPath;

        public InstallProcess(string installPath)
        {
            InitializeComponent();

            targetPath = installPath;
            procedureManager = new ProcedureManager(ProgressReceiver);

            InstallIndicator.MaximumValue = 1;

            procedureManager.AddProcedure(new ResolveCLRCompatibility());
            procedureManager.AddProcedure(new ExtractLunalipse(targetPath));

            Loaded += InstallProcess_Loaded;

            InstallIndicator.TrackBackgroundBrush = new SolidColorBrush(Color.FromArgb(125, 15, 2, 58));
            InstallIndicator.ProgressBackgroundBrush = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255));

        }

        private void InstallProcess_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InstallIndicator.Wait();
            thread = new Thread(new ThreadStart(ExecutionProcedureSequence));
            thread.Start();
        }

        void ExecutionProcedureSequence()
        {
            IEnumerator<IProcedure> enumerator = procedureManager.GetProcedures();
            while (enumerator.MoveNext())
            {
                IProcedure procedure = enumerator.Current;
                procedure.Main();
                if (procedure.GetModuleName() == "ResolveCLRCompatibility")
                {
                    exitCodeDotNetIns = (int)procedure.GetResult();
                }
            }
            Dispatcher.Invoke(new Action(() =>
            {
                MainWindow.SwitchPage(new InstallCompleted(exitCodeDotNetIns, targetPath));
            }));
        }

        void ProgressReceiver(string message,string currentTaskDetailed, double currentProgress)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (!DetailedMsg.Content.Equals(currentTaskDetailed)) DetailedMsg.Content = currentTaskDetailed;
                if (!Message.Content.Equals(message)) Message.Content = message;
                if(currentProgress == -1)
                {
                    InstallIndicator.Wait();
                }
                else
                {
                    InstallIndicator.CurrentValue = currentProgress;
                }
            }));
        }

        public void setWidth(double width)
        {
            this.Width = width;
        }


    }
}
