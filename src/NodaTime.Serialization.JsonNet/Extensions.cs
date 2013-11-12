// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using Newtonsoft.Json;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Static class containing extension methods to configure Json.NET for Noda Time types.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Configures Json.NET with everything required to properly serialize and deserialize NodaTime data types.
        /// </summary>
        /// <param name="settings">The existing settings to add Noda Time converters to.</param>
        /// <param name="provider">The time zone provider to use when parsing time zones and zoned date/times.</param>
        /// <returns>The original <paramref name="settings"/> value, for further chaining.</returns>
        public static JsonSerializerSettings ConfigureForNodaTime(this JsonSerializerSettings settings, IDateTimeZoneProvider provider)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            // Add our converters
            settings.Converters.Add(NodaConverters.InstantConverter);
            settings.Converters.Add(NodaConverters.IntervalConverter);
            settings.Converters.Add(NodaConverters.LocalDateConverter);
            settings.Converters.Add(NodaConverters.LocalDateTimeConverter);
            settings.Converters.Add(NodaConverters.LocalTimeConverter);
            settings.Converters.Add(NodaConverters.OffsetConverter);
            settings.Converters.Add(NodaConverters.CreateDateTimeZoneConverter(provider));
            settings.Converters.Add(NodaConverters.DurationConverter);
            settings.Converters.Add(NodaConverters.RoundtripPeriodConverter);
            settings.Converters.Add(NodaConverters.OffsetDateTimeConverter);
            settings.Converters.Add(NodaConverters.CreateZonedDateTimeConverter(provider));

            // Disable automatic conversion of anything that looks like a date and time to BCL types.
            settings.DateParseHandling = DateParseHandling.None;

            // return to allow fluent chaining if desired
            return settings;
        }

        /// <summary>
        /// Configures Json.NET with everything required to properly serialize and deserialize NodaTime data types.
        /// </summary>
        /// <param name="serializer">The existing serializer to add Noda Time converters to.</param>
        /// <param name="provider">The time zone provider to use when parsing time zones and zoned date/times.</param>
        /// <returns>The original <paramref name="serializer"/> value, for further chaining.</returns>
        public static JsonSerializer ConfigureForNodaTime(this JsonSerializer serializer, IDateTimeZoneProvider provider)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            // Add our converters
            serializer.Converters.Add(NodaConverters.InstantConverter);
            serializer.Converters.Add(NodaConverters.IntervalConverter);
            serializer.Converters.Add(NodaConverters.LocalDateConverter);
            serializer.Converters.Add(NodaConverters.LocalDateTimeConverter);
            serializer.Converters.Add(NodaConverters.LocalTimeConverter);
            serializer.Converters.Add(NodaConverters.OffsetConverter);
            serializer.Converters.Add(NodaConverters.CreateDateTimeZoneConverter(provider));
            serializer.Converters.Add(NodaConverters.DurationConverter);
            serializer.Converters.Add(NodaConverters.RoundtripPeriodConverter);
            serializer.Converters.Add(NodaConverters.OffsetDateTimeConverter);
            serializer.Converters.Add(NodaConverters.CreateZonedDateTimeConverter(provider));

            // Disable automatic conversion of anything that looks like a date and time to BCL types.
            serializer.DateParseHandling = DateParseHandling.None;

            // return to allow fluent chaining if desired
            return serializer;
        }
    }
}
