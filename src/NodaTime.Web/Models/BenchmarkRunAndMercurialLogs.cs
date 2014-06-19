using NodaTime.Web.Storage;
using System.Collections.Immutable;

namespace NodaTime.Web.Models
{
    public class BenchmarkRunAndMercurialLogs
    {
        public BenchmarkRun Run { get; private set; }
        public ImmutableList<MercurialLogEntry> Changes { get; private set; }

        public BenchmarkRunAndMercurialLogs(BenchmarkRun benchmarkRun, ImmutableList<MercurialLogEntry> changes)
        {
            this.Run = benchmarkRun;
            this.Changes = changes;
        }
    }
}