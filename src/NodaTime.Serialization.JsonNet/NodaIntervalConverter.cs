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
    public class NodaIntervalConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is Interval))
                throw new Exception(string.Format("Unexpected value when converting. Expected NodaTime.Interval, got {0}.", value.GetType().FullName));

            var instant = (Interval)value;
            writer.WriteStartObject();
            writer.WritePropertyName("Start");
            serializer.Serialize(writer, instant.Start);
            writer.WritePropertyName("End");
            serializer.Serialize(writer, instant.End);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (objectType != typeof(Interval?))
                    throw new Exception(string.Format("Cannot convert null value to {0}.", objectType));

                return null;
            }

            var startInstant = default(Instant);
            var endInstant = default(Instant);
            var gotStartInstant = false;
            var gotEndInstant = false;
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    continue;

                var propertyName = (string)reader.Value;
                if (!reader.Read())
                    continue;

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
                throw new Exception("An Interval must contain Start and End properties.");

            return new Interval(startInstant, endInstant);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Interval) || objectType == typeof(Interval?);
        }
    }
}
