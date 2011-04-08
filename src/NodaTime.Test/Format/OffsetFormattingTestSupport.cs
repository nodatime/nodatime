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
using System.Globalization;
using NodaTime.Format;
using NUnit.Framework;
#endregion

namespace NodaTime.Test.Format
{
    public class OffsetFormattingTestSupport : FormattingTestSupport
    {
        protected object[] FormatDataNoFormat = {
            new OffsetData { C = null, V = Offset.MaxValue, S = "+23:59:59.999", ThreadCulture = EnUs },
            new OffsetData { C = null, V = Offset.MinValue, S = "-23:59:59.999", ThreadCulture = EnUs },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "+23:59:59.999" },
            new OffsetData { C = EnUs, V = Offset.MinValue, S = "-23:59:59.999" },
            new OffsetData { C = EnUs, V = Offset.Create(5, 12, 34, 567), S = "+5:12:34.567" },
            new OffsetData { C = EnUs, V = Offset.Create(5, 12, 34, 0), S = "+5:12:34" },
            new OffsetData { C = EnUs, V = Offset.Create(5, 12, 0, 0), S = "+5:12" },
            new OffsetData { C = EnUs, V = Offset.Create(5, 0, 0, 0), S = "+5" },
            new OffsetData { C = FrFr, V = Offset.MaxValue, S = "+23:59:59,999" },
            new OffsetData { C = FrFr, V = Offset.MinValue, S = "-23:59:59,999" },
            new OffsetData { C = FrFr, V = Offset.Create(5, 12, 34, 567), S = "+5:12:34,567" },
            new OffsetData { C = FrFr, V = Offset.Create(5, 12, 34, 0), S = "+5:12:34" },
            new OffsetData { C = FrFr, V = Offset.Create(5, 12, 0, 0), S = "+5:12" },
            new OffsetData { C = FrFr, V = Offset.Create(5, 0, 0, 0), S = "+5" },
            new OffsetData { C = ItIt, V = Offset.MaxValue, S = "+23.59.59,999" },
            new OffsetData { C = ItIt, V = Offset.MinValue, S = "-23.59.59,999" },
            new OffsetData { C = ItIt, V = Offset.Create(5, 12, 34, 567), S = "+5.12.34,567" },
            new OffsetData { C = ItIt, V = Offset.Create(5, 12, 34, 0), S = "+5.12.34" },
            new OffsetData { C = ItIt, V = Offset.Create(5, 12, 0, 0), S = "+5.12" },
            new OffsetData { C = ItIt, V = Offset.Create(5, 0, 0, 0), S = "+5" },
        };

        protected object[] FormatDataWithFormat = {
            new OffsetData { C = null, V = Offset.MaxValue, S = "+23:59:59.999", F = "G", ThreadCulture = EnUs },
            new OffsetData { C = null, V = Offset.MinValue, S = "-23:59:59.999", F = "G", ThreadCulture = EnUs },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "+23:59:59.999", F = "" },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "+23:59:59.999", F = "G" },
            new OffsetData { C = EnUs, V = Offset.MinValue, S = "-23:59:59.999", F = "G" },
            new OffsetData { C = EnUs, V = Offset.Create(5, 12, 34, 567), S = "+5:12:34.567", F = "G"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 12, 34, 0), S = "+5:12:34", F = "G"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 12, 0, 0), S = "+5:12", F = "G"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 0, 0, 0), S = "+5", F = "G"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 12, 34, 567), S = "18,754,567", F = "N"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 12, 34, 567), S = "+5:12:34.567", F = "F"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 12, 34, 567), S = "+5:12:34", F = "L"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 12, 34, 567), S = "+5:12", F = "M"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 12, 34, 567), S = "+5", F = "S"  },
            new OffsetData { C = EnUs, V = Offset.Zero, S = "+0:00:00.000", F = "F"  },
            new OffsetData { C = EnUs, V = Offset.Zero, S = "+0:00:00", F = "L"  },
            new OffsetData { C = EnUs, V = Offset.Zero, S = "+0:00", F = "M"  },
            new OffsetData { C = EnUs, V = Offset.Zero, S = "+0", F = "S"  },
            new OffsetData { C = EnUs, V = Offset.Create(3, 0, 0, 0), S = "+", F = "%+"  },
            new OffsetData { C = EnUs, V = Offset.Create(3, 0, 0, 0), S = "", F = "%-"  },
            new OffsetData { C = EnUs, V = Offset.Create(-3, 0, 0, 0), S = "-", F = "%+"  },
            new OffsetData { C = EnUs, V = Offset.Create(-3, 0, 0, 0), S = "-", F = "%-"  },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "m", F = "\\m" },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "m", F = "'m'" },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "mmmmmmmmmm", F = "'mmmmmmmmmm'" },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "zqw", F = "zqw" },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "z", F = "%z" },
            new OffsetData { C = EnUs, V = Offset.Create(5, 6, 7, 8), S = "5", F = "%H"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 6, 7, 8), S = "05", F = "HH"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 6, 7, 8), S = "6", F = "%m"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 6, 7, 8), S = "06", F = "mm"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 6, 7, 8), S = "7", F = "%s"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 6, 7, 8), S = "07", F = "ss"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 6, 7, 8), S = "0", F = "%f"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 6, 7, 8), S = "00", F = "ff"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 6, 7, 8), S = "008", F = "fff"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 6, 7, 8), S = "", F = "%F"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 6, 7, 8), S = "", F = "FF"  },
            new OffsetData { C = EnUs, V = Offset.Create(5, 6, 7, 8), S = "008", F = "FFF"  },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "23", F = "%H"  },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "59", F = "%m"  },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "59", F = "%s"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 400), S = "4", F = "%f"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 400), S = "40", F = "ff"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 400), S = "400", F = "fff"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 400), S = "4", F = "%F"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 400), S = "4", F = "FF"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 400), S = "4", F = "FFF"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 450), S = "4", F = "%f"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 450), S = "45", F = "ff"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 450), S = "450", F = "fff"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 450), S = "4", F = "%F"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 450), S = "45", F = "FF"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 450), S = "45", F = "FFF"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 456), S = "4", F = "%f"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 456), S = "45", F = "ff"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 456), S = "456", F = "fff"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 456), S = "4", F = "%F"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 456), S = "45", F = "FF"  },
            new OffsetData { C = EnUs, V = Offset.Create(1, 1, 1, 456), S = "456", F = "FFF"  },
            new OffsetData { C = FrFr, V = Offset.MaxValue, S = "+23:59:59,999", F = "G" },
            new OffsetData { C = FrFr, V = Offset.MinValue, S = "-23:59:59,999", F = "G" },
            new OffsetData { C = FrFr, V = Offset.Create(5, 12, 34, 567), S = "18" + Nbsp + "754" + Nbsp + "567", F = "N" },
            new OffsetData { C = ItIt, V = Offset.MaxValue, S = "+23.59.59,999", F = "G" },
            new OffsetData { C = ItIt, V = Offset.MinValue, S = "-23.59.59,999", F = "G" },
            new OffsetData { C = ItIt, V = Offset.Create(5, 12, 34, 567), S = "18.754.567", F = "N" },
            new OffsetData { C = EnUs, V = Offset.Zero, F = "z", Kind = ParseFailureKind.ParseUnknownStandardFormat, Parameters = { '!', typeof(Offset).FullName } },
            new OffsetData { C = EnUs, V = Offset.Zero, F = "\\", Kind = ParseFailureKind.ParseUnknownStandardFormat, Parameters = { '\\', typeof(Offset).FullName } },
            new OffsetData { C = EnUs, V = Offset.Zero, F = "%", Kind = ParseFailureKind.ParseUnknownStandardFormat, Parameters = { '%', typeof(Offset).FullName } },
            new OffsetData { C = EnUs, V = Offset.Zero, F = "%%", Kind = ParseFailureKind.ParsePercentDoubled },
            new OffsetData { C = EnUs, V = Offset.Zero, F = "'", Kind = ParseFailureKind.ParseUnknownStandardFormat, Parameters = { '\'', typeof(Offset).FullName } },
            new OffsetData { C = EnUs, V = Offset.Zero, F = "'qwe", Kind = ParseFailureKind.ParseMissingEndQuote, Parameters = { '\'' } },
            new OffsetData { C = EnUs, V = Offset.Zero, F = "'qwe\\'", Kind = ParseFailureKind.ParseMissingEndQuote, Parameters = { '\'' } },
            new OffsetData { C = EnUs, V = Offset.Zero, F = "'qwe\\", Kind = ParseFailureKind.ParseEscapeAtEndOfString },
            new OffsetData { C = EnUs, V = Offset.Zero, F = "ffff", Kind = ParseFailureKind.ParseRepeatCountExceeded, Parameters = { 'f', 3 } },
            new OffsetData { C = EnUs, V = Offset.Zero, F = "HHH", Kind = ParseFailureKind.ParseRepeatCountExceeded, Parameters = { 'f', 3 } },
            new OffsetData { C = EnUs, V = Offset.Zero, F = "mmm", Kind = ParseFailureKind.ParseRepeatCountExceeded, Parameters = { 'f', 3 } },
            new OffsetData { C = EnUs, V = Offset.Zero, F = "sss", Kind = ParseFailureKind.ParseRepeatCountExceeded, Parameters = { 'f', 3 } },
            new OffsetData { C = EnUs, V = Offset.Zero, F = "mmmmmmmmmmmmmmmmmmm", Kind = ParseFailureKind.ParseRepeatCountExceeded, Parameters = { 'f', 3 } },
            new OffsetData { C = EnUs, V = Offset.Zero, F = "hh", Kind = ParseFailureKind.Parse12HourPatternNotSupported },
        };

        protected object[] ParseExactCommon = {
            new OffsetData { C = EnUs, V = Offset.Create(2, 0, 0, 0), S = "2", F = "%H" },
            new OffsetData { C = EnUs, V = Offset.Create(2, 0, 0, 0), S = "2", F = "%H" },
            new OffsetData { C = EnUs, V = Offset.Create(12, 0, 0, 0), S = "12", F = "%H" },
            new OffsetData { C = EnUs, V = Offset.Create(12, 0, 0, 0), S = "12", F = "HH" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 2, 0, 0), S = "2", F = "%m" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 12, 0, 0), S = "12", F = "%m" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 12, 0, 0), S = "12", F = "mm" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 2, 0), S = "2", F = "%s" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 12, 0), S = "12", F = "%s" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 12, 0), S = "12", F = "ss" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 100), S = "1", F = "%f" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 120), S = "12", F = "ff" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 123), S = "123", F = "fff" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 100), S = "1", F = "%F" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 100), S = "1", F = "FF" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 120), S = "12", F = "FF" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 100), S = "1", F = "FFF" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 120), S = "12", F = "FFF" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 123), S = "123", F = "FFF" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 0), S = ":", F = "%:",  Name = "date separator" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 0), S = ".", F = "%.",  Name = "decimal separator" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 600), S = ".6", F = ".f" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 678), S = ".678", F = ".fff" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 600), S = ".6", F = ".F" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 600), S = ".6", F = ".FFF",  Name = "elided zeros" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 678), S = ".678", F = ".FFF" },
            new OffsetData { C = EnUs, V = Offset.Create(1, 0, 0, 0), S = "1", F = "H.FFF",  Name = "missing fraction" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 0), S = "H", F = "\\H" },
            new OffsetData { C = EnUs, V = Offset.Create(0, 0, 0, 0), S = "HHss", F = "'HHss'" },
            // *************************************
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = LeadingSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "  12:34", F = "HH:mm", Styles = LeadingSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "  HH:mm", Styles = LeadingSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "'  'HH:mm", Styles = LeadingSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "  '  'HH:mm", Styles = LeadingSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "  12:34", F = "  '  'HH:mm", Styles = LeadingSpace },
            // *************************************
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = TrailingSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34  ", F = "HH:mm", Styles = TrailingSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH:mm  ", Styles = TrailingSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH:mm'  '", Styles = TrailingSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH:mm'  '  ", Styles = TrailingSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34  ", F = "HH:mm'  '  ", Styles = TrailingSpace },
            // *************************************
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = SurroundingSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "  12:34  ", F = "HH:mm", Styles = SurroundingSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "  HH:mm  ", Styles = SurroundingSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "'  'HH:mm'  '", Styles = SurroundingSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "  '  'HH:mm'  '  ", Styles = SurroundingSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "  12:34  ", F = "  '  'HH:mm'  '  ", Styles = SurroundingSpace },
            // *************************************
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = InnerSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = " 12:34", F = "HH:mm", Styles = InnerSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12  :34", F = "HH:mm", Styles = InnerSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = " HH:mm", Styles = InnerSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH  :mm", Styles = InnerSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = " 12:34", F = "HH :mm", Styles = InnerSpace },
            // *************************************
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "  12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "  HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "'  'HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "  '  'HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "  12:34", F = "  '  'HH:mm", Styles = AllSpace },
            // *************************************
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34  ", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH:mm  ", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH:mm'  '", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH:mm'  '  ", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34  ", F = "HH:mm'  '  ", Styles = AllSpace },
            // *************************************
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "  12:34  ", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "  HH:mm  ", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "'  'HH:mm'  '", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "  '  'HH:mm'  '  ", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "  12:34  ", F = "  '  'HH:mm'  '  ", Styles = AllSpace },
            // *************************************
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = " 12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12  :34", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = " HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = "12:34", F = "HH  :mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Offset.Create(12, 34, 0, 0), S = " 12:34", F = "HH :mm", Styles = AllSpace },
            // *************************************
            new OffsetData { C = EnUs, S = "", F = "g", Kind = ParseFailureKind.ParseValueStringEmpty },
            new OffsetData { C = EnUs, S = null, F = "g", Kind = ParseFailureKind.ArgumentNull, ArgumentName = "value" },
        };

        protected object[] ParseExactMultiple = {
            new OffsetData { V = Offset.Create(0, 1, 23, 0), C = EnUs, S = "1:23", F = "HH:mm\0m:ss"},
            new OffsetData { C = EnUs, S = "123", F = "", Kind = ParseFailureKind.ParseFormatElementInvalid },
            new OffsetData { C = EnUs, S = "123", F = null, Kind = ParseFailureKind.ArgumentNull, ArgumentName = "formats" },
        };

        protected object[] ParseExactSingle = {
            new OffsetData { C = EnUs, S = "123", F = "!", Kind = ParseFailureKind.ParseUnknownStandardFormat, Parameters = {'!', typeof(Offset).FullName}},
            new OffsetData { C = EnUs, S = "2:", F = "%H", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {":"}},
            new OffsetData { C = EnUs, S = "123", F = "H%", Kind = ParseFailureKind.ParsePercentAtEndOfString },
            new OffsetData { C = EnUs, S = "123", F = "%%H", Kind = ParseFailureKind.ParsePercentDoubled },
            new OffsetData { C = EnUs, S = "axc", F = "'abc'", Kind = ParseFailureKind.ParseQuotedStringMismatch },
            new OffsetData { C = EnUs, S = "axc", F = "%\\", Kind = ParseFailureKind.ParseEscapeAtEndOfString },
            new OffsetData { C = EnUs, S = "a", F = "\\'", Kind = ParseFailureKind.ParseEscapedCharacterMismatch, Parameters = {'\''} },
            new OffsetData { C = EnUs, S = "a", F = "%.", Kind = ParseFailureKind.ParseMissingDecimalSeparator },
            new OffsetData { C = EnUs, S = "a", F = ".H", Kind = ParseFailureKind.ParseMissingDecimalSeparator },
            new OffsetData { C = EnUs, S = "a", F = "%:", Kind = ParseFailureKind.ParseTimeSeparatorMismatch },
            new OffsetData { C = EnUs, S = "a", F = "%H", Kind = ParseFailureKind.ParseMismatchedNumber, Parameters = {"H"} },
            new OffsetData { C = EnUs, S = "1", F = "HH", Kind = ParseFailureKind.ParseMismatchedNumber, Parameters = {"HH"} },
            new OffsetData { C = EnUs, S = "a", F = "%m", Kind = ParseFailureKind.ParseMismatchedNumber, Parameters = {"m"} },
            new OffsetData { C = EnUs, S = "1", F = "mm", Kind = ParseFailureKind.ParseMismatchedNumber, Parameters = {"mm"} },
            new OffsetData { C = EnUs, S = "a", F = "%s", Kind = ParseFailureKind.ParseMismatchedNumber, Parameters = {"s"} },
            new OffsetData { C = EnUs, S = "1", F = "ss", Kind = ParseFailureKind.ParseMismatchedNumber, Parameters = {"ss"} },

            new OffsetData { C = EnUs, S = "12", F = "%f", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {"2"} },
            new OffsetData { C = EnUs, S = "123", F = "ff", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {"3"} },
            new OffsetData { C = EnUs, S = "1234", F = "fff", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {"4"} },
            new OffsetData { C = EnUs, S = "12", F = "%F", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {"2"} },
            new OffsetData { C = EnUs, S = "123", F = "FF", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {"3"} },
            new OffsetData { C = EnUs, S = "1234", F = "FFF", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {"4"} },

            new OffsetData { C = EnUs, S = "1a", F = "H ", Kind = ParseFailureKind.ParseMismatchedSpace },
            new OffsetData { C = EnUs, S = "z", F = "%y", Kind = ParseFailureKind.ParseMismatchedCharacter, Parameters = {'y'} },
            new OffsetData { C = EnUs, S = "12:34 ", F = "HH:mm", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {" "} },
            new OffsetData { C = EnUs, S = "123", F = "", Kind = ParseFailureKind.ParseFormatStringEmpty },
            new OffsetData { C = EnUs, S = "123", F = null, Kind = ParseFailureKind.ArgumentNull, ArgumentName = "format" },
        };

        #region Nested type: OffsetData
        public sealed class OffsetData : AbstractFormattingData<Offset>
        {
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