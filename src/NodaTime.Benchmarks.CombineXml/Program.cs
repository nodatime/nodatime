// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NodaTime.Benchmarks.CombineXml
{
    /// <summary>
    /// Just a tiny utility to produce index.txt and benchmarks.xml for the Noda Time web site.
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: NodaTime.Benchmarks.CombineXml <directory containing XML benchmarks>");
                return 1;
            }
            var directory = args[0];
            var index = Path.Combine(directory, "index.txt");
            var output = Path.Combine(directory, "benchmarks.xml");
            var files = Directory.GetFiles(directory, "*.xml")
                                 .Where(x => Path.GetFileName(x) != "benchmarks.xml")
                                 .ToList();
            File.WriteAllLines(index, files.Select(Path.GetFileName).ToArray());
            var doc = new XDocument(new XElement("benchmarks", files.Select(XElement.Load)));
            doc.Save(output);
            return 0;
        }
    }
}
