// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using Newtonsoft.Json;
using NodaTime.Text;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Convenience class to expose preconfigured converters for Noda Time types, and factory methods
    /// for creating those which require parameters.
    /// </summary>
    public static class NodaConverters
    {
        /// <summary>
        /// Converter for instants, using the ISO-8601 date/time pattern, extended as required to accommodate milliseconds and ticks, and
        /// specifying 'Z' at the end to show it's effectively in UTC.
        /// </summary>
        public static readonly JsonConverter InstantConverter = new NodaPatternConverter<Instant>(InstantPattern.ExtendedIsoPattern);

        /// <summary>
        /// Converter for local dates, using the ISO-8601 date pattern.
        /// </summary>
        public static readonly JsonConverter LocalDateConverter = new NodaPatternConverter<LocalDate>(
            LocalDatePattern.IsoPattern, CreateIsoValidator<LocalDate>(x => x.Calendar));

        /// <summary>
        /// Converter for local dates and times, using the ISO-8601 date/time pattern, extended as required to accommodate milliseconds and ticks.
        /// No time zone designator is applied.
        /// </summary>
        public static readonly JsonConverter LocalDateTimeConverter = new NodaPatternConverter<LocalDateTime>(
            LocalDateTimePattern.ExtendedIsoPattern, CreateIsoValidator<LocalDateTime>(x => x.Calendar));

        /// <summary>
        /// Converter for local times, using the ISO-8601 time pattern, extended as required to accommodate milliseconds and ticks.
        /// </summary>
        public static readonly JsonConverter LocalTimeConverter = new NodaPatternConverter<LocalTime>(LocalTimePattern.ExtendedIsoPattern);

        /// <summary>
        /// Converter for intervals. This must be used in a serializer which also has an instant converter.
        /// </summary>
        public static readonly JsonConverter IntervalConverter = new NodaIntervalConverter();

        /// <summary>
        /// Converter for intervals using extended ISO-8601 format, as output by <see cref="Interval.ToString"/>.
        /// </summary>
        public static readonly JsonConverter IsoIntervalConverter = new NodaIsoIntervalConverter();

        /// <summary>
        /// Converter for offsets.
        /// </summary>
        public static readonly JsonConverter OffsetConverter = new NodaPatternConverter<Offset>(OffsetPattern.GeneralInvariantPattern);

        /// <summary>
        /// Converter for offset date/times.
        /// </summary>
        public static readonly JsonConverter OffsetDateTimeConverter = new NodaPatternConverter<OffsetDateTime>(
            OffsetDateTimePattern.Rfc3339Pattern, CreateIsoValidator<OffsetDateTime>(x => x.Calendar));

        /// <summary>
        /// Creates a converter for zoned date/times, using the given time zone provider.
        /// </summary>
        /// <param name="provider">The time zone provider to use when parsing.</param>
        /// <returns>A converter to handle <see cref="ZonedDateTime"/>.</returns>
        public static JsonConverter CreateZonedDateTimeConverter(IDateTimeZoneProvider provider)
        {
            return new NodaPatternConverter<ZonedDateTime>(
                ZonedDateTimePattern.CreateWithInvariantCulture("yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z", provider),
                CreateIsoValidator<ZonedDateTime>(x => x.Calendar));
        }

        /// <summary>
        /// Creates a converter for time zones, using the given provider.
        /// </summary>
        /// <param name="provider">The time zone provider to use when parsing.</param>
        /// <returns>A converter to handle <see cref="DateTimeZone"/>.</returns>
        public static JsonConverter CreateDateTimeZoneConverter(IDateTimeZoneProvider provider)
        {
            return new NodaDateTimeZoneConverter(provider);
        }

        /// <summary>
        /// Converter for durations.
        /// </summary>
        public static readonly JsonConverter DurationConverter = new NodaPatternConverter<Duration>(DurationPattern.CreateWithInvariantCulture("-H:mm:ss.FFFFFFFFF"));

        /// <summary>
        /// Round-tripping converter for periods. Use this when you really want to preserve information,
        /// and don't need interoperability with systems expecting ISO.
        /// </summary>
        public static readonly JsonConverter RoundtripPeriodConverter = new NodaPatternConverter<Period>(PeriodPattern.RoundtripPattern);

        /// <summary>
        /// Normalizing ISO converter for periods. Use this when you want compatibility with systems expecting
        /// ISO durations (~= Noda Time periods). However, note that Noda Time can have negative periods. Note that
        /// this converter losees information - after serialization and deserialization, "90 minutes" will become "an hour and 30 minutes".
        /// </summary>
        public static readonly JsonConverter NormalizingIsoPeriodConverter = new NodaPatternConverter<Period>(PeriodPattern.NormalizingIsoPattern);

        private static Action<T> CreateIsoValidator<T>(Func<T, CalendarSystem> calendarProjection)
        {
            return value => {
                var calendar = calendarProjection(value);
                // We rely on CalendarSystem.Iso being a singleton here.
                if (calendar != CalendarSystem.Iso)
                {
                    throw new ArgumentException(
                        $"Values of type {typeof (T).Name} must (currently) use the ISO calendar in order to be serialized.");
                }
            };
        }
    }
}
