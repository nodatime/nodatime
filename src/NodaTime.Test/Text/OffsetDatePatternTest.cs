// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Globalization;
using NodaTime.Text;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.Test.Text
{
    public class OffsetDatePatternTest : PatternTestBase<OffsetDate>
    {
        // The standard example date/time used in all the MSDN samples, which means we can just cut and paste
        // the expected results of the standard patterns. We've got an offset of 1 hour though.
        private static readonly OffsetDate MsdnStandardExample = LocalDateTimePatternTest.MsdnStandardExample.Date.WithOffset(Offset.FromHours(1));
        private static readonly OffsetDate MsdnStandardExampleNoMillis = LocalDateTimePatternTest.MsdnStandardExampleNoMillis.Date.WithOffset(Offset.FromHours(1));
        private static readonly OffsetDate SampleOffsetDateCoptic = LocalDateTimePatternTest.SampleLocalDateTimeCoptic.Date.WithOffset(Offset.Zero);

        private static readonly Offset AthensOffset = Offset.FromHours(3);

        internal static readonly Data[] InvalidPatternData = {
            new Data { Pattern = "", Message = TextErrorMessages.FormatStringEmpty },
            // Note incorrect use of "u" (year) instead of "y" (year of era)
            new Data { Pattern = "dd MM uuuu gg", Message = TextErrorMessages.EraWithoutYearOfEra },
            // Era specifier and calendar specifier in the same pattern.
            new Data { Pattern = "dd MM yyyy gg c", Message = TextErrorMessages.CalendarAndEra },
            new Data { Pattern = "g", Message = TextErrorMessages.UnknownStandardFormat, Parameters = { 'g', typeof(OffsetDate) } },
            // Invalid patterns involving embedded values
            new Data { Pattern = "l<d> yyyy", Message = TextErrorMessages.DateFieldAndEmbeddedDate },
            new Data { Pattern = "l<yyyy-MM-dd> dd", Message = TextErrorMessages.DateFieldAndEmbeddedDate },
            new Data { Pattern = "l<d> l<f>", Message = TextErrorMessages.RepeatedFieldInPattern, Parameters = { 'l' } },
            new Data { Pattern = @"l<\", Message = TextErrorMessages.EscapeAtEndOfString },
        };

        internal static Data[] ParseFailureData = {
            new Data { Pattern = "dd MM yyyy", Text = "Complete mismatch", Message = TextErrorMessages.MismatchedNumber, Parameters = { "dd" }},
            new Data { Pattern = "dd MM yyyy", Text = "29 02 2001", Message = TextErrorMessages.DayOfMonthOutOfRange, Parameters = { 29, 2, 2001 } },
            new Data { Pattern = "(c)", Text = "(xxx)", Message = TextErrorMessages.NoMatchingCalendarSystem },
        };

        internal static Data[] ParseOnlyData = {
        };

        internal static Data[] FormatOnlyData = {
            new Data(2011, 10, 19) { Pattern = "ddd yyyy", Text = "Wed 2011" },

            // Our template value has an offset of 0, but the value has an offset of 1.
            // The pattern doesn't include the offset, so that information is lost - no round-trip.
            new Data(MsdnStandardExample) { Pattern = "yyyy-MM-dd", Text = "2009-06-15" }
        };

        internal static Data[] FormatAndParseData = {
            // Copied from LocalDateTimePatternTest
            // Calendar patterns are invariant
            new Data(MsdnStandardExample) { Pattern = "(c) uuuu-MM-dd o<G>", Text = "(ISO) 2009-06-15 +01", Culture = Cultures.FrFr },
            new Data(MsdnStandardExample) { Pattern = "uuuu-MM-dd(c';'o<g>)", Text = "2009-06-15(ISO;+01)", Culture = Cultures.EnUs },
            new Data(SampleOffsetDateCoptic) { Pattern = "(c) uuuu-MM-dd o<G>", Text = "(Coptic) 1976-06-19 Z", Culture = Cultures.FrFr },
            new Data(SampleOffsetDateCoptic) { Pattern = "uuuu-MM-dd'C'c o<g>", Text = "1976-06-19CCoptic +00", Culture = Cultures.EnUs },

            // Standard patterns (all invariant)
            new Data(MsdnStandardExampleNoMillis) { StandardPattern = OffsetDatePattern.GeneralIso, Pattern = "G", Text = "2009-06-15+01", Culture = Cultures.FrFr },
            new Data(MsdnStandardExample) { StandardPattern = OffsetDatePattern.FullRoundtrip, Pattern = "r", Text = "2009-06-15+01 (ISO)", Culture = Cultures.FrFr },

            // Custom embedded patterns (or mixture of custom and standard)
            new Data(2015, 10, 24, AthensOffset) { Pattern = "l<yyyy*MM*dd>'X'o<g>", Text = "2015*10*24X+03" },
            new Data(2015, 10, 24, AthensOffset) { Pattern = "l<d>'X'o<g>", Text = "10/24/2015X+03" },

            // Standard embedded patterns.
            new Data(2015, 10, 24, AthensOffset) { Pattern = "l<D> o<g>", Text = "Saturday, 24 October 2015 +03" },
            new Data(2015, 10, 24, AthensOffset) { Pattern = "l<d> o<g>", Text = "10/24/2015 +03" },
            
            // Fields not otherwise covered
            new Data(MsdnStandardExample) { Pattern = "d MMMM yyyy (g) o<g>", Text = "15 June 2009 (A.D.) +01" },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);

        [Test]
        public void CreateWithInvariantCulture()
        {
            var pattern = OffsetDatePattern.CreateWithInvariantCulture("yyyy-MM-ddo<g>");
            Assert.AreSame(NodaFormatInfo.InvariantInfo, pattern.FormatInfo);
            var od = new LocalDate(2017, 8, 23).WithOffset(Offset.FromHours(2));
            Assert.AreEqual("2017-08-23+02", pattern.Format(od));
        }

        [Test]
        public void CreateWithCurrentCulture()
        {
            var od = new LocalDate(2017, 8, 23).WithOffset(Offset.FromHours(2));
            using (CultureSaver.SetCultures(Cultures.FrFr))
            {
                var pattern = OffsetDatePattern.CreateWithCurrentCulture("l<d> o<g>");
                Assert.AreEqual("23/08/2017 +02", pattern.Format(od));
            }
            using (CultureSaver.SetCultures(Cultures.FrCa))
            {
                var pattern = OffsetDatePattern.CreateWithCurrentCulture("l<d> o<g>");
                Assert.AreEqual("2017-08-23 +02", pattern.Format(od));
            }
        }

        [Test]
        public void WithCulture()
        {
            var pattern = OffsetDatePattern.CreateWithInvariantCulture("yyyy/MM/dd o<G>").WithCulture(Cultures.FrCa);
            var text = pattern.Format(new LocalDate(2000, 1, 1).WithOffset(Offset.FromHours(1)));
            Assert.AreEqual("2000-01-01 +01", text);
        }

        [Test]
        public void WithPatternText()
        {
            var pattern = OffsetDatePattern.CreateWithInvariantCulture("yyyy-MM-dd").WithPatternText("dd MM yyyy o<g>");
            var value = new LocalDate(1970, 1, 1).WithOffset(Offset.FromHours(2));
            var text = pattern.Format(value);
            Assert.AreEqual("01 01 1970 +02", text);
        }

        [Test]
        public void WithTemplateValue()
        {
            var pattern = OffsetDatePattern.CreateWithInvariantCulture("MM-dd")
                .WithTemplateValue(new LocalDate(1970, 1, 1).WithOffset(Offset.FromHours(2)));
            var parsed = pattern.Parse("08-23").Value;
            Assert.AreEqual(new LocalDate(1970, 8, 23), parsed.Date);
            Assert.AreEqual(Offset.FromHours(2), parsed.Offset);
        }

        [Test]
        public void WithCalendar()
        {
            var pattern = OffsetDatePattern.CreateWithInvariantCulture("yyyy-MM-dd")
                .WithCalendar(CalendarSystem.Coptic);
            var parsed = pattern.Parse("0284-08-29").Value;
            Assert.AreEqual(new LocalDate(284, 8, 29, CalendarSystem.Coptic), parsed.Date);
        }

        [Test]
        public void ParseNull() => AssertParseNull(OffsetDatePattern.GeneralIso);

        internal sealed class Data : PatternTestData<OffsetDate>
        {
            // Default to the start of the year 2000 UTC
            protected override OffsetDate DefaultTemplate => OffsetDatePattern.DefaultTemplateValue;

            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>
            /// <param name="value">The value.</param>
            internal Data(OffsetDate value) : base(value)
            {
            }

            internal Data(int year, int month, int day) : this(year, month, day, Offset.Zero)
            {
            }

            internal Data(int year, int month, int day, Offset offset)
                : this(new LocalDate(year, month, day).WithOffset(offset))
            {
            }

            internal Data() : this(OffsetDatePattern.DefaultTemplateValue)
            {
            }

            internal override IPattern<OffsetDate> CreatePattern() =>
                OffsetDatePattern.Create(Pattern!, Culture, Template);
        }
    }
}
