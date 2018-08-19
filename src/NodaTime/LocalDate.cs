// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Calendars;
using NodaTime.Fields;
using NodaTime.Text;
using NodaTime.Utility;

namespace NodaTime
{
    // TODO(feature): Calendar-neutral comparer.

    /// <summary>
    /// LocalDate is an immutable struct representing a date within the calendar,
    /// with no reference to a particular time zone or time of day.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Values can freely be compared for equality: a value in a different calendar system is not equal to
    /// a value in a different calendar system. However, ordering comparisons (either via the <see cref="CompareTo"/> method
    /// or via operators) fail with <see cref="ArgumentException"/>; attempting to compare values in different calendars
    /// almost always indicates a bug in the calling code.
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
#if !NETSTANDARD
    [Serializable]
#endif
    public struct LocalDate : IEquatable<LocalDate>, IComparable<LocalDate>, IComparable, IFormattable, IXmlSerializable
#if !NETSTANDARD
        , ISerializable
#endif
    {
        [ReadWriteForEfficiency] private YearMonthDayCalendar yearMonthDayCalendar;

        /// <summary>
        /// The maximum (latest) date representable in the ISO calendar system.
        /// </summary>
        public static LocalDate MaxIsoValue => new LocalDate(new YearMonthDayCalendar(GregorianYearMonthDayCalculator.MaxGregorianYear, 12, 31, CalendarOrdinal.Iso));

        /// <summary>
        /// The minimum (earliest) date representable in the ISO calendar system.
        /// </summary>
        public static LocalDate MinIsoValue => new LocalDate(new YearMonthDayCalendar(GregorianYearMonthDayCalculator.MinGregorianYear, 1, 1, CalendarOrdinal.Iso));

        /// <summary>
        /// Constructs an instance from values which are assumed to already have been validated.
        /// </summary>
        internal LocalDate([Trusted] YearMonthDayCalendar yearMonthDayCalendar)
        {
            this.yearMonthDayCalendar = yearMonthDayCalendar;
        }
       
        /// <summary>
        /// Constructs an instance from the number of days since the unix epoch, in the ISO
        /// calendar system.
        /// </summary>
        internal LocalDate([Trusted] int daysSinceEpoch)
        {
            Preconditions.DebugCheckArgumentRange(nameof(daysSinceEpoch), daysSinceEpoch, CalendarSystem.Iso.MinDays, CalendarSystem.Iso.MaxDays);
            this.yearMonthDayCalendar = GregorianYearMonthDayCalculator.GetGregorianYearMonthDayCalendarFromDaysSinceEpoch(daysSinceEpoch);
        }

        /// <summary>
        /// Constructs an instance from the number of days since the unix epoch, and a calendar
        /// system. The calendar system is assumed to be non-null, but the days since the epoch are
        /// validated.
        /// </summary>
        internal LocalDate(int daysSinceEpoch, [Trusted] [NotNull] CalendarSystem calendar)
        {
            Preconditions.DebugCheckNotNull(calendar, nameof(calendar));
            this.yearMonthDayCalendar = calendar.GetYearMonthDayCalendarFromDaysSinceEpoch(daysSinceEpoch);
        }

        /// <summary>
        /// Constructs an instance for the given year, month and day in the ISO calendar.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <returns>The resulting date.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date.</exception>
        public LocalDate(int year, int month, int day)
        {
            GregorianYearMonthDayCalculator.ValidateGregorianYearMonthDay(year, month, day);
            yearMonthDayCalendar = new YearMonthDayCalendar(year, month, day, CalendarOrdinal.Iso);
        }

        /// <summary>
        /// Constructs an instance for the given year, month and day in the specified calendar.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so, for
        /// the ISO calendar, a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="calendar">Calendar system in which to create the date.</param>
        /// <returns>The resulting date.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date.</exception>
        public LocalDate(int year, int month, int day, [NotNull] CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, nameof(calendar));
            calendar.ValidateYearMonthDay(year, month, day);
            yearMonthDayCalendar = new YearMonthDayCalendar(year, month, day, calendar.Ordinal);
        }

        /// <summary>
        /// Constructs an instance for the given era, year of era, month and day in the ISO calendar.
        /// </summary>
        /// <param name="era">The era within which to create a date. Must be a valid era within the ISO calendar.</param>
        /// <param name="yearOfEra">The year of era.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <returns>The resulting date.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date.</exception>
        public LocalDate([NotNull] Era era, int yearOfEra, int month, int day)
            : this(era, yearOfEra, month, day, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Constructs an instance for the given era, year of era, month and day in the specified calendar.
        /// </summary>
        /// <param name="era">The era within which to create a date. Must be a valid era within the specified calendar.</param>
        /// <param name="yearOfEra">The year of era.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        /// <param name="calendar">Calendar system in which to create the date.</param>
        /// <returns>The resulting date.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid date.</exception>
        public LocalDate([NotNull] Era era, int yearOfEra, int month, int day, [NotNull] CalendarSystem calendar)
            : this(Preconditions.CheckNotNull(calendar, nameof(calendar)).GetAbsoluteYear(yearOfEra, era), month, day, calendar)
        {
        }

        /// <summary>Gets the calendar system associated with this local date.</summary>
        /// <value>The calendar system associated with this local date.</value>
        [NotNull] public CalendarSystem Calendar => CalendarSystem.ForOrdinal(yearMonthDayCalendar.CalendarOrdinal);

        /// <summary>Gets the year of this local date.</summary>
        /// <remarks>This returns the "absolute year", so, for the ISO calendar,
        /// a value of 0 means 1 BC, for example.</remarks>
        /// <value>The year of this local date.</value>
        public int Year => yearMonthDayCalendar.Year;

        /// <summary>Gets the month of this local date within the year.</summary>
        /// <value>The month of this local date within the year.</value>
        public int Month => yearMonthDayCalendar.Month;

        /// <summary>Gets the day of this local date within the month.</summary>
        /// <value>The day of this local date within the month.</value>
        public int Day => yearMonthDayCalendar.Day;

        /// <summary>Gets the number of days since the Unix epoch for this date.</summary>
        /// <value>The number of days since the Unix epoch for this date.</value>
        internal int DaysSinceEpoch => Calendar.GetDaysSinceEpoch(yearMonthDayCalendar.ToYearMonthDay());

        /// <summary>
        /// Gets the week day of this local date expressed as an <see cref="NodaTime.IsoDayOfWeek"/> value.
        /// </summary>
        /// <value>The week day of this local date expressed as an <c>IsoDayOfWeek</c>.</value>
        public IsoDayOfWeek DayOfWeek => Calendar.GetDayOfWeek(yearMonthDayCalendar.ToYearMonthDay());

        /// <summary>Gets the year of this local date within the era.</summary>
        /// <value>The year of this local date within the era.</value>
        public int YearOfEra => Calendar.GetYearOfEra(yearMonthDayCalendar.Year);

        /// <summary>Gets the era of this local date.</summary>
        /// <value>The era of this local date.</value>
        [NotNull] public Era Era => Calendar.GetEra(yearMonthDayCalendar.Year);

        /// <summary>Gets the day of this local date within the year.</summary>
        /// <value>The day of this local date within the year.</value>
        public int DayOfYear => Calendar.GetDayOfYear(yearMonthDayCalendar.ToYearMonthDay());

        internal YearMonthDay YearMonthDay => yearMonthDayCalendar.ToYearMonthDay();

        internal YearMonthDayCalendar YearMonthDayCalendar => yearMonthDayCalendar;

        /// <summary>
        /// Gets a <see cref="LocalDateTime" /> at midnight on the date represented by this local date.
        /// </summary>
        /// <returns>The <see cref="LocalDateTime" /> representing midnight on this local date, in the same calendar
        /// system.</returns>
        [Pure]
        public LocalDateTime AtMidnight() => new LocalDateTime(this, LocalTime.Midnight);

        /// <summary>
        /// Constructs a <see cref="DateTime"/> from this value which has a <see cref="DateTime.Kind" />
        /// of <see cref="DateTimeKind.Unspecified"/>. The result is midnight on the day represented
        /// by this value.
        /// </summary>
        /// <remarks>
        /// <see cref="DateTimeKind.Unspecified"/> is slightly odd - it can be treated as UTC if you use <see cref="DateTime.ToLocalTime"/>
        /// or as system local time if you use <see cref="DateTime.ToUniversalTime"/>, but it's the only kind which allows
        /// you to construct a <see cref="DateTimeOffset"/> with an arbitrary offset, which makes it as close to
        /// the Noda Time non-system-specific "local" concept as exists in .NET.
        /// </remarks>
        /// <returns>A <see cref="DateTime"/> value for the same date and time as this value.</returns>
        [Pure]
        public DateTime ToDateTimeUnspecified() =>
            new DateTime(DaysSinceEpoch * NodaConstants.TicksPerDay + NodaConstants.BclTicksAtUnixEpoch, DateTimeKind.Unspecified);

        // Helper method used by both FromDateTime overloads.
        private static int NonNegativeTicksToDays(long ticks) =>
            unchecked((int) ((ticks >> 14) / 52734375L));

        /// <summary>
        /// Converts a <see cref="DateTime" /> of any kind to a LocalDate in the ISO calendar, ignoring the time of day.
        /// This does not perform any time zone conversions, so a DateTime with a <see cref="DateTime.Kind"/> of
        /// <see cref="DateTimeKind.Utc"/> will still represent the same year/month/day - it won't be converted into the local system time.
        /// </summary>
        /// <param name="dateTime">Value to convert into a Noda Time local date</param>
        /// <returns>A new <see cref="LocalDate"/> with the same values as the specified <c>DateTime</c>.</returns>
        public static LocalDate FromDateTime(DateTime dateTime)
        {
            int days = NonNegativeTicksToDays(dateTime.Ticks) - NodaConstants.BclDaysAtUnixEpoch;
            return new LocalDate(days);
        }

        /// <summary>
        /// Converts a <see cref="DateTime" /> of any kind to a LocalDate in the specified calendar, ignoring the time of day.
        /// This does not perform any time zone conversions, so a DateTime with a <see cref="DateTime.Kind"/> of
        /// <see cref="DateTimeKind.Utc"/> will still represent the same year/month/day - it won't be converted into the local system time.
        /// </summary>
        /// <param name="dateTime">Value to convert into a Noda Time local date</param>
        /// <param name="calendar">The calendar system to convert into</param>
        /// <returns>A new <see cref="LocalDate"/> with the same values as the specified <c>DateTime</c>.</returns>
        public static LocalDate FromDateTime(DateTime dateTime, [NotNull] CalendarSystem calendar)
        {
            int days = NonNegativeTicksToDays(dateTime.Ticks) - NodaConstants.BclDaysAtUnixEpoch;
            return new LocalDate(days, calendar);
        }

        /// <summary>
        /// Returns the local date corresponding to the given "week year", "week of week year", and "day of week"
        /// in the ISO calendar system, using the ISO week-year rules.
        /// </summary>
        /// <param name="weekYear">ISO-8601 week year of value to return</param>
        /// <param name="weekOfWeekYear">ISO-8601 week of week year of value to return</param>
        /// <param name="dayOfWeek">ISO-8601 day of week to return</param>
        /// <returns>The date corresponding to the given week year / week of week year / day of week.</returns>
        public static LocalDate FromWeekYearWeekAndDay(int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek)
            => WeekYearRules.Iso.GetLocalDate(weekYear, weekOfWeekYear, dayOfWeek, CalendarSystem.Iso);

        /// <summary>
        /// Returns the local date corresponding to a particular occurrence of a day-of-week
        /// within a year and month. For example, this method can be used to ask for "the third Monday in April 2012".
        /// </summary>
        /// <remarks>
        /// The returned date is always in the ISO calendar. This method is unrelated to week-years and any rules for
        /// "business weeks" and the like - if a month begins on a Friday, then asking for the first Friday will give
        /// that day, for example.
        /// </remarks>
        /// <param name="year">The year of the value to return.</param>
        /// <param name="month">The month of the value to return.</param>
        /// <param name="occurrence">The occurrence of the value to return, which must be in the range [1, 5]. The value 5 can
        /// be used to always return the last occurrence of the specified day-of-week, even if there are only 4
        /// occurrences of that day-of-week in the month.</param>
        /// <param name="dayOfWeek">The day-of-week of the value to return.</param>
        /// <returns>The date corresponding to the given year and month, on the given occurrence of the
        /// given day of week.</returns>
        public static LocalDate FromYearMonthWeekAndDay(int year, int month, int occurrence, IsoDayOfWeek dayOfWeek)
        {
            // This validates year and month as well as getting us a useful date.
            LocalDate startOfMonth = new LocalDate(year, month, 1);
            Preconditions.CheckArgumentRange(nameof(occurrence), occurrence, 1, 5);
            Preconditions.CheckArgumentRange(nameof(dayOfWeek), (int) dayOfWeek, 1, 7);

            // Correct day of week, 1st week of month.
            int week1Day = dayOfWeek - startOfMonth.DayOfWeek + 1;
            if (week1Day <= 0)
            {
                week1Day += 7;
            }
            int targetDay = week1Day + (occurrence - 1) * 7;
            if (targetDay > CalendarSystem.Iso.GetDaysInMonth(year, month))
            {
                targetDay -= 7;
            }
            return new LocalDate(year, month, targetDay);
        }

        /// <summary>
        /// Adds the specified period to the date.
        /// Fields are added in descending order of significance (years first, then months, and so on).
        /// </summary>
        /// <param name="date">The date to add the period to</param>
        /// <param name="period">The period to add. Must not contain any (non-zero) time units.</param>
        /// <returns>The sum of the given date and period</returns>
        public static LocalDate operator +(LocalDate date, [NotNull] Period period)
        {
            Preconditions.CheckNotNull(period, nameof(period));
            Preconditions.CheckArgument(!period.HasTimeComponent, nameof(period), "Cannot add a period with a time component to a date");
            return period.AddTo(date, 1);
        }

        /// <summary>
        /// Adds the specified period to the date.
        /// Fields are added in descending order of significance (years first, then months, and so on).
        /// Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="date">The date to add the period to</param>
        /// <param name="period">The period to add. Must not contain any (non-zero) time units.</param>
        /// <returns>The sum of the given date and period</returns>
        public static LocalDate Add(LocalDate date, [NotNull] Period period) => date + period;

        /// <summary>
        /// Adds the specified period to this date.
        /// Fields are added in descending order of significance (years first, then months, and so on).
        /// Fluent alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="period">The period to add. Must not contain any (non-zero) time units.</param>
        /// <returns>The sum of this date and the given period</returns>
        [Pure]
        public LocalDate Plus([NotNull] Period period) => this + period;

        /// <summary>
        /// Combines the given <see cref="LocalDate"/> and <see cref="LocalTime"/> components
        /// into a single <see cref="LocalDateTime"/>.
        /// </summary>
        /// <param name="date">The date to add the time to</param>
        /// <param name="time">The time to add</param>
        /// <returns>The sum of the given date and time</returns>
        public static LocalDateTime operator +(LocalDate date, LocalTime time) => new LocalDateTime(date, time);

        /// <summary>
        /// Subtracts the specified period from the date.
        /// Fields are subtracted in descending order of significance (years first, then months, and so on).
        /// This is a convenience operator over the <see cref="Minus(Period)"/> method.
        /// </summary>
        /// <param name="date">The date to subtract the period from</param>
        /// <param name="period">The period to subtract. Must not contain any (non-zero) time units.</param>
        /// <returns>The result of subtracting the given period from the date</returns>
        public static LocalDate operator -(LocalDate date, [NotNull] Period period)
        {
            Preconditions.CheckNotNull(period, nameof(period));
            Preconditions.CheckArgument(!period.HasTimeComponent, nameof(period), "Cannot subtract a period with a time component from a date");
            return period.AddTo(date, -1);
        }

        /// <summary>
        /// Subtracts the specified period from the date.
        /// Fields are subtracted in descending order of significance (years first, then months, and so on).
        /// Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="date">The date to subtract the period from</param>
        /// <param name="period">The period to subtract. Must not contain any (non-zero) time units.</param>
        /// <returns>The result of subtracting the given period from the date.</returns>
        public static LocalDate Subtract(LocalDate date, [NotNull] Period period) => date - period;

        /// <summary>
        /// Subtracts the specified period from this date.
        /// Fields are subtracted in descending order of significance (years first, then months, and so on).
        /// Fluent alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="period">The period to subtract. Must not contain any (non-zero) time units.</param>
        /// <returns>The result of subtracting the given period from this date.</returns>
        [Pure]
        public LocalDate Minus([NotNull] Period period) => this - period;

        /// <summary>
        /// Subtracts one date from another, returning the result as a <see cref="Period"/> with units of years, months and days.
        /// </summary>
        /// <remarks>
        /// This is simply a convenience operator for calling <see cref="Period.Between(NodaTime.LocalDate,NodaTime.LocalDate)"/>.
        /// The calendar systems of the two dates must be the same; an exception will be thrown otherwise.
        /// </remarks>
        /// <param name="lhs">The date to subtract from</param>
        /// <param name="rhs">The date to subtract</param>
        /// <returns>The result of subtracting one date from another.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="lhs"/> and <paramref name="rhs"/> are not in the same calendar system.
        /// </exception>
        [NotNull] public static Period operator -(LocalDate lhs, LocalDate rhs) => Period.Between(rhs, lhs);

        /// <summary>
        /// Subtracts one date from another, returning the result as a <see cref="Period"/> with units of years, months and days.
        /// </summary>
        /// <remarks>
        /// This is simply a convenience method for calling <see cref="Period.Between(NodaTime.LocalDate,NodaTime.LocalDate)"/>.
        /// The calendar systems of the two dates must be the same.
        /// </remarks>
        /// <param name="lhs">The date to subtract from</param>
        /// <param name="rhs">The date to subtract</param>
        /// <returns>The result of subtracting one date from another.</returns>
        [NotNull] public static Period Subtract(LocalDate lhs, LocalDate rhs) => lhs - rhs;

        /// <summary>
        /// Subtracts the specified date from this date, returning the result as a <see cref="Period"/> with units of years, months and days.
        /// Fluent alternative to <c>operator-()</c>.
        /// </summary>
        /// <remarks>The specified date must be in the same calendar system as this.</remarks>
        /// <param name="date">The date to subtract from this</param>
        /// <returns>The difference between the specified date and this one</returns>
        [Pure] [NotNull] public Period Minus(LocalDate date) => this - date;

        /// <summary>
        /// Compares two <see cref="LocalDate" /> values for equality. This requires
        /// that the dates be the same, within the same calendar.
        /// </summary>
        /// <param name="lhs">The first value to compare</param>
        /// <param name="rhs">The second value to compare</param>
        /// <returns>True if the two dates are the same and in the same calendar; false otherwise</returns>
        public static bool operator ==(LocalDate lhs, LocalDate rhs) => lhs.yearMonthDayCalendar == rhs.yearMonthDayCalendar;

        /// <summary>
        /// Compares two <see cref="LocalDate" /> values for inequality.
        /// </summary>
        /// <param name="lhs">The first value to compare</param>
        /// <param name="rhs">The second value to compare</param>
        /// <returns>False if the two dates are the same and in the same calendar; true otherwise</returns>
        public static bool operator !=(LocalDate lhs, LocalDate rhs) => !(lhs == rhs);

        // Comparison operators: note that we can't use YearMonthDayCalendar.Compare, as only the calendar knows whether it can use
        // naive comparisons.

        /// <summary>
        /// Compares two dates to see if the left one is strictly earlier than the right
        /// one.
        /// </summary>
        /// <remarks>
        /// Only dates with the same calendar system can be compared. See the top-level type
        /// documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <exception cref="ArgumentException">The calendar system of <paramref name="rhs"/> is not the same
        /// as the calendar of <paramref name="lhs"/>.</exception>
        /// <returns>true if the <paramref name="lhs"/> is strictly earlier than <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator <(LocalDate lhs, LocalDate rhs)
        {
            Preconditions.CheckArgument(lhs.Calendar.Equals(rhs.Calendar), nameof(rhs), "Only values in the same calendar can be compared");
            return lhs.CompareTo(rhs) < 0;
        }

        /// <summary>
        /// Compares two dates to see if the left one is earlier than or equal to the right
        /// one.
        /// </summary>
        /// <remarks>
        /// Only dates with the same calendar system can be compared. See the top-level type
        /// documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <exception cref="ArgumentException">The calendar system of <paramref name="rhs"/> is not the same
        /// as the calendar of <paramref name="lhs"/>.</exception>
        /// <returns>true if the <paramref name="lhs"/> is earlier than or equal to <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator <=(LocalDate lhs, LocalDate rhs)
        {
            Preconditions.CheckArgument(lhs.Calendar.Equals(rhs.Calendar), nameof(rhs), "Only values in the same calendar can be compared");
            return lhs.CompareTo(rhs) <= 0;
        }

        /// <summary>
        /// Compares two dates to see if the left one is strictly later than the right
        /// one.
        /// </summary>
        /// <remarks>
        /// Only dates with the same calendar system can be compared. See the top-level type
        /// documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <exception cref="ArgumentException">The calendar system of <paramref name="rhs"/> is not the same
        /// as the calendar of <paramref name="lhs"/>.</exception>
        /// <returns>true if the <paramref name="lhs"/> is strictly later than <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator >(LocalDate lhs, LocalDate rhs)
        {
            Preconditions.CheckArgument(lhs.Calendar.Equals(rhs.Calendar), nameof(rhs), "Only values in the same calendar can be compared");
            return lhs.CompareTo(rhs) > 0;
        }

        /// <summary>
        /// Compares two dates to see if the left one is later than or equal to the right
        /// one.
        /// </summary>
        /// <remarks>
        /// Only dates with the same calendar system can be compared. See the top-level type
        /// documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <exception cref="ArgumentException">The calendar system of <paramref name="rhs"/> is not the same
        /// as the calendar of <paramref name="lhs"/>.</exception>
        /// <returns>true if the <paramref name="lhs"/> is later than or equal to <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator >=(LocalDate lhs, LocalDate rhs)
        {
            Preconditions.CheckArgument(lhs.Calendar.Equals(rhs.Calendar), nameof(rhs), "Only values in the same calendar can be compared");
            return lhs.CompareTo(rhs) >= 0;
        }

        /// <summary>
        /// Indicates whether this date is earlier, later or the same as another one.
        /// </summary>
        /// <remarks>
        /// Only dates within the same calendar systems can be compared with this method. Attempting to compare
        /// dates within different calendars will fail with an <see cref="ArgumentException"/>. Ideally, comparisons
        /// between values in different calendars would be a compile-time failure, but failing at execution time
        /// is almost always preferable to continuing.
        /// </remarks>
        /// <param name="other">The other date to compare this one with</param>
        /// <exception cref="ArgumentException">The calendar system of <paramref name="other"/> is not the
        /// same as the calendar system of this value.</exception>
        /// <returns>A value less than zero if this date is earlier than <paramref name="other"/>;
        /// zero if this date is the same as <paramref name="other"/>; a value greater than zero if this date is
        /// later than <paramref name="other"/>.</returns>
        public int CompareTo(LocalDate other)
        {
            Preconditions.CheckArgument(Calendar.Equals(other.Calendar), nameof(other), "Only values with the same calendar system can be compared");
            return Calendar.Compare(YearMonthDay, other.YearMonthDay);
        }

        /// <summary>
        /// Implementation of <see cref="IComparable.CompareTo"/> to compare two LocalDates.
        /// </summary>
        /// <remarks>
        /// This uses explicit interface implementation to avoid it being called accidentally. The generic implementation should usually be preferred.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is non-null but does not refer to an instance of <see cref="LocalDate"/>, or refers
        /// to a date in a different calendar system.</exception>
        /// <param name="obj">The object to compare this value with.</param>
        /// <returns>The result of comparing this LocalDate with another one; see <see cref="CompareTo(NodaTime.LocalDate)"/> for general details.
        /// If <paramref name="obj"/> is null, this method returns a value greater than 0.
        /// </returns>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            Preconditions.CheckArgument(obj is LocalDate, nameof(obj), "Object must be of type NodaTime.LocalDate.");
            return CompareTo((LocalDate)obj);
        }

        /// <summary>
        /// Returns the later date of the given two.
        /// </summary>
        /// <param name="x">The first date to compare.</param>
        /// <param name="y">The second date to compare.</param>
        /// <exception cref="ArgumentException">The two dates have different calendar systems.</exception>
        /// <returns>The later date of <paramref name="x"/> or <paramref name="y"/>.</returns>
        public static LocalDate Max(LocalDate x, LocalDate y)
        {
            Preconditions.CheckArgument(x.Calendar.Equals(y.Calendar), nameof(y), "Only values with the same calendar system can be compared");
            return x > y ? x : y;
        }

        /// <summary>
        /// Returns the earlier date of the given two.
        /// </summary>
        /// <param name="x">The first date to compare.</param>
        /// <param name="y">The second date to compare.</param>
        /// <exception cref="ArgumentException">The two dates have different calendar systems.</exception>
        /// <returns>The earlier date of <paramref name="x"/> or <paramref name="y"/>.</returns>
        public static LocalDate Min(LocalDate x, LocalDate y)
        {
            Preconditions.CheckArgument(x.Calendar.Equals(y.Calendar), nameof(y), "Only values with the same calendar system can be compared");
            return x < y ? x : y;
        }

        /// <summary>
        /// Returns a hash code for this local date.
        /// </summary>
        /// <returns>A hash code for this local date.</returns>
        public override int GetHashCode() => yearMonthDayCalendar.GetHashCode();

        /// <summary>
        /// Compares two <see cref="LocalDate"/> values for equality. This requires
        /// that the dates be the same, within the same calendar.
        /// </summary>
        /// <param name="obj">The object to compare this date with.</param>
        /// <returns>True if the given value is another local date equal to this one; false otherwise.</returns>
        public override bool Equals(object obj) => obj is LocalDate && this == (LocalDate)obj;

        /// <summary>
        /// Compares two <see cref="LocalDate"/> values for equality. This requires
        /// that the dates be the same, within the same calendar.
        /// </summary>
        /// <param name="other">The value to compare this date with.</param>
        /// <returns>True if the given value is another local date equal to this one; false otherwise.</returns>
        public bool Equals(LocalDate other) => this == other;

        /// <summary>
        /// Resolves this local date into a <see cref="ZonedDateTime"/> in the given time zone representing the
        /// start of this date in the given zone.
        /// </summary>
        /// <remarks>
        /// This is a convenience method for calling <see cref="DateTimeZone.AtStartOfDay(LocalDate)"/>.
        /// </remarks>
        /// <param name="zone">The time zone to map this local date into</param>
        /// <exception cref="SkippedTimeException">The entire day was skipped due to a very large time zone transition.
        /// (This is extremely rare.)</exception>
        /// <returns>The <see cref="ZonedDateTime"/> representing the earliest time on this date, in the given time zone.</returns>
        [Pure]
        public ZonedDateTime AtStartOfDayInZone([NotNull] DateTimeZone zone)
        {
            Preconditions.CheckNotNull(zone, nameof(zone));
            return zone.AtStartOfDay(this);
        }
        
        /// <summary>
        /// Creates a new LocalDate representing the same physical date, but in a different calendar.
        /// The returned LocalDate is likely to have different field values to this one.
        /// For example, January 1st 1970 in the Gregorian calendar was December 19th 1969 in the Julian calendar.
        /// </summary>
        /// <param name="calendar">The calendar system to convert this local date to.</param>
        /// <returns>The converted LocalDate</returns>
        [Pure]
        public LocalDate WithCalendar([NotNull] CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, nameof(calendar));
            return new LocalDate(DaysSinceEpoch, calendar);
        }

        /// <summary>
        /// Returns a new LocalDate representing the current value with the given number of years added.
        /// </summary>
        /// <remarks>
        /// If the resulting date is invalid, lower fields (typically the day of month) are reduced to find a valid value.
        /// For example, adding one year to February 29th 2012 will return February 28th 2013; subtracting one year from
        /// February 29th 2012 will return February 28th 2011.
        /// </remarks>
        /// <param name="years">The number of years to add</param>
        /// <returns>The current value plus the given number of years.</returns>
        [Pure]
        public LocalDate PlusYears(int years) => DatePeriodFields.YearsField.Add(this, years);

        /// <summary>
        /// Returns a new LocalDate representing the current value with the given number of months added.
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
        /// <returns>The current date plus the given number of months</returns>
        [Pure]
        public LocalDate PlusMonths(int months) => DatePeriodFields.MonthsField.Add(this, months);

        /// <summary>
        /// Returns a new LocalDate representing the current value with the given number of days added.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method does not try to maintain the month or year of the current value, so adding 3 days to a value of January 30th
        /// will result in a value of February 2nd.
        /// </para>
        /// </remarks>
        /// <param name="days">The number of days to add</param>
        /// <returns>The current value plus the given number of days.</returns>
        [Pure]
        public LocalDate PlusDays(int days) => DatePeriodFields.DaysField.Add(this, days);

        /// <summary>
        /// Returns a new LocalDate representing the current value with the given number of weeks added.
        /// </summary>
        /// <param name="weeks">The number of weeks to add</param>
        /// <returns>The current value plus the given number of weeks.</returns>
        [Pure]
        public LocalDate PlusWeeks(int weeks) => DatePeriodFields.WeeksField.Add(this, weeks);

        /// <summary>
        /// Returns the next <see cref="LocalDate" /> falling on the specified <see cref="IsoDayOfWeek"/>.
        /// This is a strict "next" - if this date on already falls on the target
        /// day of the week, the returned value will be a week later.
        /// </summary>
        /// <param name="targetDayOfWeek">The ISO day of the week to return the next date of.</param>
        /// <returns>The next <see cref="LocalDate"/> falling on the specified day of the week.</returns>
        /// <exception cref="InvalidOperationException">The underlying calendar doesn't use ISO days of the week.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="targetDayOfWeek"/> is not a valid day of the
        /// week (Monday to Sunday).</exception>
        [Pure]
        public LocalDate Next(IsoDayOfWeek targetDayOfWeek)
        {
            // Avoids boxing...
            if (targetDayOfWeek < IsoDayOfWeek.Monday || targetDayOfWeek > IsoDayOfWeek.Sunday)
            {
                throw new ArgumentOutOfRangeException(nameof(targetDayOfWeek));
            }
            // This will throw the desired exception for calendars with different week systems.
            IsoDayOfWeek thisDay = DayOfWeek;
            int difference = targetDayOfWeek - thisDay;
            if (difference <= 0)
            {
                difference += 7;
            }
            return PlusDays(difference);
        }

        /// <summary>
        /// Returns the previous <see cref="LocalDate" /> falling on the specified <see cref="IsoDayOfWeek"/>.
        /// This is a strict "previous" - if this date on already falls on the target
        /// day of the week, the returned value will be a week earlier.
        /// </summary>
        /// <param name="targetDayOfWeek">The ISO day of the week to return the previous date of.</param>
        /// <returns>The previous <see cref="LocalDate"/> falling on the specified day of the week.</returns>
        /// <exception cref="InvalidOperationException">The underlying calendar doesn't use ISO days of the week.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="targetDayOfWeek"/> is not a valid day of the
        /// week (Monday to Sunday).</exception>
        [Pure]
        public LocalDate Previous(IsoDayOfWeek targetDayOfWeek)
        {
            // Avoids boxing...
            if (targetDayOfWeek < IsoDayOfWeek.Monday || targetDayOfWeek > IsoDayOfWeek.Sunday)
            {
                throw new ArgumentOutOfRangeException(nameof(targetDayOfWeek));
            }
            // This will throw the desired exception for calendars with different week systems.
            IsoDayOfWeek thisDay = DayOfWeek;
            int difference = targetDayOfWeek - thisDay;
            if (difference >= 0)
            {
                difference -= 7;
            }
            return PlusDays(difference);
        }

        /// <summary>
        /// Returns an <see cref="OffsetDate"/> for this local date with the given offset.
        /// </summary>
        /// <remarks>This method is purely a convenient alternative to calling the <see cref="OffsetDate"/> constructor directly.</remarks>
        /// <param name="offset">The offset to apply.</param>
        /// <returns>The result of this date offset by the given amount.</returns>
        [Pure]
        public OffsetDate WithOffset(Offset offset) => new OffsetDate(this, offset);

        /// <summary>
        /// Combines this <see cref="LocalDate"/> with the given <see cref="LocalTime"/>
        /// into a single <see cref="LocalDateTime"/>.
        /// Fluent alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="time">The time to combine with this date.</param>
        /// <returns>The <see cref="LocalDateTime"/> representation of the given time on this date</returns>
        [Pure]
        public LocalDateTime At(LocalTime time) => this + time;

        /// <summary>
        /// Returns this date, with the given adjuster applied to it.
        /// </summary>
        /// <remarks>
        /// If the adjuster attempts to construct an invalid date (such as by trying
        /// to set a day-of-month of 30 in February), any exception thrown by
        /// that construction attempt will be propagated through this method.
        /// </remarks>
        /// <param name="adjuster">The adjuster to apply.</param>
        /// <returns>The adjusted date.</returns>
        [Pure]
        public LocalDate With([NotNull] Func<LocalDate, LocalDate> adjuster) =>
            Preconditions.CheckNotNull(adjuster, nameof(adjuster)).Invoke(this);

        /// <summary>
        /// Deconstructs the current instance into its components.
        /// </summary>
        /// <param name="year">The year component of the date.</param>
        /// <param name="month">The month component of the date.</param>
        /// <param name="day">The day component of the date.</param>
        [Pure]
        public void Deconstruct(out int year, out int month, out int day)
        {
            year = Year;
            month = Month;
            day = Day;
        }

        /// <summary>
        /// Deconstructs the current instance into its components.
        /// </summary>
        /// <param name="year">The year component of the date.</param>
        /// <param name="month">The month component of the date.</param>
        /// <param name="day">The day component of the date.</param>
        /// <param name="calendar">The <see cref="CalendarSystem"/> associated with the date.</param>
        [Pure]
        public void Deconstruct(out int year, out int month, out int day, [NotNull]out CalendarSystem calendar)
        {
            year = Year;
            month = Month;
            day = Day;
            calendar = Calendar;
        }

        #region Formatting
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// The value of the current instance in the default format pattern ("D"), using the current thread's
        /// culture to obtain a format provider.
        /// </returns>
        public override string ToString() => LocalDatePattern.BclSupport.Format(this, null, CultureInfo.CurrentCulture);

        /// <summary>
        /// Formats the value of the current instance using the specified pattern.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="patternText">The <see cref="T:System.String" /> specifying the pattern to use,
        /// or null to use the default format pattern ("D").
        /// </param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when formatting the value,
        /// or null to use the current thread's culture to obtain a format provider.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string patternText, IFormatProvider formatProvider) =>
            LocalDatePattern.BclSupport.Format(this, patternText, formatProvider);
        #endregion Formatting

        #region XML serialization
        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema() => null;

        /// <inheritdoc />
        void IXmlSerializable.ReadXml([NotNull] XmlReader reader)
        {
            Preconditions.CheckNotNull(reader, nameof(reader));
            var pattern = LocalDatePattern.Iso;
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
        void IXmlSerializable.WriteXml([NotNull] XmlWriter writer)
        {
            Preconditions.CheckNotNull(writer, nameof(writer));
            if (Calendar != CalendarSystem.Iso)
            {
                writer.WriteAttributeString("calendar", Calendar.Id);
            }
            writer.WriteString(LocalDatePattern.Iso.Format(this));
        }
        #endregion

#if !NETSTANDARD
        #region Binary serialization
        /// <summary>
        /// Private constructor only present for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        /// <param name="context">The source for this deserialization.</param>
        private LocalDate([NotNull] SerializationInfo info, StreamingContext context)
            : this(info)
        {
        }

        /// <summary>
        /// Constructor only present for serialization; internal to allow construction from LocalDateTime
        /// as part of deserialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        internal LocalDate([NotNull] SerializationInfo info)
        {
            Preconditions.CheckNotNull(info, nameof(info));
            int year = info.GetInt32(BinaryFormattingConstants.YearSerializationName);
            int month = info.GetInt32(BinaryFormattingConstants.MonthSerializationName);
            int day = info.GetInt32(BinaryFormattingConstants.DaySerializationName);
            CalendarOrdinal ordinal = (CalendarOrdinal) info.GetInt32(BinaryFormattingConstants.CalendarSerializationName);
            try
            {
                Preconditions.CheckArgument(ordinal >= 0 && ordinal < CalendarOrdinal.Size, nameof(ordinal), "Calendar ordinal out of range");
                var calendar = CalendarSystem.ForOrdinal(ordinal);
                calendar.ValidateYearMonthDay(year, month, day);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalid serialized data, details in InnerException", nameof(info), e);
            }
            yearMonthDayCalendar = new YearMonthDayCalendar(year, month, day, ordinal);
        }

        /// <summary>
        /// Implementation of <see cref="ISerializable.GetObjectData"/>.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination for this serialization.</param>
        [System.Security.SecurityCritical]
        void ISerializable.GetObjectData([NotNull] SerializationInfo info, StreamingContext context)
        {
            Serialize(info);
        }

        internal void Serialize([NotNull] SerializationInfo info)
        {
            Preconditions.CheckNotNull(info, nameof(info));
            info.AddValue(BinaryFormattingConstants.YearSerializationName, Year);
            info.AddValue(BinaryFormattingConstants.MonthSerializationName, Month);
            info.AddValue(BinaryFormattingConstants.DaySerializationName, Day);
            info.AddValue(BinaryFormattingConstants.CalendarSerializationName, yearMonthDayCalendar.CalendarOrdinal);
        }
        #endregion
#endif
    }
}
