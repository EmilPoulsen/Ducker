using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    public interface IDocGenerator
    {
        string Create(List<DuckerComponent> components);
    }
}
