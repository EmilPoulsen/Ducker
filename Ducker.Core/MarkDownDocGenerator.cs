using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    public abstract class MarkDownDocGenerator : IDocGenerator
    {
        public string FileExtension
        {
            get
            {
                return "md";
            }
        }

        public abstract string Create(List<DuckerComponent> components);

        protected string GenerateParamTable(List<DuckerParam> compParameter)
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

        protected string GetParamRow(DuckerParam p)
        {
            string header = string.Format("| {0} | {1} | {2} |", p.Name, p.NickName, p.Description);
            return header;
        }

        protected string GetParamHeader(DuckerParam p)
        {
            string header = string.Format("| {0} | {1} | {2} |", nameof(p.Name), nameof(p.NickName), nameof(p.Description));
            return header;
        }

        protected string Bold(string text)
        {
            return "**" + text + "**";
        }

        protected string Paragraph(string text)
        {
            return text + "  " + Environment.NewLine;
        }

        protected string Header(string text, int level)
        {
            string hashes = new string('#', level) + " ";
            return hashes + text + Environment.NewLine; ;
        }

        protected string Header(string text)
        {
            return Header(text, 1);
        }

    }
}
