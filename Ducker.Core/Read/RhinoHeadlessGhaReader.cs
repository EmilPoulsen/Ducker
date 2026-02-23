using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Ducker.Core
{
    /// <summary>
    /// Reads the content of a gha using a background instance of rhino.
    /// </summary>
    public class RhinoHeadlessGhaReader : IGhaReader
    {
        private static bool _initialized = false;
        private static string _rhinoSystemDir = null;
        private static string _grasshopperSystemDir = null;

        /// <summary>
        /// Initialize the assembly.
        /// </summary>
        public void AssemblyInitialize()
        {
            if (_initialized)
            {
                return;
                //throw new InvalidOperationException("AssemblyInitialize should only be called once");
            }
            _initialized = true;

            // Ensure we are 64 bit
            if (!Environment.Is64BitProcess)
            {
                throw new Exception("Tests must be run as x64");
            }

            // Set path to rhino system directory
            string envPath = Environment.GetEnvironmentVariable("path");
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            _rhinoSystemDir = Path.Combine(programFiles, "Rhino 8", "System");
            _grasshopperSystemDir = Path.Combine(programFiles, "Rhino 8", "Plug-ins", "Grasshopper");

            if (!Directory.Exists(_rhinoSystemDir))
            {
                throw new DirectoryNotFoundException($"Rhino system dir not found: {_rhinoSystemDir}");
            }

            // Add rhino system directory to path (for RhinoLibrary.dll)
            Environment.SetEnvironmentVariable("path", envPath + ";" + _rhinoSystemDir);

            // Add hook for .Net assembly resolve (for RhinoCommon.dll)
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            // Start headless Rhino process
            _ = LaunchInProcess(0, 0);

        }
        
        public DuckerPlugin ReadPlugin(string pathToDll)
        {
            var dll = Assembly.LoadFile(pathToDll);
            
            // get assembly title attribute if available, otherwise use assembly name.
            var name = dll.GetCustomAttribute<AssemblyTitleAttribute>()?
                .Title ?? dll.GetName().Name;
            
            // get informational version attribute if available, otherwise use assembly version.
            var version = dll.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion ?? dll.GetName().Version?.ToString();
            
            // attempt to get description and copy right from assembly attributes.
            var description = dll.GetCustomAttribute<AssemblyDescriptionAttribute>()?
                .Description ?? string.Empty;
            var copyright = dll.GetCustomAttribute<AssemblyCopyrightAttribute>()?
                .Copyright ?? string.Empty;
            
            return new DuckerPlugin(name, version, description, copyright);
        }
        
        /// <summary>
        /// Read the dll using a RhinoInside instance.
        /// </summary>
        /// <param name="pathToDll">Path tp the</param>
        /// <returns>List of components included in the .gha file.</returns>
        public List<DuckerComponent> ReadComponents(string pathToDll)
        {
            var dll = Assembly.LoadFile(pathToDll);
            string folder = Path.GetDirectoryName(pathToDll) + @"\";
            AssemblyName[] asm = dll.GetReferencedAssemblies();
            List<Assembly> dependencies = new List<Assembly>();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            
            for (int i = 0; i < asm.Length; i++)
            {
                string path = folder + asm[i].Name + ".dll";
                if (File.Exists(path))
                {
                    Assembly dependency = Assembly.LoadFile(path);
                    dependencies.Add(dependency);
                }
            }

            List<DuckerComponent> duckers = new List<DuckerComponent>();

            foreach (Type type in dll.GetExportedTypes())
            {
                if (!IsDerivedFromGhComponent(type) || type.IsAbstract) continue;
                
                dynamic c = Activator.CreateInstance(type);
                if (c == null) continue;
                
                Bitmap icon = null;

                try
                {
                    icon = c.Icon_24x24;
                }
                catch (Exception)
                {
                    // ignored
                }

                DuckerComponent duckerComponent = new DuckerComponent()
                {
                    Name = c.Name,
                    NickName = c.NickName,
                    Description = c.Description,
                    Icon = icon,
                    Exposure = c.Exposure.ToString()
                };

                dynamic parameters = c.Params;
                foreach (var parameter in parameters.Input)
                {
                    duckerComponent.Input.Add(CreateDuckerParam(parameter));
                }

                foreach (var parameter in parameters.Output)
                {
                    duckerComponent.Output.Add(CreateDuckerParam(parameter));
                }
                Console.WriteLine($"Successfully read {duckerComponent.Name}");
                duckers.Add(duckerComponent);
            }
            ExitInProcess();
            
            return duckers;

        }

        /// <summary>
        /// Launch Rhino
        /// </summary>
        /// <param name="reserved1">0</param>
        /// <param name="reserved2">0</param>
        /// <returns></returns>
        [DllImport("RhinoLibrary.dll")]
        internal static extern int LaunchInProcess(int reserved1, int reserved2);

        /// <summary>
        /// Kill the running instance of Rhino
        /// </summary>
        /// <returns></returns>
        [DllImport("RhinoLibrary.dll")]
        internal static extern int ExitInProcess();

        /// <summary>
        /// Resolve Grasshopper and RhinoCommon references.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Args</param>
        /// <returns>The assembly if found.</returns>
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name;

            if (name.StartsWith("RhinoCommon"))
            {
                var path = Path.Combine(_rhinoSystemDir, "RhinoCommon.dll");
                return Assembly.LoadFrom(path);
            }

            if (name.StartsWith("Grasshopper"))
            {
                var path = Path.Combine(_grasshopperSystemDir, "Grasshopper.dll");
                return Assembly.LoadFrom(path);
            }

            // check for assemblies already loaded
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;

            return null;

        }

        /// <summary>
        /// Check if a type inherits from GH_Component.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns></returns>
        private static bool IsDerivedFromGhComponent(Type type)
        {
            //recursively walk through the type's inheritance tree.
            Type currType = type;
            while (currType != null)
            {
                if (currType.Name.Equals("GH_Component"))
                {
                    return true;
                }
                currType = currType.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Create a parameter object from the grasshopper param.
        /// </summary>
        /// <param name="parameter">Grasshopper param object.</param>
        /// <returns></returns>
        public static DuckerParam CreateDuckerParam(dynamic parameter)
        {
            DuckerParam duckerParam = new DuckerParam()
            {
                Name = parameter.Name,
                NickName = parameter.NickName,
                Description = parameter.Description
            };
            return duckerParam;
        }
    }
}
