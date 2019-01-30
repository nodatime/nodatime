// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Linq;
using NodaTime.Text;
using NUnit.Framework;
using System.Globalization;

namespace NodaTime.Test.Text
{
    public class OffsetPatternTest : PatternTestBase<Offset>
    {
        /// <summary>
        /// A non-breaking space.
        /// </summary>
        public const string Nbsp = "\u00a0";

        /// <summary>
        /// Test data that can only be used to test formatting.
        /// </summary>
        internal static readonly Data[] FormatOnlyData = {
            new Data(3, 0, 0) { Culture = Cultures.EnUs, Text = "", Pattern = "%-", },
            new Data(5, 0, 0) { Culture = Cultures.EnUs, Text = "+05", Pattern = "g" },
            new Data(5, 12, 0) { Culture = Cultures.EnUs, Text = "+05:12", Pattern = "g" },
            new Data(5, 12, 34) { Culture = Cultures.EnUs, Text = "+05:12:34", Pattern = "g" },

            // Losing information
            new Data(5, 6, 7) { Culture = Cultures.EnUs, Text = "05", Pattern = "HH" },
            new Data(5, 6, 7) { Culture = Cultures.EnUs, Text = "06", Pattern = "mm" },
            new Data(5, 6, 7) { Culture = Cultures.EnUs, Text = "07", Pattern = "ss" },
            new Data(5, 6, 7) { Culture = Cultures.EnUs, Text = "5", Pattern = "%H" },
            new Data(5, 6, 7) { Culture = Cultures.EnUs, Text = "6", Pattern = "%m" },
            new Data(5, 6, 7) { Culture = Cultures.EnUs, Text = "7", Pattern = "%s" },

            new Data(Offset.MaxValue) { Culture = Cultures.EnUs, Text = "+18", Pattern = "g" },
            new Data(Offset.MaxValue) { Culture = Cultures.EnUs, Text = "18", Pattern = "%H" },
            new Data(Offset.MaxValue) { Culture = Cultures.EnUs, Text = "0", Pattern = "%m" },
            new Data(Offset.MaxValue) { Culture = Cultures.EnUs, Text = "0", Pattern = "%s" },
            new Data(Offset.MaxValue) { Culture = Cultures.EnUs, Text = "m", Pattern = "\\m" },
            new Data(Offset.MaxValue) { Culture = Cultures.EnUs, Text = "m", Pattern = "'m'" },
            new Data(Offset.MaxValue) { Culture = Cultures.EnUs, Text = "mmmmmmmmmm", Pattern = "'mmmmmmmmmm'" },
            new Data(Offset.MaxValue) { Culture = Cultures.EnUs, Text = "z", Pattern = "'z'" },
            new Data(Offset.MaxValue) { Culture = Cultures.EnUs, Text = "zqw", Pattern = "'zqw'" },
            new Data(3, 0, 0, true) { Culture = Cultures.EnUs, Text = "-", Pattern = "%-" },
            new Data(3, 0, 0) { Culture = Cultures.EnUs, Text = "+", Pattern = "%+" },
            new Data(3, 0, 0, true) { Culture = Cultures.EnUs, Text = "-", Pattern = "%+" },
            new Data(5, 12, 34) { Culture = Cultures.EnUs, Text = "+05", Pattern = "s" },
            new Data(5, 12, 34) { Culture = Cultures.EnUs, Text = "+05:12", Pattern = "m" },
            new Data(5, 12, 34) { Culture = Cultures.EnUs, Text = "+05:12:34", Pattern = "l" },
        };

        /// <summary>
        /// Test data that can only be used to test successful parsing.
        /// </summary>
        internal static readonly Data[] ParseOnlyData = {
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "*", Pattern = "%*" },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "zqw", Pattern = "'zqw'" },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "-", Pattern = "%-" },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "+", Pattern = "%+" },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "-", Pattern = "%+" },
            new Data(5, 0, 0) { Culture = Cultures.EnUs, Text = "+05", Pattern = "s" },
            new Data(5, 12, 0) { Culture = Cultures.EnUs, Text = "+05:12", Pattern = "m" },
            new Data(5, 12, 34) { Culture = Cultures.EnUs, Text = "+05:12:34", Pattern = "l" },
            new Data(Offset.Zero) { Pattern = "Z+HH:mm", Text = "+00:00" } // Lenient when parsing Z-prefixed patterns.
        };

        /// <summary>
        /// Test data for invalid patterns
        /// </summary>
        internal static readonly Data[] InvalidPatternData = {
            new Data(Offset.Zero) { Pattern = "", Message = TextErrorMessages.FormatStringEmpty },
            new Data(Offset.Zero) { Pattern = "%Z", Message = TextErrorMessages.EmptyZPrefixedOffsetPattern },
            new Data(Offset.Zero) { Pattern = "HH:mmZ", Message = TextErrorMessages.ZPrefixNotAtStartOfPattern },
            new Data(Offset.Zero) { Pattern = "%%H", Message = TextErrorMessages.PercentDoubled},
            new Data(Offset.Zero) { Pattern = "HH:HH", Message = TextErrorMessages.RepeatedFieldInPattern, Parameters = {'H'} },
            new Data(Offset.Zero) { Pattern = "mm:mm", Message = TextErrorMessages.RepeatedFieldInPattern, Parameters = {'m'} },
            new Data(Offset.Zero) { Pattern = "ss:ss", Message = TextErrorMessages.RepeatedFieldInPattern, Parameters = {'s'} },
            new Data(Offset.Zero) { Pattern = "+HH:-mm", Message = TextErrorMessages.RepeatedFieldInPattern, Parameters = {'-'} },
            new Data(Offset.Zero) { Pattern = "-HH:+mm", Message = TextErrorMessages.RepeatedFieldInPattern, Parameters = {'+'} },
            new Data(Offset.Zero) { Pattern = "!", Message = TextErrorMessages.UnknownStandardFormat, Parameters = {'!', typeof(Offset).FullName}},
            new Data(Offset.Zero) { Pattern = "%", Message = TextErrorMessages.UnknownStandardFormat, Parameters = { '%', typeof(Offset).FullName } },
            new Data(Offset.Zero) { Pattern = "%%", Message = TextErrorMessages.PercentDoubled },
            new Data(Offset.Zero) { Pattern = "%\\", Message = TextErrorMessages.EscapeAtEndOfString },
            new Data(Offset.Zero) { Pattern = "\\", Message = TextErrorMessages.UnknownStandardFormat, Parameters = { '\\', typeof(Offset).FullName } },
            new Data(Offset.Zero) { Pattern = "H%", Message = TextErrorMessages.PercentAtEndOfString },
            new Data(Offset.Zero) { Pattern = "hh", Message = TextErrorMessages.Hour12PatternNotSupported, Parameters = { typeof(Offset).FullName } },
            new Data(Offset.Zero) { Pattern = "HHH", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 'H', 2 } },
            new Data(Offset.Zero) { Pattern = "mmm", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 'm', 2 } },
            new Data(Offset.Zero) { Pattern = "mmmmmmmmmmmmmmmmmmm", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 'm', 2 } },
            new Data(Offset.Zero) { Pattern = "'qwe", Message = TextErrorMessages.MissingEndQuote, Parameters = { '\'' } },
            new Data(Offset.Zero) { Pattern = "'qwe\\", Message = TextErrorMessages.EscapeAtEndOfString },
            new Data(Offset.Zero) { Pattern = "'qwe\\'", Message = TextErrorMessages.MissingEndQuote, Parameters = { '\'' } },
            new Data(Offset.Zero) { Pattern = "sss", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 's', 2 } },
        };

        /// <summary>
        /// Tests for parsing failures (of values)
        /// </summary>
        internal static readonly Data[] ParseFailureData = {
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "", Pattern = "g", Message = TextErrorMessages.ValueStringEmpty },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "1", Pattern = "HH", Message = TextErrorMessages.MismatchedNumber, Parameters = {"HH"} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "1", Pattern = "mm", Message = TextErrorMessages.MismatchedNumber, Parameters = {"mm"} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "1", Pattern = "ss", Message = TextErrorMessages.MismatchedNumber, Parameters = {"ss"} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "12:34 ", Pattern = "HH:mm", Message = TextErrorMessages.ExtraValueCharacters, Parameters = {" "} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "1a", Pattern = "H ", Message = TextErrorMessages.MismatchedCharacter, Parameters = {' '}},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "2:", Pattern = "%H", Message = TextErrorMessages.ExtraValueCharacters, Parameters = {":"}},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "a", Pattern = "%.", Message = TextErrorMessages.MismatchedCharacter, Parameters = {'.'}},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "a", Pattern = "%:", Message = TextErrorMessages.TimeSeparatorMismatch},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "a", Pattern = "%H", Message = TextErrorMessages.MismatchedNumber, Parameters = {"H"} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "a", Pattern = "%m", Message = TextErrorMessages.MismatchedNumber, Parameters = {"m"} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "a", Pattern = "%s", Message = TextErrorMessages.MismatchedNumber, Parameters = {"s"} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "a", Pattern = ".H", Message = TextErrorMessages.MismatchedCharacter, Parameters = {'.'}},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "a", Pattern = "\\'", Message = TextErrorMessages.EscapedCharacterMismatch, Parameters = {'\''} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "axc", Pattern = "'abc'", Message = TextErrorMessages.QuotedStringMismatch},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "z", Pattern = "%*", Message = TextErrorMessages.MismatchedCharacter, Parameters = {'*'} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "24", Pattern = "HH", Message = TextErrorMessages.FieldValueOutOfRange, Parameters = {24, 'H', typeof(Offset) }},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "60", Pattern = "mm", Message = TextErrorMessages.FieldValueOutOfRange, Parameters = {60, 'm', typeof(Offset) }},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "60", Pattern = "ss", Message = TextErrorMessages.FieldValueOutOfRange, Parameters = {60, 's', typeof(Offset) }},
            new Data(Offset.Zero) { Text = "+12", Pattern = "-HH", Message = TextErrorMessages.PositiveSignInvalid },
        };

        /// <summary>
        /// Common test data for both formatting and parsing. A test should be placed here unless is truly
        /// cannot be run both ways. This ensures that as many round-trip type tests are performed as possible.
        /// </summary>
        internal static readonly Data[] FormatAndParseData = {
/*XXX*/            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = ".", Pattern = "%." }, // decimal separator
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = ":", Pattern = "%:" }, // date separator
/*XXX*/            new Data(Offset.Zero) { Culture = Cultures.DotTimeSeparator, Text = ".", Pattern = "%." }, // decimal separator (always period)
            new Data(Offset.Zero) { Culture = Cultures.DotTimeSeparator, Text = ".", Pattern = "%:" }, // date separator
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "H", Pattern = "\\H" },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "HHss", Pattern = "'HHss'" },
            new Data(0, 0, 12) { Culture = Cultures.EnUs, Text = "12", Pattern = "%s" },
            new Data(0, 0, 12) { Culture = Cultures.EnUs, Text = "12", Pattern = "ss" },
            new Data(0, 0, 2) { Culture = Cultures.EnUs, Text = "2", Pattern = "%s" },
            new Data(0, 12, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "%m" },
            new Data(0, 12, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "mm" },
            new Data(0, 2, 0) { Culture = Cultures.EnUs, Text = "2", Pattern = "%m" },

            new Data(12, 0, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "%H" },
            new Data(12, 0, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "HH" },
            new Data(2, 0, 0) { Culture = Cultures.EnUs, Text = "2", Pattern = "%H" },
            new Data(2, 0, 0) { Culture = Cultures.EnUs, Text = "2", Pattern = "%H" },

            // Standard patterns with punctuation...
            new Data(5, 0, 0) { Culture = Cultures.EnUs, Text = "+05", Pattern = "G"  },
            new Data(5, 12, 0) { Culture = Cultures.EnUs, Text = "+05:12", Pattern = "G"  },
            new Data(5, 12, 34) { Culture = Cultures.EnUs, Text = "+05:12:34", Pattern = "G"  },
            new Data(5, 0, 0) { Culture = Cultures.EnUs, Text = "+05", Pattern = "g"  },
            new Data(5, 12, 0) { Culture = Cultures.EnUs, Text = "+05:12", Pattern = "g"  },
            new Data(5, 12, 34) { Culture = Cultures.EnUs, Text = "+05:12:34", Pattern = "g"  },
            new Data(Offset.MinValue) { Culture = Cultures.EnUs, Text = "-18", Pattern = "g" },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "Z", Pattern = "G"  },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "+00", Pattern = "g"  },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "+00", Pattern = "s"  },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "+00:00", Pattern = "m"  },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "+00:00:00", Pattern = "l"  },
            new Data(5, 0, 0) { Culture = Cultures.FrFr, Text = "+05", Pattern = "g" },
            new Data(5, 12, 0) { Culture = Cultures.FrFr, Text = "+05:12", Pattern = "g" },
            new Data(5, 12, 34) { Culture = Cultures.FrFr, Text = "+05:12:34", Pattern = "g" },
            new Data(Offset.MaxValue) { Culture = Cultures.FrFr, Text = "+18", Pattern = "g" },
            new Data(Offset.MinValue) { Culture = Cultures.FrFr, Text = "-18", Pattern = "g" },
            new Data(5, 0, 0) { Culture = Cultures.DotTimeSeparator, Text = "+05", Pattern = "g" },
            new Data(5, 12, 0) { Culture = Cultures.DotTimeSeparator, Text = "+05.12", Pattern = "g" },
            new Data(5, 12, 34) { Culture = Cultures.DotTimeSeparator, Text = "+05.12.34", Pattern = "g" },
            new Data(Offset.MaxValue) { Culture = Cultures.DotTimeSeparator, Text = "+18", Pattern = "g" },
            new Data(Offset.MinValue) { Culture = Cultures.DotTimeSeparator, Text = "-18", Pattern = "g" },

            // Standard patterns without punctuation
            new Data(5, 0, 0) { Culture = Cultures.EnUs, Text = "+05", Pattern = "I"  },
            new Data(5, 12, 0) { Culture = Cultures.EnUs, Text = "+0512", Pattern = "I"  },
            new Data(5, 12, 34) { Culture = Cultures.EnUs, Text = "+051234", Pattern = "I"  },
            new Data(5, 0, 0) { Culture = Cultures.EnUs, Text = "+05", Pattern = "i"  },
            new Data(5, 12, 0) { Culture = Cultures.EnUs, Text = "+0512", Pattern = "i"  },
            new Data(5, 12, 34) { Culture = Cultures.EnUs, Text = "+051234", Pattern = "i"  },
            new Data(Offset.MinValue) { Culture = Cultures.EnUs, Text = "-18", Pattern = "i" },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "Z", Pattern = "I"  },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "+00", Pattern = "i"  },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "+00", Pattern = "S"  },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "+0000", Pattern = "M"  },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "+000000", Pattern = "L"  },
            new Data(5, 0, 0) { Culture = Cultures.FrFr, Text = "+05", Pattern = "i" },
            new Data(5, 12, 0) { Culture = Cultures.FrFr, Text = "+0512", Pattern = "i" },
            new Data(5, 12, 34) { Culture = Cultures.FrFr, Text = "+051234", Pattern = "i" },
            new Data(Offset.MaxValue) { Culture = Cultures.FrFr, Text = "+18", Pattern = "i" },
            new Data(Offset.MinValue) { Culture = Cultures.FrFr, Text = "-18", Pattern = "i" },
            new Data(5, 0, 0) { Culture = Cultures.DotTimeSeparator, Text = "+05", Pattern = "i" },
            new Data(5, 12, 0) { Culture = Cultures.DotTimeSeparator, Text = "+0512", Pattern = "i" },
            new Data(5, 12, 34) { Culture = Cultures.DotTimeSeparator, Text = "+051234", Pattern = "i" },
            new Data(Offset.MaxValue) { Culture = Cultures.DotTimeSeparator, Text = "+18", Pattern = "i" },
            new Data(Offset.MinValue) { Culture = Cultures.DotTimeSeparator, Text = "-18", Pattern = "i" },

            // Explicit patterns
            new Data(0, 30, 0, true) { Culture = Cultures.EnUs, Text = "-00:30", Pattern = "+HH:mm" },
            new Data(0, 30, 0, true) { Culture = Cultures.EnUs, Text = "-00:30", Pattern = "-HH:mm" },
            new Data(0, 30, 0, false) { Culture = Cultures.EnUs, Text = "00:30", Pattern = "-HH:mm" },

            // Z-prefixes
            new Data(Offset.Zero) { Text = "Z", Pattern = "Z+HH:mm:ss" },
            new Data(5, 12, 34) { Text = "+05:12:34", Pattern = "Z+HH:mm:ss" },
            new Data(5, 12) { Text = "+05:12", Pattern = "Z+HH:mm" },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);

        [Test]
        [TestCaseSource(nameof(ParseData))]
        public void ParsePartial(PatternTestData<Offset> data)
        {
            data.TestParsePartial();
        }

        [Test]
        public void ParseNull() => AssertParseNull(OffsetPattern.GeneralInvariant);

        [Test]
        public void NumberFormatIgnored()
        {
            var culture = (CultureInfo) Cultures.EnUs.Clone();
            culture.NumberFormat.PositiveSign = "P";
            culture.NumberFormat.NegativeSign = "N";
            var pattern = OffsetPattern.Create("+HH:mm", culture);

            Assert.AreEqual("+05:00", pattern.Format(Offset.FromHours(5)));
            Assert.AreEqual("-05:00", pattern.Format(Offset.FromHours(-5)));
        }

        [Test]
        public void CreateWithCurrentCulture()
        {
            using (CultureSaver.SetCultures(Cultures.DotTimeSeparator))
            {
                var pattern = OffsetPattern.CreateWithCurrentCulture("H:mm");
                var text = pattern.Format(Offset.FromHoursAndMinutes(1, 30));
                Assert.AreEqual("1.30", text);
            }
        }

        /// <summary>
        /// A container for test data for formatting and parsing <see cref="Offset" /> objects.
        /// </summary>
        public sealed class Data : PatternTestData<Offset>
        {
            // Ignored anyway...
            protected override Offset DefaultTemplate => Offset.Zero;

            public Data(Offset value) : base(value)
            {
            }

            public Data(int hours, int minutes) : this(Offset.FromHoursAndMinutes(hours, minutes))
            {
            }

            public Data(int hours, int minutes, int seconds)
                : this(TestObjects.CreatePositiveOffset(hours, minutes, seconds))
            {
            }

            public Data(int hours, int minutes, int seconds, bool negative)
                : this(negative ? TestObjects.CreateNegativeOffset(hours, minutes, seconds) :
                                  TestObjects.CreatePositiveOffset(hours, minutes, seconds))
            {
            }

            internal override IPattern<Offset> CreatePattern() =>
                OffsetPattern.CreateWithInvariantCulture(Pattern!)
                    .WithCulture(Culture);

            internal override IPartialPattern<Offset> CreatePartialPattern() =>
                OffsetPattern.CreateWithInvariantCulture(Pattern!).WithCulture(Culture).UnderlyingPattern;
        }

    }
}
