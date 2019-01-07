using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    interface IDocWriter
    {
        void Write(string content, string path);
    }
}
