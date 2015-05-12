// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Newtonsoft.Json;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Json.NET converter for <see cref="DateTimeZone"/>.
    /// </summary>
    internal sealed class NodaDateTimeZoneConverter : NodaConverterBase<DateTimeZone>
    {
        private readonly IDateTimeZoneProvider provider;

        /// <param name="provider">Provides the <see cref="DateTimeZone"/> that corresponds to each time zone ID in the JSON string.</param>
        public NodaDateTimeZoneConverter(IDateTimeZoneProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Reads the time zone ID (which must be a string) from the reader, and converts it to a time zone.
        /// </summary>
        /// <param name="reader">The JSON reader to fetch data from.</param>
        /// <param name="serializer">The serializer for embedded serialization.</param>
        /// <exception cref="DateTimeZoneNotFoundException">The provider does not support a time zone with the given ID.</exception>
        /// <returns>The <see cref="DateTimeZone"/> identified in the JSON, or null.</returns>
        protected override DateTimeZone ReadJsonImpl(JsonReader reader, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                throw new InvalidNodaDataException(
                    $"Unexpected token parsing instant. Expected String, got {reader.TokenType}.");
            }

            var timeZoneId = reader.Value.ToString();
            return provider[timeZoneId];
        }

        /// <summary>
        /// Writes the time zone ID to the writer.
        /// </summary>
        /// <param name="writer">The writer to write JSON data to</param>
        /// <param name="value">The value to serializer</param>
        /// <param name="serializer">The serializer to use for nested serialization</param>
        protected override void WriteJsonImpl(JsonWriter writer, DateTimeZone value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Id);
        }
    }
}
