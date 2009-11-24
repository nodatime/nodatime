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

        void Validate(IPartial partial, int[] values);

        int[] GetPartialValues(IPartial partial, LocalInstant instant);

        /// <summary>
        /// Sets the values from the partial within an existing local instant.
        /// </summary>
        LocalInstant SetPartial(IPartial partial, LocalInstant localInstant);

        int[] GetPeriodValues(IPeriod period, LocalInstant start, LocalInstant end);

        int[] GetPeriodValues(IPeriod period, Duration duration);

        LocalInstant Add(IPeriod period, LocalInstant localInstant, int scalar);

        LocalInstant Add(IPeriod period, Duration duration, int scalar);
    }
}