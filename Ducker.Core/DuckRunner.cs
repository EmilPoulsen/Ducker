using System;
using System.IO;

namespace Ducker.Core
{
    /// <summary>
    /// Main execution logic turning the input GH into a text file.
    /// </summary>
    public class DuckRunner
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DuckRunner()
        {
            AssemblyPath = string.Empty;
        }

        /// <summary>
        /// Constructor accepting the path to the input .gha file.
        /// </summary>
        /// <param name="path">Full path to the input .gha file to parse.</param>
        public DuckRunner(string path)
        {
            AssemblyPath = path;
        }

        /// <summary>
        /// Full path to the input Grasshopper Assembly (gha) to parse.
        /// </summary>
        public string AssemblyPath { get; set; }

        /// <summary>
        /// Call this method to start the process.
        /// </summary>
        /// <param name="reader">Grasshopper assembly reader.</param>
        /// <param name="docGen">Document generator.</param>
        /// <param name="docWrite">Document writer.</param>
        public void Run(IGhaReader reader, IDocGenerator docGen, IDocWriter docWrite)
        {
            Run(reader, docGen, docWrite, ExportSettings.Default);
        }

        public void TryInitializeRhino(IGhaReader reader)
        {
            var rhino = reader as RhinoHeadlessGhaReader;
            OnProgress("Starting Rhino..", 0);
            rhino?.AssemblyInitialize();
        }
        
        /// <summary>
        /// Call this method to start the process.
        /// </summary>
        /// <param name="reader">Grasshopper assembly reader.</param>
        /// <param name="docGen">Document generator.</param>
        /// <param name="docWrite">Document writer.</param>
        /// <param name="settings">Settings to apply.</param>
        public void Run(IGhaReader reader, IDocGenerator docGen, IDocWriter docWrite, ExportSettings settings)
        {
            OnProgress("Extracting..", 15);
            var plugin = reader.ReadPlugin(this.AssemblyPath);
            var components = reader.ReadComponents(this.AssemblyPath);
            
            OnProgress("Creating document..", 33);
            var content = docGen.Create(plugin, components, settings);
            
            OnProgress("Saving document..", 66); 
            string pathToOutput = CreateOutputPath(this.AssemblyPath, docGen.FileExtension);
            docWrite.Write(content, pathToOutput);
            
            OnProgress("Done! Output folder is found next to input gha.", 100);
        }

        /// <summary>
        /// Creates the full path including the extension where the output file will be saved.
        /// </summary>
        /// <param name="pathToDll">Full path to the input dll.</param>
        /// <param name="extension">File extension of the output. For instance txt or md.</param>
        /// <returns>The path where the output file will be saved.</returns>
        private string CreateOutputPath(string pathToDll, string extension)
        {
            //Get file name, and change extension
            string fileName = Path.GetFileName(pathToDll);
            fileName = Path.ChangeExtension(fileName, extension);

            //Get directory, add subfolder "ducker".
            string directory = Path.GetDirectoryName(pathToDll);
            if (string.IsNullOrEmpty(directory)) throw new InvalidOperationException();
            string combined = Path.Combine(directory, "ducker", fileName);

            return combined;
        }

        /// <summary>
        /// Event triggered whenever the Runner makes progress.
        /// </summary>
        public event EventHandler<ProgressEventArgs> Progress;

        /// <summary>
        /// Call this method to raise the Progress event.
        /// </summary>
        /// <param name="message">The message of the progress.</param>
        /// <param name="percentage">Percentage describing how far the progress has gone (max 1.00).</param>
        protected virtual void OnProgress(string message, double percentage)
        {
            ProgressEventArgs e = new ProgressEventArgs(message, percentage);
            Progress?.Invoke(this, e);
        }

    }

    /// <summary>
    /// Event arguments for events when progress is made.
    /// </summary>
    /// <remarks>
    /// Default constructor.
    /// </remarks>
    public class ProgressEventArgs(string message, double percentage) : EventArgs
    {
        public string Message { get; set; } = message;
        public double Progress { get; set; } = percentage;

    }
}
