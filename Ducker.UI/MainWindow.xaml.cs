using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

using Microsoft.Win32;

using Ducker.Core;

namespace Ducker.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DuckRunner _duckRunner;
        
        public MainWindow()
        {
            InitializeComponent();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            string assemblyVersion = $"{version.Major}.{version.Minor}.{version.Build}";
            Title = string.Format(" Ducker {0} (beta)", assemblyVersion);
            PopulateComboBoxWithIDocGeneratorTypes();
            pbStatus.Value = 0;

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
            Dispatcher.Invoke(() => {
                pbStatus.Value = e.Progress;
                tblockStatus.Text = e.Message;
            });
        }

        /// <summary>
        /// Button that starts the whole process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsPathValid(_duckRunner.AssemblyPath))
                {
                    ShowMessageBox("Path not valid");
                    return;
                }

                await RunDucker();

            }
            catch (Exception x)
            {
                ShowMessageBox($"Something bad happened: {x.Message}");
                DuckRunner_Progress(this, new ProgressEventArgs("", 0));
            }
        }
        
        private void ShowMessageBox(string message)
        {
            MessageBox.Show(this, message);
        }

        private ExportSettings CollectOptions()
        {
            ExportSettings s = new ExportSettings();
            Dispatcher.Invoke(() => {
                s.Description = cbxDescription.IsChecked.Value;
                s.ExportIcons = cbxExportIcons.IsChecked.Value;
                s.IgnoreHidden = cbxIgnoreHidden.IsChecked.Value;
                s.Name = cbxName.IsChecked.Value;
                s.NickName = cbxNickName.IsChecked.Value;
                s.Parameters = cbxParameters.IsChecked.Value;
                s.DocWriter = cmbColors.SelectedItem as Type;
            });
            return s;
        }

        private async Task RunDucker()
        {
            ExportSettings settings = CollectOptions();
            IGhaReader reader = new RhinoHeadlessGhaReader();
            IDocGenerator docGen = Activator.CreateInstance(settings.DocWriter)
                as IDocGenerator;
            IDocWriter docWrite = new MarkDownDocWriter();

            DuckRunner_Progress(this, new ProgressEventArgs("Starting Rhino...", 0));

            await Task.Run(() =>
            {
                _duckRunner.TryInitializeRhino(reader);
            });

            await Task.Run(() =>
            {
                _duckRunner.Run(reader, docGen, docWrite, settings);
                Thread.Sleep(4000); //Show the complete msg in progress bar for 1 sec.
                DuckRunner_Progress(this, new ProgressEventArgs("", 0));
            });
        }

        private static bool IsPathValid(string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        private void BtnSetPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Grasshopper assemblies|*.gha";

            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                tbGhaPath.Text = path;
                _duckRunner.AssemblyPath = path;
            }
        }
    }
}
