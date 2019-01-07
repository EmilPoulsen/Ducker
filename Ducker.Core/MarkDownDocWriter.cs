using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    public class MarkDownDocWriter : IDocWriter
    {
        public void Write(string content, string path)
        {
            try
            {
                File.WriteAllText(path, content);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
