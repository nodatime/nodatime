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
using NodaTime.Calendars;
using NodaTime.Fields;

namespace NodaTime
{
    /// <summary>
    /// CalendarSystem provides a skeleton implementation for CalendarSystem.
    /// Many utility methods are defined, but all fields are unsupported.
    /// <para>
    /// CalendarSystem is thread-safe and immutable, and all subclasses must be
    /// as well.
    /// </para>
    /// </summary>
    public abstract class CalendarSystem
    {
        /// <summary>
        /// Returns a calendar system that follows the rules of the ISO8601 standard,
        /// which is compatible with Gregorian for all modern dates.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When ISO does not define a field, but it can be determined (such as AM/PM) it is included.
        /// </para>
        /// <para>
        /// With the exception of century related fields, the ISO calendar is exactly the
        /// same as the Gregorian calendar system. In this system, centuries and year
        /// of century are zero based. For all years, the century is determined by
        /// dropping the last two digits of the year, ignoring sign. The year of century
        /// is the value of the last two year digits.
        /// </para>
        /// </remarks>
        public static readonly CalendarSystem Iso = new IsoCalendarSystem(GregorianCalendarSystem.Default);

        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarSystem"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected CalendarSystem(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets the name of this calendar system. Each calendar system must have a unique name.
        /// </summary>
        /// <value>The calendar system name.</value>
        public string Name { get { return name; } }

        /// <summary>
        /// Returns whether the day-of-week field refers to ISO days. If true, types such as LocalDateTime
        /// can use the IsoDayOfWeek property to avoid using magic numbers. This defaults to true, but can be
        /// overridden by specific calendars.
        /// </summary>
        public virtual bool UsesIsoDayOfWeek { get { return true; } }

        internal abstract FieldSet Fields { get; }

        /// <summary>
        /// Returns a local instant, formed from the given year, month, day, and ticks values.
        /// The set of given values must refer to a valid datetime, or else an IllegalArgumentException is thrown.
        /// <para>
        /// The default implementation calls upon separate DateTimeFields to
        /// determine the result. Subclasses are encouraged to provide a more
        /// efficient implementation.
        /// </para>
        /// </summary>
        /// <param name="year">Year to use</param>
        /// <param name="monthOfYear">Month to use</param>
        /// <param name="dayOfMonth">Day of month to use</param>
        /// <param name="tickOfDay">Tick of day to use</param>
        /// <returns>A LocalInstant instance</returns>
        internal virtual LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, long tickOfDay)
        {
            LocalInstant instant = Fields.Year.SetValue(LocalInstant.LocalUnixEpoch, year);
            instant = Fields.MonthOfYear.SetValue(instant, monthOfYear);
            instant = Fields.DayOfMonth.SetValue(instant, dayOfMonth);
            return Fields.TickOfDay.SetValue(instant, tickOfDay);
        }

        /// <summary>
        /// Returns a local instant, formed from the given year, month, day, 
        /// hour, minute, second, millisecond and ticks values.
        /// </summary>
        /// <para>
        /// The default implementation calls upon separate DateTimeFields to
        /// determine the result. Subclasses are encouraged to provide a more
        /// efficient implementation.
        /// </para>        
        /// <param name="year">Year to use</param>
        /// <param name="monthOfYear">Month to use</param>
        /// <param name="dayOfMonth">Day of month to use</param>
        /// <param name="hourOfDay">Hour to use</param>
        /// <param name="minuteOfHour">Minute to use</param>
        /// <param name="secondOfMinute">Second to use</param>
        /// <param name="millisecondOfSecond">Millisecond to use</param>
        /// <param name="tickOfMillisecond">Tick to use</param>
        /// <returns>A LocalInstant instance</returns>
        internal virtual LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute,
                                                      int millisecondOfSecond, int tickOfMillisecond)
        {
            LocalInstant instant = Fields.Year.SetValue(LocalInstant.LocalUnixEpoch, year);
            instant = Fields.MonthOfYear.SetValue(instant, monthOfYear);
            instant = Fields.DayOfMonth.SetValue(instant, dayOfMonth);
            instant = Fields.HourOfDay.SetValue(instant, hourOfDay);
            instant = Fields.MinuteOfHour.SetValue(instant, minuteOfHour);
            instant = Fields.SecondOfMinute.SetValue(instant, secondOfMinute);
            instant = Fields.MillisecondOfSecond.SetValue(instant, millisecondOfSecond);
            return Fields.TickOfMillisecond.SetValue(instant, tickOfMillisecond);
        }

        /// <summary>
        /// Returns a local instant, formed from the given instant, hour, minute, second, millisecond and ticks values.
        /// <para>
        /// The default implementation calls upon separate DateTimeFields to
        /// determine the result. Subclasses are encouraged to provide a more
        /// efficient implementation.
        /// </para>       
        /// </summary>
        /// <param name="localInstant">Instant to start from</param>
        /// <param name="hourOfDay">Hour to use</param>
        /// <param name="minuteOfHour">Minute to use</param>
        /// <param name="secondOfMinute">Second to use</param>
        /// <param name="millisecondOfSecond">Milliscond to use</param>
        /// <param name="tickOfMillisecond">Tick to use</param>
        /// <returns>A LocalInstant instance</returns>
        internal virtual LocalInstant GetLocalInstant(LocalInstant localInstant, int hourOfDay, int minuteOfHour, int secondOfMinute, int millisecondOfSecond,
                                                      int tickOfMillisecond)
        {
            localInstant = Fields.HourOfDay.SetValue(localInstant, hourOfDay);
            localInstant = Fields.MinuteOfHour.SetValue(localInstant, minuteOfHour);
            localInstant = Fields.SecondOfMinute.SetValue(localInstant, secondOfMinute);
            localInstant = Fields.MillisecondOfSecond.SetValue(localInstant, millisecondOfSecond);
            return Fields.TickOfMillisecond.SetValue(localInstant, tickOfMillisecond);
        }

        internal virtual LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute)
        {
            return GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute, 0, 0);
        }

        internal virtual LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour)
        {
            return GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, 0, 0, 0);
        }

        /// <summary>
        /// Returns the logical combination of this calendar system with the specified time zone.
        /// </summary>
        /// <remarks>
        /// Logically, this is equivalent to just calling the Chronology constructor specifying
        /// the calendar system and the given time zone. However, in the common case of the ISO
        /// calendar, we can optimize by caching an ISO chronology with each time zone: the override
        /// of this method in the ISO calendar system uses this cached representation, avoiding creating
        /// objects unnecessarily.
        /// </remarks>
        public virtual Chronology WithZone(DateTimeZone zone)
        {
            if (zone == null)
            {
                throw new ArgumentException("zone");
            }
            return new Chronology(zone, this);
        }

        #region Periods
        /// <summary>
        /// Gets the values of a period type from an interval.
        /// </summary>
        /// <param name="start">The start instant of an interval to query</param>
        /// <param name="end">The end instant of an interval to query</param>
        /// <param name="periodType">The period type to use</param>
        /// <returns>The values of the period extracted from the interval</returns>
        internal long[] GetPeriodValues(LocalInstant start, LocalInstant end, PeriodType periodType)
        {
            int size = periodType.Size;
            long[] values = new long[size];

            if (start == end)
            {
                return values;
            }

            LocalInstant result = start;
            for (int i = 0; i < size; i++)
            {
                DurationField field = GetField(periodType[i]);
                long value = field.GetInt64Difference(end, result);
                values[i] = value;

                result = field.Add(result, value);
            }
            return values;
        }

        internal LocalInstant Add(Period period, LocalInstant localInstant, int scalar)
        {
            if (period == null)
            {
                throw new ArgumentNullException("period");
            }
            if (scalar == 0)
            {
                return localInstant;
            }

            LocalInstant result = localInstant;
            foreach (DurationFieldValue fieldValue in period)
            {
                if (fieldValue.Value != 0)
                {
                    result = GetField(fieldValue.FieldType).Add(result, fieldValue.Value * scalar);
                }
            }
            return result;
        }

        private DurationField GetField(DurationFieldType fieldType)
        {
            switch (fieldType)
            {
                case DurationFieldType.Eras:
                    return Fields.Eras;
                case DurationFieldType.Centuries:
                    return Fields.Centuries;
                case DurationFieldType.WeekYears:
                    return Fields.WeekYears;
                case DurationFieldType.Years:
                    return Fields.Years;
                case DurationFieldType.Months:
                    return Fields.Months;
                case DurationFieldType.Weeks:
                    return Fields.Weeks;
                case DurationFieldType.Days:
                    return Fields.Days;
                case DurationFieldType.HalfDays:
                    return Fields.HalfDays;
                case DurationFieldType.Hours:
                    return Fields.Hours;
                case DurationFieldType.Minutes:
                    return Fields.Minutes;
                case DurationFieldType.Seconds:
                    return Fields.Seconds;
                case DurationFieldType.Milliseconds:
                    return Fields.Milliseconds;
                case DurationFieldType.Ticks:
                    return Fields.Ticks;
                default:
                    throw new InvalidOperationException();
            }
        }
        #endregion

        #region object overrides
        public override string ToString()
        {
            return Name;
        }
        #endregion

        /// <summary>
        /// Returns the IsoDayOfWeek corresponding to the day of week for the given local instant
        /// if this calendar uses ISO days of the week, or throws an InvalidOperationException otherwise.
        /// </summary>
        /// <param name="localInstant">The local instant to use to find the day of the week</param>
        /// <returns>The day of the week as an IsoDayOfWeek</returns>
        internal IsoDayOfWeek GetIsoDayOfWeek(LocalInstant localInstant)
        {
            if (!UsesIsoDayOfWeek)
            {
                throw new InvalidOperationException("Calendar " + Name + " does not use ISO days of the week");
            }
            return (IsoDayOfWeek)Fields.DayOfWeek.GetValue(localInstant);
        }
    }
}