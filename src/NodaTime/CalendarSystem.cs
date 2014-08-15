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
        #region Public factory members for calendars
        private const string GregorianName = "Gregorian";
        private const string IsoName = "ISO";
        private const string CopticName = "Coptic";
        private const string JulianName = "Julian";
        private const string IslamicName = "Hijri";
        private const string PersianName = "Persian";
        private const string HebrewName = "Hebrew";
        private const string HebrewCivilId = HebrewName + " Civil";
        private const string HebrewScripturalId = HebrewName + " Scriptural";
        private const string UmAlQuraName = "Um Al Qura";

        private static readonly CalendarSystem[] GregorianCalendarSystems;
        private static readonly CalendarSystem CopticCalendarSystem;
        private static readonly CalendarSystem JulianCalendarSystem;
        private static readonly CalendarSystem[,] IslamicCalendarSystems;
        private static readonly CalendarSystem IsoCalendarSystem;
        private static readonly CalendarSystem PersianCalendarSystem;
        private static readonly CalendarSystem[] HebrewCalendarSystems;
        private static readonly CalendarSystem UmAlQuraCalendarSystem;
        private static readonly CalendarSystem[] CalendarByOrdinal;

        static CalendarSystem()
        {
            CalendarByOrdinal = new CalendarSystem[(int) CalendarOrdinal.Size];
            var gregorianCalculator = new GregorianYearMonthDayCalculator();
            var gregorianEraCalculator = new GJEraCalculator(gregorianCalculator);
            IsoCalendarSystem = new CalendarSystem(CalendarOrdinal.Iso, IsoName, IsoName, gregorianCalculator, 4, gregorianEraCalculator);
            PersianCalendarSystem = new CalendarSystem(CalendarOrdinal.Persian, PersianName, PersianName, new PersianYearMonthDayCalculator(), Era.AnnoPersico);
            CopticCalendarSystem = new CalendarSystem(CalendarOrdinal.Coptic, CopticName, CopticName, new CopticYearMonthDayCalculator(), Era.AnnoMartyrum);
            var julianCalculator = new JulianYearMonthDayCalculator();
            JulianCalendarSystem = new CalendarSystem(CalendarOrdinal.Julian, JulianName, JulianName, new JulianYearMonthDayCalculator(), 4, new GJEraCalculator(julianCalculator));
            UmAlQuraCalendarSystem = UmAlQuraYearMonthDayCalculator.IsSupported ? new CalendarSystem(CalendarOrdinal.UmAlQura, UmAlQuraName, UmAlQuraName, new UmAlQuraYearMonthDayCalculator(), Era.AnnoHegirae) : null;
            HebrewCalendarSystems = new[]
            {
                new CalendarSystem(CalendarOrdinal.HebrewCivil, HebrewCivilId, HebrewName, new HebrewYearMonthDayCalculator(HebrewMonthNumbering.Civil), Era.AnnoMundi),
                new CalendarSystem(CalendarOrdinal.HebrewScriptural, HebrewScripturalId, HebrewName, new HebrewYearMonthDayCalculator(HebrewMonthNumbering.Scriptural), Era.AnnoMundi)
            };

            // Variations for the calendar systems which have different objects for different "minimum first day of week"
            // values. These share eras and year/month/day calculators where appropriate.
            GregorianCalendarSystems = new CalendarSystem[7];
            for (int i = 1; i <= 7; i++)
            {
                // CalendarOrdinal is set up to make this simple :)
                GregorianCalendarSystems[i - 1] = new CalendarSystem((CalendarOrdinal) i, GregorianName, gregorianCalculator, i, gregorianEraCalculator);
            }
            IslamicCalendarSystems = new CalendarSystem[4, 2];
            for (int i = 1; i <= 4; i++)
            {
                for (int j = 1; j <= 2; j++)
                {
                    var leapYearPattern = (IslamicLeapYearPattern)i;
                    var epoch = (IslamicEpoch)j;
                    var calculator = new IslamicYearMonthDayCalculator((IslamicLeapYearPattern)i, (IslamicEpoch)j);
                    string id = String.Format(CultureInfo.InvariantCulture, "{0} {1}-{2}", IslamicName, epoch, leapYearPattern);
                    CalendarOrdinal ordinal = (CalendarOrdinal) (8 + i + j * 4);
                    IslamicCalendarSystems[i - 1, j - 1] = new CalendarSystem(ordinal, id, IslamicName, calculator, Era.AnnoHegirae);
                }
            }
        }

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
        public static CalendarSystem ForId(string id)
        {
            Func<CalendarSystem> factory;
            if (!IdToFactoryMap.TryGetValue(id, out factory))
            {
                throw new KeyNotFoundException(String.Format("No calendar system for ID {0} exists", id));
            }
            return factory();
        }

        /// <summary>
        /// Fetches a calendar system by its ordinal value. Note that this currently assumes eager instantiation
        /// of all calendars. We may need to convert this to a switch statement if we change to use lazy instantiation.
        /// </summary>
        internal static CalendarSystem ForOrdinal([Trusted] CalendarOrdinal ordinal)
        {
            // Avoid an array lookup for the overwhelmingly common case.
            if (ordinal == CalendarOrdinal.Iso)
            {
                return IsoCalendarSystem;
            }
            CalendarSystem calendar = CalendarByOrdinal[(int) ordinal];
            if (calendar == null)
            {
                throw new NotSupportedException("Calendar " + ordinal + " is not supported on this platform");
            }
            return calendar;
        }

        /// <summary>
        /// Returns the IDs of all calendar systems available within Noda Time. The order of the keys is not guaranteed.
        /// </summary>
        public static IEnumerable<string> Ids { get { return IdToFactoryMap.Keys; } }

        private static readonly Dictionary<string, Func<CalendarSystem>> IdToFactoryMap = new Dictionary<string, Func<CalendarSystem>>
        {
            { IsoName, () => Iso },
            { PersianName, () => Persian },
            { HebrewCivilId, () => GetHebrewCalendar(HebrewMonthNumbering.Civil) },
            { HebrewScripturalId, () => GetHebrewCalendar(HebrewMonthNumbering.Scriptural) },
            { GregorianName + " 1", () => GetGregorianCalendar(1) },
            { GregorianName + " 2", () => GetGregorianCalendar(2) },
            { GregorianName + " 3", () => GetGregorianCalendar(3) },
            { GregorianName + " 4", () => GetGregorianCalendar(4) },
            { GregorianName + " 5", () => GetGregorianCalendar(5) },
            { GregorianName + " 6", () => GetGregorianCalendar(6) },
            { GregorianName + " 7", () => GetGregorianCalendar(7) },
            { CopticName, () => Coptic },
            { JulianName, () => Julian },
            { UmAlQuraName, () => UmAlQura }, 
            { IslamicName + " Civil-Indian", () => GetIslamicCalendar(IslamicLeapYearPattern.Indian, IslamicEpoch.Civil) },
            { IslamicName + " Civil-Base15", () => GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Civil) },
            { IslamicName + " Civil-Base16", () => GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Civil) },
            { IslamicName + " Civil-HabashAlHasib", () => GetIslamicCalendar(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Civil) },
            { IslamicName + " Astronomical-Indian", () => GetIslamicCalendar(IslamicLeapYearPattern.Indian, IslamicEpoch.Astronomical) },
            { IslamicName + " Astronomical-Base15", () => GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Astronomical) },
            { IslamicName + " Astronomical-Base16", () => GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Astronomical) },
            { IslamicName + " Astronomical-HabashAlHasib", () => GetIslamicCalendar(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Astronomical) },
        };

        /// <summary>
        /// Returns a calendar system that follows the rules of the ISO-8601 standard,
        /// which is compatible with Gregorian for all modern dates.
        /// </summary>
        /// <remarks>
        /// As of Noda Time 2.0, this calendar system is equivalent to the Gregorian calendar system
        /// with a "minimum number of days in the first week" of 4. The only areas in which the calendars differed
        /// were around centuries, and the members relating to those differences were removed in Noda Time 2.0.
        /// The distinction between Gregorian-4 and ISO has been maintained for the sake of simplicity, compatibility
        /// and consistency.
        /// </remarks>
        public static CalendarSystem Iso { get { return IsoCalendarSystem; } }

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
        public static CalendarSystem GetHebrewCalendar(HebrewMonthNumbering monthNumbering)
        {
            Preconditions.CheckArgumentRange("monthNumbering", (int) monthNumbering, 1, 2);
            return HebrewCalendarSystems[((int) monthNumbering) - 1];
        }

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
            Preconditions.CheckArgumentRange("minDaysInFirstWeek", minDaysInFirstWeek, 1, 7);
            return GregorianCalendarSystems[minDaysInFirstWeek - 1];
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
            Preconditions.CheckArgumentRange("leapYearPattern", (int) leapYearPattern, 1, 4);
            Preconditions.CheckArgumentRange("epoch", (int) epoch, 1, 2);
            return IslamicCalendarSystems[(int) leapYearPattern - 1, (int) epoch - 1];
        }
        #endregion

        private readonly CalendarOrdinal ordinal;
        private readonly YearMonthDayCalculator yearMonthDayCalculator;
        private readonly WeekYearCalculator weekYearCalculator;
        private readonly string id;
        private readonly string name;
        private readonly EraCalculator eraCalculator;
        private readonly int minYear;
        private readonly int maxYear;
        private readonly int minDays;
        private readonly int maxDays;

        private CalendarSystem(CalendarOrdinal ordinal, string id, string name, YearMonthDayCalculator yearMonthDayCalculator, Era singleEra)
            : this(ordinal, id, name, yearMonthDayCalculator, 4, new SingleEraCalculator(singleEra, yearMonthDayCalculator))
        {
        }

        private CalendarSystem(CalendarOrdinal ordinal, string name, YearMonthDayCalculator yearMonthDayCalculator, int minDaysInFirstWeek, EraCalculator eraCalculator)
            : this(ordinal, String.Format(CultureInfo.InvariantCulture, "{0} {1}", name, minDaysInFirstWeek),
                   name, yearMonthDayCalculator, minDaysInFirstWeek, eraCalculator)
        {
        }

        private CalendarSystem(CalendarOrdinal ordinal, string id, string name, YearMonthDayCalculator yearMonthDayCalculator, int minDaysInFirstWeek, EraCalculator eraCalculator)
        {
            this.ordinal = ordinal;
            this.id = id;
            this.name = name;
            this.yearMonthDayCalculator = yearMonthDayCalculator;
            this.weekYearCalculator = new WeekYearCalculator(yearMonthDayCalculator, minDaysInFirstWeek);
            this.minYear = yearMonthDayCalculator.MinYear;
            this.maxYear = yearMonthDayCalculator.MaxYear;
            this.minDays = yearMonthDayCalculator.GetStartOfYearInDays(minYear);
            this.maxDays = yearMonthDayCalculator.GetStartOfYearInDays(maxYear + 1) - 1;
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
        ///     <description>Equivalent factory method</description>
        ///   </listheader>
        ///   <item><term>ISO</term><description><see cref="CalendarSystem.Iso"/></description></item>
        ///   <item><term>Gregorian 1</term><description><see cref="CalendarSystem.GetGregorianCalendar"/>(1)</description></item>
        ///   <item><term>Gregorian 2</term><description><see cref="CalendarSystem.GetGregorianCalendar"/>(2)</description></item>
        ///   <item><term>Gregorian 3</term><description><see cref="CalendarSystem.GetGregorianCalendar"/>(3)</description></item>
        ///   <item><term>Gregorian 4</term><description><see cref="CalendarSystem.GetGregorianCalendar"/>(4)</description></item>
        ///   <item><term>Gregorian 5</term><description><see cref="CalendarSystem.GetGregorianCalendar"/>(5)</description></item>
        ///   <item><term>Gregorian 6</term><description><see cref="CalendarSystem.GetGregorianCalendar"/>(6)</description></item>
        ///   <item><term>Gregorian 7</term><description><see cref="CalendarSystem.GetGregorianCalendar"/>(7)</description></item>
        ///   <item><term>Coptic</term><description><see cref="CalendarSystem.Coptic"/>()</description></item>
        ///   <item><term>Julian</term><description><see cref="CalendarSystem.Julian"/>()</description></item>
        ///   <item><term>Hijri Civil-Indian</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Indian, IslamicEpoch.Civil)</description></item>
        ///   <item><term>Hijri Civil-Base15</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Base15, IslamicEpoch.Civil)</description></item>
        ///   <item><term>Hijri Civil-Base16</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Base16, IslamicEpoch.Civil)</description></item>
        ///   <item><term>Hijri Civil-HabashAlHasib</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Civil)</description></item>
        ///   <item><term>Hijri Astronomical-Indian</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Indian, IslamicEpoch.Astronomical)</description></item>
        ///   <item><term>Hijri Astronomical-Base15</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Base15, IslamicEpoch.Astronomical)</description></item>
        ///   <item><term>Hijri Astronomical-Base16</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.Base16, IslamicEpoch.Astronomical)</description></item>
        ///   <item><term>Hijri Astronomical-HabashAlHasib</term><description><see cref="CalendarSystem.GetIslamicCalendar"/>(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Astronomical)</description></item>
        ///   <item><term>Persian</term><description><see cref="CalendarSystem.Persian"/>()</description></item>
        ///   <item><term>Um Al Qura</term><description><see cref="CalendarSystem.UmAlQura"/>()</description></item>
        ///   <item><term>Hebrew Civil</term><description><see cref="CalendarSystem.GetHebrewCalendar"/>(HebrewMonthNumbering.Civil)</description></item>
        ///   <item><term>Hebrew Scriptural</term><description><see cref="CalendarSystem.GetHebrewCalendar"/>(HebrewMonthNumbering.Scriptural)</description></item>
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
        public bool UsesIsoDayOfWeek { get { return true; } }

        /// <summary>
        /// The minimum valid year (inclusive) within this calendar.
        /// </summary>
        public int MinYear { get { return minYear; } }

        /// <summary>
        /// The maximum valid year (inclusive) within this calendar.
        /// </summary>
        public int MaxYear { get { return maxYear; } }

        /// <summary>
        /// Returns the minimum day number this calendar can handle.
        /// </summary>
        internal int MinDays { get { return minDays; } }

        /// <summary>
        /// Returns the maximum day number (inclusive) this calendar can handle.
        /// </summary>
        internal int MaxDays { get { return maxDays; } }

        /// <summary>
        /// Returns the ordinal value of this calendar.
        /// </summary>
        internal CalendarOrdinal Ordinal { get { return ordinal; } }

        #region Era-based members
        /// <summary>
        /// Returns a read-only list of eras used in this calendar system.
        /// </summary>
        public IList<Era> Eras { get { return eraCalculator.Eras; } }

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
        public int GetAbsoluteYear(int yearOfEra, [NotNull] Era era)
        {
            return eraCalculator.GetAbsoluteYear(yearOfEra, era);
        }

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
        public int GetMaxYearOfEra([NotNull] Era era)
        {
            return eraCalculator.GetMaxYearOfEra(era);
        }

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
        public int GetMinYearOfEra([NotNull] Era era)
        {
            return eraCalculator.GetMinYearOfEra(era);
        }

        // TODO(2.0): Check we want this. It would be useful for eras which don't start at the beginning of year,
        // as per the Japanese calendar, but we don't have any of those yet.
        /// <summary>
        /// Returns the first valid <see cref="LocalDate"/> of the given era.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="era"/> is not an era used in this calendar.</exception>
        /// <returns>The start of the era.</returns>
        public LocalDate GetStartOfEra([NotNull] Era era)
        {
            return new LocalDate(eraCalculator.GetStartOfEra(era), this);
        }

        // TODO(2.0): Check we want this. It would be useful for eras which don't start at the beginning of year,
        // as per the Japanese calendar, but we don't have any of those yet. If we do want it, we could consider
        // using DateInterval instead of having two methods.
        /// <summary>
        /// Returns the last valid <see cref="LocalDate"/> of the given era.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="era"/> is not an era used in this calendar.</exception>
        /// <returns>The end of the era.</returns>
        public LocalDate GetEndOfEra([NotNull] Era era)
        {
            return new LocalDate(eraCalculator.GetEndOfEra(era), this);
        }
        #endregion

        internal YearMonthDayCalculator YearMonthDayCalculator { get { return yearMonthDayCalculator; } }

        /// <summary>
        /// Returns the local date corresponding to the given "week year", "week of week year", and "day of week"
        /// in this calendar system.
        /// </summary>
        /// <param name="weekYear">ISO-8601 week year of value to return</param>
        /// <param name="weekOfWeekYear">ISO-8601 week of week year of value to return</param>
        /// <param name="dayOfWeek">ISO-8601 day of week to return</param>
        /// <returns>The date corresponding to the given week year / week of week year / day of week.</returns>
        internal YearMonthDay GetYearMonthDayFromWeekYearWeekAndDayOfWeek(int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek)
        {
            return weekYearCalculator.GetYearMonthDay(weekYear, weekOfWeekYear, dayOfWeek);
        }

        internal YearMonthDay GetYearMonthDayFromDaysSinceEpoch(int daysSinceEpoch)
        {
            Preconditions.CheckArgumentRange("daysSinceEpoch", daysSinceEpoch, minDays, maxDays);
            return yearMonthDayCalculator.GetYearMonthDay(daysSinceEpoch);
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
        /// Returns the number of days since the Unix epoch (1970-01-01 ISO) for the given date.
        /// </summary>
        internal int GetDaysSinceEpoch([Trusted] YearMonthDay yearMonthDay)
        {
            DebugValidateYearMonthDay(yearMonthDay);
            return yearMonthDayCalculator.GetDaysSinceEpoch(yearMonthDay);
        }

        /// <summary>
        /// Returns the IsoDayOfWeek corresponding to the day of week for the given local instant
        /// if this calendar uses ISO days of the week, or throws an InvalidOperationException otherwise.
        /// </summary>
        /// <param name="yearMonthDay">The year, month and day to use to find the day of the week</param>
        /// <returns>The day of the week as an IsoDayOfWeek</returns>
        internal IsoDayOfWeek GetIsoDayOfWeek([Trusted] YearMonthDay yearMonthDay)
        {
            DebugValidateYearMonthDay(yearMonthDay);
            if (!UsesIsoDayOfWeek)
            {
                throw new InvalidOperationException("Calendar " + id + " does not use ISO days of the week");
            }
            return (IsoDayOfWeek) GetDayOfWeek(yearMonthDay);
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
            return yearMonthDayCalculator.GetDaysInMonth(year, month);
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
            Preconditions.CheckArgumentRange("year", year, minYear, maxYear);
            return yearMonthDayCalculator.IsLeapYear(year);
        }

        /// <summary>
        /// The maximum valid month (inclusive) within this calendar in the given year.
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
            Preconditions.CheckArgumentRange("year", year, minYear, maxYear);
            return yearMonthDayCalculator.GetMonthsInYear(year);
        }

        internal void ValidateYearMonthDay(int year, int month, int day)
        {
            yearMonthDayCalculator.ValidateYearMonthDay(year, month, day);
        }

        internal int Compare([Trusted] YearMonthDay lhs, [Trusted] YearMonthDay rhs)
        {
            DebugValidateYearMonthDay(lhs);
            DebugValidateYearMonthDay(rhs);
            return yearMonthDayCalculator.Compare(lhs, rhs);
        }

        #region "Getter" methods which used to be DateTimeField
        internal int GetDayOfWeek([Trusted] YearMonthDay yearMonthDay)
        {
            DebugValidateYearMonthDay(yearMonthDay);
            return weekYearCalculator.GetDayOfWeek(yearMonthDay);
        }

        internal int GetDayOfYear([Trusted] YearMonthDay yearMonthDay)
        {
            DebugValidateYearMonthDay(yearMonthDay);
            return yearMonthDayCalculator.GetDayOfYear(yearMonthDay);
        }

        internal int GetWeekOfWeekYear([Trusted] YearMonthDay yearMonthDay)
        {
            DebugValidateYearMonthDay(yearMonthDay);
            return weekYearCalculator.GetWeekOfWeekYear(yearMonthDay);
        }

        internal int GetWeekYear([Trusted] YearMonthDay yearMonthDay)
        {
            DebugValidateYearMonthDay(yearMonthDay);
            return weekYearCalculator.GetWeekYear(yearMonthDay);
        }

        internal int GetYearOfEra([Trusted] YearMonthDay yearMonthDay)
        {
            DebugValidateYearMonthDay(yearMonthDay);
            return eraCalculator.GetYearOfEra(yearMonthDay);
        }

        internal Era GetEra([Trusted] YearMonthDay yearMonthDay)
        {
            return eraCalculator.GetEra(yearMonthDay);
        }

        /// <summary>
        /// In debug configurations only, this method calls <see cref="ValidateYearMonthDay"/>
        /// with the components of the given YearMonthDay, ensuring that it's valid in the
        /// current calendar.
        /// </summary>
        /// <param name="yearMonthDay">The value to validate.</param>
        [Conditional("DEBUG")]
        internal void DebugValidateYearMonthDay(YearMonthDay yearMonthDay)
        {
            ValidateYearMonthDay(yearMonthDay.Year, yearMonthDay.Month, yearMonthDay.Day);
        }
        #endregion

        /// <summary>
        /// Returns a Gregorian calendar system with at least 4 days in the first week of a week-year.
        /// </summary>
        /// <seealso cref="CalendarSystem.GetGregorianCalendar"/>
        /// <returns>A Gregorian calendar system with at least 4 days in the first week of a week-year.</returns>
        public static CalendarSystem Gregorian { get { return GetGregorianCalendar(4); } }

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
        /// <para>
        /// This calendar always has at least 4 days in the first week of the week-year.
        /// </para>
        /// <returns>A suitable Julian calendar reference; the same reference may be returned by several
        /// calls as the object is immutable and thread-safe.</returns>
        public static CalendarSystem Julian { get { return JulianCalendarSystem; } }

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
        /// <para>
        /// This calendar always has at least 4 days in the first week of the week-year.
        /// </para>
        /// </remarks>
        /// <returns>A suitable Coptic calendar reference; the same reference may be returned by several
        /// calls as the object is immutable and thread-safe.</returns>
        public static CalendarSystem Coptic { get { return CopticCalendarSystem; } }

        /// <summary>
        /// Returns an Islamic calendar system equivalent to the one used by the BCL HijriCalendar.
        /// </summary>
        /// <remarks>
        /// This uses the <see cref="IslamicLeapYearPattern.Base16"/> leap year pattern and the
        /// <see cref="IslamicEpoch.Astronomical"/> epoch. This is equivalent to HijriCalendar
        /// when the HijriCalendar.HijriAdjustment is 0.
        /// </remarks>
        /// <seealso cref="CalendarSystem.GetIslamicCalendar"/>
        /// <returns>An Islamic calendar system equivalent to the one used by the BCL.</returns>
        public static CalendarSystem IslamicBcl
        {
            get
            {
                return GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Astronomical);
            }
        }

        /// <summary>
        /// Returns a Persian (also known as Solar Hijri) calendar system. This is the main calendar in Iran
        /// and Afghanistan, and is also used in some other countries where Persian is spoken.
        /// </summary>
        /// <remarks>
        /// The true Persian calendar is an astronomical one, where leap years depend on vernal equinox.
        /// A complicated algorithmic alternative approach exists, proposed by Ahmad Birashk,
        /// but this isn't generally used in society. The implementation here is somewhat simpler, using a
        /// 33-year leap cycle, where years  1, 5, 9, 13, 17, 22, 26, and 30 in each cycle are leap years.
        /// This is the same approach taken by the BCL <c>PersianCalendar</c> class, and the dates of
        /// this implementation align exactly with the BCL implementation.
        /// </remarks>
        /// <returns>A Persian calendar system.</returns>
        public static CalendarSystem Persian { get { return PersianCalendarSystem; } }

        /// <summary>
        /// Returns a Hebrew calendar system using the civil month numbering,
        /// equivalent to the one used by the BCL HebrewCalendar.
        /// </summary>
        /// <seealso cref="CalendarSystem.GetHebrewCalendar"/>
        /// <returns>A Hebrew calendar system using the civil month numbering, equivalent to the one used by the
        /// BCL.</returns>
        public static CalendarSystem HebrewCivil { get { return GetHebrewCalendar(HebrewMonthNumbering.Civil); } }

        /// <summary>
        /// Returns a Hebrew calendar system using the scriptural month numbering.
        /// </summary>
        /// <seealso cref="CalendarSystem.GetHebrewCalendar"/>
        /// <returns>A Hebrew calendar system using the scriptural month numbering.</returns>
        public static CalendarSystem HebrewScriptural { get { return GetHebrewCalendar(HebrewMonthNumbering.Scriptural); } }

#if PCL
        /// <summary>
        /// Returns an Um Al Qura calendar system - an Islamic calendar system primarily used by
        /// Saudi Arabia.
        /// </summary>
        public static CalendarSystem UmAlQura
#else
        /// <summary>
        /// Returns an Um Al Qura calendar system - an Islamic calendar system primarily used by
        /// Saudi Arabia.
        /// </summary>
        /// <remarks>
        /// This is a tabular calendar, which relies on data provided by the BCL
        /// <see cref="UmAlQuraCalendar" /> class during initialization.
        /// As such, some platforms do not support this calendar. In particular, the Mono implementation
        /// is known to be unreliable (at least as far as Mono 3.6.0). The calendar is available on
        /// some Portable Class Library variants, but not all. When in doubt, please test thoroughly
        /// on all platforms you intend to support.
        /// </remarks>
        /// <returns>A calendar system for the Um Al Qura calendar.</returns>
        /// <exception cref="NotSupportedException">The Um Al Qura calendar is not supported on the current platform.</exception>
        public static CalendarSystem UmAlQura
#endif
        {
            get
            {
                if (UmAlQuraCalendarSystem != null)
                {
                    return UmAlQuraCalendarSystem;
                }
                throw new NotSupportedException("The Um Al Qura calendar is not supported on your platform");
            }
        }
    }
}
