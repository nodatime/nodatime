// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Benchmarks.Framework;

namespace NodaTime.Benchmarks
{
    /// <summary>
    /// Entry point for benchmarking.
    /// </summary>
    internal class Program
    {
        private static int Main(string[] args)
        {
            var options = BenchmarkOptions.FromCommandLine(args);
            // Help screen / error
            if (options == null)
            {
                return 1;
            }
            var handler = CreateResultHandler(options);
            var runner = new BenchmarkRunner(options, handler);
            runner.RunTests();
            return 0;
        }

        private static BenchmarkResultHandler CreateResultHandler(BenchmarkOptions options)
        {
            BenchmarkResultHandler handler = new ConsoleResultHandler(options.DisplayRawData);
            if (options.XmlFile != null)
            {
                var xmlHandler = new XmlResultHandler(options.XmlFile);
                handler = new CompositeResultHandler(new[] { handler, xmlHandler });
            }
            return handler;
        }
    }
}
