// Copyright 2025 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using BenchmarkDotNet.Attributes;
using NodaTime.HighPerformance;

namespace NodaTime.Benchmarks.NodaTimeTests.HighPerformance;

public class Duration64Benchmarks
{
    private static readonly Duration SampleNormalDuration = new Duration(5, 10000);
    private static readonly Duration64 SampleDuration1 = Duration64.FromNanoseconds(1 * NodaConstants.NanosecondsPerDay + 500000);
    private static readonly Duration64 SampleDuration2 = Duration64.FromNanoseconds(2 * NodaConstants.NanosecondsPerDay + 300000);

    /// <summary>
    /// A duration which is less than a day (which may allow for some optimizations in Duration, but probably won't affect Duration64).
    /// </summary>
    private static readonly Duration64 SmallDuration64 = Duration64.FromDuration(Duration.FromHours(9) + Duration.FromMinutes(20));

    /// <summary>
    /// A duration which is just over a year.
    /// </summary>
    private static readonly Duration64 MediumDuration64 = Duration64.FromDuration(Duration.FromDays(400) + Duration.FromHours(10));

    [Benchmark]
    public Duration64 FromNanoseconds_TinyInt64() => Duration64.FromNanoseconds(50);
    
    [Benchmark]
    public Duration64 FromNanoseconds_MediumInt64() => Duration64.FromNanoseconds(50L * 365 * NodaConstants.NanosecondsPerDay);

    [Benchmark]
    public Duration64 Minus_Simple() => SampleDuration1 - SampleDuration2;

    // This is more complex for Duration, but simple for Duration64.
    [Benchmark]
    public Duration64 Minus_Complex() => SampleDuration2 - SampleDuration1;

    [Benchmark]
    public Duration64 Add_SmallSmall() => SmallDuration64 + SmallDuration64;

    [Benchmark]
    public Duration64 Add_MediumMedium() => MediumDuration64 + MediumDuration64;

    [Benchmark]
    public double TotalNanoseconds_Small() => SmallDuration64.TotalNanoseconds;

    [Benchmark]
    public double TotalNanoseconds_Medium() => MediumDuration64.TotalNanoseconds;

    [Benchmark]
    public Duration64 Zero() => Duration64.Zero;

    [Benchmark]
    public Duration ToDuration() => SmallDuration64.ToDuration();

    [Benchmark]
    public Duration64 FromDuration() => Duration64.FromDuration(SampleNormalDuration);
}
