using System;

namespace Ducker.Core
{
    /// <summary>
    /// Thrown by <see cref="PdfDocWriter"/> when Chrome/Edge headless is not available
    /// and the HTML file has been opened in the default browser as a fallback.
    /// </summary>
    public class PdfFallbackException : Exception
    {
        /// <summary>
        /// Full path to the HTML file that was opened in the browser.
        /// </summary>
        public string HtmlPath { get; }

        /// <inheritdoc/>
        public PdfFallbackException(string htmlPath)
            : base($"Chrome/Edge not found. The HTML file has been opened in your default browser: {htmlPath}")
        {
            HtmlPath = htmlPath;
        }
    }
}
