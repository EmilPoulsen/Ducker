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
        private DuckRunner _duckRunner;
        
        public MainWindow()
        {
            InitializeComponent();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            string assemblyVersion = $"{version.Major}.{version.Minor}.{version.Build}";
            this.Title = string.Format(" Ducker {0}", assemblyVersion);
            PopulateComboBoxWithIDocGeneratorTypes();
            this.pbStatus.Value = 0;

            _duckRunner = new DuckRunner();
            _duckRunner.Progress += DuckRunner_Progress;
        }

        /// <summary>
        /// Performs reflection of the loaded assemblies and finds all types
        /// that implements IDocWriter and add these to the CheckBox
        /// </summary>
        private void PopulateComboBoxWithIDocGeneratorTypes()
        {
            var types = GetIDocGeneratorTypes();
            cmbColors.ItemsSource = types;
            if (types.Count > 0)
            {
                cmbColors.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Performs reflection of the loaded assemblies and finds all types
        /// that implements IDocWriter
        /// </summary>
        private List<Type> GetIDocGeneratorTypes()
        {
            var type = typeof(IDocGenerator);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                .ToList();
            return types;
        }

        private void DuckRunner_Progress(object sender, ProgressEventArgs e)
        {
            this.Dispatcher.Invoke(() => {
                this.pbStatus.Value = e.Progress;
                this.tblockStatus.Text = e.Message;
            });
        }

        /// <summary>
        /// Button that starts the whole process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            if(!IsPathValid(_duckRunner.AssemblyPath))
            {
                ShowMessageBox("Path not valid");
                return;
            }

            Task.Run(() => {
            RunDucker();    
            });
        }
        
        private void ShowMessageBox(string message)
        {
            System.Windows.MessageBox.Show(this, message);
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
                s.DocWriter = this.cmbColors.SelectedItem as Type;
            });
            return s;
        }

        private void RunDucker()
        {
            ExportSettings settings = CollectOptions();
            IGhaReader reader = new RhinoHeadlessGhaReader();
            IDocGenerator docGen = Activator.CreateInstance(settings.DocWriter)
                as IDocGenerator;
            IDocWriter docWrite = new MarkDownDocWriter();

            DuckRunner_Progress(this, new ProgressEventArgs("Starting Rhino...", 0));

            this.Dispatcher.Invoke(() => {
                _duckRunner.TryInitializeRhino(reader);
            });
            
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
        }
    }
}
