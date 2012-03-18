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

using System.IO;
using Newtonsoft.Json;

namespace NodaTime.Serialization.JsonNet
{
    public class NodaIntervalConverter : NodaConverterBase<Interval>
    {
        protected override Interval ReadJsonImpl(JsonReader reader, JsonSerializer serializer)
        {
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
            {
                throw new InvalidDataException("An Interval must contain Start and End properties.");
            }

            return new Interval(startInstant, endInstant);
        }

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
