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
using Newtonsoft.Json;

namespace NodaTime.Serialization.JsonNet
{
    public class NodaDateTimeZoneConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is DateTimeZone))
                throw new Exception(string.Format("Unexpected value when converting. Expected NodaTime.DateTimeZone, got {0}.", value.GetType().FullName));

            var dateTimeZone = (DateTimeZone)value;
            writer.WriteValue(dateTimeZone.Id);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType != JsonToken.String)
                throw new Exception(string.Format("Unexpected token parsing instant. Expected String, got {0}.", reader.TokenType));

            var timeZoneId = reader.Value.ToString();
            if (string.IsNullOrEmpty(timeZoneId))
                return null;

            return DateTimeZone.ForId(timeZoneId);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTimeZone).IsAssignableFrom(objectType);
        }
    }
}
