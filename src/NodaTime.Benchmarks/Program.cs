// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Running;
using System.Reflection;

namespace NodaTime.Benchmarks
{
    /// <summary>
    /// Entry point for benchmarking.
    /// </summary>
    public class Program
    {
        // Run it with args = { "*" } for choosing all of target benchmarks
        public static void Main(string[] args)
        {
            new BenchmarkSwitcher(typeof(Program).GetTypeInfo().Assembly).Run(args);
        }
    }
}
