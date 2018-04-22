// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace SummarizeBenchmarks
{
    public class Benchmark
    {
        public string Class { get; }
        public string Method { get; }
        public double Mean { get; }

        private Benchmark(string className, string method, double mean)
        {
            Class = className;
            Method = method;
            Mean = mean;
        }

        public static IEnumerable<Benchmark> ParseFile(JObject report)
        {
            string title = (string)report["Title"];
            JArray benchmarks = (JArray)report["Benchmarks"];
            return benchmarks.Select(b => new Benchmark(title, (string)b["Method"], (double)b["Statistics"]["Mean"]));
        }
    }
}
