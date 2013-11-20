// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Newtonsoft.Json;
using NodaTime.Utility;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Json.NET converter for <see cref="Interval"/>.
    /// </summary>   
    internal sealed class NodaIntervalConverter : NodaConverterBase<Interval>
    {
        /// <summary>
        /// Reads Start and End properties for the start and end of an interval, converting them to instants
        /// using the given serializer.
        /// </summary>
        /// <param name="reader">The JSON reader to fetch data from.</param>
        /// <param name="serializer">The serializer for embedded serialization.</param>
        /// <returns>The <see cref="DateTimeZone"/> identified in the JSON, or null.</returns>
        protected override Interval ReadJsonImpl(JsonReader reader, JsonSerializer serializer)
        {
            var startInstant = default(Instant);
            var endInstant = default(Instant);
            var gotStartInstant = false;
            var gotEndInstant = false;
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    break;
                }

                var propertyName = (string)reader.Value;
                // If we haven't got a property value, that's pretty weird. Break out of the loop,
                // and let JSON.NET fail appropriately...
                if (!reader.Read())
                {
                    break;
                }

                if (propertyName == "Start")
                {
                    startInstant = serializer.Deserialize<Instant>(reader);
                    gotStartInstant = true;
                }

                if (propertyName == "End")
                {
                    endInstant = serializer.Deserialize<Instant>(reader);
                    gotEndInstant = true;
                }
            }

            if (!(gotStartInstant && gotEndInstant))
            {
                throw new InvalidNodaDataException("An Interval must contain Start and End properties.");
            }

            return new Interval(startInstant, endInstant);
        }

        /// <summary>
        /// Serializes the interval as start/end instants.
        /// </summary>
        /// <param name="writer">The writer to write JSON to</param>
        /// <param name="value">The interval to serialize</param>
        /// <param name="serializer">The serializer for embedded serialization.</param>
        protected override void WriteJsonImpl(JsonWriter writer, Interval value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Start");
            serializer.Serialize(writer, value.Start);
            writer.WritePropertyName("End");
            serializer.Serialize(writer, value.End);
            writer.WriteEndObject();
        }
    }
}
