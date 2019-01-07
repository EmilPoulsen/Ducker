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
        private string pathToDll;

        public DuckRunner(string path)
        {
            this.pathToDll = path;

        }

        public void Run(IGhaReader reader, IDocGenerator docGen, IDocWriter docWrite)
        {
            var duckers = reader.Read(pathToDll);
            var content = docGen.Create(duckers);
            string pathToOutput = CreateOutputfile(pathToDll, docGen.FileExtension);
            docWrite.Write(content, pathToOutput);
        }

        private string CreateOutputfile(string pathToDll, string extension)
        {
            string combined = Path.ChangeExtension(pathToDll, extension);
            return combined;
        }
    }
}
