// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Google.Cloud.Storage.V1;
using Google.Protobuf;
using NodaTime.Benchmarks;
using System;
using System.Collections.Generic;
using System.IO;

namespace BenchmarkUploader
{
    public class BenchmarkRepository
    {
        private const string EnvironmentObjectName = "environments.pb";
        private const string RunNameTemplate = "benchmark-run-{0}.pb";
        private readonly StorageClient client;
        private readonly string bucketName;
        private readonly string objectPrefix;
        private readonly IList<BenchmarkEnvironment> environments = new List<BenchmarkEnvironment>();
        private readonly IList<BenchmarkRun> runs = new List<BenchmarkRun>();
        private bool environmentSaveRequired;

        public BenchmarkRepository(StorageClient client, string bucketName, string objectPrefix)
        {
            this.client = client;
            this.bucketName = bucketName;
            this.objectPrefix = objectPrefix;
        }

        public void LoadEnvironments()
        {
            environments.Clear();
            var stream = new MemoryStream();
            client.DownloadObject(bucketName, objectPrefix + EnvironmentObjectName, stream);
            stream.Position = 0;
            while (stream.Position != stream.Length)
            {
                environments.Add(BenchmarkEnvironment.Parser.ParseDelimitedFrom(stream));
            }
            environmentSaveRequired = false;
        }

        public void AddRun(BenchmarkRun run)
        {
            runs.Add(run);
        }

        /// <summary>
        /// Fetches the ID for a given environment, adding it to the in-memory store
        /// and creating a new ID if necessary. If the environment is added, it will
        /// be a clone of the parameter. The parameter is not modified.
        /// </summary>
        /// <param name="environment">The environment to fetch/add</param>
        /// <returns>The ID of the environment</returns>
        public string GetOrAddEnvironmentId(BenchmarkEnvironment environment)
        {
            var environmentWithoutId = environment.Clone();
            environmentWithoutId.BenchmarkEnvironmentId = "";
            // TODO: Make this less ugly. Maybe create a Dictionary<BenchmarkEnvironment, string>.
            foreach (var candidate in environments)
            {
                var candidateWithoutId = candidate.Clone();
                candidateWithoutId.BenchmarkEnvironmentId = "";
                if (environmentWithoutId.Equals(candidateWithoutId))
                {
                    return candidate.BenchmarkEnvironmentId;
                }
            }
            // Not found, so let's add our clone after populating the ID.
            environmentWithoutId.BenchmarkEnvironmentId = Guid.NewGuid().ToString();
            environments.Add(environmentWithoutId);
            environmentSaveRequired = true;
            return environmentWithoutId.BenchmarkEnvironmentId;
        }

        public void Save()
        {
            if (environmentSaveRequired)
            {
                Console.WriteLine($"Uploading {environments.Count} environments");
                MemoryStream stream = new MemoryStream();
                foreach (var environment in environments)
                {
                    environment.WriteDelimitedTo(stream);
                }
                stream.Position = 0;
                client.UploadObject(bucketName, objectPrefix + EnvironmentObjectName, "application/x-protobuf", stream);
            }
            foreach (var run in runs)
            {
                Console.WriteLine($"Uploading run {run.BenchmarkRunId}");
                var name = objectPrefix + string.Format(RunNameTemplate, run.BenchmarkRunId);
                var stream = new MemoryStream(run.ToByteArray());
                client.UploadObject(bucketName, name, "application/x-protobuf", stream);
            }
        }
    }
}
