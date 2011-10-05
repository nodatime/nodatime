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
using NodaTime.Calendars;
using NodaTime.Utility;
using NodaTime.Fields;

namespace NodaTime
{
    /// <summary>
    /// A date and time in a particular calendar system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A LocalDateTime value does not represent an instant on the time line, mostly because it has
    /// no associated time zone: "November 12th 2009 7pm, ISO calendar" occurred at different
    /// instants for different people around the world.
    /// </para>
    /// <para>
    /// This type defaults to using the IsoCalendarSystem unless a different calendar system is
    /// specified.
    /// </para>
    /// <para>
    /// This type is immutable and thread-safe.
    /// </para>
    /// </remarks>
    public struct LocalDateTime : IEquatable<LocalDateTime>
    {
        private readonly CalendarSystem calendar;
        private readonly LocalInstant localInstant;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO
        /// calendar system.
        /// </summary>
        /// <param name="localInstant">The local instant.</param>
        internal LocalDateTime(LocalInstant localInstant) : this(localInstant, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="localInstant">The local instant.</param>
        /// <param name="calendar">The calendar system.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is <c>null</c>.</exception>
        internal LocalDateTime(LocalInstant localInstant, CalendarSystem calendar)
        {
            if (calendar == null)
            {
                throw new ArgumentNullException("calendar");
            }
            this.localInstant = localInstant;
            this.calendar = calendar;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO calendar system.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        public LocalDateTime(int year, int month, int day, int hour, int minute) : this(year, month, day, hour, minute, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="calendar">The calendar.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is <c>null</c>.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, CalendarSystem calendar)
        {
            if (calendar == null)
            {
                throw new ArgumentNullException("calendar");
            }
            localInstant = calendar.GetLocalInstant(year, month, day, hour, minute);
            this.calendar = calendar;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO calendar system.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second) : this(year, month, day, hour, minute, second, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="calendar">The calendar.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is <c>null</c>.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, CalendarSystem calendar)
        {
            if (calendar == null)
            {
                throw new ArgumentNullException("calendar");
            }
            localInstant = calendar.GetLocalInstant(year, month, day, hour, minute, second);
            this.calendar = calendar;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO calendar system.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
            : this(year, month, day, hour, minute, second, millisecond, 0, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="calendar">The calendar.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is <c>null</c>.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, CalendarSystem calendar)
            : this(year, month, day, hour, minute, second, millisecond, 0, calendar)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="tickWithinMillisecond">The tick within millisecond.</param>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int tickWithinMillisecond)
            : this(year, month, day, hour, minute, second, millisecond, tickWithinMillisecond, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="tickWithinMillisecond">The tick within millisecond.</param>
        /// <param name="calendar">The calendar.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is <c>null</c>.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int tickWithinMillisecond, CalendarSystem calendar)
        {
            if (calendar == null)
            {
                throw new ArgumentNullException("calendar");
            }
            localInstant = calendar.GetLocalInstant(year, month, day, hour, minute, second, millisecond, tickWithinMillisecond);
            this.calendar = calendar;
        }

        internal LocalInstant LocalInstant { get { return localInstant; } }

        /// <summary>Gets the calendar system associated with this local date and time.</summary>
        public CalendarSystem Calendar { get { return calendar; } }

        /// <summary>Gets the century within the era of this local date and time.</summary>
        public int CenturyOfEra { get { return calendar.Fields.CenturyOfEra.GetValue(localInstant); } }

        /// <summary>Gets the year of this local date and time.</summary>
        public int Year { get { return calendar.Fields.Year.GetValue(localInstant); } }

        /// <summary>Gets the year of this local date and time within its century.</summary>
        public int YearOfCentury { get { return calendar.Fields.YearOfCentury.GetValue(localInstant); } }

        /// <summary>Gets the year of this local date and time within its era.</summary>
        public int YearOfEra { get { return calendar.Fields.YearOfEra.GetValue(localInstant); } }

        /// <summary>Gets the era of this local date.</summary>
        public Era Era { get { return calendar.Eras[calendar.Fields.Era.GetValue(localInstant)]; } }

        /// <summary>
        /// Gets the "week year" of this local date and time.
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
        public int WeekYear { get { return calendar.Fields.WeekYear.GetValue(localInstant); } }

        /// <summary>
        /// Gets the month of this local date and time within the year.
        /// </summary>
        public int MonthOfYear { get { return calendar.Fields.MonthOfYear.GetValue(localInstant); } }

        /// <summary>
        /// Gets the week within the WeekYear. See <see cref="WeekYear"/> for more details.
        /// </summary>
        public int WeekOfWeekYear { get { return calendar.Fields.WeekOfWeekYear.GetValue(localInstant); } }

        /// <summary>
        /// Gets the day of this local date and time within the year.
        /// </summary>
        public int DayOfYear { get { return calendar.Fields.DayOfYear.GetValue(localInstant); } }

        /// <summary>
        /// Gets the day of this local date and time within the month.
        /// </summary>
        public int DayOfMonth { get { return calendar.Fields.DayOfMonth.GetValue(localInstant); } }

        /// <summary>
        /// Gets the week day of this local date and time expressed as an <see cref="NodaTime.IsoDayOfWeek"/> value,
        /// for calendars which use ISO days of the week.
        /// </summary>
        /// <exception cref="InvalidOperationException">The underlying calendar doesn't use ISO days of the week.</exception>
        public IsoDayOfWeek IsoDayOfWeek { get { return calendar.GetIsoDayOfWeek(localInstant); } }

        /// <summary>
        /// Gets the week day of this local date and time as a number.
        /// </summary>
        /// <remarks>
        /// For calendars using ISO week days, this gives 1 for Monday to 7 for Sunday.
        /// </remarks>
        /// <seealso cref="IsoDayOfWeek"/>
        public int DayOfWeek { get { return calendar.Fields.DayOfWeek.GetValue(localInstant); } }

        /// <summary>
        /// Gets the hour of day of this local date and time, in the range 0 to 23 inclusive.
        /// </summary>
        public int HourOfDay { get { return calendar.Fields.HourOfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the hour of the half-day of this local date and time, in the range 1 to 12 inclusive.
        /// </summary>
        public int ClockHourOfHalfDay { get { return calendar.Fields.ClockHourOfHalfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the minute of this local date and time, in the range 0 to 59 inclusive.
        /// </summary>
        public int MinuteOfHour { get { return calendar.Fields.MinuteOfHour.GetValue(localInstant); } }

        /// <summary>
        /// Gets the second of this local date and time within the minute, in the range 0 to 59 inclusive.
        /// </summary>
        public int SecondOfMinute { get { return calendar.Fields.SecondOfMinute.GetValue(localInstant); } }

        /// <summary>
        /// Gets the second of this local date and time within the day, in the range 0 to 86,399 inclusive.
        /// </summary>
        public int SecondOfDay { get { return calendar.Fields.SecondOfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the millisecond of this local date and time within the second, in the range 0 to 999 inclusive.
        /// </summary>
        public int MillisecondOfSecond { get { return calendar.Fields.MillisecondOfSecond.GetValue(localInstant); } }

        /// <summary>
        /// Gets the millisecond of this local date and time within the day, in the range 0 to 86,399,999 inclusive.
        /// </summary>
        public int MillisecondOfDay { get { return calendar.Fields.MillisecondOfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the tick of this local date and time within the millisecond, in the range 0 to 9,999 inclusive.
        /// </summary>
        public int TickOfMillisecond { get { return calendar.Fields.TickOfMillisecond.GetValue(localInstant); } }

        /// <summary>
        /// Gets the tick of this local time within the second, in the range 0 to 9,999,999 inclusive.
        /// </summary>
        public int TickOfSecond { get { return calendar.Fields.TickOfSecond.GetValue(localInstant); } }

        /// <summary>
        /// Gets the tick of this local date and time within the day, in the range 0 to 863,999,999,999 inclusive.
        /// </summary>
        public long TickOfDay { get { return calendar.Fields.TickOfDay.GetInt64Value(localInstant); } }

        /// <summary>
        /// Gets the time portion of this local date and time as a <see cref="LocalTime"/>.
        /// </summary>
        public LocalTime TimeOfDay
        {
            get
            {
                long ticks = localInstant.Ticks % NodaConstants.TicksPerStandardDay;
                if (ticks < 0)
                {
                    ticks += NodaConstants.TicksPerStandardDay;
                }
                return new LocalTime(new LocalInstant(ticks));
            }
        }

        /// <summary>
        /// Gets the date portion of this local date and time as a <see cref="LocalDate"/> in the same calendar system as this value.
        /// </summary>
        public LocalDate Date { get { return new LocalDate(Year, MonthOfYear, DayOfMonth, calendar); } }

        // TODO: Add an overload for the calendar to use?
        /// <summary>
        /// Constructs a <see cref="DateTime"/> from this value which has a <see cref="DateTime.Kind" />
        /// of <see cref="DateTimeKind.Unspecified"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="DateTimeKind.Unspecified"/> is slightly odd - it can be treated as UTC if you use <see cref="DateTime.ToLocalTime"/>
        /// or as system local time if you use <see cref="DateTime.ToUniversalTime"/>, but it's the only kind which allows
        /// you to construct a <see cref="DateTimeOffset"/> with an arbitrary offset, which makes it as close to
        /// the Noda Time non-system-specific "local" concept as exists in .NET.
        /// </remarks>
        /// <returns>A <see cref="DateTime"/> value for the same date and time as this value.</returns>
        public DateTime ToDateTimeUnspecified()
        {
            return localInstant.ToDateTimeUnspecified();
        }

        /// <summary>
        /// Converts a <see cref="DateTime" /> of any kind to a LocalDateTime in the ISO calendar. This does not perform
        /// any time zone conversions, so a DateTime with a <see cref="DateTime.Kind"/> of <see cref="DateTimeKind.Utc"/>
        /// will still have the same day/hour/minute etc - it won't be converted into the local system time.
        /// </summary>
        /// <param name="dateTime">Value to convert into a Noda Time local date and time</param>
        /// <returns>A new <see cref="LocalDateTime"/> with the same values as the specified one.</returns>
        internal static LocalDateTime FromDateTime(DateTime dateTime)
        {
            return new LocalDateTime(LocalInstant.FromDateTime(dateTime), CalendarSystem.Iso);
        }

        #region Implementation of IEquatable<LocalDateTime>
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(LocalDateTime other)
        {
            return localInstant == other.localInstant && calendar.Equals(other.calendar);
        }
        #endregion

        #region Operators
        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(LocalDateTime left, LocalDateTime right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(LocalDateTime left, LocalDateTime right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Adds a period to a local date/time. Fields are added in the order provided by the period.
        /// </summary>
        /// <param name="localDateTime">Initial local date and time</param>
        /// <param name="period">Period to add</param>
        /// <returns>The resulting local date and time</returns>
        public static LocalDateTime operator +(LocalDateTime localDateTime, Period period)
        {
            CalendarSystem calendar = localDateTime.Calendar;
            return new LocalDateTime(calendar.Add(period, localDateTime.LocalInstant, 1), calendar);
        }

        /// <summary>
        /// Subtracts a period from a local date/time. Fields are subtracted in the order provided by the period.
        /// </summary>
        /// <param name="localDateTime">Initial local date and time</param>
        /// <param name="period">Period to subtract</param>
        /// <returns>The resulting local date and time</returns>
        public static LocalDateTime operator -(LocalDateTime localDateTime, Period period)
        {
            CalendarSystem calendar = localDateTime.Calendar;
            return new LocalDateTime(calendar.Add(period, localDateTime.LocalInstant, -1), calendar);
        }
        #endregion

        #region object overrides
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is LocalDateTime)
            {
                return Equals((LocalDateTime)obj);
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, LocalInstant);
            hash = HashCodeHelper.Hash(hash, Calendar);
            return hash;
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Calendar + ": " + LocalInstant;
        }
        #endregion

        /// <summary>
        /// Creates a new LocalDateTime representing the same physical date and time, but in a different calendar.
        /// The returned LocalDateTime is likely to have different date field values to this one.
        /// For example, January 1st 1970 in the Gregorian calendar was December 19th 1969 in the Julian calendar.
        /// </summary>
        /// <param name="calendarSystem">The calendar system to convert this local date to. Must not be null.</param>
        /// <returns>The converted LocalDateTime.</returns>
        public LocalDateTime WithCalendar(CalendarSystem calendarSystem)
        {
            if (calendarSystem == null)
            {
                throw new ArgumentNullException("calendarSystem");
            }
            return new LocalDateTime(localInstant, calendarSystem);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of years added.
        /// </summary>
        /// <remarks>
        /// If the resulting date is invalid, lower fields (typically the day of month) are reduced to find a valid value.
        /// For example, adding one year to February 29th 2012 will return February 28th 2013; subtracting one year from
        /// February 29th 2012 will return February 28th 2011.
        /// </remarks>
        /// <param name="years">The number of years to add</param>
        /// <returns>The current value plus the given number of years.</returns>
        public LocalDateTime PlusYears(int years)
        {
            LocalInstant newLocalInstant = calendar.Fields.Years.Add(localInstant, years);
            return new LocalDateTime(newLocalInstant, calendar);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of months added.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method does not try to maintain the year of the current value, so adding four months to a value in 
        /// October will result in a value in the following February.
        /// </para>
        /// <para>
        /// If the resulting date is invalid, the day of month is reduced to find a valid value.
        /// For example, adding one month to January 30th 2011 will return February 28th 2011; subtracting one month from
        /// March 30th 2011 will return February 28th 2011.
        /// </para>
        /// </remarks>
        /// <param name="months">The number of months to add</param>
        /// <returns>The current value plus the given number of months.</returns>
        public LocalDateTime PlusMonths(int months)
        {
            LocalInstant newLocalInstant = calendar.Fields.Months.Add(localInstant, months);
            return new LocalDateTime(newLocalInstant, calendar);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of days added.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method does not try to maintain the month or year of the current value, so adding 3 days to a value on January 30th
        /// will result in a value on February 2nd.
        /// </para>
        /// </remarks>
        /// <param name="days">The number of days to add</param>
        /// <returns>The current value plus the given number of days.</returns>
        public LocalDateTime PlusDays(int days)
        {
            LocalInstant newLocalInstant = calendar.Fields.Days.Add(localInstant, days);
            return new LocalDateTime(newLocalInstant, calendar);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of weeks added.
        /// </summary>
        /// <param name="weeks">The number of weeks to add</param>
        /// <returns>The current value plus the given number of weeks.</returns>
        public LocalDateTime PlusWeeks(int weeks)
        {
            LocalInstant newLocalInstant = calendar.Fields.Weeks.Add(localInstant, weeks);
            return new LocalDateTime(newLocalInstant, calendar);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of hours added.
        /// </summary>
        /// <param name="hours">The number of hours to add</param>
        /// <returns>The current value plus the given number of hours.</returns>
        public LocalDateTime PlusHours(long hours)
        {
            LocalInstant newLocalInstant = calendar.Fields.Hours.Add(localInstant, hours);
            return new LocalDateTime(newLocalInstant, calendar);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of minutes added.
        /// </summary>
        /// <param name="minutes">The number of minutes to add</param>
        /// <returns>The current value plus the given number of minutes.</returns>
        public LocalDateTime PlusMinutes(long minutes)
        {
            LocalInstant newLocalInstant = calendar.Fields.Minutes.Add(localInstant, minutes);
            return new LocalDateTime(newLocalInstant, calendar);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of seconds added.
        /// </summary>
        /// <param name="seconds">The number of seconds to add</param>
        /// <returns>The current value plus the given number of seconds.</returns>
        public LocalDateTime PlusSeconds(long seconds)
        {
            LocalInstant newLocalInstant = calendar.Fields.Seconds.Add(localInstant, seconds);
            return new LocalDateTime(newLocalInstant, calendar);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of seconds added.
        /// </summary>
        /// <param name="milliseconds">The number of seconds to add</param>
        /// <returns>The current value plus the given number of seconds.</returns>
        public LocalDateTime PlusMilliseconds(long milliseconds)
        {
            LocalInstant newLocalInstant = calendar.Fields.Milliseconds.Add(localInstant, milliseconds);
            return new LocalDateTime(newLocalInstant, calendar);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of ticks added.
        /// </summary>
        /// <param name="ticks">The number of ticks to add</param>
        /// <returns>The current value plus the given number of seconds.</returns>
        public LocalDateTime PlusTicks(long ticks)
        {
            LocalInstant newLocalInstant = calendar.Fields.Ticks.Add(localInstant, ticks);
            return new LocalDateTime(newLocalInstant, calendar);
        }
    }
}
