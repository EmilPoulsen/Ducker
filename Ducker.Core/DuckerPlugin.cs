
namespace Ducker.Core
{
    /// <summary>
    /// Mirror object of the Grasshopper Plugin
    /// </summary>
    public class DuckerPlugin
    {
        public DuckerPlugin(string name, string version, string description = "", string copyright = "")
        {
            Name = name;
            Version = version;
            Description = description;
            Copyright = copyright;
        }
        
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Version of the plugin.
        /// </summary>
        public string Version { get; }
        
        /// <summary>
        /// Description of the plugin.
        /// </summary>
        public string Description { get; }
        
        /// <summary>
        /// Copyright of the plugin.
        /// </summary>
        public string Copyright{ get; }

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
