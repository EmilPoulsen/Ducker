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
            
            var duckers = reader.Read(pathToDll);

            string content = CreateMarkdown(duckers);
            string pathToOutput = @"C:\Users\epoulsen\Documents\GitHub\Emu\Emu\Emu.Grasshopper\bin\Release\Emu.Grasshopper.md";
            WriteFile(content, pathToOutput);
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
    }
}
