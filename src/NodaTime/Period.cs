// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using static NodaTime.NodaConstants;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Fields;
using NodaTime.Text;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// Represents a period of time expressed in human chronological terms: hours, days,
    /// weeks, months and so on.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <see cref="Period"/> contains a set of properties such as <see cref="Years"/>, <see cref="Months"/>, and so on
    /// that return the number of each unit contained within this period. Note that these properties are not normalized in
    /// any way by default, and so a <see cref="Period"/> may contain values such as "2 hours and 90 minutes". The
    /// <see cref="Normalize"/> method will convert equivalent periods into a standard representation.
    /// </para>
    /// <para>
    /// Periods can contain negative units as well as positive units ("+2 hours, -43 minutes, +10 seconds"), but do not
    /// differentiate between properties that are zero and those that are absent (i.e. a period created as "10 years"
    /// and one created as "10 years, zero months" are equal periods; the <see cref="Months"/> property returns zero in
    /// both cases).
    /// </para>
    /// <para>
    /// <see cref="Period"/> equality is implemented by comparing each property's values individually.
    /// </para>
    /// <para>
    /// Periods operate on calendar-related types such as
    /// <see cref="LocalDateTime" /> whereas <see cref="Duration"/> operates on instants
    /// on the time line. (Note that although <see cref="ZonedDateTime" /> includes both concepts, it only supports
    /// duration-based arithmetic.)
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
#if !NETSTANDARD1_3
    [Serializable]
#endif
    [Immutable]
    public sealed class Period : IEquatable<Period>
#if !NETSTANDARD1_3
        , ISerializable
#endif
    {
        // General implementation note: operations such as normalization work out the total number of nanoseconds as an Int64
        // value. This can handle +/- 106,751 days, or 292 years. We could move to using BigInteger if we feel that's required,
        // but it's unlikely to be an issue. Ideally, we'd switch to use BigInteger after detecting that it could be a problem,
        // but without the hit of having to catch the exception...

        /// <summary>
        /// A period containing only zero-valued properties.
        /// </summary>
        /// <value>A period containing only zero-valued properties.</value>
        [NotNull] public static Period Zero { get; } = new Period(0, 0, 0, 0);

        /// <summary>
        /// Returns an equality comparer which compares periods by first normalizing them - so 24 hours is deemed equal to 1 day, and so on.
        /// Note that as per the <see cref="Normalize"/> method, years and months are unchanged by normalization - so 12 months does not
        /// equal 1 year.
        /// </summary>
        /// <value>An equality comparer which compares periods by first normalizing them.</value>
        [NotNull] public static IEqualityComparer<Period> NormalizingEqualityComparer => NormalizingPeriodEqualityComparer.Instance;

        // The fields that make up this period.

        /// <summary>
        /// Gets the number of nanoseconds within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        /// <value>The number of nanoseconds within this period.</value>
        public long Nanoseconds { get; }

        /// <summary>
        /// Gets the number of ticks within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        /// <value>The number of ticks within this period.</value>
        public long Ticks { get; }

        /// <summary>
        /// Gets the number of milliseconds within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        /// <value>The number of milliseconds within this period.</value>
        public long Milliseconds { get; }

        /// <summary>
        /// Gets the number of seconds within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        /// <value>The number of seconds within this period.</value>
        public long Seconds { get; }

        /// <summary>
        /// Gets the number of minutes within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        /// <value>The number of minutes within this period.</value>
        public long Minutes { get; }
        
        /// <summary>
        /// Gets the number of hours within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        /// <value>The number of hours within this period.</value>
        public long Hours { get; }

        /// <summary>
        /// Gets the number of days within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        /// <value>The number of days within this period.</value>
        public int Days { get; }

        /// <summary>
        /// Gets the number of weeks within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        /// <value>The number of weeks within this period.</value>
        public int Weeks { get; }

        /// <summary>
        /// Gets the number of months within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        /// <value>The number of months within this period.</value>
        public int Months { get; }

        /// <summary>
        /// Gets the number of years within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        /// <value>The number of years within this period.</value>
        public int Years { get; }

        /// <summary>
        /// Creates a period with the given date values.
        /// </summary>
        private Period(int years, int months, int weeks, int days)
        {
            this.Years = years;
            this.Months = months;
            this.Weeks = weeks;
            this.Days = days;
        }

        /// <summary>
        /// Creates a period with the given time values.
        /// </summary>
        private Period(long hours, long minutes, long seconds, long milliseconds, long ticks, long nanoseconds)
        {
            this.Hours = hours;
            this.Minutes = minutes;
            this.Seconds = seconds;
            this.Milliseconds = milliseconds;
            this.Ticks = ticks;
            this.Nanoseconds = nanoseconds;
        }

        /// <summary>
        /// Creates a new period from the given values.
        /// </summary>
        internal Period(int years, int months, int weeks, int days, long hours, long minutes, long seconds,
            long milliseconds, long ticks, long nanoseconds)
        {
            this.Years = years;
            this.Months = months;
            this.Weeks = weeks;
            this.Days = days;
            this.Hours = hours;
            this.Minutes = minutes;
            this.Seconds = seconds;
            this.Milliseconds = milliseconds;
            this.Ticks = ticks;
            this.Nanoseconds = nanoseconds;
        }

        /// <summary>
        /// Creates a period representing the specified number of years.
        /// </summary>
        /// <param name="years">The number of years in the new period</param>
        /// <returns>A period consisting of the given number of years.</returns>
        [NotNull]
        public static Period FromYears(int years) => new Period(years, 0, 0, 0);

        /// <summary>
        /// Creates a period representing the specified number of months.
        /// </summary>
        /// <param name="months">The number of months in the new period</param>
        /// <returns>A period consisting of the given number of months.</returns>
        [NotNull]
        public static Period FromMonths(int months) => new Period(0, months, 0, 0);

        /// <summary>
        /// Creates a period representing the specified number of weeks.
        /// </summary>
        /// <param name="weeks">The number of weeks in the new period</param>
        /// <returns>A period consisting of the given number of weeks.</returns>
        [NotNull]
        public static Period FromWeeks(int weeks) => new Period(0, 0, weeks, 0);

        /// <summary>
        /// Creates a period representing the specified number of days.
        /// </summary>
        /// <param name="days">The number of days in the new period</param>
        /// <returns>A period consisting of the given number of days.</returns>
        [NotNull]
        public static Period FromDays(int days) => new Period(0, 0, 0, days);

        /// <summary>
        /// Creates a period representing the specified number of hours.
        /// </summary>
        /// <param name="hours">The number of hours in the new period</param>
        /// <returns>A period consisting of the given number of hours.</returns>
        [NotNull]
        public static Period FromHours(long hours) => new Period(hours, 0L, 0L, 0L, 0L, 0L);

        /// <summary>
        /// Creates a period representing the specified number of minutes.
        /// </summary>
        /// <param name="minutes">The number of minutes in the new period</param>
        /// <returns>A period consisting of the given number of minutes.</returns>
        [NotNull]
        public static Period FromMinutes(long minutes) => new Period(0L, minutes, 0L, 0L, 0L, 0L);

        /// <summary>
        /// Creates a period representing the specified number of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds in the new period</param>
        /// <returns>A period consisting of the given number of seconds.</returns>
        [NotNull]
        public static Period FromSeconds(long seconds) => new Period(0L, 0L, seconds, 0L, 0L, 0L);

        /// <summary>
        /// Creates a period representing the specified number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds in the new period</param>
        /// <returns>A period consisting of the given number of milliseconds.</returns>
        [NotNull]
        public static Period FromMilliseconds(long milliseconds) => new Period(0L, 0L, 0L, milliseconds, 0L, 0L);

        /// <summary>
        /// Creates a period representing the specified number of ticks.
        /// </summary>
        /// <param name="ticks">The number of ticks in the new period</param>
        /// <returns>A period consisting of the given number of ticks.</returns>
        [NotNull]
        public static Period FromTicks(long ticks) => new Period(0L, 0L, 0L, 0L, ticks, 0L);

        /// <summary>
        /// Creates a period representing the specified number of nanooseconds.
        /// </summary>
        /// <param name="nanoseconds">The number of nanoseconds in the new period</param>
        /// <returns>A period consisting of the given number of nanoseconds.</returns>
        [NotNull]
        public static Period FromNanoseconds(long nanoseconds) => new Period(0L, 0L, 0L, 0L, 0L, nanoseconds);

        /// <summary>
        /// Adds two periods together, by simply adding the values for each property.
        /// </summary>
        /// <param name="left">The first period to add</param>
        /// <param name="right">The second period to add</param>
        /// <returns>The sum of the two periods. The units of the result will be the union of those in both
        /// periods.</returns>
        [NotNull]
        public static Period operator +([NotNull] Period left, [NotNull] Period right)
        {
            Preconditions.CheckNotNull(left, nameof(left));
            Preconditions.CheckNotNull(right, nameof(right));
            return new Period(
                left.Years + right.Years,
                left.Months + right.Months,
                left.Weeks + right.Weeks,
                left.Days + right.Days,
                left.Hours + right.Hours,
                left.Minutes + right.Minutes,
                left.Seconds + right.Seconds,
                left.Milliseconds + right.Milliseconds,
                left.Ticks + right.Ticks,
                left.Nanoseconds + right.Nanoseconds);
        }

        /// <summary>
        /// Creates an <see cref="IComparer{T}"/> for periods, using the given "base" local date/time.
        /// </summary>
        /// <remarks>
        /// Certain periods can't naturally be compared without more context - how "one month" compares to
        /// "30 days" depends on where you start. In order to compare two periods, the returned comparer
        /// effectively adds both periods to the "base" specified by <paramref name="baseDateTime"/> and compares
        /// the results. In some cases this arithmetic isn't actually required - when two periods can be
        /// converted to durations, the comparer uses that conversion for efficiency.
        /// </remarks>
        /// <param name="baseDateTime">The base local date/time to use for comparisons.</param>
        /// <returns>The new comparer.</returns>
        [NotNull]
        public static IComparer<Period> CreateComparer(LocalDateTime baseDateTime) => new PeriodComparer(baseDateTime);

        /// <summary>
        /// Subtracts one period from another, by simply subtracting each property value.
        /// </summary>
        /// <param name="minuend">The period to subtract the second operand from</param>
        /// <param name="subtrahend">The period to subtract the first operand from</param>
        /// <returns>The result of subtracting all the values in the second operand from the values in the first. The
        /// units of the result will be the union of both periods, even if the subtraction caused some properties to
        /// become zero (so "2 weeks, 1 days" minus "2 weeks" is "zero weeks, 1 days", not "1 days").</returns>
        [NotNull]
        public static Period operator -([NotNull] Period minuend, [NotNull] Period subtrahend)
        {
            Preconditions.CheckNotNull(minuend, nameof(minuend));
            Preconditions.CheckNotNull(subtrahend, nameof(subtrahend));
            return new Period(
                minuend.Years - subtrahend.Years,
                minuend.Months - subtrahend.Months,
                minuend.Weeks - subtrahend.Weeks,
                minuend.Days - subtrahend.Days,
                minuend.Hours - subtrahend.Hours,
                minuend.Minutes - subtrahend.Minutes,
                minuend.Seconds - subtrahend.Seconds,
                minuend.Milliseconds - subtrahend.Milliseconds,
                minuend.Ticks - subtrahend.Ticks,
                minuend.Nanoseconds - subtrahend.Nanoseconds);
        }

        /// <summary>
        /// Returns the period between a start and an end date/time, using only the given units.
        /// </summary>
        /// <remarks>
        /// If <paramref name="end"/> is before <paramref name="start" />, each property in the returned period
        /// will be negative. If the given set of units cannot exactly reach the end point (e.g. finding
        /// the difference between 1am and 3:15am in hours) the result will be such that adding it to <paramref name="start"/>
        /// will give a value between <paramref name="start"/> and <paramref name="end"/>. In other words,
        /// any rounding is "towards start"; this is true whether the resulting period is negative or positive.
        /// </remarks>
        /// <param name="start">Start date/time</param>
        /// <param name="end">End date/time</param>
        /// <param name="units">Units to use for calculations</param>
        /// <exception cref="ArgumentException"><paramref name="units"/> is empty or contained unknown values.</exception>
        /// <exception cref="ArgumentException"><paramref name="start"/> and <paramref name="end"/> use different calendars.</exception>
        /// <returns>The period between the given date/times, using the given units.</returns>
        [NotNull]
        public static Period Between(LocalDateTime start, LocalDateTime end, PeriodUnits units)
        {
            Preconditions.CheckArgument(units != 0, nameof(units), "Units must not be empty");
            Preconditions.CheckArgument((units & ~PeriodUnits.AllUnits) == 0, nameof(units), "Units contains an unknown value: {0}", units);
            CalendarSystem calendar = start.Calendar;
            Preconditions.CheckArgument(calendar.Equals(end.Calendar), nameof(end), "start and end must use the same calendar system");

            if (start == end)
            {
                return Zero;
            }

            // Adjust for situations like "days between 5th January 10am and 7th Janary 5am" which should be one
            // day, because if we actually reach 7th January with date fields, we've overshot.
            // The date adjustment will always be valid, because it's just moving it towards start.
            // We need this for all date-based period fields. We could potentially optimize by not doing this
            // in cases where we've only got time fields...
            LocalDate endDate = end.Date;
            if (start < end)
            {
                if (start.TimeOfDay > end.TimeOfDay)
                {
                    endDate = endDate.PlusDays(-1);
                }
            }
            else if (start > end && start.TimeOfDay < end.TimeOfDay)
            {
                endDate = endDate.PlusDays(1);
            }

            // Optimization for single field
            switch (units)
            {
                case PeriodUnits.Years: return FromYears(DatePeriodFields.YearsField.UnitsBetween(start.Date, endDate));
                case PeriodUnits.Months: return FromMonths(DatePeriodFields.MonthsField.UnitsBetween(start.Date, endDate));
                case PeriodUnits.Weeks: return FromWeeks(DatePeriodFields.WeeksField.UnitsBetween(start.Date, endDate));
                case PeriodUnits.Days: return FromDays(DaysBetween(start.Date, endDate));
                case PeriodUnits.Hours: return FromHours(TimePeriodField.Hours.UnitsBetween(start, end));
                case PeriodUnits.Minutes: return FromMinutes(TimePeriodField.Minutes.UnitsBetween(start, end));
                case PeriodUnits.Seconds: return FromSeconds(TimePeriodField.Seconds.UnitsBetween(start, end));
                case PeriodUnits.Milliseconds: return FromMilliseconds(TimePeriodField.Milliseconds.UnitsBetween(start, end));
                case PeriodUnits.Ticks: return FromTicks(TimePeriodField.Ticks.UnitsBetween(start, end));
                case PeriodUnits.Nanoseconds: return FromNanoseconds(TimePeriodField.Nanoseconds.UnitsBetween(start, end));
            }

            // Multiple fields
            LocalDateTime remaining = start;
            int years = 0, months = 0, weeks = 0, days = 0;
            if ((units & PeriodUnits.AllDateUnits) != 0)
            {
                LocalDate remainingDate = DateComponentsBetween(
                    start.Date, endDate, units, out years, out months, out weeks, out days);
                remaining = new LocalDateTime(remainingDate, start.TimeOfDay);
            }
            if ((units & PeriodUnits.AllTimeUnits) == 0)
            {
                return new Period(years, months, weeks, days);
            }

            // The remainder of the computation is with fixed-length units, so we can do it all with
            // Duration instead of Local* values. We don't know for sure that this is small though - we *could*
            // be trying to find the difference between 9998 BC and 9999 CE in nanoseconds...
            // Where we can optimize, do everything with long arithmetic (as we do for Between(LocalTime, LocalTime)).
            // Otherwise (rare case), use duration arithmetic.
            long hours, minutes, seconds, milliseconds, ticks, nanoseconds;
            var duration = end.ToLocalInstant().TimeSinceLocalEpoch - remaining.ToLocalInstant().TimeSinceLocalEpoch;
            if (duration.IsInt64Representable)
            {
                TimeComponentsBetween(duration.ToInt64Nanoseconds(), units, out hours, out minutes, out seconds, out milliseconds, out ticks, out nanoseconds);
            }
            else
            {
                hours = UnitsBetween(PeriodUnits.Hours, TimePeriodField.Hours);
                minutes = UnitsBetween(PeriodUnits.Minutes, TimePeriodField.Minutes);
                seconds = UnitsBetween(PeriodUnits.Seconds, TimePeriodField.Seconds);
                milliseconds = UnitsBetween(PeriodUnits.Milliseconds, TimePeriodField.Milliseconds);
                ticks = UnitsBetween(PeriodUnits.Ticks, TimePeriodField.Ticks);
                nanoseconds = UnitsBetween(PeriodUnits.Ticks, TimePeriodField.Nanoseconds);
            }
            return new Period(years, months, weeks, days, hours, minutes, seconds, milliseconds, ticks, nanoseconds);

            long UnitsBetween(PeriodUnits mask, TimePeriodField timeField)
            {
                if ((mask & units) == 0)
                {
                    return 0;
                }
                long value = timeField.GetUnitsInDuration(duration);
                duration -= timeField.ToDuration(value);
                return value;
            }
        }

        /// <summary>
        /// Common code to perform the date parts of the Between methods.
        /// </summary>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <param name="units">Units to compute</param>
        /// <param name="years">(Out) Year component of result</param>
        /// <param name="months">(Out) Months component of result</param>
        /// <param name="weeks">(Out) Weeks component of result</param>
        /// <param name="days">(Out) Days component of result</param>
        /// <returns>The resulting date after adding the result components to <paramref name="start"/> (to
        /// allow further computations to be made)</returns>
        private static LocalDate DateComponentsBetween(LocalDate start, LocalDate end, PeriodUnits units,
            out int years, out int months, out int weeks, out int days)
        {
            LocalDate result = start;
            years = UnitsBetween(units & PeriodUnits.Years, ref result, end, DatePeriodFields.YearsField);
            months = UnitsBetween(units & PeriodUnits.Months, ref result, end, DatePeriodFields.MonthsField);
            weeks = UnitsBetween(units & PeriodUnits.Weeks, ref result, end, DatePeriodFields.WeeksField);
            days = UnitsBetween(units & PeriodUnits.Days, ref result, end, DatePeriodFields.DaysField);

            int UnitsBetween(PeriodUnits maskedUnits, ref LocalDate startDate, LocalDate endDate, IDatePeriodField dateField)
            {
                if (maskedUnits == 0)
                {
                    return 0;
                }
                int value = dateField.UnitsBetween(startDate, endDate);
                startDate = dateField.Add(startDate, value);
                return value;
            }
            return result;
        }

        /// <summary>
        /// Common code to perform the time parts of the Between methods for long-representable nanos.
        /// </summary>
        /// <param name="totalNanoseconds">Number of nanoseconds to compute the units of</param>
        /// <param name="units">Units to compute</param>
        /// <param name="hours">(Out) Hours component of result</param>
        /// <param name="minutes">(Out) Minutes component of result</param>
        /// <param name="seconds">(Out) Seconds component of result</param>
        /// <param name="milliseconds">(Out) Milliseconds component of result</param>
        /// <param name="ticks">(Out) Ticks component of result</param>
        /// <param name="nanoseconds">(Out) Nanoseconds component of result</param>
        private static void TimeComponentsBetween(long totalNanoseconds, PeriodUnits units,
            out long hours, out long minutes, out long seconds, out long milliseconds, out long ticks, out long nanoseconds)
        {
            hours = UnitsBetween(PeriodUnits.Hours, NanosecondsPerHour);
            minutes = UnitsBetween(PeriodUnits.Minutes, NanosecondsPerMinute);
            seconds = UnitsBetween(PeriodUnits.Seconds, NanosecondsPerSecond);
            milliseconds = UnitsBetween(PeriodUnits.Milliseconds, NanosecondsPerMillisecond);
            ticks = UnitsBetween(PeriodUnits.Ticks, NanosecondsPerTick);
            nanoseconds = UnitsBetween(PeriodUnits.Nanoseconds, 1);

            long UnitsBetween(PeriodUnits mask, long nanosecondsPerUnit)
            {
                if ((mask & units) == 0)
                {
                    return 0;
                }
#if NET45
                return Math.DivRem(totalNanoseconds, nanosecondsPerUnit, out totalNanoseconds);
#else
                unchecked
                {
                    long value = totalNanoseconds / nanosecondsPerUnit;
                    // This has been tested and found to be faster than using totalNanoseconds %= nanosecondsPerUnit
                    totalNanoseconds -= value * nanosecondsPerUnit;
                    return value;
                }
#endif
            }
        }

        // TODO(optimization): These three methods are only ever used with scalar values of 1 or -1. Unlikely that
        // the multiplications are going to be relevant, but may be worth testing. (Easy enough to break out
        // code for the two values separately.)

        /// <summary>
        /// Adds the time components of this period to the given time, scaled accordingly.
        /// </summary>
        [Pure]
        internal LocalTime AddTo(LocalTime time, int scalar) =>
            time.PlusHours(Hours * scalar)
                .PlusMinutes(Minutes * scalar)
                .PlusSeconds(Seconds * scalar)
                .PlusMilliseconds(Milliseconds * scalar)
                .PlusTicks(Ticks * scalar)
                .PlusNanoseconds(Nanoseconds * scalar);

        /// <summary>
        /// Adds the date components of this period to the given time, scaled accordingly.
        /// </summary>
        [Pure]
        internal LocalDate AddTo(LocalDate date, int scalar) =>
            date.PlusYears(Years * scalar)
                .PlusMonths(Months * scalar)
                .PlusWeeks(Weeks * scalar)
                .PlusDays(Days * scalar);

        /// <summary>
        /// Adds the contents of this period to the given date and time, with the given scale (either 1 or -1, usually).
        /// </summary>
        internal LocalDateTime AddTo(LocalDate date, LocalTime time, int scalar)
        {
            date = AddTo(date, scalar);
            int extraDays = 0;
            time = TimePeriodField.Hours.Add(time, Hours * scalar, ref extraDays);
            time = TimePeriodField.Minutes.Add(time, Minutes * scalar, ref extraDays);
            time = TimePeriodField.Seconds.Add(time, Seconds * scalar, ref extraDays);
            time = TimePeriodField.Milliseconds.Add(time, Milliseconds * scalar, ref extraDays);
            time = TimePeriodField.Ticks.Add(time, Ticks * scalar, ref extraDays);
            time = TimePeriodField.Nanoseconds.Add(time, Nanoseconds * scalar, ref extraDays);
            // TODO(optimization): Investigate the performance impact of us calling PlusDays twice.
            // Could optimize by including that in a single call...
            return new LocalDateTime(date.PlusDays(extraDays), time);
        }

        /// <summary>
        /// Returns the exact difference between two date/times.
        /// </summary>
        /// <remarks>
        /// If <paramref name="end"/> is before <paramref name="start" />, each property in the returned period
        /// will be negative.
        /// </remarks>
        /// <param name="start">Start date/time</param>
        /// <param name="end">End date/time</param>
        /// <returns>The period between the two date and time values, using all units.</returns>
        [Pure]
        [NotNull]
        public static Period Between(LocalDateTime start, LocalDateTime end) => Between(start, end, PeriodUnits.DateAndTime);

        /// <summary>
        /// Returns the period between a start and an end date, using only the given units.
        /// </summary>
        /// <remarks>
        /// If <paramref name="end"/> is before <paramref name="start" />, each property in the returned period
        /// will be negative. If the given set of units cannot exactly reach the end point (e.g. finding
        /// the difference between 12th February and 15th March in months) the result will be such that adding it to <paramref name="start"/>
        /// will give a value between <paramref name="start"/> and <paramref name="end"/>. In other words,
        /// any rounding is "towards start"; this is true whether the resulting period is negative or positive.
        /// </remarks>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <param name="units">Units to use for calculations</param>
        /// <exception cref="ArgumentException"><paramref name="units"/> contains time units, is empty or contains unknown values.</exception>
        /// <exception cref="ArgumentException"><paramref name="start"/> and <paramref name="end"/> use different calendars.</exception>
        /// <returns>The period between the given dates, using the given units.</returns>
        [Pure]
        [NotNull]
        public static Period Between(LocalDate start, LocalDate end, PeriodUnits units)
        {
            Preconditions.CheckArgument((units & PeriodUnits.AllTimeUnits) == 0, nameof(units), "Units contains time units: {0}", units);
            Preconditions.CheckArgument(units != 0, nameof(units), "Units must not be empty");
            Preconditions.CheckArgument((units & ~PeriodUnits.AllUnits) == 0, nameof(units), "Units contains an unknown value: {0}", units);
            CalendarSystem calendar = start.Calendar;
            Preconditions.CheckArgument(calendar.Equals(end.Calendar), nameof(end), "start and end must use the same calendar system");

            if (start == end)
            {
                return Zero;
            }

            // Optimization for single field
            switch (units)
            {
                case PeriodUnits.Years: return FromYears(DatePeriodFields.YearsField.UnitsBetween(start, end));
                case PeriodUnits.Months: return FromMonths(DatePeriodFields.MonthsField.UnitsBetween(start, end));
                case PeriodUnits.Weeks: return FromWeeks(DatePeriodFields.WeeksField.UnitsBetween(start, end));
                case PeriodUnits.Days: return FromDays(DaysBetween(start, end));
            }

            // Multiple fields
            DateComponentsBetween(start, end, units, out int years, out int months, out int weeks, out int days);
            return new Period(years, months, weeks, days);
        }

        /// <summary>
        /// Returns the exact difference between two dates.
        /// </summary>
        /// <remarks>
        /// If <paramref name="end"/> is before <paramref name="start" />, each property in the returned period
        /// will be negative.
        /// The calendar systems of the two dates must be the same; an exception will be thrown otherwise.
        /// </remarks>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <returns>The period between the two dates, using year, month and day units.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="start"/> and <paramref name="end"/> are not in the same calendar system.
        /// </exception>
        [Pure]
        [NotNull]
        public static Period Between(LocalDate start, LocalDate end) => Between(start, end, PeriodUnits.YearMonthDay);

        /// <summary>
        /// Returns the period between a start and an end time, using only the given units.
        /// </summary>
        /// <remarks>
        /// If <paramref name="end"/> is before <paramref name="start" />, each property in the returned period
        /// will be negative. If the given set of units cannot exactly reach the end point (e.g. finding
        /// the difference between 3am and 4.30am in hours) the result will be such that adding it to <paramref name="start"/>
        /// will give a value between <paramref name="start"/> and <paramref name="end"/>. In other words,
        /// any rounding is "towards start"; this is true whether the resulting period is negative or positive.
        /// </remarks>
        /// <param name="start">Start time</param>
        /// <param name="end">End time</param>
        /// <param name="units">Units to use for calculations</param>
        /// <exception cref="ArgumentException"><paramref name="units"/> contains date units, is empty or contains unknown values.</exception>
        /// <exception cref="ArgumentException"><paramref name="start"/> and <paramref name="end"/> use different calendars.</exception>
        /// <returns>The period between the given times, using the given units.</returns>
        [Pure]
        [NotNull]
        public static Period Between(LocalTime start, LocalTime end, PeriodUnits units)
        {
            Preconditions.CheckArgument((units & PeriodUnits.AllDateUnits) == 0, nameof(units), "Units contains date units: {0}", units);
            Preconditions.CheckArgument(units != 0, nameof(units), "Units must not be empty");
            Preconditions.CheckArgument((units & ~PeriodUnits.AllUnits) == 0, nameof(units), "Units contains an unknown value: {0}", units);

            // We know that the difference is in the range of +/- 1 day, which is a relatively small
            // number of nanoseconds. All the operations can be done with simple long division/remainder ops,
            // so we don't need to delegate to TimePeriodField.

            long remaining = unchecked (end.NanosecondOfDay - start.NanosecondOfDay);

            // Optimization for a single unit
            switch (units)
            {
                case PeriodUnits.Hours: return FromHours(remaining / NanosecondsPerHour);
                case PeriodUnits.Minutes: return FromMinutes(remaining / NanosecondsPerMinute);
                case PeriodUnits.Seconds: return FromSeconds(remaining / NanosecondsPerSecond);
                case PeriodUnits.Milliseconds: return FromMilliseconds(remaining / NanosecondsPerMillisecond);
                case PeriodUnits.Ticks: return FromTicks(remaining / NanosecondsPerTick);
                case PeriodUnits.Nanoseconds: return FromNanoseconds(remaining);
            }

            TimeComponentsBetween(remaining, units, out long hours, out long minutes, out long seconds, out long milliseconds, out long ticks, out long nanoseconds);
            return new Period(hours, minutes, seconds, milliseconds, ticks, nanoseconds);
        }

        /// <summary>
        /// Returns the exact difference between two times.
        /// </summary>
        /// <remarks>
        /// If <paramref name="end"/> is before <paramref name="start" />, each property in the returned period
        /// will be negative.
        /// </remarks>
        /// <param name="start">Start time</param>
        /// <param name="end">End time</param>
        /// <returns>The period between the two times, using the time period units.</returns>
        [Pure]
        [NotNull]
        public static Period Between(LocalTime start, LocalTime end) => Between(start, end, PeriodUnits.AllTimeUnits);

        /// <summary>
        /// Returns the number of days between two dates. This allows optimizations in DateInterval,
        /// and for date calculations which just use days - we don't need state or a virtual method invocation.
        /// </summary>
        internal static int DaysBetween(LocalDate start, LocalDate end)
        {
            // We already assume the calendars are the same.
            if (start.YearMonthDay == end.YearMonthDay)
            {
                return 0;
            }
            // Note: I've experimented with checking for the dates being in the same year and optimizing that.
            // It helps a little if they're in the same month, but just that test has a cost for other situations.
            // Being able to find the day of year if they're in the same year but different months doesn't help,
            // somewhat surprisingly.
            int startDays = start.DaysSinceEpoch;
            int endDays = end.DaysSinceEpoch;
            return endDays - startDays;
        }

        /// <summary>
        /// Returns whether or not this period contains any non-zero-valued time-based properties (hours or lower).
        /// </summary>
        /// <value>true if the period contains any non-zero-valued time-based properties (hours or lower); false otherwise.</value>
        public bool HasTimeComponent => Hours != 0 || Minutes != 0 || Seconds != 0 || Milliseconds != 0 || Ticks != 0 || Nanoseconds != 0;

        /// <summary>
        /// Returns whether or not this period contains any non-zero date-based properties (days or higher).
        /// </summary>
        /// <value>true if this period contains any non-zero date-based properties (days or higher); false otherwise.</value>
        public bool HasDateComponent => Years != 0 || Months != 0 || Weeks != 0 || Days != 0;

        /// <summary>
        /// For periods that do not contain a non-zero number of years or months, returns a duration for this period
        /// assuming a standard 7-day week, 24-hour day, 60-minute hour etc.
        /// </summary>
        /// <exception cref="InvalidOperationException">The month or year property in the period is non-zero.</exception>
        /// <exception cref="OverflowException">The period doesn't have years or months, but the calculation
        /// overflows the bounds of <see cref="Duration"/>. In some cases this may occur even though the theoretical
        /// result would be valid due to balancing positive and negative values, but for simplicity there is
        /// no attempt to work around this - in realistic periods, it shouldn't be a problem.</exception>
        /// <returns>The duration of the period.</returns>
        [Pure]
        public Duration ToDuration()
        {
            if (Months != 0 || Years != 0)
            {
                throw new InvalidOperationException("Cannot construct duration of period with non-zero months or years.");
            }
            return Duration.FromNanoseconds(TotalNanoseconds);
        }

        /// <summary>
        /// Gets the total number of nanoseconds duration for the 'standard' properties (all bar years and months).
        /// </summary>
        /// <value>The total number of nanoseconds duration for the 'standard' properties (all bar years and months).</value>
        private long TotalNanoseconds =>
            Nanoseconds +
                Ticks * NanosecondsPerTick +
                Milliseconds * NanosecondsPerMillisecond +
                Seconds * NanosecondsPerSecond +
                Minutes * NanosecondsPerMinute +
                Hours * NanosecondsPerHour +
                Days * NanosecondsPerDay +
                Weeks * NanosecondsPerWeek;

        /// <summary>
        /// Creates a <see cref="PeriodBuilder"/> from this instance. The new builder
        /// is populated with the values from this period, but is then detached from it:
        /// changes made to the builder are not reflected in this period.
        /// </summary>
        /// <returns>A builder with the same values and units as this period.</returns>
        [Pure] [NotNull] [TestExemption(TestExemptionCategory.ConversionName)] public PeriodBuilder ToBuilder() => new PeriodBuilder(this);

        /// <summary>
        /// Returns a normalized version of this period, such that equivalent (but potentially non-equal) periods are
        /// changed to the same representation.
        /// </summary>
        /// <remarks>
        /// Months and years are unchanged
        /// (as they can vary in length), but weeks are multiplied by 7 and added to the
        /// Days property, and all time properties are normalized to their natural range.
        /// Subsecond values are normalized to millisecond and "nanosecond within millisecond" values.
        /// So for example, a period of 25 hours becomes a period of 1 day
        /// and 1 hour. A period of 1,500,750,000 nanoseconds becomes 1 second, 500 milliseconds and
        /// 750,000 nanoseconds. Aside from months and years, either all the properties
        /// end up positive, or they all end up negative. "Week" and "tick" units in the returned period are always 0.
        /// </remarks>
        /// <exception cref="OverflowException">The period doesn't have years or months, but it contains more than
        /// <see cref="Int64.MaxValue"/> nanoseconds when the combined weeks/days/time portions are considered. This is
        /// over 292 years, so unlikely to be a problem in normal usage.
        /// In some cases this may occur even though the theoretical result would be valid due to balancing positive and
        /// negative values, but for simplicity there is no attempt to work around this.</exception>
        /// <returns>The normalized period.</returns>
        /// <seealso cref="NormalizingEqualityComparer"/>
        [Pure] [NotNull] public Period Normalize()
        {
            // Simplest way to normalize: grab all the fields up to "week" and
            // sum them.
            long totalNanoseconds = TotalNanoseconds;
            int days = (int) (totalNanoseconds / NanosecondsPerDay);
            long hours = (totalNanoseconds / NanosecondsPerHour) % HoursPerDay;
            long minutes = (totalNanoseconds / NanosecondsPerMinute) % MinutesPerHour;
            long seconds = (totalNanoseconds / NanosecondsPerSecond) % SecondsPerMinute;
            long milliseconds = (totalNanoseconds / NanosecondsPerMillisecond) % MillisecondsPerSecond;
            long nanoseconds = totalNanoseconds % NanosecondsPerMillisecond;

            return new Period(this.Years, this.Months, 0 /* weeks */, days, hours, minutes, seconds, milliseconds, 0 /* ticks */, nanoseconds);
        }

        #region Object overrides

        /// <summary>
        /// Returns this string formatted according to the <see cref="PeriodPattern.Roundtrip"/>.
        /// </summary>
        /// <returns>A formatted representation of this period.</returns>
        public override string ToString() => PeriodPattern.Roundtrip.Format(this);

        /// <summary>
        /// Compares the given object for equality with this one, as per <see cref="Equals(Period)"/>.
        /// </summary>
        /// <param name="other">The value to compare this one with.</param>
        /// <returns>true if the other object is a period equal to this one, consistent with <see cref="Equals(Period)"/></returns>
        public override bool Equals(object other) => Equals(other as Period);

        /// <summary>
        /// Returns the hash code for this period, consistent with <see cref="Equals(Period)"/>.
        /// </summary>
        /// <returns>The hash code for this period.</returns>
        public override int GetHashCode() =>
            HashCodeHelper.Initialize()
                .Hash(Years)
                .Hash(Months)
                .Hash(Weeks)
                .Hash(Days)
                .Hash(Hours)
                .Hash(Minutes)
                .Hash(Seconds)
                .Hash(Milliseconds)
                .Hash(Ticks)
                .Hash(Nanoseconds)
                .Value;

        /// <summary>
        /// Compares the given period for equality with this one.
        /// </summary>
        /// <remarks>
        /// Periods are equal if they contain the same values for the same properties.
        /// However, no normalization takes place, so "one hour" is not equal to "sixty minutes".
        /// </remarks>
        /// <param name="other">The period to compare this one with.</param>
        /// <returns>True if this period has the same values for the same properties as the one specified.</returns>
        public bool Equals(Period other) =>
            other != null &&
            Years == other.Years &&
            Months == other.Months &&
            Weeks == other.Weeks &&
            Days == other.Days &&
            Hours == other.Hours &&
            Minutes == other.Minutes &&
            Seconds == other.Seconds &&
            Milliseconds == other.Milliseconds &&
            Ticks == other.Ticks &&
            Nanoseconds == other.Nanoseconds;
        #endregion

#if !NETSTANDARD1_3
        #region Binary serialization
        /// <summary>
        /// Private constructor only present for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        /// <param name="context">The source for this deserialization.</param>
        private Period(SerializationInfo info, StreamingContext context)
            : this((int) info.GetInt64(BinaryFormattingConstants.YearsSerializationName),
                   (int) info.GetInt64(BinaryFormattingConstants.MonthsSerializationName),
                   (int) info.GetInt64(BinaryFormattingConstants.WeeksSerializationName),
                   (int) info.GetInt64(BinaryFormattingConstants.DaysSerializationName),
                   info.GetInt64(BinaryFormattingConstants.HoursSerializationName),
                   info.GetInt64(BinaryFormattingConstants.MinutesSerializationName),
                   info.GetInt64(BinaryFormattingConstants.SecondsSerializationName),
                   info.GetInt64(BinaryFormattingConstants.MillisecondsSerializationName),
                   info.GetInt64(BinaryFormattingConstants.TicksSerializationName),
                   info.GetInt64(BinaryFormattingConstants.NanosecondsSerializationName))
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
            info.AddValue(BinaryFormattingConstants.YearsSerializationName, (long) Years);
            info.AddValue(BinaryFormattingConstants.MonthsSerializationName, (long) Months);
            info.AddValue(BinaryFormattingConstants.WeeksSerializationName, (long) Weeks);
            info.AddValue(BinaryFormattingConstants.DaysSerializationName, (long) Days);
            info.AddValue(BinaryFormattingConstants.HoursSerializationName, Hours);
            info.AddValue(BinaryFormattingConstants.MinutesSerializationName, Minutes);
            info.AddValue(BinaryFormattingConstants.SecondsSerializationName, Seconds);
            info.AddValue(BinaryFormattingConstants.MillisecondsSerializationName, Milliseconds);
            info.AddValue(BinaryFormattingConstants.TicksSerializationName, Ticks);
            info.AddValue(BinaryFormattingConstants.NanosecondsSerializationName, Nanoseconds);
        }
        #endregion
#endif


        /// <summary>
        /// Equality comparer which simply normalizes periods before comparing them.
        /// </summary>
        private sealed class NormalizingPeriodEqualityComparer : EqualityComparer<Period>
        {
            internal static readonly NormalizingPeriodEqualityComparer Instance = new NormalizingPeriodEqualityComparer();

            private NormalizingPeriodEqualityComparer()
            {
            }

            public override bool Equals(Period x, Period y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                {
                    return false;
                }
                return x.Normalize().Equals(y.Normalize());
            }

            public override int GetHashCode([NotNull] Period obj) =>
                Preconditions.CheckNotNull(obj, nameof(obj)).Normalize().GetHashCode();
        }

        private sealed class PeriodComparer : Comparer<Period>
        {
            private readonly LocalDateTime baseDateTime;

            internal PeriodComparer(LocalDateTime baseDateTime)
            {
                this.baseDateTime = baseDateTime;
            }

            public override int Compare(Period x, Period y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }
                if (x == null)
                {
                    return -1;
                }
                if (y == null)
                {
                    return 1;
                }
                if (x.Months == 0 && y.Months == 0 &&
                    x.Years == 0 && y.Years == 0)
                {
                    // Note: this *could* throw an OverflowException when the normal approach
                    // wouldn't, but it's highly unlikely
                    return x.ToDuration().CompareTo(y.ToDuration());
                }
                return (baseDateTime + x).CompareTo(baseDateTime + y);
            }
        }
    }
}
