using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    public class DuckerParam
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
