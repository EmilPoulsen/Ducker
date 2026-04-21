using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Ducker.Core
{
    /// <summary>
    /// Attempts to generate a PDF via Chrome/Edge headless.
    /// Falls back to opening the HTML in the default browser (which auto-triggers the print
    /// dialog) and throws <see cref="PdfFallbackException"/> to notify the caller.
    /// </summary>
    public class PdfDocWriter : IDocWriter
    {
        private static readonly string[] _browserPaths =
        [
            // Chrome
            @"C:\Program Files\Google\Chrome\Application\chrome.exe",
            @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
            // Edge
            @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
            @"C:\Program Files\Microsoft\Edge\Application\msedge.exe",
        ];

        /// <inheritdoc/>
        public void Write(DocumentContent docContent, string pdfPath)
        {
            string dir = Path.GetDirectoryName(pdfPath);
            if (string.IsNullOrEmpty(dir)) throw new DirectoryNotFoundException($"Invalid output path: {pdfPath}");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // Always write the HTML file — used either by headless or by the browser fallback.
            string htmlPath = Path.ChangeExtension(pdfPath, "html");
            File.WriteAllText(htmlPath, docContent.Document);
            SaveIcons(docContent, htmlPath);

            string browserExe = FindBrowser();

            if (browserExe != null)
            {
                RunHeadlessPrint(browserExe, htmlPath, pdfPath);
            }
            else
            {
                OpenInDefaultBrowser(htmlPath);
                throw new PdfFallbackException(htmlPath);
            }
        }

        /// <summary>
        /// Runs the browser in headless mode and prints the HTML file to PDF.
        /// </summary>
        private static void RunHeadlessPrint(string browserExe, string htmlPath, string pdfPath)
        {
            string args = $"--headless=new --disable-gpu --print-to-pdf-no-header-footer --print-to-pdf=\"{pdfPath}\" \"{htmlPath}\"";
            using Process process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = browserExe,
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = false,
            };

            process.Start();
            process.WaitForExit();
        }

        /// <summary>
        /// Opens an HTML file using the system default browser.
        /// </summary>
        private static void OpenInDefaultBrowser(string htmlPath)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = htmlPath,
                UseShellExecute = true,
            });
        }

        /// <summary>
        /// Returns the path of the first supported browser found on this machine, or null.
        /// </summary>
        private static string FindBrowser()
        {
            foreach (string path in _browserPaths)
            {
                if (File.Exists(path))
                    return path;
            }
            return null;
        }

        /// <summary>
        /// Saves component icons as PNG files in the icons subfolder next to the output file.
        /// </summary>
        private static void SaveIcons(DocumentContent docContent, string htmlPath)
        {
            List<Bitmap> icons = docContent.Icons;
            if (icons == null || icons.Count == 0) return;

            string dir = Path.GetDirectoryName(htmlPath);
            if (string.IsNullOrEmpty(dir)) throw new DirectoryNotFoundException($"Invalid output path: {htmlPath}");

            string iconsDir = Path.Combine(dir, docContent.RelativePathIcons);
            if (!Directory.Exists(iconsDir))
                Directory.CreateDirectory(iconsDir);

            foreach (var icon in icons)
            {
                if (icon == null) continue;
                string name = icon.Tag as string;
                string fileName = Path.Combine(iconsDir, name + ".png");
                icon.Save(fileName, ImageFormat.Png);
            }
        }
    }
}
