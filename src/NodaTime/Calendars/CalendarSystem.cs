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
using NodaTime.Periods;

namespace NodaTime.Calendars
{
    // TODO: Move to NodaTime namespace now that users only need to know about CalendarSystem?
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

        #region Periods
        /// <summary>
        /// Gets the values of a period type from an interval.
        /// </summary>
        /// <param name="periodType">The period type to use</param>
        /// <param name="start">The start instant of an interval to query</param>
        /// <param name="end">The end instant of an interval to query</param>
        /// <returns>The values of the period extracted from the interval</returns>
        internal int[] GetPeriodValues(PeriodType periodType, LocalInstant start, LocalInstant end)
        {
            int size = periodType.Size;
            int[] values = new int[size];

            if (start == end)
            {
                return values;
            }

            LocalInstant result = start;
            for (int i = 0; i < size; i++)
            {
                DurationField field = GetField(periodType.GetFieldType(i));
                int value = field.GetDifference(end, result);
                values[i] = value;

                result = field.Add(result, value);
            }
            return values;
        }

        /// <summary>
        /// Adds the period to the instant, specifying the number of times to add.
        /// </summary>
        /// <param name="period">The period to add, null means add nothing</param>
        /// <param name="instant">The instant to add to</param>
        /// <param name="scalar">The number of times to add</param>
        /// <returns>The updated instant</returns>
        internal LocalInstant Add(IPeriod period, LocalInstant instant, int scalar)
        {
            LocalInstant result = instant;

            if (scalar != 0 && period != null)
            {
                for (int i = 0, isize = period.Size; i < isize; i++)
                {
                    long value = period[i]; // use long to allow for multiplication (fits OK)
                    if (value != 0)
                    {
                        result = GetField(period.GetFieldType(i)).Add(result, value * scalar);
                    }
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
                default:
                    throw new InvalidOperationException();
            }
        }
        #endregion

        #region Partials
        /// <summary>
        /// Validates whether the values are valid for the fields of a partial instant.
        /// </summary>
        /// <param name="partial">The partial instant to validate</param>
        /// <param name="values">The values to validate, not null, match fields in partial</param>
        internal void Validate(IPartial partial, int[] values)
        {
            int size = partial.Size;

            // check values in standard range, catching really stupid cases like -1
            // this means that the second check will not hit trouble
            for (int i = 0; i < size; i++)
            {
                int value = values[i];
                DateTimeField field = partial.GetField(i);
                if (value < field.GetMinimumValue())
                {
                    throw new FieldValueException(field.FieldType, value, field.GetMinimumValue(), null);
                }
                if (value > field.GetMaximumValue())
                {
                    throw new FieldValueException(field.FieldType, value, null, field.GetMaximumValue());
                }
            }

            // check values in specific range, catching really odd cases like 30th Feb
            for (int i = 0; i < size; i++)
            {
                int value = values[i];
                DateTimeField field = partial.GetField(i);
                if (value < field.GetMinimumValue(partial, values))
                {
                    throw new FieldValueException(field.FieldType, value, field.GetMinimumValue(partial, values), null);
                }
                if (value > field.GetMaximumValue(partial, values))
                {
                    throw new FieldValueException(field.FieldType, value, null, field.GetMaximumValue(partial, values));
                }
            }
        }

        /// <summary>
        /// Gets the values of a partial from an instant.
        /// </summary>
        /// <param name="partial">The partial instant to use</param>
        /// <param name="instant">The instant to query</param>
        /// <returns>The values of this partial extracted from the instant</returns>
        internal int[] GetPartialValues(IPartial partial, LocalInstant instant)
        {
            int size = partial.Size;
            int[] values = new int[size];
            for (int i = 0; i < size; i++)
            {
                values[i] = partial.GetFieldType(i).GetField(this).GetValue(instant);
            }
            return values;
        }

        /// <summary>
        /// Sets the values from the partial within an existing local instant.
        /// </summary>
        /// <param name="partial">The partial instant to use</param>
        /// <param name="localInstant">The instant to update</param>
        /// <returns>The updated instant</returns>
        internal LocalInstant SetPartial(IPartial partial, LocalInstant localInstant)
        {
            for (int i = 0, isize = partial.Size; i < isize; i++)
            {
                localInstant = partial.GetFieldType(i).GetField(this).SetValue(localInstant, partial.GetValue(i));
            }
            return localInstant;
        }
        #endregion

        #region object overrides
        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}