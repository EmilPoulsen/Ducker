using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Ducker.Core;

namespace Ducker
{
    class Program
    {
        static bool initialized = false;
        static string rhinoSystemDir = null;
        static string grasshopperSystemDir = null;

        public static void AssemblyInitialize()
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
                        duckerComponent.Output.Add(CreateDuckerParam(parameter));
                    }
                    Console.WriteLine(string.Format("Successfully read {0}", duckerComponent.Name));
                    duckers.Add(duckerComponent);
                }
            }

            string content = CreateMarkdown(duckers);
            string pathToOutput = @"C:\Users\epoulsen\Documents\GitHub\Emu\Emu\Emu.Grasshopper\bin\Release\Emu.Grasshopper.md";
            WriteFile(content, pathToOutput);
            ExitInProcess();
        }

        public static void WriteFile(string content, string path)
        {
            try
            {
                File.WriteAllText(path, content);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static string CreateMarkdown(List<DuckerComponent> components)
        {
            //StringBuilder
            StringBuilder builder = new StringBuilder();

            foreach (var component in components)
            {
                builder.Append(Header(component.Name));
                builder.Append(Paragraph(Bold(nameof(component.Name) + ":") + " " + component.Name));
                builder.Append(Paragraph(Bold(nameof(component.NickName) + ":") + " " + component.NickName));
                builder.Append(Paragraph(Bold(nameof(component.Description) + ":") + " " + component.Description));
                builder.Append(Environment.NewLine);

                if (component.Input.Count > 0) {
                    builder.Append(Header(nameof(component.Input), 3));
                    string table = GenerateParamTable(component.Input);
                    builder.Append(table);
                }
                if (component.Output.Count > 0)
                {
                    builder.Append(Header(nameof(component.Output), 3));
                    string table = GenerateParamTable(component.Output);
                    builder.Append(table);
                }
            }

            return builder.ToString();
        }

        private static string GenerateParamTable(List<DuckerParam> compParameter)
        {
            StringBuilder builder = new StringBuilder();
            var p = compParameter[0];
            string header = GetParamHeader(p);
            string splitter = "| ------ | ------ | ------ |";
            builder.AppendLine(header);
            builder.AppendLine(splitter);
            foreach (var parameter in compParameter)
            {
                string row = GetParamRow(parameter);
                builder.AppendLine(row);
            }
            return builder.ToString();

        }

        private static string GetParamRow(DuckerParam p)
        {
            string header = string.Format("| {0} | {1} | {2} |", p.Name, p.NickName, p.Description);
            return header;
        }

        private static string GetParamHeader(DuckerParam p)
        {
            string header = string.Format("| {0} | {1} | {2} |", nameof(p.Name), nameof(p.NickName), nameof(p.Description));
            return header;
        }

        public static string Bold(string text)
        {
            return "**" + text + "**";
        }

        public static string Paragraph(string text)
        {
            return text + "  " + Environment.NewLine;
        }

        public static string Header(string text, int level)
        {
            string hashes = new string('#', level) + " ";
            return hashes + text + Environment.NewLine; ;
        }

        public static string Header(string text)
        {
            return Header(text, 1);
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
        
        private static bool IsDerivedFromGhComponent(Type type)
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

        [DllImport("RhinoLibrary.dll")]
        internal static extern int LaunchInProcess(int reserved1, int reserved2);

        [DllImport("RhinoLibrary.dll")]
        internal static extern int ExitInProcess();


        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
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
    }
}
