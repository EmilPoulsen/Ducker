using System;

using Ducker.Core;

namespace Ducker
{
    internal static class Program
    {

        static int Main(string[] args)
        {
            if (args.Length == 0 || args[0] == "--help" || args[0] == "-h")
            {
                PrintUsage();
                return 0;
            }

            string pathToDll = args[0];
            
            if (!System.IO.File.Exists(pathToDll))
            {
                Console.WriteLine($"An error has occured: File not found at {pathToDll}");
                return 1;
            }
            
            ExportSettings settings = ParseSettings(args);

            IGhaReader reader = new RhinoHeadlessGhaReader();
            IDocGenerator docGen = Activator.CreateInstance(settings.DocWriter) as IDocGenerator;
            IDocWriter docWrite = new MarkDownDocWriter();

            DuckRunner duckerRunner = new DuckRunner
            {
                AssemblyPath = pathToDll
            };
            
            try
            {
                duckerRunner.TryInitializeRhino(reader);
                duckerRunner.Run(reader, docGen, docWrite, settings);
            }
            catch (Exception e)
            {
                Console.WriteLine($"A fatal error has occured: {e.Message}");
                Console.WriteLine(e.StackTrace);
                return 1;
            }
            return 0;

        }

        private static ExportSettings ParseSettings(string[] args)
        {
            var settings = new ExportSettings()
            {
                IgnoreHidden = true,
                Name = true,
                Description = true,
                ExportIcons = true,
                NickName = true,
                Parameters = true,
                DocWriter = typeof(StandardMdDocGenerator)
            };

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--writer":
                    case "-w":
                        if (i + 1 < args.Length)
                        {
                            settings.DocWriter = GetDocWriterType(args[++i]);
                        }
                        break;
                    case "--include_hidden": 
                        settings.IgnoreHidden = false; 
                        break;
                    case "--no_name": 
                        settings.Name = false; 
                        break;
                    case "--no_nickname":     
                        settings.NickName = false; 
                        break;
                    case "--no_description": 
                        settings.Description = false; 
                        break;
                    case "--no_params":       
                        settings.Parameters = false; 
                        break;
                    case "--no_icons": 
                        settings.ExportIcons = false; 
                        break;
                }
            }

            return settings;
        }

        private static Type GetDocWriterType(string writerName)
        {
            return writerName.ToLower() switch
            {
                "standard" => typeof(StandardMdDocGenerator),
                "standardtoc" => typeof(StandardWithTocMdDocGenerator),
                // Add other generators here as you create them:
                // "custom" => typeof(CustomDocGenerator),
                _ => typeof(StandardMdDocGenerator) // Fallback
            };
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: Ducker <pathToDll> [flags]");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  -w, --writer <type>    Documentation generator (default: standard)");
            Console.WriteLine("                             Valid types: standard, standardToC");
            Console.WriteLine("  --include_hidden       Include hidden components");
            Console.WriteLine("  --no_name              Include component names");
            Console.WriteLine("  --no_nickname          Include nicknames");
            Console.WriteLine("  --no_description       Include descriptions");
            Console.WriteLine("  --no_params            Include parameters");
            Console.WriteLine("  --no_icons             Export component icons");
            Console.WriteLine("\nExample: Ducker \"my_plugin.gha\" --writer standard --name --icons");
        }
    }
}
