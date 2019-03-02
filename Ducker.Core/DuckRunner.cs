using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    public class DuckRunner
    {
        public DuckRunner()
        {
            this.AssemblyPath = string.Empty;
        }

        public DuckRunner(string path)
        {
            this.AssemblyPath = path;
        }

        public string AssemblyPath { get; set; }

        public void Run(IGhaReader reader, IDocGenerator docGen, IDocWriter docWrite)
        {
            OnProgress("Extracting..", 0);
            var duckers = reader.Read(this.AssemblyPath);
            OnProgress("Creating document..", 33);
            var content = docGen.Create(duckers);
            OnProgress("Saving document..", 66); 
            string pathToOutput = CreateOutputfile(this.AssemblyPath, docGen.FileExtension);
            docWrite.Write(content, pathToOutput);
            OnProgress("Done..", 100);
        }

        private string CreateOutputfile(string pathToDll, string extension)
        {
            string combined = Path.ChangeExtension(pathToDll, extension);
            return combined;
        }

        /// <summary>
        /// Event triggered when the visibility settings of the document is updated.
        /// </summary>
        public event EventHandler<ProgressEventArgs> Progress;

        protected virtual void OnProgress(string message, double percentage)
        {
            ProgressEventArgs e = new ProgressEventArgs(message, percentage);
            EventHandler<ProgressEventArgs> handler = Progress;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }

    /// <summary>
    /// Event arguments for events when progress is made.
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProgressEventArgs(string message, double percentage)
        {
            this.Message = message;
            this.Progress = percentage;            
        }
        
        public string Message { get; set; }
        public double Progress { get; set; }

    }
}
