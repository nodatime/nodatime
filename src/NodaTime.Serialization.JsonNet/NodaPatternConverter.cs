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
using NodaTime.Text;

namespace NodaTime.Serialization.JsonNet
{
    public class NodaPatternConverter<T> : NodaConverterBase<T> where T : struct
    {
        private readonly IPattern<T> pattern;

        public NodaPatternConverter(IPattern<T> pattern)
        {
            // Note: We could use Preconditions.CheckNotNull, but only if we either made that public in NodaTime
            // or made InternalsVisibleTo this assembly. 
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            this.pattern = pattern;
        }

        protected override T ReadJsonImpl(JsonReader reader, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                throw new InvalidDataException(
                    string.Format("Unexpected token parsing instant. Expected String, got {0}.",
                    reader.TokenType));
            }
            string text = reader.Value.ToString();
            return pattern.Parse(text).Value;
        }

        protected override void WriteJsonImpl(JsonWriter writer, T value, JsonSerializer serializer)
        {
            writer.WriteValue(pattern.Format(value));
        }
    }
}
