// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System.Collections.Generic;

namespace BenchmarkUploader
{
    /// <summary>
    /// Class containing all the important parts of a briefjson export
    /// </summary>
    public class BriefJsonModel
    {
        public HostEnvironment HostEnvironmentInfo { get; set; }
        public List<JsonBenchmark> Benchmarks { get; set; }

        public class OptionalString
        {
            public bool IsValueCreated { get; set; }
            public string Value { get; set; }

            public string GetValue() => IsValueCreated ? Value : null;
        }

        public class HostEnvironment
        {
            public string RuntimeVersion { get; set; }
            public string Architecture { get; set; }
            public bool HasRyuJit { get; set; }
            public string BenchmarkDotNetVersion { get; set; }
            public string JitModules { get; set; }
            public OptionalString OsVersion { get; set; }
            public OptionalString ProcessorName { get; set; }
            public int ProcessorCount { get; set; }
        }

        public class JsonBenchmark
        {
            public string Namespace { get; set; }
            public string Type { get; set; }
            public string Method { get; set; }
            public string MethodTitle { get; set; }
            public string Parameters { get; set; }
            public JsonStatistics Statistics { get; set; }
        }

        public class JsonStatistics
        {
            public int N { get; set; }
            public double Mean { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }
            public double Median { get; set; }
            public double LowerFence { get; set; }
            public double UpperFence { get; set; }
            public double Q1 { get; set; }
            public double Q3 { get; set; }
            public double InterquartileRange { get; set; }
            public List<double> Outliers { get; set; }
            public double StandardError { get; set; }
            public double Variance { get; set; }
            public double StandardDeviation { get; set; }
            public double Skewness { get; set; }
            public double Kurtosis { get; set; }
            // TODO: Confidence interval?
            public Percentiles Percentiles { get; set; }
        }

        public class Percentiles
        {
            public double P0 { get; set; }
            public double P25 { get; set; }
            public double P50 { get; set; }
            public double P67 { get; set; }
            public double P80 { get; set; }
            public double P85 { get; set; }
            public double P90 { get; set; }
            public double P95 { get; set; }
            public double P100 { get; set; }            
        }
    }
}
