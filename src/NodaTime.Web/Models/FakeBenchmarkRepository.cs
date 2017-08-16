// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NodaTime.Benchmarks;
using System;
using System.Collections.Generic;

namespace NodaTime.Web.Models
{
    public class FakeBenchmarkRepository : IBenchmarkRepository
    {
        public BenchmarkEnvironment GetEnvironment(string environmentId) => null;
        public BenchmarkRun GetRun(string benchmarkRunId) => null;
        public BenchmarkType GetType(string benchmarkTypeId) => null;
        public Benchmark GetBenchmark(string benchmarkId) => null;
        public IList<BenchmarkEnvironment> ListEnvironments() => new List<BenchmarkEnvironment>();
        public IList<BenchmarkType> GetTypesByCommitAndType(string commit, string fullTypeName) => new List<BenchmarkType>();
    }
}
