using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Ducker.Core
{
    /// <summary>
    /// Mirror object of the Grasshopper component.
    /// </summary>
    public class DuckerComponent
    {
        public DuckerComponent()
        {
            Input = new List<DuckerParam>();
            Output = new List<DuckerParam>();
        }

        /// <summary>
        /// The input parameters of the component.
        /// </summary>
        public List<DuckerParam> Input { get; set; }

        /// <summary>
        /// The output parameters of the component.
        /// </summary>
        public List<DuckerParam> Output { get; }

        /// <summary>
        /// The description of the component.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// The name of the component.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The nick name of the component.
        /// </summary>
        public string NickName { get; init; }

        /// <summary>
        /// The icon of the component (png)
        /// </summary>
        public Bitmap Icon { get; init; }
        
        /// <summary>
        /// The exposure level of the component as string.
        /// </summary>
        public string Exposure { get; init; }

        /// <summary>
        /// Returns a valid file name, used for the icon file of the component.
        /// </summary>
        /// <param name="replacementChar"></param>
        /// <returns></returns>
        public string GetValidFileName(char replacementChar = '_')
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            char[] ignoredChars = ['.', ' '];

            var sb = new StringBuilder();

            foreach (var c in Name)
            {
                if (invalidChars.Contains(c))
                {
                    sb.Append(replacementChar);
                }
                else if (ignoredChars.Contains((c)))
                {
                    sb.Append(string.Empty);
                }
                else
                {
                    sb.Append(c);
                }
            }
            // TODO Additional considerations:
            // 1. Ensure the file name is not empty or composed solely of dots.
            // 2. Truncate the name if it exceeds the maximum path length (often ~260 characters).
            // 3. Avoid using reserved names like "CON", "PRN", "AUX", etc.
            // The basic character replacement handles most cases.
            return sb.ToString();
        }

        /// <summary>
        /// Returns this.Name
        /// </summary>
        /// <returns>The name of the component.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
