using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="components">The components included in the gha.</param>
        /// <returns>Content of the document.</returns>
        public DocumentContent Create(List<DuckerComponent> components)
        {
            return this.Create(components, ExportSettings.Default);
        }

        protected string GenerateParamTable(List<DuckerParam> compParameter)
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
        protected string GetParamRow(DuckerParam p)
        {
            string header = string.Format("| {0} | {1} | {2} |", p.Name, p.NickName, p.Description);
            return header;
        }

        /// <summary>
        /// Returns a row representing the headers in the param table.
        /// </summary>
        /// <param name="p">The parameter to generate a row from.</param>
        /// <returns>Formatted .md row string.</returns>
        protected string GetParamHeader(DuckerParam p)
        {
            string header = string.Format("| {0} | {1} | {2} |", nameof(p.Name), nameof(p.NickName), nameof(p.Description));
            return header;
        }

        /// <summary>
        /// Make a piece of text bold.
        /// </summary>
        /// <param name="text">Text to make bold.</param>
        /// <returns>Bold formatted text.</returns>
        protected string Bold(string text)
        {
            return "**" + text + "**";
        }
        
        /// <summary>
        /// Put some text in a paragraph.
        /// </summary>
        /// <param name="text">Text to put in paragraph.</param>
        /// <returns>Paragraph formatted text.</returns>
        protected string Paragraph(string text)
        {
            return text + "  " + Environment.NewLine;
        }

        /// <summary>
        /// Turn a string into a header.
        /// </summary>
        /// <param name="text">Text to make header.</param>
        /// <param name="level">Level of header. 1 = max header.</param>
        /// <returns>Header formatted text.</returns>
        protected string Header(string text, int level)
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
        /// Create the text needed to place an image in the markdown file.
        /// </summary>
        /// <param name="caption">Caption of the picture.</param>
        /// <param name="relativePath">Relative path to the image.</param>
        /// <param name="fileName">File name of the image.</param>
        /// <returns>Markdown text to generate image.</returns>
        protected string Image(string caption, string relativePath, string fileName)
        {
            return string.Format("![{0}]({1}/{2}.png)", caption, relativePath, fileName);
        }

        /// <summary>
        /// Reades the icons of components.
        /// </summary>
        /// <param name="components">Components.</param>
        /// <returns>Synced list of icons.</returns>
        protected List<Bitmap> ReadIcons(List<DuckerComponent> components)
        {
            components.ForEach(c => {
                if(c.Icon != null)
                {
                    c.Icon.Tag = c.GetNameWithoutSpaces();                    
                }
            });
            return components.Select(c => c.Icon).ToList();
        }

        /// <summary>
        /// Creates the contents of the document based on components and the export settings
        /// </summary>
        /// <param name="components">The components included in the gha.</param>
        /// <param name="settings">The output settings.</param>
        /// <returns>Content of the document.</returns>
        public abstract DocumentContent Create(List<DuckerComponent> components, ExportSettings settings);
    }
}
