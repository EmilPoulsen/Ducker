using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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
        /// <exception cref="DirectoryNotFoundException">Thrown when the directory of the output path does not exist.</exception>
        public void Write(DocumentContent docContent, string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(dir)) throw new DirectoryNotFoundException($"Invalid output path: {path}");
            
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            File.WriteAllText(path, docContent.Document);
            SaveIcons(docContent, path);
        }

        /// <summary>
        /// Saves the icons in the document to the specified path.
        /// </summary>
        /// <param name="docContent">The contents of the document.</param>
        /// <param name="path">The output destination.</param>
        /// <exception cref="DirectoryNotFoundException">Thrown when the directory of the output path does not exist.</exception>
        private static void SaveIcons(DocumentContent docContent, string path)
        {
            List<Bitmap> icons = docContent.Icons;

            string dir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(dir)) throw new DirectoryNotFoundException($"Invalid output path: {path}");

            path = Path.Combine(dir, docContent.RelativePathIcons);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (var icon in icons)
            {
                if (icon == null) continue;
                
                string name = icon.Tag as string;
                string fileName = Path.Combine(path, name + ".png");
                
                icon.Save(fileName, ImageFormat.Png);
            }
            
        }
    }
}
