#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime.Properties;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    /// <summary>
    /// Defines the test data for the <see cref="Offset" /> type formatting and parsing tests.
    /// </summary>
    public class OffsetFormattingTestSupport : FormattingTestSupport
    {
        /// <summary>
        /// Test data that can only be used to test formatting.
        /// </summary>
        internal static readonly OffsetData[] FormatData = {
            new OffsetData(3, 0, 0, 0) { C = Cultures.EnUs, S = "", P = "%-", PV = Offset.Zero },
            new OffsetData(5, 6, 7, 8) { C = Cultures.EnUs, S = "", P = "%F"  },
            new OffsetData(5, 6, 7, 8) { C = Cultures.EnUs, S = "", P = "FF"  },
            new OffsetData(5, 0, 0, 0) { C = Cultures.EnUs, S = "+5" },
            new OffsetData(5, 12, 0, 0) { C = Cultures.EnUs, S = "+5:12" },
            new OffsetData(5, 12, 34, 0) { C = Cultures.EnUs, S = "+5:12:34" },
            new OffsetData(5, 12, 34, 567) { C = Cultures.EnUs, S = "+5:12:34.567" },
            // Note: we ignore the decimal separator, as per issue 21.
            new OffsetData(Offset.MaxValue) { C = Cultures.EnUs, S = "+23:59:59.999", P = "" },
            new OffsetData(Offset.MaxValue) { C = Cultures.EnUs, S = "+23:59:59.999" },
            new OffsetData(Offset.MinValue) { C = Cultures.EnUs, S = "-23:59:59.999" },
            new OffsetData(Offset.MaxValue) { C = Cultures.FrFr, S = "+23:59:59.999" },
            new OffsetData(Offset.MinValue) { C = Cultures.FrFr, S = "-23:59:59.999" },
            new OffsetData(Offset.MaxValue) { C = Cultures.ItIt, S = "+23.59.59.999" },
            new OffsetData(Offset.MinValue) { C = Cultures.ItIt, S = "-23.59.59.999" },
            new OffsetData(Offset.MaxValue) { C = null, S = "+23:59:59.999", ThreadCulture = Cultures.EnUs },
            new OffsetData(Offset.MinValue) { C = null, S = "-23:59:59.999", ThreadCulture = Cultures.EnUs },

            // Demonstrating fractional digit behaviour. Some of these could be parse as well, and duplicate
            // other tests, but they're mostly here for reference :)
            new OffsetData(0, 0, 3, 456) { C = Cultures.EnUs, S = "3.456", P = "s.fff" },
            new OffsetData(0, 0, 3, 456) { C = Cultures.EnUs, S = "3.45", P = "s.ff" }, // Fractions truncate for rounding purposes
            new OffsetData(0, 0, 3, 456) { C = Cultures.EnUs, S = "3.4", P = "s.f" },
            new OffsetData(0, 0, 3, 456) { C = Cultures.EnUs, S = "3.456", P = "s.FFF" },
            new OffsetData(0, 0, 3, 456) { C = Cultures.EnUs, S = "3.45", P = "s.FF" },
            new OffsetData(0, 0, 3, 456) { C = Cultures.EnUs, S = "3.4", P = "s.F" },
            // Demonstrate the difference between F and FF
            new OffsetData(0, 0, 3, 450) { C = Cultures.EnUs, S = "3.450", P = "s.fff" },
            new OffsetData(0, 0, 3, 450) { C = Cultures.EnUs, S = "3.45", P = "s.ff" },
            new OffsetData(0, 0, 3, 450) { C = Cultures.EnUs, S = "3.45", P = "s.FFF" }, // Last digit dropped
            new OffsetData(0, 0, 3, 450) { C = Cultures.EnUs, S = "3.45", P = "s.FF" },

            // Decimal point is dropped for F
            new OffsetData(0, 0, 3, 0) { C = Cultures.EnUs, S = "3.00", P = "s.ff" },
            new OffsetData(0, 0, 3, 0) { C = Cultures.EnUs, S = "3", P = "s.FF" },

            // But no other character is dropped - e.g. not the time separator
            new OffsetData(0, 0, 3, 0) { C = Cultures.EnUs, S = "3:00", P = "s:ff" },
            new OffsetData(0, 0, 3, 0) { C = Cultures.EnUs, S = "3:", P = "s:FF" },
        };

        /// <summary>
        /// Test data that can only be used to test parsing.
        /// </summary>
        internal static readonly OffsetData[] ParseData = {
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "", P = "g", Exception=typeof(UnparsableValueException), Message = Messages.Parse_ValueStringEmpty },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "1", P = "HH", Exception=typeof(UnparsableValueException), Message = Messages.Parse_MismatchedNumber, Parameters = {"HH"} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "1", P = "mm", Exception=typeof(UnparsableValueException), Message = Messages.Parse_MismatchedNumber, Parameters = {"mm"} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "1", P = "ss", Exception=typeof(UnparsableValueException), Message = Messages.Parse_MismatchedNumber, Parameters = {"ss"} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "12", P = "%f", Exception=typeof(UnparsableValueException), Message = Messages.Parse_ExtraValueCharacters, Parameters = {"2"} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "12", P = "%F", Exception=typeof(UnparsableValueException), Message = Messages.Parse_ExtraValueCharacters, Parameters = {"2"} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "12:34 ", P = "HH:mm", Exception=typeof(UnparsableValueException), Message = Messages.Parse_ExtraValueCharacters, Parameters = {" "} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "", Exception=typeof(InvalidPatternException), Message = Messages.Parse_FormatStringEmpty},
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "%%H", Exception=typeof(InvalidPatternException), Message = Messages.Parse_PercentDoubled},
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "ff", Exception=typeof(UnparsableValueException), Message = Messages.Parse_ExtraValueCharacters, Parameters = {"3"} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "FF", Exception=typeof(UnparsableValueException), Message = Messages.Parse_ExtraValueCharacters, Parameters = {"3"} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = null, Exception=typeof(ArgumentNullException), ArgumentName = "pattern" },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "1234", P = "fff", Exception=typeof(UnparsableValueException), Message = Messages.Parse_ExtraValueCharacters, Parameters = {"4"} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "1234", P = "FFF", Exception=typeof(UnparsableValueException), Message = Messages.Parse_ExtraValueCharacters, Parameters = {"4"} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "1a", P = "H ", Exception=typeof(UnparsableValueException), Message = Messages.Parse_MismatchedCharacter, Parameters = {' '}},
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "2:", P = "%H", Exception=typeof(UnparsableValueException), Message = Messages.Parse_ExtraValueCharacters, Parameters = {":"}},
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "a", P = "%.", Exception=typeof(UnparsableValueException), Message = Messages.Parse_MismatchedCharacter, Parameters = {'.'}},
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "a", P = "%:", Exception=typeof(UnparsableValueException), Message = Messages.Parse_TimeSeparatorMismatch},
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "a", P = "%H", Exception=typeof(UnparsableValueException), Message = Messages.Parse_MismatchedNumber, Parameters = {"H"} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "a", P = "%m", Exception=typeof(UnparsableValueException), Message = Messages.Parse_MismatchedNumber, Parameters = {"m"} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "a", P = "%s", Exception=typeof(UnparsableValueException), Message = Messages.Parse_MismatchedNumber, Parameters = {"s"} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "a", P = ".H", Exception=typeof(UnparsableValueException), Message = Messages.Parse_MismatchedCharacter, Parameters = {'.'}},
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "a", P = "\\'", Exception=typeof(UnparsableValueException), Message = Messages.Parse_EscapedCharacterMismatch, Parameters = {'\''} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "axc", P = "'abc'", Exception=typeof(UnparsableValueException), Message = Messages.Parse_QuotedStringMismatch},
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "z", P = "%y", Exception=typeof(UnparsableValueException), Message = Messages.Parse_MismatchedCharacter, Parameters = {'y'} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "10:10", P = "HH:HH", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatedFieldInPattern, Parameters = {'H'} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "10:10", P = "FF:ff", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatedFieldInPattern, Parameters = {'f'} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "10:10", P = "ff:FF", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatedFieldInPattern, Parameters = {'F'} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "10:10", P = "mm:mm", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatedFieldInPattern, Parameters = {'m'} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "10:10", P = "ss:ss", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatedFieldInPattern, Parameters = {'s'} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "-10:10", P = "+HH:-mm", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatedFieldInPattern, Parameters = {'-'} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "-10:10", P = "-HH:+mm", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatedFieldInPattern, Parameters = {'+'} },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = null, P = "g", Exception=typeof(ArgumentNullException), ArgumentName = "value" },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "24", P = "HH", Exception=typeof(UnparsableValueException), Message = Messages.Parse_FieldValueOutOfRange, Parameters = {24, 'H', typeof(Offset) }},
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "60", P = "mm", Exception=typeof(UnparsableValueException), Message = Messages.Parse_FieldValueOutOfRange, Parameters = {60, 'm', typeof(Offset) }},
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "60", P = "ss", Exception=typeof(UnparsableValueException), Message = Messages.Parse_FieldValueOutOfRange, Parameters = {60, 's', typeof(Offset) }},
            new OffsetData(Offset.Zero) { S = "+12", P = "-HH", Exception = typeof(UnparsableValueException), Message = Messages.Parse_PositiveSignInvalid }
        };

        /// <summary>
        /// Common test data for both formatting and parsing. A test should be placed here unless is truly
        /// cannot be run both ways. This ensures that as many round-trip type tests are performed as possible.
        /// </summary>
        internal static readonly OffsetData[] CommonData = {
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = ".", P = "%.", Name = "decimal separator" },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = ":", P = "%:", Name = "date separator" },
            new OffsetData(Offset.Zero) { C = Cultures.ItIt, S = ".", P = "%.", Name = "decimal separator (always period)" },
            new OffsetData(Offset.Zero) { C = Cultures.ItIt, S = ".", P = "%:", Name = "date separator" },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "H", P = "\\H" },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "HHss", P = "'HHss'" },
            new OffsetData(0, 0, 0, 100) { C = Cultures.EnUs, S = "1", P = "%f" },
            new OffsetData(0, 0, 0, 100) { C = Cultures.EnUs, S = "1", P = "%F" },
            new OffsetData(0, 0, 0, 100) { C = Cultures.EnUs, S = "1", P = "FF" },
            new OffsetData(0, 0, 0, 100) { C = Cultures.EnUs, S = "1", P = "FFF" },
            new OffsetData(0, 0, 0, 120) { C = Cultures.EnUs, S = "12", P = "ff" },
            new OffsetData(0, 0, 0, 120) { C = Cultures.EnUs, S = "12", P = "FF" },
            new OffsetData(0, 0, 0, 120) { C = Cultures.EnUs, S = "12", P = "FFF" },
            new OffsetData(0, 0, 0, 123) { C = Cultures.EnUs, S = "123", P = "fff" },
            new OffsetData(0, 0, 0, 123) { C = Cultures.EnUs, S = "123", P = "FFF" },
            new OffsetData(0, 0, 0, 600) { C = Cultures.EnUs, S = ".6", P = ".f" },
            new OffsetData(0, 0, 0, 600) { C = Cultures.EnUs, S = ".6", P = ".F" },
            new OffsetData(0, 0, 0, 600) { C = Cultures.EnUs, S = ".6", P = ".FFF", Name = "elided zeros" },
            new OffsetData(0, 0, 0, 678) { C = Cultures.EnUs, S = ".678", P = ".fff" },
            new OffsetData(0, 0, 0, 678) { C = Cultures.EnUs, S = ".678", P = ".FFF" },
            new OffsetData(0, 0, 12, 0) { C = Cultures.EnUs, S = "12", P = "%s" },
            new OffsetData(0, 0, 12, 0) { C = Cultures.EnUs, S = "12", P = "ss" },
            new OffsetData(0, 0, 2, 0) { C = Cultures.EnUs, S = "2", P = "%s" },
            new OffsetData(0, 1, 23, 0) { C = Cultures.EnUs, S = "1:23", P = "HH:mm\0m:ss"},
            new OffsetData(0, 12, 0, 0) { C = Cultures.EnUs, S = "12", P = "%m" },
            new OffsetData(0, 12, 0, 0) { C = Cultures.EnUs, S = "12", P = "mm" },
            new OffsetData(0, 2, 0, 0) { C = Cultures.EnUs, S = "2", P = "%m" },
            new OffsetData(1, 0, 0, 0) { C = Cultures.EnUs, S = "1", P = "H.FFF", Name = "missing fraction" },
            new OffsetData(1, 1, 1, 400) { C = Cultures.EnUs, S = "4", P = "%f", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 400) { C = Cultures.EnUs, S = "4", P = "%F", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 400) { C = Cultures.EnUs, S = "4", P = "FF", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 400) { C = Cultures.EnUs, S = "4", P = "FFF", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 400) { C = Cultures.EnUs, S = "40", P = "ff", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 400) { C = Cultures.EnUs, S = "400", P = "fff", PV = Offset.Create(0, 0, 0, 400) },
            new OffsetData(1, 1, 1, 450) { C = Cultures.EnUs, S = "4", P = "%f", PV = Offset.Create(0, 0, 0, 400) },
            new OffsetData(1, 1, 1, 450) { C = Cultures.EnUs, S = "4", P = "%F", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 450) { C = Cultures.EnUs, S = "45", P = "ff", PV = Offset.Create(0, 0, 0, 450)  },
            new OffsetData(1, 1, 1, 450) { C = Cultures.EnUs, S = "45", P = "FF", PV = Offset.Create(0, 0, 0, 450)  },
            new OffsetData(1, 1, 1, 450) { C = Cultures.EnUs, S = "45", P = "FFF", PV = Offset.Create(0, 0, 0, 450)  },
            new OffsetData(1, 1, 1, 450) { C = Cultures.EnUs, S = "450", P = "fff", PV = Offset.Create(0, 0, 0, 450)  },
            new OffsetData(1, 1, 1, 456) { C = Cultures.EnUs, S = "4", P = "%f", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 456) { C = Cultures.EnUs, S = "4", P = "%F", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 456) { C = Cultures.EnUs, S = "45", P = "ff", PV = Offset.Create(0, 0, 0, 450)  },
            new OffsetData(1, 1, 1, 456) { C = Cultures.EnUs, S = "45", P = "FF", PV = Offset.Create(0, 0, 0, 450)  },
            new OffsetData(1, 1, 1, 456) { C = Cultures.EnUs, S = "456", P = "fff", PV = Offset.Create(0, 0, 0, 456)  },
            new OffsetData(1, 1, 1, 456) { C = Cultures.EnUs, S = "456", P = "FFF", PV = Offset.Create(0, 0, 0, 456)  },
            new OffsetData(12, 0, 0, 0) { C = Cultures.EnUs, S = "12", P = "%H" },
            new OffsetData(12, 0, 0, 0) { C = Cultures.EnUs, S = "12", P = "HH" },
            new OffsetData(2, 0, 0, 0) { C = Cultures.EnUs, S = "2", P = "%H" },
            new OffsetData(2, 0, 0, 0) { C = Cultures.EnUs, S = "2", P = "%H" },
            new OffsetData(3, 0, 0, 0, true) { C = Cultures.EnUs, S = "-", P = "%-", PV = Offset.Zero  },
            new OffsetData(3, 0, 0, 0) { C = Cultures.EnUs, S = "+", P = "%+", PV = Offset.Zero  },
            new OffsetData(3, 0, 0, 0, true) { C = Cultures.EnUs, S = "-", P = "%+", PV = Offset.Zero  },
            new OffsetData(5, 0, 0, 0) { C = Cultures.EnUs, S = "+5", P = "g"  },
            new OffsetData(5, 12, 0, 0) { C = Cultures.EnUs, S = "+5:12", P = "g"  },
            new OffsetData(5, 12, 34, 0) { C = Cultures.EnUs, S = "+5:12:34", P = "g"  },
            new OffsetData(5, 12, 34, 567) { C = Cultures.EnUs, S = "+5", P = "s", PV = Offset.Create(5, 0, 0, 0)  },
            new OffsetData(5, 12, 34, 567) { C = Cultures.EnUs, S = "+5:12", P = "m", PV = Offset.Create(5, 12, 0, 0)  },
            new OffsetData(5, 12, 34, 567) { C = Cultures.EnUs, S = "+5:12:34", P = "l", PV = Offset.Create(5, 12, 34, 0)  },
            new OffsetData(5, 12, 34, 567) { C = Cultures.EnUs, S = "+5:12:34.567", P = "f"  },
            new OffsetData(5, 12, 34, 567) { C = Cultures.EnUs, S = "+5:12:34.567", P = "g"  },
            new OffsetData(5, 12, 34, 567) { C = Cultures.EnUs, S = "18,754,567", P = "n"  },
            // See issue 15
            new OffsetData(0, 0, 12, 340) { C = Cultures.EnUs, S = "12.34", P = "ss.FFF"  },
            new OffsetData(5, 6, 7, 8) { C = Cultures.EnUs, S = "0", P = "%f", PV = Offset.Create(0, 0, 0, 0)  },
            new OffsetData(5, 6, 7, 8) { C = Cultures.EnUs, S = "00", P = "ff", PV = Offset.Create(0, 0, 0, 0)  },
            new OffsetData(5, 6, 7, 8) { C = Cultures.EnUs, S = "008", P = "fff", PV = Offset.Create(0, 0, 0, 8)  },
            new OffsetData(5, 6, 7, 8) { C = Cultures.EnUs, S = "008", P = "FFF", PV = Offset.Create(0, 0, 0, 8)  },
            new OffsetData(5, 6, 7, 8) { C = Cultures.EnUs, S = "05", P = "HH", PV = Offset.Create(5, 0, 0, 0)  },
            new OffsetData(5, 6, 7, 8) { C = Cultures.EnUs, S = "06", P = "mm", PV = Offset.Create(0, 6, 0, 0)  },
            new OffsetData(5, 6, 7, 8) { C = Cultures.EnUs, S = "07", P = "ss", PV = Offset.Create(0, 0, 7, 0)  },
            new OffsetData(5, 6, 7, 8) { C = Cultures.EnUs, S = "5", P = "%H", PV = Offset.Create(5, 0, 0, 0)  },
            new OffsetData(5, 6, 7, 8) { C = Cultures.EnUs, S = "6", P = "%m", PV = Offset.Create(0, 6, 0, 0)  },
            new OffsetData(5, 6, 7, 8) { C = Cultures.EnUs, S = "7", P = "%s", PV = Offset.Create(0, 0, 7, 0)  },
            new OffsetData(Offset.MaxValue) { C = Cultures.EnUs, S = "+23:59:59.999", P = "g" },
            new OffsetData(Offset.MaxValue) { C = Cultures.EnUs, S = "23", P = "%H", PV = Offset.Create(23, 0, 0, 0)  },
            new OffsetData(Offset.MaxValue) { C = Cultures.EnUs, S = "59", P = "%m", PV = Offset.Create(0, 59, 0, 0)  },
            new OffsetData(Offset.MaxValue) { C = Cultures.EnUs, S = "59", P = "%s", PV = Offset.Create(0, 0, 59, 0)  },
            new OffsetData(Offset.MaxValue) { C = Cultures.EnUs, S = "m", P = "\\m", PV = Offset.Create(0, 0, 0, 0) },
            new OffsetData(Offset.MaxValue) { C = Cultures.EnUs, S = "m", P = "'m'", PV = Offset.Create(0, 0, 0, 0) },
            new OffsetData(Offset.MaxValue) { C = Cultures.EnUs, S = "mmmmmmmmmm", P = "'mmmmmmmmmm'", PV = Offset.Create(0, 0, 0, 0) },
            new OffsetData(Offset.MaxValue) { C = Cultures.EnUs, S = "z", P = "%z", PV = Offset.Create(0, 0, 0, 0) },
            new OffsetData(Offset.MaxValue) { C = Cultures.EnUs, S = "zqw", P = "zqw", PV = Offset.Create(0, 0, 0, 0) },
            new OffsetData(Offset.MinValue) { C = Cultures.EnUs, S = "-23:59:59.999", P = "g" },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "+0", P = "s"  },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "+0:00", P = "m"  },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "+0:00:00", P = "l"  },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "+0:00:00.000", P = "f"  },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "!", Exception=typeof(InvalidPatternException), Message = Messages.Parse_UnknownStandardFormat, Parameters = {'!', typeof(Offset).FullName}},
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "%", Exception=typeof(InvalidPatternException), Message = Messages.Parse_UnknownStandardFormat, Parameters = { '%', typeof(Offset).FullName } },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "%%", Exception=typeof(InvalidPatternException), Message = Messages.Parse_PercentDoubled },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "%\\", Exception=typeof(InvalidPatternException), Message = Messages.Parse_EscapeAtEndOfString },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "\\", Exception=typeof(InvalidPatternException), Message = Messages.Parse_UnknownStandardFormat, Parameters = { '\\', typeof(Offset).FullName } },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "ffff", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'f', 3 } },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "H%", Exception=typeof(InvalidPatternException), Message = Messages.Parse_PercentAtEndOfString },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "hh", Exception=typeof(InvalidPatternException), Message = Messages.Parse_Hour12PatternNotSupported, Parameters = { typeof(Offset).FullName } },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "HHH", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'H', 2 } },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "mmm", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'm', 2 } },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "mmmmmmmmmmmmmmmmmmm", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'm', 2 } },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "'qwe", Exception=typeof(InvalidPatternException), Message = Messages.Parse_MissingEndQuote, Parameters = { '\'' } },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "'qwe\\", Exception=typeof(InvalidPatternException), Message = Messages.Parse_EscapeAtEndOfString },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "'qwe\\'", Exception=typeof(InvalidPatternException), Message = Messages.Parse_MissingEndQuote, Parameters = { '\'' } },
            new OffsetData(Offset.Zero) { C = Cultures.EnUs, S = "123", P = "sss", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatCountExceeded, Parameters = { 's', 2 } },
            new OffsetData(5, 0, 0, 0) { C = Cultures.FrFr, S = "+5", P = "g" },
            new OffsetData(5, 12, 0, 0) { C = Cultures.FrFr, S = "+5:12", P = "g" },
            new OffsetData(5, 12, 34, 0) { C = Cultures.FrFr, S = "+5:12:34", P = "g" },
            new OffsetData(5, 12, 34, 567) { C = Cultures.FrFr, S = "+5:12:34.567", P = "g" }, // Note still a period, not a comma
            new OffsetData(5, 12, 34, 567) { C = Cultures.FrFr, S = "18" + Nbsp + "754" + Nbsp + "567", P = "n" },
            new OffsetData(Offset.MaxValue) { C = Cultures.FrFr, S = "+23:59:59.999", P = "g" },
            new OffsetData(Offset.MinValue) { C = Cultures.FrFr, S = "-23:59:59.999", P = "g" },
            new OffsetData(5, 0, 0, 0) { C = Cultures.ItIt, S = "+5", P = "g" },
            new OffsetData(5, 12, 0, 0) { C = Cultures.ItIt, S = "+5.12", P = "g" },
            new OffsetData(5, 12, 34, 0) { C = Cultures.ItIt, S = "+5.12.34", P = "g" },
            new OffsetData(5, 12, 34, 567) { C = Cultures.ItIt, S = "+5.12.34.567", P = "g" },
            new OffsetData(5, 12, 34, 567) { C = Cultures.ItIt, S = "18.754.567", P = "n" },
            new OffsetData(Offset.MaxValue) { C = Cultures.ItIt, S = "+23.59.59.999", P = "g" },
            new OffsetData(Offset.MinValue) { C = Cultures.ItIt, S = "-23.59.59.999", P = "g" },
            new OffsetData(Offset.MaxValue) { C = null, S = "+23:59:59.999", P = "g", ThreadCulture = Cultures.EnUs },
            new OffsetData(Offset.MinValue) { C = null, S = "-23:59:59.999", P = "g", ThreadCulture = Cultures.EnUs },
            new OffsetData(Offset.MaxValue) { C = Cultures.EnUs, S = "+23:59:59.999", P = "g", ThreadCulture = Cultures.ItIt },
            new OffsetData(Offset.MinValue) { C = Cultures.EnUs, S = "-23:59:59.999", P = "g", ThreadCulture = Cultures.ItIt },
            new OffsetData(0, 30, 0, 0, true) { C = Cultures.EnUs, S = "-00:30", P = "+HH:mm" },
            new OffsetData(0, 30, 0, 0, true) { C = Cultures.EnUs, S = "-00:30", P = "-HH:mm" },
            new OffsetData(0, 30, 0, 0, false) { C = Cultures.EnUs, S = "00:30", P = "-HH:mm" },
        };

        internal static readonly IEnumerable<OffsetData> AllParseData = ParseData.Concat(CommonData);
        internal static readonly IEnumerable<OffsetData> AllFormatData = FormatData.Concat(CommonData);
        internal static readonly IEnumerable<OffsetData> FormatWithoutFormat = AllFormatData.Where(data => data.P == null);

        #region Nested type: OffsetData
        /// <summary>
        /// A container for test data for formatting and parsing <see cref="Offset" /> objects.
        /// </summary>
        public sealed class OffsetData : AbstractFormattingData<Offset>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="OffsetData" /> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public OffsetData(Offset value)
                : base(value)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="OffsetData" /> class.
            /// </summary>
            /// <param name="hours">The hours.</param>
            /// <param name="minutes">The minutes.</param>
            /// <param name="seconds">The seconds.</param>
            /// <param name="fractions">The fractions.</param>
            public OffsetData(int hours, int minutes, int seconds, int fractions)
                : this(hours, minutes, seconds, fractions, false)
            {
            }

            public OffsetData(int hours, int minutes, int seconds, int fractions, bool negative)
                : this(Offset.Create(hours, minutes, seconds, fractions, negative))
            {
            }

            /// <summary>
            /// Returns a string representation of the given value. This will usually not call the ToString()
            /// method as that is problably being tested. The returned string is only used in test code and
            /// labels so it doesn't have to be beautiful. Must handle <c>null</c> if the type is a reference
            /// type. This should not throw an exception.
            /// </summary>
            /// <param name="value">The value to format.</param>
            /// <returns>The string representation.</returns>
            protected override string ValueLabel(Offset value)
            {
                if (value == Offset.MaxValue)
                {
                    return "Offset.MaxValue";
                }
                if (value == Offset.MinValue)
                {
                    return "Offset.MinValue";
                }
                if (value == Offset.Zero)
                {
                    return "Offset.Zero";
                }
                return string.Format("Offset.Create({0}{1}, {2}, {3}, {4})",
                                     value.IsNegative ? "-" : "",
                                     value.Hours,
                                     value.Minutes,
                                     value.Seconds,
                                     value.FractionalSeconds);
            }
        }
        #endregion
    }
}
