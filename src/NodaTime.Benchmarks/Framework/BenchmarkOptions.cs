// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using CommandLine.Text;
using System;

namespace NodaTime.Benchmarks.Framework
{
    /// <summary>
    /// Encapsulates all the options for benchmarking, such as
    /// the approximate length of each test, the timer to use
    /// and so on.
    /// </summary>
    internal class BenchmarkOptions
    {
        private BenchmarkOptions()
        {
        }

        internal Duration WarmUpTime { get; private set; }
        internal Duration TestTime { get; private set; }
        internal IBenchTimer Timer { get; private set; }
        internal string TypeFilter { get; private set; }
        internal string MethodFilter { get; private set; }
        internal bool DisplayRawData { get; private set; }

        private class MutableOptions
        {
            [Option("w", "warmup", HelpText = "Duration of warm-up time per test, in seconds. Default=1")]
            public int WarmUpTimeSeconds { get; set; }
            [Option("d", "duration", HelpText = "Targeted per-test duration, in seconds. Default=10")]
            public int TestTimeSeconds { get; set; }
            [Option("t", "type", HelpText = "Type filter")]
            public string TypeFilter { get; set; }
            [Option("m", "method", HelpText = "Method filter")]
            public string MethodFilter { get; set; }
            [Option("r", "raw", HelpText = "Whether or not to display the raw data.")]
            public bool DisplayRawData { get; set; }

            [HelpOption("?", "help", HelpText = "Display this help screen.")]
            public string GetUsage()
            {
                var help = new HelpText(new HeadingInfo("NodaTime.Benchmarks"))
                {
                    AdditionalNewLineAfterOption = true,
                };
                help.AddOptions(this);
                return help;
            }
        }

        internal static BenchmarkOptions FromCommandLine(string[] args)
        {
            MutableOptions options = new MutableOptions()
            {
                WarmUpTimeSeconds = 1,
                TestTimeSeconds = 10
            };
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options))
            {
                return null;
            }

            return new BenchmarkOptions
            {
                TypeFilter = options.TypeFilter,
                MethodFilter = options.MethodFilter,
                WarmUpTime = Duration.FromSeconds(options.WarmUpTimeSeconds),
                TestTime = Duration.FromSeconds(options.TestTimeSeconds),
                Timer = new WallTimer(),
                DisplayRawData = options.DisplayRawData
            };
        }
    }
}