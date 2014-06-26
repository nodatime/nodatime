// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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
#if !PCL
    [Serializable]
#endif
    [Immutable]
    public sealed class Period : IEquatable<Period>
#if !PCL
        , ISerializable
#endif
    {
        /// <summary>
        /// In some cases, periods are represented as <c>long[]</c> arrays containing all possible units (years to
        /// ticks). This is the size of those arrays.
        /// </summary>
        private const int ValuesArraySize = 9;

        // The indexes into those arrays, for readability.
        // Note that these must match up with the single values in PeriodUnits, such
        // that 1<<index is the same as the equivalent value in PeriodUnits.
        private const int YearIndex = 0;
        private const int MonthIndex = 1;
        private const int WeekIndex = 2;
        private const int DayIndex = 3;
        private const int HourIndex = 4;
        private const int MinuteIndex = 5;
        private const int SecondIndex = 6;
        private const int MillisecondIndex = 7;
        private const int TickIndex = 8;

        /// <summary>
        /// A period containing only zero-valued properties.
        /// </summary>
        public static readonly Period Zero = new Period(0, 0, 0, 0);

        /// <summary>
        /// Returns an equality comparer which compares periods by first normalizing them - so 24 hours is deemed equal to 1 day, and so on.
        /// Note that as per the <see cref="Normalize"/> method, years and months are unchanged by normalization - so 12 months does not
        /// equal 1 year.
        /// </summary>
        public static IEqualityComparer<Period> NormalizingEqualityComparer { get { return NormalizingPeriodEqualityComparer.Instance; } }

        // The fields that make up this period.
        private readonly long ticks;
        private readonly long milliseconds;
        private readonly long seconds;
        private readonly long minutes;
        private readonly long hours;
        private readonly int days;
        private readonly int weeks;
        private readonly int months;
        private readonly int years;

        /// <summary>
        /// Creates a period with the given date values.
        /// </summary>
        internal Period(int years, int months, int weeks, int days)
        {
            this.years = years;
            this.months = months;
            this.weeks = weeks;
            this.days = days;
        }

        /// <summary>
        /// Creates a period with the given time values.
        /// </summary>
        internal Period(long hours, long minutes, long seconds, long milliseconds, long ticks)
        {
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
            this.milliseconds = milliseconds;
            this.ticks = ticks;
        }

        /// <summary>
        /// Creates a new period from the given values.
        /// </summary>
        internal Period(int years, int months, int weeks, int days, long hours, long minutes, long seconds,
            long milliseconds, long ticks)
        {
            this.years = years;
            this.months = months;
            this.weeks = weeks;
            this.days = days;
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
            this.milliseconds = milliseconds;
            this.ticks = ticks;
        }


        /// <summary>
        /// Creates a period representing the specified number of years.
        /// </summary>
        /// <param name="years">The number of years in the new period</param>
        /// <returns>A period consisting of the given number of years.</returns>
        [NotNull]
        public static Period FromYears(int years)
        {
            return new Period(years, 0, 0, 0);
        }

        /// <summary>
        /// Creates a period representing the specified number of months.
        /// </summary>
        /// <param name="months">The number of months in the new period</param>
        /// <returns>A period consisting of the given number of months.</returns>
        public static Period FromMonths(int months)
        {
            return new Period(0, months, 0, 0);
        }

        /// <summary>
        /// Creates a period representing the specified number of weeks.
        /// </summary>
        /// <param name="weeks">The number of weeks in the new period</param>
        /// <returns>A period consisting of the given number of weeks.</returns>
        public static Period FromWeeks(int weeks)
        {
            return new Period(0, 0, weeks, 0);
        }

        /// <summary>
        /// Creates a period representing the specified number of days.
        /// </summary>
        /// <param name="days">The number of days in the new period</param>
        /// <returns>A period consisting of the given number of days.</returns>
        public static Period FromDays(int days)
        {
            return new Period(0, 0, 0, days);
        }

        /// <summary>
        /// Creates a period representing the specified number of hours.
        /// </summary>
        /// <param name="hours">The number of hours in the new period</param>
        /// <returns>A period consisting of the given number of hours.</returns>
        public static Period FromHours(long hours)
        {
            return new Period(hours, 0L, 0L, 0L, 0L);
        }

        /// <summary>
        /// Creates a period representing the specified number of minutes.
        /// </summary>
        /// <param name="minutes">The number of minutes in the new period</param>
        /// <returns>A period consisting of the given number of minutes.</returns>
        public static Period FromMinutes(long minutes)
        {
            return new Period(0L, minutes, 0L, 0L, 0L);
        }

        /// <summary>
        /// Creates a period representing the specified number of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds in the new period</param>
        /// <returns>A period consisting of the given number of seconds.</returns>
        public static Period FromSeconds(long seconds)
        {
            return new Period(0L, 0L, seconds, 0L, 0L);
        }

        /// <summary>
        /// Creates a period representing the specified number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds in the new period</param>
        /// <returns>A period consisting of the given number of milliseconds.</returns>
        public static Period FromMilliseconds(long milliseconds)
        {
            return new Period(0L, 0L, 0L, milliseconds, 0L);
        }

        /// <summary>
        /// Creates a period representing the specified number of ticks.
        /// </summary>
        /// <param name="ticks">The number of ticks in the new period</param>
        /// <returns>A period consisting of the given number of ticks.</returns>
        public static Period FromTicks(long ticks)
        {
            return new Period(0L, 0L, 0L, 0L, ticks);
        }

        /// <summary>
        /// Adds two periods together, by simply adding the values for each property.
        /// </summary>
        /// <param name="left">The first period to add</param>
        /// <param name="right">The second period to add</param>
        /// <returns>The sum of the two periods. The units of the result will be the union of those in both
        /// periods.</returns>
        public static Period operator +(Period left, Period right)
        {
            Preconditions.CheckNotNull(left, "left");
            Preconditions.CheckNotNull(right, "right");
            return new Period(
                left.Years + right.Years,
                left.Months + right.Months,
                left.Weeks + right.Weeks,
                left.Days + right.Days,
                left.Hours + right.Hours,
                left.Minutes + right.Minutes,
                left.Seconds + right.Seconds,
                left.Milliseconds + right.Milliseconds,
                left.Ticks + right.Ticks);
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
        public static IComparer<Period> CreateComparer(LocalDateTime baseDateTime)
        {
            return new PeriodComparer(baseDateTime);
        }

        /// <summary>
        /// Subtracts one period from another, by simply subtracting each property value.
        /// </summary>
        /// <param name="minuend">The period to subtract the second operand from</param>
        /// <param name="subtrahend">The period to subtract the first operand from</param>
        /// <returns>The result of subtracting all the values in the second operand from the values in the first. The
        /// units of the result will be the union of both periods, even if the subtraction caused some properties to
        /// become zero (so "2 weeks, 1 days" minus "2 weeks" is "zero weeks, 1 days", not "1 days").</returns>
        public static Period operator -(Period minuend, Period subtrahend)
        {
            Preconditions.CheckNotNull(minuend, "minuend");
            Preconditions.CheckNotNull(subtrahend, "subtrahend");
            return new Period(
                minuend.Years - subtrahend.Years,
                minuend.Months - subtrahend.Months,
                minuend.Weeks - subtrahend.Weeks,
                minuend.Days - subtrahend.Days,
                minuend.Hours - subtrahend.Hours,
                minuend.Minutes - subtrahend.Minutes,
                minuend.Seconds - subtrahend.Seconds,
                minuend.Milliseconds - subtrahend.Milliseconds,
                minuend.Ticks - subtrahend.Ticks);
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
        public static Period Between(LocalDateTime start, LocalDateTime end, PeriodUnits units)
        {
            Preconditions.CheckArgument(units != 0, "units", "Units must not be empty");
            Preconditions.CheckArgument((units & ~PeriodUnits.AllUnits) == 0, "units", "Units contains an unknown value: {0}", units);
            CalendarSystem calendar = start.Calendar;
            Preconditions.CheckArgument(calendar.Equals(end.Calendar), "end", "start and end must use the same calendar system");

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
                case PeriodUnits.Years: return FromYears(DatePeriodFields.YearsField.Subtract(endDate, start.Date));
                case PeriodUnits.Months: return FromMonths(DatePeriodFields.MonthsField.Subtract(endDate, start.Date));
                case PeriodUnits.Weeks: return FromWeeks(DatePeriodFields.WeeksField.Subtract(endDate, start.Date));
                case PeriodUnits.Days: return FromDays(DatePeriodFields.DaysField.Subtract(endDate, start.Date));
                case PeriodUnits.Hours: return FromHours(GetTimeBetween(start, end, TimePeriodField.Hours));
                case PeriodUnits.Minutes: return FromMinutes(GetTimeBetween(start, end, TimePeriodField.Minutes));
                case PeriodUnits.Seconds: return FromSeconds(GetTimeBetween(start, end, TimePeriodField.Seconds));
                case PeriodUnits.Milliseconds: return FromMilliseconds(GetTimeBetween(start, end, TimePeriodField.Milliseconds));
                case PeriodUnits.Ticks: return FromTicks(GetTimeBetween(start, end, TimePeriodField.Ticks));
            }

            // Multiple fields
            LocalDateTime remaining = start;
            int years = 0, months = 0, weeks = 0, days = 0;
            if ((units & PeriodUnits.AllDateUnits) != 0)
            {
                LocalDate remainingDate = start.Date;
                years = FieldBetween(units & PeriodUnits.Years, endDate, ref remainingDate, DatePeriodFields.YearsField);
                months = FieldBetween(units & PeriodUnits.Months, endDate, ref remainingDate, DatePeriodFields.MonthsField);
                weeks = FieldBetween(units & PeriodUnits.Weeks, endDate, ref remainingDate, DatePeriodFields.WeeksField);
                days = FieldBetween(units & PeriodUnits.Days, endDate, ref remainingDate, DatePeriodFields.DaysField);
                remaining = new LocalDateTime(remainingDate, start.TimeOfDay);
            }

            long hours = 0, minutes = 0, seconds = 0, milliseconds = 0, ticks = 0;
            if ((units & PeriodUnits.AllTimeUnits) != 0)
            {
                hours = FieldBetween(units & PeriodUnits.Hours, end, ref remaining, TimePeriodField.Hours);
                minutes = FieldBetween(units & PeriodUnits.Minutes, end, ref remaining, TimePeriodField.Minutes);
                seconds = FieldBetween(units & PeriodUnits.Seconds, end, ref remaining, TimePeriodField.Seconds);
                milliseconds = FieldBetween(units & PeriodUnits.Milliseconds, end, ref remaining, TimePeriodField.Milliseconds);
                ticks = FieldBetween(units & PeriodUnits.Ticks, end, ref remaining, TimePeriodField.Ticks);
            }

            return new Period(years, months, weeks, days, hours, minutes, seconds, milliseconds, ticks);
        }

        private static int FieldBetween(PeriodUnits units, LocalDate end, ref LocalDate remaining, IDatePeriodField dateField)
        {
            if (units == 0)
            {
                return 0;
            }
            int value = dateField.Subtract(end, remaining);
            remaining = dateField.Add(remaining, value);
            return value;
        }

        private static long FieldBetween(PeriodUnits units, LocalDateTime end, ref LocalDateTime remaining, TimePeriodField timeField)
        {
            if (units == 0)
            {
                return 0;
            }
            long value = GetTimeBetween(remaining, end, timeField);
            remaining = timeField.Add(remaining, value);
            return value;
        }

        private static long GetTimeBetween(LocalDateTime start, LocalDateTime end, TimePeriodField periodField)
        {
            int days = DatePeriodFields.DaysField.Subtract(end.Date, start.Date);
            long units = periodField.Subtract(end.TimeOfDay, start.TimeOfDay);
            return units + days * periodField.UnitsPerDay;
        }

        /// <summary>
        /// Adds the contents of this period to the given date and time, with the given scale (either 1 or -1, usually).
        /// </summary>
        internal LocalDateTime AddTo(LocalDate date, LocalTime time, int scalar)
        {
            date = DatePeriodFields.YearsField.Add(date, years * scalar);
            date = DatePeriodFields.MonthsField.Add(date, months * scalar);
            date = DatePeriodFields.WeeksField.Add(date, weeks * scalar);
            date = DatePeriodFields.DaysField.Add(date, days * scalar);

            LocalDateTime result = new LocalDateTime(date, time);
            result = TimePeriodField.Hours.Add(result, hours * scalar);
            result = TimePeriodField.Minutes.Add(result, minutes * scalar);
            result = TimePeriodField.Seconds.Add(result, seconds * scalar);
            result = TimePeriodField.Milliseconds.Add(result, milliseconds * scalar);
            result = TimePeriodField.Ticks.Add(result, ticks * scalar);

            return result;
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
        public static Period Between(LocalDateTime start, LocalDateTime end)
        {
            return Between(start, end, PeriodUnits.DateAndTime);
        }

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
        public static Period Between(LocalDate start, LocalDate end, PeriodUnits units)
        {
            Preconditions.CheckArgument((units & PeriodUnits.AllTimeUnits) == 0, "units", "Units contains time units: {0}", units);
            return Between(start.AtMidnight(), end.AtMidnight(), units);
        }

        /// <summary>
        /// Returns the exact difference between two dates.
        /// </summary>
        /// <remarks>
        /// If <paramref name="end"/> is before <paramref name="start" />, each property in the returned period
        /// will be negative.
        /// </remarks>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <returns>The period between the two dates, using year, month and day units.</returns>
        public static Period Between(LocalDate start, LocalDate end)
        {
            return Between(start, end, PeriodUnits.YearMonthDay);
        }

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
        public static Period Between(LocalTime start, LocalTime end, PeriodUnits units)
        {
            Preconditions.CheckArgument((units & PeriodUnits.AllDateUnits) == 0, "units", "Units contains date units: {0}", units);
            // FIXME(2.0): Don't do this! (Horrible temporary hack.)
            return Between(new LocalDate(1970, 1, 1) + start,
                           new LocalDate(1970, 1, 1) + end, units);
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
        public static Period Between(LocalTime start, LocalTime end)
        {
            return Between(start, end, PeriodUnits.AllTimeUnits);
        }

        /// <summary>
        /// Returns whether or not this period contains any non-zero-valued time-based properties (hours or lower).
        /// </summary>
        public bool HasTimeComponent
        {
            get
            {
                return hours != 0 || minutes != 0 || seconds != 0 || milliseconds != 0 || ticks != 0;
            }
        }

        /// <summary>
        /// Returns whether or not this period contains any non-zero date-based properties (days or higher).
        /// </summary>
        public bool HasDateComponent
        {
            get
            {
                return years != 0 || months != 0 || weeks != 0 || days != 0;
            }
        }

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
            return Duration.FromTicks(TotalStandardTicks);
        }

        /// <summary>
        /// Gets the total number of ticks duration for the 'standard' properties (all bar years and months).
        /// </summary>
        private long TotalStandardTicks
        {
            get
            {
                // This can overflow even when it wouldn't necessarily need to. See comments elsewhere.
                return ticks +
                    milliseconds * NodaConstants.TicksPerMillisecond +
                    seconds * NodaConstants.TicksPerSecond +
                    minutes * NodaConstants.TicksPerMinute +
                    hours * NodaConstants.TicksPerHour +
                    days * NodaConstants.TicksPerStandardDay +
                    weeks * NodaConstants.TicksPerStandardWeek;
            }
        }

        /// <summary>
        /// Creates a <see cref="PeriodBuilder"/> from this instance. The new builder
        /// is populated with the values from this period, but is then detached from it:
        /// changes made to the builder are not reflected in this period.
        /// </summary>
        /// <returns>A builder with the same values and units as this period.</returns>
        [Pure]
        public PeriodBuilder ToBuilder()
        {
            return new PeriodBuilder(this);
        }

        /// <summary>
        /// Returns a normalized version of this period, such that equivalent (but potentially non-equal) periods are
        /// changed to the same representation.
        /// </summary>
        /// <remarks>
        /// Months and years are unchanged
        /// (as they can vary in length), but weeks are multiplied by 7 and added to the
        /// Days property, and all time properties are normalized to their natural range
        /// (where ticks are "within a millisecond"), adding to the larger property where
        /// necessary. So for example, a period of 25 hours becomes a period of 1 day
        /// and 1 hour. Aside from months and years, either all the properties
        /// end up positive, or they all end up negative.
        /// </remarks>
        /// <exception cref="OverflowException">The period doesn't have years or months, but it contains more than
        /// <see cref="Int64.MaxValue"/> ticks when the combined weeks/days/time portions are considered. Such a period
        /// could never be useful anyway, however.
        /// In some cases this may occur even though the theoretical result would be valid due to balancing positive and
        /// negative values, but for simplicity there is no attempt to work around this - in realistic periods, it
        /// shouldn't be a problem.</exception>
        /// <returns>The normalized period.</returns>
        /// <seealso cref="NormalizingEqualityComparer"/>
        [Pure]
        public Period Normalize()
        {
            // Simplest way to normalize: grab all the fields up to "week" and
            // sum them.
            long totalTicks = TotalStandardTicks;
            int days = (int) (totalTicks / NodaConstants.TicksPerStandardDay);
            long hours = (totalTicks / NodaConstants.TicksPerHour) % NodaConstants.HoursPerStandardDay;
            long minutes = (totalTicks / NodaConstants.TicksPerMinute) % NodaConstants.MinutesPerHour;
            long seconds = (totalTicks / NodaConstants.TicksPerSecond) % NodaConstants.SecondsPerMinute;
            long milliseconds = (totalTicks / NodaConstants.TicksPerMillisecond) % NodaConstants.MillisecondsPerSecond;
            long ticks = totalTicks % NodaConstants.TicksPerMillisecond;

            return new Period(this.years, this.months, 0 /* weeks */, days, hours, minutes, seconds, milliseconds, ticks);
        }

        #region Helper properties
        /// <summary>
        /// Gets the number of years within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        public int Years { get { return years; } }
        /// <summary>
        /// Gets the number of months within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        public int Months { get { return months; } }
        /// <summary>
        /// Gets the number of weeks within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        public int Weeks { get { return weeks; } }
        /// <summary>
        /// Gets the number of days within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        public int Days { get { return days; } }
        /// <summary>
        /// Gets the number of hours within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        public long Hours { get { return hours; } }
        /// <summary>
        /// Gets the number of minutes within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        public long Minutes { get { return minutes; } }
        /// <summary>
        /// Gets the number of seconds within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        public long Seconds { get { return seconds; } }
        /// <summary>
        /// Gets the number of milliseconds within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        public long Milliseconds { get { return milliseconds; } }
        /// <summary>
        /// Gets the number of ticks within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        public long Ticks { get { return ticks; } }
        #endregion

        #region Object overrides

        /// <summary>
        /// Returns this string formatted according to the <see cref="PeriodPattern.RoundtripPattern"/>.
        /// </summary>
        /// <returns>A formatted representation of this period.</returns>
        public override string ToString()
        {
            return PeriodPattern.RoundtripPattern.Format(this);
        }

        /// <summary>
        /// Compares the given object for equality with this one, as per <see cref="Equals(Period)"/>.
        /// </summary>
        /// <param name="other">The value to compare this one with.</param>
        /// <returns>true if the other object is a period equal to this one, consistent with <see cref="Equals(Period)"/></returns>
        public override bool Equals(object other)
        {
            return Equals(other as Period);
        }

        /// <summary>
        /// Returns the hash code for this period, consistent with <see cref="Equals(Period)"/>.
        /// </summary>
        /// <returns>The hash code for this period.</returns>
        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, years);
            hash = HashCodeHelper.Hash(hash, months);
            hash = HashCodeHelper.Hash(hash, weeks);
            hash = HashCodeHelper.Hash(hash, days);
            hash = HashCodeHelper.Hash(hash, hours);
            hash = HashCodeHelper.Hash(hash, minutes);
            hash = HashCodeHelper.Hash(hash, seconds);
            hash = HashCodeHelper.Hash(hash, milliseconds);
            hash = HashCodeHelper.Hash(hash, ticks);
            return hash;
        }

        /// <summary>
        /// Compares the given period for equality with this one.
        /// </summary>
        /// <remarks>
        /// Periods are equal if they contain the same values for the same properties.
        /// However, no normalization takes place, so "one hour" is not equal to "sixty minutes".
        /// </remarks>
        /// <param name="other">The period to compare this one with.</param>
        /// <returns>True if this period has the same values for the same properties as the one specified.</returns>
        public bool Equals(Period other)
        {
            if (other == null)
            {
                return false;
            }

            return years == other.years &&
                months == other.months &&
                weeks == other.weeks &&
                days == other.days &&
                hours == other.hours &&
                minutes == other.minutes &&
                seconds == other.seconds &&
                milliseconds == other.milliseconds &&
                ticks == other.ticks;
        }
        #endregion

#if !PCL
        #region Binary serialization
        private const string YearsSerializationName = "years";
        private const string MonthsSerializationName = "months";
        private const string WeeksSerializationName = "weeks";
        private const string DaysSerializationName = "days";
        private const string HoursSerializationName = "hours";
        private const string MinutesSerializationName = "minutes";
        private const string SecondsSerializationName = "seconds";
        private const string MillisecondsSerializationName = "milliseconds";
        private const string TicksSerializationName = "ticks";

        /// <summary>
        /// Private constructor only present for serialization.
        /// TODO(2.0): Revisit this for 2.0.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        /// <param name="context">The source for this deserialization.</param>
        private Period(SerializationInfo info, StreamingContext context)
            : this((int) info.GetInt64(YearsSerializationName),
                   (int) info.GetInt64(MonthsSerializationName),
                   (int) info.GetInt64(WeeksSerializationName),
                   (int) info.GetInt64(DaysSerializationName),
                   info.GetInt64(HoursSerializationName),
                   info.GetInt64(MinutesSerializationName),
                   info.GetInt64(SecondsSerializationName),
                   info.GetInt64(MillisecondsSerializationName),
                   info.GetInt64(TicksSerializationName))
        {
        }

        /// <summary>
        /// Implementation of <see cref="ISerializable.GetObjectData"/>.
        /// TODO(2.0): Revisit this for 2.0.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination for this serialization.</param>
        [System.Security.SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(YearsSerializationName, (long) years);
            info.AddValue(MonthsSerializationName, (long) months);
            info.AddValue(WeeksSerializationName, (long) weeks);
            info.AddValue(DaysSerializationName, (long) days);
            info.AddValue(HoursSerializationName, hours);
            info.AddValue(MinutesSerializationName, minutes);
            info.AddValue(SecondsSerializationName, seconds);
            info.AddValue(MillisecondsSerializationName, milliseconds);
            info.AddValue(TicksSerializationName, ticks);
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

            public override int GetHashCode(Period obj)
            {
                return Preconditions.CheckNotNull(obj, "obj").Normalize().GetHashCode();
            }
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
