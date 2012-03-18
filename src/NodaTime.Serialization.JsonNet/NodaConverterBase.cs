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
namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Base class for all the Json.NET converters which handle value types (which is most of them).
    /// This deals handles all the boilerplate code dealing with nullity.
    /// </summary>
    public abstract class NodaConverterBase<T> : JsonConverter where T : struct
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T) || objectType == typeof(T?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (objectType != typeof(T?))
                {
                    throw new InvalidDataException(string.Format("Cannot convert null value to {0}.", objectType));
                }
                return null;
            }

            // Handle empty strings automatically
            if (reader.TokenType == JsonToken.String)
            {
                string value = (string) reader.Value;
                if (value == "")
                {
                    if (objectType != typeof(T?))
                    {
                        throw new InvalidDataException(string.Format("Cannot convert null value to {0}.", objectType));
                    }
                    return null;
                }
            }

            // Delegate to the concrete subclass. At this point we know that we don't want to return null, so we
            // can ask the subclass to return a T, which we will box. That will be valid even if objectType is
            // T? because the boxed form of a non-null T? value is just the boxed value itself.

            // Note that we don't currently pass existingValue down; we could change this if we ever found a use for it.
            return ReadJsonImpl(reader, serializer);
        }

        /// <summary>
        /// Implemented by concrete subclasses, this performs the final conversion from a non-null JSON value to
        /// a value of type T.
        /// </summary>
        protected abstract T ReadJsonImpl(JsonReader reader, JsonSerializer serializer);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Json.NET should prevent this happening, but let's validate...
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            // Note: don't need to worry about value is T? due to the way boxing works.
            if (!(value is T))
            {
                throw new ArgumentException(string.Format("Unexpected value when converting. Expected NodaTime.Instant, got {0}.", value.GetType().FullName));
            }
            WriteJsonImpl(writer, (T)value, serializer);
        }

        /// <summary>
        /// Implemented by concrete subclasses, this performs the final write operation for a non-null value of type T
        /// to JSON.
        /// </summary>
        protected abstract void WriteJsonImpl(JsonWriter writer, T value, JsonSerializer serializer);
    }
}
