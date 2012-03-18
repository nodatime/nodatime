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
using System.IO;
using Newtonsoft.Json;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Converts an <see cref="Instant"/> to and from the ISO 8601 date format (e.g. 2008-04-12T12:53Z).
    /// </summary>
    public class NodaInstantConverter : NodaConverterBase<Instant>
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

        protected override Instant ReadJsonImpl(JsonReader reader, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                throw new InvalidDataException(
                    string.Format("Unexpected token parsing instant. Expected String, got {0}.",
                    reader.TokenType));
            }
            string text = reader.Value.ToString();
            return string.IsNullOrEmpty(DateTimeFormat)
                       ? Instant.Parse(text, Culture)
                       : Instant.ParseExact(text, DateTimeFormat, Culture);
        }

        protected override void WriteJsonImpl(JsonWriter writer, Instant value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(DateTimeFormat, Culture));
        }
    }
}
