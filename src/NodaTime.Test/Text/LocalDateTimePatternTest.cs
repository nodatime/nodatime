// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NodaTime.Text;
using NUnit.Framework;
using NodaTime.Test.Calendars;

namespace NodaTime.Test.Text
{
    public class LocalDateTimePatternTest : PatternTestBase<LocalDateTime>
    {
        private static readonly LocalDateTime SampleLocalDateTime = new LocalDateTime(1976, 6, 19, 21, 13, 34).PlusNanoseconds(123456789L);
        private static readonly LocalDateTime SampleLocalDateTimeToTicks = new LocalDateTime(1976, 6, 19, 21, 13, 34).PlusNanoseconds(123456700L);
        private static readonly LocalDateTime SampleLocalDateTimeToMillis = new LocalDateTime(1976, 6, 19, 21, 13, 34, 123);
        private static readonly LocalDateTime SampleLocalDateTimeToSeconds = new LocalDateTime(1976, 6, 19, 21, 13, 34);
        private static readonly LocalDateTime SampleLocalDateTimeToMinutes = new LocalDateTime(1976, 6, 19, 21, 13);
        internal static readonly LocalDateTime SampleLocalDateTimeCoptic = new LocalDateTime(1976, 6, 19, 21, 13, 34, CalendarSystem.Coptic).PlusNanoseconds(123456789L);
        
        private static readonly string[] AllStandardPatterns = { "f", "F", "g", "G", "o", "O", "s" };

        private static readonly object[] AllCulturesStandardPatterns = (from culture in Cultures.AllCultures
                                                                        from format in AllStandardPatterns
                                                                        select new TestCaseData(culture, format).SetName(culture + ": " + format)).ToArray();

        // The standard example date/time used in all the MSDN samples, which means we can just cut and paste
        // the expected results of the standard patterns.
        internal static readonly LocalDateTime MsdnStandardExample = new LocalDateTime(2009, 06, 15, 13, 45, 30, 90);
        internal static readonly LocalDateTime MsdnStandardExampleNoMillis = new LocalDateTime(2009, 06, 15, 13, 45, 30);
        private static readonly LocalDateTime MsdnStandardExampleNoSeconds = new LocalDateTime(2009, 06, 15, 13, 45);

        internal static readonly Data[] InvalidPatternData = {
            new Data { Pattern = "", Message = TextErrorMessages.FormatStringEmpty },
            new Data { Pattern = "a", Message = TextErrorMessages.UnknownStandardFormat, Parameters = { 'a', typeof(LocalDateTime) } },
            new Data { Pattern = "dd MM yyyy HH:MM:SS", Message = TextErrorMessages.RepeatedFieldInPattern, Parameters = { 'M' } },
            // Note incorrect use of "u" (year) instead of "y" (year of era)
            new Data { Pattern = "dd MM uuuu HH:mm:ss gg", Message = TextErrorMessages.EraWithoutYearOfEra },
            // Era specifier and calendar specifier in the same pattern.
            new Data { Pattern = "dd MM yyyy HH:mm:ss gg c", Message = TextErrorMessages.CalendarAndEra },
            // Embedded pattern start without ld or lt
            new Data { Pattern = "yyyy MM dd <", Message = TextErrorMessages.UnquotedLiteral, Parameters = { '<' } },
            // Attempt to use a full embedded date/time pattern (not valid for LocalDateTime)
            new Data { Pattern = "l<yyyy MM dd HH:mm>", Message = TextErrorMessages.InvalidEmbeddedPatternType },
            // Invalid nested pattern (local date pattern doesn't know about embedded patterns)
            new Data { Pattern = "ld<<D>>", Message = TextErrorMessages.UnquotedLiteral, Parameters = { '<' } },
        };

        internal static Data[] ParseFailureData = {
            new Data { Pattern = "dd MM yyyy HH:mm:ss", Text = "Complete mismatch", Message = TextErrorMessages.MismatchedNumber, Parameters = { "dd" }},
            new Data { Pattern = "(c)", Text = "(xxx)", Message = TextErrorMessages.NoMatchingCalendarSystem },
            // 24 as an hour is only valid when the time is midnight
            new Data { Pattern = "yyyy-MM-dd", Text = "2017-02-30", Message = TextErrorMessages.DayOfMonthOutOfRange, Parameters = { 30, 2, 2017 } },
            new Data { Pattern = "yyyy-MM-dd HH:mm:ss", Text = "2011-10-19 24:00:05", Message = TextErrorMessages.InvalidHour24 },
            new Data { Pattern = "yyyy-MM-dd HH:mm:ss", Text = "2011-10-19 24:01:00", Message = TextErrorMessages.InvalidHour24 },
            new Data { Pattern = "yyyy-MM-dd HH:mm", Text = "2011-10-19 24:01", Message = TextErrorMessages.InvalidHour24 },
            new Data { Pattern = "yyyy-MM-dd HH:mm", Text = "2011-10-19 24:00", Template = new LocalDateTime(1970, 1, 1, 0, 0, 5), Message = TextErrorMessages.InvalidHour24},
            new Data { Pattern = "yyyy-MM-dd HH", Text = "2011-10-19 24", Template = new LocalDateTime(1970, 1, 1, 0, 5, 0), Message = TextErrorMessages.InvalidHour24},
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
            // Note trunction of the "89" nanoseconds; o and O are BCL roundtrip patterns, with tick precision.
            new Data(SampleLocalDateTime) { Pattern = "o", Text = "1976-06-19T21:13:34.1234567" },
            new Data(SampleLocalDateTime) { Pattern = "O", Text = "1976-06-19T21:13:34.1234567" }
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
            new Data(MsdnStandardExample) { Pattern = "r", Text = "2009-06-15T13:45:30.090000000 (ISO)", Culture = Cultures.EnUs },
            new Data(SampleLocalDateTimeCoptic) { Pattern = "r", Text = "1976-06-19T21:13:34.123456789 (Coptic)", Culture = Cultures.EnUs },
            // Note: No RFC1123, as that requires a time zone.
            // Sortable / ISO8601
            new Data(MsdnStandardExampleNoMillis) { Pattern = "s", Text = "2009-06-15T13:45:30", Culture = Cultures.EnUs },

            // Standard patterns (French)
            new Data(MsdnStandardExampleNoSeconds) { Pattern = "f", Text = "lundi 15 juin 2009 13:45", Culture = Cultures.FrFr },
            new Data(MsdnStandardExampleNoMillis) { Pattern = "F", Text = "lundi 15 juin 2009 13:45:30", Culture = Cultures.FrFr },
            new Data(MsdnStandardExampleNoSeconds) { Pattern = "g", Text = "15/06/2009 13:45", Culture = Cultures.FrFr },
            new Data(MsdnStandardExampleNoMillis) { Pattern = "G", Text = "15/06/2009 13:45:30", Culture = Cultures.FrFr },
            // Culture has no impact on round-trip or sortable formats
            new Data(MsdnStandardExample) { StandardPattern = LocalDateTimePattern.BclRoundtrip, Pattern = "o", Text = "2009-06-15T13:45:30.0900000", Culture = Cultures.FrFr },
            new Data(MsdnStandardExample) { StandardPattern = LocalDateTimePattern.BclRoundtrip, Pattern = "O", Text = "2009-06-15T13:45:30.0900000", Culture = Cultures.FrFr },
            new Data(MsdnStandardExample) { StandardPattern = LocalDateTimePattern.FullRoundtripWithoutCalendar, Pattern = "R", Text = "2009-06-15T13:45:30.090000000", Culture = Cultures.FrFr },
            new Data(MsdnStandardExample) { StandardPattern = LocalDateTimePattern.FullRoundtrip, Pattern = "r", Text = "2009-06-15T13:45:30.090000000 (ISO)", Culture = Cultures.FrFr },
            new Data(MsdnStandardExampleNoMillis) { StandardPattern = LocalDateTimePattern.GeneralIso, Pattern = "s", Text = "2009-06-15T13:45:30", Culture = Cultures.FrFr },
            new Data(SampleLocalDateTime) { StandardPattern = LocalDateTimePattern.FullRoundtripWithoutCalendar, Pattern = "R", Text = "1976-06-19T21:13:34.123456789", Culture = Cultures.FrFr },
            new Data(SampleLocalDateTime) { StandardPattern = LocalDateTimePattern.FullRoundtrip, Pattern = "r", Text = "1976-06-19T21:13:34.123456789 (ISO)", Culture = Cultures.FrFr },

            // Calendar patterns are invariant
            new Data(MsdnStandardExample) { Pattern = "(c) uuuu-MM-dd'T'HH:mm:ss.FFFFFFFFF", Text = "(ISO) 2009-06-15T13:45:30.09", Culture = Cultures.FrFr },
            new Data(MsdnStandardExample) { Pattern = "uuuu-MM-dd(c)'T'HH:mm:ss.FFFFFFFFF", Text = "2009-06-15(ISO)T13:45:30.09", Culture = Cultures.EnUs },
            new Data(SampleLocalDateTimeCoptic) { Pattern = "(c) uuuu-MM-dd'T'HH:mm:ss.FFFFFFFFF", Text = "(Coptic) 1976-06-19T21:13:34.123456789", Culture = Cultures.FrFr },
            new Data(SampleLocalDateTimeCoptic) { Pattern = "uuuu-MM-dd'C'c'T'HH:mm:ss.FFFFFFFFF", Text = "1976-06-19CCopticT21:13:34.123456789", Culture = Cultures.EnUs },
            
            // Standard invariant patterns with a property but no pattern character
            new Data(MsdnStandardExample) { StandardPattern = LocalDateTimePattern.ExtendedIso, Pattern = "uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFF", Text = "2009-06-15T13:45:30.09", Culture = Cultures.FrFr },            

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

            // Check that unquoted T still works.
            new Data(2012, 1, 31, 17, 36, 45) { Text = "2012-01-31T17:36:45", Pattern = "yyyy-MM-ddTHH:mm:ss" },

            // Custom embedded patterns (or mixture of custom and standard)
            new Data(2015, 10, 24, 11, 55, 30, 0) { Pattern = "ld<yyyy*MM*dd>'X'lt<HH_mm_ss>", Text = "2015*10*24X11_55_30" },
            new Data(2015, 10, 24, 11, 55, 30, 0) { Pattern = "lt<HH_mm_ss>'Y'ld<yyyy*MM*dd>", Text = "11_55_30Y2015*10*24" },
            new Data(2015, 10, 24, 11, 55, 30, 0) { Pattern = "ld<d>'X'lt<HH_mm_ss>", Text = "10/24/2015X11_55_30" },
            new Data(2015, 10, 24, 11, 55, 30, 0) { Pattern = "ld<yyyy*MM*dd>'X'lt<T>", Text = "2015*10*24X11:55:30" },

            // Standard embedded patterns (main use case of embedded patterns). Short time versions have a seconds value of 0 so they can round-trip.
            new Data(2015, 10, 24, 11, 55, 30, 90) { Pattern = "ld<D> lt<r>", Text = "Saturday, 24 October 2015 11:55:30.09" },
            new Data(2015, 10, 24, 11, 55, 0) { Pattern = "ld<d> lt<t>", Text = "10/24/2015 11:55" },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);

        [Test]
        public void WithCalendar()
        {
            var pattern = LocalDateTimePattern.GeneralIso.WithCalendar(CalendarSystem.Coptic);
            var value = pattern.Parse("0284-08-29T12:34:56").Value;
            Assert.AreEqual(new LocalDateTime(284, 8, 29, 12, 34, 56, CalendarSystem.Coptic), value);
        }

        [Test]
        public void CreateWithCurrentCulture()
        {
            var dateTime = new LocalDateTime(2017, 8, 23, 12, 34, 56);
            using (CultureSaver.SetCultures(Cultures.FrFr))
            {
                var pattern = LocalDateTimePattern.CreateWithCurrentCulture("g");
                Assert.AreEqual("23/08/2017 12:34", pattern.Format(dateTime));
            }
            using (CultureSaver.SetCultures(Cultures.FrCa))
            {
                var pattern = LocalDateTimePattern.CreateWithCurrentCulture("g");
                Assert.AreEqual("2017-08-23 12:34", pattern.Format(dateTime));
            }
        }

        [Test]
        public void ParseNull() => AssertParseNull(LocalDateTimePattern.ExtendedIso);

        [Test]
        [TestCaseSource(nameof(AllCulturesStandardPatterns))]
        public void BclStandardPatternComparison(CultureInfo culture, string pattern)
        {
            AssertBclNodaEquality(culture, pattern);
        }

        [Test]
        [TestCaseSource(nameof(AllCulturesStandardPatterns))]
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
                                Is.EqualTo(SampleLocalDateTimeToTicks) |
                                Is.EqualTo(SampleLocalDateTimeToMillis) |
                                Is.EqualTo(SampleLocalDateTimeToSeconds) |
                                Is.EqualTo(SampleLocalDateTimeToMinutes));
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

            // The BCL never seems to use abbreviated month genitive names.
            // I think it's reasonable that we do. Hmm.
            // See https://github.com/nodatime/nodatime/issues/377
            if ((patternText == "G" || patternText == "g") &&
                (culture.DateTimeFormat.ShortDatePattern.Contains("MMM") && !culture.DateTimeFormat.ShortDatePattern.Contains("MMMM")) &&
                culture.DateTimeFormat.AbbreviatedMonthGenitiveNames[SampleLocalDateTime.Month - 1] != culture.DateTimeFormat.AbbreviatedMonthNames[SampleLocalDateTime.Month - 1])
            {
                return;
            }

            // Formatting a DateTime with an always-invariant pattern (round-trip, sortable) converts to the ISO
            // calendar in .NET (which is reasonable, as there's no associated calendar).
            // We should use the Gregorian calendar for those tests.
            bool alwaysInvariantPattern = "Oos".Contains(patternText);
            Calendar calendar = alwaysInvariantPattern ? CultureInfo.InvariantCulture.Calendar : culture.Calendar;

            var calendarSystem = BclCalendars.CalendarSystemForCalendar(calendar);
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
            protected override LocalDateTime DefaultTemplate => LocalDateTimePattern.DefaultTemplateValue;

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

            public Data(LocalDate date, LocalTime time) : this(date + time)
            {
            }

            public Data() : this(LocalDateTimePattern.DefaultTemplateValue)
            {
            }

            internal override IPattern<LocalDateTime> CreatePattern() =>
                LocalDateTimePattern.CreateWithInvariantCulture(Pattern)
                    .WithTemplateValue(Template)
                    .WithCulture(Culture);
        }
    }
}
