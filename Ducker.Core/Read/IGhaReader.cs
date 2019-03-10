using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    /// <summary>
    /// Interface for objects that can read the content of a .gha file.
    /// </summary>
    public interface IGhaReader
    {
        /// <summary>
        /// Triggers the read process.
        /// </summary>
        /// <param name="path">Path to the .gha file.</param>
        /// <returns>List of components included in the .gha file.</returns>
        List<DuckerComponent> Read(string path);          
    }
}
