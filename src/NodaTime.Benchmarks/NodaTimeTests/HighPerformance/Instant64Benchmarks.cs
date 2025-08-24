// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Globalization;
using BenchmarkDotNet.Attributes;
using NodaTime.HighPerformance;

namespace NodaTime.Benchmarks.NodaTimeTests.HighPerformance;

public class Instant64Benchmarks
{
    private static readonly Instant64 Sample = Instant64.FromUtc(2011, 8, 24, 12, 29, 30);

    [Benchmark]
    public string ToStringIso() => Sample.ToString("g", CultureInfo.InvariantCulture);

    [Benchmark]
    public Instant64 PlusDuration() => Sample.Plus(Duration64.Epsilon);

    [Benchmark]
    public Instant64 PlusNanoseconds() => Sample.PlusNanoseconds(100);
}
