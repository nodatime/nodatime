// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NodaTime.Benchmarks.Framework;
using NodaTime.Serialization.JsonNet;

namespace NodaTime.Benchmarks.NodaTimeTests.JsonNet
{
    internal sealed class FormattingBenchmarks
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

        private static readonly Instant sampleInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
        private static readonly DateTimeZone sampleDateTimeZone = DateTimeZoneProviders.Tzdb["Europe/London"];
        private static readonly LocalDateTime sampleLocalDateTime = new LocalDateTime(2013, 12, 11, 8, 31, 20, 123, 4567);
        private static readonly LocalDate sampleLocalDate = sampleLocalDateTime.Date;
        private static readonly LocalTime sampleLocalTime = sampleLocalDateTime.TimeOfDay;
        private static readonly Offset sampleOffset = Offset.FromHoursAndMinutes(5, 30);
        private static readonly OffsetDateTime sampleOffsetDateTime = sampleLocalDateTime.WithOffset(sampleOffset);
        private static readonly ZonedDateTime sampleZonedDateTime = sampleInstant.InZone(sampleDateTimeZone);
        private static readonly Duration sampleDuration = new Duration(1234567890L);
        private static readonly Period samplePeriod = new PeriodBuilder { Days = 1, Hours = 20, Seconds = 3, Milliseconds = 5 }.Build();
        private static readonly Interval sampleInterval = new Interval(sampleInstant, sampleInstant + sampleDuration);

        [Benchmark]
        public void FormatInstant()
        {
            FormatJson(sampleInstant);
        }

        [Benchmark]
        public void FormatLocalDate()
        {
            FormatJson(sampleLocalDate);
        }

        [Benchmark]
        public void FormatLocalTime()
        {
            FormatJson(sampleLocalTime);
        }

        [Benchmark]
        public void FormatLocalDateTime()
        {
            FormatJson(sampleLocalDateTime);
        }

        [Benchmark]
        public void FormatOffsetDateTime()
        {
            FormatJson(sampleOffsetDateTime);
        }

        [Benchmark]
        public void FormatZonedDateTime()
        {
            FormatJson(sampleZonedDateTime);
        }

        [Benchmark]
        public void FormatInterval()
        {
            FormatJson(sampleInterval);
        }

        [Benchmark]
        public void FormatOffset()
        {
            FormatJson(sampleOffset);
        }

        [Benchmark]
        public void FormatDuration()
        {
            FormatJson(sampleDuration);
        }

        [Benchmark]
        public void FormatPeriod()
        {
            FormatJson(samplePeriod);
        }

        [Benchmark]
        public void FormatDateTimeZone()
        {
            FormatJson(sampleDateTimeZone);
        }

        private static void FormatJson<T>(T value)
        {
            JsonConvert.SerializeObject(value, Formatting.None, settings);
        }
    }
}
