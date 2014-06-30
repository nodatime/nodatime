using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public TimeSpan LoadingTime { get; private set; }

        private BenchmarkRepository(ImmutableList<BenchmarkRun> allRuns, TimeSpan loadingTime)
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
            Stopwatch sw = Stopwatch.StartNew();
            string index = Path.Combine(directory, "index.txt");
            DateTime lastModified = File.GetLastWriteTime(index);
            var files = File.ReadLines(index)
                .Select(file => XDocument.Load(Path.Combine(directory, file)))
                .Select(BenchmarkRun.FromXDocument)
                .OrderByDescending(b => b.StartTime)
                .ToImmutableList();
            sw.Stop();
            return new BenchmarkRepository(files, sw.Elapsed);
        }

        public static string HashForLabel(string label)
        {
            return label.Split('.')[0];
        }
    }
}
