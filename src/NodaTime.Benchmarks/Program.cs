// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NodaTime.Benchmarks.Framework;
using NodaTime.Text;

namespace NodaTime.Benchmarks
{
    /// <summary>
    /// Entry point for benchmarking.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            var options = BenchmarkOptions.FromCommandLine(args);
            // Help screen / error
            if (options == null)
            {
                return;
            }
            var handler = CreateResultHandler(options);
            var runner = new BenchmarkRunner(options, handler);
            runner.RunTests();
        }

        private static BenchmarkResultHandler CreateResultHandler(BenchmarkOptions options)
        {
            BenchmarkResultHandler handler = new ConsoleResultHandler(options.DisplayRawData);
            if (options.WriteToXml)
            {
                string path = options.OutputFile;
                if (path == null)
                {
                    string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NodaTime.Benchmarks");
                    Directory.CreateDirectory(folder);
                    path = Path.Combine(folder, string.Format("benchmarks-{0:yyyyMMdd-HHmm}.xml", SystemClock.Instance.Now));
                }
                var xmlHandler = new XmlResultHandler(path);
                handler = new CompositeResultHandler(new[] { handler, xmlHandler });
            }
            return handler;
        }
    }
}
