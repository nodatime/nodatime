// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Newtonsoft.Json;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Static class containing extension methods to configure Json.NET for Noda Time types.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Configures json.net with everything required to properly serialize and deserialize NodaTime data types.
        /// </summary>
        public static JsonSerializerSettings ConfigureForNodaTime(this JsonSerializerSettings settings, IDateTimeZoneProvider provider)
        {
            // add our converters
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

            // return to allow fluent chaining if desired
            return settings;
        }

        /// <summary>
        /// Configures json.net with everything required to properly serialize and deserialize NodaTime data types.
        /// </summary>
        public static JsonSerializer ConfigureForNodaTime(this JsonSerializer serializer, IDateTimeZoneProvider provider)
        {
            // add our converters
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

            // return to allow fluent chaining if desired
            return serializer;
        }
    }
}
