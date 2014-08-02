using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;

namespace NodaTime.Web.Storage
{
    /// <summary>
    /// Storage for a set of benchmark runs.
    /// </summary>
    public class BenchmarkRepository
    {
        public ImmutableList<BenchmarkRun> AllRuns { get; private set;  }
        public ILookup<string, BenchmarkRun> RunsByMachine { get; private set; }

        private BenchmarkRepository(ImmutableList<BenchmarkRun> allRuns)
        {
            AllRuns = allRuns;
            RunsByMachine = allRuns.ToLookup(x => x.Machine);
        }

        internal BenchmarkRun GetRun(string machine, string label)
        {
            return RunsByMachine[machine].FirstOrDefault(f => f.Label == label);
        }

        internal static BenchmarkRepository Load(string directory)
        {
            string index = Path.Combine(directory, "index.txt");
            var files = File.ReadLines(index)
                .Select(file => XDocument.Load(Path.Combine(directory, file)))
                .Select(x => BenchmarkRun.FromXElement(x.Root))
                .OrderByDescending(b => b.StartTime)
                .ToImmutableList();
            return new BenchmarkRepository(files);
        }

        internal static BenchmarkRepository LoadSingleFile(string file)
        {
            var runs = XDocument.Load(file)
                .Root
                .Elements("benchmark")
                .Select(BenchmarkRun.FromXElement)
                .OrderByDescending(b => BuildForLabel(b.Label))
                .ThenByDescending(b => b.StartTime)
                .ToImmutableList();
            return new BenchmarkRepository(runs);
        }

        public static string HashForLabel(string label)
        {
            return label.Split('.')[0];
        }

        public static int BuildForLabel(string label)
        {
            return int.Parse(label.Split('.')[1]);
        }
    }
}
