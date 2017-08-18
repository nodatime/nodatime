// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime.Benchmarks;
using NodaTime.Web.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NodaTime.Web.Models
{
    public class GoogleStorageBenchmarkRepository : IBenchmarkRepository
    {
        private const string BucketName = "nodatime";
        private const string EnvironmentObjectName = "benchmarks/environments.pb";
        private const string ContainerObjectsPrefix = "benchmarks/benchmark-run-";

        private static readonly Duration CacheRefreshTime = Duration.FromMinutes(15);
        private readonly TimerCache<CacheValue> cache;

        public GoogleStorageBenchmarkRepository(
            IApplicationLifetime lifetime,
            ILoggerFactory loggerFactory,
            GoogleCredential credential)
        {
            StorageClient client = StorageClient.Create(credential);
            cache = new TimerCache<CacheValue>(lifetime, CacheRefreshTime, () => CacheValue.Refresh(cache.Value, client), loggerFactory,
                CacheValue.Empty);
        }
        
        public IList<BenchmarkEnvironment> ListEnvironments() => cache.Value.Environments;
        // TODO: Don't throw if asked for an invalid value...
        public BenchmarkEnvironment GetEnvironment(string environmentId) => cache.Value.EnvironmentsById[environmentId];
        public BenchmarkType GetType(string benchmarkTypeId) => cache.Value.TypesById[benchmarkTypeId];
        public BenchmarkRun GetRun(string benchmarkRunId) => cache.Value.RunsById[benchmarkRunId];
        public Benchmark GetBenchmark(string benchmarkId) => cache.Value.BenchmarksById[benchmarkId];
        public IList<BenchmarkType> GetTypesByCommitAndType(string commit, string fullTypeName) =>
            cache.Value.TypesByCommitAndFullName[(commit, fullTypeName)].ToList();

        private class CacheValue
        {
            public static CacheValue Empty { get; } = new CacheValue(new List<BenchmarkEnvironment>(), "", new Dictionary<string, BenchmarkRun>());

            // Processed properties, used for 
            public IList<BenchmarkEnvironment> Environments { get; }
            public IDictionary<string, BenchmarkEnvironment> EnvironmentsById { get; }
            public IDictionary<string, BenchmarkRun> RunsById { get; }
            public IDictionary<string, BenchmarkType> TypesById { get; }
            public IDictionary<string, Benchmark> BenchmarksById { get; }
            public ILookup<(string, string), BenchmarkType> TypesByCommitAndFullName { get; }

            private readonly string environmentCrc32c;
            // Key is the storage object name.
            private readonly Dictionary<string, BenchmarkRun> runsByStorageName;

            private CacheValue(
                IList<BenchmarkEnvironment> environments,
                string environmentCrc32c,
                Dictionary<string, BenchmarkRun> runsByStorageName)
            {
                Environments = environments;

                this.environmentCrc32c = environmentCrc32c;
                this.runsByStorageName = runsByStorageName;

                EnvironmentsById = Environments.ToDictionary(e => e.BenchmarkEnvironmentId);
                RunsById = runsByStorageName.Values.ToDictionary(r => r.BenchmarkRunId);
                TypesById = RunsById.Values.SelectMany(r => r.Types_).ToDictionary(t => t.BenchmarkTypeId);
                TypesByCommitAndFullName = RunsById.Values
                    .SelectMany(r => r.Types_.Select(type => (r.Commit, type)))
                    .ToLookup(pair => (pair.Commit, pair.type.FullTypeName), pair => pair.type);
                BenchmarksById = TypesById.Values.SelectMany(t => t.Benchmarks).ToDictionary(b => b.BenchmarkId);
            }

            public static CacheValue Refresh(CacheValue previous, StorageClient client)
            {
                var containerObjects = client.ListObjects(BucketName, ContainerObjectsPrefix).Select(obj => obj.Name).ToList();
                var newContainerNames = containerObjects.Except(previous.runsByStorageName.Keys).ToList();

                // If there are no new runs to load, use the previous set even if there are new environments.
                // An empty environment is fairly pointless... (and we don't mind too much if there are old ones that have been removed;
                // they'll go when there's next a new one.)
                if (!newContainerNames.Any())
                {
                    return previous;
                }

                // We won't reload everything from storage, but we'll come up with completely new set of objects each time, so we
                // don't need to worry about the changes involved as we reattach links.
                var environmentObject = client.GetObject(BucketName, EnvironmentObjectName);
                var environments = environmentObject.Crc32c == previous.environmentCrc32c
                    ? previous.Environments.Select(env => { var clone = env.Clone(); clone.Runs.Clear(); return clone; }).ToList()
                    : LoadEnvironments(client).OrderBy(e => e.Machine).ThenBy(e => e.TargetFramework).ThenBy(e => e.OperatingSystem).ToList();

                // Don't just use previous.runsByStorageName blindly - some may have been removed.
                var runsByStorageName = new Dictionary<string, BenchmarkRun>();
                foreach (var runStorageName in containerObjects)
                {
                    if (previous.runsByStorageName.TryGetValue(runStorageName, out var runToAdd))
                    {
                        runToAdd = runToAdd.Clone();
                    }
                    else
                    {
                        runToAdd = LoadContainer(client, runStorageName);
                    }
                    runToAdd.Environment = environments.FirstOrDefault(env => env.BenchmarkEnvironmentId == runToAdd.BenchmarkEnvironmentId);
                    runToAdd.PopulateLinks();
                    runsByStorageName[runStorageName] = runToAdd;
                }
                // Attach the runs to environments, in reverse chronological order
                foreach (var run in runsByStorageName.Values.OrderByDescending(r => r.Start.ToInstant()))
                {
                    run.Environment.Runs.Add(run);
                }
                return new CacheValue(environments, environmentObject.Crc32c, runsByStorageName);
            }

            private static BenchmarkRun LoadContainer(StorageClient client, string runStorageName)
            {
                var stream = new MemoryStream();
                client.DownloadObject(BucketName, runStorageName, stream);
                stream.Position = 0;
                return BenchmarkRun.Parser.ParseFrom(stream);
            }

            private static IList<BenchmarkEnvironment> LoadEnvironments(StorageClient client)
            {
                var environments = new List<BenchmarkEnvironment>();
                var stream = new MemoryStream();
                client.DownloadObject(BucketName, EnvironmentObjectName, stream);
                stream.Position = 0;
                while (stream.Position != stream.Length)
                {
                    environments.Add(BenchmarkEnvironment.Parser.ParseDelimitedFrom(stream));
                }
                return environments;
            }
        }
    }
}
