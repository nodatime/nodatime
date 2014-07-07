// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using JetBrains.Annotations;
using NodaTime.Calendars;
using NodaTime.Fields;
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
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
#if !PCL
    [Serializable]
#endif
    public struct LocalDateTime : IEquatable<LocalDateTime>, IComparable<LocalDateTime>, IComparable, IFormattable, IXmlSerializable
#if !PCL
        , ISerializable
#endif
    {
        private readonly LocalDate date;
        private readonly LocalTime time;

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
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the given
        /// calendar system.
        /// </summary>
        /// <param name="localInstant">The local instant.</param>
        /// <param name="calendar">The calendar system.</param>
        /// <returns>The resulting date/time.</returns>
        internal LocalDateTime(LocalInstant localInstant, [NotNull] CalendarSystem calendar)
        {
            // This will validate that calendar is non-null.
            date = new LocalDate(localInstant.DaysSinceEpoch, calendar);
            time = new LocalTime(localInstant.NanosecondOfDay);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO calendar system.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year",
        /// so a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute)
            : this(new LocalDate(year, month, day),
                   new LocalTime(hour, minute))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so, for
        /// the ISO calendar, a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, [NotNull] CalendarSystem calendar)
            : this(new LocalDate(year, month, day, calendar),
                   new LocalTime(hour, minute))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO calendar system.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year",
        /// so a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second)
            : this(new LocalDate(year, month, day),
                   new LocalTime(hour, minute, second))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so, for
        /// the ISO calendar, a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, [NotNull] CalendarSystem calendar)
            : this(new LocalDate(year, month, day, calendar),
                   new LocalTime(hour, minute, second))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO calendar system.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year",
        /// so a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
            : this(new LocalDate(year, month, day),
                   new LocalTime(hour, minute, second, millisecond))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so, for
        /// the ISO calendar, a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, [NotNull] CalendarSystem calendar)
            : this(year, month, day, hour, minute, second, millisecond, 0, calendar)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year",
        /// so a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="tickWithinMillisecond">The tick within millisecond.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int tickWithinMillisecond)
            : this(new LocalDate(year, month, day),
                   new LocalTime(hour, minute, second, millisecond, tickWithinMillisecond))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so, for
        /// the ISO calendar, a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="tickWithinMillisecond">The tick within millisecond.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns>The resulting date/time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date/time.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int tickWithinMillisecond, [NotNull] CalendarSystem calendar)
            : this(new LocalDate(year, month, day, calendar),
                   new LocalTime(hour, minute, second, millisecond, tickWithinMillisecond))
        {
        }

        internal LocalDateTime(LocalDate date, LocalTime time)
        {
            this.date = date;
            this.time = time;
        }

        /// <summary>Gets the calendar system associated with this local date and time.</summary>
        public CalendarSystem Calendar
        {
            get { return date.Calendar; }
        }

        /// <summary>Gets the century within the era of this local date and time.</summary>
        public int CenturyOfEra { get { return date.CenturyOfEra; } }

        /// <summary>Gets the year of this local date and time.</summary>
        /// <remarks>This returns the "absolute year", so, for the ISO calendar,
        /// a value of 0 means 1 BC, for example.</remarks>
        public int Year { get { return date.Year; } }

        /// <summary>Gets the year of this local date and time within its century.</summary>
        /// <remarks>This always returns a value in the range 0 to 99 inclusive.</remarks>
        public int YearOfCentury { get { return date.YearOfCentury; } }

        /// <summary>Gets the year of this local date and time within its era.</summary>
        public int YearOfEra { get { return date.YearOfEra; } }

        /// <summary>Gets the era of this local date and time.</summary>
        public Era Era { get { return date.Era; } }

        /// <summary>
        /// Gets the "week year" of this local date and time.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The WeekYear is the year that matches with the <see cref="WeekOfWeekYear"/> field.
        /// In the standard ISO8601 week algorithm, the first week of the year
        /// is that in which at least 4 days are in the year. As a result of this
        /// definition, day 1 of the first week may be in the previous year.
        /// The WeekYear allows you to query the effective year for that day.
        /// </para>
        /// <para>
        /// For example, January 1st 2011 was a Saturday, so only two days of that week
        /// (Saturday and Sunday) were in 2011. Therefore January 1st is part of
        /// week 52 of WeekYear 2010. Conversely, December 31st 2012 is a Monday,
        /// so is part of week 1 of WeekYear 2013.
        /// </para>
        /// </remarks>
        public int WeekYear { get { return date.WeekYear; } }

        /// <summary>
        /// Gets the month of this local date and time within the year.
        /// </summary>
        public int Month { get { return date.Month; } }

        /// <summary>
        /// Gets the week within the WeekYear. See <see cref="WeekYear"/> for more details.
        /// </summary>
        public int WeekOfWeekYear { get { return date.WeekOfWeekYear; } }

        /// <summary>
        /// Gets the day of this local date and time within the year.
        /// </summary>
        public int DayOfYear { get { return date.DayOfYear; } }

        /// <summary>
        /// Gets the day of this local date and time within the month.
        /// </summary>
        public int Day { get { return date.Day; } }

        /// <summary>
        /// Gets the week day of this local date and time expressed as an <see cref="NodaTime.IsoDayOfWeek"/> value,
        /// for calendars which use ISO days of the week.
        /// </summary>
        /// <exception cref="InvalidOperationException">The underlying calendar doesn't use ISO days of the week.</exception>
        /// <seealso cref="DayOfWeek"/>
        public IsoDayOfWeek IsoDayOfWeek { get { return date.IsoDayOfWeek; } }

        /// <summary>
        /// Gets the week day of this local date and time as a number.
        /// </summary>
        /// <remarks>
        /// For calendars using ISO week days, this gives 1 for Monday to 7 for Sunday.
        /// </remarks>
        /// <seealso cref="IsoDayOfWeek"/>
        public int DayOfWeek { get { return date.DayOfWeek; } }

        /// <summary>
        /// Gets the hour of day of this local date and time, in the range 0 to 23 inclusive.
        /// </summary>
        public int Hour { get { return time.Hour; } }

        /// <summary>
        /// Gets the hour of the half-day of this local date and time, in the range 1 to 12 inclusive.
        /// </summary>
        public int ClockHourOfHalfDay { get { return time.ClockHourOfHalfDay; } }

        /// <summary>
        /// Gets the minute of this local date and time, in the range 0 to 59 inclusive.
        /// </summary>
        public int Minute { get { return time.Minute; } }

        /// <summary>
        /// Gets the second of this local date and time within the minute, in the range 0 to 59 inclusive.
        /// </summary>
        public int Second { get { return time.Second; } }

        /// <summary>
        /// Gets the millisecond of this local date and time within the second, in the range 0 to 999 inclusive.
        /// </summary>
        public int Millisecond { get { return time.Millisecond; } }

        /// <summary>
        /// Gets the tick of this local time within the second, in the range 0 to 9,999,999 inclusive.
        /// </summary>
        public int TickOfSecond { get { return time.TickOfSecond; } }

        /// <summary>
        /// Gets the tick of this local date and time within the day, in the range 0 to 863,999,999,999 inclusive.
        /// </summary>
        public long TickOfDay { get { return time.TickOfDay; } }

        /// <summary>
        /// Gets the nanosecond of this local time within the second, in the range 0 to 999,999,999 inclusive.
        /// </summary>
        public int NanosecondOfSecond { get { return time.NanosecondOfSecond; } }

        /// <summary>
        /// Gets the nanosecond of this local date and time within the day, in the range 0 to 86,399,999,999,999 inclusive.
        /// </summary>
        public long NanosecondOfDay { get { return time.NanosecondOfDay; } }

        /// <summary>
        /// Gets the time portion of this local date and time as a <see cref="LocalTime"/>.
        /// </summary>
        public LocalTime TimeOfDay { get { return time; } }

        /// <summary>
        /// Gets the date portion of this local date and time as a <see cref="LocalDate"/> in the same calendar system as this value.
        /// </summary>
        public LocalDate Date { get { return date; } }

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
        [Pure]
        public DateTime ToDateTimeUnspecified()
        {
            return ToLocalInstant().ToDateTimeUnspecified();
        }

        [Pure]
        internal LocalInstant ToLocalInstant()
        {
            return new LocalInstant(date.DaysSinceEpoch, time.NanosecondOfDay);
        }

        /// <summary>
        /// Converts a <see cref="DateTime" /> of any kind to a LocalDateTime in the ISO calendar. This does not perform
        /// any time zone conversions, so a DateTime with a <see cref="DateTime.Kind"/> of <see cref="DateTimeKind.Utc"/>
        /// will still have the same day/hour/minute etc - it won't be converted into the local system time.
        /// </summary>
        /// <param name="dateTime">Value to convert into a Noda Time local date and time</param>
        /// <returns>A new <see cref="LocalDateTime"/> with the same values as the specified <c>DateTime</c>.</returns>
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
            return date == other.Date && time == other.time;
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
            return Equals(lhs.Calendar, rhs.Calendar) && lhs.CompareTo(rhs) < 0;
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
            return Equals(lhs.Calendar, rhs.Calendar) && lhs.CompareTo(rhs) <= 0;
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
            return Equals(lhs.Calendar, rhs.Calendar) && lhs.CompareTo(rhs) > 0;
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
            return Equals(lhs.Calendar, rhs.Calendar) && lhs.CompareTo(rhs) >= 0;
        }

        /// <summary>
        /// Indicates whether this date/time is earlier, later or the same as another one.
        /// </summary>
        /// <remarks>
        /// The comparison is performed in terms of a calendar-independent notion of dates and times;
        /// the calendar systems of both <see cref="LocalDateTime" /> values are ignored. When both values use the same calendar,
        /// this is absolutely natural. However, when comparing a value in one calendar with a value in another,
        /// this can lead to surprising results. For example, 1945 in the ISO calendar corresponds to around 1364
        /// in the Islamic calendar, so an Islamic date in year 1400 is "after" a date in 1945 in the ISO calendar.
        /// </remarks>
        /// <param name="other">The other local date/time to compare with this value.</param>
        /// <returns>A value less than zero if this date/time is earlier than <paramref name="other"/>;
        /// zero if this date/time is the same as <paramref name="other"/>; a value greater than zero if this date/time is
        /// later than <paramref name="other"/>.</returns>
        public int CompareTo(LocalDateTime other)
        {
            // FIXME(2.0): Work out what we actually want to do if the calendars aren't equal.
            // Probably throw!
            return ToLocalInstant().CompareTo(other.ToLocalInstant());
        }

        /// <summary>
        /// Implementation of <see cref="IComparable.CompareTo"/> to compare two LocalDateTimes.
        /// </summary>
        /// <remarks>
        /// This uses explicit interface implementation to avoid it being called accidentally. The generic implementation should usually be preferred.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is non-null but does not refer to an instance of <see cref="LocalDateTime"/>.</exception>
        /// <param name="obj">The object to compare this value with.</param>
        /// <returns>The result of comparing this LocalDateTime with another one; see <see cref="CompareTo(NodaTime.LocalDateTime)"/> for general details.
        /// If <paramref name="obj"/> is null, this method returns a value greater than 0.
        /// </returns>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            Preconditions.CheckArgument(obj is LocalDateTime, "obj", "Object must be of type NodaTime.LocalDateTime.");
            return CompareTo((LocalDateTime)obj);
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
        [Pure]
        public LocalDateTime Plus(Period period)
        {
            Preconditions.CheckNotNull(period, "period");
            return period.AddTo(date, time, 1);
        }

        /// <summary>
        /// Subtracts a period from a local date/time. Fields are subtracted in the order provided by the period.
        /// This is a convenience operator over the <see cref="Minus(Period)"/> method.
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
        /// </summary>
        /// <param name="period">Period to subtract</param>
        /// <returns>The resulting local date and time</returns>
        [Pure]
        public LocalDateTime Minus(Period period)
        {
            Preconditions.CheckNotNull(period, "period");
            return period.AddTo(date, time, -1);
        }

        /// <summary>
        /// Subtracts one date/time from another, returning the result as a <see cref="Period"/>.
        /// </summary>
        /// <remarks>
        /// This is simply a convenience operator for calling <see cref="Period.Between(NodaTime.LocalDateTime,NodaTime.LocalDateTime)"/>.
        /// The calendar systems of the two date/times must be the same.
        /// </remarks>
        /// <param name="lhs">The date/time to subtract from</param>
        /// <param name="rhs">The date/time to subtract</param>
        /// <returns>The result of subtracting one date/time from another.</returns>
        public static Period operator -(LocalDateTime lhs, LocalDateTime rhs)
        {
            return Period.Between(rhs, lhs);
        }

        /// <summary>
        /// Subtracts one date/time from another, returning the result as a <see cref="Period"/>.
        /// </summary>
        /// <remarks>
        /// This is simply a convenience method for calling <see cref="Period.Between(NodaTime.LocalDateTime,NodaTime.LocalDateTime)"/>.
        /// The calendar systems of the two date/times must be the same.
        /// </remarks>
        /// <param name="lhs">The date/time to subtract from</param>
        /// <param name="rhs">The date/time to subtract</param>
        /// <returns>The result of subtracting one date/time from another.</returns>
        public static Period Subtract(LocalDateTime lhs, LocalDateTime rhs)
        {
            return lhs - rhs;
        }

        /// <summary>
        /// Subtracts the specified date/time from this date/time, returning the result as a <see cref="Period"/>.
        /// Fluent alternative to <c>operator-()</c>.
        /// </summary>
        /// <remarks>The specified date/time must be in the same calendar system as this.</remarks>
        /// <param name="localDateTime">The date/time to subtract from this</param>
        /// <returns>The difference between the specified date/time and this one</returns>
        [Pure]
        public Period Minus(LocalDateTime localDateTime)
        {
            return this - localDateTime;
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
            hash = HashCodeHelper.Hash(hash, date);
            hash = HashCodeHelper.Hash(hash, time);
            hash = HashCodeHelper.Hash(hash, Calendar);
            return hash;
        }
        #endregion
        /// <summary>
        /// Returns this date/time, with the given date adjuster applied to it, maintaing the existing time of day.
        /// </summary>
        /// <remarks>
        /// If the adjuster attempts to construct an
        /// invalid date (such as by trying to set a day-of-month of 30 in February), any exception thrown by
        /// that construction attempt will be propagated through this method.
        /// </remarks>
        /// <param name="adjuster">The adjuster to apply.</param>
        /// <returns>The adjusted date/time.</returns>
        [Pure]
        public LocalDateTime With([NotNull] Func<LocalDate, LocalDate> adjuster)
        {
            return date.With(adjuster) + time;
        }

        /// <summary>
        /// Returns this date/time, with the given time adjuster applied to it, maintaining the existing date.
        /// </summary>
        /// <remarks>
        /// If the adjuster attempts to construct an invalid time, any exception thrown by
        /// that construction attempt will be propagated through this method.
        /// </remarks>
        /// <param name="adjuster">The adjuster to apply.</param>
        /// <returns>The adjusted date/time.</returns>
        [Pure]
        public LocalDateTime With([NotNull] Func<LocalTime, LocalTime> adjuster)
        {
            return date + time.With(adjuster);
        }

        /// <summary>
        /// Creates a new LocalDateTime representing the same physical date and time, but in a different calendar.
        /// The returned LocalDateTime is likely to have different date field values to this one.
        /// For example, January 1st 1970 in the Gregorian calendar was December 19th 1969 in the Julian calendar.
        /// </summary>
        /// <param name="calendarSystem">The calendar system to convert this local date to.</param>
        /// <returns>The converted LocalDateTime.</returns>
        [Pure]
        public LocalDateTime WithCalendar([NotNull] CalendarSystem calendarSystem)
        {
            Preconditions.CheckNotNull(calendarSystem, "calendarSystem");
            return new LocalDateTime(date.WithCalendar(calendarSystem), time);
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
        [Pure]
        public LocalDateTime PlusYears(int years)
        {
            return new LocalDateTime(date.PlusYears(years), time);
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
        [Pure]
        public LocalDateTime PlusMonths(int months)
        {
            return new LocalDateTime(date.PlusMonths(months), time);
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
        [Pure]
        public LocalDateTime PlusDays(int days)
        {
            return new LocalDateTime(date.PlusDays(days), time);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of weeks added.
        /// </summary>
        /// <param name="weeks">The number of weeks to add</param>
        /// <returns>The current value plus the given number of weeks.</returns>
        [Pure]
        public LocalDateTime PlusWeeks(int weeks)
        {
            return new LocalDateTime(date.PlusWeeks(weeks), time);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of hours added.
        /// </summary>
        /// <param name="hours">The number of hours to add</param>
        /// <returns>The current value plus the given number of hours.</returns>
        [Pure]
        public LocalDateTime PlusHours(long hours)
        {
            return TimePeriodField.Hours.Add(this, hours);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of minutes added.
        /// </summary>
        /// <param name="minutes">The number of minutes to add</param>
        /// <returns>The current value plus the given number of minutes.</returns>
        [Pure]
        public LocalDateTime PlusMinutes(long minutes)
        {
            return TimePeriodField.Minutes.Add(this, minutes);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of seconds added.
        /// </summary>
        /// <param name="seconds">The number of seconds to add</param>
        /// <returns>The current value plus the given number of seconds.</returns>
        [Pure]
        public LocalDateTime PlusSeconds(long seconds)
        {
            return TimePeriodField.Seconds.Add(this, seconds);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of milliseconds added.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to add</param>
        /// <returns>The current value plus the given number of milliseconds.</returns>
        [Pure]
        public LocalDateTime PlusMilliseconds(long milliseconds)
        {
            return TimePeriodField.Milliseconds.Add(this, milliseconds);
        }

        /// <summary>
        /// Returns a new LocalDateTime representing the current value with the given number of ticks added.
        /// </summary>
        /// <param name="ticks">The number of ticks to add</param>
        /// <returns>The current value plus the given number of ticks.</returns>
        [Pure]
        public LocalDateTime PlusTicks(long ticks)
        {
            return TimePeriodField.Ticks.Add(this, ticks);
        }

        [Pure]
        internal LocalDateTime PlusNanoseconds(long nanoseconds)
        {
            return TimePeriodField.Nanoseconds.Add(this, nanoseconds);
        }

        /// <summary>
        /// Returns the next <see cref="LocalDateTime" /> falling on the specified <see cref="IsoDayOfWeek"/>,
        /// at the same time of day as this value.
        /// This is a strict "next" - if this value on already falls on the target
        /// day of the week, the returned value will be a week later.
        /// </summary>
        /// <param name="targetDayOfWeek">The ISO day of the week to return the next date of.</param>
        /// <returns>The next <see cref="LocalDateTime"/> falling on the specified day of the week.</returns>
        /// <exception cref="InvalidOperationException">The underlying calendar doesn't use ISO days of the week.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="targetDayOfWeek"/> is not a valid day of the
        /// week (Monday to Sunday).</exception>
        [Pure]
        public LocalDateTime Next(IsoDayOfWeek targetDayOfWeek)
        {
            return new LocalDateTime(Date.Next(targetDayOfWeek), TimeOfDay);
        }

        /// <summary>
        /// Returns the previous <see cref="LocalDateTime" /> falling on the specified <see cref="IsoDayOfWeek"/>,
        /// at the same time of day as this value.
        /// This is a strict "previous" - if this value on already falls on the target
        /// day of the week, the returned value will be a week earlier.
        /// </summary>
        /// <param name="targetDayOfWeek">The ISO day of the week to return the previous date of.</param>
        /// <returns>The previous <see cref="LocalDateTime"/> falling on the specified day of the week.</returns>
        /// <exception cref="InvalidOperationException">The underlying calendar doesn't use ISO days of the week.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="targetDayOfWeek"/> is not a valid day of the
        /// week (Monday to Sunday).</exception>
        [Pure]
        public LocalDateTime Previous(IsoDayOfWeek targetDayOfWeek)
        {
            return new LocalDateTime(Date.Previous(targetDayOfWeek), TimeOfDay);
        }

        /// <summary>
        /// Returns an <see cref="OffsetDateTime"/> for this local date/time with the given offset.
        /// </summary>
        /// <remarks>This method is purely a convenient alternative to calling the <see cref="OffsetDateTime"/> constructor directly.</remarks>
        /// <param name="offset">The offset to apply.</param>
        /// <returns>The result of this local date/time offset by the given amount.</returns>
        [Pure]
        public OffsetDateTime WithOffset(Offset offset)
        {
            return new OffsetDateTime(this, offset);
        }

        /// <summary>
        /// Returns the mapping of this local date/time within <see cref="DateTimeZone.Utc"/>.
        /// </summary>
        /// <remarks>As UTC is a fixed time zone, there is no chance that this local date/time is ambiguous or skipped.</remarks>
        /// <returns>The result of mapping this local date/time in UTC.</returns>
        [Pure]
        public ZonedDateTime InUtc()
        {
            // Use the internal constructor to avoid validation. We know it will be fine.
            return new ZonedDateTime(this, Offset.Zero, DateTimeZone.Utc);
        }

        /// <summary>
        /// Returns the mapping of this local date/time within the given <see cref="DateTimeZone" />,
        /// with "strict" rules applied such that an exception is thrown if either the mapping is
        /// ambiguous or the time is skipped.
        /// </summary>
        /// <remarks>
        /// See <see cref="InZoneLeniently"/> and <see cref="InZone"/> for alternative ways to map a local time to a
        /// specific instant.
        /// This is solely a convenience method for calling <see cref="DateTimeZone.AtStrictly" />.
        /// </remarks>
        /// <param name="zone">The time zone in which to map this local date/time.</param>
        /// <exception cref="SkippedTimeException">This local date/time is skipped in the given time zone.</exception>
        /// <exception cref="AmbiguousTimeException">This local date/time is ambiguous in the given time zone.</exception>
        /// <returns>The result of mapping this local date/time in the given time zone.</returns>
        [Pure]
        public ZonedDateTime InZoneStrictly([NotNull] DateTimeZone zone)
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
        /// See <see cref="InZoneStrictly"/> and <see cref="InZone"/> for alternative ways to map a local time to a
        /// specific instant.
        /// This is solely a convenience method for calling <see cref="DateTimeZone.AtLeniently" />.
        /// </remarks>
        /// <param name="zone">The time zone in which to map this local date/time.</param>
        /// <returns>The result of mapping this local date/time in the given time zone.</returns>
        [Pure]
        public ZonedDateTime InZoneLeniently([NotNull] DateTimeZone zone)
        {
            Preconditions.CheckNotNull(zone, "zone");
            return zone.AtLeniently(this);
        }

        /// <summary>
        /// Resolves this local date and time into a <see cref="ZonedDateTime"/> in the given time zone, following
        /// the given <see cref="ZoneLocalMappingResolver"/> to handle ambiguity and skipped times.
        /// </summary>
        /// <remarks>
        /// See <see cref="InZoneStrictly"/> and <see cref="InZoneLeniently"/> for alternative ways to map a local time
        /// to a specific instant.
        /// This is a convenience method for calling <see cref="DateTimeZone.ResolveLocal"/>.
        /// </remarks>
        /// <param name="zone">The time zone to map this local date and time into</param>
        /// <param name="resolver">The resolver to apply to the mapping.</param>
        /// <returns>The result of resolving the mapping.</returns>
        [Pure]
        public ZonedDateTime InZone([NotNull] DateTimeZone zone, [NotNull] ZoneLocalMappingResolver resolver)
        {
            Preconditions.CheckNotNull(zone, "zone");
            Preconditions.CheckNotNull(resolver, "resolver");
            return zone.ResolveLocal(this, resolver);
        }

        #region Formatting
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// The value of the current instance in the default format pattern ("G"), using the current thread's
        /// culture to obtain a format provider.
        /// </returns>
        public override string ToString()
        {
            return LocalDateTimePattern.BclSupport.Format(this, null, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Formats the value of the current instance using the specified pattern.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="patternText">The <see cref="T:System.String" /> specifying the pattern to use,
        /// or null to use the default format pattern ("G").
        /// </param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when formatting the value,
        /// or null to use the current thread's culture to obtain a format provider.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string patternText, IFormatProvider formatProvider)
        {
            return LocalDateTimePattern.BclSupport.Format(this, patternText, formatProvider);
        }
        #endregion Formatting

        #region XML serialization
        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Preconditions.CheckNotNull(reader, "reader");
            var pattern = LocalDateTimePattern.ExtendedIsoPattern;
            if (reader.MoveToAttribute("calendar"))
            {
                string newCalendarId = reader.Value;
                CalendarSystem newCalendar = CalendarSystem.ForId(newCalendarId);
                var newTemplateValue = pattern.TemplateValue.WithCalendar(newCalendar);
                pattern = pattern.WithTemplateValue(newTemplateValue);
                reader.MoveToElement();
            }
            string text = reader.ReadElementContentAsString();
            this = pattern.Parse(text).Value;
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Preconditions.CheckNotNull(writer, "writer");
            if (Calendar != CalendarSystem.Iso)
            {
                writer.WriteAttributeString("calendar", Calendar.Id);
            }
            writer.WriteString(LocalDateTimePattern.ExtendedIsoPattern.Format(this));
        }
        #endregion

#if !PCL
        #region Binary serialization
        private const string DaysSerializationName = "days";
        private const string NanosecondOfDaySerializationName = "nanoOfDay";
        private const string CalendarIdSerializationName = "calendar";

        /// <summary>
        /// Private constructor only present for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        /// <param name="context">The source for this deserialization.</param>
        private LocalDateTime(SerializationInfo info, StreamingContext context)
            : this(new LocalDate(info.GetInt32(DaysSerializationName),
                                 CalendarSystem.ForId(info.GetString(CalendarIdSerializationName))),
                   LocalTime.FromNanosecondsSinceMidnight(info.GetInt64(NanosecondOfDaySerializationName)))
        {
        }

        /// <summary>
        /// Implementation of <see cref="ISerializable.GetObjectData"/>.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination for this serialization.</param>
        [System.Security.SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // FIXME(2.0): Revisit the serialization format
            info.AddValue(DaysSerializationName, date.DaysSinceEpoch);
            info.AddValue(NanosecondOfDaySerializationName, time.NanosecondOfDay);
            info.AddValue(CalendarIdSerializationName, Calendar.Id);
        }
        #endregion
#endif
    }
}
