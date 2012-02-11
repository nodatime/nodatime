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
using NodaTime.Calendars;
using NodaTime.Fields;

namespace NodaTime
{
    /// <summary>
    /// A calendar system maps the non-calendar-specific "local time line" to human concepts
    /// such as years, months and days.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Many developers will never need to touch this class, other than to potentially ask a calendar
    /// how many days are in a particular year/month and the like. Noda Time defaults to using the ISO-8601
    /// calendar anywhere that a calendar system is required but hasn't been explicitly specified.
    /// </para>
    /// <para>
    /// If you need to obtain a <see cref="CalendarSystem" /> instance, use one of the static properties or methods in this
    /// class, such as the <see cref="Iso" /> property or the <see cref="GetGregorianCalendar(int)" /> method.
    /// </para>
    /// <para>Although this class is abstract, other assemblies cannot derive from it: it contains internal
    /// abstract methods, referring to internal types. This ensures that all calendar types are genuinely
    /// immutable and thread-safe, aside from anything else. If you require a calendar system which is not
    /// currently supported, please file a feature request and we'll see what we can do.
    /// </para>
    /// <para>
    /// This class roughly corresponds to Chronology in Joda Time, although it includes
    /// the functionality of Chronology, BaseChronology and AssembledChronology all mashed
    /// together. Unlike Chronology, there's no time zone handling in CalendarSystem - that's
    /// treated entirely separately to calendaring.
    /// </para>
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

        #region Public factory members for calendars
        /// <summary>
        /// Returns a calendar system that follows the rules of the ISO-8601 standard,
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
        public static CalendarSystem Iso { get { return GregorianCalendarSystem.IsoInstance; } }

        /// <summary>
        /// Returns a pure proleptic Gregorian calendar system, which defines every
        /// fourth year as leap, unless the year is divisible by 100 and not by 400.
        /// This improves upon the Julian calendar leap year rule.
        /// </summary>
        /// <remarks>
        /// Although the Gregorian calendar did not exist before 1582 CE, this
        /// calendar system assumes it did, thus it is proleptic. This implementation also
        /// fixes the start of the year at January 1.
        /// </remarks>
        /// <param name="minDaysInFirstWeek">The minimum number of days in the first week of the year.
        /// When computing the WeekOfWeekYear and WeekYear properties of a particular date, this is
        /// used to decide at what point the week year changes.</param>
        /// <returns>A suitable Gregorian calendar reference; the same reference may be returned by several
        /// calls as the object is immutable and thread-safe.</returns>
        public static CalendarSystem GetGregorianCalendar(int minDaysInFirstWeek)
        {
            return GregorianCalendarSystem.GetInstance(minDaysInFirstWeek);
        }

        /// <summary>
        /// Returns a pure proleptic Julian calendar system, which defines every
        /// fourth year as a leap year. This implementation follows the leap year rule
        /// strictly, even for dates before 8 CE, where leap years were actually
        /// irregular.
        /// </summary>
        /// <remarks>
        /// Although the Julian calendar did not exist before 45 BCE, this chronology
        /// assumes it did, thus it is proleptic. This implementation also fixes the
        /// start of the year at January 1.
        /// </remarks>
        /// <param name="minDaysInFirstWeek">The minimum number of days in the first week of the year.
        /// When computing the WeekOfWeekYear and WeekYear properties of a particular date, this is
        /// used to decide at what point the week year changes.</param>
        /// <returns>A suitable Julian calendar reference; the same reference may be returned by several
        /// calls as the object is immutable and thread-safe.</returns>
        public static CalendarSystem GetJulianCalendar(int minDaysInFirstWeek)
        {
            return JulianCalendarSystem.GetInstance(minDaysInFirstWeek);
        }

        /// <summary>
        /// Returns a Coptic calendar system, which defines every fourth year as
        /// leap, much like the Julian calendar. The year is broken down into 12 months,
        /// each 30 days in length. An extra period at the end of the year is either 5
        /// or 6 days in length. In this implementation, it is considered a 13th month.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Year 1 in the Coptic calendar began on August 29, 284 CE (Julian), thus
        /// Coptic years do not begin at the same time as Julian years. This chronology
        /// is not proleptic, as it does not allow dates before the first Coptic year.
        /// </para>
        /// <para>
        /// This implementation defines a day as midnight to midnight exactly as per
        /// the ISO chronology. Some references indicate that a coptic day starts at
        /// sunset on the previous ISO day, but this has not been confirmed and is not
        /// implemented.
        /// </para>
        /// </remarks>
        public static CalendarSystem GetCopticCalendar(int minDaysInFirstWeek)
        {
            return CopticCalendarSystem.GetInstance(minDaysInFirstWeek);
        }

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
        /// <param name="leapYearPattern">The pattern of years in the 30-year cycle to consider as leap years</param>
        /// <param name="epoch">The kind of epoch to use (astronomical or civil)</param>
        /// <returns>A suitable Islamic calendar reference; the same reference may be returned by several
        /// calls as the object is immutable and thread-safe.</returns>
        public static CalendarSystem GetIslamicCalendar(IslamicLeapYearPattern leapYearPattern, IslamicEpoch epoch)
        {
            return IslamicCalendar.GetInstance(leapYearPattern, epoch);
        }
        #endregion

        private readonly FieldSet fields;
        private readonly string name;
        private readonly IList<Era> eras; 

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarSystem"/> class.
        /// </summary>
        /// <param name="name">The name of the calendar</param>
        /// <param name="fieldAssembler">Delegate to invoke in order to assemble fields for this calendar.</param>
        /// <param name="eras">The eras within this calendar, which need not be unique to the calendar.</param>
        internal CalendarSystem(string name, FieldAssembler fieldAssembler, IEnumerable<Era> eras)
        {
            this.name = name;
            this.eras = new List<Era>(eras).AsReadOnly();
            FieldSet.Builder builder = new FieldSet.Builder();
            fieldAssembler(builder, this);
            this.fields = builder.Build();
        }

        /// <summary>
        /// Gets the name of this calendar system. Each kind of calendar system has a unique name, although this
        /// does not necessarily provide enough information for round-tripping. (For example, the name of an
        /// Islamic calendar system does not indicate which kind of leap cycle it uses.)
        /// </summary>
        /// <value>The calendar system name.</value>
        public string Name { get { return name; } }

        /// <summary>
        /// Returns whether the day-of-week field refers to ISO days. If true, types such as <see cref="LocalDateTime" />
        /// can use the <see cref="IsoDayOfWeek" /> property to avoid using magic numbers.
        /// This defaults to true, but can be overridden by specific calendars.
        /// </summary>
        public virtual bool UsesIsoDayOfWeek { get { return true; } }

        /// <summary>
        /// Returns the number of days in the given month within the given year.
        /// </summary>
        /// <param name="year">The year in which to consider the month</param>
        /// <param name="month">The month to determine the number of days in</param>
        /// <exception cref="ArgumentOutOfRangeException">The given year / month combination
        /// is invalid for this calendar.</exception>
        /// <returns>The number of days in the given month and year.</returns>
        public abstract int GetDaysInMonth(int year, int month);

        /// <summary>
        /// Returns whether or not the given year is a leap year in this calendar.
        /// </summary>
        /// <param name="year">The year to consider.</param>
        /// <exception cref="ArgumentOutOfRangeException">The given year is invalid for this calendar.
        /// In some cases this may not be thrown whatever value you provide, for example if all years have
        /// the same months in this calendar. Failure to throw an exception should not be treated as an
        /// indication that the year is valid.</exception>
        /// <returns>True if the given year is a leap year; false otherwise.</returns>
        public abstract bool IsLeapYear(int year);

        /// <summary>
        /// The minimum valid year (inclusive) within this calendar.
        /// </summary>
        // TODO: Back these by simple fields?
        public abstract int MinYear { get; }

        /// <summary>
        /// The maximum valid year (inclusive) within this calendar.
        /// </summary>
        public abstract int MaxYear { get; }

        /// <summary>
        /// The maximum valid month (inclusive) within this calendar in the given year. It is assumed that
        /// all calendars start with month 1 and go up to this month number in any valid year.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The given year is invalid for this calendar.
        /// In some cases this may not be thrown whatever value you provide, for example if all years have
        /// the same months in this calendar. Failure to throw an exception should not be treated as an
        /// indication that the year is valid.</exception>
        /// <returns>The maximum month number within the given year.</returns>
        public abstract int GetMaxMonth(int year);

        #region Era-based members
        /// <summary>
        /// Returns a read-only list of eras for this calendar.
        /// </summary>
        public IList<Era> Eras { get { return eras; } }

        /// <summary>
        /// Returns the "absolute year" (the one used throughout most of the API, without respect to eras)
        /// from a year-of-era and an era.
        /// </summary>
        /// <param name="yearOfEra">The year within the era.</param>
        /// <param name="era">The era in which to consider the year</param>
        /// <returns>The absolute year represented by the specified year of era.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="era"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="yearOfEra"/> is out of the range of years for the given era</exception>
        /// <exception cref="ArgumentException"><paramref name="era"/> is not an era of this calendar</exception>
        public int GetAbsoluteYear(int yearOfEra, Era era)
        {
            return GetAbsoluteYear(yearOfEra, GetEraIndex(era));
        }

        /// <summary>
        /// Returns the maximum valid year in the given era.
        /// </summary>
        /// <param name="era">The era in which to find the greatest year</param>
        /// <returns>The maximum valid year in the given era.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="era"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="era"/> is not an era of this calendar</exception>
        public int GetMaxYearOfEra(Era era)
        {
            return GetMaxYearOfEra(GetEraIndex(era));
        }

        /// <summary>
        /// Returns the minimum valid year in the given era.
        /// </summary>
        /// <param name="era">The era in which to find the greatest year</param>
        /// <returns>The minimum valid year in the given eraera.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="era"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="era"/> is not an era of this calendar</exception>
        public int GetMinYearOfEra(Era era)
        {
            return GetMinYearOfEra(GetEraIndex(era));
        }

        /// <summary>
        /// Convenience method to perform nullity and validity checking on the era, converting it to
        /// the index within the list of eras supported by this calendar system.
        /// </summary>
        private int GetEraIndex(Era era)
        {
            if (era == null)
            {
                throw new ArgumentNullException("era");
            }
            int index = Eras.IndexOf(era);
            if (index == -1)
            {
                throw new ArgumentException("Era does not belong to this calendar", "era");
            }
            return index;
        }

        /// <summary>
        /// See <see cref="GetMinYearOfEra(NodaTime.Calendars.Era)" /> - but this uses a pre-validated index.
        /// This default implementation returns 1, but can be overridden by derived classes.
        /// </summary>
        internal virtual int GetMinYearOfEra(int eraIndex)
        {
            return 1;
        }

        /// <summary>
        /// See <see cref="GetMaxYearOfEra(Era)"/> - but this uses a pre-validated index.
        /// This default implementation returns the maximum year for this calendar, which is
        /// a valid implementation for single-era calendars.
        /// </summary>
        internal virtual int GetMaxYearOfEra(int eraIndex)
        {
            return MaxYear;
        }

        /// <summary>
        /// See <see cref="GetAbsoluteYear(int, Era)"/> - but this uses a pre-validated index.
        /// This default implementation validates that the year is between 1 and MaxYear inclusive,
        /// but then returns it as-is, expecting that there's no further work to be
        /// done. This is valid for single-era calendars; the method should be overridden for multi-era calendars.
        /// </summary>
        internal virtual int GetAbsoluteYear(int yearOfEra, int eraIndex)
        {
            if (yearOfEra < 1 || yearOfEra > MaxYear)
            {
                throw new ArgumentOutOfRangeException("yearOfEra");
            }
            return yearOfEra;
        }
        #endregion

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
        /// <returns>A <see cref="LocalInstant"/> with the given year, month, day and tick-of-day.</returns>
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
        /// <param name="year">Absolute year (not year within era; may be negative)</param>
        /// <param name="monthOfYear">Month of year</param>
        /// <param name="dayOfMonth">Day of month</param>
        /// <param name="hourOfDay">Hour within the day (0-23)</param>
        /// <param name="minuteOfHour">Minute within the hour</param>
        /// <param name="secondOfMinute">Second within the minute</param>
        /// <param name="millisecondOfSecond">Millisecond within the second</param>
        /// <param name="tickOfMillisecond">Tick within the millisecond</param>
        /// <returns>A <see cref="LocalInstant"/> with the given values.</returns>
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
        /// <returns>A <see cref="LocalInstant"/> value with the given values.</returns>
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

        #region object overrides
        /// <summary>
        /// Converts this calendar system to text by simply returning its name.
        /// </summary>
        /// <returns>The name of this calendar system.</returns>
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
