﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ducker
{
    class Program
    {
        static bool initialized = false;
        static string systemDir = null;

        public static void AssemblyInitialize()
        {
            if (initialized)
            {
                throw new InvalidOperationException("AssemblyInitialize should only be called once");
            }
            initialized = true;

            // Ensure we are 64 bit
            if (!Environment.Is64BitProcess) {
                throw new Exception("Tests must be run as x64");
            } 

            // Set path to rhino system directory
            string envPath = Environment.GetEnvironmentVariable("path");
             string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            systemDir = System.IO.Path.Combine(programFiles, "Rhino WIP", "System");

            if (!System.IO.Directory.Exists(systemDir))
            {
                throw new Exception(string.Format("Rhino system dir not found: {0}", systemDir));
            }
            
            // Add rhino system directory to path (for RhinoLibrary.dll)
            Environment.SetEnvironmentVariable("path", envPath + ";" + systemDir);

            // Add hook for .Net assmbly resolve (for RhinoCommmon.dll)
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            // Start headless Rhino process
            LaunchInProcess(0, 0);

        }

        static void Main(string[] args)
        {
            AssemblyInitialize();

            string pathToDll = @"C:\Users\epoulsen\Documents\GitHub\Emu\Emu\Emu.Grasshopper\bin\Release\Emu.Grasshopper.gha";
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
                        duckerComponent.Input.Add(CreateDuckerParam(parameter));
                    }
                    Console.WriteLine(string.Format("Successfully read {0}", duckerComponent.Name));
                    duckers.Add(duckerComponent);
                }
            }
            ExitInProcess();
        }  

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

        public class DuckerParam
        {
            public string Description { get; set; }
            public string Name { get; set; }
            public string NickName { get; set; }

            public override string ToString()
            {
                return this.Name;
            }
        }

        public class DuckerComponent
        {
            public DuckerComponent()
            {
                this.Input = new List<DuckerParam>();
                this.Output = new List<DuckerParam>();
            }

            public List<DuckerParam> Input { get; set; }
            public List<DuckerParam> Output { get; set; }
            public string Description { get; set; }
            public string Name { get; set; }
            public string NickName { get; set; }

            public override string ToString()
            {
                return this.Name;
            }
        }

        private static bool IsDerivedFromGhComponent(Type type)
        {
            Type currType = type;

            while(currType != null)
            {
                if (currType.Name.Equals("GH_Component"))
                {
                    return true;
                }
                currType = currType.BaseType;
            }
            return false;
        }

        [DllImport("RhinoLibrary.dll")]
        internal static extern int LaunchInProcess(int reserved1, int reserved2);

        [DllImport("RhinoLibrary.dll")]
        internal static extern int ExitInProcess();


        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name;

            if (name.StartsWith("RhinoCommon"))
            {
                var path = System.IO.Path.Combine(systemDir, "RhinoCommon.dll");
                return Assembly.LoadFrom(path);
            }

            if (name.StartsWith("Grasshopper"))
            {

            }

            // Ignore missing resources
            if (args.Name.Contains(".resources"))
                return null;

            // check for assemblies already loaded
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;

            // Try to load by filename - split out the filename of the full assembly name
            // and append the base path of the original assembly (ie. look in the same dir)
            string filename = args.Name.Split(',')[0] + ".dll".ToLower();

            string asmFile = Path.Combine(@".\", "Addins", filename);

            try
            {
                return System.Reflection.Assembly.LoadFrom(asmFile);
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
