using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    public interface IDocGenerator
    {
        DocumentContent Create(List<DuckerComponent> components);

        string FileExtension { get; }
    }

    public class DocumentContent
    {
        public DocumentContent()
        {
            this.Icons = new List<Bitmap>();
        }

        public string Document { get; set; }

        public List<Bitmap> Icons { get; set; }
    }
}
