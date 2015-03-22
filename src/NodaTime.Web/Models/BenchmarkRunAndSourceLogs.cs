using NodaTime.Web.Storage;
using System.Collections.Immutable;
using Minibench.Framework;

namespace NodaTime.Web.Models
{
    public class BenchmarkRunAndSourceLogs
    {
        public BenchmarkRun Run { get; private set; }
        public ImmutableList<SourceLogEntry> Changes { get; private set; }

        public BenchmarkRunAndSourceLogs(BenchmarkRun benchmarkRun, ImmutableList<SourceLogEntry> changes)
        {
            this.Run = benchmarkRun;
            this.Changes = changes;
        }
    }
}