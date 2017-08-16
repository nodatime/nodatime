// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NodaTime.Benchmarks;
using System.Collections.Generic;

namespace NodaTime.Web.Models
{
    public interface IBenchmarkRepository
    {
        BenchmarkEnvironment GetEnvironment(string environmentId);
        BenchmarkType GetType(string benchmarkTypeId);
        BenchmarkRun GetRun(string benchmarkRunId);
        Benchmark GetBenchmark(string benchmarkId);
        IList<BenchmarkEnvironment> ListEnvironments();
        IList<BenchmarkType> GetTypesByCommitAndType(string commit, string fullTypeName);
    }
}
