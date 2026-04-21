using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Text;

namespace Ducker.Core
{
    /// <summary>
    /// Generates an HTML document from Grasshopper plugin components.
    /// </summary>
    public class HtmlDocGenerator : IDocGenerator
    {
        /// <inheritdoc/>
        public virtual string FileExtension => "html";

        /// <inheritdoc/>
        public virtual IDocWriter CreateWriter() => new HtmlDocWriter();

        /// <inheritdoc/>
        public DocumentContent Create(DuckerPlugin plugin, List<DuckerComponent> components)
            => Create(plugin, components, ExportSettings.Default);

        /// <inheritdoc/>
        public virtual DocumentContent Create(DuckerPlugin plugin, List<DuckerComponent> components, ExportSettings settings)
        {
            DocumentContent docContent = new DocumentContent();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"UTF-8\">");
            sb.AppendLine($"  <title>{WebUtility.HtmlEncode(plugin.Name)}</title>");
            sb.AppendLine("  <style>");
            sb.AppendLine("    body { font-family: sans-serif; max-width: 960px; margin: auto; padding: 2rem; color: #222; }");
            sb.AppendLine("    h1, h2, h3 { color: #333; }");
            sb.AppendLine("    h2 { border-bottom: 1px solid #ddd; padding-bottom: .3rem; }");
            sb.AppendLine("    table { border-collapse: collapse; width: 100%; margin-bottom: 1rem; }");
            sb.AppendLine("    th, td { border: 1px solid #ccc; padding: .4rem .7rem; text-align: left; }");
            sb.AppendLine("    th { background-color: #f4f4f4; }");
            sb.AppendLine("    img.icon { width: 24px; height: 24px; vertical-align: middle; margin-right: .4rem; }");
            sb.AppendLine("    .toc { background: #f9f9f9; padding: 1rem 2rem; border: 1px solid #eee; display: inline-block; margin-bottom: 2rem; }");
            sb.AppendLine("    .toc a { text-decoration: none; color: #0366d6; }");
            sb.AppendLine("    .toc a:hover { text-decoration: underline; }");
            sb.AppendLine("    hr { border: none; border-top: 1px solid #eee; margin: 2rem 0; }");
            sb.AppendLine("    .btn-print { position: fixed; bottom: 1.5rem; right: 1.5rem; padding: .6rem 1.2rem; background: #0366d6; color: #fff; border: none; border-radius: 4px; font-size: 1rem; cursor: pointer; box-shadow: 0 2px 6px rgba(0,0,0,.2); }");
            sb.AppendLine("    .btn-print:hover { background: #0056b3; }");
            sb.AppendLine("    @media print { .btn-print { display: none; } }");
            sb.AppendLine("  </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            // Plugin header
            sb.AppendLine($"<h1>{WebUtility.HtmlEncode(plugin.Name)} <small>({WebUtility.HtmlEncode(plugin.Version)})</small></h1>");
            if (!string.IsNullOrEmpty(plugin.Description))
                sb.AppendLine($"<p><em>{WebUtility.HtmlEncode(plugin.Description)}</em></p>");
            if (!string.IsNullOrEmpty(plugin.Copyright))
                sb.AppendLine($"<p>{WebUtility.HtmlEncode(plugin.Copyright)}</p>");

            // Table of contents
            sb.AppendLine("<div class=\"toc\">");
            sb.AppendLine("  <h2>Components</h2>");
            sb.AppendLine("  <ul>");
            foreach (var component in components)
            {
                if (component.Exposure == "hidden" && settings.IgnoreHidden)
                    continue;

                string anchor = GetAnchor(component.Name);
                sb.AppendLine($"    <li><a href=\"#{anchor}\">{WebUtility.HtmlEncode(component.Name)}</a></li>");
            }
            sb.AppendLine("  </ul>");
            sb.AppendLine("</div>");

            // Components
            foreach (var component in components)
            {
                if (component.Exposure == "hidden" && settings.IgnoreHidden)
                    continue;

                string anchor = GetAnchor(component.Name);
                sb.AppendLine($"<section id=\"{anchor}\">");
                sb.Append("  <h2>");
                if (settings.ExportIcons)
                    sb.Append($"<img class=\"icon\" src=\"{docContent.RelativePathIcons}/{component.GetValidFileName()}.png\" alt=\"icon\" />");
                sb.AppendLine($"{WebUtility.HtmlEncode(component.Name)}</h2>");

                if (settings.Name)
                    sb.AppendLine($"  <p><strong>Name:</strong> {WebUtility.HtmlEncode(component.Name)}</p>");
                if (settings.NickName)
                    sb.AppendLine($"  <p><strong>NickName:</strong> {WebUtility.HtmlEncode(component.NickName)}</p>");
                if (settings.Description)
                    sb.AppendLine($"  <p><strong>Description:</strong> {WebUtility.HtmlEncode(component.Description)}</p>");

                if (settings.Parameters && component.Input.Count > 0)
                {
                    sb.AppendLine("  <h3>Input</h3>");
                    sb.Append(GenerateParamTable(component.Input));
                }

                if (settings.Parameters && component.Output.Count > 0)
                {
                    sb.AppendLine("  <h3>Output</h3>");
                    sb.Append(GenerateParamTable(component.Output));
                }

                sb.AppendLine("</section>");
                sb.AppendLine("<hr/>");
            }

            sb.AppendLine("  <button class=\"btn-print\" onclick=\"window.print()\">🖨 Print / Save as PDF</button>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            docContent.Document = sb.ToString();
            if (settings.ExportIcons)
                docContent.Icons = ReadIcons(components);

            return docContent;
        }

        /// <summary>
        /// Generates an HTML table from a list of parameters.
        /// </summary>
        private static string GenerateParamTable(List<DuckerParam> parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("  <table>");
            sb.AppendLine("    <thead><tr><th>Name</th><th>NickName</th><th>Description</th></tr></thead>");
            sb.AppendLine("    <tbody>");
            foreach (var p in parameters)
            {
                sb.AppendLine($"      <tr><td>{WebUtility.HtmlEncode(p.Name)}</td><td>{WebUtility.HtmlEncode(p.NickName)}</td><td>{WebUtility.HtmlEncode(p.Description)}</td></tr>");
            }
            sb.AppendLine("    </tbody>");
            sb.AppendLine("  </table>");
            return sb.ToString();
        }

        /// <summary>
        /// Reads icons from the components, cloning each bitmap.
        /// </summary>
        private static List<Bitmap> ReadIcons(List<DuckerComponent> components)
        {
            var icons = new List<Bitmap>();
            foreach (var component in components)
            {
                if (component.Icon is null) continue;

                Bitmap clone = new Bitmap(component.Icon);
                clone.Tag = component.GetValidFileName();
                icons.Add(clone);
            }
            return icons;
        }

        /// <summary>
        /// Converts a component name into a valid HTML anchor id.
        /// </summary>
        private static string GetAnchor(string name)
            => name.ToLower().Replace(" ", "-");
    }
}
