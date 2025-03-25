using KpBinEditor.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KpBinEditor
{
    internal class ResMeta
    {
        public List<StructData> Structs { get; } = new();

        public void Init()
        {
            var xml = new XmlDocument();
            xml.Load(Settings.Default.ResMetaDrFile);
            var nodes = xml.SelectNodes("/metalib/struct");
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    var name = node.Attributes?["name"]?.Value;
                    if (!string.IsNullOrEmpty(name))
                    {
                        var desc = node.Attributes?["desc"]?.Value ?? name;
                        var sd = new StructData(name)
                        {
                            Desc = desc
                        };
                        var entries = node.SelectNodes("entry");
                        if (entries != null)
                        {
                            foreach (XmlNode entry in entries)
                            {
                                var entryName = entry.Attributes?["name"]?.Value;
                                var type = entry.Attributes?["type"]?.Value;
                                if (!string.IsNullOrEmpty(entryName) && !string.IsNullOrEmpty(type))
                                {
                                    var e = new StructEntry(entryName, type)
                                    {
                                        CName = entry.Attributes?["cname"]?.Value,
                                    };
                                    var count = entry.Attributes?["count"]?.Value;
                                    if (!string.IsNullOrEmpty(count) && int.TryParse(count, out var c))
                                    {
                                        e.Count = c;
                                    }
                                    sd.Entries.Add(e);
                                }
                            }
                        }

                        Structs.Add(sd);
                    }
                }
            }
        }
    }


    internal class StructData
    {
        public StructData(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public string? Desc { get; set; }

        public List<StructEntry> Entries { get; set; } = new();
    }

    internal class StructEntry
    {
        public StructEntry(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public int Count { get; set; } = 1;
        public string Name { get; set; }
        public string Type { get; set; }
        public string? CName { get; set; }
    }
}
