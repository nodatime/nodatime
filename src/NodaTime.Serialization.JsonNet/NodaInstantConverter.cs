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
using System.Globalization;
using Newtonsoft.Json;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Converts an <see cref="Instant"/> to and from the ISO 8601 date format (e.g. 2008-04-12T12:53Z).
    /// </summary>
    public class NodaInstantConverter : JsonConverter
    {
        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFZ";

        public NodaInstantConverter()
        {
            // default values
            DateTimeFormat = DefaultDateTimeFormat;
            Culture = CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Gets or sets the date time format used when converting to and from JSON.
        /// </summary>
        /// <value>The date time format used when converting to and from JSON.</value>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// Gets or sets the culture used when converting to and from JSON.
        /// </summary>
        /// <value>The culture used when converting to and from JSON.</value>
        public CultureInfo Culture { get; set; }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (Instant) || objectType == typeof (Instant?);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is Instant))
                throw new Exception(string.Format("Unexpected value when converting. Expected NodaTime.Instant, got {0}.", value.GetType().FullName));
            
            var instant = (Instant) value;
            var text = instant.ToString(DateTimeFormat, Culture);
            writer.WriteValue(text);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (objectType != typeof(Instant?))
                    throw new Exception(string.Format("Cannot convert null value to {0}.", objectType));

                return null;
            }

            if (reader.TokenType != JsonToken.String)
                throw new Exception(string.Format("Unexpected token parsing instant. Expected String, got {0}.", reader.TokenType));

            var instantText = reader.Value.ToString();

            if (string.IsNullOrEmpty(instantText) && objectType == typeof(Instant?))
                return null;

            return string.IsNullOrEmpty(DateTimeFormat)
                       ? Instant.Parse(instantText, Culture)
                       : Instant.ParseExact(instantText, DateTimeFormat, Culture);
        }
    }
}
