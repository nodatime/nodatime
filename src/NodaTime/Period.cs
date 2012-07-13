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
using System.Collections.Generic;
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
    /// Periods operate on calendar-related types such as
    /// <see cref="LocalDateTime" /> whereas <see cref="Duration"/> operates on instants
    /// on the time line. Although <see cref="ZonedDateTime" /> includes both concepts, it is generally
    /// simpler to consider period-based arithmetic solely on local dates and times, so only
    /// duration-based arithmetic is supported on ZonedDateTime. This avoids ambiguities
    /// and skipped date/time values becoming a problem within a series of calculations; instead,
    /// these can be considered just once, at the point of conversion to a ZonedDateTime.
    /// </remarks>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    public sealed class Period : IEquatable<Period>
    {
        private static readonly int TypeInitializationChecking = NodaTime.Utility.TypeInitializationChecker.RecordInitializationStart();

        /// <summary>
        /// The number of values in an array for a compound period. This is always the same, representing
        /// all possible units.
        /// </summary>
        private const int ValuesArraySize = 9;

        /// <summary>
        /// An empty period with no units.
        /// </summary>
        public static readonly Period Empty = new Period(0, new long[ValuesArraySize]);

        /// <summary>
        /// Returns an equality comparer which compares periods by first normalizing them - so 24 hours is deemed equal to 1 day, and so on.
        /// Note that as per the <see cref="Normalize"/> method, years and months are unchanged by normalization - so 12 months does not
        /// equal 1 year.
        /// </summary>
        public static IEqualityComparer<Period> NormalizingEqualityComparer { get { return NormalizingPeriodEqualityComparer.Instance; } }

        // Just to avoid magic numbers elsewhere. Not an enum as we normally want to use
        // the value as an index immediately afterwards.
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
        /// The units contained within this period.
        /// </summary>
        private readonly PeriodUnits units;

        /// <summary>
        /// The single value for single-valued periods. Ignored for compound periods.
        /// </summary>
        private readonly long singleValue;

        /// <summary>
        /// All values for compound periods, or null for single-valued periods.
        /// </summary>
        private readonly long[] values;

        /// <summary>
        /// Creates a new period with the given array without copying it. The array contents must
        /// not be changed after the value has been constructed - which is why this method is private.
        /// </summary>
        /// <param name="units">The units within this period, describing which fields are present</param>
        /// <param name="values">Values for each field in the period type</param>
        private Period(PeriodUnits units, long[] values)
        {
            this.values = values;
            this.units = units;
            this.singleValue = 0;
        }

        /// <summary>
        /// Createds a new period with the given single value. The unit is assumed to be single.
        /// </summary>
        private Period(PeriodUnits periodUnit, long value)
        {
            this.values = null;
            this.units = periodUnit;
            this.singleValue = value;
        }

        /// <summary>
        /// Internal method to create a new period with the given units and values.
        /// This just delegates to the private constructor with the same parameters;
        /// this method only exists (rather than the constructor being internal) for
        /// clarity of naming. It must *only* be used where the array will never
        /// be published.
        /// </summary>
        internal static Period UnsafeCreate(PeriodUnits units, long[] values)
        {
            return new Period(units, values);
        }

        /// <summary>
        /// Returns the units within this period, such as "year, month, day" or "hour".
        /// </summary>
        public PeriodUnits Units { get { return units; } }

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

        /// <summary>
        /// Creates a period representing the specified number of miliseconds.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds in the new period</param>
        /// <returns>A period consisting of the given number of milliseconds.</returns>
        public static Period FromMillseconds(long milliseconds)
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
        /// Adds two periods together, by simply adding the values for each field. Currently this
        /// returns a period with a period type of "all fields".
        /// </summary>
        /// <param name="left">The first period to add</param>
        /// <param name="right">The second period to add</param>
        /// <returns>The sum of the two periods.</returns>
        public static Period operator +(Period left, Period right)
        {
            Preconditions.CheckNotNull(left, "left");
            Preconditions.CheckNotNull(right, "right");
            long[] sum = new long[ValuesArraySize];
            left.AddValuesTo(sum);
            right.AddValuesTo(sum);
            return new Period(left.Units | right.Units, sum);
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
        /// <param name="baseDateTime"></param>
        /// <returns></returns>
        public static IComparer<Period> CreateComparer(LocalDateTime baseDateTime)
        {
            return new PeriodComparer(baseDateTime);
        }

        /// <summary>
        /// Adds all the values in this period to the given array of values (which is assumed to be of the right length).
        /// </summary>
        private void AddValuesTo(long[] newValues)
        {
            if (values == null)
            {
                int index = GetSingleFieldIndex(units);
                newValues[index] += singleValue;
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                {
                    newValues[i] += values[i];
                }
            }
        }

        /// <summary>
        /// Subtracts one periods from another, by simply subtracting each field value. Currently this
        /// returns a period with a period type of "all fields".
        /// </summary>
        /// <param name="minuend">The period to subtract the second operand from</param>
        /// <param name="subtrahend">The period to subtract the first operand from</param>
        /// <returns>The result of subtracting all the values in the second operand from the values in the first.</returns>
        public static Period operator -(Period minuend, Period subtrahend)
        {
            Preconditions.CheckNotNull(minuend, "minuend");
            Preconditions.CheckNotNull(subtrahend, "subtrahend");
            long[] sum = new long[ValuesArraySize];
            subtrahend.AddValuesTo(sum);
            // Not terribly efficient, but it's only 9 values...
            for (int i = 0; i < sum.Length; i++)
            {
                sum[i] = -sum[i];
            }
            minuend.AddValuesTo(sum);
            return new Period(minuend.Units | subtrahend.Units, sum);
        }

        /// <summary>
        /// Returns the period between a start and an end date/time, using the set of fields in the given
        /// period type.
        /// </summary>
        /// <remarks>
        /// If <paramref name="end"/> is before <paramref name="start" />, each field in the returned period
        /// will be negative. If the given period type cannot exactly reach the end point (e.g. finding
        /// the difference between 1am and 3:15am in hours) the result will be such that adding it to <paramref name="start"/>
        /// will give a value between <paramref name="start"/> and <paramref name="end"/>. In other words,
        /// any rounding is "towards start"; this is true whether the resulting period is negative or positive.
        /// </remarks>
        /// <param name="start">Start date/time</param>
        /// <param name="end">End date/time</param>
        /// <param name="units">Period type to use for calculations</param>
        /// <exception cref="ArgumentException"><paramref name="units"/> is empty or contained unknown values</exception>
        /// <exception cref="ArgumentException"><paramref name="start"/> and <paramref name="end"/> use different calendars</exception>
        /// <returns>The period between the given date/times</returns>
        public static Period Between(LocalDateTime start, LocalDateTime end, PeriodUnits units)
        {
            Preconditions.CheckArgument(units != 0, "units", "Units must not be empty");
            Preconditions.CheckArgument((units & ~PeriodUnits.AllUnits) == 0, "units", "Units contains an unknown value: " + units);
            CalendarSystem calendar = start.Calendar;
            Preconditions.CheckArgument(calendar.Equals(end.Calendar), "end", "start and end must use the same calendar system");

            LocalInstant startLocalInstant = start.LocalInstant;
            LocalInstant endLocalInstant = end.LocalInstant;

            FieldSet fieldSet = calendar.Fields;

            // Optimization for single field
            int singleIndex = GetSingleFieldIndex(units);
            if (singleIndex != -1)
            {
                long value = startLocalInstant == endLocalInstant ? 0 : GetFieldForIndex(fieldSet, singleIndex).GetInt64Difference(end.LocalInstant, start.LocalInstant);
                return new Period(units, value);
            }

            // Multiple fields
            long[] values = new long[ValuesArraySize];

            if (startLocalInstant == endLocalInstant)
            {
                return new Period(units, values);
            }

            LocalInstant remaining = startLocalInstant;
            int numericFields = (int) units;
            for (int i = 0; i < ValuesArraySize; i++)
            {
                if ((numericFields & (1 << i)) != 0)
                {
                    var field = GetFieldForIndex(fieldSet, i);
                    values[i] = field.GetInt64Difference(endLocalInstant, remaining);
                    remaining = field.Add(remaining, values[i]);
                }
            }
            return new Period(units, values);
        }

        /// <summary>
        /// Adds the contents of this period to the given local instant in the given calendar system.
        /// </summary>
        /// <param name="localInstant"></param>
        /// <param name="calendar"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        internal LocalInstant AddTo(LocalInstant localInstant, CalendarSystem calendar, int scalar)
        {
            Preconditions.CheckNotNull(calendar, "calendar");
            if (scalar == 0)
            {
                return localInstant;
            }

            FieldSet fieldSet = calendar.Fields;
            if (values == null)
            {
                int index = GetSingleFieldIndex(units);
                PeriodField field = GetFieldForIndex(fieldSet, index);
                return field.Add(localInstant, singleValue * scalar);
            }

            LocalInstant result = localInstant;

            for (int i = 0; i < values.Length; i++)
            {
                long value = values[i];
                if (value != 0)
                {
                    result = GetFieldForIndex(fieldSet, i).Add(result, value * scalar);
                }
            }
            return result;
        }


        /// <summary>
        /// Returns the difference between two date/times using the "all fields" period type.
        /// </summary>
        /// <param name="start">Start date/time</param>
        /// <param name="end">End date/time</param>
        /// <returns>The period between the two date and time values, using all period fields.</returns>
        public static Period Between(LocalDateTime start, LocalDateTime end)
        {
            return Between(start, end, PeriodUnits.DateAndTime);
        }

        /// <summary>
        /// Returns the period between a start and an end date, using the set of fields in the given
        /// period type.
        /// </summary>
        /// <remarks>
        /// If <paramref name="end"/> is before <paramref name="start" />, each field in the returned period
        /// will be negative. If the given period type cannot exactly reach the end point (e.g. finding
        /// the difference between 12th February and 15th March in months) the result will be such that adding it to <paramref name="start"/>
        /// will give a value between <paramref name="start"/> and <paramref name="end"/>. In other words,
        /// any rounding is "towards start"; this is true whether the resulting period is negative or positive.
        /// </remarks>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <param name="units">Units to use for calculations</param>
        /// <exception cref="ArgumentException"><paramref name="units"/> contains time fields, is empty or contains unknown values</exception>
        /// <exception cref="ArgumentException"><paramref name="start"/> and <paramref name="end"/> use different calendars</exception>
        /// <returns>The period between the given dates using the specified period type</returns>
        public static Period Between(LocalDate start, LocalDate end, PeriodUnits units)
        {
            Preconditions.CheckArgument((units & PeriodUnits.AllTimeUnits) == 0, "units", "Units contain time fields: " + units);
            return Between(start.AtMidnight(), end.AtMidnight(), units);
        }

        /// <summary>
        /// Returns the difference between two dates using the "year month day" period type.
        /// </summary>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <returns>The period between the two dates, using year, month and day fields.</returns>
        public static Period Between(LocalDate start, LocalDate end)
        {
            return Between(start.AtMidnight(), end.AtMidnight(), PeriodUnits.YearMonthDay);
        }

        /// <summary>
        /// Returns the period between a start and an end time, using the set of fields in the given
        /// period type.
        /// </summary>
        /// <remarks>
        /// If <paramref name="end"/> is before <paramref name="start" />, each field in the returned period
        /// will be negative. If the given period type cannot exactly reach the end point (e.g. finding
        /// the difference between 3am and 4.30am in hours) the result will be such that adding it to <paramref name="start"/>
        /// will give a value between <paramref name="start"/> and <paramref name="end"/>. In other words,
        /// any rounding is "towards start"; this is true whether the resulting period is negative or positive.
        /// </remarks>
        /// <param name="start">Start time</param>
        /// <param name="end">End time</param>
        /// <param name="units">Units to use for calculations</param>
        /// <exception cref="ArgumentException"><paramref name="units"/> contains date fields, is empty or contains unknown values</exception>
        /// <exception cref="ArgumentException"><paramref name="start"/> and <paramref name="end"/> use different calendars</exception>
        /// <returns>The period between the given times</returns>
        public static Period Between(LocalTime start, LocalTime end, PeriodUnits units)
        {
            Preconditions.CheckArgument((units & PeriodUnits.AllDateUnits) == 0, "units", "Units contain date fields: " + units);
            return Between(start.LocalDateTime, end.LocalDateTime, units);
        }

        /// <summary>
        /// Returns the difference between two dates using the "time" period type.
        /// </summary>
        /// <param name="start">Start time</param>
        /// <param name="end">End time</param>
        /// <returns>The period between the two times, using the "time" period fields.</returns>
        public static Period Between(LocalTime start, LocalTime end)
        {
            return Between(start.LocalDateTime, end.LocalDateTime, PeriodUnits.AllTimeUnits);
        }

        /// <summary>
        /// Returns whether or not this period contains any non-zero-valued time-based units (hours or lower).
        /// The units of this period may include time units, so long as they have zero values.
        /// </summary>
        public bool HasTimeComponent
        {
            get
            {
                // Simple case: there are no time units anyway
                if ((units & PeriodUnits.AllTimeUnits) == 0)
                {
                    return false;
                }
                // Single value case - no need to check unit type, as it must be a time one by now.
                if (values == null)
                {
                    return singleValue != 0;
                }
                // Compound case: just check the time-related indexes
                for (int i = HourIndex; i <= TickIndex; i++)
                {
                    if (values[i] != 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Returns whether or not this period contains any non-zero date-based units (days or higher).
        /// The units of this period may include date units, so long as they have zero values.
        /// </summary>
        public bool HasDateComponent
        {
            get
            {
                // Simple case: there are no date units anyway
                if ((units & PeriodUnits.AllDateUnits) == 0)
                {
                    return false;
                }
                // Single value case - no need to check unit type, as it must be a date one by now.
                if (values == null)
                {
                    return singleValue != 0;
                }
                // Compound case: just check the date-related indexes
                for (int i = YearIndex; i <= DayIndex; i++)
                {
                    if (values[i] != 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// For periods which don't contain months or years, computes the duration assuming a standard
        /// 7-day week, 24-hour day, 60-minute hour etc. The period may contain year or month units,
        /// so long as the values of those components are zero.
        /// </summary>
        /// <exception cref="InvalidOperationException">The month or year value in the period is non-zero.</exception>
        /// <exception cref="OverflowException">The period doesn't have years or months, but the calculation
        /// overflows the bounds of <see cref="Duration"/>. In some cases this may occur even though the theoretical
        /// result would be valid due to balancing positive and negative values, but for simplicity there is
        /// no attempt to work around this - in realistic periods, it shouldn't be a problem.</exception>
        /// <returns>The duration of the period using standard measures.</returns>
        public Duration ToDuration()
        {
            if (Months != 0 || Years != 0)
            {
                throw new InvalidOperationException("Cannot construct duration of period with non-zero months or years.");
            }
            long totalTicks = Ticks +
                Milliseconds * NodaConstants.TicksPerMillisecond +
                Seconds * NodaConstants.TicksPerSecond +
                Minutes * NodaConstants.TicksPerMinute +
                Hours * NodaConstants.TicksPerHour +
                Days * NodaConstants.TicksPerStandardDay +
                Weeks * NodaConstants.TicksPerStandardWeek;
            return Duration.FromTicks(totalTicks);
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
        /// Returns a normalized version of this period.
        /// </summary>
        /// <remarks>
        /// Months and years are unchanged
        /// (as they can vary in length), but weeks are multiplied by 7 and added to the
        /// Days property, and all time fields are normalized to their natural range
        /// (where ticks are "within a millisecond"), adding to larger property where
        /// necessary. So for example, a period of 25 hours becomes a period of 1 day
        /// and 1 hour. Units are also normalized - only non-zero values have their
        /// units retained. Aside from months and years, either all the properties
        /// end up negative, or they all end up positive.
        /// </remarks>
        /// <returns>The normalized period.</returns>
        public Period Normalize()
        {
            // TODO(Post-V1): Consider improving the efficiency of this: return "this" when it's already normalized.
            // Simplest way to normalize: grab all the fields up to "week" and
            // sum them.
            long totalTicks = Ticks +
                Milliseconds * NodaConstants.TicksPerMillisecond +
                Seconds * NodaConstants.TicksPerSecond +
                Minutes * NodaConstants.TicksPerMinute +
                Hours * NodaConstants.TicksPerHour +
                Days * NodaConstants.TicksPerStandardDay +
                Weeks * NodaConstants.TicksPerStandardWeek;
            // TODO(Post-V1): Could use Duration for this...
            long days = totalTicks / NodaConstants.TicksPerStandardDay;
            long hours = (totalTicks / NodaConstants.TicksPerHour) % NodaConstants.HoursPerStandardDay;
            long minutes = (totalTicks / NodaConstants.TicksPerMinute) % NodaConstants.MinutesPerHour;
            long seconds = (totalTicks / NodaConstants.TicksPerSecond) % NodaConstants.SecondsPerMinute;
            long milliseconds = (totalTicks / NodaConstants.TicksPerMillisecond) % NodaConstants.MillisecondsPerSecond;
            long ticks = totalTicks % NodaConstants.TicksPerMillisecond;
            return new PeriodBuilder
            {
                Years = this.Years == 0 ? (long?)null : this.Years,
                Months = this.Months == 0 ? (long?)null : this.Months,
                Days = days == 0 ? (long?)null : days,
                Hours = hours == 0 ? (long?)null : hours,
                Minutes = minutes == 0 ? (long?)null : minutes,
                Seconds = seconds == 0 ? (long?)null : seconds,
                Milliseconds = milliseconds == 0 ? (long?)null : milliseconds,
                Ticks = ticks == 0 ? (long?)null : ticks,
            }.Build();
        }

        /// <summary>
        /// Returns the value of the given field within this period. If the period does not contain
        /// the given field, 0 is returned.
        /// </summary>
        /// <param name="index">The index to fetch the value of.</param>
        /// <returns>The value of the given field within this period, or 0 if this period does not contain the given field.</returns>
        private long this[int index]
        {
            get
            {
                if (values == null)
                {
                    return (int) units == (1 << index) ? singleValue : 0;
                }
                return values[index];
            }
        }

        /// <summary>
        /// Returns the index (0-8 inclusive) for the given single field, or -1 if the value does
        /// not represent a single field.
        /// </summary>
        private static int GetSingleFieldIndex(PeriodUnits units)
        {
            switch (units)
            {
                case PeriodUnits.Years: return 0;
                case PeriodUnits.Months: return 1;
                case PeriodUnits.Weeks: return 2;
                case PeriodUnits.Days: return 3;
                case PeriodUnits.Hours: return 4;
                case PeriodUnits.Minutes: return 5;
                case PeriodUnits.Seconds: return 6;
                case PeriodUnits.Milliseconds: return 7;
                case PeriodUnits.Ticks: return 8;
                default: return -1;
            }
        }

        /// <summary>
        /// Returns the PeriodField for the given index, in the range 0-8 inclusive.
        /// </summary>
        private static PeriodField GetFieldForIndex(FieldSet fields, int index)
        {
            switch (index)
            {
                case 0: return fields.Years;
                case 1: return fields.Months;
                case 2: return fields.Weeks;
                case 3: return fields.Days;
                case 4: return fields.Hours;
                case 5: return fields.Minutes;
                case 6: return fields.Seconds;
                case 7: return fields.Milliseconds;
                case 8: return fields.Ticks;
                default: throw new ArgumentOutOfRangeException("index");
            }
        }

        #region Helper properties
        /// <summary>
        /// Gets the number of years within this period.
        /// </summary>
        public long Years { get { return this[YearIndex]; } }
        /// <summary>
        /// Gets the number of months within this period.
        /// </summary>
        public long Months { get { return this[MonthIndex]; } }
        /// <summary>
        /// Gets the number of weeks within this period.
        /// </summary>
        public long Weeks { get { return this[WeekIndex]; } }
        /// <summary>
        /// Gets the number of days within this period.
        /// </summary>
        public long Days { get { return this[DayIndex]; } }
        /// <summary>
        /// Gets the number of hours within this period.
        /// </summary>
        public long Hours { get { return this[HourIndex]; } }
        /// <summary>
        /// Gets the number of minutes within this period.
        /// </summary>
        public long Minutes { get { return this[MinuteIndex]; } }
        /// <summary>
        /// Gets the number of seconds within this period.
        /// </summary>
        public long Seconds { get { return this[SecondIndex]; } }
        /// <summary>
        /// Gets the number of milliseconds within this period.
        /// </summary>
        public long Milliseconds { get { return this[MillisecondIndex]; } }
        /// <summary>
        /// Gets the number of ticks within this period.
        /// </summary>
        public long Ticks { get { return this[TickIndex]; } }
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
            for (int i = 0; i < ValuesArraySize; i++)
            {
                hash = HashCodeHelper.Hash(hash, this[i]);
            }
            return hash;
        }

        /// <summary>
        /// Compares the given period for equality with this one.
        /// </summary>
        /// <remarks>
        /// Periods are equal if they contain the same values for the same fields, regardless of period type
        /// - so a period of "one hour" is the same whether or not it's potentially got other fields with
        /// a zero value. However, no normalization takes place, so "one hour" is not equal to "sixty minutes".
        /// </remarks>
        /// <param name="other">The period to compare this one with.</param>
        /// <returns>True if this period has the same values for the same fields as the one specified.</returns>
        public bool Equals(Period other)
        {
            if (other == null)
            {
                return false;
            }

            for (int i = 0; i < ValuesArraySize; i++)
            {
                if (this[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

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
