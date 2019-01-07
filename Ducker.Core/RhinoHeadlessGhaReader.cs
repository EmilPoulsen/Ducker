using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    public class RhinoHeadlessGhaReader : IGhaReader
    {
        static bool initialized = false;
        static string rhinoSystemDir = null;
        static string grasshopperSystemDir = null;

        public void AssemblyInitialize()
        {
            if (initialized)
            {
                throw new InvalidOperationException("AssemblyInitialize should only be called once");
            }
            initialized = true;

            // Ensure we are 64 bit
            if (!Environment.Is64BitProcess)
            {
                throw new Exception("Tests must be run as x64");
            }

            // Set path to rhino system directory
            string envPath = Environment.GetEnvironmentVariable("path");
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            rhinoSystemDir = System.IO.Path.Combine(programFiles, "Rhino WIP", "System");
            grasshopperSystemDir = System.IO.Path.Combine(programFiles, "Rhino WIP", "Plug-ins", "Grasshopper");

            if (!System.IO.Directory.Exists(rhinoSystemDir))
            {
                throw new Exception(string.Format("Rhino system dir not found: {0}", rhinoSystemDir));
            }

            // Add rhino system directory to path (for RhinoLibrary.dll)
            Environment.SetEnvironmentVariable("path", envPath + ";" + rhinoSystemDir);

            // Add hook for .Net assmbly resolve (for RhinoCommmon.dll)
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            // Start headless Rhino process
            LaunchInProcess(0, 0);

        }

        public List<DuckerComponent> Read(string pathToDll)
        {
            AssemblyInitialize();

            var DLL = Assembly.LoadFile(pathToDll);
            string folder = Path.GetDirectoryName(pathToDll) + @"\";

            AssemblyName[] asm = DLL.GetReferencedAssemblies();
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
                //paths.Add(path);

            }

            List<DuckerComponent> duckers = new List<DuckerComponent>();

            foreach (Type type in DLL.GetExportedTypes())
            {

                if (IsDerivedFromGhComponent(type) && !type.IsAbstract)
                {
                    dynamic c = Activator.CreateInstance(type);

                    DuckerComponent duckerComponent = new DuckerComponent()
                    {
                        Name = c.Name,
                        NickName = c.NickName,
                        Description = c.Description
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
                    Console.WriteLine(string.Format("Successfully read {0}", duckerComponent.Name));
                    duckers.Add(duckerComponent);
                }
            }
            ExitInProcess();
            return duckers;

        }

        [DllImport("RhinoLibrary.dll")]
        internal static extern int LaunchInProcess(int reserved1, int reserved2);

        [DllImport("RhinoLibrary.dll")]
        internal static extern int ExitInProcess();


        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name;

            if (name.StartsWith("RhinoCommon"))
            {
                var path = System.IO.Path.Combine(rhinoSystemDir, "RhinoCommon.dll");
                return Assembly.LoadFrom(path);
            }

            if (name.StartsWith("Grasshopper"))
            {
                var path = System.IO.Path.Combine(grasshopperSystemDir, "Grasshopper.dll");
                return Assembly.LoadFrom(path);
            }

            // check for assemblies already loaded
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;

            return null;

        }

        private bool IsDerivedFromGhComponent(Type type)
        {
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

        public DuckerParam CreateDuckerParam(dynamic parameter)
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
