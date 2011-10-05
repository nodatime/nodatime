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
using NodaTime.Fields;
using NodaTime.Globalization;
using NodaTime.Text.Patterns;

namespace NodaTime
{
    /// <summary>
    /// LocalTime is an immutable struct representing a time of day, with no reference
    /// to a particular calendar, time zone or date.
    /// </summary>
    public struct LocalTime : IEquatable<LocalTime>
    {
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
        /// Creates a local time at the given hour, minute and second,
        /// with millisecond-of-second and tick-of-millisecond values of zero.
        /// </summary>
        /// <param name="hour">The hour of day.</param>
        /// <param name="minute">The minute of the hour.</param>
        /// <param name="second">The second of the minute.</param>
        public LocalTime(int hour, int minute, int second)
        {
            FieldUtils.VerifyValueBounds(DateTimeFieldType.HourOfDay, hour, 0, NodaConstants.HoursPerStandardDay - 1);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.MinuteOfHour, minute, 0, NodaConstants.MinutesPerHour - 1);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.SecondOfMinute, second, 0, NodaConstants.SecondsPerMinute - 1);
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
        public LocalTime(int hour, int minute, int second, int millisecond)
        {
            FieldUtils.VerifyValueBounds(DateTimeFieldType.HourOfDay, hour, 0, NodaConstants.HoursPerStandardDay - 1);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.MinuteOfHour, minute, 0, NodaConstants.MinutesPerHour - 1);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.SecondOfMinute, second, 0, NodaConstants.SecondsPerMinute - 1);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.MillisecondOfSecond, millisecond, 0, NodaConstants.MillisecondsPerSecond - 1);
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
        public LocalTime(int hour, int minute, int second, int millisecond, int tickWithinMillisecond)
        {
            FieldUtils.VerifyValueBounds(DateTimeFieldType.HourOfDay, hour, 0, NodaConstants.HoursPerStandardDay - 1);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.MinuteOfHour, minute, 0, NodaConstants.MinutesPerHour - 1);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.SecondOfMinute, second, 0, NodaConstants.SecondsPerMinute - 1);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.MillisecondOfSecond, millisecond, 0, NodaConstants.MillisecondsPerSecond - 1);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.TickOfMillisecond, tickWithinMillisecond, 0, NodaConstants.TicksPerMillisecond - 1);
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
        /// <returns>The resulting time.</returns>
        public static LocalTime FromHourMinuteSecondTick(int hour, int minute, int second, int tickWithinSecond)
        {
            FieldUtils.VerifyValueBounds(DateTimeFieldType.HourOfDay, hour, 0, NodaConstants.HoursPerStandardDay - 1);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.MinuteOfHour, minute, 0, NodaConstants.MinutesPerHour - 1);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.SecondOfMinute, second, 0, NodaConstants.SecondsPerMinute - 1);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.TickOfSecond, tickWithinSecond, 0, NodaConstants.TicksPerSecond - 1);
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
        public int HourOfDay { get { return IsoFields.HourOfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the hour of the half-day of this date and time, in the range 1 to 12 inclusive.
        /// </summary>
        public int ClockHourOfHalfDay { get { return IsoFields.ClockHourOfHalfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the minute of this local time, in the range 0 to 59 inclusive.
        /// </summary>
        public int MinuteOfHour { get { return IsoFields.MinuteOfHour.GetValue(localInstant); ; } }

        /// <summary>
        /// Gets the second of this local time within the minute, in the range 0 to 59 inclusive.
        /// </summary>
        public int SecondOfMinute { get { return IsoFields.SecondOfMinute.GetValue(localInstant); } }

        /// <summary>
        /// Gets the second of this local time within the day, in the range 0 to 86,399 inclusive.
        /// </summary>
        public int SecondOfDay { get { return IsoFields.SecondOfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the millisecond of this local time within the second, in the range 0 to 999 inclusive.
        /// </summary>
        public int MillisecondOfSecond { get { return IsoFields.MillisecondOfSecond.GetValue(localInstant); } }

        /// <summary>
        /// Gets the millisecond of this local time within the day, in the range 0 to 86,399,999 inclusive.
        /// </summary>
        public int MillisecondOfDay { get { return IsoFields.MillisecondOfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the tick of this local time within the millisecond, in the range 0 to 9,999 inclusive.
        /// </summary>
        public int TickOfMillisecond { get { return IsoFields.TickOfMillisecond.GetValue(localInstant); } }

        /// <summary>
        /// Gets the tick of this local time within the second, in the range 0 to 9,999,999 inclusive.
        /// </summary>
        public int TickOfSecond { get { return IsoFields.TickOfSecond.GetValue(localInstant); } }

        /// <summary>
        /// Gets the tick of this local time within the day, in the range 0 to 863,999,999,999 inclusive.
        /// </summary>
        public long TickOfDay { get { return IsoFields.TickOfDay.GetInt64Value(localInstant); ; } }

        /// <summary>
        /// Returns a LocalDateTime with this local time, on January 1st 1970 in the ISO calendar.
        /// </summary>
        public LocalDateTime LocalDateTime { get { return new LocalDateTime(localInstant); } }

        // TODO: Assert no units as large a day
        /// <summary>
        /// Creates a new local time by adding a period to an existing time. The period must not contain
        /// any fields as large as a day or larger.
        /// </summary>
        /// <param name="time">The time to add the period to</param>
        /// <param name="period">The period to add</param>
        /// <returns>The result of adding the period to the time, wrapping via midnight if necessary</returns>
        public static LocalTime operator +(LocalTime time, Period period)
        {
            return (time.LocalDateTime + period).TimeOfDay;
        }

        // TODO: Assert no units as large as a day
        /// <summary>
        /// Creates a new local time by subtracting a period from an existing time. The period must not contain
        /// any fields as large as a day or larger.
        /// </summary>
        /// <param name="time">The time to subtract the period from</param>
        /// <param name="period">The period to subtract</param>
        /// <returns>The result of subtract the period from the time, wrapping via midnight if necessary</returns>
        public static LocalTime operator -(LocalTime time, Period period)
        {
            return (time.LocalDateTime - period).TimeOfDay;
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
        ///   Formats the value of the current instance using the specified format.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="patternText">The <see cref="T:System.String" /> specifying the pattern to use.
        ///   -or- 
        ///   null to use the default pattern defined for the type of the <see cref="T:System.IFormattable" /> implementation. 
        /// </param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use to format the value.
        ///   -or- 
        ///   null to obtain the numeric format information from the current locale setting of the operating system. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string patternText, IFormatProvider formatProvider)
        {
            return LocalTimePattern.Format(this, patternText, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return LocalTimePattern.Format(this, null, NodaFormatInfo.CurrentInfo);
        }

        /// <summary>
        ///   Formats the value of the current instance using the specified format.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="patternText">The <see cref="T:System.String" /> specifying the pattern to use.
        ///   -or- 
        ///   null to use the default pattern defined for the type of the <see cref="T:System.IFormattable" /> implementation. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string patternText)
        {
            return LocalTimePattern.Format(this, patternText, NodaFormatInfo.CurrentInfo);
        }

        /// <summary>
        ///   Formats the value of the current instance using the specified format.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use to format the value.
        ///   -or- 
        ///   null to obtain the format information from the current locale setting of the operating system. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(IFormatProvider formatProvider)
        {
            return LocalTimePattern.Format(this, null, NodaFormatInfo.GetInstance(formatProvider));
        }
        #endregion Formatting

        #region Parsing
        private static readonly string[] AllPatterns = { "T", "t", "r" }; // Long, short, round-trip
        private const string DefaultFormatPattern = "T"; // Long

        private static readonly PatternBclSupport<LocalTime> LocalTimePattern = new PatternBclSupport<LocalTime>(AllPatterns, DefaultFormatPattern, LocalTime.Midnight, fi => fi.LocalTimePatternParser);
        /// <summary>
        /// Parses the given string using the current culture's default format provider.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed local time.</returns>
        public static LocalTime Parse(string value)
        {
            return LocalTimePattern.Parse(value, NodaFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Parses the given string using the specified format provider.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="formatProvider">The format provider to use for culture-specific settings.</param>
        /// <returns>The parsed local time.</returns>
        public static LocalTime Parse(string value, IFormatProvider formatProvider)
        {
            return LocalTimePattern.Parse(value, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        /// Parses the given string using the specified format pattern and format provider.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="patternText">The text of the pattern to use for parsing.</param>
        /// <param name="formatProvider">The format provider to use for culture-specific settings.</param>
        /// <returns>The parsed local time.</returns>
        public static LocalTime ParseExact(string value, string patternText, IFormatProvider formatProvider)
        {
            return LocalTimePattern.ParseExact(value, patternText, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        /// Parses the given string using the specified patterns and format provider.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="patterns">The patterns to use for parsing.</param>
        /// <param name="formatProvider">The format provider to use for culture-specific settings.</param>
        /// <returns>The parsed local time.</returns>
        public static LocalTime ParseExact(string value, string[] patterns, IFormatProvider formatProvider)
        {
            return LocalTimePattern.ParseExact(value, patterns, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        /// Attempts to parse the given string using the current culture's default format provider. If the parse is successful,
        /// the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="LocalTime.Midnight"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="result">The parsed local time, when successful.</param>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParse(string value, out LocalTime result)
        {
            return LocalTimePattern.TryParse(value, NodaFormatInfo.CurrentInfo, out result);
        }

        /// <summary>
        /// Attempts to parse the given string using the specified format provider.
        /// If the parse is successful, the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="LocalTime.Midnight"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="formatProvider">The format provider to use for culture-specific settings.</param>
        /// <param name="result">The parsed local time, when successful.</param>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParse(string value, IFormatProvider formatProvider, out LocalTime result)
        {
            return LocalTimePattern.TryParse(value, NodaFormatInfo.GetInstance(formatProvider), out result);
        }

        /// <summary>
        /// Attempts to parse the given string using the specified pattern, format provider and style.
        /// If the parse is successful, the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="LocalTime.Midnight"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="patternText">The text of the pattern to use for parsing.</param>
        /// <param name="formatProvider">The format provider to use for culture-specific settings.</param>
        /// <param name="result">The parsed local time, when successful.</param>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParseExact(string value, string patternText, IFormatProvider formatProvider, out LocalTime result)
        {
            return LocalTimePattern.TryParseExact(value, patternText, NodaFormatInfo.GetInstance(formatProvider), out result);
        }

        /// <summary>
        /// Attempts to parse the given string using the specified patterns and format provider.
        /// If the parse is successful, the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="LocalTime.Midnight"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="patterns">The patterns to use for parsing.</param>
        /// <param name="formatProvider">The format provider to use for culture-specific settings.</param>
        /// <param name="result">The parsed local time, when successful.</param>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParseExact(string value, string[] patterns, IFormatProvider formatProvider, out LocalTime result)
        {
            return LocalTimePattern.TryParseExact(value, patterns, NodaFormatInfo.GetInstance(formatProvider), out result);
        }
        #endregion Parsing

    }
}
