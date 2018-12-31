using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;

namespace Ducker
{
    public class TestComponent : GH_Component
    {
        public override Guid ComponentGuid
        {
            get
            {
                return Guid.NewGuid();
            }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
    
        }
    }

    class Program
    {


        static void Main(string[] args)
        {

            TestComponent component = new TestComponent();

            //string pathToDll = @"C:\Users\epoulsen\Documents\GitHub\Emu\Emu\Emu.Grasshopper\bin\Release\Emu.Grasshopper.gha";
            //var DLL = Assembly.LoadFile(pathToDll);
            //string folder = Path.GetDirectoryName(pathToDll) + @"\";

            //AssemblyName[] asm = DLL.GetReferencedAssemblies();
            //List<Assembly> dependencies = new List<Assembly>();

            //TestComponent component = new TestComponent();


            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            //for (int i = 0; i < asm.Length; i++)
            //{
            //    string path = folder + asm[i].Name + ".dll";
            //    if (File.Exists(path))
            //    {
            //        Assembly dependency = Assembly.LoadFile(path);
            //        dependencies.Add(dependency);
            //    }
            //    //paths.Add(path);

            //}



            //foreach (Type type in DLL.GetExportedTypes())
            //{
            //    //var c = Activator.CreateInstance(type);
            //    //type.InvokeMember("Output", BindingFlags.InvokeMethod, null, c, new object[] { @"Hello" });
            //}

        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Ignore missing resources
            if (args.Name.Contains(".resources"))
                return null;

            Assembly[] loaded = AppDomain.CurrentDomain.GetAssemblies();
            // check for assemblies already loaded
            Assembly assembly = loaded.FirstOrDefault(a => a.FullName == args.Name);
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
