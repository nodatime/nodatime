// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

#if SERIALIZATION_RELEASED && !V1_0 && !V1_1

using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using NodaTime.Serialization.JsonNet;

namespace NodaTime.Benchmarks.NodaTimeTests.JsonNet
{
    public class ParsingBenchmarks
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

        [Benchmark]
        public void ParseInstant()
        {
            ParseJson<Instant>("\"2012-01-02T03:04:05Z\"");
        }

        [Benchmark]
        public void ParseLocalDate()
        {
            ParseJson<LocalDate>("\"2012-01-02\"");
        }

        [Benchmark]
        public void ParseLocalTime()
        {
            ParseJson<LocalTime>("\"07:12:15\"");
        }

        [Benchmark]
        public void ParseLocalDateTime()
        {
            ParseJson<LocalDateTime>("\"2012-01-02T07:12:15\"");
        }

        [Benchmark]
        public void ParseOffsetDateTime()
        {
            ParseJson<OffsetDateTime>("\"2012-01-02T07:12:15+01:00\"");
        }

        [Benchmark]
        public void ParseZonedDateTime()
        {
            ParseJson<ZonedDateTime>("\"2012-06-02T07:12:15+01:00 Europe/London\"");
        }

        [Benchmark]
        public void ParseInterval()
        {
            ParseJson<Interval>("{\"Start\":\"2012-01-02T03:04:05Z\",\"End\":\"2013-06-07T08:09:10Z\"}");
        }

        [Benchmark]
        public void ParseOffset()
        {
            ParseJson<Offset>("\"-03:30\"");
        }

        [Benchmark]
        public void ParseDuration()
        {
            ParseJson<Duration>("\"48:00:03.1234567\"");
        }

        [Benchmark]
        public void ParsePeriod()
        {
            ParseJson<Period>("\"P2DT4H30M\"");
        }

        [Benchmark]
        public void ParseDateTimeZone()
        {
            ParseJson<DateTimeZone>("\"Europe/London\"");
        }

        private static void ParseJson<T>(string json)
        {
            JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}
#endif
