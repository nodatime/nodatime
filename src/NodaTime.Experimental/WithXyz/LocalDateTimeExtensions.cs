#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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
using NodaTime.Fields;

namespace NodaTime.Experimental.WithXyz
{
    /// <summary>
    /// Extensions methods for LocalDate, providing WithXYZ methods.
    /// </summary>
    public static class LocalDateTimeExtensions
    {
        #region Pseudo-mutators
        /// <summary>
        /// Returns a new local date and time with the same month and day of month as this one, but in the specified year.
        /// The time of day is unaffected.
        /// </summary>
        /// <remarks>
        /// If the month/day combination are invalid for the specified year, they are rounded accordingly.
        /// For example, calling <c>WithYear(2011)</c> on a local date representing February 29th 2012
        /// would return a date representing February 28th 2011.
        /// </remarks>
        public static LocalDateTime WithYear(this LocalDateTime @this, int year)
        {
            return WithField(@this, @this.Calendar.Fields.Year, year);
        }

        /// <summary>
        /// Returns a new local date and time with the same year and day of month as this one, but in the specified month.
        /// The time of day is unaffected.
        /// </summary>
        /// <remarks>
        /// If the year/day combination are invalid for the specified month, they are rounded accordingly.
        /// For example, calling <c>WithMonthOfYear(2)</c> on a local date representing January 30th 2011
        /// would return a date representing February 28th 2011.
        /// </remarks>
        public static LocalDateTime WithMonthOfYear(this LocalDateTime @this, int month)
        {
            return WithField(@this, @this.Calendar.Fields.MonthOfYear, month);
        }

        /// <summary>
        /// Returns a new local date and time with the same year and month as this one, but on the specified day.
        /// The time of day is unaffected.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The specified day is invalid for the current date's year and month.</exception>
        public static LocalDateTime WithDayOfMonth(this LocalDateTime @this, int day)
        {
            return WithField(@this, @this.Calendar.Fields.DayOfMonth, day);
        }

        private static LocalDateTime WithField(LocalDateTime ldt, DateTimeField field, long value)
        {
            return new LocalDateTime(field.SetValue(ldt.LocalInstant, value), ldt.Calendar);
        }
        #endregion

    }
}
