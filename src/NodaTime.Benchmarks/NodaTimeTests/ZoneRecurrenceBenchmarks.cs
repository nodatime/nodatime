// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.TimeZones;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class ZoneRecurrenceBenchmarks
    {
        private static readonly ZoneRecurrence SampleRecurrence =
            new ZoneRecurrence("Summer", Offset.FromHours(1), new ZoneYearOffset(TransitionMode.Wall, 3, 15, (int)IsoDayOfWeek.Sunday, true, new LocalTime(1, 0, 0)), int.MinValue, int.MaxValue);

        private static readonly Instant WinterInstant = Instant.FromUtc(2010, 2, 4, 5, 10);
        private static readonly Instant SummerInstant = Instant.FromUtc(2010, 6, 19, 5, 10);

        // Note: we can't declare the actual return type, as it's internal. So we wrap it.
        
        [Benchmark]
        public NullableTransitionWrapper Next() => new NullableTransitionWrapper(SampleRecurrence.Next(WinterInstant, Offset.Zero, Offset.Zero));

        [Benchmark]
        public TransitionWrapper NextOrFail() => new TransitionWrapper(SampleRecurrence.NextOrFail(WinterInstant, Offset.Zero, Offset.Zero));

        [Benchmark]
        public NullableTransitionWrapper PreviousOrSame() => new NullableTransitionWrapper(SampleRecurrence.PreviousOrSame(WinterInstant, Offset.Zero, Offset.Zero));

        [Benchmark]
        public TransitionWrapper PreviousOrSameOrFail() => new TransitionWrapper(SampleRecurrence.PreviousOrSameOrFail(WinterInstant, Offset.Zero, Offset.Zero));

        public struct TransitionWrapper
        {
            private readonly Transition transition;

            internal TransitionWrapper(Transition transition)
            {
                this.transition = transition;
            }
        }

        public struct NullableTransitionWrapper
        {
            private readonly Transition? transition;

            internal NullableTransitionWrapper(Transition? transition)
            {
                this.transition = transition;
            }
        }

    }
}
