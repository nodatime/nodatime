// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BenchmarkUploader
{
    using Google.Cloud.Storage.V1;
    using Google.Protobuf;
    using Google.Protobuf.WellKnownTypes;
    using NodaTime.Benchmarks;
    using System.Collections.Generic;
    using static BenchmarkUploader.BriefJsonModel;

    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Arguments: <results root directory> <bucket> <object prefix>");
                return 1;
            }
            try
            {
                MainImpl(args[0], args[1], args[2]);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
                return 1;
            }
        }

        private static void MainImpl(string directory, string bucket, string prefix)
        {
            var storageClient = StorageClient.Create();
            var environmentRepository = new BenchmarkRepository(storageClient, bucket, prefix);
            environmentRepository.LoadEnvironments();
            // Only find test runs that have ended.
            foreach (var endFile in Directory.EnumerateFiles(directory, "end.txt", SearchOption.AllDirectories))
            {
                ProcessRun(environmentRepository, Path.GetDirectoryName(endFile));
                var runDirectory = Path.GetDirectoryName(endFile);
            }
            environmentRepository.Save();
        }

        private static void ProcessRun(BenchmarkRepository environmentRepository, string runDirectory)
        {
            var outputFile = Path.Combine(runDirectory, "benchmarks.pb");

            // If the output file already exists, assume we're done.
            if (File.Exists(outputFile))
            {
                return;
            }

            var startFile = Path.Combine(runDirectory, "start.txt");
            var endFile = Path.Combine(runDirectory, "end.txt");

            var start = DateTimeOffset.ParseExact(File.ReadAllText(startFile).Trim(), "yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
            var end = DateTimeOffset.ParseExact(File.ReadAllText(endFile).Trim(), "yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);

            var tfm = Path.GetFileName(runDirectory);
            var commit = Path.GetFileName(Path.GetDirectoryName(runDirectory));

            var models = Directory.GetFiles(runDirectory, "*.json")
                .Select(file => JsonConvert.DeserializeObject<BriefJsonModel>(File.ReadAllText(file)))
                .Where(m => m.Benchmarks.Any())
                .ToList();
            if (!models.Any())
            {
                // TODO: Throw?
                return;
            }

            var runKey = Guid.NewGuid().ToString();

            File.WriteAllText(outputFile, runKey);

            var hostEnvironment = models.First().HostEnvironmentInfo;
            var environment = new BenchmarkEnvironment
            {
                Machine = Environment.MachineName.ToLowerInvariant(),
                OperatingSystem = hostEnvironment.OsVersion?.GetValue() ?? "",
                Processor = hostEnvironment.ProcessorName?.GetValue() ?? "",
                ProcessorCount = hostEnvironment.ProcessorCount,
                TargetFramework = tfm,
                JitModules = hostEnvironment.JitModules,
                HasRyuJit = hostEnvironment.HasRyuJit,
                Architecture = hostEnvironment.Architecture,
                RuntimeVersion = hostEnvironment.RuntimeVersion
            };
            var environmentId = environmentRepository.GetOrAddEnvironmentId(environment);
            var run = new BenchmarkRun
            {
                BenchmarkRunId = Guid.NewGuid().ToString(),
                BenchmarkEnvironmentId = environmentId,
                Commit = commit,
                Start = start.ToTimestamp(),
                End = end.ToTimestamp(),
                BenchmarkDotNetVersion = hostEnvironment.BenchmarkDotNetVersion,
            };
            var types = new List<BenchmarkType>();
            foreach (var model in models)
            {
                var firstBenchmark = model.Benchmarks.First();
                var typeId = Guid.NewGuid().ToString();
                types.Add(new BenchmarkType
                {
                    BenchmarkRunId = run.BenchmarkRunId,
                    BenchmarkTypeId = typeId,
                    FullTypeName = $"{firstBenchmark.Namespace}.{firstBenchmark.Type}",
                    Namespace = firstBenchmark.Namespace,
                    Type = firstBenchmark.Type,
                    Benchmarks = { model.Benchmarks.Select(b => CreateBenchmark(b, typeId)).OrderBy(b => b.Method) }
                });
            }
            run.Types_.AddRange(types.OrderBy(t => t.FullTypeName));
            File.WriteAllBytes(outputFile, run.ToByteArray());
            environmentRepository.AddRun(run);
            Console.WriteLine($"Created benchmark file for {commit} / {tfm}");
        }

        static Benchmark CreateBenchmark(JsonBenchmark benchmark, string typeId) =>
            new Benchmark
            {
                BenchmarkId = Guid.NewGuid().ToString(),
                BenchmarkTypeId = typeId,
                FullMethodName = $"{benchmark.Namespace}.{benchmark.Type}.{benchmark.Method}",
                Method = benchmark.Method,
                Parameters = benchmark.Parameters,
                Statistics = benchmark.Statistics == null ? null : CreateStatistics(benchmark.Statistics)
            };

        static Statistics CreateStatistics(JsonStatistics statistics) => new Statistics
        {
            // TODO: Outliers?
            InterquartileRange = statistics.InterquartileRange,
            TestCount = statistics.N,
            Kurtosis = statistics.Kurtosis,
            LowerFence = statistics.LowerFence,
            Max = statistics.Max,
            Mean = statistics.Mean,
            Median = statistics.Median,
            Min = statistics.Min,
            P0 = statistics.Percentiles.P0,
            P25 = statistics.Percentiles.P25,
            P50 = statistics.Percentiles.P50,
            P67 = statistics.Percentiles.P67,
            P80 = statistics.Percentiles.P80,
            P85 = statistics.Percentiles.P85,
            P90 = statistics.Percentiles.P90,
            P95 = statistics.Percentiles.P95,
            P100 = statistics.Percentiles.P100,
            Q1 = statistics.Q1,
            Q3 = statistics.Q3,
            Skewness = statistics.Skewness,
            StandardDeviation = statistics.StandardDeviation,
            StandardError = statistics.StandardError,
            UpperFence = statistics.UpperFence,
            Variance = statistics.Variance
        };        
    }
}