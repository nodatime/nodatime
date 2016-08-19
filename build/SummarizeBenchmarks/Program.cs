// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SummarizeBenchmarks
{
    /// <summary>
    /// Loads all xyz-report-brief.json files for BenchmarkDotNet in a directory,
    /// and prints out the mean for each benchmark.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Specify the project or results directories");
                return;
            }

            var resultsByDirectory = args.Select(ParseDirectory).ToList();
            // Munge all the results together, relying on the order being preserved.
            var lookup = resultsByDirectory.SelectMany(r => r)
                                           .ToLookup(b => $"{b.Class}.{b.Method}", b => b.Mean);

            var discarded = lookup.Where(g => g.Count() != resultsByDirectory.Count);
            var retained = lookup.Where(g => g.Count() == resultsByDirectory.Count);

            foreach (var item in retained)
            {
                string means = string.Join("", item.Select(mean => $"{mean,10:0.#}"));
                Console.WriteLine($"{item.Key,-50} {means}");
            }
            Console.WriteLine();
            Console.WriteLine($"Discarded {discarded.Count()} results - not universally present.");
        }

        private static List<Benchmark> ParseDirectory(string directory)
        {
            string resultsDir = Path.Combine(directory, "BenchmarkDotNet.Artifacts", "results");
            if (Directory.Exists(resultsDir))
            {
                directory = resultsDir;
            }
            return Directory.GetFiles(directory, "*-report-brief.json")
                            .Select(f => JObject.Parse(File.ReadAllText(f)))
                            .SelectMany(report => Benchmark.ParseFile(report))
                            .ToList();
        }
    }
}
