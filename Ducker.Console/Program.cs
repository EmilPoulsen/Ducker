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
            //TODO. https://github.com/EmilPoulsen/Ducker/issues/1 
            // The idea here was to create a simple console app, instead of the
            // full blown WPF app, so that Ducker can be included in automation workflows.
            // It never got there, but please feel free to give it a go if you read this :)

            string pathToDll = @"C:\Users\emil\Documents\GitHub\Emu\Emu\Emu.Grasshopper\bin\Release\Emu.Grasshopper.gha";
            
            IGhaReader reader = new RhinoHeadlessGhaReader();
            IDocGenerator docGen = new StandardMdDocGenerator();
            IDocWriter docWrite = new MarkDownDocWriter();

            DuckRunner ducker = new DuckRunner(pathToDll);
            ducker.Run(reader, docGen, docWrite);
        }
    }
}
