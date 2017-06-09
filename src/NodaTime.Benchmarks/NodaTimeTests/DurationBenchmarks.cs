// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using BenchmarkDotNet.Attributes;
using System.Numerics;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class DurationBenchmarks
    {
        private static readonly TimeSpan SampleTimeSpan = new TimeSpan(1, 2, 3);
        private static readonly Duration SampleDuration1 = new Duration(1, 500000);
        private static readonly Duration SampleDuration2 = new Duration(2, 300000);

        /// <summary>
        /// A duration which is less than a day (which may allow for some optimizations).
        /// </summary>
        private static readonly Duration SmallDuration = Duration.FromHours(9) + Duration.FromMinutes(20);

        /// <summary>
        /// A duration which is just over a year.
        /// </summary>
        private static readonly Duration MediumDuration = Duration.FromDays(400) + Duration.FromHours(10);

        /// <summary>
        /// A duration which isn't representable as a 64-bit integer number of nanoseconds.
        /// </summary>
        private static readonly Duration LargeDuration = Duration.FromDays(365 * 400) + Duration.FromHours(5);

        private static readonly BigInteger BigInteger50 = 50;
        private static readonly BigInteger BigInteger50Years = 50L * 365 * NodaConstants.NanosecondsPerDay;
        private static readonly BigInteger BigInteger3000Years = ((BigInteger) 365 * NodaConstants.NanosecondsPerDay) * 3000;

        [Benchmark]
        public Duration FromDays() =>
#if !V1
            Duration.FromDays(100);
#else
            Duration.FromStandardDays(100);
#endif

        // Durations where * is efficient
        [Benchmark]
        public Duration FromHours_Tiny() => Duration.FromHours(50);

        [Benchmark]
        public Duration FromMinutes_Tiny() => Duration.FromMinutes(50);

        [Benchmark]
        public Duration FromSeconds_Tiny() => Duration.FromSeconds(50);

        [Benchmark]
        public Duration FromMilliseconds_Tiny() => Duration.FromMilliseconds(50);

        [Benchmark]
        public Duration FromTicks_Tiny() => Duration.FromTicks(50);

        [Benchmark]
        public Duration FromNanoseconds_TinyInt64() => Duration.FromNanoseconds(50);

        [Benchmark]
        public Duration FromNanoseconds_TinyBigInteger() => Duration.FromNanoseconds(BigInteger50);

        // Durations of about 50 years (representative of Instant.FromUnixEpoch* for modern instants)
        [Benchmark]
        public Duration FromHours_Medium() => Duration.FromHours(50 * 365 * NodaConstants.HoursPerDay);

        [Benchmark]
        public Duration FromMinutes_Medium() => Duration.FromMinutes(50L * 365 * NodaConstants.MinutesPerDay);

        [Benchmark]
        public Duration FromSeconds_Medium() => Duration.FromSeconds(50L * 365 * NodaConstants.SecondsPerDay);

        [Benchmark]
        public Duration FromMilliseconds_Medium() => Duration.FromMilliseconds(50L * 365 * NodaConstants.MillisecondsPerDay);

        [Benchmark]
        public Duration FromTicks_Medium() => Duration.FromTicks(50L * 365 * NodaConstants.TicksPerDay);

        [Benchmark]
        public Duration FromNanoseconds_MediumInt64() => Duration.FromNanoseconds(50L * 365 * NodaConstants.NanosecondsPerDay);

        [Benchmark]
        public Duration FromNanoseconds_MediumBigInteger() => Duration.FromNanoseconds(BigInteger50Years);

        // Out of the range of Int64 nanoseconds: about 3000 years
        [Benchmark]
        public Duration FromHours_Large() => Duration.FromHours(3000 * 365 * NodaConstants.HoursPerDay);

        [Benchmark]
        public Duration FromMinutes_Large() => Duration.FromMinutes(3000L * 365 * NodaConstants.MinutesPerDay);

        [Benchmark]
        public Duration FromSeconds_Large() => Duration.FromSeconds(3000L * 365 * NodaConstants.SecondsPerDay);

        [Benchmark]
        public Duration FromMilliseconds_Large() => Duration.FromMilliseconds(3000L * 365 * NodaConstants.MillisecondsPerDay);

        [Benchmark]
        public Duration FromTicks_Large() => Duration.FromTicks(3000L * 365 * NodaConstants.TicksPerDay);

#if !V1
        [Benchmark]
        public Duration FromNanoseconds_LargeBigInteger() => Duration.FromNanoseconds(BigInteger3000Years);

        // No equivalent for Int64 as it won't fit...

#endif

        [Benchmark]
        public Duration FromTimeSpan() => Duration.FromTimeSpan(SampleTimeSpan);

        [Benchmark]
        public Duration Minus_Simple() => SampleDuration1 - SampleDuration2;
        
        // This is more complex because subtracting the nanos gives us a negative nanos value
        // which we need to correct.
        [Benchmark]
        public Duration Minus_Complex() => SampleDuration2 - SampleDuration1;

        [Benchmark]
        public Duration Multiply_SmallBoth() => SmallDuration * 25;

        [Benchmark]
        public Duration Multiply_SmallByDouble() => SmallDuration * 25.5;

        [Benchmark]
        public Duration Multiply_LargeByDouble() => LargeDuration * 25.5;

        // Here "large" is in the context of the optimizations we make
        [Benchmark]
        public Duration Multiply_SmallLeftLargeRight() => SmallDuration * 10000;

        [Benchmark]
        public Duration Multiply_MediumLeftSmallRight() => MediumDuration * 25;

        [Benchmark]
        public Duration Multiply_MediumLeftLargeRight() => MediumDuration * 10000;

        [Benchmark]
        public Duration Multiply_LargeLeftSmallRight() => LargeDuration * 25;

        [Benchmark]
        public Duration Add_SmallSmall() => SmallDuration + SmallDuration;

        [Benchmark]
        public Duration Add_MediumMedium() => MediumDuration + MediumDuration;

        [Benchmark]
        public Duration Add_MediumLarge() => MediumDuration + LargeDuration;

        [Benchmark]
        public long ToInt64Nanoseconds_Small() => SmallDuration.ToInt64Nanoseconds();

        [Benchmark]
        public long ToInt64Nanoseconds_Medium() => MediumDuration.ToInt64Nanoseconds();

        [Benchmark]
        public BigInteger ToBigIntegerNanoseconds_Small() => SmallDuration.ToBigIntegerNanoseconds();

        [Benchmark]
        public BigInteger ToBigIntegerNanoseconds_Medium() => MediumDuration.ToBigIntegerNanoseconds();

        [Benchmark]
        public BigInteger ToBigIntegerNanoseconds_Large() => LargeDuration.ToBigIntegerNanoseconds();

        [Benchmark]
        public double TotalNanoseconds_Small() => SmallDuration.TotalNanoseconds;

        [Benchmark]
        public double TotalNanoseconds_Medium() => MediumDuration.TotalNanoseconds;

        [Benchmark]
        public double TotalNanoseconds_Large() => LargeDuration.TotalNanoseconds;
    }
}
