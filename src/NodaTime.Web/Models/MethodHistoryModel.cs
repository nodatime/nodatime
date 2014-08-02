using NodaTime.Web.Storage;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NodaTime.Web.Models
{
    public sealed class MethodHistoryModel
    {
        public string Machine { get; private set; }
        public string Method { get; private set; }
        public ImmutableList<Entry> Entries { get; private set; }
        public bool AllResults { get; private set; }

        private MethodHistoryModel()
        {
        }

        public static MethodHistoryModel ForMachineMethod(BenchmarkRepository repository, string machine, string method, bool allResults)
        {
            var runs = repository.RunsByMachine[machine];
            List<Entry> entries = new List<Entry>();
            BenchmarkResult previousResult = null;
            // Work things out in ascending time order, then reverse later. Too confusing otherwise!
            foreach (var run in runs.OrderBy(f => f.StartTime))
            {
                var result = run.Results.FirstOrDefault(m => m.FullyQualifiedMethod == method);
                if (result == null && previousResult == null)
                {
                    continue;
                }
                if (result == null)
                {
                    entries.Add(new Entry(run, "Method removed"));
                }
                else if (allResults)
                {
                    entries.Add(new Entry(run, result.NanosecondsPerCall + "ns"));
                }
                else if (previousResult == null)
                {
                    entries.Add(new Entry(run, "Method introduced"));
                }
                else
                {
                    // TODO: This only spots sudden improvements, not gradual improvements over time. Consider
                    // remembering the "last change" result and using that instead.
                    long percent = (result.PicosecondsPerCall * 100) / previousResult.PicosecondsPerCall;
                    if (percent > BenchmarkDiff.RegressionThreshold)
                    {
                        entries.Add(new Entry(run, string.Format("Regression: {0}ns to {1}ns", previousResult.NanosecondsPerCall, result.NanosecondsPerCall)));
                    }
                    if (percent < BenchmarkDiff.ImprovementThreshold)
                    {
                        entries.Add(new Entry(run, string.Format("Improvement: {0}ns to {1}ns", previousResult.NanosecondsPerCall, result.NanosecondsPerCall)));
                    }
                }
                previousResult = result;
            }
            entries.Reverse();
            return new MethodHistoryModel { Machine = machine, Method = method, Entries = entries.ToImmutableList(), AllResults = allResults};
        }

        public class Entry
        {
            public BenchmarkRun Run { get; private set; }
            public string Description { get; private set; }

            public Entry(BenchmarkRun run, string description)
            {
                this.Run = run;
                this.Description = description;
            }
        }
    }
}