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

using Newtonsoft.Json;

namespace NodaTime.Serialization.JsonNet
{
    public static class Extensions
    {
        /// <summary>
        /// Configures json.net with everything required to properly serialize and deserialize NodaTime data types.
        /// </summary>
        public static JsonSerializerSettings ConfigureForNodaTime(this JsonSerializerSettings settings)
        {
            // add our converters
            settings.Converters.Add(new NodaInstantConverter());
            settings.Converters.Add(new NodaIntervalConverter());
            settings.Converters.Add(new NodaLocalDateConverter());
            settings.Converters.Add(new NodaLocalDateTimeConverter());
            settings.Converters.Add(new NodaLocalTimeConverter());

            // return to allow fluent chaining if desired
            return settings;
        }

        /// <summary>
        /// Configures json.net with everything required to properly serialize and deserialize NodaTime data types.
        /// </summary>
        public static JsonSerializer ConfigureForNodaTime(this JsonSerializer serializer)
        {
            // add our converters
            serializer.Converters.Add(new NodaInstantConverter());
            serializer.Converters.Add(new NodaIntervalConverter());
            serializer.Converters.Add(new NodaLocalDateConverter());
            serializer.Converters.Add(new NodaLocalDateTimeConverter());
            serializer.Converters.Add(new NodaLocalTimeConverter());

            // return to allow fluent chaining if desired
            return serializer;
        }
    }
}
