// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime.Benchmarks;
using NodaTime.Web.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NodaTime.Web.Models
{
    /// <summary>
    /// Benchmark repository that loads benchmarks from the file system.
    /// TODO: Remove the redundancy between this and GoogleStorageBenchmarkRepository.
    /// Maybe a storage-based IFileProvider would work...
    /// </summary>
    public class LocalBenchmarkRepository : IBenchmarkRepository
    {
        private readonly IList<BenchmarkEnvironment>? environments;
        private readonly IDictionary<string, BenchmarkEnvironment>? environmentsById;
        private readonly IDictionary<string, BenchmarkRun>? runsById;
        private readonly IDictionary<string, BenchmarkType>? typesById;
        private readonly IDictionary<string, Benchmark>? benchmarksById;
        private readonly ILookup<(string, string), BenchmarkType>? typesByCommitAndFullName;

        public LocalBenchmarkRepository(IHostingEnvironment environment, ILogger<LocalBenchmarkRepository> logger)
        {
            var directory = Path.Combine(environment.ContentRootPath, "benchmarks");
            if (!Directory.Exists(directory))
            {
                logger.Log(LogLevel.Information, "No benchmarks directory; skipping");
                return; // Just leave everything as null...
            }
            logger.Log(LogLevel.Information, "Loading benchmarks from {0}", directory);
            environments = LoadEnvironments(directory);
            var runs = LoadRuns(directory, environments);
            environmentsById = environments.ToDictionary(e => e.BenchmarkEnvironmentId);
            runsById = runs.ToDictionary(r => r.BenchmarkRunId);
            logger.Log(LogLevel.Information, "Loaded {0} runs in {1} environments", runsById.Count, environments.Count);
            typesById = runsById.Values.SelectMany(r => r.Types_).ToDictionary(t => t.BenchmarkTypeId);
            typesByCommitAndFullName = runsById.Values
                .SelectMany(r => r.Types_.Select(type => (r.Commit, type)))
                .ToLookup(pair => (pair.Commit, pair.type.FullTypeName), pair => pair.type);
            
            benchmarksById = typesById.Values.SelectMany(t => t.Benchmarks).ToDictionary(b => b.BenchmarkId);
        }

        private static IList<BenchmarkEnvironment> LoadEnvironments(string directory)
        {
            var environments = new List<BenchmarkEnvironment>();
            using (var stream = File.OpenRead(Path.Combine(directory, "environments.pb")))
            {
                while (stream.Position != stream.Length)
                {
                    environments.Add(BenchmarkEnvironment.Parser.ParseDelimitedFrom(stream));
                }
            }
            return environments;
        }

        private static IList<BenchmarkRun> LoadRuns(string directory, IList<BenchmarkEnvironment> environments)
        {
            var unfilteredRuns = Directory.GetFiles(directory, "benchmark-run-*.pb")
                .Select(file => BenchmarkRun.Parser.ParseFrom(File.ReadAllBytes(file)));
            var runs = new List<BenchmarkRun>();
            foreach (var run in unfilteredRuns.OrderByDescending(r => r.Start.ToInstant()))
            {
                var environment = environments.FirstOrDefault(env => env.BenchmarkEnvironmentId == run.BenchmarkEnvironmentId);
                // Skip any runs we don't have an environment for
                if (environment == null)
                {
                    continue;
                }
                run.Environment = environment;
                environment.Runs.Add(run);
                run.PopulateLinks();
                runs.Add(run);
            }
            return runs;
        }

        public IList<BenchmarkEnvironment>? ListEnvironments() => environments;
        public BenchmarkEnvironment? GetEnvironment(string environmentId) => environmentsById?[environmentId];
        public BenchmarkType? GetType(string benchmarkTypeId) => typesById?[benchmarkTypeId];
        public BenchmarkRun? GetRun(string benchmarkRunId) => runsById?[benchmarkRunId];
        public Benchmark? GetBenchmark(string benchmarkId) => benchmarksById?[benchmarkId];
        public IList<BenchmarkType>? GetTypesByCommitAndType(string commit, string fullTypeName) =>
            typesByCommitAndFullName?[(commit, fullTypeName)].ToList();
    }
}
