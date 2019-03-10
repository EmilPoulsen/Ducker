using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    /// <summary>
    /// The contents of the document. This object is created by the generator and 
    /// contains everything you need to create presentable document.
    /// </summary>
    public class DocumentContent
    {
        /// <summary>
        /// Defualt constructor.
        /// </summary>
        public DocumentContent()
        {
            this.Icons = new List<Bitmap>();
            this.RelativePathIcons = "img";
        }

        /// <summary>
        /// The actual text based content of the main document. The format of this string
        /// will depend on the doc generator used.
        /// </summary>
        public string Document { get; set; }

        /// <summary>
        /// Component icons
        /// </summary>
        public List<Bitmap> Icons { get; set; }

        /// <summary>
        /// Relative path of to the icons. For instance
        /// </summary>
        public string RelativePathIcons { get; set; }
    }
}
