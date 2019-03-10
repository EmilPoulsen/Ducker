using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    /// <summary>
    /// Generates a markdown with Emu style.
    /// </summary>
    public class EmuMdDocGenerator : MarkDownDocGenerator
    {
        /// <summary>
        /// Creates the contents of the document based on components and the export settings
        /// </summary>
        /// <param name="components">The components included in the gha.</param>
        /// <param name="settings">The output settings.</param>
        /// <returns>Content of the document.</returns>
        public override DocumentContent Create(List<DuckerComponent> components, ExportSettings settings)
        {
            DocumentContent docContent = new DocumentContent();
            StringBuilder builder = new StringBuilder();

            foreach (var component in components)
            {
                if (component.Exposure == "hidden" && settings.IgnoreHidden)
                    continue;

                builder.AppendLine(string.Format("{0} {1}", Header(component.Name), Image("",
                    docContent.RelativePathIcons, component.GetNameWithoutSpaces())));
                builder.Append(Paragraph(Bold(nameof(component.Name) + ":") + " " + component.Name));
                builder.Append(Paragraph(Bold(nameof(component.NickName) + ":") + " " + component.NickName));
                builder.Append(Paragraph(Bold(nameof(component.Description) + ":") + " " + component.Description));
                builder.Append(Environment.NewLine);

                if (component.Input.Count > 0)
                {
                    builder.AppendLine(Header(nameof(component.Input), 3));
                    string table = GenerateParamTable(component.Input);
                    builder.Append(table);
                }
                if (component.Output.Count > 0)
                {
                    builder.AppendLine(Header(nameof(component.Output), 3));
                    string table = GenerateParamTable(component.Output);
                    builder.Append(table);
                }
            }

            docContent.Document = builder.ToString();
            docContent.Icons = base.ReadIcons(components);
            return docContent;
        }
    }
}
