using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

namespace NodaTime.Web.Storage
{
    /// <summary>
    /// A single run of a set of benchmarks.
    /// </summary>
    public sealed class BenchmarkRun
    {
        private readonly string machine;
        private readonly string label;
        private readonly DateTimeOffset startTime;
        private readonly ImmutableList<BenchmarkResult> results;

        public string Machine { get { return machine; } }
        public string Label { get { return label; } }
        public DateTimeOffset StartTime { get { return startTime; } }
        public ImmutableList<BenchmarkResult> Results { get { return results; } }

        internal BenchmarkRun(string machine, string label, DateTimeOffset startTime, ImmutableList<BenchmarkResult> results)
        {
            this.machine = machine;
            this.label = label;
            this.startTime = startTime;
            this.results = results;
        }

        internal static BenchmarkRun FromXDocument(XDocument document)
        {
            var root = document.Root;
            return new BenchmarkRun(
                machine: root.Element("environment").Attribute("machine").Value,
                label: (string) root.Attribute("label"),
                startTime: (DateTimeOffset) root.Attribute("start"),
                results: root.Descendants("test")
                             .Select(BenchmarkResult.FromXElement)
                             .OrderBy(result => result.Namespace)
                             .ThenBy(result => result.Type)
                             .ThenBy(result => result.Method)
                             .ToImmutableList());
        }
    }
}
