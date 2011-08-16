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

namespace NodaTime.Experimental.WithXyz
{
    /// <summary>
    /// Extensions methods for LocalDate, providing WithXYZ methods.
    /// </summary>
    public static class LocalDateExtensions
    {
        /// <summary>
        /// Returns a new local date with the same month and day of month as this one, but in the specified year.
        /// </summary>
        /// <remarks>
        /// If the month/day combination are invalid for the specified year, they are rounded accordingly.
        /// For example, calling <c>WithYear(2011)</c> on a local date representing February 29th 2012
        /// would return a date representing February 28th 2011.
        /// </remarks>
        public static LocalDate WithYear(this LocalDate @this, int year)
        {
            return new LocalDate(@this.LocalDateTime.WithYear(year));
        }

        /// <summary>
        /// Returns a new local date with the same year and day of month as this one, but in the specified month.
        /// </summary>
        /// <remarks>
        /// If the year/day combination are invalid for the specified month, they are rounded accordingly.
        /// For example, calling <c>WithMonthOfYear(2)</c> on a local date representing January 30th 2011
        /// would return a date representing February 28th 2011.
        /// </remarks>
        public static LocalDate WithMonthOfYear(this LocalDate @this, int month)
        {
            return new LocalDate(@this.LocalDateTime.WithMonthOfYear(month));
        }

        /// <summary>
        /// Returns a new local date with the same year and month as this one, but on the specified day.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The specified day is invalid for the current date's year and month.</exception>
        public static LocalDate WithDayOfMonth(this LocalDate @this, int day)
        {
            return new LocalDate(@this.LocalDateTime.WithDayOfMonth(day));
        }


    }
}
