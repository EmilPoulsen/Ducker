using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ducker.Core
{
    public class EmuMdDocGenerator : MarkDownDocGenerator
    {
        public override string Create(List<DuckerComponent> components)
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

                if (component.Input.Count > 0)
                {
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
    }
}
