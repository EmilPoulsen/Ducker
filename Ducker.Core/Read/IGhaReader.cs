using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    public interface IGhaReader
    {
        List<DuckerComponent> Read(string path);          
    }
}
