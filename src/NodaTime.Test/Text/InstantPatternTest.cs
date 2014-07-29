// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime.Properties;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public class InstantPatternTest : PatternTestBase<Instant>
    {
        internal static readonly Data[] InvalidPatternData = {
            new Data { Pattern = "!", Message = Messages.Parse_UnknownStandardFormat, Parameters = {'!', typeof(Instant).FullName}},
            new Data { Pattern = "%", Message = Messages.Parse_UnknownStandardFormat, Parameters = { '%', typeof(Instant).FullName } },
            new Data { Pattern = "\\", Message = Messages.Parse_UnknownStandardFormat, Parameters = { '\\', typeof(Instant).FullName } },
            // Just a few - these are taken from other tests
            new Data { Pattern = "%%", Message = Messages.Parse_PercentDoubled },
            new Data { Pattern = "%\\", Message = Messages.Parse_EscapeAtEndOfString },
            new Data { Pattern = "ffffffffff", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'f', 9 } },
            new Data { Pattern = "FFFFFFFFFF", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'F', 9 } },
        };

        internal static Data[] ParseFailureData = {
            new Data { Text = "rubbish", Pattern = "yyyyMMdd'T'HH:mm:ss", Message = Messages.Parse_MismatchedNumber, Parameters = { "yyyy" } },
            new Data { Text = "17 6", Pattern = "HH h", Message = Messages.Parse_InconsistentValues2, Parameters = {'H', 'h', typeof(LocalTime).FullName}},
            new Data { Text = "17 AM", Pattern = "HH tt", Message = Messages.Parse_InconsistentValues2, Parameters = {'H', 't', typeof(LocalTime).FullName}},
        };

        internal static Data[] ParseOnlyData = {
        };

        internal static Data[] FormatOnlyData = {
        };

        [Test]
        public void IsoHandlesCommas()
        {
            Instant expected = Instant.FromUtc(2012, 1, 1, 0, 0) + Duration.Epsilon;
            Instant actual = InstantPattern.ExtendedIsoPattern.Parse("2012-01-01T00:00:00,000000001Z").Value;
            Assert.AreEqual(actual, expected);
        }

        [Test]
        public void NullLabels()
        {
            Assert.Throws<ArgumentNullException>(() => InstantPattern.GeneralPattern.WithMinMaxLabels(null, "x"));
            Assert.Throws<ArgumentNullException>(() => InstantPattern.GeneralPattern.WithMinMaxLabels("x", null));
        }

        [TestCase("", "x")]
        [TestCase("x", "")]
        [TestCase("same", "same")]
        public void BadLabels(string min, string max)
        {
            Assert.Throws<ArgumentException>(() => InstantPattern.GeneralPattern.WithMinMaxLabels(min, max));
        }

        [Test]
        public void Format_CustomLabels()
        {
            var pattern = InstantPattern.GeneralPattern.WithMinMaxLabels("min", "max");
            Assert.AreEqual("min", pattern.Format(Instant.MinValue));
            Assert.AreEqual("max", pattern.Format(Instant.MaxValue));
            Assert.AreEqual(InstantPattern.GeneralPattern.Format(NodaConstants.UnixEpoch),
                pattern.Format(NodaConstants.UnixEpoch));
        }

        [Test]
        public void Parse_CustomLabels()
        {
            var pattern = InstantPattern.GeneralPattern.WithMinMaxLabels("min", "max");
            Assert.AreEqual(Instant.MinValue, pattern.Parse("min").Value);
            Assert.AreEqual(Instant.MaxValue, pattern.Parse("max").Value);
            Assert.AreEqual(NodaConstants.UnixEpoch,
                pattern.Parse(InstantPattern.GeneralPattern.Format(NodaConstants.UnixEpoch)).Value);
        }

        [Test]
        public void OutOfRange_Low()
        {
            var ticks = CalendarSystem.Iso.MinTicks - 1;
            var formatted = InstantPattern.ExtendedIsoPattern.Format(Instant.FromTicksSinceUnixEpoch(ticks));
            StringAssert.StartsWith(InstantPattern.OutOfRangeLabel, formatted);
        }

        [Test]
        public void OutOfRange_High()
        {
            var ticks = CalendarSystem.Iso.MaxTicks + 1;
            var formatted = InstantPattern.ExtendedIsoPattern.Format(Instant.FromTicksSinceUnixEpoch(ticks));
            StringAssert.StartsWith(InstantPattern.OutOfRangeLabel, formatted);
        }

        [Test]
        public void Extremities()
        {
            AssertRoundTrip(Instant.FromTicksSinceUnixEpoch(CalendarSystem.Iso.MinTicks), InstantPattern.ExtendedIsoPattern);
            AssertRoundTrip(Instant.FromTicksSinceUnixEpoch(CalendarSystem.Iso.MaxTicks), InstantPattern.ExtendedIsoPattern);
        }

        /// <summary>
        /// Common test data for both formatting and parsing. A test should be placed here unless is truly
        /// cannot be run both ways. This ensures that as many round-trip type tests are performed as possible.
        /// </summary>
        internal static readonly Data[] FormatAndParseData = {
            new Data(2012, 1, 31, 17, 36, 45) { Text = "2012-01-31T17:36:45", Pattern = "yyyy-MM-dd'T'HH:mm:ss" },
            // Check that unquoted T still works.
            new Data(2012, 1, 31, 17, 36, 45) { Text = "2012-01-31T17:36:45", Pattern = "yyyy-MM-ddTHH:mm:ss" },
            new Data(2012, 4, 28, 0, 0, 0) { Text = "2012 avr. 28", Pattern = "yyyy MMM dd", Culture = Cultures.FrFr },
            new Data { Text = " 1970 ", Pattern = " yyyy " }
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);

        /// <summary>
        /// A container for test data for formatting and parsing <see cref="LocalTime" /> objects.
        /// </summary>
        public sealed class Data : PatternTestData<Instant>
        {
            protected override Instant DefaultTemplate
            {
                get { return NodaConstants.UnixEpoch; }
            }

            public Data(Instant value)
                : base(value)
            {
            }

            public Data(int year, int month, int day, int hour, int minute, int second)
                : this(Instant.FromUtc(year, month, day, hour, minute, second))
            {
            }

            public Data()
                : this(NodaConstants.UnixEpoch)
            {
            }

            internal override IPattern<Instant> CreatePattern()
            {
                return InstantPattern.CreateWithInvariantCulture(Pattern)
                    .WithCulture(Culture);
            }
        }
    }
}
