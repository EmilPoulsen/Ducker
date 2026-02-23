
namespace Ducker.Core
{
    /// <summary>
    /// Mirror object of the grasshopper parameter. Each component has a 
    /// list of input params and output params.
    /// </summary>
    public class DuckerParam
    {
        /// <summary>
        /// Description of the parameter.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The nickname of the parameter.
        /// </summary>
        public string NickName { get; init; }

        /// <summary>
        /// Returns this.Name
        /// </summary>
        /// <returns>The name of the parameter.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
