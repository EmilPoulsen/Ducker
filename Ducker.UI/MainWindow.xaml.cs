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
            this.pbStatus.Value = 50;

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
                System.Windows.MessageBox.Show(this, "Path not valid");
                return;
            }

            //Task.Run(() => RunDucker());
            RunDucker();
            Task.Run(() => {

                Thread.Sleep(1000);

                this.Dispatcher.Invoke(() => {
                    tblockStatus.Text = "";
                    pbStatus.Value = 0;
                });
            });

            //bw.RunWorkerAsync();
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

                throw;
            }
            
        }

        private void RunDucker()
        {
            IGhaReader reader = new RhinoHeadlessGhaReader();
            IDocGenerator docGen = new EmuMdDocGenerator();
            IDocWriter docWrite = new MarkDownDocWriter();
            _duckRunner.Run(reader, docGen, docWrite);
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
