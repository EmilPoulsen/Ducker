using System.Collections.Generic;

namespace Ducker.Core
{
    /// <summary>
    /// Interface for objects that can read the content of a .gha file.
    /// </summary>
    public interface IGhaReader
    {
        
        DuckerPlugin ReadPlugin(string path);
        
        /// <summary>
        /// Triggers the read of components.
        /// </summary>
        /// <param name="path">Path to the .gha file.</param>
        /// <returns>List of components included in the .gha file.</returns>
        List<DuckerComponent> ReadComponents(string path);          
    }
}
    