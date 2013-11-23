// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace NodaTime.Benchmarks.Analyzer
{
    class Program
    {
        private static readonly Uri BaseUri = new Uri("http://nodatime.org/benchmarks/");

        static int Main(string[] args)
        {
            Options options = new Options();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options))
            {
                return 1;
            }
            if (!options.OfflineMode)
            {
                DownloadBenchmarks(options.Directory);
            }
            var benchmarks = LoadBenchmarks(options.Directory, options.Machine);
            foreach (var group in benchmarks.GroupBy(file => file.Machine))
            {
                AnalyzeResults(group, options);
            }
            return 0;
        }

        static void DownloadBenchmarks(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (WebClient client = new WebClient())
            {
                string index = client.DownloadString(new Uri(BaseUri, "index.txt"));
                var names = index.Split('\n').Select(x => x.Trim()).Where(x => x != "").ToList();
                Console.WriteLine("Site holds {0} benchmarks", names.Count);
                foreach (var name in names)
                {
                    string downloadedPath = Path.Combine(directory, name);
                    if (File.Exists(downloadedPath))
                    {
                        Console.WriteLine("{0} exists: skipping", name);
                    }
                    else
                    {
                        Console.WriteLine("Downloading {0}", name);
                        client.DownloadFile(new Uri(BaseUri, name), downloadedPath);
                    }
                }
            }
        }

        static IEnumerable<BenchmarkFile> LoadBenchmarks(string directory, string machine)
        {
            return Directory.GetFiles(directory, "*.xml")
                            .Select(file => BenchmarkFile.FromXDocument(XDocument.Load(file)))
                            .Where(file => machine == null || file.Machine == machine)
                            .OrderBy(file => file.StartTime);
        }

        static void AnalyzeResults(IEnumerable<BenchmarkFile> files, Options options)
        {
            Console.WriteLine("Results for {0}", files.First().Machine);
            var methodResults = from file in files
                                from result in file.Results
                                group new { file, result } by result.FullyQualifiedMethod;

            foreach (var resultSet in methodResults)
            {
                var method = resultSet.First().result.Type + "." + resultSet.First().result.Method;
                var results = Smooth(resultSet,
                                     pairs => new { StartTime = pairs.First().file.StartTime,
                                                    Label = pairs.First().file.Label,
                                                    Average = pairs.Average(x => x.result.NanosecondsPerCall) },
                                     options.SmoothingCount);
                var resultPairs = results.Zip(results.Skip(1), (previous, current) => new { previous, current });
                foreach (var pair in resultPairs)
                {
                    var currentAverage = (int) pair.current.Average;
                    var previousAverage = (int)pair.previous.Average;
                    var start = pair.current.StartTime;
                    var label = pair.current.Label ?? "None";
                    if (currentAverage * 100 < previousAverage * options.ImprovementThreshold)
                    {
                        Console.WriteLine("{0:yyyy-MM-dd} ({1}): Improvement\r\n{2}: {3} to {4} ns per call",
                            start, label, method, previousAverage, currentAverage);
                    }
                    if (pair.current.Average * 100 > pair.previous.Average * options.RegressionThreshold)
                    {
                        Console.WriteLine("{0:yyyy-MM-dd} ({1}): Regression\r\n{2} from {3} to {4} ns per call",
                            start, label, method, previousAverage, currentAverage);
                    }
                }
            }
        }

        static IEnumerable<TResult> Smooth<TItem, TResult>(IEnumerable<TItem> source,
            Func<IEnumerable<TItem>, TResult> smoothingFunction, int count)
        {
            Queue<List<TItem>> queue = new Queue<List<TItem>>();
            foreach (var item in source)
            {
                if (queue.Count < count)
                {
                    queue.Enqueue(new List<TItem>());
                }
                foreach (var list in queue)
                {
                    list.Add(item);
                }
                if (queue.Count == count)
                {
                    yield return smoothingFunction(queue.Dequeue());
                    queue.Enqueue(new List<TItem>());
                }
            }
        }
    }
}
