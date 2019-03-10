using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    /// <summary>
    /// Object containing instructions about what of the component information 
    /// to include in the output file.
    /// </summary>
    public class ExportSettings
    {
        /// <summary>
        /// Ignore printing components with "hidden" exposure property.
        /// </summary>
        public bool IgnoreHidden { get; set; }

        /// <summary>
        /// Include the component name in the output file?
        /// </summary>
        public bool Name { get; set; }

        /// <summary>
        /// Include the component description in the output file?
        /// </summary>
        public bool Description { get; set; }

        /// <summary>
        /// Export and include the components' icons?
        /// </summary>
        public bool ExportIcons { get; set; }

        /// <summary>
        /// Include the component nickname in the output file?
        /// </summary>
        public bool NickName { get; set; }

        /// <summary>
        /// Include the component input/output parameter data in the output file?
        /// </summary>
        public bool Parameters { get; set; }

        /// <summary>
        /// Default settings: Description = true, ExportIcons = true, IgnoreHidden = true, 
        /// Name = true, NickName = true, Parameters = true
        /// </summary>
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
