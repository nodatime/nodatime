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

namespace NodaTime
{
    /// <summary>
    /// LocalDate is an immutable struct representing a date within the calendar,
    /// with no reference to a particular time zone or time of day.
    /// </summary>
    public struct LocalDate
    {
        private readonly LocalDateTime localTime;

        /// <summary>
        /// Constructs an instance for the given year, month and day in the ISO calendar.
        /// </summary>
        public LocalDate(int year, int month, int day) : this(year, month, day, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Constructs an instance for the given year, month and day in the specified calendar.
        /// </summary>
        public LocalDate(int year, int month, int day, CalendarSystem calendar)
            : this(new LocalDateTime(year, month, day, 0, 0, calendar))
        {
        }

        // Visible for extension methods.
        internal LocalDate(LocalDateTime localTime)
        {
            this.localTime = localTime;
        }

        /// <summary>
        /// Gets the calendar system associated with this local date.
        /// </summary>
        public CalendarSystem Calendar { get { return localTime.Calendar; } }

        /// <summary>
        /// Gets the year of this local date.
        /// </summary>
        public int Year { get { return localTime.Year; } }

        /// <summary>
        /// Gets the month of this local date within the year.
        /// </summary>
        public int MonthOfYear { get { return localTime.MonthOfYear; } }

        /// <summary>
        /// Gets the day of this local date within the month.
        /// </summary>
        public int DayOfMonth { get { return localTime.DayOfMonth; } }

        /// <summary>
        /// Week day of this local date expressed as an <see cref="NodaTime.IsoDayOfWeek"/> value,
        /// for calendars which use ISO days of the week.
        /// </summary>
        /// <exception cref="InvalidOperationException">The underlying calendar doesn't use ISO days of the week.</exception>
        public IsoDayOfWeek IsoDayOfWeek { get { return localTime.IsoDayOfWeek; } }

        /// <summary>
        /// Week day of this local date as a number.
        /// </summary>
        /// <remarks>
        /// For calendars using ISO week days, this gives 1 for Monday to 7 for Sunday.
        /// </remarks>
        /// <seealso cref="IsoDayOfWeek"/>
        public int DayOfWeek { get { return localTime.DayOfWeek; } }

        /// <summary>
        /// Gets the "week year" of this local date.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The WeekYear is the year that matches with the WeekOfWeekYear field.
        /// In the standard ISO8601 week algorithm, the first week of the year
        /// is that in which at least 4 days are in the year. As a result of this
        /// definition, day 1 of the first week may be in the previous year.
        /// The WeekYear allows you to query the effective year for that day
        /// </para>
        /// <para>
        /// For example, January 1st 2011 was a Saturday, so only two days of that week
        /// (Saturday and Sunday) were in 2011. Therefore January 1st is part of
        /// week 52 of WeekYear 2010. Conversely, December 31st 2012 is a Monday,
        /// so is part of week 1 of WeekYear 2013.
        /// </para>
        /// </remarks>
        public int WeekYear { get { return localTime.WeekYear; } }

        /// <summary>
        /// Gets the week within the WeekYear. See <see cref="WeekYear"/> for more details.
        /// </summary>
        public int WeekOfWeekYear { get { return localTime.WeekOfWeekYear; } }

        /// <summary>
        /// Gets the year of this local date within the century.
        /// </summary>
        public int YearOfCentury { get { return localTime.YearOfCentury; } }

        /// <summary>
        /// Gets the year of this local date within the era.
        /// </summary>
        public int YearOfEra { get { return localTime.YearOfEra; } }

        /// <summary>
        /// Gets the day of this local date within the year.
        /// </summary>
        public int DayOfYear { get { return localTime.DayOfYear; } }

        /// <summary>
        /// Gets a <see cref="LocalDateTime" /> at midnight on the date represented by this local date, in the same calendar system.
        /// </summary>
        public LocalDateTime LocalDateTime { get { return localTime; } }

        /// <summary>
        /// Adds the specified period to the date.
        /// </summary>
        // TODO: Assert no units smaller than a day, and more documentation.
        public static LocalDate operator +(LocalDate date, Period period)
        {
            if (period == null)
            {
                throw new ArgumentNullException("period");
            }
            return new LocalDate(date.LocalDateTime + period);
        }

        /// <summary>
        /// Combines the given <see cref="LocalDate"/> and <code>LocalTime</code> components
        /// into a single <see cref="LocalDateTime"/>.
        /// </summary>
        public static LocalDateTime operator +(LocalDate date, LocalTime time)
        {
            LocalInstant localDateInstant = date.localTime.LocalInstant;
            LocalInstant localInstant = new LocalInstant(localDateInstant.Ticks + time.TickOfDay);
            return new LocalDateTime(localInstant, date.localTime.Calendar);
        }

        /// <summary>
        /// Subtracts the specified period to the date.
        /// </summary>
        // TODO: Assert no units smaller than a day, and more documentation.
        public static LocalDate operator -(LocalDate date, Period period)
        {
            if (period == null)
            {
                throw new ArgumentNullException("period");
            }
            return new LocalDate(date.LocalDateTime - period);
        }

        /// <summary>
        /// Compares two <code>LocalDate</code> values for equality. This requires
        /// that the dates be the same, within the same calendar.
        /// </summary>
        public static bool operator ==(LocalDate lhs, LocalDate rhs)
        {
            return lhs.localTime == rhs.localTime;
        }

        /// <summary>
        /// Compares two <code>LocalDate</code> values for inequality.
        /// </summary>
        public static bool operator !=(LocalDate lhs, LocalDate rhs)
        {
            return lhs.localTime != rhs.localTime;
        }

        // TODO: Implement IEquatable etc

        /// <summary>
        /// Formats this local date according to the current format provider.
        /// </summary>
        public override string ToString()
        {
            // TODO: Implement as part of general formatting work
            return string.Format("{0:00}-{1:00}-{2:00}", Year, MonthOfYear, DayOfMonth);
        }

        /// <summary>
        /// Returns a hash code for this local date.
        /// </summary>
        public override int GetHashCode()
        {
            return localTime.GetHashCode();
        }

        /// <summary>
        /// Compares two <code>LocalDate</code> values for equality. This requires
        /// that the dates be the same, within the same calendar.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is LocalDate))
            {
                return false;
            }
            return this == (LocalDate)obj;
        }
    }
}