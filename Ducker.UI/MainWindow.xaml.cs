using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Ducker.Core;
using System.ComponentModel;
using System.Threading;

namespace Ducker.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int InsertBindingHere { get; set; } = 50;

        private DuckRunner _duckRunner;
        private BackgroundWorker bw;

        public MainWindow()
        {
            InitializeComponent();
            double scale = 0.3;
            this.Width = IPhoneXWidth(scale);
            this.Height = 600;//IPhoneXHeigh(scale);
            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Title = string.Format("Ducker {0}", assemblyVersion);
            cmbColors.ItemsSource = typeof(Colors).GetProperties();
            this.pbStatus.Value = 0;

            _duckRunner = new DuckRunner();
            _duckRunner.Progress += DuckRunner_Progress;
            SetupBw();
            
        }

        private void DuckRunner_Progress(object sender, ProgressEventArgs e)
        {
            this.Dispatcher.Invoke(() => {
                this.pbStatus.Value = e.Progress;
                this.tblockStatus.Text = e.Message;
            });
        }

        private void SetupBw()
        {
            //Background Worker code///
            this.bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.DoWork += Bw_DoWork;
            bw.ProgressChanged += Bw_ProgressChanged;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
        }

        private int IPhoneXWidth(double scale)
        {
            double width =  1125 * scale;
            return (int) width;
        }

        private int IPhoneXHeigh(double scale)
        {
            double height = 2436 * scale;
            return (int)height;
        }


        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {

            if(!IsPathValid(_duckRunner.AssemblyPath))
            {
                ShowMessageBox("Path not valid");
                return;
            }

            RunDucker();
            //SimpleDelegate d = new SimpleDelegate(RunDucker);
            //d.BeginInvoke(null,null);

            //RunDucker();
            //bw.RunWorkerAsync();
            Task.Run(() => {
                //Thread.Sleep(1000);
                //this.Dispatcher.Invoke(() => {
                    //RunDucker();
                //});
            });
        }

        public delegate void SimpleDelegate();


        private void ShowMessageBox(string message)
        {
            System.Windows.MessageBox.Show(this, message);
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                RunDucker();
            }
            catch (Exception ex)
            {
                ShowMessageBox($"Fatal error: {ex.Message}{Environment.NewLine}Ducker will terminate." );
                return;
            }
            
        }

        private ExportSettings CollectOptions()
        {
            ExportSettings s = new ExportSettings();
            this.Dispatcher.Invoke(() => {
                s.Description = this.cbxDescription.IsChecked.Value;
                s.ExportIcons = this.cbxExportIcons.IsChecked.Value;
                s.IgnoreHidden = this.cbxIgnoreHidden.IsChecked.Value;
                s.Name = this.cbxName.IsChecked.Value;
                s.NickName = this.cbxNickName.IsChecked.Value;
                s.Parameters = this.cbxParameters.IsChecked.Value;
            });
            return s;
        }

        private void RunDucker()
        {
            ExportSettings settings = CollectOptions();
            IGhaReader reader = new RhinoHeadlessGhaReader();
            IDocGenerator docGen = new EmuMdDocGenerator();
            IDocWriter docWrite = new MarkDownDocWriter();

            _duckRunner.TryInitializeRhino(reader);

            Task.Run(() => {
                _duckRunner.Run(reader, docGen, docWrite);
                Thread.Sleep(1000);
                DuckRunner_Progress(this, new ProgressEventArgs("", 0));
            });
        }

        private bool IsPathValid(string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        private void btnSetPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Grasshopper assemblies|*.gha"; ;

            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                tbGhaPath.Text = path;
                _duckRunner.AssemblyPath = path;
            }

            // do something
        }
    }
}
