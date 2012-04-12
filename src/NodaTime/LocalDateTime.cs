#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
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
using NodaTime.Globalization;
using NodaTime.Text;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// A date and time in a particular calendar system. A LocalDateTime value does not represent an
    /// instant on the time line, because it has no associated time zone: "November 12th 2009 7pm, ISO calendar"
    /// occurred at different instants for different people around the world.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type defaults to using the IsoCalendarSystem unless a different calendar system is
    /// specified.
    /// </para>
    /// <para>Comparisons of values can be handled in a way which is either calendar-sensitive or calendar-insensitive.
    /// Noda Time implements all the operators (and the <see cref="Equals(LocalDateTime)"/> method) such that all operators other than <see cref="op_Inequality"/>
    /// will return false if asked to compare two values in different calendar systems.
    /// </para>
    /// <para>
    /// However, the <see cref="CompareTo"/> method (implementing <see cref="IComparable{T}"/>) is calendar-insensitive; it compares the two
    /// values historically in terms of when they actually occurred, as if they're both converted to some "neutral" calendar system first.
    /// </para>
    /// <para>
    /// It's unclear at the time of this writing whether this is the most appropriate approach, and it may change in future versions. In general,
    /// it would be a good idea for users to avoid comparing dates in different calendar systems, and indeed most users are unlikely to ever explicitly
    /// consider which calendar system they're working in anyway.
    /// </para>
    /// </remarks>
    public struct LocalDateTime : IEquatable<LocalDateTime>, IComparable<LocalDateTime>, IFormattable
    {
        private static readonly int TypeInitializationChecking = NodaTime.Utility.TypeInitializationChecker.RecordInitializationStart();

        private readonly CalendarSystem calendar;
        private readonly LocalInstant localInstant;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO
        /// calendar system.
        /// </summary>
        /// <param name="localInstant">The local instant.</param>
        /// <returns>The resulting date/time.</returns>
        internal LocalDateTime(LocalInstant localInstant) : this(localInstant, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="localInstant">The local instant.</param>
        /// <param name="calendar">The calendar system.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is <c>null</c>.</exception>
        /// <returns>The resulting date/time.</returns>
        internal LocalDateTime(LocalInstant localInstant, CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, "calendar");
            this.localInstant = localInstant;
            this.calendar = calendar;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO calendar system.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so a value of 0 means 1BC, for example.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time</exception>
        /// <returns>The resulting date/time.</returns>
        public LocalDateTime(int year, int month, int day, int hour, int minute)
            : this(year, month, day, hour, minute, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so a value of 0 means 1BC, for example.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="calendar">The calendar.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time</exception>
        /// <returns>The resulting date/time.</returns>
        public LocalDateTime(int year, int month, int day, int hour, int minute, CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, "calendar");
            localInstant = calendar.GetLocalInstant(year, month, day, hour, minute);
            this.calendar = calendar;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO calendar system.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so a value of 0 means 1BC, for example.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time</exception>
        /// <returns>The resulting date/time.</returns>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second)
            : this(year, month, day, hour, minute, second, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so a value of 0 means 1BC, for example.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="calendar">The calendar.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time</exception>
        /// <returns>The resulting date/time.</returns>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, "calendar");
            localInstant = calendar.GetLocalInstant(year, month, day, hour, minute, second);
            this.calendar = calendar;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO calendar system.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so a value of 0 means 1BC, for example.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time</exception>
        /// <returns>The resulting date/time.</returns>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
            : this(year, month, day, hour, minute, second, millisecond, 0, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so a value of 0 means 1BC, for example.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="calendar">The calendar.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time</exception>
        /// <returns>The resulting date/time.</returns>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, CalendarSystem calendar)
            : this(year, month, day, hour, minute, second, millisecond, 0, calendar)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so a value of 0 means 1BC, for example.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="tickWithinMillisecond">The tick within millisecond.</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time</exception>
        /// <returns>The resulting date/time.</returns>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int tickWithinMillisecond)
            : this(year, month, day, hour, minute, second, millisecond, tickWithinMillisecond, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so a value of 0 means 1BC, for example.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="tickWithinMillisecond">The tick within millisecond.</param>
        /// <param name="calendar">The calendar.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time</exception>
        /// <returns>The resulting date/time.</returns>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int tickWithinMillisecond, CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, "calendar");
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
        public int Month { get { return calendar.Fields.MonthOfYear.GetValue(localInstant); } }

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
        public int Day { get { return calendar.Fields.DayOfMonth.GetValue(localInstant); } }

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
        public int Hour { get { return calendar.Fields.HourOfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the hour of the half-day of this local date and time, in the range 1 to 12 inclusive.
        /// </summary>
        public int ClockHourOfHalfDay { get { return calendar.Fields.ClockHourOfHalfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the minute of this local date and time, in the range 0 to 59 inclusive.
        /// </summary>
        public int Minute { get { return calendar.Fields.MinuteOfHour.GetValue(localInstant); } }

        /// <summary>
        /// Gets the second of this local date and time within the minute, in the range 0 to 59 inclusive.
        /// </summary>
        public int Second { get { return calendar.Fields.SecondOfMinute.GetValue(localInstant); } }

        /// <summary>
        /// Gets the second of this local date and time within the day, in the range 0 to 86,399 inclusive.
        /// </summary>
        public int SecondOfDay { get { return calendar.Fields.SecondOfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the millisecond of this local date and time within the second, in the range 0 to 999 inclusive.
        /// </summary>
        public int Millisecond { get { return calendar.Fields.MillisecondOfSecond.GetValue(localInstant); } }

        /// <summary>
        /// Gets the millisecond of this local date and time within the day, in the range 0 to 86,399,999 inclusive.
        /// </summary>
        public int MillisecondOfDay { get { return calendar.Fields.MillisecondOfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the tick of this local date and time within the millisecond, in the range 0 to 9,999 inclusive.
        /// </summary>
        public int Tick { get { return calendar.Fields.TickOfMillisecond.GetValue(localInstant); } }

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
        public LocalDate Date
        { 
            get 
            { 
                // Work out how far into the current day we are, and subtract that from our current ticks.
                // This is much quicker than finding out the current day, month, year etc and then reconstructing everything.
                long dayTicks = localInstant.Ticks % NodaConstants.TicksPerStandardDay;
                if (dayTicks < 0)
                {
                    dayTicks += NodaConstants.TicksPerStandardDay;
                }
                return new LocalDate(new LocalDateTime(new LocalInstant(localInstant.Ticks - dayTicks), calendar));
            }
        }

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
        public static LocalDateTime FromDateTime(DateTime dateTime)
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
        /// Compares two LocalDateTime values to see if the left one is strictly earlier than the right
        /// one.
        /// </summary>
        /// <remarks>
        /// This operator always returns false if the two operands have different calendars. See the top-level type
        /// documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <returns>true if the <paramref name="lhs"/> is strictly earlier than <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator <(LocalDateTime lhs, LocalDateTime rhs)
        {
            return lhs.LocalInstant < rhs.LocalInstant && Equals(lhs.calendar, rhs.calendar);
        }

        /// <summary>
        /// Compares two LocalDateTime values to see if the left one is earlier than or equal to the right
        /// one.
        /// </summary>
        /// <remarks>
        /// This operator always returns false if the two operands have different calendars. See the top-level type
        /// documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <returns>true if the <paramref name="lhs"/> is earlier than or equal to <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator <=(LocalDateTime lhs, LocalDateTime rhs)
        {
            return lhs.LocalInstant <= rhs.LocalInstant && Equals(lhs.calendar, rhs.calendar);
        }

        /// <summary>
        /// Compares two LocalDateTime values to see if the left one is strictly later than the right
        /// one.
        /// </summary>
        /// <remarks>
        /// This operator always returns false if the two operands have different calendars. See the top-level type
        /// documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <returns>true if the <paramref name="lhs"/> is strictly later than <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator >(LocalDateTime lhs, LocalDateTime rhs)
        {
            return lhs.LocalInstant > rhs.LocalInstant && Equals(lhs.calendar, rhs.calendar);
        }

        /// <summary>
        /// Compares two LocalDateTime values to see if the left one is later than or equal to the right
        /// one.
        /// </summary>
        /// <remarks>
        /// This operator always returns false if the two operands have different calendars. See the top-level type
        /// documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <returns>true if the <paramref name="lhs"/> is later than or equal to <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator >=(LocalDateTime lhs, LocalDateTime rhs)
        {
            return lhs.LocalInstant >= rhs.LocalInstant && Equals(lhs.calendar, rhs.calendar);
        }

        /// <summary>
        /// Indicates whether this date/time is earlier, later or the same as another one. This is purely
        /// done in terms of the local instant represented; the calendar system is ignored. This can lead
        /// to surprising results - for example, 1945 in the ISO calendar corresponds to around 1364
        /// in the Islamic calendar, so an Islamic date in year 1400 is "after" a date in 1945 in the ISO calendar.
        /// </summary>
        /// <param name="other">The other date/time to compare this one with</param>
        /// <returns>A value less than zero if this date/time is earlier than <paramref name="other"/>;
        /// zero if this date/time is the same as <paramref name="other"/>; a value greater than zero if this date/time is
        /// later than <paramref name="other"/>.</returns>
        public int CompareTo(LocalDateTime other)
        {
            return LocalInstant.CompareTo(other.LocalInstant);
        }

        /// <summary>
        /// Adds a period to a local date/time. Fields are added in the order provided by the period.
        /// This is a convenience operator over the <see cref="Plus"/> method.
        /// </summary>
        /// <param name="localDateTime">Initial local date and time</param>
        /// <param name="period">Period to add</param>
        /// <returns>The resulting local date and time</returns>
        public static LocalDateTime operator +(LocalDateTime localDateTime, Period period)
        {
            return localDateTime.Plus(period);
        }

        /// <summary>
        /// Add the specified period to the date and time. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="localDateTime">Initial local date and time</param>
        /// <param name="period">Period to add</param>
        /// <returns>The resulting local date and time</returns>
        public static LocalDateTime Add(LocalDateTime localDateTime, Period period)
        {
            return localDateTime.Plus(period);
        }

        /// <summary>
        /// Adds a period to this local date/time. Fields are added in the order provided by the period.
        /// </summary>
        /// <param name="period">Period to add</param>
        /// <returns>The resulting local date and time</returns>
        public LocalDateTime Plus(Period period)
        {
            Preconditions.CheckNotNull(period, "period");
            return new LocalDateTime(period.AddTo(localInstant, calendar, 1), calendar);
        }

        /// <summary>
        /// Subtracts a period from a local date/time. Fields are subtracted in the order provided by the period.
        /// This is a convenience operator over the <see cref="Minus"/> method.
        /// </summary>
        /// <param name="localDateTime">Initial local date and time</param>
        /// <param name="period">Period to subtract</param>
        /// <returns>The resulting local date and time</returns>
        public static LocalDateTime operator -(LocalDateTime localDateTime, Period period)
        {
            return localDateTime.Minus(period);
        }

        /// <summary>
        /// Subtracts the specified period from the date and time. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="localDateTime">Initial local date and time</param>
        /// <param name="period">Period to subtract</param>
        /// <returns>The resulting local date and time</returns>
        public static LocalDateTime Subtract(LocalDateTime localDateTime, Period period)
        {
            return localDateTime.Minus(period);
        }

        /// <summary>
        /// Subtracts a period from a local date/time. Fields are subtracted in the order provided by the period.
        /// This is a convenience operator over the <see cref="Minus"/> method.
        /// </summary>
        /// <param name="period">Period to subtract</param>
        /// <returns>The resulting local date and time</returns>
        public LocalDateTime Minus(Period period)
        {
            Preconditions.CheckNotNull(period, "period");
            return new LocalDateTime(period.AddTo(localInstant, calendar, -1), calendar);
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
            Preconditions.CheckNotNull(calendarSystem, "calendarSystem");
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

        /// <summary>
        /// Returns the next <see cref="LocalDateTime" /> falling on the specified <see cref="IsoDayOfWeek"/>,
        /// at the same time of day as this value.
        /// This is a strict "next" - if this value on already falls on the target
        /// day of the week, the returned value will be a week later.
        /// </summary>
        /// <param name="targetDayOfWeek">The ISO day of the week to return the next date of.</param>
        /// <returns>The next <see cref="LocalDate"/> falling on the specified day of the week.</returns>
        /// <exception cref="InvalidOperationException">The underlying calendar doesn't use ISO days of the week.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="targetDayOfWeek"/> is not a valid day of the
        /// week (Monday to Sunday).</exception>
        public LocalDateTime Next(IsoDayOfWeek targetDayOfWeek)
        {
            // Avoids boxing...
            if (targetDayOfWeek < IsoDayOfWeek.Monday || targetDayOfWeek > IsoDayOfWeek.Sunday)
            {
                throw new ArgumentOutOfRangeException("targetDayOfWeek");
            }
            // This will throw the desired exception for calendars with different week systems.
            IsoDayOfWeek thisDay = IsoDayOfWeek;
            int difference = targetDayOfWeek - thisDay;
            if (difference <= 0)
            {
                difference += 7;
            }
            return PlusDays(difference);
        }

        /// <summary>
        /// Returns the previous <see cref="LocalDate" /> falling on the specified <see cref="IsoDayOfWeek"/>,
        /// at the same time of day as this value.
        /// This is a strict "previous" - if this value on already falls on the target
        /// day of the week, the returned value will be a week earlier.
        /// </summary>
        /// <param name="targetDayOfWeek">The ISO day of the week to return the previous date of.</param>
        /// <returns>The previous <see cref="LocalDate"/> falling on the specified day of the week.</returns>
        /// <exception cref="InvalidOperationException">The underlying calendar doesn't use ISO days of the week.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="targetDayOfWeek"/> is not a valid day of the
        /// week (Monday to Sunday).</exception>
        public LocalDateTime Previous(IsoDayOfWeek targetDayOfWeek)
        {
            // Avoids boxing...
            if (targetDayOfWeek < IsoDayOfWeek.Monday || targetDayOfWeek > IsoDayOfWeek.Sunday)
            {
                throw new ArgumentOutOfRangeException("targetDayOfWeek");
            }
            // This will throw the desired exception for calendars with different week systems.
            IsoDayOfWeek thisDay = IsoDayOfWeek;
            int difference = targetDayOfWeek - thisDay;
            if (difference >= 0)
            {
                difference -= 7;
            }
            return PlusDays(difference);
        }

        /// <summary>
        /// Returns the mapping of this local date/time within the given <see cref="DateTimeZone" />,
        /// with "strict" rules applied such that an exception is thrown if either the mapping is
        /// ambiguous or the time is skipped.
        /// </summary>
        /// <remarks>
        /// This is solely a convenience method for calling <see cref="DateTimeZone.AtStrictly" />.
        /// </remarks>
        /// <param name="zone">The time zone in which to map this local date/time.</param>
        /// <exception cref="ArgumentNullException"><paramref name="zone"/> is null</exception>
        /// <returns>The result of mapping this local date/time in the given time zone.</returns>
        public ZonedDateTime InZoneStrictly(DateTimeZone zone)
        {
            Preconditions.CheckNotNull(zone, "zone");
            return zone.AtStrictly(this);
        }

        /// <summary>
        /// Returns the mapping of this local date/time within the given <see cref="DateTimeZone" />,
        /// with "lenient" rules applied such that ambiguous values map to the
        /// later of the alternatives, and "skipped" values map to the start of the zone interval
        /// after the "gap".
        /// </summary>
        /// <remarks>
        /// This is solely a convenience method for calling <see cref="DateTimeZone.AtLeniently" />.
        /// </remarks>
        /// <param name="zone">The time zone in which to map this local date/time.</param>
        /// <exception cref="ArgumentNullException"><paramref name="zone"/> is null</exception>
        /// <returns>The result of mapping this local date/time in the given time zone.</returns>
        public ZonedDateTime InZoneLeniently(DateTimeZone zone)
        {
            Preconditions.CheckNotNull(zone, "zone");
            return zone.AtLeniently(this);
        }

        /// <summary>
        /// Resolves this local date and time into a <see cref="ZonedDateTime"/> in the given time zone, following
        /// the given <see cref="ZoneLocalMappingResolver"/> to handle ambiguity and skipped times.
        /// </summary>
        /// <remarks>
        /// This is a convenience method for calling <see cref="DateTimeZone.ResolveLocal"/>.
        /// </remarks>
        /// <param name="zone">The time zone to map this local date and time into</param>
        /// <param name="resolver">The resolver to apply to the mapping.</param>
        /// <exception cref="ArgumentNullException"><paramref name="zone"/> or <paramref name="resolver"/> is null</exception>
        /// <returns>The result of resolving the mapping.</returns>
        public ZonedDateTime InZone(DateTimeZone zone, ZoneLocalMappingResolver resolver)
        {
            Preconditions.CheckNotNull(zone, "zone");
            Preconditions.CheckNotNull(resolver, "resolver");
            return zone.ResolveLocal(this, resolver);
        }

        #region Formatting
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance. Equivalent to
        /// calling <c>ToString(null)</c>.
        /// </summary>
        /// <returns>
        /// The value of the current instance in the standard format pattern, using the current thread's
        /// culture to obtain a format provider.
        /// </returns>
        public override string ToString()
        {
            return LocalDateTimePattern.BclSupport.Format(this, null, NodaFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Formats the value of the current instance using the specified pattern.
        /// </summary>
        /// <returns>
        /// The value of the current instance in the specified format.
        /// </returns>
        /// <param name="patternText">The <see cref="T:System.String" /> specifying the pattern to use,
        /// or null to use the default format pattern.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string patternText)
        {
            return LocalDateTimePattern.BclSupport.Format(this, patternText, NodaFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Formats the value of the current instance using the specified <see cref="IFormatProvider" />.
        /// </summary>
        /// <returns>
        /// The formatted value of the current instance.
        /// </returns>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when formatting the value,
        /// or null to use the current thread's culture to obtain a format provider.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(IFormatProvider formatProvider)
        {
            return LocalDateTimePattern.BclSupport.Format(this, null, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        /// Formats the value of the current instance using the specified pattern.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="patternText">The <see cref="T:System.String" /> specifying the pattern to use,
        /// or null to use the default format pattern.
        /// </param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when formatting the value,
        /// or null to use the current thread's culture to obtain a format provider.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string patternText, IFormatProvider formatProvider)
        {
            return LocalDateTimePattern.BclSupport.Format(this, patternText, NodaFormatInfo.GetInstance(formatProvider));
        }
        #endregion Formatting

        #region Parsing
        /// <summary>
        /// Parses the given string using the current culture's default format provider.
        /// </summary>
        /// <param name="value">The text to parse.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <returns>The parsed value.</returns>
        public static LocalDateTime Parse(string value)
        {
            return LocalDateTimePattern.BclSupport.Parse(value, NodaFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Parses the given string using the specified format provider.
        /// </summary>
        /// <param name="value">The text to parse.</param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when parsing the text,
        /// or null to use the current thread's culture to obtain a format provider.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <returns>The parsed value.</returns>
        public static LocalDateTime Parse(string value, IFormatProvider formatProvider)
        {
            return LocalDateTimePattern.BclSupport.Parse(value, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        /// Parses the given string using the specified pattern and format provider.
        /// </summary>
        /// <param name="value">The text to parse.</param>
        /// <param name="patternText">The text of the pattern to use for parsing.</param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when parsing the text,
        /// or null to use the current thread's culture to obtain a format provider.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> or <paramref name="patternText"/> is null.</exception>
        /// <returns>The parsed value.</returns>
        public static LocalDateTime ParseExact(string value, string patternText, IFormatProvider formatProvider)
        {
            return LocalDateTimePattern.BclSupport.ParseExact(value, patternText, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        /// Parses the given string using the specified patterns and format provider.
        /// </summary>
        /// <param name="value">The text to parse.</param>
        /// <param name="patterns">The patterns to use for parsing.</param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when parsing the text,
        /// or null to use the current thread's culture to obtain a format provider.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> or <paramref name="patterns"/> is null.</exception>
        /// <exception cref="InvalidPatternException"><paramref name="patterns"/> is empty.</exception>
        /// <returns>The parsed value.</returns>
        public static LocalDateTime ParseExact(string value, string[] patterns, IFormatProvider formatProvider)
        {
            return LocalDateTimePattern.BclSupport.ParseExact(value, patterns, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        /// Attempts to parse the given string using the current culture's default format provider. If the parse is successful,
        /// the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Instant.MinValue"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <param name="value">The text to parse.</param>
        /// <param name="result">The parsed value, when successful.</param>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParse(string value, out LocalDateTime result)
        {
            return LocalDateTimePattern.BclSupport.TryParse(value, NodaFormatInfo.CurrentInfo, out result);
        }

        /// <summary>
        /// Attempts to parse the given string using the specified format provider.
        /// If the parse is successful, the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Instant.MinValue"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <param name="value">The text to parse.</param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when parsing the text,
        /// or null to use the current thread's culture to obtain a format provider.</param>
        /// <param name="result">The parsed value, when successful.</param>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParse(string value, IFormatProvider formatProvider, out LocalDateTime result)
        {
            return LocalDateTimePattern.BclSupport.TryParse(value, NodaFormatInfo.GetInstance(formatProvider), out result);
        }

        /// <summary>
        /// Attempts to parse the given string using the specified pattern and format provider.
        /// If the parse is successful, the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Instant.MinValue"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <param name="value">The text to parse.</param>
        /// <param name="patternText">The text of the pattern to use for parsing.</param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when parsing the text,
        /// or null to use the current thread's culture to obtain a format provider.</param>
        /// <param name="result">The parsed value, when successful.</param>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParseExact(string value, string patternText, IFormatProvider formatProvider, out LocalDateTime result)
        {
            return LocalDateTimePattern.BclSupport.TryParseExact(value, patternText, NodaFormatInfo.GetInstance(formatProvider), out result);
        }

        /// <summary>
        /// Attempts to parse the given string using the specified patterns and format provider.
        /// If the parse is successful, the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Instant.MinValue"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <param name="value">The text to parse.</param>
        /// <param name="patterns">The patterns to use for parsing.</param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when parsing the text,
        /// or null to use the current thread's culture to obtain a format provider.</param>
        /// <param name="result">The parsed value, when successful.</param>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParseExact(string value, string[] patterns, IFormatProvider formatProvider, out LocalDateTime result)
        {
            return LocalDateTimePattern.BclSupport.TryParseExact(value, patterns, NodaFormatInfo.GetInstance(formatProvider), out result);
        }
        #endregion Parsing
    }
}
