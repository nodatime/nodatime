#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.IO;
using Newtonsoft.Json;
using NodaTime.TimeZones;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Json.NET converter for <see cref="DateTimeZone"/>.
    /// </summary>
    public sealed class NodaDateTimeZoneConverter : NodaConverterBase<DateTimeZone>
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
                throw new InvalidDataException(
                    string.Format("Unexpected token parsing instant. Expected String, got {0}.",
                    reader.TokenType));
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
