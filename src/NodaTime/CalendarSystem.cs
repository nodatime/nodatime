// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NodaTime.Calendars;
using NodaTime.Fields;
using NodaTime.Utility;

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
    /// <para>Although this class is abstract, other assemblies cannot introduce types which derive from it, as the
    /// constructor is internal, and makes use of internal types. This ensures that all calendar types are genuinely
    /// immutable and thread-safe, aside from anything else. If you require a calendar system which is not
    /// currently supported, please file a feature request and we'll see what we can do.
    /// </para>
    /// </remarks>
    /// <threadsafety>
    /// All calendar implementations within Noda Time are immutable and thread-safe. See the thread safety
    /// section of the user guide for more information.
    /// </threadsafety>
    public abstract class CalendarSystem
    {
        /// <summary>
        /// Delegate used to construct fields. This is called within the base constructor, before the
        /// derived class constructor bodies have been run - so it's *somewhat* unsafe to pass "this"
        /// reference, but derived classes will just need to be careful.
        /// </summary>
        internal delegate void FieldAssembler(FieldSet.Builder builder, CalendarSystem @this);

        // TODO(Post-V1): Consider moving the static accessors into a separate class. As we get more calendars,
        // this approach will become unwieldy.

        #region Public factory members for calendars
        /// <summary>
        /// Fetches a calendar system by its unique identifier. This provides full round-tripping of a calendar
        /// system. It is not guaranteed that calling this method twice with the same identifier will return
        /// identical references, but the references objects will be equal.
        /// </summary>
        /// <param name="id">The ID of the calendar system. This is case-sensitive.</param>
        /// <returns>The calendar system with the given ID.</returns>
        /// <seealso cref="Id"/>
        /// <exception cref="KeyNotFoundException">No calendar system for the specified ID can be found.</exception>
        public static CalendarSystem ForId(string id)
        {
            Func<CalendarSystem> factory;
            if (!IdToFactoryMap.TryGetValue(id, out factory))
            {
                throw new KeyNotFoundException(string.Format("No calendar system for ID {0} exists", id));
            }
            return factory();
        }

        /// <summary>
        /// Returns the IDs of all calendar systems available within Noda Time. The order of the keys is not guaranteed.
        /// </summary>
        public static IEnumerable<string> Ids { get { return IdToFactoryMap.Keys; } }

        private static readonly Dictionary<string, Func<CalendarSystem>> IdToFactoryMap = new Dictionary<string, Func<CalendarSystem>>
        {
            { "ISO", () => Iso },
            { "Gregorian 1", () => GetGregorianCalendar(1) },
            { "Gregorian 2", () => GetGregorianCalendar(2) },
            { "Gregorian 3", () => GetGregorianCalendar(3) },
            { "Gregorian 4", () => GetGregorianCalendar(4) }, 
            { "Gregorian 5", () => GetGregorianCalendar(5) },
            { "Gregorian 6", () => GetGregorianCalendar(6) },
            { "Gregorian 7", () => GetGregorianCalendar(7) },
            { "Coptic 1", () => GetCopticCalendar(1) },
            { "Coptic 2", () => GetCopticCalendar(2) },
            { "Coptic 3", () => GetCopticCalendar(3) },
            { "Coptic 4", () => GetCopticCalendar(4) },
            { "Coptic 5", () => GetCopticCalendar(5) },
            { "Coptic 6", () => GetCopticCalendar(6) },
            { "Coptic 7", () => GetCopticCalendar(7) },
            { "Julian 1", () => GetJulianCalendar(1) },
            { "Julian 2", () => GetJulianCalendar(2) },
            { "Julian 3", () => GetJulianCalendar(3) },
            { "Julian 4", () => GetJulianCalendar(4) },
            { "Julian 5", () => GetJulianCalendar(5) },
            { "Julian 6", () => GetJulianCalendar(6) },
            { "Julian 7", () => GetJulianCalendar(7) },
            { "Hijri Civil-Indian", () => GetIslamicCalendar(IslamicLeapYearPattern.Indian, IslamicEpoch.Civil) },
            { "Hijri Civil-Base15", () => GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Civil) },
            { "Hijri Civil-Base16", () => GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Civil) },
            { "Hijri Civil-HabashAlHasib", () => GetIslamicCalendar(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Civil) },
            { "Hijri Astronomical-Indian", () => GetIslamicCalendar(IslamicLeapYearPattern.Indian, IslamicEpoch.Astronomical) },
            { "Hijri Astronomical-Base15", () => GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Astronomical) },
            { "Hijri Astronomical-Base16", () => GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Astronomical) },
            { "Hijri Astronomical-HabashAlHasib", () => GetIslamicCalendar(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Astronomical) },
        };

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
        /// same as the Gregorian calendar system. In the ISO system, centuries and year
        /// of century are zero based. For all years, the century is determined by
        /// dropping the last two digits of the year, ignoring sign. The year of century
        /// is the value of the last two year digits.
        /// </para>
        /// </remarks>
        public static CalendarSystem Iso { get { return GregorianCalendarSystem.IsoHelper.Instance; } }

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
        /// Although the Julian calendar did not exist before 45 BCE, this calendar
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
        /// Coptic years do not begin at the same time as Julian years. This calendar
        /// is not proleptic, as it does not allow dates before the first Coptic year.
        /// </para>
        /// <para>
        /// This implementation defines a day as midnight to midnight exactly as per
        /// the ISO calendar. Some references indicate that a Coptic day starts at
        /// sunset on the previous ISO day, but this has not been confirmed and is not
        /// implemented.
        /// </para>
        /// </remarks>
        /// <param name="minDaysInFirstWeek">The minimum number of days in the first week of the year.
        /// When computing the WeekOfWeekYear and WeekYear properties of a particular date, this is
        /// used to decide at what point the week year changes.</param>
        /// <returns>A suitable Coptic calendar reference; the same reference may be returned by several
        /// calls as the object is immutable and thread-safe.</returns>
        public static CalendarSystem GetCopticCalendar(int minDaysInFirstWeek)
        {
            return CopticCalendarSystem.GetInstance(minDaysInFirstWeek);
        }

        /// <summary>
        /// Returns an Islamic, or Hijri, calendar system.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This returns a tablular calendar, rather than one based on lunar observation. This calendar is a
        /// lunar calendar with 12 months, each of 29 or 30 days, resulting in a year of 354 days (or 355 on a leap
        /// year).
        /// </para>
        /// <para>
        /// Year 1 in the Islamic calendar began on July 15th or 16th, 622 CE (Julian), thus
        /// Islamic years do not begin at the same time as Julian years. This calendar
        /// is not proleptic, as it does not allow dates before the first Islamic year.
        /// </para>
        /// <para>
        /// There are two basic forms of the Islamic calendar, the tabular and the
        /// observed. The observed form cannot easily be used by computers as it
        /// relies on human observation of the new moon. The tabular calendar, implemented here, is an
        /// arithmetic approximation of the observed form that follows relatively simple rules.
        /// </para>
        /// <para>You should choose an epoch based on which external system you wish
        /// to be compatible with. The epoch beginning on July 16th is the more common
        /// one for the tabular calendar, so using <see cref="IslamicEpoch.Civil" />
        /// would usually be a logical choice. However, Windows uses July 15th, so
        /// if you need to be compatible with other Windows systems, you may wish to use
        /// <see cref="IslamicEpoch.Astronomical" />. The fact that the Islamic calendar
        /// traditionally starts at dusk, a Julian day traditionally starts at noon,
        /// and all calendar systems in Noda Time start their days at midnight adds
        /// somewhat inevitable confusion to the mix, unfortunately.</para>
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
        /// the ISO calendar. This correct start of day is at sunset on the previous
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
        private readonly string id;
        private readonly string name;
        private readonly IList<Era> eras;
        private readonly int minYear;
        private readonly int maxYear;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarSystem"/> class.
        /// </summary>
        /// <param name="id">The unique ID of the calendar</param>
        /// <param name="name">The name of the calendar</param>
        /// <param name="minYear">Minimum year in the calendar</param>
        /// <param name="maxYear">Maximum year in the calendar</param>
        /// <param name="fieldAssembler">Delegate to invoke in order to assemble fields for this calendar.</param>
        /// <param name="eras">The eras used in this calendar, which need not be unique to the calendar.</param>
        internal CalendarSystem(string id, string name, int minYear, int maxYear, FieldAssembler fieldAssembler, IEnumerable<Era> eras)
        {
            this.id = id;
            this.name = name;
            this.minYear = minYear;
            this.maxYear = maxYear;
            this.eras = new ReadOnlyCollection<Era>(new List<Era>(eras));
            FieldSet.Builder builder = new FieldSet.Builder();
            fieldAssembler(builder, this);
            fields = builder.Build();
        }

        /// <summary>
        /// Returns the unique identifier for this calendar system. This is provides full round-trip capability
        /// using <see cref="ForId" /> to retrieve the calendar system from the identifier.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A unique ID for a calendar is required when serializing types which include a <see cref="CalendarSystem"/>.
        /// As of 2 Nov 2012 (ISO calendar) there are no ISO or RFC standards for naming a calendar system. As such,
        /// the identifiers provided here are specific to Noda Time, and are not guaranteed to interoperate with any other
        /// date and time API.
        /// </para>
        /// <list type="table">
        ///   <listheader>
        ///     <term>Calendar ID</term>
        ///     <description>Equivalent factory method</description>
        ///   </listheader>
        ///   <item><term>ISO</term><description><see cref="CalendarSystem.Iso"/></description></item>
        ///   <item><term>Gregorian 1</term><description><see cref="CalendarSystem.GetGregorianCalendar"/>(1)</description></item>
        ///   <item><term>Gregorian 2</term><description><see cref="CalendarSystem.GetGregorianCalendar"/>(2)</description></item>
        ///   <item><term>Gregorian 3</term><description><see cref="CalendarSystem.GetGregorianCalendar"/>(3)</description></item>
        ///   <item><term>Gregorian 3</term><description><see cref="CalendarSystem.GetGregorianCalendar"/>(4)</description></item>
        ///   <item><term>Gregorian 5</term><description><see cref="CalendarSystem.GetGregorianCalendar"/>(5)</description></item>
        ///   <item><term>Gregorian 6</term><description><see cref="CalendarSystem.GetGregorianCalendar"/>(6)</description></item>
        ///   <item><term>Gregorian 7</term><description><see cref="CalendarSystem.GetGregorianCalendar"/>(7)</description></item>
        ///   <item><term>Coptic 1</term><description><see cref="CalendarSystem.GetCopticCalendar"/>(1)</description></item>
        ///   <item><term>Coptic 2</term><description><see cref="CalendarSystem.GetCopticCalendar"/>(2)</description></item>
        ///   <item><term>Coptic 3</term><description><see cref="CalendarSystem.GetCopticCalendar"/>(3)</description></item>
        ///   <item><term>Coptic 4</term><description><see cref="CalendarSystem.GetCopticCalendar"/>(4)</description></item>
        ///   <item><term>Coptic 5</term><description><see cref="CalendarSystem.GetCopticCalendar"/>(5)</description></item>
        ///   <item><term>Coptic 6</term><description><see cref="CalendarSystem.GetCopticCalendar"/>(6)</description></item>
        ///   <item><term>Coptic 7</term><description><see cref="CalendarSystem.GetCopticCalendar"/>(7)</description></item>
        ///   <item><term>Julian 1</term><description><see cref="CalendarSystem.GetJulianCalendar"/>(1)</description></item>
        ///   <item><term>Julian 2</term><description><see cref="CalendarSystem.GetJulianCalendar"/>(2)</description></item>
        ///   <item><term>Julian 3</term><description><see cref="CalendarSystem.GetJulianCalendar"/>(3)</description></item>
        ///   <item><term>Julian 4</term><description><see cref="CalendarSystem.GetJulianCalendar"/>(4)</description></item>
        ///   <item><term>Julian 5</term><description><see cref="CalendarSystem.GetJulianCalendar"/>(5)</description></item>
        ///   <item><term>Julian 6</term><description><see cref="CalendarSystem.GetJulianCalendar"/>(6)</description></item>
        ///   <item><term>Julian 7</term><description><see cref="CalendarSystem.GetJulianCalendar"/>(7)</description></item>
        ///   <item><term>Hijri Civil-Indian</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Indian, IslamicEpoch.Civil)</description></item>
        ///   <item><term>Hijri Civil-Base15</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Base15, IslamicEpoch.Civil)</description></item>
        ///   <item><term>Hijri Civil-Base16</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Base16, IslamicEpoch.Civil)</description></item>
        ///   <item><term>Hijri Civil-HabashAlHasib</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Civil)</description></item>
        ///   <item><term>Hijri Astronomical-Indian</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Indian, IslamicEpoch.Astronomical)</description></item>
        ///   <item><term>Hijri Astronomical-Base15</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Base15, IslamicEpoch.Astronomical)</description></item>
        ///   <item><term>Hijri Astronomical-Base16</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Base16, IslamicEpoch.Astronomical)</description></item>
        ///   <item><term>Hijri Astronomical-HabashAlHasib</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Astronomical)</description></item>
        /// </list>
        /// </remarks>
        public string Id { get { return id; } }

        /// <summary>
        /// Returns the name of this calendar system. Each kind of calendar system has a unique name, but this
        /// does not usually provide enough information for round-tripping. (For example, the name of an
        /// Islamic calendar system does not indicate which kind of leap cycle it uses, and other calendars
        /// specify the minimum number of days in the first week of a year.)
        /// </summary>
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
        /// Note that some implementations may return a value rather than throw this exception. Failure to throw an
        /// exception should not be treated as an indication that the year is valid.</exception>
        /// <returns>True if the given year is a leap year; false otherwise.</returns>
        public abstract bool IsLeapYear(int year);

        /// <summary>
        /// The minimum valid year (inclusive) within this calendar.
        /// </summary>
        public int MinYear { get { return minYear; } }

        /// <summary>
        /// The maximum valid year (inclusive) within this calendar.
        /// </summary>
        public int MaxYear { get { return maxYear; } }

        /// <summary>
        /// The maximum valid month (inclusive) within this calendar in the given year. It is assumed that
        /// all calendars start with month 1 and go up to this month number in any valid year.
        /// </summary>
        /// <param name="year">The year to consider.</param>
        /// <exception cref="ArgumentOutOfRangeException">The given year is invalid for this calendar.
        /// Note that some implementations may return a month rather than throw this exception (for example, if all
        /// years have the same number of months in this calendar system). Failure to throw an exception should not be
        /// treated as an indication that the year is valid.</exception>
        /// <returns>The maximum month number within the given year.</returns>
        public abstract int GetMaxMonth(int year);

        #region Era-based members
        /// <summary>
        /// Returns a read-only list of eras used in this calendar system.
        /// </summary>
        public IList<Era> Eras { get { return eras; } }

        /// <summary>
        /// Returns the "absolute year" (the one used throughout most of the API, without respect to eras)
        /// from a year-of-era and an era.
        /// </summary>
        /// <remarks>
        /// For example, in the Gregorian and Julian calendar systems, the BCE era starts at year 1, which is
        /// equivalent to an "absolute year" of 0 (then BCE year 2 has an absolute year of -1, and so on).  The absolute
        /// year is the year that is used throughout the API; year-of-era is typically used primarily when formatting
        /// and parsing date values to and from text.
        /// </remarks>
        /// <param name="yearOfEra">The year within the era.</param>
        /// <param name="era">The era in which to consider the year</param>
        /// <returns>The absolute year represented by the specified year of era.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="era"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="yearOfEra"/> is out of the range of years for the given era</exception>
        /// <exception cref="ArgumentException"><paramref name="era"/> is not an era used in this calendar</exception>
        public int GetAbsoluteYear(int yearOfEra, Era era)
        {
            return GetAbsoluteYear(yearOfEra, GetEraIndex(era));
        }

        /// <summary>
        /// Returns the maximum valid year-of-era in the given era.
        /// </summary>
        /// <param name="era">The era in which to find the greatest year</param>
        /// <returns>The maximum valid year in the given era.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="era"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="era"/> is not an era used in this calendar</exception>
        public int GetMaxYearOfEra(Era era)
        {
            return GetMaxYearOfEra(GetEraIndex(era));
        }

        /// <summary>
        /// Returns the minimum valid year-of-era in the given era.
        /// </summary>
        /// <param name="era">The era in which to find the greatest year</param>
        /// <returns>The minimum valid year in the given eraera.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="era"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="era"/> is not an era used in this calendar</exception>
        public int GetMinYearOfEra(Era era)
        {
            return GetMinYearOfEra(GetEraIndex(era));
        }

        /// <summary>
        /// Convenience method to perform nullity and validity checking on the era, converting it to
        /// the index within the list of eras used in this calendar system.
        /// </summary>
        private int GetEraIndex(Era era)
        {
            Preconditions.CheckNotNull(era, "era");
            int index = Eras.IndexOf(era);
            Preconditions.CheckArgument(index != -1, "era", "Era is not used in this calendar");
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
        /// The set of given values must refer to a valid datetime.
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
        /// <exception cref="ArgumentOutOfRangeException">The year of era, month of year and day of month values don't
        /// form a valid date.</exception>
        /// <returns>A <see cref="LocalInstant"/> with the given year, month, day and tick-of-day.</returns>
        internal virtual LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, long tickOfDay)
        {
            LocalInstant instant = Fields.Year.SetValue(LocalInstant.LocalUnixEpoch, year);
            instant = Fields.MonthOfYear.SetValue(instant, monthOfYear);
            instant = Fields.DayOfMonth.SetValue(instant, dayOfMonth);
            return Fields.TickOfDay.SetValue(instant, tickOfDay);
        }

        /// <summary>
        /// Returns a local instant, at the start of the day formed from the given year of era, month, day, and era arguments.
        /// The set of given values must refer to a valid datetime.
        /// </summary>
        /// <param name="era">Era to use. This must be one of the eras used in this calendar</param>
        /// <param name="yearOfEra">Year of era to use</param>
        /// <param name="monthOfYear">Month to use</param>
        /// <param name="dayOfMonth">Day of month to use</param>
        /// <exception cref="ArgumentNullException"><paramref name="era" />is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="era"/> is not an era used in this calendar</exception>
        /// <exception cref="ArgumentOutOfRangeException">The year of era, month of year and day of month values don't
        /// form a valid date.</exception>
        /// <returns>A <see cref="LocalInstant"/> with the given year, month, day and era.</returns>
        internal virtual LocalInstant GetLocalInstant(Era era, int yearOfEra, int monthOfYear, int dayOfMonth)
        {
            int eraIndex = GetEraIndex(era);
            LocalInstant instant = Fields.Era.SetValue(LocalInstant.LocalUnixEpoch, eraIndex);
            instant = Fields.YearOfEra.SetValue(instant, yearOfEra);
            instant = Fields.MonthOfYear.SetValue(instant, monthOfYear);
            return Fields.DayOfMonth.SetValue(instant, dayOfMonth);
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
        /// Converts this calendar system to text by simply returning its unique ID.
        /// </summary>
        /// <returns>The ID of this calendar system.</returns>
        public override string ToString()
        {
            return id;
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
                throw new InvalidOperationException("Calendar " + id + " does not use ISO days of the week");
            }
            return (IsoDayOfWeek)Fields.DayOfWeek.GetValue(localInstant);
        }
    }
}
