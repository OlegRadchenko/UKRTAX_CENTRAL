using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;

namespace UAFMDR
{
    public sealed class ApplicationModel
    {
        public string Name { get; set; } = "";
        public List<SectionModel> Sections { get; } = new();
    }

    public sealed class SectionModel
    {
        public string Name { get; set; } = "";
        public List<DataSetModel> DataSets { get; } = new();
    }

    public sealed class DataSetModel
    {
        public string Name { get; set; } = "";
        public string? Link { get; set; }             // optional
        public List<DataSetModel> Children { get; } = new();
    }

    public static class AppXmlParser
    {
        public static ApplicationModel Parse(XDocument doc)
        {
            if (doc.Root?.Name != "application")
                throw new InvalidOperationException("Root <application> was not found.");

            var app = new ApplicationModel
            {
                Name = (string?)doc.Root.Attribute("name") ?? ""
            };

            foreach (var s in doc.Root.Elements("section"))
            {
                var section = new SectionModel
                {
                    Name = (string?)s.Attribute("name") ?? ""
                };

                foreach (var ds in s.Elements("dataset"))
                    section.DataSets.Add(ParseDataset(ds));

                app.Sections.Add(section);
            }
            return app;
        }

        private static DataSetModel ParseDataset(XElement el)
        {
            var ds = new DataSetModel
            {
                Name = (string?)el.Attribute("name") ?? "",
                Link = (string?)el.Attribute("link")
            };

            foreach (var child in el.Elements("dataset"))
                ds.Children.Add(ParseDataset(child));

            return ds;
        }

        // Пошук першого dataset за ім’ям у всьому додатку
        public static DataSetModel? FindDataset(ApplicationModel app, string name) =>
            app.Sections.SelectMany(s => s.DataSets)
                        .SelectMany(Flatten)
                        .FirstOrDefault(d => string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase));

        // Розплющення дерева dataset’ів у послідовність
        public static IEnumerable<DataSetModel> Flatten(DataSetModel ds)
        {
            yield return ds;
            foreach (var c in ds.Children)
                foreach (var n in Flatten(c))
                    yield return n;
        }
    }
}
