using System.Collections.Generic;

namespace Ducker.Core
{
    /// <summary>
    /// Interface for objects that turns ducker components to a readable document.
    /// </summary>
    public interface IDocGenerator
    {
        /// <summary>
        /// Creates the contents of the document based on components and the export settings
        /// </summary>
        /// <param name="plugin">The plugin general details.</param>
        /// <param name="components">The components included in the gha.</param>
        /// <param name="settings">The output settings.</param>
        /// <returns>Content of the document.</returns>
        DocumentContent Create(DuckerPlugin plugin, List<DuckerComponent> components, ExportSettings settings);

        /// <summary>
        /// Creates the contents of the document based on components. Uses default settings.
        /// </summary>
        /// <param name="plugin">The plugin general details.</param>
        /// <param name="components">The components included in the gha.</param>
        /// <returns>Content of the document.</returns>
        DocumentContent Create(DuckerPlugin plugin, List<DuckerComponent> components);

        /// <summary>
        /// File extension of the output file. For instance txt or md.
        /// </summary>
        string FileExtension { get; }
    }
}
