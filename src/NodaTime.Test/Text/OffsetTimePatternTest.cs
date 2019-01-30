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
    public class OffsetTimePatternTest : PatternTestBase<OffsetTime>
    {
        // The standard example date/time used in all the MSDN samples, which means we can just cut and paste
        // the expected results of the standard patterns. We've got an offset of 1 hour though.
        private static readonly OffsetTime MsdnStandardExample =
            LocalDateTimePatternTest.MsdnStandardExample.TimeOfDay.WithOffset(Offset.FromHours(1));
        private static readonly OffsetTime MsdnStandardExampleNoMillis =
            LocalDateTimePatternTest.MsdnStandardExampleNoMillis.TimeOfDay.WithOffset(Offset.FromHours(1));

        private static readonly Offset AthensOffset = Offset.FromHours(3);

        internal static readonly Data[] InvalidPatternData = {
            new Data { Pattern = "", Message = TextErrorMessages.FormatStringEmpty },
            // Invalid patterns involving embedded values
            new Data { Pattern = "l<t> l<T>", Message = TextErrorMessages.RepeatedFieldInPattern, Parameters = { 'l' } },
            new Data { Pattern = "l<T> HH", Message = TextErrorMessages.TimeFieldAndEmbeddedTime },
            new Data { Pattern = "l<HH:mm:ss> HH", Message = TextErrorMessages.TimeFieldAndEmbeddedTime },
            new Data { Pattern = @"l<\", Message = TextErrorMessages.EscapeAtEndOfString },
            new Data { Pattern = "x", Message = TextErrorMessages.UnknownStandardFormat, Parameters = { 'x', typeof(OffsetTime) } },
        };

        internal static Data[] ParseFailureData = {
            // Failures copied from LocalDateTimePatternTest
            new Data { Pattern = "HH:mm:ss", Text = "Complete mismatch", Message = TextErrorMessages.MismatchedNumber, Parameters = { "HH" }},

            new Data { Pattern = "HH:mm:ss o<+HH>", Text = "16:02 +15:00", Message = TextErrorMessages.TimeSeparatorMismatch },
            // It's not ideal that the type reported is LocalTime rather than OffsetTime, but probably not worth fixing.
            new Data { Pattern = "HH:mm:ss tt o<+HH>", Text = "16:02:00 AM +15:00", Message = TextErrorMessages.InconsistentValues2, Parameters = { 'H', 't', typeof(LocalTime) } },
        };

        internal static Data[] ParseOnlyData = {
            // Parsing using the semi-colon "comma dot" specifier
            new Data(16, 05, 20, 352) { Pattern = "HH:mm:ss;fff", Text = "16:05:20,352" },
            new Data(16, 05, 20, 352) { Pattern = "HH:mm:ss;FFF", Text = "16:05:20,352" },            
        };

        internal static Data[] FormatOnlyData = {
            // Our template value has an offset of 0, but the value has an offset of 1.
            // The pattern doesn't include the offset, so that information is lost - no round-trip.
            new Data(MsdnStandardExample) { Pattern = "HH:mm:ss.FF", Text = "13:45:30.09" },
            // The value includes milliseconds, which aren't formatted.
            new Data(MsdnStandardExample) { StandardPattern = OffsetTimePattern.GeneralIso, Pattern = "G", Text = "13:45:30+01", Culture = Cultures.FrFr },
        };

        internal static Data[] FormatAndParseData = {
            // Copied from LocalDateTimePatternTest

            // Standard patterns (all invariant)
            new Data(MsdnStandardExampleNoMillis) { StandardPattern = OffsetTimePattern.GeneralIso, Pattern = "G", Text = "13:45:30+01", Culture = Cultures.FrFr },
            new Data(MsdnStandardExample) { StandardPattern = OffsetTimePattern.ExtendedIso, Pattern = "o", Text = "13:45:30.09+01", Culture = Cultures.FrFr },

            // Property-only patterns            
            new Data(MsdnStandardExample) { StandardPattern = OffsetTimePattern.Rfc3339, Pattern = "HH':'mm':'ss;FFFFFFFFFo<Z+HH:mm>", Text = "13:45:30.09+01:00", Culture = Cultures.FrFr },

            // Embedded patterns
            new Data(11, 55, 30, AthensOffset) { Pattern = "l<HH_mm_ss> o<g>", Text = "11_55_30 +03" },
            new Data(11, 55, 30, AthensOffset) { Pattern = "l<T> o<g>", Text = "11:55:30 +03" },
            
            // Fields not otherwise covered
            new Data(MsdnStandardExample) { Pattern = "h:mm:ss.FF tt o<g>", Text = "1:45:30.09 PM +01" },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);

        [Test]
        public void CreateWithInvariantCulture()
        {
            var pattern = OffsetTimePattern.CreateWithInvariantCulture("HH:mm:sso<g>");
            Assert.AreSame(NodaFormatInfo.InvariantInfo, pattern.FormatInfo);
            var ot = new LocalTime(12, 34, 56).WithOffset(Offset.FromHours(2));
            Assert.AreEqual("12:34:56+02", pattern.Format(ot));
        }

        [Test]
        public void CreateWithCurrentCulture()
        {
            var ot = new LocalTime(12, 34, 56).WithOffset(Offset.FromHours(2));
            using (CultureSaver.SetCultures(Cultures.FrFr))
            {
                var pattern = OffsetTimePattern.CreateWithCurrentCulture("l<t> o<g>");
                Assert.AreEqual("12:34 +02", pattern.Format(ot));
            }
            using (CultureSaver.SetCultures(Cultures.DotTimeSeparator))
            {
                var pattern = OffsetTimePattern.CreateWithCurrentCulture("l<t> o<g>");
                Assert.AreEqual("12.34 +02", pattern.Format(ot));
            }
        }

        [Test]
        public void WithCulture()
        {
            var pattern = OffsetTimePattern.CreateWithInvariantCulture("HH:mm").WithCulture(Cultures.DotTimeSeparator);
            var text = pattern.Format(new LocalTime(19, 30).WithOffset(Offset.Zero));
            Assert.AreEqual("19.30", text);
        }

        [Test]
        public void WithPatternText()
        {
            var pattern = OffsetTimePattern.CreateWithInvariantCulture("HH:mm:ss").WithPatternText("HH:mm");
            var value = new LocalTime(13, 30).WithOffset(Offset.FromHours(2));
            var text = pattern.Format(value);
            Assert.AreEqual("13:30", text);
        }

        [Test]
        public void WithTemplateValue()
        {
            var pattern = OffsetTimePattern.CreateWithInvariantCulture("o<G>")
                .WithTemplateValue(new LocalTime(13, 30).WithOffset(Offset.Zero));
            var parsed = pattern.Parse("+02").Value;
            // Local time is taken from the template value; offset is from the text
            Assert.AreEqual(new LocalTime(13, 30), parsed.TimeOfDay);
            Assert.AreEqual(Offset.FromHours(2), parsed.Offset);
        }

        [Test]
        public void ParseNull() => AssertParseNull(OffsetTimePattern.ExtendedIso);

        internal sealed class Data : PatternTestData<OffsetTime>
        {
            // Default to the start of the year 2000 UTC
            protected override OffsetTime DefaultTemplate => OffsetTimePattern.DefaultTemplateValue;

            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>
            /// <param name="value">The value.</param>
            internal Data(OffsetTime value) : base(value)
            {
            }            

            internal Data(int hour, int minute, Offset offset) : this(hour, minute, 0, offset)
            {
            }

            internal Data(int hour, int minute, int second) : this(hour, minute, second, 0)
            {
            }

            internal Data(int hour, int minute, int second, Offset offset)
                : this(hour, minute, second, 0, offset)
            {
            }

            internal Data(int hour, int minute, int second, int millis)
                : this(hour, minute, second, millis, Offset.Zero)
            {
            }

            internal Data(int hour, int minute, int second, int millis, Offset offset)
                : this(new LocalTime(hour, minute, second, millis).WithOffset(offset))
            {
            }

            internal Data() : this(OffsetTimePattern.DefaultTemplateValue)
            {
            }

            internal override IPattern<OffsetTime> CreatePattern() =>
                OffsetTimePattern.Create(Pattern!, Culture, Template);
        }
    }
}
