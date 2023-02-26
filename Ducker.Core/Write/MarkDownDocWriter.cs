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
    /// <summary>
    /// Saves the document content to an .md file.
    /// </summary>
    public class MarkDownDocWriter : IDocWriter
    {
        /// <summary>
        /// Writes the document content to an output destination.
        /// </summary>
        /// <param name="docContent">The contents of the document.</param>
        /// <param name="path">The output destination.</param>
        public void Write(DocumentContent docContent, string path)
        {
            try
            {
                string dir = Path.GetDirectoryName(path);
                if(!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllText(path, docContent.Document);
                SaveIcons(docContent, path);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Saves the icons in the document to the specified path.
        /// </summary>
        /// <param name="docContent">The contents of the document.</param>
        /// <param name="path">The output destination.</param>
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
