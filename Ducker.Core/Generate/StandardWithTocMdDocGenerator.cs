using System;
using System.Collections.Generic;
using System.Text;

namespace Ducker.Core
{
    /// <summary>
    /// Generates a mark-down with Header Table of Contents.
    /// </summary>
    public class StandardWithTocMdDocGenerator : MarkDownDocGenerator
    {
        /// <summary>
        /// Creates the contents of the document based on components and the export settings
        /// </summary>
        /// <param name="plugin">The plugin details.</param>
        /// <param name="components">The components included in the gha.</param>
        /// <param name="settings">The output settings.</param>
        /// <returns>Content of the document.</returns>
        public override DocumentContent Create(DuckerPlugin plugin, List<DuckerComponent> components,
            ExportSettings settings)
        {
            DocumentContent docContent = new DocumentContent();
            StringBuilder builder = new StringBuilder();

            // Generate plugin header
            builder.AppendLine(Header($"{plugin.Name} ({plugin.Version})", 1));
            builder.AppendLine(Paragraph(Italic(plugin.Description)));
            builder.AppendLine(Paragraph(plugin.Copyright));
            
            // Generate components table of contents
            builder.AppendLine(Paragraph(Header("Components:", 2)));
            foreach (var component in components)
            {
                if (component.Exposure == "hidden" && settings.IgnoreHidden)
                    continue;

                builder.AppendLine(($"- {HeaderLink(component.Name)}"));
            }

            builder.Append(Environment.NewLine);

            // Generate each component
            foreach (var component in components)
            {
                if (component.Exposure == "hidden" && settings.IgnoreHidden)
                    continue;

                if (settings.ExportIcons)
                {
                    builder.AppendLine($"{Header(component.Name, 2)} {Image("",
                        docContent.RelativePathIcons, component.GetValidFileName())}");
                }
                else
                {
                    builder.AppendLine($"{Header(component.Name, 2)}");
                }

                if (settings.Name)
                {
                    builder.Append(Paragraph(Bold(nameof(component.Name) + ":") + " " + component.Name));
                }

                if (settings.NickName)
                {
                    builder.Append(Paragraph(Bold(nameof(component.NickName) + ":") + " " + component.NickName));
                }

                if (settings.Description)
                {
                    builder.Append(Paragraph(Bold(nameof(component.Description) + ":") + " " + component.Description));
                }
               
                builder.Append(Environment.NewLine);

                if (settings.Parameters & component.Input.Count > 0)
                {
                    builder.AppendLine(Header(nameof(component.Input), 3));
                    string table = GenerateParamTable(component.Input);
                    builder.Append(table);
                }

                builder.Append(Environment.NewLine);

                if (settings.Parameters & component.Output.Count > 0)
                {
                    builder.AppendLine(Header(nameof(component.Output), 3));
                    string table = GenerateParamTable(component.Output);
                    builder.Append(table);
                }

                builder.Append(Environment.NewLine);
            }

            docContent.Document = builder.ToString();

            if (settings.ExportIcons)
            {
                docContent.Icons = ReadIcons(components);
            }
            
            return docContent;
        }
    }
}
