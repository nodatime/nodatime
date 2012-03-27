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
    /// <summary>
    /// Static class containing extension methods to configure Json.NET for Noda Time types.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Configures json.net with everything required to properly serialize and deserialize NodaTime data types.
        /// </summary>
        public static JsonSerializerSettings ConfigureForNodaTime(this JsonSerializerSettings settings)
        {
            // add our converters
            settings.Converters.Add(NodaConverters.InstantConverter);
            settings.Converters.Add(NodaConverters.IntervalConverter);
            settings.Converters.Add(NodaConverters.LocalDateConverter);
            settings.Converters.Add(NodaConverters.LocalDateTimeConverter);
            settings.Converters.Add(NodaConverters.LocalTimeConverter);
            settings.Converters.Add(NodaConverters.OffsetConverter);
            settings.Converters.Add(NodaConverters.DateTimeZoneConverter);
            settings.Converters.Add(NodaConverters.DurationConverter);
            settings.Converters.Add(NodaConverters.RoundtripPeriodConverter);

            // return to allow fluent chaining if desired
            return settings;
        }

        /// <summary>
        /// Configures json.net with everything required to properly serialize and deserialize NodaTime data types.
        /// </summary>
        public static JsonSerializer ConfigureForNodaTime(this JsonSerializer serializer)
        {
            // add our converters
            serializer.Converters.Add(NodaConverters.InstantConverter);
            serializer.Converters.Add(NodaConverters.IntervalConverter);
            serializer.Converters.Add(NodaConverters.LocalDateConverter);
            serializer.Converters.Add(NodaConverters.LocalDateTimeConverter);
            serializer.Converters.Add(NodaConverters.LocalTimeConverter);
            serializer.Converters.Add(NodaConverters.OffsetConverter);
            serializer.Converters.Add(NodaConverters.DateTimeZoneConverter);
            serializer.Converters.Add(NodaConverters.DurationConverter);
            serializer.Converters.Add(NodaConverters.RoundtripPeriodConverter);

            // return to allow fluent chaining if desired
            return serializer;
        }
    }
}
