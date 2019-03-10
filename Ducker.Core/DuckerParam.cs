using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    /// <summary>
    /// Mirror object of the grasshopper parameter. Each component has a 
    /// list of input params and output params.
    /// </summary>
    public class DuckerParam
    {
        /// <summary>
        /// Description of the parameter.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The nickname of the parameter.
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// Returns this.Name
        /// </summary>
        /// <returns>The name of the parameter.</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
