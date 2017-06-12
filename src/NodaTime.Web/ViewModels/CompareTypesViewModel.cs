// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using MoreLinq;
using NodaTime.Benchmarks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.Web.ViewModels
{
    /// <summary>
    /// View-model for comparing two runs of the same type (by fully-qualified name).
    /// </summary>
    public class CompareTypesViewModel
    {
        public BenchmarkEnvironment LeftEnvironment { get; }
        public BenchmarkEnvironment RightEnvironment { get; }

        public bool SingleEnvironment => LeftEnvironment.BenchmarkEnvironmentId == RightEnvironment.BenchmarkEnvironmentId;

        public BenchmarkRun LeftRun { get; }
        public BenchmarkRun RightRun { get; }

        public BenchmarkType LeftType { get; }
        public BenchmarkType RightType { get; }

        public CompareTypesViewModel(
            (BenchmarkEnvironment, BenchmarkRun, BenchmarkType) left,
            (BenchmarkEnvironment, BenchmarkRun, BenchmarkType) right)
        {
            LeftEnvironment = left.Item1;
            LeftRun = left.Item2;
            LeftType = left.Item3;

            RightEnvironment = right.Item1;
            RightRun = right.Item2;
            RightType = right.Item3;
        }

        public IEnumerable<(Benchmark description, Statistics left, Statistics right, bool important)> GetBenchmarks()
        {
            // TODO: Handle missing types
            var leftBenchmarks = LeftType.Benchmarks.ToDictionary(b => b.Method);
            var rightBenchmarks = RightType.Benchmarks.ToDictionary(b => b.Method);
            var union = LeftType.Benchmarks.Concat(RightType.Benchmarks).DistinctBy(b => b.Method).OrderBy(b => b.Method);
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
