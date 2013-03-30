// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using NodaTime.Calendars;
using NodaTime.Properties;
using NodaTime.Text;
using NodaTime.Text.Patterns;
using NodaTime.Utility;

namespace NodaTime.Globalization
{
    /// <summary>
    /// A <see cref="IFormatProvider"/> for Noda Time types, initialised from a <see cref="CultureInfo"/>.
    /// This provides a single place defining how NodaTime values are formatted and displayed, depending on the culture.
    /// </summary>
    /// <remarks>
    /// Currently this is "shallow-immutable" - although none of these properties can be changed, the
    /// CultureInfo itself may be mutable. In the future we will make this fully immutable.
    /// </remarks>
    /// <threadsafety>Instances which use read-only CultureInfo instances are immutable,
    /// and may be used freely between threads. Instances with mutable cultures should not be shared between threads
    /// without external synchronization.
    /// See the thread safety section of the user guide for more information.</threadsafety>
    internal sealed class NodaFormatInfo : IFormatProvider
    {
        // Names that we can use to check for broken Mono behaviour.
        // The cloning is *also* to work around a Mono bug, where even read-only cultures can change...
        // See http://bugzilla.xamarin.com/show_bug.cgi?id=3279
        private static readonly string[] ShortInvariantMonthNames = (string[]) CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedMonthNames.Clone();
        private static readonly string[] LongInvariantMonthNames = (string[]) CultureInfo.InvariantCulture.DateTimeFormat.MonthNames.Clone();

        #region Patterns and pattern parsers
        private static readonly IPatternParser<Offset> GeneralOffsetPatternParser = new OffsetPatternParser();
        private static readonly IPatternParser<Instant> GeneralInstantPatternParser = new InstantPatternParser();
        private static readonly IPatternParser<LocalTime> GeneralLocalTimePatternParser = new LocalTimePatternParser(LocalTime.Midnight);
        private static readonly IPatternParser<LocalDate> GeneralLocalDatePatternParser = new LocalDatePatternParser(LocalDatePattern.DefaultTemplateValue);
        private static readonly IPatternParser<LocalDateTime> GeneralLocalDateTimePatternParser = new LocalDateTimePatternParser(LocalDateTimePattern.DefaultTemplateValue);

        // Not read-only as they need to be changed after cloning.
        private FixedFormatInfoPatternParser<Offset> offsetPatternParser;
        private FixedFormatInfoPatternParser<Instant> instantPatternParser;
        private FixedFormatInfoPatternParser<LocalTime> localTimePatternParser;
        private FixedFormatInfoPatternParser<LocalDate> localDatePatternParser;
        private FixedFormatInfoPatternParser<LocalDateTime> localDateTimePatternParser;
        #endregion

        /// <summary>
        /// A NodaFormatInfo wrapping the invariant culture.
        /// </summary>
        // Note: this must occur below the pattern parsers, to make type initialization work...
        public static readonly NodaFormatInfo InvariantInfo = new NodaFormatInfo(CultureInfo.InvariantCulture);

        // TODO(Post-V1): Reconsider everything about caching, cloning etc.
        private static readonly IDictionary<CultureInfo, NodaFormatInfo> Cache = new Dictionary<CultureInfo, NodaFormatInfo>(new ReferenceEqualityComparer<CultureInfo>());

        private readonly string description;
        private readonly DateTimeFormatInfo dateTimeFormat;
        private readonly NumberFormatInfo numberFormat;
        private readonly CultureInfo cultureInfo;
        private readonly string offsetPatternFull;
        private readonly string offsetPatternLong;
        private readonly string offsetPatternMedium;
        private readonly string offsetPatternShort;
#if PCL
        private readonly string dateSeparator;
        private readonly string timeSeparator;
#endif
        private readonly IList<string> longMonthNames;
        private readonly IList<string> longMonthGenitiveNames;
        private readonly IList<string> longDayNames;
        private readonly IList<string> shortMonthNames;
        private readonly IList<string> shortMonthGenitiveNames;
        private readonly IList<string> shortDayNames;

        // TODO(Post-V1): Have a single EraDescription class and one dictionary?
        private readonly Dictionary<Era, IList<string>> eraNamesCache;
        private readonly Dictionary<Era, string> eraPrimaryNameCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodaFormatInfo" /> class.
        /// </summary>
        /// <param name="cultureInfo">The culture info to base this on.</param>
        internal NodaFormatInfo(CultureInfo cultureInfo)
        {
            Preconditions.CheckNotNull(cultureInfo, "cultureInfo");
            this.cultureInfo = cultureInfo;
            numberFormat = cultureInfo.NumberFormat;
            dateTimeFormat = cultureInfo.DateTimeFormat;
            Name = cultureInfo.Name;
            description = "NodaFormatInfo[" + cultureInfo.Name + "]";
            var manager = PatternResources.ResourceManager;
            offsetPatternFull = manager.GetString("OffsetPatternFull", cultureInfo);
            offsetPatternLong = manager.GetString("OffsetPatternLong", cultureInfo);
            offsetPatternMedium = manager.GetString("OffsetPatternMedium", cultureInfo);
            offsetPatternShort = manager.GetString("OffsetPatternShort", cultureInfo);

            offsetPatternParser = FixedFormatInfoPatternParser<Offset>.CreateCachingParser(GeneralOffsetPatternParser, this);
            instantPatternParser = FixedFormatInfoPatternParser<Instant>.CreateCachingParser(GeneralInstantPatternParser, this);
            localTimePatternParser = FixedFormatInfoPatternParser<LocalTime>.CreateCachingParser(GeneralLocalTimePatternParser, this);
            localDatePatternParser = FixedFormatInfoPatternParser<LocalDate>.CreateCachingParser(GeneralLocalDatePatternParser, this);
            localDateTimePatternParser = FixedFormatInfoPatternParser<LocalDateTime>.CreateCachingParser(GeneralLocalDateTimePatternParser, this);

            // Turn month names into 1-based read-only lists
            longMonthNames = ConvertMonthArray(cultureInfo.DateTimeFormat.MonthNames);
            shortMonthNames = ConvertMonthArray(cultureInfo.DateTimeFormat.AbbreviatedMonthNames);
            longMonthGenitiveNames = ConvertGenitiveMonthArray(longMonthNames, cultureInfo.DateTimeFormat.MonthGenitiveNames, LongInvariantMonthNames);
            shortMonthGenitiveNames = ConvertGenitiveMonthArray(shortMonthNames, cultureInfo.DateTimeFormat.AbbreviatedMonthGenitiveNames, ShortInvariantMonthNames);
            longDayNames = ConvertDayArray(cultureInfo.DateTimeFormat.DayNames);
            shortDayNames = ConvertDayArray(cultureInfo.DateTimeFormat.AbbreviatedDayNames);
            eraNamesCache = new Dictionary<Era, IList<string>>();
            eraPrimaryNameCache = new Dictionary<Era, string>();
#if PCL
            // Horrible, but it does the job...
            dateSeparator = DateTime.MinValue.ToString("%/", cultureInfo);
            timeSeparator = DateTime.MinValue.ToString("%:", cultureInfo);
#endif
        }

        /// <summary>
        /// The BCL returns arrays of month names starting at 0; we want a read-only list starting at 1 (with 0 as null).
        /// </summary>
        private static IList<string> ConvertMonthArray(string[] monthNames)
        {
            List<string> list = new List<string>(monthNames);
            list.Insert(0, null);
            return new ReadOnlyCollection<string>(list);
        }

        /// <summary>
        /// The BCL returns arrays of week names starting at 0 as Sunday; we want a read-only list starting at 1 (with 0 as null)
        /// and with 7 as Sunday.
        /// </summary>
        private static IList<string> ConvertDayArray(string[] dayNames)
        {
            List<string> list = new List<string>(dayNames);
            list.Add(dayNames[0]);
            list[0] = null;
            return new ReadOnlyCollection<string>(list);
        }

        /// <summary>
        /// Checks whether any of the genitive names differ from the non-genitive names, and returns
        /// either a reference to the non-genitive names or a converted list as per ConvertMonthArray.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Mono uses the invariant month names for the genitive month names by default, so we'll assume that
        /// if we see an invariant name, that *isn't* deliberately a genitive month name. A non-invariant culture
        /// which decided to have genitive month names exactly matching the invariant ones would be distinctly odd.
        /// See http://bugzilla.xamarin.com/show_bug.cgi?id=3278 for more details and progress.
        /// </para>
        /// <para>
        /// Mono 3.0.6 has an exciting and different bug, where all the abbreviated genitive month names are just numbers ("1" etc).
        /// So again, if we detect that, we'll go back to the non-genitive version.
        /// See http://bugzilla.xamarin.com/show_bug.cgi?id=11361 for more details and progress.
        /// </para>
        /// </remarks>
        private IList<string> ConvertGenitiveMonthArray(IList<string> nonGenitiveNames, string[] bclNames, string[] invariantNames)
        {
            int ignored;
            if (int.TryParse(bclNames[0], out ignored))
            {
                return nonGenitiveNames;
            }
            for (int i = 0; i < bclNames.Length; i++)
            {
                if (bclNames[i] != nonGenitiveNames[i + 1] && bclNames[i] != invariantNames[i])
                {
                    return ConvertMonthArray(bclNames);
                }
            }
            return nonGenitiveNames;
        }

        /// <summary>
        /// Gets the culture info associated with this format provider.
        /// </summary>
        public CultureInfo CultureInfo { get { return cultureInfo; } }

        /// <summary>
        /// Gets the text comparison information associated with this format provider.
        /// </summary>
        public CompareInfo CompareInfo { get { return cultureInfo.CompareInfo; } }

        internal FixedFormatInfoPatternParser<Offset> OffsetPatternParser { get { return offsetPatternParser; } }
        internal FixedFormatInfoPatternParser<Instant> InstantPatternParser { get { return instantPatternParser; } }
        internal FixedFormatInfoPatternParser<LocalTime> LocalTimePatternParser { get { return localTimePatternParser; } }
        internal FixedFormatInfoPatternParser<LocalDate> LocalDatePatternParser { get { return localDatePatternParser; } }
        internal FixedFormatInfoPatternParser<LocalDateTime> LocalDateTimePatternParser { get { return localDateTimePatternParser; } }

        // TODO(Post-V1): Make these writable?
        /// <summary>
        /// Returns a read-only list of the names of the months for the default calendar for this culture.
        /// See the usage guide for caveats around the use of these names for other calendars.
        /// Element 0 of the list is null, to allow a more natural mapping from (say) 1 to the string "January".
        /// </summary>
        public IList<string> LongMonthNames { get { return longMonthNames; } }
        /// <summary>
        /// Returns a read-only list of the abbreviated names of the months for the default calendar for this culture.
        /// See the usage guide for caveats around the use of these names for other calendars.
        /// Element 0 of the list is null, to allow a more natural mapping from (say) 1 to the string "Jan".
        /// </summary>
        public IList<string> ShortMonthNames { get { return shortMonthNames; } }
        /// <summary>
        /// Returns a read-only list of the names of the months for the default calendar for this culture.
        /// See the usage guide for caveats around the use of these names for other calendars.
        /// Element 0 of the list is null, to allow a more natural mapping from (say) 1 to the string "January".
        /// The genitive form is used for month text where the day of month also appears in the pattern.
        /// If the culture does not use genitive month names, this property will return the same reference as
        /// <see cref="LongMonthNames"/>.
        /// </summary>
        public IList<string> LongMonthGenitiveNames { get { return longMonthGenitiveNames; } }
        /// <summary>
        /// Returns a read-only list of the abbreviated names of the months for the default calendar for this culture.
        /// See the usage guide for caveats around the use of these names for other calendars.
        /// Element 0 of the list is null, to allow a more natural mapping from (say) 1 to the string "Jan".
        /// The genitive form is used for month text where the day also appears in the pattern.
        /// If the culture does not use genitive month names, this property will return the same reference as
        /// <see cref="ShortMonthNames"/>.
        /// </summary>
        public IList<string> ShortMonthGenitiveNames { get { return shortMonthGenitiveNames; } }
        /// <summary>
        /// Returns a read-only list of the names of the days of the week for the default calendar for this culture.
        /// See the usage guide for caveats around the use of these names for other calendars.
        /// Element 0 of the list is null, and the other elements correspond with the index values returned from
        /// <see cref="LocalDateTime.DayOfWeek"/> and similar properties.
        /// </summary>
        public IList<string> LongDayNames { get { return longDayNames; } }
        /// <summary>
        /// Returns a read-only list of the abbreviated names of the days of the week for the default calendar for this culture.
        /// See the usage guide for caveats around the use of these names for other calendars.
        /// Element 0 of the list is null, and the other elements correspond with the index values returned from
        /// <see cref="LocalDateTime.DayOfWeek"/> and similar properties.
        /// </summary>
        public IList<string> ShortDayNames { get { return shortDayNames; } }

        /// <summary>
        /// Gets the number format associated with this formatting information.
        /// </summary>
        public NumberFormatInfo NumberFormat { get { return numberFormat; } }

        /// <summary>
        /// Gets the BCL date time format associated with this formatting information.
        /// </summary>
        public DateTimeFormatInfo DateTimeFormat { get { return dateTimeFormat; } }

        /// <summary>
        /// Gets the decimal separator from the number format associated with this provider.
        /// </summary>
        public string DecimalSeparator { get { return NumberFormat.NumberDecimalSeparator; } }

        /// <summary>
        /// Name of the culture providing this formatting information.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///   Gets the positive sign.
        /// </summary>
        public string PositiveSign { get { return NumberFormat.PositiveSign; } }

        /// <summary>
        /// Gets the negative sign.
        /// </summary>
        public string NegativeSign { get { return NumberFormat.NegativeSign; } }

#if PCL
        /// <summary>
        /// Gets the time separator.
        /// </summary>
        public string TimeSeparator { get { return timeSeparator; } }

        /// <summary>
        /// Gets the date separator.
        /// </summary>
        public string DateSeparator { get { return dateSeparator; } }
#else
        /// <summary>
        /// Gets the time separator.
        /// </summary>
        public string TimeSeparator { get { return DateTimeFormat.TimeSeparator; } }

        /// <summary>
        /// Gets the date separator.
        /// </summary>
        public string DateSeparator { get { return DateTimeFormat.DateSeparator; } }
#endif
        /// <summary>
        /// Gets the AM designator.
        /// </summary>
        public string AMDesignator { get { return DateTimeFormat.AMDesignator; } }

        /// <summary>
        /// Gets the PM designator.
        /// </summary>
        public string PMDesignator { get { return DateTimeFormat.PMDesignator; } }

        /// <summary>
        /// Returns the names for the given era in this culture.
        /// </summary>
        /// <param name="era">The era to find the names of.</param>
        /// <returns>A read-only list of names for the given era, or an empty list if
        /// the era is not known in this culture.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="era"/> is null.</exception>
        public IList<string> GetEraNames(Era era)
        {
            Preconditions.CheckNotNull(era, "era");
            lock (eraNamesCache)
            {
                IList<string> names;
                if (eraNamesCache.TryGetValue(era, out names))
                {
                    return names;
                }
                string pipeDelimited = PatternResources.ResourceManager.GetString(era.ResourceIdentifier, cultureInfo);
                if (pipeDelimited == null)
                {
                    names = new string[0];
                    eraPrimaryNameCache[era] = "";
                }
                else
                {
                    string[] values = pipeDelimited.Split('|');
                    eraPrimaryNameCache[era] = values[0];
                    // Order by length, descending to avoid early out (e.g. parsing BCE as BC and then having a spare E)
                    Array.Sort(values, (x, y) => y.Length.CompareTo(x.Length));
                    names = new ReadOnlyCollection<string>(values);
                }
                eraNamesCache[era] = names;
                return names;
            }
        }

        /// <summary>
        /// Returns the primary name for the given era in this culture.
        /// </summary>
        /// <param name="era">The era to find the primary name of.</param>
        /// <returns>The primary name for the given era, or an empty string if the era name is not known.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="era"/> is null.</exception>
        public string GetEraPrimaryName(Era era)
        {
            Preconditions.CheckNotNull(era, "era");

            // The era names (plural) cache is used as the lock for both this and the primary name.
            lock (eraNamesCache)
            {
                string name;
                if (eraPrimaryNameCache.TryGetValue(era, out name))
                {
                    return name;
                }
                // This will force the primary name cache to be populated
                GetEraNames(era);
                return eraPrimaryNameCache[era];
            }
        }

        /// <summary>
        /// Gets the <see cref="NodaFormatInfo" /> object for the current thread.
        /// </summary>
        public static NodaFormatInfo CurrentInfo
        {
            get { return GetInstance(Thread.CurrentThread.CurrentCulture); }
        }

        /// <summary>
        /// Gets the <see cref="Offset" /> "F" pattern.
        /// </summary>
        public string OffsetPatternFull { get { return offsetPatternFull; } }

        /// <summary>
        /// Gets the <see cref="Offset" /> "L" pattern.
        /// </summary>
        public string OffsetPatternLong { get { return offsetPatternLong; } }

        /// <summary>
        /// Gets the <see cref="Offset" /> "M" pattern.
        /// </summary>
        public string OffsetPatternMedium { get { return offsetPatternMedium; } }

        /// <summary>
        /// Gets the <see cref="Offset" /> "S" pattern.
        /// </summary>
        public string OffsetPatternShort { get { return offsetPatternShort; } }

        #region IFormatProvider Members
        /// <summary>
        ///   Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="formatType">An object that specifies the type of format object to return.</param>
        /// <returns>
        ///   An instance of the object specified by <paramref name = "formatType" />, if the <see cref="T:System.IFormatProvider" />
        ///   implementation can supply that type of object; otherwise, null.
        /// </returns>      
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(NodaFormatInfo))
            {
                return this;
            }
            if (formatType == typeof(NumberFormatInfo))
            {
                return NumberFormat;
            }
            if (formatType == typeof(DateTimeFormatInfo))
            {
                return DateTimeFormat;
            }
            return null;
        }
        #endregion

        /// <summary>
        /// Clears the cache. Only used for test purposes.
        /// </summary>
        internal static void ClearCache()
        {
            lock (Cache) Cache.Clear();
        }

        /// <summary>
        /// Gets the <see cref="NodaFormatInfo" /> for the given <see cref="CultureInfo" />.
        /// </summary>
        /// <remarks>
        /// This method maintains a cache of results for read-only cultures.
        /// </remarks>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>The <see cref="NodaFormatInfo" />. Will never be null.</returns>
        internal static NodaFormatInfo GetFormatInfo(CultureInfo cultureInfo)
        {
            Preconditions.CheckNotNull(cultureInfo, "cultureInfo");
            if (cultureInfo == CultureInfo.InvariantCulture)
            {
                return InvariantInfo;
            }
            // Never cache (or consult the cache) for non-read-only cultures.
            if (!cultureInfo.IsReadOnly)
            {
                return new NodaFormatInfo(cultureInfo);
            }
            NodaFormatInfo result;
            lock (Cache)
            {
                if (!Cache.TryGetValue(cultureInfo, out result))
                {
                    result = new NodaFormatInfo(cultureInfo);
                    Cache.Add(cultureInfo, result);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the <see cref="NodaFormatInfo" /> for the given <see cref="IFormatProvider" />. If the
        /// format provider is null or if it does not provide a <see cref="NodaFormatInfo" />
        /// object then the format object for the current thread is returned.
        /// </summary>
        /// <param name="provider">The <see cref="IFormatProvider" />.</param>
        /// <returns>The <see cref="NodaFormatInfo" />. Will never be null.</returns>
        public static NodaFormatInfo GetInstance(IFormatProvider provider)
        {
            if (provider != null)
            {
                var format = provider as NodaFormatInfo;
                if (format != null)
                {
                    return format;
                }
                format = provider.GetFormat(typeof(NodaFormatInfo)) as NodaFormatInfo;
                if (format != null)
                {
                    return format;
                }
                var cultureInfo = provider as CultureInfo;
                if (cultureInfo != null)
                {
                    return GetFormatInfo(cultureInfo);
                }
            }
            return GetInstance(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return description;
        }
    }
}
