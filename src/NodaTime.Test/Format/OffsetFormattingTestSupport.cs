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
#region usings
using System;
using System.Collections.Generic;
using NodaTime.Format;
using NodaTime.Properties;

#endregion

namespace NodaTime.Test.Format
{
    /// <summary>
    ///   Defines the test data for the <see cref="Offset" /> type formatting and parsing tests.
    /// </summary>
    public class OffsetFormattingTestSupport : FormattingTestSupport
    {
        /// <summary>
        ///   Test data that can only be used to test formatting.
        /// </summary>
        internal static OffsetData[] OffsetFormatData = {
            new OffsetData(3, 0, 0, 0) { C = EnUs, S = "", F = "%-", PV = Offset.Zero },
            new OffsetData(5, 6, 7, 8) { C = EnUs, S = "", F = "%F"  },
            new OffsetData(5, 6, 7, 8) { C = EnUs, S = "", F = "FF"  },
            new OffsetData(5, 0, 0, 0) { C = EnUs, S = "+5" },
            new OffsetData(5, 12, 0, 0) { C = EnUs, S = "+5:12" },
            new OffsetData(5, 12, 34, 0) { C = EnUs, S = "+5:12:34" },
            new OffsetData(5, 12, 34, 567) { C = EnUs, S = "+5:12:34.567" },
            new OffsetData(Offset.MaxValue) { C = EnUs, S = "+23:59:59.999", F = "" },
            new OffsetData(Offset.MaxValue) { C = EnUs, S = "+23:59:59.999" },
            new OffsetData(Offset.MinValue) { C = EnUs, S = "-23:59:59.999" },
            new OffsetData(Offset.MaxValue) { C = FrFr, S = "+23:59:59,999" },
            new OffsetData(Offset.MinValue) { C = FrFr, S = "-23:59:59,999" },
            new OffsetData(Offset.MaxValue) { C = ItIt, S = "+23.59.59,999" },
            new OffsetData(Offset.MinValue) { C = ItIt, S = "-23.59.59,999" },
            new OffsetData(Offset.MaxValue) { C = null, S = "+23:59:59.999", ThreadCulture = EnUs },
            new OffsetData(Offset.MinValue) { C = null, S = "-23:59:59.999", ThreadCulture = EnUs },
        };

        /// <summary>
        ///   Test data that can only be used to test parsing.
        /// </summary>
        internal static OffsetData[] OffsetParseData = {
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "  12:34  ", F = "  '  'HH:mm'  '  ", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "  12:34  ", F = "  '  'HH:mm'  '  ", Styles = SurroundingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "  12:34  ", F = "HH:mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "  12:34  ", F = "HH:mm", Styles = SurroundingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "  12:34", F = "  '  'HH:mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "  12:34", F = "  '  'HH:mm", Styles = LeadingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "  12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "  12:34", F = "HH:mm", Styles = LeadingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = " 12:34", F = "HH :mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = " 12:34", F = "HH :mm", Styles = InnerSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = " 12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = " 12:34", F = "HH:mm", Styles = InnerSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12  :34", F = "HH:mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12  :34", F = "HH:mm", Styles = InnerSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34  ", F = "HH:mm'  '  ", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34  ", F = "HH:mm'  '  ", Styles = TrailingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34  ", F = "HH:mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34  ", F = "HH:mm", Styles = TrailingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "  '  'HH:mm'  '  ", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "  '  'HH:mm'  '  ", Styles = SurroundingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "  '  'HH:mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "  '  'HH:mm", Styles = LeadingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "  HH:mm  ", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "'  'HH:mm'  '", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "  HH:mm  ", Styles = SurroundingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "'  'HH:mm'  '", Styles = SurroundingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "  HH:mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "'  'HH:mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "  HH:mm", Styles = LeadingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "'  'HH:mm", Styles = LeadingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = " HH:mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = " HH:mm", Styles = InnerSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH  :mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH  :mm", Styles = InnerSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH:mm'  '  ", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH:mm'  '  ", Styles = TrailingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH:mm  ", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH:mm'  '", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH:mm  ", Styles = TrailingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH:mm'  '", Styles = TrailingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH:mm", Styles = InnerSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH:mm", Styles = LeadingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH:mm", Styles = SurroundingSpace },
            new OffsetData(12, 34, 0, 0) { C = EnUs, S = "12:34", F = "HH:mm", Styles = TrailingSpace },
            new OffsetData(Offset.Zero) { C = EnUs, S = "", F = "g", Exception=typeof(FormatException), Message = Resources.Parse_ValueStringEmpty },
            new OffsetData(Offset.Zero) { C = EnUs, S = "1", F = "HH", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_MismatchedNumber, Parameters = {"HH"} },
            new OffsetData(Offset.Zero) { C = EnUs, S = "1", F = "mm", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_MismatchedNumber, Parameters = {"mm"} },
            new OffsetData(Offset.Zero) { C = EnUs, S = "1", F = "ss", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_MismatchedNumber, Parameters = {"ss"} },
            new OffsetData(Offset.Zero) { C = EnUs, S = "12", F = "%f", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_ExtraValueCharacters, Parameters = {"2"} },
            new OffsetData(Offset.Zero) { C = EnUs, S = "12", F = "%F", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_ExtraValueCharacters, Parameters = {"2"} },
            new OffsetData(Offset.Zero) { C = EnUs, S = "12:34 ", F = "HH:mm", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_ExtraValueCharacters, Parameters = {" "} },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "", Exception=typeof(FormatException), Message = Resources.Parse_FormatStringEmpty},
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "%%H", Exception=typeof(FormatException), Message = Resources.Parse_PercentDoubled},
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "ff", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_ExtraValueCharacters, Parameters = {"3"} },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "FF", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_ExtraValueCharacters, Parameters = {"3"} },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = null, Exception=typeof(ArgumentNullException), ArgumentName = "format" },
            new OffsetData(Offset.Zero) { C = EnUs, S = "1234", F = "fff", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_ExtraValueCharacters, Parameters = {"4"} },
            new OffsetData(Offset.Zero) { C = EnUs, S = "1234", F = "FFF", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_ExtraValueCharacters, Parameters = {"4"} },
            new OffsetData(Offset.Zero) { C = EnUs, S = "1a", F = "H ", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_MismatchedSpace},
            new OffsetData(Offset.Zero) { C = EnUs, S = "2:", F = "%H", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_ExtraValueCharacters, Parameters = {":"}},
            new OffsetData(Offset.Zero) { C = EnUs, S = "a", F = "%.", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_MissingDecimalSeparator},
            new OffsetData(Offset.Zero) { C = EnUs, S = "a", F = "%:", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_TimeSeparatorMismatch},
            new OffsetData(Offset.Zero) { C = EnUs, S = "a", F = "%H", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_MismatchedNumber, Parameters = {"H"} },
            new OffsetData(Offset.Zero) { C = EnUs, S = "a", F = "%m", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_MismatchedNumber, Parameters = {"m"} },
            new OffsetData(Offset.Zero) { C = EnUs, S = "a", F = "%s", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_MismatchedNumber, Parameters = {"s"} },
            new OffsetData(Offset.Zero) { C = EnUs, S = "a", F = ".H", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_MissingDecimalSeparator},
            new OffsetData(Offset.Zero) { C = EnUs, S = "a", F = "\\'", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_EscapedCharacterMismatch, Parameters = {'\''} },
            new OffsetData(Offset.Zero) { C = EnUs, S = "axc", F = "'abc'", Exception=typeof(FormatException), Message = Resources.Parse_QuotedStringMismatch},
            new OffsetData(Offset.Zero) { C = EnUs, S = "z", F = "%y", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_MismatchedCharacter, Parameters = {'y'} },
            new OffsetData(Offset.Zero) { C = EnUs, S = null, F = "g", Exception=typeof(ArgumentNullException), ArgumentName = "value" },
        };

        /// <summary>
        ///   Common test data for both formatting and parsing. A test should be placed here unless is truely
        ///   cannot be run both ways. This ensures that as many roud-trip type tests are performed as possible.
        /// </summary>
        internal static OffsetData[] OffsetFormattingCommonData = {
            new OffsetData(Offset.Zero) { C = EnUs, S = ".", F = "%.", Name = "decimal separator" },
            new OffsetData(Offset.Zero) { C = EnUs, S = ":", F = "%:", Name = "date separator" },
            new OffsetData(Offset.Zero) { C = ItIt, S = ",", F = "%.", Name = "decimal separator" },
            new OffsetData(Offset.Zero) { C = ItIt, S = ".", F = "%:", Name = "date separator" },
            new OffsetData(Offset.Zero) { C = EnUs, S = "H", F = "\\H" },
            new OffsetData(Offset.Zero) { C = EnUs, S = "HHss", F = "'HHss'" },
            new OffsetData(0, 0, 0, 100) { C = EnUs, S = "1", F = "%f" },
            new OffsetData(0, 0, 0, 100) { C = EnUs, S = "1", F = "%F" },
            new OffsetData(0, 0, 0, 100) { C = EnUs, S = "1", F = "FF" },
            new OffsetData(0, 0, 0, 100) { C = EnUs, S = "1", F = "FFF" },
            new OffsetData(0, 0, 0, 120) { C = EnUs, S = "12", F = "ff" },
            new OffsetData(0, 0, 0, 120) { C = EnUs, S = "12", F = "FF" },
            new OffsetData(0, 0, 0, 120) { C = EnUs, S = "12", F = "FFF" },
            new OffsetData(0, 0, 0, 123) { C = EnUs, S = "123", F = "fff" },
            new OffsetData(0, 0, 0, 123) { C = EnUs, S = "123", F = "FFF" },
            new OffsetData(0, 0, 0, 600) { C = EnUs, S = ".6", F = ".f" },
            new OffsetData(0, 0, 0, 600) { C = EnUs, S = ".6", F = ".F" },
            new OffsetData(0, 0, 0, 600) { C = EnUs, S = ".6", F = ".FFF", Name = "elided zeros" },
            new OffsetData(0, 0, 0, 678) { C = EnUs, S = ".678", F = ".fff" },
            new OffsetData(0, 0, 0, 678) { C = EnUs, S = ".678", F = ".FFF" },
            new OffsetData(0, 0, 12, 0) { C = EnUs, S = "12", F = "%s" },
            new OffsetData(0, 0, 12, 0) { C = EnUs, S = "12", F = "ss" },
            new OffsetData(0, 0, 2, 0) { C = EnUs, S = "2", F = "%s" },
            new OffsetData(0, 1, 23, 0) { C = EnUs, S = "1:23", F = "HH:mm\0m:ss"},
            new OffsetData(0, 12, 0, 0) { C = EnUs, S = "12", F = "%m" },
            new OffsetData(0, 12, 0, 0) { C = EnUs, S = "12", F = "mm" },
            new OffsetData(0, 2, 0, 0) { C = EnUs, S = "2", F = "%m" },
            new OffsetData(1, 0, 0, 0) { C = EnUs, S = "1", F = "H.FFF", Name = "missing fraction" },
            new OffsetData(1, 1, 1, 400) { C = EnUs, S = "4", F = "%f", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 400) { C = EnUs, S = "4", F = "%F", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 400) { C = EnUs, S = "4", F = "FF", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 400) { C = EnUs, S = "4", F = "FFF", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 400) { C = EnUs, S = "40", F = "ff", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 400) { C = EnUs, S = "400", F = "fff", PV = Offset.Create(0, 0, 0, 400) },
            new OffsetData(1, 1, 1, 450) { C = EnUs, S = "4", F = "%f", PV = Offset.Create(0, 0, 0, 400) },
            new OffsetData(1, 1, 1, 450) { C = EnUs, S = "4", F = "%F", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 450) { C = EnUs, S = "45", F = "ff", PV = Offset.Create(0, 0, 0, 450)  },
            new OffsetData(1, 1, 1, 450) { C = EnUs, S = "45", F = "FF", PV = Offset.Create(0, 0, 0, 450)  },
            new OffsetData(1, 1, 1, 450) { C = EnUs, S = "45", F = "FFF", PV = Offset.Create(0, 0, 0, 450)  },
            new OffsetData(1, 1, 1, 450) { C = EnUs, S = "450", F = "fff", PV = Offset.Create(0, 0, 0, 450)  },
            new OffsetData(1, 1, 1, 456) { C = EnUs, S = "4", F = "%f", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 456) { C = EnUs, S = "4", F = "%F", PV = Offset.Create(0, 0, 0, 400)  },
            new OffsetData(1, 1, 1, 456) { C = EnUs, S = "45", F = "ff", PV = Offset.Create(0, 0, 0, 450)  },
            new OffsetData(1, 1, 1, 456) { C = EnUs, S = "45", F = "FF", PV = Offset.Create(0, 0, 0, 450)  },
            new OffsetData(1, 1, 1, 456) { C = EnUs, S = "456", F = "fff", PV = Offset.Create(0, 0, 0, 456)  },
            new OffsetData(1, 1, 1, 456) { C = EnUs, S = "456", F = "FFF", PV = Offset.Create(0, 0, 0, 456)  },
            new OffsetData(12, 0, 0, 0) { C = EnUs, S = "12", F = "%H" },
            new OffsetData(12, 0, 0, 0) { C = EnUs, S = "12", F = "HH" },
            new OffsetData(2, 0, 0, 0) { C = EnUs, S = "2", F = "%H" },
            new OffsetData(2, 0, 0, 0) { C = EnUs, S = "2", F = "%H" },
            new OffsetData(-3, 0, 0, 0) { C = EnUs, S = "-", F = "%-", PV = Offset.Zero  },
            new OffsetData(3, 0, 0, 0) { C = EnUs, S = "+", F = "%+", PV = Offset.Zero  },
            new OffsetData(-3, 0, 0, 0) { C = EnUs, S = "-", F = "%+", PV = Offset.Zero  },
            new OffsetData(5, 0, 0, 0) { C = EnUs, S = "+5", F = "g"  },
            new OffsetData(5, 12, 0, 0) { C = EnUs, S = "+5:12", F = "g"  },
            new OffsetData(5, 12, 34, 0) { C = EnUs, S = "+5:12:34", F = "g"  },
            new OffsetData(5, 12, 34, 567) { C = EnUs, S = "+5", F = "s", PV = Offset.Create(5, 0, 0, 0)  },
            new OffsetData(5, 12, 34, 567) { C = EnUs, S = "+5:12", F = "m", PV = Offset.Create(5, 12, 0, 0)  },
            new OffsetData(5, 12, 34, 567) { C = EnUs, S = "+5:12:34", F = "l", PV = Offset.Create(5, 12, 34, 0)  },
            new OffsetData(5, 12, 34, 567) { C = EnUs, S = "+5:12:34.567", F = "f"  },
            new OffsetData(5, 12, 34, 567) { C = EnUs, S = "+5:12:34.567", F = "g"  },
            new OffsetData(5, 12, 34, 567) { C = EnUs, S = "18,754,567", F = "n"  },
            new OffsetData(5, 6, 7, 8) { C = EnUs, S = "0", F = "%f", PV = Offset.Create(0, 0, 0, 0)  },
            new OffsetData(5, 6, 7, 8) { C = EnUs, S = "00", F = "ff", PV = Offset.Create(0, 0, 0, 0)  },
            new OffsetData(5, 6, 7, 8) { C = EnUs, S = "008", F = "fff", PV = Offset.Create(0, 0, 0, 8)  },
            new OffsetData(5, 6, 7, 8) { C = EnUs, S = "008", F = "FFF", PV = Offset.Create(0, 0, 0, 8)  },
            new OffsetData(5, 6, 7, 8) { C = EnUs, S = "05", F = "HH", PV = Offset.Create(5, 0, 0, 0)  },
            new OffsetData(5, 6, 7, 8) { C = EnUs, S = "06", F = "mm", PV = Offset.Create(0, 6, 0, 0)  },
            new OffsetData(5, 6, 7, 8) { C = EnUs, S = "07", F = "ss", PV = Offset.Create(0, 0, 7, 0)  },
            new OffsetData(5, 6, 7, 8) { C = EnUs, S = "5", F = "%H", PV = Offset.Create(5, 0, 0, 0)  },
            new OffsetData(5, 6, 7, 8) { C = EnUs, S = "6", F = "%m", PV = Offset.Create(0, 6, 0, 0)  },
            new OffsetData(5, 6, 7, 8) { C = EnUs, S = "7", F = "%s", PV = Offset.Create(0, 0, 7, 0)  },
            new OffsetData(Offset.MaxValue) { C = EnUs, S = "+23:59:59.999", F = "g" },
            new OffsetData(Offset.MaxValue) { C = EnUs, S = "23", F = "%H", PV = Offset.Create(23, 0, 0, 0)  },
            new OffsetData(Offset.MaxValue) { C = EnUs, S = "59", F = "%m", PV = Offset.Create(0, 59, 0, 0)  },
            new OffsetData(Offset.MaxValue) { C = EnUs, S = "59", F = "%s", PV = Offset.Create(0, 0, 59, 0)  },
            new OffsetData(Offset.MaxValue) { C = EnUs, S = "m", F = "\\m", PV = Offset.Create(0, 0, 0, 0) },
            new OffsetData(Offset.MaxValue) { C = EnUs, S = "m", F = "'m'", PV = Offset.Create(0, 0, 0, 0) },
            new OffsetData(Offset.MaxValue) { C = EnUs, S = "mmmmmmmmmm", F = "'mmmmmmmmmm'", PV = Offset.Create(0, 0, 0, 0) },
            new OffsetData(Offset.MaxValue) { C = EnUs, S = "z", F = "%z", PV = Offset.Create(0, 0, 0, 0) },
            new OffsetData(Offset.MaxValue) { C = EnUs, S = "zqw", F = "zqw", PV = Offset.Create(0, 0, 0, 0) },
            new OffsetData(Offset.MinValue) { C = EnUs, S = "-23:59:59.999", F = "g" },
            new OffsetData(Offset.Zero) { C = EnUs, S = "+0", F = "s"  },
            new OffsetData(Offset.Zero) { C = EnUs, S = "+0:00", F = "m"  },
            new OffsetData(Offset.Zero) { C = EnUs, S = "+0:00:00", F = "l"  },
            new OffsetData(Offset.Zero) { C = EnUs, S = "+0:00:00.000", F = "f"  },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "!", Exception=typeof(FormatException), Message = Resources.Parse_UnknownStandardFormat, Parameters = {'!', typeof(Offset).FullName}},
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "%", Exception=typeof(FormatException), Message = Resources.Parse_UnknownStandardFormat, Parameters = { '%', typeof(Offset).FullName } },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "%%", Exception=typeof(FormatException), Message = Resources.Parse_PercentDoubled },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "%\\", Exception=typeof(FormatException), Message = Resources.Parse_EscapeAtEndOfString },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "\\", Exception=typeof(FormatException), Message = Resources.Parse_UnknownStandardFormat, Parameters = { '\\', typeof(Offset).FullName } },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "ffff", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_RepeatCountExceeded, Parameters = { 'f', 3 } },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "H%", Exception=typeof(FormatException), Message = Resources.Parse_PercentAtEndOfString },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "hh", Exception=typeof(FormatException), Message = Resources.Parse_Hour12PatternNotSupported, Parameters = { typeof(Offset).FullName } },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "HHH", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_RepeatCountExceeded, Parameters = { 'H', 2 } },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "mmm", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_RepeatCountExceeded, Parameters = { 'm', 2 } },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "mmmmmmmmmmmmmmmmmmm", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_RepeatCountExceeded, Parameters = { 'm', 2 } },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "'qwe", Exception=typeof(FormatException), Message = Resources.Parse_MissingEndQuote, Parameters = { '\'' } },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "'qwe\\", Exception=typeof(FormatException), Message = Resources.Parse_EscapeAtEndOfString },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "'qwe\\'", Exception=typeof(FormatException), Message = Resources.Parse_MissingEndQuote, Parameters = { '\'' } },
            new OffsetData(Offset.Zero) { C = EnUs, S = "123", F = "sss", Exception=typeof(FormatError.FormatValueException), Message = Resources.Parse_RepeatCountExceeded, Parameters = { 's', 2 } },
            new OffsetData(5, 0, 0, 0) { C = FrFr, S = "+5", F = "g" },
            new OffsetData(5, 12, 0, 0) { C = FrFr, S = "+5:12", F = "g" },
            new OffsetData(5, 12, 34, 0) { C = FrFr, S = "+5:12:34", F = "g" },
            new OffsetData(5, 12, 34, 567) { C = FrFr, S = "+5:12:34,567", F = "g" },
            new OffsetData(5, 12, 34, 567) { C = FrFr, S = "18" + Nbsp + "754" + Nbsp + "567", F = "n" },
            new OffsetData(Offset.MaxValue) { C = FrFr, S = "+23:59:59,999", F = "g" },
            new OffsetData(Offset.MinValue) { C = FrFr, S = "-23:59:59,999", F = "g" },
            new OffsetData(5, 0, 0, 0) { C = ItIt, S = "+5", F = "g" },
            new OffsetData(5, 12, 0, 0) { C = ItIt, S = "+5.12", F = "g" },
            new OffsetData(5, 12, 34, 0) { C = ItIt, S = "+5.12.34", F = "g" },
            new OffsetData(5, 12, 34, 567) { C = ItIt, S = "+5.12.34,567", F = "g" },
            new OffsetData(5, 12, 34, 567) { C = ItIt, S = "18.754.567", F = "n" },
            new OffsetData(Offset.MaxValue) { C = ItIt, S = "+23.59.59,999", F = "g" },
            new OffsetData(Offset.MinValue) { C = ItIt, S = "-23.59.59,999", F = "g" },
            new OffsetData(Offset.MaxValue) { C = null, S = "+23:59:59.999", F = "g", ThreadCulture = EnUs },
            new OffsetData(Offset.MinValue) { C = null, S = "-23:59:59.999", F = "g", ThreadCulture = EnUs },
            new OffsetData(Offset.MaxValue) { C = EnUs, S = "+23:59:59.999", F = "g", ThreadCulture = ItIt },
            new OffsetData(Offset.MinValue) { C = EnUs, S = "-23:59:59.999", F = "g", ThreadCulture = ItIt },
        };

        /// <summary>
        ///   Base for building filtered lists of parsing test data. This is here because we do not have access
        ///   to LINQ.
        /// </summary>
        /// <param name="test">The test predicate.</param>
        /// <returns>An <see cref="IEnumerable{OffsetData}" /></returns>
        internal static IEnumerable<OffsetData> FilteredParseTests(Predicate<OffsetData> test)
        {
            foreach (var data in OffsetParseData)
            {
                if (test(data))
                {
                    yield return data;
                }
            }
            foreach (var data in OffsetFormattingCommonData)
            {
                if (test(data))
                {
                    yield return data;
                }
            }
        }

        /// <summary>
        ///   Base for building filtered lists of formatting test data. This is here because we do not have access
        ///   to LINQ.
        /// </summary>
        /// <param name="test">The test predicate.</param>
        /// <returns>An <see cref="IEnumerable{OffsetData}" /></returns>
        internal static IEnumerable<OffsetData> FilteredFormatTests(Predicate<OffsetData> test)
        {
            foreach (var data in OffsetFormatData)
            {
                if (test(data))
                {
                    yield return data;
                }
            }
            foreach (var data in OffsetFormattingCommonData)
            {
                if (test(data))
                {
                    yield return data;
                }
            }
        }

        /// <summary>
        ///   Returns an iterator of test data with the parse style specified.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{OffsetData}" /></returns>
        internal static IEnumerable<OffsetData> ParseWithStyles()
        {
            return FilteredParseTests(data => data.Styles != DateTimeParseStyles.None);
        }

        /// <summary>
        ///   Returns an iterator of test data with no parse style specified.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{OffsetData}" /></returns>
        internal static IEnumerable<OffsetData> ParseWithoutStyles()
        {
            return FilteredParseTests(data => data.Styles == DateTimeParseStyles.None);
        }

        /// <summary>
        ///   Returns an iterator of test data with no format string specified.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{OffsetData}" /></returns>
        internal static IEnumerable<OffsetData> FormatWithoutFormat()
        {
            return FilteredFormatTests(data => data.F == null);
        }

        #region Nested type: OffsetData
        /// <summary>
        ///   A container for test data for formatting and parsing <see cref="Offset" /> objects.
        /// </summary>
        public sealed class OffsetData : AbstractFormattingData<Offset>
        {
            /// <summary>
            ///   Initializes a new instance of the <see cref="OffsetData" /> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public OffsetData(Offset value)
                : base(value)
            {
            }

            /// <summary>
            ///   Initializes a new instance of the <see cref="OffsetData" /> class.
            /// </summary>
            /// <param name="hours">The hours.</param>
            /// <param name="minutes">The minutes.</param>
            /// <param name="seconds">The seconds.</param>
            /// <param name="fractions">The fractions.</param>
            public OffsetData(int hours, int minutes, int seconds, int fractions)
                : this(Offset.Create(hours, minutes, seconds, fractions))
            {
            }

            /// <summary>
            ///   Returns a string representation of the given value. This will usually not call the ToString()
            ///   method as that is problably being tested. The returned string is only used in test code and
            ///   labels so it doesn't have to be beautiful. Must handle <c>null</c> if the type is a reference
            ///   type. This should not throw an exception.
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

        public static string Resource { get; set; }
    }
}