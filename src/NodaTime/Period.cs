// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
        public static readonly Period Zero = new Period(new long[ValuesArraySize]);

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
        private readonly long days;
        private readonly long weeks;
        private readonly long months;
        private readonly long years;

        /// <summary>
        /// Creates a new period from the given array.
        /// </summary>
        /// <param name="values">Values for each field</param>
        private Period(long[] values)
        {
            years = values[YearIndex];
            months = values[MonthIndex];
            weeks = values[WeekIndex];
            days = values[DayIndex];
            hours = values[HourIndex];
            minutes = values[MinuteIndex];
            seconds = values[SecondIndex];
            milliseconds = values[MillisecondIndex];
            ticks = values[TickIndex];
        }

        /// <summary>
        /// Creates a new period from the given values.
        /// </summary>
        internal Period(long years, long months, long weeks, long days, long hours, long minutes, long seconds,
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
        /// Creates a new period with the given single value.
        /// </summary>
        private Period(PeriodUnits periodUnit, long value)
        {
            switch (periodUnit)
            {
                case PeriodUnits.Years: years = value; break;
                case PeriodUnits.Months: months = value; break;
                case PeriodUnits.Weeks: weeks = value; break;
                case PeriodUnits.Days: days = value; break;
                case PeriodUnits.Hours: hours = value; break;
                case PeriodUnits.Minutes: minutes = value; break;
                case PeriodUnits.Seconds: seconds = value; break;
                case PeriodUnits.Milliseconds: milliseconds = value; break;
                case PeriodUnits.Ticks: ticks = value; break;
                default: throw new ArgumentException("Unit must be singular", "periodUnit");
            }
        }

        /// <summary>
        /// Creates a period representing the specified number of years.
        /// </summary>
        /// <param name="years">The number of years in the new period</param>
        /// <returns>A period consisting of the given number of years.</returns>
        public static Period FromYears(long years)
        {
            return new Period(PeriodUnits.Years, years);
        }

        /// <summary>
        /// Creates a period representing the specified number of weeks.
        /// </summary>
        /// <param name="weeks">The number of weeks in the new period</param>
        /// <returns>A period consisting of the given number of weeks.</returns>
        public static Period FromWeeks(long weeks)
        {
            return new Period(PeriodUnits.Weeks, weeks);
        }

        /// <summary>
        /// Creates a period representing the specified number of months.
        /// </summary>
        /// <param name="months">The number of months in the new period</param>
        /// <returns>A period consisting of the given number of months.</returns>
        public static Period FromMonths(long months)
        {
            return new Period(PeriodUnits.Months, months);
        }

        /// <summary>
        /// Creates a period representing the specified number of days.
        /// </summary>
        /// <param name="days">The number of days in the new period</param>
        /// <returns>A period consisting of the given number of days.</returns>
        public static Period FromDays(long days)
        {
            return new Period(PeriodUnits.Days, days);
        }

        /// <summary>
        /// Creates a period representing the specified number of hours.
        /// </summary>
        /// <param name="hours">The number of hours in the new period</param>
        /// <returns>A period consisting of the given number of hours.</returns>
        public static Period FromHours(long hours)
        {
            return new Period(PeriodUnits.Hours, hours);
        }

        /// <summary>
        /// Creates a period representing the specified number of minutes.
        /// </summary>
        /// <param name="minutes">The number of minutes in the new period</param>
        /// <returns>A period consisting of the given number of minutes.</returns>
        public static Period FromMinutes(long minutes)
        {
            return new Period(PeriodUnits.Minutes, minutes);
        }

        /// <summary>
        /// Creates a period representing the specified number of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds in the new period</param>
        /// <returns>A period consisting of the given number of seconds.</returns>
        public static Period FromSeconds(long seconds)
        {
            return new Period(PeriodUnits.Seconds, seconds);
        }

#if !PCL
        /// <summary>
        /// Creates a period representing the specified number of miliseconds.
        /// </summary>
        /// <remarks>This method is not available in the PCL version, as it was made obsolete in Noda Time 1.1.</remarks>
        /// <param name="milliseconds">The number of milliseconds in the new period</param>
        /// <returns>A period consisting of the given number of milliseconds.</returns>
        [Obsolete("Use FromMilliseconds instead. This method's name was a typo, and it will be removed in a future release.")]
        public static Period FromMillseconds(long milliseconds)
        {
            return FromMilliseconds(milliseconds);
        }
#endif

        /// <summary>
        /// Creates a period representing the specified number of miliseconds.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds in the new period</param>
        /// <returns>A period consisting of the given number of milliseconds.</returns>
        public static Period FromMilliseconds(long milliseconds)
        {
            return new Period(PeriodUnits.Milliseconds, milliseconds);
        }

        /// <summary>
        /// Creates a period representing the specified number of ticks.
        /// </summary>
        /// <param name="ticks">The number of ticks in the new period</param>
        /// <returns>A period consisting of the given number of ticks.</returns>
        public static Period FromTicks(long ticks)
        {
            return new Period(PeriodUnits.Ticks, ticks);
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
            long[] sum = left.ToArray();
            right.AddValuesTo(sum);
            return new Period(sum);
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
        /// Returns the property values in this period as an array.
        /// </summary>
        private long[] ToArray()
        {
            long[] values = new long[ValuesArraySize];
            values[YearIndex] = years;
            values[MonthIndex] = months;
            values[WeekIndex] = weeks;
            values[DayIndex] = days;
            values[HourIndex] = hours;
            values[MinuteIndex] = minutes;
            values[SecondIndex] = seconds;
            values[MillisecondIndex] = milliseconds;
            values[TickIndex] = ticks;
            return values;
        }

        /// <summary>
        /// Adds all the values in this period to the given array of values (which is assumed to be of the right
        /// length).
        /// </summary>
        private void AddValuesTo(long[] values)
        {
            values[YearIndex] += years;
            values[MonthIndex] += months;
            values[WeekIndex] += weeks;
            values[DayIndex] += days;
            values[HourIndex] += hours;
            values[MinuteIndex] += minutes;
            values[SecondIndex] += seconds;
            values[MillisecondIndex] += milliseconds;
            values[TickIndex] += ticks;
        }

        /// <summary>
        /// Subtracts all the values in this period from the given array of values (which is assumed to be of the right
        /// length).
        /// </summary>
        private void SubtractValuesFrom(long[] values)
        {
            values[YearIndex] -= years;
            values[MonthIndex] -= months;
            values[WeekIndex] -= weeks;
            values[DayIndex] -= days;
            values[HourIndex] -= hours;
            values[MinuteIndex] -= minutes;
            values[SecondIndex] -= seconds;
            values[MillisecondIndex] -= milliseconds;
            values[TickIndex] -= ticks;
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
            long[] sum = minuend.ToArray();
            subtrahend.SubtractValuesFrom(sum);
            return new Period(sum);
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

            LocalInstant startLocalInstant = start.LocalInstant;
            LocalInstant endLocalInstant = end.LocalInstant;

            if (startLocalInstant == endLocalInstant)
            {
                return Zero;
            }

            PeriodFieldSet fields = calendar.PeriodFields;

            // Optimization for single field
            var singleField = GetSingleField(fields, units);
            if (singleField != null)
            {
                long value = singleField.Subtract(end.LocalInstant, start.LocalInstant);
                return new Period(units, value);
            }

            // Multiple fields
            long[] values = new long[ValuesArraySize];

            LocalInstant remaining = startLocalInstant;
            int numericFields = (int) units;
            for (int i = 0; i < ValuesArraySize; i++)
            {
                if ((numericFields & (1 << i)) != 0)
                {
                    var field = GetFieldForIndex(fields, i);
                    values[i] = field.Subtract(endLocalInstant, remaining);
                    remaining = field.Add(remaining, values[i]);
                }
            }
            return new Period(values);
        }

        /// <summary>
        /// Adds the contents of this period to the given local instant in the given calendar system.
        /// </summary>
        internal LocalInstant AddTo(LocalInstant localInstant, CalendarSystem calendar, int scalar)
        {
            Preconditions.CheckNotNull(calendar, "calendar");

            PeriodFieldSet fields = calendar.PeriodFields;
            LocalInstant result = localInstant;

            if (years != 0) result = fields.Years.Add(result, years * scalar);
            if (months != 0) result = fields.Months.Add(result, months * scalar);
            if (weeks != 0) result = fields.Weeks.Add(result, weeks * scalar);
            if (days != 0) result = fields.Days.Add(result, days * scalar);
            if (hours != 0) result = fields.Hours.Add(result, hours * scalar);
            if (minutes != 0) result = fields.Minutes.Add(result, minutes * scalar);
            if (seconds != 0) result = fields.Seconds.Add(result, seconds * scalar);
            if (milliseconds != 0) result = fields.Milliseconds.Add(result, milliseconds * scalar);
            if (ticks != 0) result = fields.Ticks.Add(result, ticks * scalar);

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
            return Between(start.AtMidnight(), end.AtMidnight(), PeriodUnits.YearMonthDay);
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
            return Between(start.LocalDateTime, end.LocalDateTime, units);
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
            return Between(start.LocalDateTime, end.LocalDateTime, PeriodUnits.AllTimeUnits);
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
        public Period Normalize()
        {
            // TODO: Consider improving the efficiency of this: return "this" when it's already normalized.
            // Simplest way to normalize: grab all the fields up to "week" and
            // sum them.
            long totalTicks = TotalStandardTicks;
            long days = totalTicks / NodaConstants.TicksPerStandardDay;
            long hours = (totalTicks / NodaConstants.TicksPerHour) % NodaConstants.HoursPerStandardDay;
            long minutes = (totalTicks / NodaConstants.TicksPerMinute) % NodaConstants.MinutesPerHour;
            long seconds = (totalTicks / NodaConstants.TicksPerSecond) % NodaConstants.SecondsPerMinute;
            long milliseconds = (totalTicks / NodaConstants.TicksPerMillisecond) % NodaConstants.MillisecondsPerSecond;
            long ticks = totalTicks % NodaConstants.TicksPerMillisecond;

            return new Period(this.years, this.months, 0 /* weeks */, days, hours, minutes, seconds, milliseconds, ticks);
        }

        /// <summary>
        /// Returns the PeriodField for the given unit value, or null if the values does
        /// not represent a single unit.
        /// </summary>
        private static IPeriodField GetSingleField(PeriodFieldSet fields, PeriodUnits units)
        {
            switch (units)
            {
                case PeriodUnits.Years: return fields.Years;
                case PeriodUnits.Months: return fields.Months;
                case PeriodUnits.Weeks: return fields.Weeks;;
                case PeriodUnits.Days: return fields.Days;
                case PeriodUnits.Hours: return fields.Hours;
                case PeriodUnits.Minutes: return fields.Minutes;
                case PeriodUnits.Seconds: return fields.Seconds;
                case PeriodUnits.Milliseconds: return fields.Milliseconds;
                case PeriodUnits.Ticks: return fields.Ticks;
                default: return null;
            }
        }

        /// <summary>
        /// Returns the PeriodField for the given index, in the range 0-8 inclusive.
        /// </summary>
        private static IPeriodField GetFieldForIndex(PeriodFieldSet fields, int index)
        {
            switch (index)
            {
                case YearIndex: return fields.Years;
                case MonthIndex: return fields.Months;
                case WeekIndex: return fields.Weeks;
                case DayIndex: return fields.Days;
                case HourIndex: return fields.Hours;
                case MinuteIndex: return fields.Minutes;
                case SecondIndex: return fields.Seconds;
                case MillisecondIndex: return fields.Milliseconds;
                case TickIndex: return fields.Ticks;
                default: throw new ArgumentOutOfRangeException("index");
            }
        }

        #region Helper properties
        /// <summary>
        /// Gets the number of years within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        public long Years { get { return years; } }
        /// <summary>
        /// Gets the number of months within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        public long Months { get { return months; } }
        /// <summary>
        /// Gets the number of weeks within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        public long Weeks { get { return weeks; } }
        /// <summary>
        /// Gets the number of days within this period.
        /// </summary>
        /// <remarks>
        /// This property returns zero both when the property has been explicitly set to zero and when the period does not
        /// contain this property.
        /// </remarks>
        public long Days { get { return days; } }
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
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        /// <param name="context">The source for this deserialization.</param>
        private Period(SerializationInfo info, StreamingContext context)
            : this(info.GetInt64(YearsSerializationName),
                   info.GetInt64(MonthsSerializationName),
                   info.GetInt64(WeeksSerializationName),
                   info.GetInt64(DaysSerializationName),
                   info.GetInt64(HoursSerializationName),
                   info.GetInt64(MinutesSerializationName),
                   info.GetInt64(SecondsSerializationName),
                   info.GetInt64(MillisecondsSerializationName),
                   info.GetInt64(TicksSerializationName))
        {
        }

        /// <summary>
        /// Implementation of <see cref="ISerializable.GetObjectData"/>.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination for this serialization.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(YearsSerializationName, years);
            info.AddValue(MonthsSerializationName, months);
            info.AddValue(WeeksSerializationName, weeks);
            info.AddValue(DaysSerializationName, days);
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
