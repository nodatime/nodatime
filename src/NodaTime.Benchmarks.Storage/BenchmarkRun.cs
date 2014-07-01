using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;
using BenchmarkRunProto = NodaTime.Benchmarks.Storage.Proto.BenchmarkRun;

namespace NodaTime.Benchmarks.Storage
{
    /// <summary>
    /// A single run of a set of benchmarks.
    /// </summary>
    public sealed class BenchmarkRun
    {
        private readonly string machine;
        private readonly string label;
        private readonly DateTimeOffset startTime;
        private readonly ImmutableList<BenchmarkTypeResult> results;

        public string Machine { get { return machine; } }
        public string Label { get { return label; } }
        public DateTimeOffset StartTime { get { return startTime; } }
        public ImmutableList<BenchmarkTypeResult> Results { get { return results; } }

        internal BenchmarkRun(string machine, string label, DateTimeOffset startTime, ImmutableList<BenchmarkTypeResult> results)
        {
            this.machine = machine;
            this.label = label;
            this.startTime = startTime;
            this.results = results;
        }

        public static BenchmarkRun FromXDocument(XDocument document)
        {
            var root = document.Root;
            return new BenchmarkRun(
                machine: root.Element("environment").Attribute("machine").Value,
                label: (string) root.Attribute("label"),
                startTime: (DateTimeOffset) root.Attribute("start"),
                results: root.Elements("type")
                             .Select(BenchmarkTypeResult.FromXElement)
                             .OrderBy(result => result.Namespace)
                             .ThenBy(result => result.Type)
                             .ToImmutableList());
        }

        public BenchmarkRunProto ToProto()
        {
            var builder = new BenchmarkRunProto.Builder
            {
                Machine = this.Machine,
                Label = this.Label,
                StartTicksSince0001 = startTime.ToUniversalTime().Ticks,
                TypeResultsList = { results.Select(x => x.ToProto()) }
            };
            if (this.Label != null)
            {
                builder.Label = Label;
            }
            return builder.Build();
        }

        public static BenchmarkRun FromProto(BenchmarkRunProto proto)
        {
            return null;
        }
    }
}
