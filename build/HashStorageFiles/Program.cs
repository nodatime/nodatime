// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace HashStorageFiles
{
    /// <summary>
    /// Downloads all Noda Time releases from Google Cloud Storage, and validates or populates
    /// the SHA-256 hash.
    /// </summary>
    public class Program
    {
        private const string Bucket = "nodatime";
        private const string Prefix = "releases/";
        private const string Sha256Key = "SHA-256";

        public static void Main(string[] args)
        {

            bool validate = args.Length == 1 && args[0] == "--validate";
            var client = StorageClient.Create();
            var files = client.ListObjects(Bucket, Prefix).Where(x => !x.Name.EndsWith("/")).ToList();

            using (SHA256 sha = SHA256.Create())
            {
                foreach (var file in files)
                {
                    if (file.Metadata == null)
                    {
                        file.Metadata = new Dictionary<string, string>();
                    }
                    bool hashExists = file.Metadata.TryGetValue(Sha256Key, out string existingHash);
                    if (hashExists && !validate)
                    {
                        Console.WriteLine($"Skipping {file.Name}");
                        continue;
                    }
                    Console.WriteLine($"{(hashExists ? "Validating" : "Hashing")} {file.Name}");
                    var stream = new MemoryStream();
                    client.DownloadObject(file, stream, new DownloadObjectOptions { IfGenerationMatch = file.Generation });
                    stream.Position = 0;
                    byte[] hash = sha.ComputeHash(stream);
                    string hex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    if (hashExists)
                    {
                        if (hex != existingHash)
                        {
                            Console.WriteLine($"ERROR: Existing hash: {existingHash}; computed hash: {hex}");
                        }
                    }
                    else
                    {
                        file.Metadata[Sha256Key] = hex;
                        client.UpdateObject(file);
                    }
                }
            }
        }
    }
}
