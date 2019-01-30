// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Testing.TimeZones;
using NodaTime.Text;
using NodaTime.TimeZones;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NodaTime.Test.Text
{
    public class ZonedDateTimePatternTest : PatternTestBase<ZonedDateTime>
    {
        // Three zones with a deliberately leading-substring-matching set of names.
        // Transition is at 1am local time, going forward an hour.
        private static readonly SingleTransitionDateTimeZone TestZone1 = new SingleTransitionDateTimeZone(
            Instant.FromUtc(2010, 1, 1, 0, 0), Offset.FromHours(1), Offset.FromHours(2), "ab");
        // Transition is at 2am local time, going back an hour.
        private static readonly SingleTransitionDateTimeZone TestZone2 = new SingleTransitionDateTimeZone(
            Instant.FromUtc(2010, 1, 1, 0, 0), Offset.FromHours(2), Offset.FromHours(1), "abc");
        private static readonly SingleTransitionDateTimeZone TestZone3 = new SingleTransitionDateTimeZone(
            Instant.FromUtc(2010, 1, 1, 0, 0), Offset.FromHours(1), Offset.FromHours(2), "abcd");
        private static readonly IDateTimeZoneProvider TestProvider =
            new FakeDateTimeZoneSource.Builder { TestZone1, TestZone2, TestZone3 }.Build().ToProvider();
        private static readonly DateTimeZone FixedPlus1 = FixedDateTimeZone.ForOffset(Offset.FromHours(1));
        private static readonly DateTimeZone FixedWithMinutes = FixedDateTimeZone.ForOffset(Offset.FromHoursAndMinutes(1, 30));
        private static readonly DateTimeZone FixedWithSeconds = FixedDateTimeZone.ForOffset(Offset.FromSeconds(5));
        private static readonly DateTimeZone FixedMinus1 = FixedDateTimeZone.ForOffset(Offset.FromHours(-1));

        private static readonly DateTimeZone France = DateTimeZoneProviders.Tzdb["Europe/Paris"];
        private static readonly DateTimeZone Athens = DateTimeZoneProviders.Tzdb["Europe/Athens"];

        private static readonly ZonedDateTime SampleZonedDateTimeCoptic = LocalDateTimePatternTest.SampleLocalDateTimeCoptic.InUtc();

        // The standard example date/time used in all the MSDN samples, which means we can just cut and paste
        // the expected results of the standard patterns.
        private static readonly ZonedDateTime MsdnStandardExample = LocalDateTimePatternTest.MsdnStandardExample.InUtc();
        private static readonly ZonedDateTime MsdnStandardExampleNoMillis = LocalDateTimePatternTest.MsdnStandardExampleNoMillis.InUtc();

        internal static readonly Data[] InvalidPatternData = {
            new Data { Pattern = "", Message = TextErrorMessages.FormatStringEmpty },
            new Data { Pattern = "dd MM yyyy HH:MM:SS", Message = TextErrorMessages.RepeatedFieldInPattern, Parameters = { 'M' } },
            // Note incorrect use of "u" (year) instead of "y" (year of era)
            new Data { Pattern = "dd MM uuuu HH:mm:ss gg", Message = TextErrorMessages.EraWithoutYearOfEra },
            // Era specifier and calendar specifier in the same pattern.
            new Data { Pattern = "dd MM yyyy HH:mm:ss gg c", Message = TextErrorMessages.CalendarAndEra },
            new Data { Pattern = "g", Message = TextErrorMessages.UnknownStandardFormat, Parameters = { 'g', typeof(ZonedDateTime) } },
            // Invalid patterns involving embedded values
            new Data { Pattern = "ld<d> yyyy", Message = TextErrorMessages.DateFieldAndEmbeddedDate },
            new Data { Pattern = "l<yyyy-MM-dd HH:mm:ss> dd", Message = TextErrorMessages.DateFieldAndEmbeddedDate },
            new Data { Pattern = "ld<d> ld<f>", Message = TextErrorMessages.RepeatedFieldInPattern, Parameters = { 'l' } },
            new Data { Pattern = "lt<T> HH", Message = TextErrorMessages.TimeFieldAndEmbeddedTime },
            new Data { Pattern = "l<yyyy-MM-dd HH:mm:ss> HH", Message = TextErrorMessages.TimeFieldAndEmbeddedTime },
            new Data { Pattern = "lt<T> lt<t>", Message = TextErrorMessages.RepeatedFieldInPattern, Parameters = { 'l' } },
            new Data { Pattern = "ld<d> l<F>", Message = TextErrorMessages.RepeatedFieldInPattern, Parameters = { 'l' } },
            new Data { Pattern = "l<F> ld<d>", Message = TextErrorMessages.RepeatedFieldInPattern, Parameters = { 'l' } },
            new Data { Pattern = "lt<T> l<F>", Message = TextErrorMessages.RepeatedFieldInPattern, Parameters = { 'l' } },
            new Data { Pattern = "l<F> lt<T>", Message = TextErrorMessages.RepeatedFieldInPattern, Parameters = { 'l' } },
        };

        internal static Data[] ParseFailureData = {
            // Skipped value
            new Data { Pattern = "yyyy-MM-dd HH:mm z", Text = "2010-01-01 01:30 ab", Message = TextErrorMessages.SkippedLocalTime},
            // Ambiguous value
            new Data { Pattern = "yyyy-MM-dd HH:mm z", Text = "2010-01-01 01:30 abc", Message = TextErrorMessages.AmbiguousLocalTime },

            // Invalid offset within a skipped time
            new Data { Pattern = "yyyy-MM-dd HH:mm z o<g>", Text = "2010-01-01 01:30 ab +01", Message = TextErrorMessages.InvalidOffset},
            // Invalid offset within an ambiguous time (doesn't match either option)
            new Data { Pattern = "yyyy-MM-dd HH:mm z o<g>", Text = "2010-01-01 01:30 abc +05", Message = TextErrorMessages.InvalidOffset},
            // Invalid offset for an unambiguous time
            new Data { Pattern = "yyyy-MM-dd HH:mm z o<g>", Text = "2005-01-01 01:30 ab +02", Message = TextErrorMessages.InvalidOffset},

            // Failures copied from LocalDateTimePatternTest
            new Data { Pattern = "dd MM yyyy HH:mm:ss", Text = "Complete mismatch", Message = TextErrorMessages.MismatchedNumber, Parameters = { "dd" }},
            new Data { Pattern = "(c)", Text = "(xxx)", Message = TextErrorMessages.NoMatchingCalendarSystem },
            // 24 as an hour is only valid when the time is midnight
            new Data { Pattern = "yyyy-MM-dd HH:mm:ss", Text = "2011-10-19 24:00:05", Message = TextErrorMessages.InvalidHour24 },
            new Data { Pattern = "yyyy-MM-dd HH:mm:ss", Text = "2011-10-19 24:01:00", Message = TextErrorMessages.InvalidHour24 },
            new Data { Pattern = "yyyy-MM-dd HH:mm", Text = "2011-10-19 24:01", Message = TextErrorMessages.InvalidHour24 },
            new Data { Pattern = "yyyy-MM-dd HH:mm", Text = "2011-10-19 24:00", Template = new LocalDateTime(1970, 1, 1, 0, 0, 5).InZoneStrictly(TestZone1), Message = TextErrorMessages.InvalidHour24},
            new Data { Pattern = "yyyy-MM-dd HH", Text = "2011-10-19 24", Template = new LocalDateTime(1970, 1, 1, 0, 5, 0).InZoneStrictly(TestZone1), Message = TextErrorMessages.InvalidHour24},

            // Redundant specification of fixed zone but not enough digits - we'll parse UTC+01:00:00 and unexpectedly be left with 00
            new Data { Pattern = "yyyy-MM-dd HH:mm z", Text = "2013-01-13 15:44 UTC+01:00:00.00", Message = TextErrorMessages.ExtraValueCharacters, Parameters = { ".00" }},

            // Can't parse a pattern with a time zone abbreviation.
            new Data { Pattern = "yyyy-MM-dd HH:mm x", Text = "ignored", Message = TextErrorMessages.FormatOnlyPattern },

            // Can't parse using a pattern that has no provider
            new Data { ZoneProvider = null, Pattern = "yyyy-MM-dd z", Text = "ignored", Message = TextErrorMessages.FormatOnlyPattern },

            // Invalid ID
            new Data { Pattern = "yyyy-MM-dd z", Text = "2017-08-21 LemonCurdIceCream", Message = TextErrorMessages.NoMatchingZoneId }
        };

        internal static Data[] ParseOnlyData = {
            // Template value time zone is from a different provider, but it's not part of the pattern.
            new Data(2013, 1, 13, 16, 2, France) { Pattern = "yyyy-MM-dd HH:mm", Text = "2013-01-13 16:02", Template = NodaConstants.UnixEpoch.InZone(France) },

            // Skipped value, resolver returns start of second interval
            new Data(TestZone1.Transition.InZone(TestZone1)) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2010-01-01 01:30 ab", Resolver = Resolvers.CreateMappingResolver(Resolvers.ThrowWhenAmbiguous, Resolvers.ReturnStartOfIntervalAfter) },

            // Skipped value, resolver returns end of first interval
            new Data(TestZone1.Transition.Minus(Duration.Epsilon).InZone(TestZone1)) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2010-01-01 01:30 ab", Resolver = Resolvers.CreateMappingResolver(Resolvers.ThrowWhenAmbiguous, Resolvers.ReturnEndOfIntervalBefore) },

            // Parse-only tests from LocalDateTimeTest.
            new Data(2011, 10, 19, 16, 05, 20) { Pattern = "dd MM yyyy", Text = "19 10 2011", Template = new LocalDateTime(2000, 1, 1, 16, 05, 20).InUtc() },
            new Data(2011, 10, 19, 16, 05, 20) { Pattern = "HH:mm:ss", Text = "16:05:20", Template = new LocalDateTime(2011, 10, 19, 0, 0, 0).InUtc() },

            // Parsing using the semi-colon "comma dot" specifier
            new Data(2011, 10, 19, 16, 05, 20, 352) { Pattern = "yyyy-MM-dd HH:mm:ss;fff", Text = "2011-10-19 16:05:20,352" },
            new Data(2011, 10, 19, 16, 05, 20, 352) { Pattern = "yyyy-MM-dd HH:mm:ss;FFF", Text = "2011-10-19 16:05:20,352" },

            // 24:00 meaning "start of next day"
            new Data(2011, 10, 20) { Pattern = "yyyy-MM-dd HH:mm:ss", Text = "2011-10-19 24:00:00" },
            new Data(2011, 10, 20, 0, 0, TestZone1) { Pattern = "yyyy-MM-dd HH:mm:ss", Text = "2011-10-19 24:00:00", Template = new LocalDateTime(1970, 1, 1, 0, 5, 0).InZoneStrictly(TestZone1) },
            new Data(2011, 10, 20) { Pattern = "yyyy-MM-dd HH:mm", Text = "2011-10-19 24:00" },
            new Data(2011, 10, 20) { Pattern = "yyyy-MM-dd HH", Text = "2011-10-19 24" },

            // Redundant specification of offset
            new Data(2013, 01, 13, 15, 44, FixedPlus1) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2013-01-13 15:44 UTC+01:00" },
            new Data(2013, 01, 13, 15, 44, FixedPlus1) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2013-01-13 15:44 UTC+01:00:00" },
        };

        internal static Data[] FormatOnlyData = {
            new Data(2011, 10, 19, 16, 05, 20) { Pattern = "ddd yyyy", Text = "Wed 2011" },

            // Time zone isn't in the provider
            new Data(2013, 1, 13, 16, 2, France) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2013-01-13 16:02 Europe/Paris" },

            // Ambiguous value - would be invalid if parsed with a strict parser.
            new Data(TestZone2.Transition.Plus(Duration.FromMinutes(30)).InZone(TestZone2)) { Pattern = "yyyy-MM-dd HH:mm", Text = "2010-01-01 01:30" },

            // Winter
            new Data(2013, 1, 13, 16, 2, France) { Pattern = "yyyy-MM-dd HH:mm x", Text = "2013-01-13 16:02 CET" },
            // Summer
            new Data(2013, 6, 13, 16, 2, France) { Pattern = "yyyy-MM-dd HH:mm x", Text = "2013-06-13 16:02 CEST" },

            new Data(2013, 6, 13, 16, 2, France) { ZoneProvider = null, Pattern = "yyyy-MM-dd HH:mm z", Text = "2013-06-13 16:02 Europe/Paris" },

            // Standard patterns without a DateTimeZoneProvider
            new Data(MsdnStandardExampleNoMillis) { StandardPattern = ZonedDateTimePattern.GeneralFormatOnlyIso, Pattern = "G", Text = "2009-06-15T13:45:30 UTC (+00)", Culture = Cultures.FrFr, ZoneProvider = null},
            new Data(MsdnStandardExample) { StandardPattern = ZonedDateTimePattern.ExtendedFormatOnlyIso, Pattern = "F", Text = "2009-06-15T13:45:30.09 UTC (+00)", Culture = Cultures.FrFr, ZoneProvider = null },
            // Standard patterns without a resolver
            new Data(MsdnStandardExampleNoMillis) { StandardPattern = ZonedDateTimePattern.GeneralFormatOnlyIso, Pattern = "G", Text = "2009-06-15T13:45:30 UTC (+00)", Culture = Cultures.FrFr, Resolver = null},
            new Data(MsdnStandardExample) { StandardPattern = ZonedDateTimePattern.ExtendedFormatOnlyIso, Pattern = "F", Text = "2009-06-15T13:45:30.09 UTC (+00)", Culture = Cultures.FrFr, Resolver = null },
        };

        internal static Data[] FormatAndParseData = {

            // Zone ID at the end
            new Data(2013, 01, 13, 15, 44, TestZone1) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2013-01-13 15:44 ab" },
            new Data(2013, 01, 13, 15, 44, TestZone2) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2013-01-13 15:44 abc" },
            new Data(2013, 01, 13, 15, 44, TestZone3) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2013-01-13 15:44 abcd" },
            new Data(2013, 01, 13, 15, 44, FixedPlus1) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2013-01-13 15:44 UTC+01" },
            new Data(2013, 01, 13, 15, 44, FixedMinus1) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2013-01-13 15:44 UTC-01" },
            new Data(2013, 01, 13, 15, 44, DateTimeZone.Utc) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2013-01-13 15:44 UTC" },

            // Zone ID at the start
            new Data(2013, 01, 13, 15, 44, TestZone1) { Pattern = "z yyyy-MM-dd HH:mm", Text = "ab 2013-01-13 15:44" },
            new Data(2013, 01, 13, 15, 44, TestZone2) { Pattern = "z yyyy-MM-dd HH:mm", Text = "abc 2013-01-13 15:44" },
            new Data(2013, 01, 13, 15, 44, TestZone3) { Pattern = "z yyyy-MM-dd HH:mm", Text = "abcd 2013-01-13 15:44" },
            new Data(2013, 01, 13, 15, 44, FixedPlus1) { Pattern = "z yyyy-MM-dd HH:mm", Text = "UTC+01 2013-01-13 15:44" },
            new Data(2013, 01, 13, 15, 44, FixedMinus1) { Pattern = "z yyyy-MM-dd HH:mm", Text = "UTC-01 2013-01-13 15:44" },
            new Data(2013, 01, 13, 15, 44, DateTimeZone.Utc) { Pattern = "z yyyy-MM-dd HH:mm", Text = "UTC 2013-01-13 15:44" },

            // More precise fixed zones.
            new Data(2013, 01, 13, 15, 44, FixedWithMinutes) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2013-01-13 15:44 UTC+01:30" },
            new Data(2013, 01, 13, 15, 44, FixedWithSeconds) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2013-01-13 15:44 UTC+00:00:05" },

            // Valid offset for an unambiguous time
            new Data(new LocalDateTime(2005, 1, 1, 1, 30).InZoneStrictly(TestZone1)) { Pattern = "yyyy-MM-dd HH:mm z o<g>", Text = "2005-01-01 01:30 ab +01"},
            // Valid offset (in the middle of the pattern) for an unambiguous time
            new Data(new LocalDateTime(2005, 1, 1, 1, 30).InZoneStrictly(TestZone1)) { Pattern = "yyyy-MM-dd o<g> HH:mm z", Text = "2005-01-01 +01 01:30 ab"},
            
            // Ambiguous value, resolver returns later value.
            new Data(TestZone2.Transition.Plus(Duration.FromMinutes(30)).InZone(TestZone2)) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2010-01-01 01:30 abc", Resolver = Resolvers.CreateMappingResolver(Resolvers.ReturnLater, Resolvers.ThrowWhenSkipped) },

            // Ambiguous value, resolver returns earlier value.
            new Data(TestZone2.Transition.Plus(Duration.FromMinutes(-30)).InZone(TestZone2)) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2010-01-01 01:30 abc", Resolver = Resolvers.CreateMappingResolver(Resolvers.ReturnEarlier, Resolvers.ThrowWhenSkipped) },

            // Ambiguous local value, but with offset for later value (smaller offset).
            new Data(TestZone2.Transition.Plus(Duration.FromMinutes(30)).InZone(TestZone2)) { Pattern = "yyyy-MM-dd HH:mm z o<g>", Text = "2010-01-01 01:30 abc +01"},

            // Ambiguous local value, but with offset for earlier value (greater offset).
            new Data(TestZone2.Transition.Plus(Duration.FromMinutes(-30)).InZone(TestZone2)) { Pattern = "yyyy-MM-dd HH:mm z o<g>", Text = "2010-01-01 01:30 abc +02"},

            // Specify the provider
            new Data(2013, 1, 13, 16, 2, France) { Pattern = "yyyy-MM-dd HH:mm z", Text = "2013-01-13 16:02 Europe/Paris", ZoneProvider = DateTimeZoneProviders.Tzdb},

            // Tests without zones, copied from LocalDateTimePatternTest
            // Calendar patterns are invariant
            new Data(MsdnStandardExample) { Pattern = "(c) uuuu-MM-dd'T'HH:mm:ss.FFFFFFF", Text = "(ISO) 2009-06-15T13:45:30.09", Culture = Cultures.FrFr },
            new Data(MsdnStandardExample) { Pattern = "uuuu-MM-dd(c)'T'HH:mm:ss.FFFFFFF", Text = "2009-06-15(ISO)T13:45:30.09", Culture = Cultures.EnUs },
            new Data(SampleZonedDateTimeCoptic) { Pattern = "(c) uuuu-MM-dd'T'HH:mm:ss.FFFFFFFFF", Text = "(Coptic) 1976-06-19T21:13:34.123456789", Culture = Cultures.FrFr },
            new Data(SampleZonedDateTimeCoptic) { Pattern = "uuuu-MM-dd'C'c'T'HH:mm:ss.FFFFFFFFF", Text = "1976-06-19CCopticT21:13:34.123456789", Culture = Cultures.EnUs },
            
            // Use of the semi-colon "comma dot" specifier
            new Data(2011, 10, 19, 16, 05, 20, 352) { Pattern = "yyyy-MM-dd HH:mm:ss;fff", Text = "2011-10-19 16:05:20.352" },
            new Data(2011, 10, 19, 16, 05, 20, 352) { Pattern = "yyyy-MM-dd HH:mm:ss;FFF", Text = "2011-10-19 16:05:20.352" },
            new Data(2011, 10, 19, 16, 05, 20, 352) { Pattern = "yyyy-MM-dd HH:mm:ss;FFF 'end'", Text = "2011-10-19 16:05:20.352 end" },
            new Data(2011, 10, 19, 16, 05, 20) { Pattern = "yyyy-MM-dd HH:mm:ss;FFF 'end'", Text = "2011-10-19 16:05:20 end" },

            // Standard patterns with a time zone provider
            new Data(2013, 01, 13, 15, 44, 30, 0, TestZone1) { StandardPattern = ZonedDateTimePattern.GeneralFormatOnlyIso.WithZoneProvider(TestProvider) , Pattern = "G", Text = "2013-01-13T15:44:30 ab (+02)", Culture = Cultures.FrFr },
            new Data(2013, 01, 13, 15, 44, 30, 90, TestZone1) { StandardPattern = ZonedDateTimePattern.ExtendedFormatOnlyIso.WithZoneProvider(TestProvider), Pattern = "F", Text = "2013-01-13T15:44:30.09 ab (+02)", Culture = Cultures.FrFr },

            // Custom embedded patterns (or mixture of custom and standard)
            new Data(2015, 10, 24, 11, 55, 30, 0, Athens) { Pattern = "ld<yyyy*MM*dd>'X'lt<HH_mm_ss> z o<g>", Text = "2015*10*24X11_55_30 Europe/Athens +03", ZoneProvider = DateTimeZoneProviders.Tzdb },
            new Data(2015, 10, 24, 11, 55, 30, 0, Athens) { Pattern = "lt<HH_mm_ss>'Y'ld<yyyy*MM*dd> z o<g>", Text = "11_55_30Y2015*10*24 Europe/Athens +03", ZoneProvider = DateTimeZoneProviders.Tzdb },
            new Data(2015, 10, 24, 11, 55, 30, 0, Athens) { Pattern = "l<HH_mm_ss'Y'yyyy*MM*dd> z o<g>", Text = "11_55_30Y2015*10*24 Europe/Athens +03", ZoneProvider = DateTimeZoneProviders.Tzdb },
            new Data(2015, 10, 24, 11, 55, 30, 0, Athens) { Pattern = "ld<d>'X'lt<HH_mm_ss> z o<g>", Text = "10/24/2015X11_55_30 Europe/Athens +03", ZoneProvider = DateTimeZoneProviders.Tzdb },
            new Data(2015, 10, 24, 11, 55, 30, 0, Athens) { Pattern = "ld<yyyy*MM*dd>'X'lt<T> z o<g>", Text = "2015*10*24X11:55:30 Europe/Athens +03", ZoneProvider = DateTimeZoneProviders.Tzdb },

            // Standard embedded patterns. Short time versions have a seconds value of 0 so they can round-trip.
            new Data(2015, 10, 24, 11, 55, 30, 90, Athens) { Pattern = "ld<D> lt<r> z o<g>", Text = "Saturday, 24 October 2015 11:55:30.09 Europe/Athens +03", ZoneProvider = DateTimeZoneProviders.Tzdb },
            new Data(2015, 10, 24, 11, 55, 0, 0, Athens) { Pattern = "l<f> z o<g>", Text = "Saturday, 24 October 2015 11:55 Europe/Athens +03", ZoneProvider = DateTimeZoneProviders.Tzdb },
            new Data(2015, 10, 24, 11, 55, 30, 0, Athens) { Pattern = "l<F> z o<g>", Text = "Saturday, 24 October 2015 11:55:30 Europe/Athens +03", ZoneProvider = DateTimeZoneProviders.Tzdb },
            new Data(2015, 10, 24, 11, 55, 0, 0, Athens) { Pattern = "l<g> z o<g>", Text = "10/24/2015 11:55 Europe/Athens +03", ZoneProvider = DateTimeZoneProviders.Tzdb },
            new Data(2015, 10, 24, 11, 55, 30, 0, Athens) { Pattern = "l<G> z o<g>", Text = "10/24/2015 11:55:30 Europe/Athens +03", ZoneProvider = DateTimeZoneProviders.Tzdb },

            // Nested embedded patterns
            new Data(2015, 10, 24, 11, 55, 30, 90, Athens) { Pattern = "l<ld<D> lt<r>> z o<g>", Text = "Saturday, 24 October 2015 11:55:30.09 Europe/Athens +03", ZoneProvider = DateTimeZoneProviders.Tzdb },
            new Data(2015, 10, 24, 11, 55, 30, 0, Athens) { Pattern = "l<'X'lt<HH_mm_ss>'Y'ld<yyyy*MM*dd>'X'> z o<g>", Text = "X11_55_30Y2015*10*24X Europe/Athens +03", ZoneProvider = DateTimeZoneProviders.Tzdb },

            // Check that unquoted T still works.
            new Data(2012, 1, 31, 17, 36, 45) { Text = "2012-01-31T17:36:45", Pattern = "yyyy-MM-ddTHH:mm:ss" },

            // Issue981
            new Data(1906, 8, 29, 20, 58, 32, 0, DateTimeZoneProviders.Tzdb["Etc/GMT-12"]) { Text = "1906-08-29T20:58:32 Etc/GMT-12 (+12)", Pattern = "uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFF z '('o<g>')'", ZoneProvider = DateTimeZoneProviders.Tzdb },

            // Fields not otherwise covered (according to tests running on AppVeyor...)
            new Data(MsdnStandardExample) { Pattern = "d MMMM yyyy (g) h:mm:ss.FF tt", Text = "15 June 2009 (A.D.) 1:45:30.09 PM" },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);

        [Test]
        public void WithTemplateValue()
        {
            var pattern = ZonedDateTimePattern.CreateWithInvariantCulture("yyyy-MM-dd", TestProvider)
                .WithTemplateValue(Instant.FromUtc(1970, 1, 1, 11, 30).InZone(TestZone3));
            var parsed = pattern.Parse("2017-08-23").Value;
            Assert.AreSame(TestZone3, parsed.Zone);
            // TestZone3 is at UTC+1 in 1970, so the template value's *local* time is 12pm.
            // Even though we're parsing a date in 2017, it's the local time from the template value that's used.
            Assert.AreEqual(new LocalDateTime(2017, 8, 23, 12, 30, 0), parsed.LocalDateTime);
            Assert.AreEqual(Offset.FromHours(2), parsed.Offset);
        }

        [Test]
        public void WithCalendar()
        {
            var pattern = ZonedDateTimePattern.CreateWithInvariantCulture("yyyy-MM-dd", TestProvider).WithCalendar(CalendarSystem.Coptic);
            var parsed = pattern.Parse("0284-08-29").Value;
            Assert.AreEqual(new LocalDateTime(284, 8, 29, 0, 0, CalendarSystem.Coptic), parsed.LocalDateTime);
        }

        [Test]
        public void WithPatternText()
        {
            var pattern = ZonedDateTimePattern.CreateWithInvariantCulture("yyyy", TestProvider).WithPatternText("yyyy-MM-dd");
            var text = pattern.Format(NodaConstants.UnixEpoch.InUtc());
            Assert.AreEqual("1970-01-01", text);
        }

        [Test]
        public void CreateWithCurrentCulture()
        {
            using (CultureSaver.SetCultures(Cultures.DotTimeSeparator))
            {
                var pattern = ZonedDateTimePattern.CreateWithCurrentCulture("HH:mm", null);
                var text = pattern.Format(Instant.FromUtc(2000, 1, 1, 19, 30).InUtc());
                Assert.AreEqual("19.30", text);
            }
        }

        [Test]
        public void WithCulture()
        {
            var pattern = ZonedDateTimePattern.CreateWithInvariantCulture("HH:mm", null).WithCulture(Cultures.DotTimeSeparator);
            var text = pattern.Format(Instant.FromUtc(2000, 1, 1, 19, 30).InUtc());
            Assert.AreEqual("19.30", text);
        }

        // Test to hit each exit condition in the time zone ID parsing part of ZonedDateTimePatternParser
        [Test]
        public void FindLongestZoneId()
        {
            var source = new FakeDateTimeZoneSource.Builder
            {
                CreateZone("ABC"),
                CreateZone("ABCA"),
                CreateZone("ABCB"),
                CreateZone("ABCBX"),
                CreateZone("ABCD")
            }.Build();

            var provider = new DateTimeZoneCache(source);
            var pattern = ZonedDateTimePattern.Create("z 'x'", CultureInfo.InvariantCulture, Resolvers.StrictResolver,
                provider, NodaConstants.UnixEpoch.InUtc());

            foreach (var id in provider.Ids)
            {
                var value = pattern.Parse($"{id} x").Value;
                Assert.AreEqual(id, value.Zone.Id);
            }

            DateTimeZone CreateZone(string id) =>
                new SingleTransitionDateTimeZone(NodaConstants.UnixEpoch - Duration.FromDays(1),Offset.FromHours(-1), Offset.FromHours(0), id);
        }

        [Test]
        public void ParseNull() => AssertParseNull(ZonedDateTimePattern.ExtendedFormatOnlyIso.WithZoneProvider(TestProvider));

        public sealed class Data : PatternTestData<ZonedDateTime>
        {
            // Default to the start of the year 2000 UTC
            protected override ZonedDateTime DefaultTemplate => ZonedDateTimePattern.DefaultTemplateValue;

            internal ZoneLocalMappingResolver Resolver { get; set; }
            internal IDateTimeZoneProvider ZoneProvider { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public Data(ZonedDateTime value)
                : base(value)
            {
                Resolver = Resolvers.StrictResolver;
                ZoneProvider = TestProvider;
            }

            public Data(int year, int month, int day)
                : this(new LocalDateTime(year, month, day, 0, 0).InUtc())
            {
            }

            // Coincidentally, we don't specify time zones in tests other than the
            // ones which just go down to the date and hour/minute.
            public Data(int year, int month, int day, int hour, int minute, DateTimeZone zone)
                : this(new LocalDateTime(year, month, day, hour, minute).InZoneStrictly(zone))
            {
            }

            public Data(int year, int month, int day, int hour, int minute, int second)
                : this(new LocalDateTime(year, month, day, hour, minute, second).InUtc())
            {
            }

            public Data(int year, int month, int day, int hour, int minute, int second, int millis)
                : this(new LocalDateTime(year, month, day, hour, minute, second, millis).InUtc())
            {
            }

            public Data(int year, int month, int day, int hour, int minute, int second, int millis, DateTimeZone zone)
                : this(new LocalDateTime(year, month, day, hour, minute, second, millis).InZoneStrictly(zone))
            {
            }

            public Data() : this(ZonedDateTimePattern.DefaultTemplateValue)
            {
            }

            internal override IPattern<ZonedDateTime> CreatePattern() =>
                ZonedDateTimePattern.Create(Pattern!, Culture, Resolver, ZoneProvider, Template);
        }
    }
}
