// Copyright 2024 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

// This program is for any custom benchmarks that can't easily be tested with BenchmarkDotNet.
// (Where things become possible over time, we should migrate benchmarks to BenchmarkDotNet.)
// These benchmarks are not run or recorded automatically.

using NodaTime.Benchmarks.Custom;
using System;
using System.Collections.Generic;
using System.Linq;

Dictionary<string, Action<string[]>> commands = new()
{
    { "concurrent-tostring", ConcurrentToString.Execute },
    { "concurrent-patternformat", ConcurrentPatternFormat.Execute },
    { "concurrent-newstring", ConcurrentNewString.Execute },
    { "concurrent-noop", ConcurrentNoOp.Execute },
};

if (args.Length < 1)
{
    Console.WriteLine("Please specify a command:");
    Console.WriteLine(string.Join(" ", commands.Keys.OrderBy(x => x)));
    return 1;
}

if (!commands.TryGetValue(args[0], out var command))
{
    Console.WriteLine("Unknown command. Available commands:");
    Console.WriteLine(string.Join(" ", commands.Keys.OrderBy(x => x)));
    return 1;
}

try
{
    command.Invoke(args.Skip(1).ToArray());
}
catch (ConfigurationException e)
{
    Console.WriteLine(e.Message);
    return 1;
}
return 0;
