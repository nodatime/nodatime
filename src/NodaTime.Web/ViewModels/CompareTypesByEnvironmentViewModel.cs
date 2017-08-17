// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using MoreLinq;
using NodaTime.Benchmarks;
using NodaTime.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.Web.ViewModels
{
    /// <summary>
    /// View-model for comparing multiple runs of the same type at the same commit.
    /// </summary>
    public class CompareTypesByEnvironmentViewModel
    {
        public IList<BenchmarkType> Types { get; }
        public BenchmarkType TargetType { get; }

        public CompareTypesByEnvironmentViewModel(IList<BenchmarkType> types)
        {
            Types = types;
            TargetType = Types.First();
        }

        // TODO: Handle missing benchmarks
        public IEnumerable<(Benchmark description, IEnumerable<Statistics> statistics)> GetBenchmarks()
        {
            List<Dictionary<string, Statistics>> statisticsByNameInTypeOrder = Types
                .Select(t => t.Benchmarks.ToDictionary(tb => tb.Method, tb => tb.Statistics))
                .ToList();
            return TargetType.Benchmarks
                .OrderBy(b => b.Method)
                .Select(b => (b, statisticsByNameInTypeOrder.Select(d => d.GetValueOrDefault(b.Method))));
        }
        
        /* TODO: Reinstate this, on a per cell basis 
        private static bool IsImportant(Statistics left, Statistics right)
        {
            if (left == null || right == null)
            {
                return false;
            }
            // Just in case we have strange results that might cause problems on division. (If something takes
            // less than a picosecond, I'm suspicious...)
            if (left.Mean < 0.001 || right.Mean < 0.001)
            {
                return true;
            }
            double min = Math.Min(left.Mean, right.Mean);
            double max = Math.Max(left.Mean, right.Mean);
            return min / max < 0.8; // A 20% change either way is important. (Arbitrary starting point...)
        } */
    }
}
