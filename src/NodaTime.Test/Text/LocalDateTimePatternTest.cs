// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NodaTime.Properties;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public class LocalDateTimePatternTest : PatternTestBase<LocalDateTime>
    {
        private static readonly LocalDateTime SampleLocalDateTime = new LocalDateTime(1976, 6, 19, 21, 13, 34, 123, 4567);
        private static readonly LocalDateTime SampleLocalDateTimeNoTicks = new LocalDateTime(1976, 6, 19, 21, 13, 34, 123);
        private static readonly LocalDateTime SampleLocalDateTimeNoMillis = new LocalDateTime(1976, 6, 19, 21, 13, 34);
        private static readonly LocalDateTime SampleLocalDateTimeNoSeconds = new LocalDateTime(1976, 6, 19, 21, 13);
        internal static readonly LocalDateTime SampleLocalDateTimeCoptic = new LocalDateTime(1976, 6, 19, 21, 13, 34, 123, 4567, CalendarSystem.GetCopticCalendar(1));
        
        private static readonly string[] AllStandardPatterns = { "f", "F", "g", "G", "o", "O", "s" };

#pragma warning disable 0414 // Used by tests via reflection - do not remove!
        private static readonly object[] AllCulturesStandardPatterns = (from culture in Cultures.AllCultures
                                                                        from format in AllStandardPatterns
                                                                        select new TestCaseData(culture, format).SetName(culture + ": " + format)).ToArray();
#pragma warning restore 0414

        // The standard example date/time used in all the MSDN samples, which means we can just cut and paste
        // the expected results of the standard patterns.
        internal static readonly LocalDateTime MsdnStandardExample = new LocalDateTime(2009, 06, 15, 13, 45, 30, 90);
        internal static readonly LocalDateTime MsdnStandardExampleNoMillis = new LocalDateTime(2009, 06, 15, 13, 45, 30);
        private static readonly LocalDateTime MsdnStandardExampleNoSeconds = new LocalDateTime(2009, 06, 15, 13, 45);

        internal static readonly Data[] InvalidPatternData = {
            new Data { Pattern = "dd MM yyyy HH:MM:SS", Message = Messages.Parse_RepeatedFieldInPattern, Parameters = { 'M' } },
            // Note incorrect use of "y" (year) instead of "Y" (year of era)
            new Data { Pattern = "dd MM yyyy HH:mm:ss gg", Message = Messages.Parse_EraWithoutYearOfEra },
            // Era specifier and calendar specifier in the same pattern.
            new Data { Pattern = "dd MM YYYY HH:mm:ss gg c", Message = Messages.Parse_CalendarAndEra },
        };

        internal static Data[] ParseFailureData = {
            new Data { Pattern = "dd MM yyyy HH:mm:ss", Text = "Complete mismatch", Message = Messages.Parse_MismatchedNumber, Parameters = { "dd" }},
            new Data { Pattern = "(c)", Text = "(xxx)", Message = Messages.Parse_NoMatchingCalendarSystem },
            // 24 as an hour is only valid when the time is midnight
            new Data { Pattern = "yyyy-MM-dd HH:mm:ss", Text = "2011-10-19 24:00:05", Message = Messages.Parse_InvalidHour24 },
            new Data { Pattern = "yyyy-MM-dd HH:mm:ss", Text = "2011-10-19 24:01:00", Message = Messages.Parse_InvalidHour24 },
            new Data { Pattern = "yyyy-MM-dd HH:mm", Text = "2011-10-19 24:01", Message = Messages.Parse_InvalidHour24 },
            new Data { Pattern = "yyyy-MM-dd HH:mm", Text = "2011-10-19 24:00", Template = new LocalDateTime(1970, 1, 1, 0, 0, 5), Message = Messages.Parse_InvalidHour24},
            new Data { Pattern = "yyyy-MM-dd HH", Text = "2011-10-19 24", Template = new LocalDateTime(1970, 1, 1, 0, 5, 0), Message = Messages.Parse_InvalidHour24},
        };

        internal static Data[] ParseOnlyData = {
            new Data(2011, 10, 19, 16, 05, 20) { Pattern = "dd MM yyyy", Text = "19 10 2011", Template = new LocalDateTime(2000, 1, 1, 16, 05, 20) },
            new Data(2011, 10, 19, 16, 05, 20) { Pattern = "HH:mm:ss", Text = "16:05:20", Template = new LocalDateTime(2011, 10, 19, 0, 0, 0) },
            // Parsing using the semi-colon "comma dot" specifier
            new Data(2011, 10, 19, 16, 05, 20, 352) { Pattern = "yyyy-MM-dd HH:mm:ss;fff", Text = "2011-10-19 16:05:20,352" },
            new Data(2011, 10, 19, 16, 05, 20, 352) { Pattern = "yyyy-MM-dd HH:mm:ss;FFF", Text = "2011-10-19 16:05:20,352" },

            // 24:00 meaning "start of next day"
            new Data(2011, 10, 20) { Pattern = "yyyy-MM-dd HH:mm:ss", Text = "2011-10-19 24:00:00" },
            new Data(2011, 10, 20) { Pattern = "yyyy-MM-dd HH:mm:ss", Text = "2011-10-19 24:00:00", Template = new LocalDateTime(1970, 1, 1, 0, 5, 0) },
            new Data(2011, 10, 20) { Pattern = "yyyy-MM-dd HH:mm", Text = "2011-10-19 24:00" },
            new Data(2011, 10, 20) { Pattern = "yyyy-MM-dd HH", Text = "2011-10-19 24" },
        };

        internal static Data[] FormatOnlyData = {
            new Data(2011, 10, 19, 16, 05, 20) { Pattern = "ddd yyyy", Text = "Wed 2011" },
        };

        internal static Data[] FormatAndParseData = {
            // Standard patterns (US)
            // Full date/time (short time)
            new Data(MsdnStandardExampleNoSeconds) { Pattern = "f", Text = "Monday, June 15, 2009 1:45 PM", Culture = Cultures.EnUs },
            // Full date/time (long time)
            new Data(MsdnStandardExampleNoMillis) { Pattern = "F", Text = "Monday, June 15, 2009 1:45:30 PM", Culture = Cultures.EnUs },
            // General date/time (short time)
            new Data(MsdnStandardExampleNoSeconds) { Pattern = "g", Text = "6/15/2009 1:45 PM", Culture = Cultures.EnUs },
            // General date/time (longtime)
            new Data(MsdnStandardExampleNoMillis) { Pattern = "G", Text = "6/15/2009 1:45:30 PM", Culture = Cultures.EnUs },
            // Round-trip (o and O - same effect)
            new Data(MsdnStandardExample) { Pattern = "o", Text = "2009-06-15T13:45:30.0900000", Culture = Cultures.EnUs },
            new Data(MsdnStandardExample) { Pattern = "O", Text = "2009-06-15T13:45:30.0900000", Culture = Cultures.EnUs },
            new Data(MsdnStandardExample) { Pattern = "r", Text = "2009-06-15T13:45:30.0900000 (ISO)", Culture = Cultures.EnUs },
            new Data(SampleLocalDateTimeCoptic) { Pattern = "r", Text = "1976-06-19T21:13:34.1234567 (Coptic 1)", Culture = Cultures.EnUs },
            // Note: No RFC1123, as that requires a time zone.
            // Sortable / ISO8601
            new Data(MsdnStandardExampleNoMillis) { Pattern = "s", Text = "2009-06-15T13:45:30", Culture = Cultures.EnUs },

            // Standard patterns (French)
            new Data(MsdnStandardExampleNoSeconds) { Pattern = "f", Text = "lundi 15 juin 2009 13:45", Culture = Cultures.FrFr },
            new Data(MsdnStandardExampleNoMillis) { Pattern = "F", Text = "lundi 15 juin 2009 13:45:30", Culture = Cultures.FrFr },
            new Data(MsdnStandardExampleNoSeconds) { Pattern = "g", Text = "15/06/2009 13:45", Culture = Cultures.FrFr },
            new Data(MsdnStandardExampleNoMillis) { Pattern = "G", Text = "15/06/2009 13:45:30", Culture = Cultures.FrFr },
            // Culture has no impact on round-trip or sortable formats
            new Data(MsdnStandardExample) { Pattern = "o", Text = "2009-06-15T13:45:30.0900000", Culture = Cultures.FrFr },
            new Data(MsdnStandardExample) { Pattern = "O", Text = "2009-06-15T13:45:30.0900000", Culture = Cultures.FrFr },
            new Data(MsdnStandardExample) { Pattern = "r", Text = "2009-06-15T13:45:30.0900000 (ISO)", Culture = Cultures.FrFr },
            new Data(MsdnStandardExampleNoMillis) { Pattern = "s", Text = "2009-06-15T13:45:30", Culture = Cultures.FrFr },

            // Calendar patterns are invariant
            new Data(MsdnStandardExample) { Pattern = "(c) yyyy-MM-ddTHH:mm:ss.FFFFFFF", Text = "(ISO) 2009-06-15T13:45:30.09", Culture = Cultures.FrFr },
            new Data(MsdnStandardExample) { Pattern = "yyyy-MM-dd(c)THH:mm:ss.FFFFFFF", Text = "2009-06-15(ISO)T13:45:30.09", Culture = Cultures.EnUs },
            new Data(SampleLocalDateTimeCoptic) { Pattern = "(c) yyyy-MM-ddTHH:mm:ss.FFFFFFF", Text = "(Coptic 1) 1976-06-19T21:13:34.1234567", Culture = Cultures.FrFr },
            new Data(SampleLocalDateTimeCoptic) { Pattern = "yyyy-MM-dd'C'c'T'HH:mm:ss.FFFFFFF", Text = "1976-06-19CCoptic 1T21:13:34.1234567", Culture = Cultures.EnUs },
            
            // Use of the semi-colon "comma dot" specifier
            new Data(2011, 10, 19, 16, 05, 20, 352) { Pattern = "yyyy-MM-dd HH:mm:ss;fff", Text = "2011-10-19 16:05:20.352" },
            new Data(2011, 10, 19, 16, 05, 20, 352) { Pattern = "yyyy-MM-dd HH:mm:ss;FFF", Text = "2011-10-19 16:05:20.352" },
            new Data(2011, 10, 19, 16, 05, 20, 352) { Pattern = "yyyy-MM-dd HH:mm:ss;FFF 'end'", Text = "2011-10-19 16:05:20.352 end" },
            new Data(2011, 10, 19, 16, 05, 20) { Pattern = "yyyy-MM-dd HH:mm:ss;FFF 'end'", Text = "2011-10-19 16:05:20 end" },

            // When the AM designator is a leading substring of the PM designator...
            new Data(2011, 10, 19, 16, 05, 20) { Pattern = "yyyy-MM-dd h:mm:ss tt", Text = "2011-10-19 4:05:20 FooBar", Culture = Cultures.AwkwardAmPmDesignatorCulture },
            new Data(2011, 10, 19, 4, 05, 20) { Pattern = "yyyy-MM-dd h:mm:ss tt", Text = "2011-10-19 4:05:20 Foo", Culture = Cultures.AwkwardAmPmDesignatorCulture },

            // Current culture decimal separator is irrelevant when trimming the dot for truncated fractional settings
            new Data(2011, 10, 19, 4, 5, 6) { Pattern="yyyy-MM-dd HH:mm:ss.FFF", Text="2011-10-19 04:05:06", Culture = Cultures.FrFr },
            new Data(2011, 10, 19, 4, 5, 6, 123) { Pattern="yyyy-MM-dd HH:mm:ss.FFF", Text="2011-10-19 04:05:06.123", Culture = Cultures.FrFr },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);

        [Test]
        [TestCaseSource("AllCulturesStandardPatterns")]
        public void BclStandardPatternComparison(CultureInfo culture, string pattern)
        {
            AssertBclNodaEquality(culture, pattern);
        }

        [Test]
        [TestCaseSource("AllCulturesStandardPatterns")]
        public void ParseFormattedStandardPattern(CultureInfo culture, string patternText)
        {
            var pattern = CreatePatternOrNull(patternText, culture, new LocalDateTime(2000, 1, 1, 0, 0));
            if (pattern == null)
            {
                return;
            }

            // If the pattern really can't distinguish between AM and PM (e.g. it's 12 hour with an
            // abbreviated AM/PM designator) then let's let it go.
            if (pattern.Format(SampleLocalDateTime) == pattern.Format(SampleLocalDateTime.PlusHours(-12)))
            {
                return;
            }

            // If the culture doesn't have either AM or PM designators, we'll end up using the template value
            // AM/PM, so let's make sure that's right. (This happens on Mono for a few cultures.)
            if (culture.DateTimeFormat.AMDesignator == "" &&
                culture.DateTimeFormat.PMDesignator == "")
            {
                pattern = pattern.WithTemplateValue(new LocalDateTime(2000, 1, 1, 12, 0));
            }

            string formatted = pattern.Format(SampleLocalDateTime);
            var parseResult = pattern.Parse(formatted);
            Assert.IsTrue(parseResult.Success);
            var parsed = parseResult.Value;
            Assert.That(parsed, Is.EqualTo(SampleLocalDateTime) |
                                Is.EqualTo(SampleLocalDateTimeNoTicks) |
                                Is.EqualTo(SampleLocalDateTimeNoMillis) |
                                Is.EqualTo(SampleLocalDateTimeNoSeconds));
        }

        private void AssertBclNodaEquality(CultureInfo culture, string patternText)
        {
            // On Mono, some general patterns include an offset at the end. For the moment, ignore them.
            // TODO(V1.2): Work out what to do in such cases...
            if ((patternText == "f" && culture.DateTimeFormat.ShortTimePattern.EndsWith("z")) ||
                (patternText == "F" && culture.DateTimeFormat.FullDateTimePattern.EndsWith("z")) ||
                (patternText == "g" && culture.DateTimeFormat.ShortTimePattern.EndsWith("z")) ||
                (patternText == "G" && culture.DateTimeFormat.LongTimePattern.EndsWith("z")))
            {
                return;
            }

            var pattern = CreatePatternOrNull(patternText, culture, LocalDateTimePattern.DefaultTemplateValue);
            if (pattern == null)
            {
                return;
            }

            // Formatting a DateTime with an always-invariant pattern (round-trip, sortable) converts to the ISO
            // calendar in .NET (which is reasonable, as there's no associated calendar).
            // We should use the Gregorian calendar for those tests.
            // However, on Mono (at least some versions) the round-trip format (o and O) is broken - it uses
            // the calendar of the culture instead of the ISO-8601 calendar. So for those cultures,
            // we'll skip round-trip format tests.
            // See https://bugzilla.xamarin.com/show_bug.cgi?id=11364
            bool alwaysInvariantPattern = "Oos".Contains(patternText);
            if (alwaysInvariantPattern && TestHelper.IsRunningOnMono && !(culture.Calendar is GregorianCalendar))
            {
                return;
            }
            Calendar calendar = alwaysInvariantPattern ? CultureInfo.InvariantCulture.Calendar : culture.Calendar;

            var calendarSystem = CalendarSystemForCalendar(calendar);
            if (calendarSystem == null)
            {
                // We can't map this calendar system correctly yet; the test would be invalid.
                return;
            }

            // Use the sample date/time, but in the target culture's calendar system, as near as we can get.
            // We need to specify the right calendar system so that the days of week align properly.
            var inputValue = SampleLocalDateTime.WithCalendar(calendarSystem);
            Assert.AreEqual(inputValue.ToDateTimeUnspecified().ToString(patternText, culture),
                pattern.Format(inputValue));
        }

        // Helper method to make it slightly easier for tests to skip "bad" cultures.
        private LocalDateTimePattern CreatePatternOrNull(string patternText, CultureInfo culture, LocalDateTime templateValue)
        {
            try
            {
                return LocalDateTimePattern.Create(patternText, culture);
            }
            catch (InvalidPatternException)
            {
                // The Malta long date/time pattern in Mono 3.0 is invalid (not just wrong; invalid due to the wrong number of quotes).
                // Skip it :(
                // See https://bugzilla.xamarin.com/show_bug.cgi?id=11363
                return null;
            }
        }

        public sealed class Data : PatternTestData<LocalDateTime>
        {
            // Default to the start of the year 2000.
            protected override LocalDateTime DefaultTemplate
            {
                get { return LocalDateTimePattern.DefaultTemplateValue; }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public Data(LocalDateTime value)
                : base(value)
            {
            }

            public Data(int year, int month, int day)
                : this(new LocalDateTime(year, month, day, 0, 0))
            {
            }

            public Data(int year, int month, int day, int hour, int minute, int second)
                : this(new LocalDateTime(year, month, day, hour, minute, second))
            {
            }

            public Data(int year, int month, int day, int hour, int minute, int second, int millis)
                : this(new LocalDateTime(year, month, day, hour, minute, second, millis))
            {
            }

            public Data(LocalDate date, LocalTime time)
                : this(date + time)
            {
            }

            public Data()
                : this(LocalDateTimePattern.DefaultTemplateValue)
            {
            }

            internal override IPattern<LocalDateTime> CreatePattern()
            {
                return LocalDateTimePattern.CreateWithInvariantCulture(Pattern)
                    .WithTemplateValue(Template)
                    .WithCulture(Culture);
            }
        }
    }
}
