using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    public class ExportSettings
    {
        public bool IgnoreHidden { get; set; }
        public bool Name { get; set; }
        public bool Description { get; set; }
        public bool ExportIcons { get; set; }
        public bool NickName { get; set; }
        public bool Parameters { get; set; }

        public static ExportSettings Default
        {
            get
            {
                return new ExportSettings()
                {
                    Description = true,
                    ExportIcons = true,
                    IgnoreHidden = true,
                    Name = true,
                    NickName = true,
                    Parameters = true
                };
            }
        }
    }
}
