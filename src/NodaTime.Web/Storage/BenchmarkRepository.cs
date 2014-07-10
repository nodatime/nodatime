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
        public TimeSpan LoadingTime { get; private set; }
        public DateTimeOffset Loaded { get; private set; }

        private BenchmarkRepository(ImmutableList<BenchmarkRun> allRuns, TimeSpan loadingTime, DateTimeOffset loaded)
        {
            AllRuns = allRuns;
            RunsByMachine = allRuns.ToLookup(x => x.Machine);
            LoadingTime = loadingTime;
            Loaded = loaded;
        }

        internal BenchmarkRun GetRun(string machine, string label)
        {
            return RunsByMachine[machine].FirstOrDefault(f => f.Label == label);
        }

        internal static BenchmarkRepository Load(string directory)
        {
            Stopwatch sw = Stopwatch.StartNew();
            string index = Path.Combine(directory, "index.txt");
            var files = File.ReadLines(index)
                .Select(file => XDocument.Load(Path.Combine(directory, file)))
                .Select(x => BenchmarkRun.FromXElement(x.Root))
                .OrderByDescending(b => b.StartTime)
                .ToImmutableList();
            sw.Stop();
            return new BenchmarkRepository(files, sw.Elapsed, DateTimeOffset.Now);
        }

        internal static BenchmarkRepository LoadSingleFile(string file)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var runs = XDocument.Load(file)
                .Root
                .Elements("benchmark")
                .Select(BenchmarkRun.FromXElement)
                .OrderByDescending(b => BuildForLabel(b.Label))
                .ThenByDescending(b => b.StartTime)
                .ToImmutableList();
            sw.Stop();
            return new BenchmarkRepository(runs, sw.Elapsed, DateTimeOffset.Now);
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
