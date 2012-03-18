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
using NodaTime.Text;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Convenience class to expose preconfigured converters for Noda Time types.
    /// </summary>
    public static class NodaConverters
    {
        /// <summary>
        /// Converter for instants, using the ISO-8601 date/time pattern, extended as required to accommodate milliseconds and ticks, and
        /// specifying 'Z' at the end to show it's effectively in UTC.
        /// </summary>
        public static readonly JsonConverter InstantConverter = new NodaPatternConverter<Instant>(InstantPattern.CreateWithInvariantInfo("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFF'Z'"));

        /// <summary>
        /// Converter for local dates, using the ISO-8601 date pattern.
        /// </summary>
        // TODO: Consider the behaviour with non-ISO calendars. We probably want a pattern which "knows" about a particular calendar, and restricts itself to that calendar.
        public static readonly JsonConverter LocalDateConverter = new NodaPatternConverter<LocalDate>(LocalDatePattern.CreateWithInvariantInfo("yyyy'-'MM'-'dd"));

        /// <summary>
        /// Converter for local dates and times, using the ISO-8601 date/time pattern, extended as required to accommodate milliseconds and ticks.
        /// No time zone designator is applied.
        /// </summary>
        public static readonly JsonConverter LocalDateTimeConverter = new NodaPatternConverter<LocalDateTime>(LocalDateTimePattern.CreateWithInvariantInfo("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFF"));

        /// <summary>
        /// Converter for local times, using the ISO-8601 time pattern, extended as required to accommodate milliseconds and ticks.
        /// </summary>
        public static readonly JsonConverter LocalTimeConverter = new NodaPatternConverter<LocalTime>(LocalTimePattern.CreateWithInvariantInfo("HH':'mm':'ss.FFFFFFF"));

        /// <summary>
        /// Converter for intervals. This must be used in a serializer which also has an instant converter.
        /// </summary>
        public static readonly JsonConverter IntervalConverter = new NodaIntervalConverter();

        /// <summary>
        /// Converter for offsets.
        /// </summary>
        public static readonly JsonConverter OffsetConverter = new NodaPatternConverter<Offset>(OffsetPattern.CreateWithInvariantInfo("g"));
    }
}
