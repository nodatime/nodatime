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

using NodaTime.Periods;
using NodaTime.Calendars;

namespace NodaTime
{
    /// <summary>
    /// Original name: DateTimeUtils.
    /// Contains the methods to check for null values of some NodaTime classes
    /// and to return valid not-null values
    /// </summary>
    internal static class NodaDefaults
    {
        /// <summary>
        /// Gets the period type handling null.
        /// </summary>
        /// <param name="periodType">The period type to use, null means the standard period type</param>
        /// <returns>The type to use, never null</returns>
        /// <remarks>
        /// If the period type is <code>null</code>, <see cref="PeriodType.Standart"/>
        /// will be returned. Otherwise, the type specified is returned.
        /// </remarks>
        internal static PeriodType CheckPeriodType(PeriodType periodType)
        {
            return periodType ?? PeriodType.Standart;
        }

        /// <summary>
        /// Gets the calendar system handling null.
        /// </summary>
        /// <param name="calendar">the chronology to use, null means ISO in the default zone</param>
        /// <returns>The calendar system, never null</returns>
        /// <remarks>
        /// If the calendar is <code>null</code>, <see cref="IsoCalendarSystem.Instance"/>
        /// will be returned. Otherwise, the calendar specified is returned.
        /// </remarks>
        internal static ICalendarSystem CheckCalendarSystem(ICalendarSystem calendar)
        {
            return calendar ?? IsoCalendarSystem.Instance;
        }
        
    }
}
