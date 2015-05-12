// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
                throw new ArgumentNullException(nameof(settings));
            }
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            // Add our converters
            AddDefaultConverters(settings.Converters, provider);

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
                throw new ArgumentNullException(nameof(serializer));
            }
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            // Add our converters
            AddDefaultConverters(serializer.Converters, provider);

            // Disable automatic conversion of anything that looks like a date and time to BCL types.
            serializer.DateParseHandling = DateParseHandling.None;

            // return to allow fluent chaining if desired
            return serializer;
        }

        /// <summary>
        /// Configures the given serializer settings to use <see cref="NodaConverters.IsoIntervalConverter"/>.
        /// Any other converters which can convert <see cref="Interval"/> are removed from the serializer.
        /// </summary>
        /// <param name="settings">The existing serializer settings to add Noda Time converters to.</param>
        /// <returns>The original <paramref name="settings"/> value, for further chaining.</returns>
        public static JsonSerializerSettings WithIsoIntervalConverter(this JsonSerializerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            ReplaceExistingConverters<Interval>(settings.Converters, NodaConverters.IsoIntervalConverter);
            return settings;
        }

        /// <summary>
        /// Configures the given serializer to use <see cref="NodaConverters.IsoIntervalConverter"/>.
        /// Any other converters which can convert <see cref="Interval"/> are removed from the serializer.
        /// </summary>
        /// <param name="serializer">The existing serializer to add Noda Time converters to.</param>
        /// <returns>The original <paramref name="serializer"/> value, for further chaining.</returns>
        public static JsonSerializer WithIsoIntervalConverter(this JsonSerializer serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }
            ReplaceExistingConverters<Interval>(serializer.Converters, NodaConverters.IsoIntervalConverter);
            return serializer;
        }

        private static void AddDefaultConverters(IList<JsonConverter> converters, IDateTimeZoneProvider provider)
        {
            converters.Add(NodaConverters.InstantConverter);
            converters.Add(NodaConverters.IntervalConverter);
            converters.Add(NodaConverters.LocalDateConverter);
            converters.Add(NodaConverters.LocalDateTimeConverter);
            converters.Add(NodaConverters.LocalTimeConverter);
            converters.Add(NodaConverters.OffsetConverter);
            converters.Add(NodaConverters.CreateDateTimeZoneConverter(provider));
            converters.Add(NodaConverters.DurationConverter);
            converters.Add(NodaConverters.RoundtripPeriodConverter);
            converters.Add(NodaConverters.OffsetDateTimeConverter);
            converters.Add(NodaConverters.CreateZonedDateTimeConverter(provider));
        }

        private static void ReplaceExistingConverters<T>(IList<JsonConverter> converters, JsonConverter newConverter)
        {
            for (int i = converters.Count - 1; i >= 0; i--)
            {
                if (converters[i].CanConvert(typeof(T)))
                {
                    converters.RemoveAt(i);
                }
            }
            converters.Add(newConverter);
        }
    }
}
