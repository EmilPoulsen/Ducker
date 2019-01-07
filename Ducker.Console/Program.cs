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

        static void Main(string[] args)
        {
            string pathToDll = @"C:\Users\epoulsen\Documents\GitHub\Emu\Emu\Emu.Grasshopper\bin\Release\Emu.Grasshopper.gha";
            
            IGhaReader reader = new RhinoHeadlessGhaReader();
            IDocGenerator docGen = new EmuMdDocGenerator();
            IDocWriter docWrite = new MarkDownDocWriter();

            DuckRunner ducker = new DuckRunner(pathToDll);
            ducker.Run(reader, docGen, docWrite);



        }
    }
}
