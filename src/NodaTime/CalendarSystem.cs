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
    /// <remarks>
    /// This class roughly corresponds to Chronology in Joda Time, although it includes
    /// the functionality of Chronology, BaseChronology and AssembledChronology all mashed
    /// together. Unlike Chronology, there's no time zone handling in CalendarSystem - that's
    /// treated entirely separately to calendaring.
    /// </remarks>
    public abstract class CalendarSystem
    {
        /// <summary>
        /// Delegate used to construct fields. This is called within the base constructor, before the
        /// derived class constructor bodies have been run - so it's *somewhat* unsafe to pass "this"
        /// reference, but derived classes will just need to be careful.
        /// </summary>
        internal delegate void FieldAssembler(FieldSet.Builder builder, CalendarSystem @this);

        // TODO: Consider moving the static accessors into a separate class. As we get more calendars,
        // this approach will become unwieldy.

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
        public static CalendarSystem Iso { get { return IsoCalendarSystem.Instance; } }

        /// <summary>
        /// Returns a pure proleptic Gregorian calendar system, which defines every
        /// fourth year as leap, unless the year is divisible by 100 and not by 400.
        /// This improves upon the Julian calendar leap year rule.
        /// </summary>
        /// <remarks>
        /// Although the Gregorian calendar did not exist before 1582 CE, this
        /// chronology assumes it did, thus it is proleptic. This implementation also
        /// fixes the start of the year at January 1, and defines the year zero.
        /// </remarks>
        /// <param name="minDaysInFirstWeek">The minimum number of days in the first week of the year.
        /// When computing the WeekOfWeekYear and WeekYear properties of a particular date, this is
        /// used to decide at what point the week year changes.</param>
        public static CalendarSystem GetGregorianCalendar(int minDaysInFirstWeek)
        {
            return GregorianCalendarSystem.GetInstance(minDaysInFirstWeek);
        }

        /// <summary>
        /// Returns a pure proleptic Julian calendar system, which defines every
        /// fourth year as leap. This implementation follows the leap year rule
        /// strictly, even for dates before 8 CE, where leap years were actually
        /// irregular. In the Julian calendar, year zero does not exist: 1 BCE is
        /// followed by 1 CE.
        /// </summary>
        /// <remarks>
        /// Although the Julian calendar did not exist before 45 BCE, this chronology
        /// assumes it did, thus it is proleptic. This implementation also fixes the
        /// start of the year at January 1.
        /// </remarks>
        /// <param name="minDaysInFirstWeek">The minimum number of days in the first week of the year.
        /// When computing the WeekOfWeekYear and WeekYear properties of a particular date, this is
        /// used to decide at what point the week year changes.</param>
        public static CalendarSystem GetJulianCalendar(int minDaysInFirstWeek)
        {
            return JulianCalendarSystem.GetInstance(minDaysInFirstWeek);
        }

        /*//  * Implements the Islamic, or Hijri, calendar system using arithmetic rules.
 * <p>
 * This calendar is a lunar calendar with a shorter year than ISO.
 * Year 1 in the Islamic calendar began on July 16, 622 CE (Julian), thus
 * Islamic years do not begin at the same time as Julian years. This chronology
 * is not proleptic, as it does not allow dates before the first Islamic year.
 * <p>
 * There are two basic forms of the Islamic calendar, the tabular and the
 * observed. The observed form cannot easily be used by computers as it
 * relies on human observation of the new moon.
 * The tabular calendar, implemented here, is an arithmetical approximation
 * of the observed form that follows relatively simple rules.
 * <p>
 * The tabular form of the calendar defines 12 months of alternately
 * 30 and 29 days. The last month is extended to 30 days in a leap year.
 * Leap years occur according to a 30 year cycle. There are four recognised
 * patterns of leap years in the 30 year cycle:
 * <pre>
 * Years 2, 5, 7, 10, 13, 15, 18, 21, 24, 26 & 29 - 15-based, used by Microsoft
 * Years 2, 5, 7, 10, 13, 16, 18, 21, 24, 26 & 29 - 16-based, most commonly used
 * Years 2, 5, 8, 10, 13, 16, 19, 21, 24, 27 & 29 - Indian
 * Years 2, 5, 8, 11, 13, 16, 19, 21, 24, 27 & 30 - Habash al-Hasib
 * </pre>
 * You can select which pattern to use via the factory methods, or use the
 * default (16-based).
 * <p>
 * This implementation defines a day as midnight to midnight exactly as per
 * the ISO chronology. This correct start of day is at sunset on the previous
 * day, however this cannot readily be modelled and has been ignored. */
        /// <summary>
        /// Returns an Islamic, or Hijri, calendar system.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This represents the civil Hijri calendar, rather than the religious one based
        /// on lunar observation. This calendar is a lunar calendar with a shorter year than ISO.
        /// Year 1 in the Islamic calendar began on July 15th or 16th, 622 CE (Julian), thus
        /// Islamic years do not begin at the same time as Julian years. This chronology
        /// is not proleptic, as it does not allow dates before the first Islamic year.
        /// </para>
        /// <para>
        /// There are two basic forms of the Islamic calendar, the tabular and the
        /// observed. The observed form cannot easily be used by computers as it
        /// relies on human observation of the new moon. The tabular calendar, implemented here, is an
        /// arithmetic approximation of the observed form that follows relatively simple rules.
        /// </para>
        /// <para>
        /// The tabular form of the calendar defines 12 months of alternately
        /// 30 and 29 days. The last month is extended to 30 days in a leap year.
        /// Leap years occur according to a 30 year cycle. There are four recognised
        /// patterns of leap years in the 30 year cycle:
        /// </para>
        /// <list type="table">
        ///    <listheader><term>Origin</term><description>Leap years</description></listheader>
        ///    <item><term>Kūshyār ibn Labbān</term><description>2, 5, 7, 10, 13, 15, 18, 21, 24, 26, 29</description></item>
        ///    <item><term>al-Fazārī</term><description>2, 5, 7, 10, 13, 16, 18, 21, 24, 26, 29</description></item>
        ///    <item><term>Fātimid (also known as Misri or Bohra)</term><description>2, 5, 8, 10, 13, 16, 19, 21, 24, 27, 29</description></item>
        ///    <item><term>Habash al-Hasib</term><description>2, 5, 8, 11, 13, 16, 19, 21, 24, 27, 30</description></item>
        /// </list>
        /// <para>
        /// The leap year pattern to use is determined from the first parameter to this factory method.
        /// The second parameter determines which epoch is used - the "astronomical" or "Thursday" epoch
        /// (July 15th 622CE) or the "civil" or "Friday" epoch (July 16th 622CE).
        /// </para>
        /// <para>
        /// This implementation defines a day as midnight to midnight exactly as per
        /// the ISO chronology. This correct start of day is at sunset on the previous
        /// day, however this cannot readily be modelled and has been ignored.
        /// </para>
        /// </remarks>
        public static CalendarSystem GetIslamicCalendar(IslamicLeapYearPattern leapYearPattern, IslamicEpoch epoch)
        {
            return IslamicCalendar.GetInstance(leapYearPattern, epoch);
        }

        private readonly FieldSet fields;
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarSystem"/> class.
        /// </summary>
        /// <param name="name">The name of the calendar</param>
        /// <param name="fieldAssembler">Delegate to invoke in order to assemble fields for this calendar.</param>
        internal CalendarSystem(string name, FieldAssembler fieldAssembler)
        {
            this.name = name;
            FieldSet.Builder builder = new FieldSet.Builder();
            fieldAssembler(builder, this);
            this.fields = builder.Build();
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

        /// <summary>
        /// Returns the number of days in the given month within the given year.
        /// </summary>
        public abstract int GetDaysInMonth(int year, int month);

        /// <summary>
        /// Returns whether or not the given year is a leap year in this calendar.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public abstract bool IsLeapYear(int year);

        internal FieldSet Fields { get { return fields; } }

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
        /// <summary>
        /// Converts this calendar system to text by simply returning its name.
        /// </summary>
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
