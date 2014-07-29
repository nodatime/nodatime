// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Linq;
using NodaTime.Properties;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Text
{
    [TestFixture]
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
            new Data(Offset.Zero) { Pattern = "", Message = Messages.Parse_FormatStringEmpty },
            new Data(Offset.Zero) { Pattern = "%Z", Message = Messages.Parse_EmptyZPrefixedOffsetPattern },
            new Data(Offset.Zero) { Pattern = "HH:mmZ", Message = Messages.Parse_ZPrefixNotAtStartOfPattern },
            new Data(Offset.Zero) { Pattern = "%%H", Message = Messages.Parse_PercentDoubled},
            new Data(Offset.Zero) { Pattern = "HH:HH", Message = Messages.Parse_RepeatedFieldInPattern, Parameters = {'H'} },
            new Data(Offset.Zero) { Pattern = "mm:mm", Message = Messages.Parse_RepeatedFieldInPattern, Parameters = {'m'} },
            new Data(Offset.Zero) { Pattern = "ss:ss", Message = Messages.Parse_RepeatedFieldInPattern, Parameters = {'s'} },
            new Data(Offset.Zero) { Pattern = "+HH:-mm", Message = Messages.Parse_RepeatedFieldInPattern, Parameters = {'-'} },
            new Data(Offset.Zero) { Pattern = "-HH:+mm", Message = Messages.Parse_RepeatedFieldInPattern, Parameters = {'+'} },
            new Data(Offset.Zero) { Pattern = "!", Message = Messages.Parse_UnknownStandardFormat, Parameters = {'!', typeof(Offset).FullName}},
            new Data(Offset.Zero) { Pattern = "%", Message = Messages.Parse_UnknownStandardFormat, Parameters = { '%', typeof(Offset).FullName } },
            new Data(Offset.Zero) { Pattern = "%%", Message = Messages.Parse_PercentDoubled },
            new Data(Offset.Zero) { Pattern = "%\\", Message = Messages.Parse_EscapeAtEndOfString },
            new Data(Offset.Zero) { Pattern = "\\", Message = Messages.Parse_UnknownStandardFormat, Parameters = { '\\', typeof(Offset).FullName } },
            new Data(Offset.Zero) { Pattern = "H%", Message = Messages.Parse_PercentAtEndOfString },
            new Data(Offset.Zero) { Pattern = "hh", Message = Messages.Parse_Hour12PatternNotSupported, Parameters = { typeof(Offset).FullName } },
            new Data(Offset.Zero) { Pattern = "HHH", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'H', 2 } },
            new Data(Offset.Zero) { Pattern = "mmm", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'm', 2 } },
            new Data(Offset.Zero) { Pattern = "mmmmmmmmmmmmmmmmmmm", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'm', 2 } },
            new Data(Offset.Zero) { Pattern = "'qwe", Message = Messages.Parse_MissingEndQuote, Parameters = { '\'' } },
            new Data(Offset.Zero) { Pattern = "'qwe\\", Message = Messages.Parse_EscapeAtEndOfString },
            new Data(Offset.Zero) { Pattern = "'qwe\\'", Message = Messages.Parse_MissingEndQuote, Parameters = { '\'' } },
            new Data(Offset.Zero) { Pattern = "sss", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 's', 2 } },
        };

        /// <summary>
        /// Tests for parsing failures (of values)
        /// </summary>
        internal static readonly Data[] ParseFailureData = {
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "", Pattern = "g", Message = Messages.Parse_ValueStringEmpty },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "1", Pattern = "HH", Message = Messages.Parse_MismatchedNumber, Parameters = {"HH"} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "1", Pattern = "mm", Message = Messages.Parse_MismatchedNumber, Parameters = {"mm"} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "1", Pattern = "ss", Message = Messages.Parse_MismatchedNumber, Parameters = {"ss"} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "12:34 ", Pattern = "HH:mm", Message = Messages.Parse_ExtraValueCharacters, Parameters = {" "} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "1a", Pattern = "H ", Message = Messages.Parse_MismatchedCharacter, Parameters = {' '}},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "2:", Pattern = "%H", Message = Messages.Parse_ExtraValueCharacters, Parameters = {":"}},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "a", Pattern = "%.", Message = Messages.Parse_MismatchedCharacter, Parameters = {'.'}},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "a", Pattern = "%:", Message = Messages.Parse_TimeSeparatorMismatch},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "a", Pattern = "%H", Message = Messages.Parse_MismatchedNumber, Parameters = {"H"} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "a", Pattern = "%m", Message = Messages.Parse_MismatchedNumber, Parameters = {"m"} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "a", Pattern = "%s", Message = Messages.Parse_MismatchedNumber, Parameters = {"s"} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "a", Pattern = ".H", Message = Messages.Parse_MismatchedCharacter, Parameters = {'.'}},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "a", Pattern = "\\'", Message = Messages.Parse_EscapedCharacterMismatch, Parameters = {'\''} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "axc", Pattern = "'abc'", Message = Messages.Parse_QuotedStringMismatch},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "z", Pattern = "%*", Message = Messages.Parse_MismatchedCharacter, Parameters = {'*'} },
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "24", Pattern = "HH", Message = Messages.Parse_FieldValueOutOfRange, Parameters = {24, 'H', typeof(Offset) }},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "60", Pattern = "mm", Message = Messages.Parse_FieldValueOutOfRange, Parameters = {60, 'm', typeof(Offset) }},
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = "60", Pattern = "ss", Message = Messages.Parse_FieldValueOutOfRange, Parameters = {60, 's', typeof(Offset) }},
            new Data(Offset.Zero) { Text = "+12", Pattern = "-HH", Message = Messages.Parse_PositiveSignInvalid },
        };

        /// <summary>
        /// Common test data for both formatting and parsing. A test should be placed here unless is truly
        /// cannot be run both ways. This ensures that as many round-trip type tests are performed as possible.
        /// </summary>
        internal static readonly Data[] FormatAndParseData = {
/*XXX*/            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = ".", Pattern = "%." }, // decimal separator
            new Data(Offset.Zero) { Culture = Cultures.EnUs, Text = ":", Pattern = "%:" }, // date separator
/*XXX*/            new Data(Offset.Zero) { Culture = Cultures.ItIt, Text = ".", Pattern = "%." }, // decimal separator (always period)
            new Data(Offset.Zero) { Culture = Cultures.ItIt, Text = ".", Pattern = "%:" }, // date separator
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
            new Data(5, 0, 0) { Culture = Cultures.ItIt, Text = "+05", Pattern = "g" },
            new Data(5, 12, 0) { Culture = Cultures.ItIt, Text = "+05.12", Pattern = "g" },
            new Data(5, 12, 34) { Culture = Cultures.ItIt, Text = "+05.12.34", Pattern = "g" },
            new Data(Offset.MaxValue) { Culture = Cultures.ItIt, Text = "+18", Pattern = "g" },
            new Data(Offset.MinValue) { Culture = Cultures.ItIt, Text = "-18", Pattern = "g" },
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
        [TestCaseSource("ParseData")]
        public void ParsePartial(PatternTestData<Offset> data)
        {
            data.TestParsePartial();
        }

        [Test]
        [TestCaseSource("FormatData")]
        public void FormatPartial(PatternTestData<Offset> data)
        {
            data.TestFormatPartial();
        }

        /// <summary>
        /// A container for test data for formatting and parsing <see cref="Offset" /> objects.
        /// </summary>
        public sealed class Data : PatternTestData<Offset>
        {
            // Ignored anyway...
            protected override Offset DefaultTemplate
            {
                get { return Offset.Zero; }
            }

            public Data(Offset value)
                : base(value)
            {
            }

            public Data(int hours, int minutes)
                : this(Offset.FromHoursAndMinutes(hours, minutes))
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

            internal override IPattern<Offset> CreatePattern()
            {
                return OffsetPattern.CreateWithInvariantCulture(Pattern)
                    .WithCulture(Culture);
            }

            internal override IPartialPattern<Offset> CreatePartialPattern()
            {
                return OffsetPattern.CreateWithInvariantCulture(Pattern).WithCulture(Culture).UnderlyingPattern;
            }
        }

    }
}
