// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Calendars;
using NodaTime.Utility;
using System.Diagnostics.CodeAnalysis;

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
    /// class, such as the <see cref="Iso" /> property or the <see cref="GetHebrewCalendar(HebrewMonthNumbering)" /> method.
    /// </para>
    /// <para>Although this class is currently sealed (as of Noda Time 1.2), in the future this decision may
    /// be reversed. In any case, there is no current intention for third-party developers to be able to implement
    /// their own calendar systems (for various reasons). If you require a calendar system which is not
    /// currently supported, please file a feature request and we'll see what we can do.
    /// </para>
    /// </remarks>
    /// <threadsafety>
    /// All calendar implementations are immutable and thread-safe. See the thread safety
    /// section of the user guide for more information.
    /// </threadsafety>
    [Immutable]
    public sealed class CalendarSystem
    {
        // IDs and names are separated out (usually with the ID either being the same as the name,
        // or the base ID being the same as a name and then other IDs being formed from it.) The
        // differentiation is only present for clarity.
        private const string GregorianName = "Gregorian";
        private const string GregorianId = GregorianName;

        private const string IsoName = "ISO";
        private const string IsoId = IsoName;

        private const string CopticName = "Coptic";
        private const string CopticId = CopticName;

        private const string BadiName = "Badi";
        private const string BadiId = BadiName;

        private const string JulianName = "Julian";
        private const string JulianId = JulianName;

        private const string IslamicName = "Hijri";
        private const string IslamicIdBase = IslamicName;
        // Not part of IslamicCalendars as we want to be able to call it without triggering type initialization.
        internal static string GetIslamicId(IslamicLeapYearPattern leapYearPattern, IslamicEpoch epoch)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1}-{2}", IslamicIdBase, epoch, leapYearPattern);
        }

        private const string PersianName = "Persian";
        private const string PersianIdBase = PersianName;
        private const string PersianSimpleId = PersianIdBase + " Simple";
        private const string PersianAstronomicalId = PersianIdBase + " Algorithmic";
        private const string PersianArithmeticId = PersianIdBase + " Arithmetic";

        private const string HebrewName = "Hebrew";
        private const string HebrewIdBase = HebrewName;
        private const string HebrewCivilId = HebrewIdBase + " Civil";
        private const string HebrewScripturalId = HebrewIdBase + " Scriptural";

        private const string UmAlQuraName = "Um Al Qura";
        private const string UmAlQuraId = UmAlQuraName;

        // While we could implement some of these as auto-props, it probably adds more confusion than convenience.
        private static readonly CalendarSystem IsoCalendarSystem;
        private static readonly CalendarSystem[] CalendarByOrdinal = new CalendarSystem[(int)CalendarOrdinal.Size];

        static CalendarSystem()
        {
            var gregorianCalculator = new GregorianYearMonthDayCalculator();
            var gregorianEraCalculator = new GJEraCalculator(gregorianCalculator);
            IsoCalendarSystem = new CalendarSystem(CalendarOrdinal.Iso, IsoId, IsoName, gregorianCalculator, gregorianEraCalculator);
        }

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
        /// <exception cref="NotSupportedException">The calendar system with the specified ID is known, but not supported on this platform.</exception>
        [NotNull] public static CalendarSystem ForId([NotNull] string id)
        {
            Preconditions.CheckNotNull(id, nameof(id));
            if (!IdToFactoryMap.TryGetValue(id, out Func<CalendarSystem> factory))
            {
                throw new KeyNotFoundException($"No calendar system for ID {id} exists");
            }
            return factory();
        }

        /// <summary>
        /// Fetches a calendar system by its ordinal value, constructing it if necessary.
        /// </summary>
        [NotNull] internal static CalendarSystem ForOrdinal([Trusted] CalendarOrdinal ordinal)
        {
            Preconditions.DebugCheckArgument(ordinal >= 0 && ordinal < CalendarOrdinal.Size, nameof(ordinal),
                "Unknown ordinal value {0}", ordinal);
            // Avoid an array lookup for the overwhelmingly common case.
            if (ordinal == CalendarOrdinal.Iso)
            {
                return IsoCalendarSystem;
            }
            CalendarSystem calendar = CalendarByOrdinal[(int)ordinal];
            if (calendar != null)
            {
                return calendar;
            }
            // Not found it in the array. This can happen if the calendar system was initialized in
            // a different thread, and the write to the array isn't visible in this thread yet.
            // A simple switch will do the right thing. This is separated out (directly below) to allow
            // it to be tested separately. (It may also help this method be inlined...) The return
            // statement below is unlikely to ever be hit by code coverage, as it's handling a very
            // unusual and hard-to-provoke situation.
            return ForOrdinalUncached(ordinal);
        }

        [VisibleForTesting]
        internal static CalendarSystem ForOrdinalUncached([Trusted] CalendarOrdinal ordinal)
        {
            switch (ordinal)
            {
                // This entry is really just for completeness. We'd never get called with this.
                case CalendarOrdinal.Iso:
                    return Iso;
                case CalendarOrdinal.Gregorian:
                    return Gregorian;
                case CalendarOrdinal.Julian:
                    return Julian;
                case CalendarOrdinal.Coptic:
                    return Coptic;
                case CalendarOrdinal.Badi:
                    return Badi;
                case CalendarOrdinal.HebrewCivil:
                    return HebrewCivil;
                case CalendarOrdinal.HebrewScriptural:
                    return HebrewScriptural;
                case CalendarOrdinal.PersianSimple:
                    return PersianSimple;
                case CalendarOrdinal.PersianArithmetic:
                    return PersianArithmetic;
                case CalendarOrdinal.PersianAstronomical:
                    return PersianAstronomical;
                case CalendarOrdinal.IslamicAstronomicalBase15:
                    return GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Astronomical);
                case CalendarOrdinal.IslamicAstronomicalBase16:
                    return GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Astronomical);
                case CalendarOrdinal.IslamicAstronomicalIndian:
                    return GetIslamicCalendar(IslamicLeapYearPattern.Indian, IslamicEpoch.Astronomical);
                case CalendarOrdinal.IslamicAstronomicalHabashAlHasib:
                    return GetIslamicCalendar(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Astronomical);
                case CalendarOrdinal.IslamicCivilBase15:
                    return GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Civil);
                case CalendarOrdinal.IslamicCivilBase16:
                    return GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Civil);
                case CalendarOrdinal.IslamicCivilIndian:
                    return GetIslamicCalendar(IslamicLeapYearPattern.Indian, IslamicEpoch.Civil);
                case CalendarOrdinal.IslamicCivilHabashAlHasib:
                    return GetIslamicCalendar(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Civil);
                case CalendarOrdinal.UmAlQura:
                    return UmAlQura;
                default:
                    throw new InvalidOperationException($"Bug in Noda Time: calendar ordinal {ordinal} missing from switch in CalendarSystem.ForOrdinal.");
            }
        }

        /// <summary>
        /// Returns the IDs of all calendar systems available within Noda Time. The order of the keys is not guaranteed.
        /// </summary>
        /// <value>The IDs of all calendar systems available within Noda Time.</value>
        [NotNull] public static IEnumerable<string> Ids => IdToFactoryMap.Keys;

        private static readonly Dictionary<string, Func<CalendarSystem>> IdToFactoryMap = new Dictionary<string, Func<CalendarSystem>>
        {
            {IsoId, () => Iso},
            {PersianSimpleId, () => PersianSimple},
            {PersianArithmeticId, () => PersianArithmetic},
            {PersianAstronomicalId, () => PersianAstronomical},
            {HebrewCivilId, () => GetHebrewCalendar(HebrewMonthNumbering.Civil)},
            {HebrewScripturalId, () => GetHebrewCalendar(HebrewMonthNumbering.Scriptural)},
            {GregorianId, () => Gregorian},
            {CopticId, () => Coptic},
            {BadiId, () => Badi},
            {JulianId, () => Julian},
            {UmAlQuraId, () => UmAlQura},
            {GetIslamicId(IslamicLeapYearPattern.Indian, IslamicEpoch.Civil), () => GetIslamicCalendar(IslamicLeapYearPattern.Indian, IslamicEpoch.Civil)},
            {GetIslamicId(IslamicLeapYearPattern.Base15, IslamicEpoch.Civil), () => GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Civil)},
            {GetIslamicId(IslamicLeapYearPattern.Base16, IslamicEpoch.Civil), () => GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Civil)},
            {GetIslamicId(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Civil), () => GetIslamicCalendar(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Civil)},
            {GetIslamicId(IslamicLeapYearPattern.Indian, IslamicEpoch.Astronomical), () => GetIslamicCalendar(IslamicLeapYearPattern.Indian, IslamicEpoch.Astronomical)},
            {GetIslamicId(IslamicLeapYearPattern.Base15, IslamicEpoch.Astronomical), () => GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Astronomical)},
            {GetIslamicId(IslamicLeapYearPattern.Base16, IslamicEpoch.Astronomical), () => GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Astronomical)},
            {GetIslamicId(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Astronomical), () => GetIslamicCalendar(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Astronomical)},
        };

        /// <summary>
        /// Returns a calendar system that follows the rules of the ISO-8601 standard,
        /// which is compatible with Gregorian for all modern dates.
        /// </summary>
        /// <remarks>
        /// As of Noda Time 2.0, this calendar system is equivalent to <see cref="Gregorian"/>.
        /// The only areas in which the calendars differed were around centuries, and the members
        /// relating to those differences were removed in Noda Time 2.0.
        /// The distinction between Gregorian and ISO has been maintained for the sake of simplicity, compatibility
        /// and consistency.
        /// </remarks>
        /// <value>The ISO calendar system.</value>
        [NotNull] public static CalendarSystem Iso => IsoCalendarSystem;

        /// <summary>
        /// Returns a Hebrew calendar, as described at http://en.wikipedia.org/wiki/Hebrew_calendar. This is a
        /// purely mathematical calculator, applied proleptically to the period where the real calendar was observational. 
        /// </summary>
        /// <remarks>
        /// <para>Please note that in version 1.3.0 of Noda Time, support for the Hebrew calendar is somewhat experimental,
        /// particularly in terms of calculations involving adding or subtracting years. Additionally, text formatting
        /// and parsing using month names is not currently supported, due to the challenges of handling leap months.
        /// It is hoped that this will be improved in future versions.</para>
        /// <para>The implementation for this was taken from http://www.cs.tau.ac.il/~nachum/calendar-book/papers/calendar.ps,
        /// which is a public domain algorithm presumably equivalent to that given in the Calendrical Calculations book
        /// by the same authors (Nachum Dershowitz and Edward Reingold).
        /// </para>
        /// </remarks>
        /// <param name="monthNumbering">The month numbering system to use</param>
        /// <returns>A Hebrew calendar system for the given month numbering.</returns>
        [NotNull] public static CalendarSystem GetHebrewCalendar(HebrewMonthNumbering monthNumbering)
        {
            Preconditions.CheckArgumentRange(nameof(monthNumbering), (int) monthNumbering, 1, 2);
            return HebrewCalendars.ByMonthNumbering[((int) monthNumbering) - 1];
        }

        /// <summary>
        /// Returns the Badíʿ (meaning "wondrous" or "unique") calendar, as described at https://en.wikipedia.org/wiki/Badi_calendar. 
        /// This is a purely solar calendar with years starting at the vernal equinox.
        /// </summary>
        /// <remarks>
        /// <para>The Badíʿ calendar was developed and defined by the founders of the Bahá'í Faith in the mid to late
        /// 1800's A.D. The first year in the calendar coincides with 1844 A.D. Years are labeled "B.E." for Bahá'í Era.</para>
        /// <para>A year consists of 19 months, each with 19 days. Each day starts at sunset. Years are grouped into sets
        /// of 19 "Unities" (Váḥid) and 19 Unities make up 1 "All Things" (Kull-i-Shay’).</para>
        /// <para>A period of days (usually 4 or 5, called Ayyám-i-Há) occurs between the 18th and 19th months. The length of this 
        /// period of intercalary days is solely determined by the date of the following vernal equinox. The vernal equinox is 
        /// a momentary point in time, so the "date" of the equinox is determined by the date (beginning 
        /// at sunset) in effect in Tehran, Iran at the moment of the equinox.</para>
        /// <para>In this Noda Time implementation, days start at midnight and lookup tables are used to determine vernal equinox dates.
        /// Ayyám-i-Há is internally modelled as extra days added to the 18th month. As a result, a few functions will
        /// not work as expected for Ayyám-i-Há, such as EndOfMonth.</para>
        /// </remarks>
        /// <returns>The Badíʿ calendar system.</returns>
        [NotNull] public static CalendarSystem Badi => MiscellaneousCalendars.Badi;

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
        [NotNull] public static CalendarSystem GetIslamicCalendar(IslamicLeapYearPattern leapYearPattern, IslamicEpoch epoch)
        {
            Preconditions.CheckArgumentRange(nameof(leapYearPattern), (int) leapYearPattern, 1, 4);
            Preconditions.CheckArgumentRange(nameof(epoch), (int) epoch, 1, 2);
            return IslamicCalendars.ByLeapYearPatterAndEpoch[(int) leapYearPattern - 1, (int) epoch - 1];
        }

        #endregion

        // Other fields back read-only automatic properties.
        private readonly EraCalculator eraCalculator;

        private CalendarSystem(CalendarOrdinal ordinal, string id, string name, YearMonthDayCalculator yearMonthDayCalculator, Era singleEra)
            : this(ordinal, id, name, yearMonthDayCalculator, new SingleEraCalculator(singleEra, yearMonthDayCalculator))
        {
        }

        private CalendarSystem(CalendarOrdinal ordinal, string id, string name, YearMonthDayCalculator yearMonthDayCalculator, EraCalculator eraCalculator)
        {
            this.Ordinal = ordinal;
            this.Id = id;
            this.Name = name;
            this.YearMonthDayCalculator = yearMonthDayCalculator;
            this.MinYear = yearMonthDayCalculator.MinYear;
            this.MaxYear = yearMonthDayCalculator.MaxYear;
            this.MinDays = yearMonthDayCalculator.GetStartOfYearInDays(MinYear);
            this.MaxDays = yearMonthDayCalculator.GetStartOfYearInDays(MaxYear + 1) - 1;
            // We trust the construction code not to mutate the array...
            this.eraCalculator = eraCalculator;
            CalendarByOrdinal[(int) ordinal] = this;
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
        ///     <description>Equivalent factory method or property</description>
        ///   </listheader>
        ///   <item><term>ISO</term><description><see cref="CalendarSystem.Iso"/></description></item>
        ///   <item><term>Gregorian</term><description><see cref="CalendarSystem.Gregorian"/></description></item>
        ///   <item><term>Coptic</term><description><see cref="CalendarSystem.Coptic"/></description></item>
        ///   <item><term>Badíʿ</term><description><see cref="CalendarSystem.Badi"/></description></item>
        ///   <item><term>Julian</term><description><see cref="CalendarSystem.Julian"/></description></item>
        ///   <item><term>Hijri Civil-Indian</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Indian, IslamicEpoch.Civil)</description></item>
        ///   <item><term>Hijri Civil-Base15</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Base15, IslamicEpoch.Civil)</description></item>
        ///   <item><term>Hijri Civil-Base16</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Base16, IslamicEpoch.Civil)</description></item>
        ///   <item><term>Hijri Civil-HabashAlHasib</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Civil)</description></item>
        ///   <item><term>Hijri Astronomical-Indian</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Indian, IslamicEpoch.Astronomical)</description></item>
        ///   <item><term>Hijri Astronomical-Base15</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Base15, IslamicEpoch.Astronomical)</description></item>
        ///   <item><term>Hijri Astronomical-Base16</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Base16, IslamicEpoch.Astronomical)</description></item>
        ///   <item><term>Hijri Astronomical-HabashAlHasib</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Astronomical)</description></item>
        ///   <item><term>Persian Simple</term><description><see cref="CalendarSystem.PersianSimple"/></description></item>
        ///   <item><term>Persian Arithmetic</term><description><see cref="CalendarSystem.PersianArithmetic"/></description></item>
        ///   <item><term>Persian Astronomical</term><description><see cref="CalendarSystem.PersianAstronomical"/></description></item>
        ///   <item><term>Um Al Qura</term><description><see cref="CalendarSystem.UmAlQura"/>()</description></item>
        ///   <item><term>Hebrew Civil</term><description><see cref="CalendarSystem.HebrewCivil"/></description></item>
        ///   <item><term>Hebrew Scriptural</term><description><see cref="CalendarSystem.HebrewScriptural"/></description></item>
        /// </list>
        /// </remarks>
        /// <value>The unique identifier for this calendar system.</value>
        [NotNull] public string Id { get; }

        /// <summary>
        /// Returns the name of this calendar system. Each kind of calendar system has a unique name, but this
        /// does not usually provide enough information for round-tripping. (For example, the name of an
        /// Islamic calendar system does not indicate which kind of leap cycle it uses.)
        /// </summary>
        /// <value>The name of this calendar system.</value>
        [NotNull] public string Name { get; }

        /// <summary>
        /// Gets the minimum valid year (inclusive) within this calendar.
        /// </summary>
        /// <value>The minimum valid year (inclusive) within this calendar.</value>
        public int MinYear { get; }

        /// <summary>
        /// Gets the maximum valid year (inclusive) within this calendar.
        /// </summary>
        /// <value>The maximum valid year (inclusive) within this calendar.</value>
        public int MaxYear { get; }

        /// <summary>
        /// Returns the minimum day number this calendar can handle.
        /// </summary>
        internal int MinDays { get; }

        /// <summary>
        /// Returns the maximum day number (inclusive) this calendar can handle.
        /// </summary>
        internal int MaxDays { get; }

        /// <summary>
        /// Returns the ordinal value of this calendar.
        /// </summary>
        internal CalendarOrdinal Ordinal { get; }

        #region Era-based members

        /// <summary>
        /// Gets a read-only list of eras used in this calendar system.
        /// </summary>
        /// <value>A read-only list of eras used in this calendar system.</value>
        [NotNull] public IList<Era> Eras => eraCalculator.Eras;

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
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="yearOfEra"/> is out of the range of years for the given era.</exception>
        /// <exception cref="ArgumentException"><paramref name="era"/> is not an era used in this calendar.</exception>
        public int GetAbsoluteYear(int yearOfEra, [NotNull] Era era) => eraCalculator.GetAbsoluteYear(yearOfEra, era);

        /// <summary>
        /// Returns the maximum valid year-of-era in the given era.
        /// </summary>
        /// <remarks>Note that depending on the calendar system, it's possible that only
        /// part of the returned year falls within the given era. It is also possible that
        /// the returned value represents the earliest year of the era rather than the latest
        /// year. (See the BC era in the Gregorian calendar, for example.)</remarks>
        /// <param name="era">The era in which to find the greatest year</param>
        /// <returns>The maximum valid year in the given era.</returns>
        /// <exception cref="ArgumentException"><paramref name="era"/> is not an era used in this calendar.</exception>
        public int GetMaxYearOfEra([NotNull] Era era) => eraCalculator.GetMaxYearOfEra(era);

        /// <summary>
        /// Returns the minimum valid year-of-era in the given era.
        /// </summary>
        /// <remarks>Note that depending on the calendar system, it's possible that only
        /// part of the returned year falls within the given era. It is also possible that
        /// the returned value represents the latest year of the era rather than the earliest
        /// year. (See the BC era in the Gregorian calendar, for example.)</remarks>
        /// <param name="era">The era in which to find the greatest year</param>
        /// <returns>The minimum valid year in the given eraera.</returns>
        /// <exception cref="ArgumentException"><paramref name="era"/> is not an era used in this calendar.</exception>
        public int GetMinYearOfEra([NotNull] Era era) => eraCalculator.GetMinYearOfEra(era);

        #endregion

        internal YearMonthDayCalculator YearMonthDayCalculator { get; }

        internal YearMonthDayCalendar GetYearMonthDayCalendarFromDaysSinceEpoch(int daysSinceEpoch)
        {
            Preconditions.CheckArgumentRange(nameof(daysSinceEpoch), daysSinceEpoch, MinDays, MaxDays);
            return YearMonthDayCalculator.GetYearMonthDay(daysSinceEpoch).WithCalendarOrdinal(Ordinal);
        }

        #region object overrides

        /// <summary>
        /// Converts this calendar system to text by simply returning its unique ID.
        /// </summary>
        /// <returns>The ID of this calendar system.</returns>
        public override string ToString() => Id;

        #endregion

        /// <summary>
        /// Returns the number of days since the Unix epoch (1970-01-01 ISO) for the given date.
        /// </summary>
        internal int GetDaysSinceEpoch([Trusted] YearMonthDay yearMonthDay)
        {
            DebugValidateYearMonthDay(yearMonthDay);
            return YearMonthDayCalculator.GetDaysSinceEpoch(yearMonthDay);
        }

        /// <summary>
        /// Returns the IsoDayOfWeek corresponding to the day of week for the given year, month and day.
        /// </summary>
        /// <param name="yearMonthDay">The year, month and day to use to find the day of the week</param>
        /// <returns>The day of the week as an IsoDayOfWeek</returns>
        internal IsoDayOfWeek GetDayOfWeek([Trusted] YearMonthDay yearMonthDay)
        {
            DebugValidateYearMonthDay(yearMonthDay);
            int daysSinceEpoch = YearMonthDayCalculator.GetDaysSinceEpoch(yearMonthDay);
            int numericDayOfWeek = unchecked(daysSinceEpoch >= -3 ? 1 + ((daysSinceEpoch + 3) % 7)
                                           : 7 + ((daysSinceEpoch + 4) % 7));
            return (IsoDayOfWeek) numericDayOfWeek;
        }

        /// <summary>
        /// Returns the number of days in the given year.
        /// </summary>
        /// <param name="year">The year to determine the number of days in</param>
        /// <exception cref="ArgumentOutOfRangeException">The given year is invalid for this calendar.</exception>
        /// <returns>The number of days in the given year.</returns>
        public int GetDaysInYear(int year)
        {
            Preconditions.CheckArgumentRange(nameof(year), year, MinYear, MaxYear);
            return YearMonthDayCalculator.GetDaysInYear(year);
        }

        /// <summary>
        /// Returns the number of days in the given month within the given year.
        /// </summary>
        /// <param name="year">The year in which to consider the month</param>
        /// <param name="month">The month to determine the number of days in</param>
        /// <exception cref="ArgumentOutOfRangeException">The given year / month combination
        /// is invalid for this calendar.</exception>
        /// <returns>The number of days in the given month and year.</returns>
        public int GetDaysInMonth(int year, int month)
        {
            // Simplest way to validate the year and month. Assume it's quick enough to validate the day...
            ValidateYearMonthDay(year, month, 1);
            return YearMonthDayCalculator.GetDaysInMonth(year, month);
        }

        /// <summary>
        /// Returns whether or not the given year is a leap year in this calendar.
        /// </summary>
        /// <param name="year">The year to consider.</param>
        /// <exception cref="ArgumentOutOfRangeException">The given year is invalid for this calendar.
        /// Note that some implementations may return a value rather than throw this exception. Failure to throw an
        /// exception should not be treated as an indication that the year is valid.</exception>
        /// <returns>True if the given year is a leap year; false otherwise.</returns>
        public bool IsLeapYear(int year)
        {
            Preconditions.CheckArgumentRange(nameof(year), year, MinYear, MaxYear);
            return YearMonthDayCalculator.IsLeapYear(year);
        }

        /// <summary>
        /// Returns the maximum valid month (inclusive) within this calendar in the given year.
        /// </summary>
        /// <remarks>
        /// It is assumed that in all calendars, every month between 1 and this month
        /// number is valid for the given year. This does not necessarily mean that the first month of the year
        /// is 1, however. (See the Hebrew calendar system using the scriptural month numbering system for example.)
        /// </remarks>
        /// <param name="year">The year to consider.</param>
        /// <exception cref="ArgumentOutOfRangeException">The given year is invalid for this calendar.
        /// Note that some implementations may return a month rather than throw this exception (for example, if all
        /// years have the same number of months in this calendar system). Failure to throw an exception should not be
        /// treated as an indication that the year is valid.</exception>
        /// <returns>The maximum month number within the given year.</returns>
        public int GetMonthsInYear(int year)
        {
            Preconditions.CheckArgumentRange(nameof(year), year, MinYear, MaxYear);
            return YearMonthDayCalculator.GetMonthsInYear(year);
        }

        internal void ValidateYearMonthDay(int year, int month, int day)
        {
            YearMonthDayCalculator.ValidateYearMonthDay(year, month, day);
        }

        internal int Compare([Trusted] YearMonthDay lhs, [Trusted] YearMonthDay rhs)
        {
            DebugValidateYearMonthDay(lhs);
            DebugValidateYearMonthDay(rhs);
            return YearMonthDayCalculator.Compare(lhs, rhs);
        }

        #region "Getter" methods which used to be DateTimeField

        internal int GetDayOfYear([Trusted] YearMonthDay yearMonthDay)
        {
            DebugValidateYearMonthDay(yearMonthDay);
            return YearMonthDayCalculator.GetDayOfYear(yearMonthDay);
        }

        internal int GetYearOfEra([Trusted] int absoluteYear)
        {
            Preconditions.DebugCheckArgumentRange(nameof(absoluteYear), absoluteYear, MinYear, MaxYear);
            return eraCalculator.GetYearOfEra(absoluteYear);
        }

        internal Era GetEra([Trusted] int absoluteYear)
        {
            Preconditions.DebugCheckArgumentRange(nameof(absoluteYear), absoluteYear, MinYear, MaxYear);
            return eraCalculator.GetEra(absoluteYear);
        }

        /// <summary>
        /// In debug configurations only, this method calls <see cref="ValidateYearMonthDay"/>
        /// with the components of the given YearMonthDay, ensuring that it's valid in the
        /// current calendar.
        /// </summary>
        /// <param name="yearMonthDay">The value to validate.</param>
        [Conditional("DEBUG")]
#if NET45
        [ExcludeFromCodeCoverage]
#endif
        internal void DebugValidateYearMonthDay(YearMonthDay yearMonthDay)
        {
            // Avoid the line even being compiled in a release build...
#if DEBUG
            ValidateYearMonthDay(yearMonthDay.Year, yearMonthDay.Month, yearMonthDay.Day);
#endif
        }

        #endregion

        /// <summary>
        /// Returns a Gregorian calendar system.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Gregorian calendar system defines every
        /// fourth year as leap, unless the year is divisible by 100 and not by 400.
        /// This improves upon the Julian calendar leap year rule.
        /// </para>
        /// <para>
        /// Although the Gregorian calendar did not exist before 1582 CE, this
        /// calendar system assumes it did, thus it is proleptic. This implementation also
        /// fixes the start of the year at January 1.
        /// </para>
        /// </remarks>
        /// <value>A Gregorian calendar system.</value>
        [NotNull] public static CalendarSystem Gregorian => GregorianJulianCalendars.Gregorian;

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
        /// <value>A suitable Julian calendar reference; the same reference may be returned by several
        /// calls as the object is immutable and thread-safe.</value>
        [NotNull] public static CalendarSystem Julian => GregorianJulianCalendars.Julian;

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
        /// <value>A suitable Coptic calendar reference; the same reference may be returned by several
        /// calls as the object is immutable and thread-safe.</value>
        [NotNull] public static CalendarSystem Coptic => MiscellaneousCalendars.Coptic;

        /// <summary>
        /// Returns an Islamic calendar system equivalent to the one used by the BCL HijriCalendar.
        /// </summary>
        /// <remarks>
        /// This uses the <see cref="IslamicLeapYearPattern.Base16"/> leap year pattern and the
        /// <see cref="IslamicEpoch.Astronomical"/> epoch. This is equivalent to HijriCalendar
        /// when the <c>HijriCalendar.HijriAdjustment</c> is 0.
        /// </remarks>
        /// <seealso cref="CalendarSystem.GetIslamicCalendar"/>
        /// <value>An Islamic calendar system equivalent to the one used by the BCL.</value>
        [NotNull] public static CalendarSystem IslamicBcl => GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Astronomical);

        /// <summary>
        /// Returns a Persian (also known as Solar Hijri) calendar system implementing the behaviour of the
        /// BCL <c>PersianCalendar</c> before .NET 4.6, and the sole Persian calendar in Noda Time 1.3.
        /// </summary>
        /// <remarks>
        /// This implementation uses a simple 33-year leap cycle, where years  1, 5, 9, 13, 17, 22, 26, and 30
        /// in each cycle are leap years.
        /// </remarks>
        /// <value>A Persian calendar system using a simple 33-year leap cycle.</value>
        [NotNull] public static CalendarSystem PersianSimple => PersianCalendars.Simple;

        /// <summary>
        /// Returns a Persian (also known as Solar Hijri) calendar system implementing the behaviour of the
        /// BCL <c>PersianCalendar</c> from .NET 4.6 onwards (and Windows 10), and the astronomical
        /// system described in Wikipedia and Calendrical Calculations.
        /// </summary>
        /// <remarks>
        /// This implementation uses data derived from the .NET 4.6 implementation (with the data built into Noda Time, so there's
        /// no BCL dependency) for simplicity; the actual implementation involves computing the time of noon in Iran, and
        /// is complex.
        /// </remarks>
        /// <value>A Persian calendar system using astronomical calculations to determine leap years.</value>
        [NotNull] public static CalendarSystem PersianArithmetic => PersianCalendars.Arithmetic;

        /// <summary>
        /// Returns a Persian (also known as Solar Hijri) calendar system implementing the behaviour
        /// proposed by Ahmad Birashk with nested cycles of years determining which years are leap years.
        /// </summary>
        /// <remarks>
        /// This calendar is also known as the algorithmic Solar Hijri calendar.
        /// </remarks>
        /// <value>A Persian calendar system using cycles-within-cycles of years to determine leap years.</value>
        [NotNull] public static CalendarSystem PersianAstronomical => PersianCalendars.Astronomical;

        /// <summary>
        /// Returns a Hebrew calendar system using the civil month numbering,
        /// equivalent to the one used by the BCL HebrewCalendar.
        /// </summary>
        /// <seealso cref="CalendarSystem.GetHebrewCalendar"/>
        /// <value>A Hebrew calendar system using the civil month numbering, equivalent to the one used by the
        /// BCL.</value>
        [NotNull] public static CalendarSystem HebrewCivil => GetHebrewCalendar(HebrewMonthNumbering.Civil);

        /// <summary>
        /// Returns a Hebrew calendar system using the scriptural month numbering.
        /// </summary>
        /// <seealso cref="CalendarSystem.GetHebrewCalendar"/>
        /// <value>A Hebrew calendar system using the scriptural month numbering.</value>
        [NotNull] public static CalendarSystem HebrewScriptural => GetHebrewCalendar(HebrewMonthNumbering.Scriptural);

        /// <summary>
        /// Returns an Um Al Qura calendar system - an Islamic calendar system primarily used by
        /// Saudi Arabia.
        /// </summary>
        /// <remarks>
        /// This is a tabular calendar, relying on pregenerated data.
        /// </remarks>
        /// <value>A calendar system for the Um Al Qura calendar.</value>
        [NotNull] public static CalendarSystem UmAlQura => MiscellaneousCalendars.UmAlQura;

        // "Holder" classes for lazy initialization of calendar systems

        private static class PersianCalendars
        {
            internal static readonly CalendarSystem Simple =
                new CalendarSystem(CalendarOrdinal.PersianSimple, PersianSimpleId, PersianName, new PersianYearMonthDayCalculator.Simple(), Era.AnnoPersico);
            internal static readonly CalendarSystem Arithmetic =
                new CalendarSystem(CalendarOrdinal.PersianArithmetic, PersianArithmeticId, PersianName, new PersianYearMonthDayCalculator.Arithmetic(), Era.AnnoPersico);
            internal static readonly CalendarSystem Astronomical =
                new CalendarSystem(CalendarOrdinal.PersianAstronomical, PersianAstronomicalId, PersianName, new PersianYearMonthDayCalculator.Astronomical(), Era.AnnoPersico);

            // Static constructor to enforce laziness.
            static PersianCalendars() {}
        }

        /// <summary>
        /// Specifically the calendars implemented by IslamicYearMonthDayCalculator, as opposed to all
        /// Islam-based calendars (which would include UmAlQura and Persian, for example).
        /// </summary>
        private static class IslamicCalendars
        {
            internal static readonly CalendarSystem[,] ByLeapYearPatterAndEpoch;

            static IslamicCalendars()
            {
                ByLeapYearPatterAndEpoch = new CalendarSystem[4, 2];
                for (int i = 1; i <= 4; i++)
                {
                    for (int j = 1; j <= 2; j++)
                    {
                        var leapYearPattern = (IslamicLeapYearPattern) i;
                        var epoch = (IslamicEpoch) j;
                        var calculator = new IslamicYearMonthDayCalculator((IslamicLeapYearPattern) i, (IslamicEpoch) j);
                        CalendarOrdinal ordinal = CalendarOrdinal.IslamicAstronomicalBase15 + (i - 1) + (j - 1) * 4;
                        ByLeapYearPatterAndEpoch[i - 1, j - 1] = new CalendarSystem(ordinal, GetIslamicId(leapYearPattern, epoch), IslamicName, calculator, Era.AnnoHegirae);
                    }
                }
            }
        }

        /// <summary>
        /// Odds and ends, with an assumption that it's not *that* painful to initialize UmAlQura if you only
        /// need Coptic, for example.
        /// </summary>
        private static class MiscellaneousCalendars
        {
            internal static readonly CalendarSystem Coptic =
                new CalendarSystem(CalendarOrdinal.Coptic, CopticId, CopticName, new CopticYearMonthDayCalculator(), Era.AnnoMartyrum);
            internal static readonly CalendarSystem UmAlQura =
                new CalendarSystem(CalendarOrdinal.UmAlQura, UmAlQuraId, UmAlQuraName, new UmAlQuraYearMonthDayCalculator(), Era.AnnoHegirae);
            internal static readonly CalendarSystem Badi =
                new CalendarSystem(CalendarOrdinal.Badi, BadiId, BadiName, new BadiYearMonthDayCalculator(), Era.Bahai);

            // Static constructor to enforce laziness. This used to be important to avoid a Heisenbug.
            // I don't believe it's strictly required now, but it does no harm and I don't want to go
            // through the pain I went through before. Besides, very few users will actually want these
            // calendars, so making this fully lazy avoids unnecessary initialization.
            static MiscellaneousCalendars() { }
        }

        private static class GregorianJulianCalendars
        {
            internal static readonly CalendarSystem Gregorian;
            internal static readonly CalendarSystem Julian;

            static GregorianJulianCalendars()
            {
                var julianCalculator = new JulianYearMonthDayCalculator();
                Julian = new CalendarSystem(CalendarOrdinal.Julian, JulianId, JulianName, julianCalculator, new GJEraCalculator(julianCalculator));
                Gregorian = new CalendarSystem(CalendarOrdinal.Gregorian, GregorianId, GregorianName, IsoCalendarSystem.YearMonthDayCalculator, IsoCalendarSystem.eraCalculator);
            }
        }

        private static class HebrewCalendars
        {
            internal static readonly CalendarSystem[] ByMonthNumbering =
            {
                new CalendarSystem(CalendarOrdinal.HebrewCivil, HebrewCivilId, HebrewName, new HebrewYearMonthDayCalculator(HebrewMonthNumbering.Civil), Era.AnnoMundi),
                new CalendarSystem(CalendarOrdinal.HebrewScriptural, HebrewScripturalId, HebrewName, new HebrewYearMonthDayCalculator(HebrewMonthNumbering.Scriptural), Era.AnnoMundi)
            };

            // Static constructor to enforce laziness.
            static HebrewCalendars() { }
        }
    }
}
