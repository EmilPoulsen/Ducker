using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    public class DuckerComponent
    {
        public DuckerComponent()
        {
            this.Input = new List<DuckerParam>();
            this.Output = new List<DuckerParam>();
        }

        public List<DuckerParam> Input { get; set; }
        public List<DuckerParam> Output { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
