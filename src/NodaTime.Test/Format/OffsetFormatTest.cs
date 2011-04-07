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
using NodaTime.Globalization;
using NUnit.Framework;
#endregion

namespace NodaTime.Test.Format
{
    [TestFixture]
    [Category("Formating")]
    [Category("Format")]
    public class OffsetFormatTest
    {
        private static readonly CultureInfo EnUs = new CultureInfo("en-US");
        private static readonly CultureInfo FrFr = new CultureInfo("fr-FR");
        private static readonly CultureInfo ItIt = new CultureInfo("it-IT");

        private const DateTimeParseStyles LeadingSpace = DateTimeParseStyles.AllowLeadingWhite;
        private const DateTimeParseStyles TrailingSpace = DateTimeParseStyles.AllowTrailingWhite;
        private const DateTimeParseStyles InnerSpace = DateTimeParseStyles.AllowInnerWhite;
        private const DateTimeParseStyles SurroundingSpace = DateTimeParseStyles.AllowLeadingWhite | DateTimeParseStyles.AllowTrailingWhite;
        private const DateTimeParseStyles AllSpace = DateTimeParseStyles.AllowWhiteSpaces;

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
                return string.Format("Make({0}{1}, {2}, {3}, {4})",
                    value.IsNegative ? "-" : "",
                    value.Hours,
                    value.Minutes,
                    value.Seconds,
                    value.FractionalSeconds);
            }
        }

        private static Offset Make(int hours, int minutes, int seconds, int fractional)
        {
            return Offset.Create(hours, minutes, seconds, fractional);
        }

        private object[] formatData = {
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "+23:59:59.999" },
            new OffsetData { C = EnUs, V = Offset.MinValue, S = "-23:59:59.999" },
            new OffsetData { C = EnUs, V = Make(5, 12, 34, 567), S = "+5:12:34.567" },
            new OffsetData { C = EnUs, V = Make(5, 12, 34, 0), S = "+5:12:34" },
            new OffsetData { C = EnUs, V = Make(5, 12, 0, 0), S = "+5:12" },
            new OffsetData { C = EnUs, V = Make(5, 0, 0, 0), S = "+5" },
            new OffsetData { C = FrFr, V = Offset.MaxValue, S = "+23:59:59,999" },
            new OffsetData { C = FrFr, V = Offset.MinValue, S = "-23:59:59,999" },
            new OffsetData { C = FrFr, V = Make(5, 12, 34, 567), S = "+5:12:34,567" },
            new OffsetData { C = FrFr, V = Make(5, 12, 34, 0), S = "+5:12:34" },
            new OffsetData { C = FrFr, V = Make(5, 12, 0, 0), S = "+5:12" },
            new OffsetData { C = FrFr, V = Make(5, 0, 0, 0), S = "+5" },
            new OffsetData { C = ItIt, V = Offset.MaxValue, S = "+23.59.59,999" },
            new OffsetData { C = ItIt, V = Offset.MinValue, S = "-23.59.59,999" },
            new OffsetData { C = ItIt, V = Make(5, 12, 34, 567), S = "+5.12.34,567" },
            new OffsetData { C = ItIt, V = Make(5, 12, 34, 0), S = "+5.12.34" },
            new OffsetData { C = ItIt, V = Make(5, 12, 0, 0), S = "+5.12" },
            new OffsetData { C = ItIt, V = Make(5, 0, 0, 0), S = "+5" },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "+23:59:59.999", F = "" },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "+23:59:59.999", F = "G" },
            new OffsetData { C = EnUs, V = Offset.MinValue, S = "-23:59:59.999", F = "G" },
            new OffsetData { C = EnUs, V = Make(5, 12, 34, 567), S = "+5:12:34.567", F = "G"  },
            new OffsetData { C = EnUs, V = Make(5, 12, 34, 0), S = "+5:12:34", F = "G"  },
            new OffsetData { C = EnUs, V = Make(5, 12, 0, 0), S = "+5:12", F = "G"  },
            new OffsetData { C = EnUs, V = Make(5, 0, 0, 0), S = "+5", F = "G"  },
            new OffsetData { C = EnUs, V = Make(5, 12, 34, 567), S = "18,754,567", F = "N"  },
            new OffsetData { C = EnUs, V = Make(5, 12, 34, 567), S = "+5:12:34.567", F = "F"  },
            new OffsetData { C = EnUs, V = Make(5, 12, 34, 567), S = "+5:12:34", F = "L"  },
            new OffsetData { C = EnUs, V = Make(5, 12, 34, 567), S = "+5:12", F = "M"  },
            new OffsetData { C = EnUs, V = Make(5, 12, 34, 567), S = "+5", F = "S"  },
            new OffsetData { C = EnUs, V = Offset.Zero, S = "+0:00:00.000", F = "F"  },
            new OffsetData { C = EnUs, V = Offset.Zero, S = "+0:00:00", F = "L"  },
            new OffsetData { C = EnUs, V = Offset.Zero, S = "+0:00", F = "M"  },
            new OffsetData { C = EnUs, V = Offset.Zero, S = "+0", F = "S"  },
            new OffsetData { C = EnUs, V = Make(3, 0, 0, 0), S = "+", F = "%+"  },
            new OffsetData { C = EnUs, V = Make(3, 0, 0, 0), S = "", F = "%-"  },
            new OffsetData { C = EnUs, V = Make(-3, 0, 0, 0), S = "-", F = "%+"  },
            new OffsetData { C = EnUs, V = Make(-3, 0, 0, 0), S = "-", F = "%-"  },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "m", F = "\\m" },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "m", F = "'m'" },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "mmmmmmmmmm", F = "'mmmmmmmmmm'" },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "zqw", F = "zqw" },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "z", F = "%z" },
            new OffsetData { C = EnUs, V = Make(5, 6, 7, 8), S = "5", F = "%H"  },
            new OffsetData { C = EnUs, V = Make(5, 6, 7, 8), S = "05", F = "HH"  },
            new OffsetData { C = EnUs, V = Make(5, 6, 7, 8), S = "6", F = "%m"  },
            new OffsetData { C = EnUs, V = Make(5, 6, 7, 8), S = "06", F = "mm"  },
            new OffsetData { C = EnUs, V = Make(5, 6, 7, 8), S = "7", F = "%s"  },
            new OffsetData { C = EnUs, V = Make(5, 6, 7, 8), S = "07", F = "ss"  },
            new OffsetData { C = EnUs, V = Make(5, 6, 7, 8), S = "0", F = "%f"  },
            new OffsetData { C = EnUs, V = Make(5, 6, 7, 8), S = "00", F = "ff"  },
            new OffsetData { C = EnUs, V = Make(5, 6, 7, 8), S = "008", F = "fff"  },
            new OffsetData { C = EnUs, V = Make(5, 6, 7, 8), S = "", F = "%F"  },
            new OffsetData { C = EnUs, V = Make(5, 6, 7, 8), S = "", F = "FF"  },
            new OffsetData { C = EnUs, V = Make(5, 6, 7, 8), S = "008", F = "FFF"  },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "23", F = "%H"  },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "59", F = "%m"  },
            new OffsetData { C = EnUs, V = Offset.MaxValue, S = "59", F = "%s"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 400), S = "4", F = "%f"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 400), S = "40", F = "ff"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 400), S = "400", F = "fff"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 400), S = "4", F = "%F"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 400), S = "4", F = "FF"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 400), S = "4", F = "FFF"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 450), S = "4", F = "%f"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 450), S = "45", F = "ff"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 450), S = "450", F = "fff"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 450), S = "4", F = "%F"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 450), S = "45", F = "FF"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 450), S = "45", F = "FFF"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 456), S = "4", F = "%f"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 456), S = "45", F = "ff"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 456), S = "456", F = "fff"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 456), S = "4", F = "%F"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 456), S = "45", F = "FF"  },
            new OffsetData { C = EnUs, V = Make(1, 1, 1, 456), S = "456", F = "FFF"  },
            new OffsetData { C = FrFr, V = Offset.MaxValue, S = "+23:59:59,999", F = "G" },
            new OffsetData { C = FrFr, V = Offset.MinValue, S = "-23:59:59,999", F = "G" },
            new OffsetData { C = FrFr, V = Make(5, 12, 34, 567), S = "18" + Nbsp + "754" + Nbsp + "567", F = "N" },
            new OffsetData { C = ItIt, V = Offset.MaxValue, S = "+23.59.59,999", F = "G" },
            new OffsetData { C = ItIt, V = Offset.MinValue, S = "-23.59.59,999", F = "G" },
            new OffsetData { C = ItIt, V = Make(5, 12, 34, 567), S = "18.754.567", F = "N" },
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
        private const string Nbsp = "\u00a0";

        /*
        */

        [Test]
        public void TestJlk()
        {
            TestFormat(new OffsetData { C = EnUs, V = Offset.Zero, F = "ffff", Kind = ParseFailureKind.ParseRepeatCountExceeded });
        }

        [Test]
        [TestCaseSource("formatData")]
        public void TestFormat(OffsetData data)
        {
            using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
            {
                var formatInfo = new NodaFormatInfo(data.C);
                bool isSuccess = data.Kind == ParseFailureKind.None;
                if (isSuccess)
                {
                    string actual = OffsetFormat.Format(data.V, data.F, formatInfo);
                    Assert.AreEqual(data.S, actual);
                }
                else
                {
                    TestDelegate test = () => OffsetFormat.Format(data.V, data.F, formatInfo);
                    if (data.Kind == ParseFailureKind.ArgumentNull)
                    {
                        Assert.Throws<ArgumentNullException>(test);
                    }
                    else
                    {
                        Assert.Throws(Is.TypeOf<ParseException>().And.Property("Kind").EqualTo(data.Kind), test);
                    }
                }
            }
        }
    }
}