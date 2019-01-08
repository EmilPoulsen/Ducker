using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    public class DocumentContent
    {
        public DocumentContent()
        {
            this.Icons = new List<Bitmap>();
            this.RelativePathIcons = "img";
        }

        public string Document { get; set; }

        public List<Bitmap> Icons { get; set; }

        public string RelativePathIcons { get; set; }
    }
}
