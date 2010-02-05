#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
using NodaTime.Fields;

namespace NodaTime.Calendars
{
    /// <summary>
    /// A system of defining time in terms of years, months, days and so forth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The most commonly use calendar system in Noda Time is <see cref="Calendars.IsoCalendarSystem" />,
    /// which is used as a default value in many overloaded methods and constructors.
    /// </para>
    /// <para>
    /// A calendar system has no specific time zone; a <see cref="Chronology" /> represents the union
    /// of a time zone with a calendar system.
    /// </para>
    /// <para>
    /// The members of this class are unlikely to be used directly by most users of the API.
    /// </para>
    /// </remarks>
    public interface ICalendarSystem
    {
        LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int tickOfDay);

        LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth,
                                     int hourOfDay, int minuteOfHour, int secondOfMinute, int millisecondOfSecond, int tickOfMillisecond);

        LocalInstant GetLocalInstant(LocalInstant localInstant,
                                     int hourOfDay, int minuteOfHour, int secondOfMinute, int millisecondOfSecond, int tickOfMillisecond);

        FieldSet Fields { get; }

        #region Periods

        /// <summary>
        /// Gets the values of a period from a duration.
        /// </summary>
        /// <param name="period">The period instant to use</param>
        /// <param name="duration">The duration to query</param>
        /// <returns>The values of the period extracted from the duration</returns>
        int[] GetPeriodValues(IPeriod period, Duration duration);

        /// <summary>
        /// Gets the values of a period from an interval.
        /// </summary>
        /// <param name="period">The period instant to use</param>
        /// <param name="start">The start instant of an interval to query</param>
        /// <param name="end">The end instant of an interval to query</param>
        /// <returns>The values of the period extracted from the interval</returns>
        int[] GetPeriodValues(IPeriod period, LocalInstant start, LocalInstant end);

        /// <summary>
        /// Adds the period to the instant, specifying the number of times to add.
        /// </summary>
        /// <param name="period">The period to add, null means add nothing</param>
        /// <param name="instant">The instant to add to</param>
        /// <param name="scalar">The number of times to add</param>
        /// <returns>The updated instant</returns>
        LocalInstant Add(IPeriod period, LocalInstant instant, int scalar);

        #endregion

        #region Partials

        /// <summary>
        /// Validates whether the values are valid for the fields of a partial instant.
        /// </summary>
        /// <param name="partial">The partial instant to validate</param>
        /// <param name="values">The values to validate, not null, match fields in partial</param>
        void Validate(IPartial partial, int[] values);

        /// <summary>
        /// Gets the values of a partial from an instant.
        /// </summary>
        /// <param name="partial">The partial instant to use</param>
        /// <param name="instant">The instant to query</param>
        /// <returns>The values of this partial extracted from the instant</returns>
        int[] GetPartialValues(IPartial partial, LocalInstant instant);

        /// <summary>
        /// Sets the values from the partial within an existing local instant.
        /// </summary>
        /// <param name="partial">The partial instant to use</param>
        /// <param name="localInstant">The instant to update</param>
        /// <returns>The updated instant</returns>
        LocalInstant SetPartial(IPartial partial, LocalInstant localInstant);
        
        #endregion
    }
}