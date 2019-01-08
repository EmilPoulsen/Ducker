using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    public class MarkDownDocWriter : IDocWriter
    {
        public void Write(DocumentContent docContent, string path)
        {
            try
            {
                File.WriteAllText(path, docContent.Document);
                SaveIcons(docContent, path);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void SaveIcons(DocumentContent docContent, string path)
        {
            List<Bitmap> icons = docContent.Icons;
            var copy = new List<Bitmap>(icons);
            copy.RemoveAll(i => i == null);
            path = Path.GetDirectoryName(path);
            path = Path.Combine(path, docContent.RelativePathIcons);
            Directory.CreateDirectory(path);

            foreach (var icon in copy)
            {
                string name = icon.Tag as string;
                string fileName = Path.Combine(path, name + ".png");
                icon.Save(fileName, ImageFormat.Png);
            }

                                 
        }
    }
}
