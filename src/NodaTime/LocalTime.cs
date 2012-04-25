#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
using NodaTime.Fields;
using NodaTime.Globalization;
using NodaTime.Text;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// LocalTime is an immutable struct representing a time of day, with no reference
    /// to a particular calendar, time zone or date.
    /// </summary>
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
    public struct LocalTime : IEquatable<LocalTime>, IComparable<LocalTime>, IFormattable
    {
        private static readonly int TypeInitializationChecking = NodaTime.Utility.TypeInitializationChecker.RecordInitializationStart();

        /// <summary>
        /// Local time at midnight, i.e. 0 hours, 0 minutes, 0 seconds.
        /// </summary>
        public static readonly LocalTime Midnight = new LocalTime(0, 0, 0);
        /// <summary>
        /// Local time at noon, i.e. 12 hours, 0 minutes, 0 seconds.
        /// </summary>
        public static readonly LocalTime Noon = new LocalTime(12, 0, 0);

        private static readonly FieldSet IsoFields = CalendarSystem.Iso.Fields;

        private readonly LocalInstant localInstant;

        /// <summary>
        /// Creates a local time at the given hour and minute, with second, millisecond-of-second
        /// and tick-of-millisecond values of zero.
        /// </summary>
        /// <param name="hour">The hour of day.</param>
        /// <param name="minute">The minute of the hour.</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid time</exception>
        /// <returns>The resulting time.</returns>
        public LocalTime(int hour, int minute)
        {
            Preconditions.CheckArgumentRange("hour", hour, 0, NodaConstants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minute", minute, 0, NodaConstants.MinutesPerHour - 1);
            localInstant = new LocalInstant(
                hour * NodaConstants.TicksPerHour +
                minute * NodaConstants.TicksPerMinute);
        }

        /// <summary>
        /// Creates a local time at the given hour, minute and second,
        /// with millisecond-of-second and tick-of-millisecond values of zero.
        /// </summary>
        /// <param name="hour">The hour of day.</param>
        /// <param name="minute">The minute of the hour.</param>
        /// <param name="second">The second of the minute.</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid time</exception>
        /// <returns>The resulting time.</returns>
        public LocalTime(int hour, int minute, int second)
        {
            Preconditions.CheckArgumentRange("hour", hour, 0, NodaConstants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minute", minute, 0, NodaConstants.MinutesPerHour - 1);
            Preconditions.CheckArgumentRange("second", second, 0, NodaConstants.SecondsPerMinute - 1);
            localInstant = new LocalInstant(
                hour * NodaConstants.TicksPerHour +
                minute * NodaConstants.TicksPerMinute +
                second * NodaConstants.TicksPerSecond);
        }

        /// <summary>
        /// Creates a local time at the given hour, minute, second and millisecond,
        /// with a tick-of-millisecond value of zero.
        /// </summary>
        /// <param name="hour">The hour of day.</param>
        /// <param name="minute">The minute of the hour.</param>
        /// <param name="second">The second of the minute.</param>
        /// <param name="millisecond">The millisecond of the second.</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid time</exception>
        /// <returns>The resulting time.</returns>
        public LocalTime(int hour, int minute, int second, int millisecond)
        {
            Preconditions.CheckArgumentRange("hour", hour, 0, NodaConstants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minute", minute, 0, NodaConstants.MinutesPerHour - 1);
            Preconditions.CheckArgumentRange("second", second, 0, NodaConstants.SecondsPerMinute - 1);
            Preconditions.CheckArgumentRange("millisecond", millisecond, 0, NodaConstants.MillisecondsPerSecond - 1);
            localInstant = new LocalInstant(
                hour * NodaConstants.TicksPerHour +
                minute * NodaConstants.TicksPerMinute +
                second * NodaConstants.TicksPerSecond +
                millisecond * NodaConstants.TicksPerMillisecond);
        }

        /// <summary>
        /// Creates a local time at the given hour, minute, second, millisecond and tick within millisecond.
        /// </summary>
        /// <param name="hour">The hour of day.</param>
        /// <param name="minute">The minute of the hour.</param>
        /// <param name="second">The second of the minute.</param>
        /// <param name="millisecond">The millisecond of the second.</param>
        /// <param name="tickWithinMillisecond">The tick within the millisecond.</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid time</exception>
        /// <returns>The resulting time.</returns>
        public LocalTime(int hour, int minute, int second, int millisecond, int tickWithinMillisecond)
        {
            Preconditions.CheckArgumentRange("hour", hour, 0, NodaConstants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minute", minute, 0, NodaConstants.MinutesPerHour - 1);
            Preconditions.CheckArgumentRange("second", second, 0, NodaConstants.SecondsPerMinute - 1);
            Preconditions.CheckArgumentRange("millisecond", millisecond, 0, NodaConstants.MillisecondsPerSecond - 1);
            Preconditions.CheckArgumentRange("tickWithinMillisecond", tickWithinMillisecond, 0, NodaConstants.TicksPerMillisecond - 1);
            localInstant = new LocalInstant(
                hour * NodaConstants.TicksPerHour +
                minute * NodaConstants.TicksPerMinute +
                second * NodaConstants.TicksPerSecond +
                millisecond * NodaConstants.TicksPerMillisecond +
                tickWithinMillisecond);
        }

        /// <summary>
        /// Factory method for creating a local time from the hour of day, minute of hour, second of minute, and tick of second.
        /// This is not a constructor overload as it would have the same signature as the one taking millisecond of second.
        /// </summary>
        /// <param name="hour">The hour of day in the desired time, in the range [0, 23].</param>
        /// <param name="minute">The minute of hour in the desired time, in the range [0, 59].</param>
        /// <param name="second">The second of minute in the desired time, in the range [0, 59].</param>
        /// <param name="tickWithinSecond">The tick within the second in the desired time, in the range [0, 9999999].</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid time</exception>
        /// <returns>The resulting time.</returns>
        public static LocalTime FromHourMinuteSecondTick(int hour, int minute, int second, int tickWithinSecond)
        {
            Preconditions.CheckArgumentRange("hour", hour, 0, NodaConstants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minute", minute, 0, NodaConstants.MinutesPerHour - 1);
            Preconditions.CheckArgumentRange("second", second, 0, NodaConstants.SecondsPerMinute - 1);
            Preconditions.CheckArgumentRange("tickWithinSecond", tickWithinSecond, 0, NodaConstants.TicksPerSecond - 1);
            return new LocalTime(new LocalInstant(
                hour * NodaConstants.TicksPerHour +
                minute * NodaConstants.TicksPerMinute +
                second * NodaConstants.TicksPerSecond +
                tickWithinSecond));
        }

        /// <summary>
        /// Constructor only called from other parts of Noda Time - trusted to be within January 1st 1970 UTC.
        /// </summary>
        internal LocalTime(LocalInstant localInstant)
        {
            this.localInstant = localInstant;
        }

        /// <summary>
        /// Gets the hour of day of this local time, in the range 0 to 23 inclusive.
        /// </summary>
        public int Hour { get { return IsoFields.HourOfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the hour of the half-day of this date and time, in the range 1 to 12 inclusive.
        /// </summary>
        public int ClockHourOfHalfDay { get { return IsoFields.ClockHourOfHalfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the minute of this local time, in the range 0 to 59 inclusive.
        /// </summary>
        public int Minute { get { return IsoFields.MinuteOfHour.GetValue(localInstant); ; } }

        /// <summary>
        /// Gets the second of this local time within the minute, in the range 0 to 59 inclusive.
        /// </summary>
        public int Second { get { return IsoFields.SecondOfMinute.GetValue(localInstant); } }

        /// <summary>
        /// Gets the second of this local time within the day, in the range 0 to 86,399 inclusive.
        /// </summary>
        public int SecondOfDay { get { return IsoFields.SecondOfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the millisecond of this local time within the second, in the range 0 to 999 inclusive.
        /// </summary>
        public int Millisecond { get { return IsoFields.MillisecondOfSecond.GetValue(localInstant); } }

        /// <summary>
        /// Gets the millisecond of this local time within the day, in the range 0 to 86,399,999 inclusive.
        /// </summary>
        public int MillisecondOfDay { get { return IsoFields.MillisecondOfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the tick of this local time within the millisecond, in the range 0 to 9,999 inclusive.
        /// </summary>
        public int Tick { get { return IsoFields.TickOfMillisecond.GetValue(localInstant); } }

        /// <summary>
        /// Gets the tick of this local time within the second, in the range 0 to 9,999,999 inclusive.
        /// </summary>
        public int TickOfSecond { get { return IsoFields.TickOfSecond.GetValue(localInstant); } }

        /// <summary>
        /// Gets the tick of this local time within the day, in the range 0 to 863,999,999,999 inclusive.
        /// </summary>
        // TODO(Post-V1): Consider making the same optimization for the fields above - but this is the one which will
        // benefit most. It's safe because we know the local instant will be on January 1st 1970.
        public long TickOfDay { get { return localInstant.Ticks; } }

        /// <summary>
        /// Returns a LocalDateTime with this local time, on January 1st 1970 in the ISO calendar.
        /// </summary>
        public LocalDateTime LocalDateTime { get { return new LocalDateTime(localInstant); } }

        /// <summary>
        /// Creates a new local time by adding a period to an existing time. The period must not contain
        /// any date-related units (days etc) with non-zero values.
        /// </summary>
        /// <param name="time">The time to add the period to</param>
        /// <param name="period">The period to add</param>
        /// <returns>The result of adding the period to the time, wrapping via midnight if necessary</returns>
        public static LocalTime operator +(LocalTime time, Period period)
        {
            Preconditions.CheckNotNull(period, "period");
            Preconditions.CheckArgument(!period.HasDateComponent, "period", "Cannot add a period with a date component to a time");
            return (time.LocalDateTime + period).TimeOfDay;
        }

        /// <summary>
        /// Adds the specified period to the time. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="time">The time to add the period to</param>
        /// <param name="period">The period to add. Must not contain any (non-zero) date units.</param>
        /// <returns>The sum of the given time and period</returns>
        public static LocalTime Add(LocalTime time, Period period)
        {
            return time + period;
        }

        /// <summary>
        /// Adds the specified period to this time. Fluent alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="period">The period to add. Must not contain any (non-zero) date units.</param>
        /// <returns>The sum of this time and the given period</returns>
        public LocalTime Plus(Period period)
        {
            return this + period;
        }

        /// <summary>
        /// Creates a new local time by subtracting a period from an existing time. The period must not contain
        /// any date-related units (days etc) with non-zero values.
        /// </summary>
        /// <param name="time">The time to subtract the period from</param>
        /// <param name="period">The period to subtract</param>
        /// <returns>The result of subtract the period from the time, wrapping via midnight if necessary</returns>
        public static LocalTime operator -(LocalTime time, Period period)
        {
            Preconditions.CheckNotNull(period, "period");
            Preconditions.CheckArgument(!period.HasDateComponent, "period", "Cannot subtract a period with a date component from a time");
            return (time.LocalDateTime - period).TimeOfDay;
        }

        /// <summary>
        /// Subtracts the specified period from the time. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="time">The time to subtract the period from</param>
        /// <param name="period">The period to subtract. Must not contain any (non-zero) date units.</param>
        /// <returns>The result of subtracting the given period from the time.</returns>
        public static LocalTime Subtract(LocalTime time, Period period)
        {
            return time - period;
        }

        /// <summary>
        /// Subtracts the specified period from this time. Fluent alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="period">The period to subtract. Must not contain any (non-zero) date units.</param>
        /// <returns>The result of subtracting the given period from this time.</returns>
        public LocalTime Minus(Period period)
        {
            return this - period;
        }

        /// <summary>
        /// Compares two local times for equality, by checking whether they represent
        /// the exact same local time, down to the tick.
        /// </summary>
        /// <param name="lhs">The first value to compare</param>
        /// <param name="rhs">The second value to compare</param>
        /// <returns>True if the two times are the same; false otherwise</returns>
        public static bool operator ==(LocalTime lhs, LocalTime rhs)
        {
            return lhs.localInstant == rhs.localInstant;
        }

        /// <summary>
        /// Compares two local times for inequality.
        /// </summary>
        /// <param name="lhs">The first value to compare</param>
        /// <param name="rhs">The second value to compare</param>
        /// <returns>False if the two times are the same; true otherwise</returns>
        public static bool operator !=(LocalTime lhs, LocalTime rhs)
        {
            return lhs.localInstant != rhs.localInstant;
        }

        /// <summary>
        /// Compares two LocalTime values to see if the left one is strictly earlier than the right
        /// one.
        /// </summary>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <returns>true if the <paramref name="lhs"/> is strictly earlier than <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator <(LocalTime lhs, LocalTime rhs)
        {
            return lhs.localInstant < rhs.localInstant;
        }

        /// <summary>
        /// Compares two LocalTime values to see if the left one is earlier than or equal to the right
        /// one.
        /// </summary>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <returns>true if the <paramref name="lhs"/> is earlier than or equal to <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator <=(LocalTime lhs, LocalTime rhs)
        {
            return lhs.localInstant <= rhs.localInstant;
        }

        /// <summary>
        /// Compares two LocalTime values to see if the left one is strictly later than the right
        /// one.
        /// </summary>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <returns>true if the <paramref name="lhs"/> is strictly later than <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator >(LocalTime lhs, LocalTime rhs)
        {
            return lhs.localInstant > rhs.localInstant;
        }

        /// <summary>
        /// Compares two LocalTime values to see if the left one is later than or equal to the right
        /// one.
        /// </summary>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <returns>true if the <paramref name="lhs"/> is later than or equal to <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator >=(LocalTime lhs, LocalTime rhs)
        {
            return lhs.localInstant >= rhs.localInstant;
        }

        /// <summary>
        /// Indicates whether this time is earlier, later or the same as another one.
        /// </summary>
        /// <param name="other">The other date/time to compare this one with</param>
        /// <returns>A value less than zero if this time is earlier than <paramref name="other"/>;
        /// zero if this time is the same as <paramref name="other"/>; a value greater than zero if this time is
        /// later than <paramref name="other"/>.</returns>
        public int CompareTo(LocalTime other)
        {
            return localInstant.CompareTo(other.localInstant);
        }

        /// <summary>
        /// Returns a hash code for this local time.
        /// </summary>
        /// <returns>A hash code for this local time.</returns>
        public override int GetHashCode()
        {
            return localInstant.GetHashCode();
        }

        /// <summary>
        /// Compares this local time with the specified one for equality,
        /// by checking whether the two values represent the exact same local time, down to the tick.
        /// </summary>
        /// <param name="other">The other local time to compare this one with</param>
        /// <returns>True if the specified time is equal to this one; false otherwise</returns>
        public bool Equals(LocalTime other)
        {
            return this == other;
        }

        /// <summary>
        /// Compares this local time with the specified reference. A local time is
        /// only equal to another local time with the same underlying tick value.
        /// </summary>
        /// <param name="obj">The object to compare this one with</param>
        /// <returns>True if the specified value is a local time is equal to this one; false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is LocalTime))
            {
                return false;
            }
            return this == (LocalTime)obj;
        }

        /// <summary>
        /// Returns a new LocalTime representing the current value with the given number of hours added.
        /// </summary>
        /// <remarks>
        /// If the value goes past the start or end of the day, it wraps - so 11pm plus two hours is 1am, for example.
        /// </remarks>
        /// <param name="hours">The number of hours to add</param>
        /// <returns>The current value plus the given number of hours.</returns>
        public LocalTime PlusHours(long hours)
        {
            return LocalDateTime.PlusHours(hours).TimeOfDay;
        }

        /// <summary>
        /// Returns a new LocalTime representing the current value with the given number of minutes added.
        /// </summary>
        /// <remarks>
        /// If the value goes past the start or end of the day, it wraps - so 11pm plus 120 minutes is 1am, for example.
        /// </remarks>
        /// <param name="minutes">The number of minutes to add</param>
        /// <returns>The current value plus the given number of minutes.</returns>
        public LocalTime PlusMinutes(long minutes)
        {
            return LocalDateTime.PlusMinutes(minutes).TimeOfDay;
        }

        /// <summary>
        /// Returns a new LocalTime representing the current value with the given number of seconds added.
        /// </summary>
        /// <remarks>
        /// If the value goes past the start or end of the day, it wraps - so 11:59pm plus 120 seconds is 12:01am, for example.
        /// </remarks>
        /// <param name="seconds">The number of seconds to add</param>
        /// <returns>The current value plus the given number of seconds.</returns>
        public LocalTime PlusSeconds(long seconds)
        {
            return LocalDateTime.PlusSeconds(seconds).TimeOfDay;
        }

        /// <summary>
        /// Returns a new LocalTime representing the current value with the given number of seconds added.
        /// </summary>
        /// <param name="milliseconds">The number of seconds to add</param>
        /// <returns>The current value plus the given number of seconds.</returns>
        public LocalTime PlusMilliseconds(long milliseconds)
        {
            return LocalDateTime.PlusMilliseconds(milliseconds).TimeOfDay;
        }

        /// <summary>
        /// Returns a new LocalTime representing the current value with the given number of ticks added.
        /// </summary>
        /// <param name="ticks">The number of ticks to add</param>
        /// <returns>The current value plus the given number of seconds.</returns>
        public LocalTime PlusTicks(long ticks)
        {
            return LocalDateTime.PlusTicks(ticks).TimeOfDay;
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
            return LocalTimePattern.BclSupport.Format(this, null, NodaFormatInfo.CurrentInfo);
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
            return LocalTimePattern.BclSupport.Format(this, patternText, NodaFormatInfo.CurrentInfo);
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
            return LocalTimePattern.BclSupport.Format(this, null, NodaFormatInfo.GetInstance(formatProvider));
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
            return LocalTimePattern.BclSupport.Format(this, patternText, NodaFormatInfo.GetInstance(formatProvider));
        }
        #endregion Formatting

        #region Parsing
        /// <summary>
        /// Parses the given string using the current culture's default format provider.
        /// </summary>
        /// <param name="value">The text to parse.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <returns>The parsed value.</returns>
        public static LocalTime Parse(string value)
        {
            return LocalTimePattern.BclSupport.Parse(value, NodaFormatInfo.CurrentInfo);
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
        public static LocalTime Parse(string value, IFormatProvider formatProvider)
        {
            return LocalTimePattern.BclSupport.Parse(value, NodaFormatInfo.GetInstance(formatProvider));
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
        public static LocalTime ParseExact(string value, string patternText, IFormatProvider formatProvider)
        {
            return LocalTimePattern.BclSupport.ParseExact(value, patternText, NodaFormatInfo.GetInstance(formatProvider));
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
        public static LocalTime ParseExact(string value, string[] patterns, IFormatProvider formatProvider)
        {
            return LocalTimePattern.BclSupport.ParseExact(value, patterns, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        /// Attempts to parse the given string using the current culture's default format provider. If the parse is successful,
        /// the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Instant.MinValue"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <param name="value">The text to parse.</param>
        /// <param name="result">The parsed value, when successful.</param>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParse(string value, out LocalTime result)
        {
            return LocalTimePattern.BclSupport.TryParse(value, NodaFormatInfo.CurrentInfo, out result);
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
        public static bool TryParse(string value, IFormatProvider formatProvider, out LocalTime result)
        {
            return LocalTimePattern.BclSupport.TryParse(value, NodaFormatInfo.GetInstance(formatProvider), out result);
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
        public static bool TryParseExact(string value, string patternText, IFormatProvider formatProvider, out LocalTime result)
        {
            return LocalTimePattern.BclSupport.TryParseExact(value, patternText, NodaFormatInfo.GetInstance(formatProvider), out result);
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
        public static bool TryParseExact(string value, string[] patterns, IFormatProvider formatProvider, out LocalTime result)
        {
            return LocalTimePattern.BclSupport.TryParseExact(value, patterns, NodaFormatInfo.GetInstance(formatProvider), out result);
        }
        #endregion Parsing

    }
}
