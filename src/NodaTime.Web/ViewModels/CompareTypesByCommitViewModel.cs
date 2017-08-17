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
    /// View-model for comparing two runs of the same type, in the same environment, at different commits.
    /// </summary>
    public class CompareTypesByCommitViewModel
    {
        public BenchmarkEnvironment Environment => Left.Environment;
        public BenchmarkType Left { get; }
        public BenchmarkType Right { get; }

        public CompareTypesByCommitViewModel(BenchmarkType left, BenchmarkType right)
        {
            Left = left;
            Right = right;
        }

        public IEnumerable<(Benchmark description, Statistics left, Statistics right, bool important)> GetBenchmarks()
        {
            // TODO: Handle missing types
            var leftBenchmarks = Left.Benchmarks.ToDictionary(b => b.Method);
            var rightBenchmarks = Right.Benchmarks.ToDictionary(b => b.Method);
            var union = Left.Benchmarks.Concat(Right.Benchmarks).DistinctBy(b => b.Method).OrderBy(b => b.Method);
            foreach (var benchmark in union)
            {
                var leftStats = leftBenchmarks.GetValueOrDefault(benchmark.Method)?.Statistics;
                var rightStats = rightBenchmarks.GetValueOrDefault(benchmark.Method)?.Statistics;
                yield return (benchmark, leftStats, rightStats, IsImportant(leftStats, rightStats));
            }
        }

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
        }
    }
}
