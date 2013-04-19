// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Benchmarks.Framework
{
    internal sealed class ConsoleResultHandler : BenchmarkResultHandler
    {
        private const string LongFormatString = "  {0}: {1:N0} cps; ({2:N0} iterations in {3:N0} ticks; {4:N0} ticks per iteration)";
        private const string ShortFormatString = "  {0}: {1:N0} cps ({4:N0} tpc)";

        private readonly string formatString;

        internal ConsoleResultHandler(bool rawResults)
        {
            formatString = rawResults ? LongFormatString : ShortFormatString;
        }

        internal override void HandleStartRun(BenchmarkOptions options)
        {
            Console.WriteLine("Environment: CLR {0} on {1}", Environment.Version, Environment.OSVersion);
            if (options.Identifier != null)
            {
                Console.WriteLine("Run identifier: {0}", options.Identifier);
            }
        }

        internal override void HandleStartType(Type type)
        {
            Console.WriteLine("Running benchmarks in {0}", GetTypeDisplayName(type));
        }

        internal override void HandleResult(BenchmarkResult result)
        {
            Console.WriteLine(formatString, result.Method.Name, result.CallsPerSecond,
                result.Iterations, result.Duration.Ticks, result.TicksPerCall);
        }

        private static string GetTypeDisplayName(Type type)
        {
            return type.FullName.Replace("NodaTime.Benchmarks.", "");
        }
    }
}
