// Copyright 2024 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Threading;

namespace NodaTime.Benchmarks.Custom;

internal class ConcurrentNewString : ConcurrencyCommandBase
{
    private ConcurrentNewString(int threadCount, Duration executionTime) : base(threadCount, executionTime)
    {
    }

    protected override void DoWork()
    {
        while (Interlocked.Add(ref iterations, ChunkSize) > 0)
        {
            for (int i = 0; i < ChunkSize; i++)
            {
                _ = new string('x', 8);
            }
        }
    }

    internal static void Execute(string[] args)
    {
        if (args.Length != 2)
        {
            throw new ConfigurationException("Command arguments: <processing thread count> <execution time in seconds>");
        }

        int threadCount = int.Parse(args[0]);
        int executionTimeSeconds = int.Parse(args[1]);
        var command = new ConcurrentNewString(threadCount, Duration.FromSeconds(executionTimeSeconds));
        command.Execute();
    }
}
