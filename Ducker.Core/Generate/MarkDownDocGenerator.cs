using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Ducker.Core
{
    /// <summary>
    /// Base class for all sorts of mark down generators, regardless of format.
    /// Contains alot of handy helper methods for formatting.
    /// </summary>
    public abstract class MarkDownDocGenerator : IDocGenerator
    {
        /// <summary>
        /// Returns the markdown extension 'md'.
        /// </summary>
        public string FileExtension
        {
            get
            {
                return "md";
            }
        }

        /// <summary>
        /// Creates a markdown file based on the provided components. Uses default settings.
        /// </summary>
        /// <param name="plugin">The plugin that the components belong to.</param>
        /// <param name="components">The components included in the gha.</param>
        /// <returns>Content of the document.</returns>
        public DocumentContent Create(DuckerPlugin plugin, List<DuckerComponent> components)
        {
            return Create(plugin, components, ExportSettings.Default);
        }

        protected static string GenerateParamTable(List<DuckerParam> compParameter)
        {
            StringBuilder builder = new StringBuilder();
            var p = compParameter[0];
            string header = GetParamHeader(p);
            string splitter = "| ------ | ------ | ------ |";
            builder.AppendLine(header);
            builder.AppendLine(splitter);
            foreach (var parameter in compParameter)
            {
                string row = GetParamRow(parameter);
                builder.AppendLine(row);
            }
            return builder.ToString();

        }

        /// <summary>
        /// Returns a table row representing a parameter.
        /// </summary>
        /// <param name="p">The parameter to generate a row from.</param>
        /// <returns>Formatted .md row string.</returns>
        protected static string GetParamRow(DuckerParam p)
        {
            string header = string.Format("| {0} | {1} | {2} |", p.Name, p.NickName, p.Description);
            return header;
        }

        /// <summary>
        /// Returns a row representing the headers in the param table.
        /// </summary>
        /// <param name="p">The parameter to generate a row from.</param>
        /// <returns>Formatted .md row string.</returns>
        protected static string GetParamHeader(DuckerParam p)
        {
            string header = string.Format("| {0} | {1} | {2} |", nameof(p.Name), nameof(p.NickName), nameof(p.Description));
            return header;
        }

        /// <summary>
        /// Make a piece of text bold.
        /// </summary>
        /// <param name="text">Text to make bold.</param>
        /// <returns>Bold formatted text.</returns>
        protected static string Bold(string text)
        {
            return "**" + text + "**";
        }
        
        /// <summary>
        /// Make a piece of text italic.
        /// </summary>
        /// <param name="text">Text to make italic.</param>
        /// <returns>Italic formatted text.</returns>
        protected static string Italic(string text)
        {
            return "*" + text + "*";
        }
        
        /// <summary>
        /// Creates a divider.
        /// </summary>
        /// <returns>Returns a divider.</returns>
        protected static string Divider()
        {
            return "---";
        }
        
        /// <summary>
        /// Put some text in a paragraph.
        /// </summary>
        /// <param name="text">Text to put in paragraph.</param>
        /// <returns>Paragraph formatted text.</returns>
        protected static string Paragraph(string text)
        {
            return text + "  " + Environment.NewLine;
        }

        /// <summary>
        /// Turn a string into a header.
        /// </summary>
        /// <param name="text">Text to make header.</param>
        /// <param name="level">Level of header. 1 = max header.</param>
        /// <returns>Header formatted text.</returns>
        protected static string Header(string text, int level)
        {
            string hashes = new string('#', level) + " ";
            return hashes + text;
        }
        
        /// <summary>
        /// Generate level 1 formatted header.
        /// </summary>
        /// <param name="text">Text to make header.</param>
        /// <returns>Header formatted text.</returns>
        protected string Header(string text)
        {
            return Header(text, 1);
        }
        
        /// <summary>
        /// Turn a string into a header link.
        /// </summary>
        /// <param name="text">Text to make header.</param>
        /// <returns>Header formatted text.</returns>
        protected static string HeaderLink(string text)
        {
            return "[" + text + "](#" + text.ToLower().Replace(" ", "-") + ")";
        }
        
        /// <summary>
        /// Create the text needed to place an image in the markdown file.
        /// </summary>
        /// <param name="caption">Caption of the picture.</param>
        /// <param name="relativePath">Relative path to the image.</param>
        /// <param name="fileName">File name of the image.</param>
        /// <returns>Markdown text to generate image.</returns>
        protected static string Image(string caption, string relativePath, string fileName)
        {
            return string.Format("![{0}]({1}/{2}.png)", caption, relativePath, fileName);
        }

        /// <summary>
        /// Reads the icons of components.
        /// </summary>
        /// <param name="components">Components.</param>
        /// <returns>Synced list of icons.</returns>
        protected static List<Bitmap> ReadIcons(List<DuckerComponent> components)
        {

            List<Bitmap> icons = new List<Bitmap>();
            foreach (var component in components)
            {
                if (component.Icon is null) continue;

                Bitmap clonedBmp = BitmapDeepClone(component.Icon);

                if (clonedBmp == null) continue;
                clonedBmp.Tag = component.GetValidFileName();

                icons.Add(clonedBmp);
            }

            return icons;
        }

        /// <summary>
        /// Clones a bitmap deeply, creating a new instance with its own pixel data.
        /// (Prevents issues with shared icon references.)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private static Bitmap BitmapDeepClone(Bitmap original)
        {
            if (original == null)
            {
                return null;
            }

            Bitmap copy = new Bitmap(original.Width, original.Height, original.PixelFormat);

            using (Graphics graphics = Graphics.FromImage(copy))
            {
                Rectangle imageRectangle = new Rectangle(0, 0, original.Width, original.Height);
                graphics.DrawImage(original, imageRectangle, imageRectangle, GraphicsUnit.Pixel);
            }

            copy.Tag = original.Tag;

            return copy;
        }
        
        /// <summary>
        /// Creates the contents of the document based on components and the export settings
        /// </summary>
        /// <param name="plugin">The plugin that the components belong to.</param>
        /// <param name="components">The components included in the gha.</param>
        /// <param name="settings">The output settings.</param>
        /// <returns>Content of the document.</returns>
        public abstract DocumentContent Create(DuckerPlugin plugin, List<DuckerComponent> components, ExportSettings settings);
    }
}
