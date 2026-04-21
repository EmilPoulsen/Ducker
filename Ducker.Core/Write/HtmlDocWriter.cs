using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;

namespace Ducker.Core
{
    /// <summary>
    /// Saves the document content to an .html file.
    /// </summary>
    public class HtmlDocWriter : IDocWriter
    {
        /// <inheritdoc/>
        public void Write(DocumentContent docContent, string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(dir)) throw new DirectoryNotFoundException($"Invalid output path: {path}");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(path, docContent.Document);
            SaveIcons(docContent, path);
        }

        /// <summary>
        /// Saves the component icons as PNG files next to the HTML output.
        /// </summary>
        private static void SaveIcons(DocumentContent docContent, string path)
        {
            if (docContent.Icons == null || docContent.Icons.Count == 0) return;

            string dir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(dir)) throw new DirectoryNotFoundException($"Invalid output path: {path}");

            string iconsDir = Path.Combine(dir, docContent.RelativePathIcons);
            if (!Directory.Exists(iconsDir))
                Directory.CreateDirectory(iconsDir);

            foreach (var icon in docContent.Icons)
            {
                if (icon == null) continue;

                string name = icon.Tag as string;
                string fileName = Path.Combine(iconsDir, name + ".png");
                icon.Save(fileName, ImageFormat.Png);
            }
        }
    }
}
