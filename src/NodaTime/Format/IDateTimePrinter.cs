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
using NodaTime.Calendars;

namespace NodaTime.Format
{
    /// <summary>
    /// Internal interface for creating textual representations of datetimes.
    /// </summary>
    internal interface IDateTimePrinter
    {
        /// <summary>
        /// Returns the expected maximum number of characters produced.
        /// The actual amount should rarely exceed this estimate.
        /// </summary>
        int EstimatedPrintedLength { get; }

        /// <summary>
        /// Prints a local instant, using the given calendar system
        /// </summary>
        /// <param name="writer">Formatted instant is written to this writer, not null</param>
        /// <param name="instant">Local instant to print</param>
        /// <param name="calendarSystem">The calendar system to use, not null</param>
        /// <param name="timezoneOffset"></param>
        /// <param name="dateTimeZone">The time zone to use, null means local time</param>
        /// <param name="provider">Provider to use</param>
        void PrintTo(TextWriter writer, LocalInstant instant, CalendarSystem calendarSystem, Offset timezoneOffset, DateTimeZone dateTimeZone,
                     IFormatProvider provider);

        /// <summary>
        /// Prints a partial.
        /// </summary>
        /// <param name="writer">Formatted partial is written to this builder, not null</param>
        /// <param name="partial">A partial instance to print</param>
        /// <param name="provider">Provider to use</param>
        void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider);
    }
}