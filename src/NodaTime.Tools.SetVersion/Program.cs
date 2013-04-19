// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace NodaTime.Tools.SetVersion
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: SetVersion <version-number>");
                return 1;
            }
            if (!Directory.Exists("src"))
            {
                Console.Error.WriteLine("Run from the top-level NodaTime directory; parent of the src directory");
                return 1;
            }
            ProjectVersion version = new ProjectVersion(args[0]);
            FixNuSpec(version, "src/NodaTime/NodaTime.nuspec");
            FixNuSpec(version, "src/NodaTime.Testing/NodaTime.Testing.nuspec");
            FixAssemblyInfo(version, "src/NodaTime/Properties/AssemblyInfo.cs");
            FixAssemblyInfo(version, "src/NodaTime.Testing/Properties/AssemblyInfo.cs");
            return 0;
        }

        private static void FixNuSpec(ProjectVersion version, string file)
        {
            XNamespace ns = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";
            var readerSettings = new XmlReaderSettings
            {
                IgnoreComments = false,
                IgnoreWhitespace = false         
            };
            XDocument doc;
            using (var reader = XmlReader.Create(file, readerSettings))
            {
                doc = XDocument.Load(reader);
            }
            var versionElement = doc.Descendants(ns + "version").Single();
            versionElement.Value = version.FullText;
            var dependency = doc.Descendants(ns + "dependency").Where(x => (string)x.Attribute("id") == "NodaTime").FirstOrDefault();
            if (dependency != null)
            {
                dependency.SetAttributeValue("version", version.Dependency);
            }

            var writerSettings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = false,
                NewLineHandling = NewLineHandling.None,
            };

            using (var writer = XmlWriter.Create(file, writerSettings))
            {
                doc.Save(writer);
            }
        }

        private static void FixAssemblyInfo(ProjectVersion version, string file)
        {
            string[] lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                line = ReplaceAttribute(line, "AssemblyVersion", version.MajorMinor);
                line = ReplaceAttribute(line, "AssemblyFileVersion", version.MajorMinorPatch);
                line = ReplaceAttribute(line, "AssemblyInformationalVersion", version.FullText);
                lines[i] = line;
            }
            File.WriteAllLines(file, lines);
        }

        private static string ReplaceAttribute(string line, string attributeName, string newValue)
        {
            string prefix = "[assembly: " + attributeName + "(\"";
            return line.StartsWith(prefix) ? prefix + newValue + "\")]" : line;
        }
    }
}
