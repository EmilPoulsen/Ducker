using System.Collections.Generic;
using System.Drawing;

namespace Ducker.Core
{
    /// <summary>
    /// Mirror object of the Grasshopper component.
    /// </summary>
    public class DuckerComponent
    {
        public DuckerComponent()
        {
            this.Input = new List<DuckerParam>();
            this.Output = new List<DuckerParam>();
        }

        /// <summary>
        /// The input parameters of the component.
        /// </summary>
        public List<DuckerParam> Input { get; set; }

        /// <summary>
        /// The output parameters of the component.
        /// </summary>
        public List<DuckerParam> Output { get; set; }

        /// <summary>
        /// The description of the component.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The name of the component.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The nick name of the component.
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// The icon of the component (png)
        /// </summary>
        public Bitmap Icon { get; set; }
        
        /// <summary>
        /// The exposure level of the component as string.
        /// </summary>
        public string Exposure { get; set; }

        /// <summary>
        /// Returns this.Name without any spaces. Used when generating file names.
        /// </summary>
        /// <returns>The name without any spaces.</returns>
        public string GetNameWithoutSpaces()
        {
            return this.Name.Replace(" ", string.Empty);
        }

        /// <summary>
        /// Returns this.Name
        /// </summary>
        /// <returns>The name of the component.</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
