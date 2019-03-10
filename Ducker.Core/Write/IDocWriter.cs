using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    /// <summary>
    /// Handles serializing the document content into an actual file. Can be as simple as
    /// saving the file to the harddrive or putting the contents in a db.
    /// </summary>
    public interface IDocWriter
    {
        /// <summary>
        /// Writes the document content to an output destination.
        /// </summary>
        /// <param name="content">The contents of the document.</param>
        /// <param name="path">The output destination.</param>
        void Write(DocumentContent content, string path);
    }
}
