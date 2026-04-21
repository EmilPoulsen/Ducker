using System.Collections.Generic;

namespace Ducker.Core
{
    /// <summary>
    /// Generates an HTML document optimised for PDF printing.
    /// When opened in a browser, the print dialog is triggered automatically.
    /// If Chrome or Edge is available, the PDF is generated headlessly without user interaction.
    /// </summary>
    public class PdfDocGenerator : HtmlDocGenerator
    {
        /// <inheritdoc/>
        public override string FileExtension => "pdf";

        /// <inheritdoc/>
        public override IDocWriter CreateWriter() => new PdfDocWriter();

        /// <inheritdoc/>
        public override DocumentContent Create(DuckerPlugin plugin, List<DuckerComponent> components, ExportSettings settings)
        {
            DocumentContent docContent = base.Create(plugin, components, settings);

            // Inject print-specific CSS and auto-print script into the generated HTML.
            docContent.Document = docContent.Document
                .Replace("  </style>", PrintCss + "  </style>")
                .Replace("</body>", PrintScript + "\n</body>");

            return docContent;
        }

        /// <summary>
        /// CSS rules applied only during printing: hides ToC and expands layout to full width.
        /// </summary>
        private static string PrintCss =>
            """
                @media print {
                  .toc { display: none; }
                  body { max-width: 100%; padding: 0; }
                  section { page-break-inside: avoid; }
                }

            """;

        /// <summary>
        /// JavaScript that triggers the browser print dialog automatically on page load.
        /// Used as fallback when Chrome/Edge headless is not available.
        /// </summary>
        private static string PrintScript =>
            """
              <script>
                window.onload = function () { window.print(); };
              </script>
            """;
    }
}
