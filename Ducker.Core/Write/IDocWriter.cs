using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    public interface IDocWriter
    {
        void Write(DocumentContent content, string path);
    }
}
