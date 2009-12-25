#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
using NUnit.Framework;
using NodaTime.Format;
using System.Globalization;
using NodaTime.Periods;

namespace NodaTime.Test.Format
{
    [TestFixture]
    public partial class PeriodFormatterBuilderTest
    {
        PeriodFormatterBuilder builder;
        Period zeroPeriod;
        Period StandardPeriod;
        Period timePeriod;
        Period datePeriod;
        Period emptyYearDayTimePeriod;
        Period fullYearDayTimePeriod;

        [SetUp]
        public void Init()
        {
            builder = new PeriodFormatterBuilder();
            zeroPeriod = new Period(0, 0, 0, 0, 0, 0, 0, 0);
            StandardPeriod = new Period(1, 2, 3, 4, 5, 6, 7, 8);
            timePeriod = new Period(0, 0, 0, 0, 5, 6, 7, 8);
            datePeriod = new Period(1, 2, 3, 4, 0, 0, 0, 0);
            emptyYearDayTimePeriod = new Period(0, 0, 0, 0, 0, 0, 0, 0, PeriodType.YearDayTime);
            fullYearDayTimePeriod = new Period(1, 0, 0, 4, 5, 6, 7, 8, PeriodType.YearDayTime);
        }

        [Test]
        public void AppendLiteral_BuildsCorrectPrinter_ForStandardPeriod()
        {
            var formatter = builder
                .AppendLiteral("HELLO")
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("HELLO", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(0, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendFormatter_BuildsCorrectPrinter_ForStandardPeriod()
        {
            var baseFormatter = builder
                .AppendYears()
                .AppendLiteral("-")
                .ToFormatter();

            var formatter = new PeriodFormatterBuilder()
                .Append(baseFormatter)
                .AppendYears()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("1-1", printedValue);
            Assert.AreEqual(3, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(2, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        #region AppendPrinterParser

        [Test]
        public void AppendPrinterParser_Throws_IfBothNull()
        {
            Assert.Throws<ArgumentException>(()=>builder.Append(null, null));
        }

        [Test]
        public void AppendPrinterParser_ReturnsOnlyPrinter_IfParserNull()
        {
            var printer = builder
                .AppendYears()
                .AppendLiteral("-")
                .ToPrinter();
            
            var builder2 = new PeriodFormatterBuilder()
                .Append(printer, null)
                .AppendMonths();

            Assert.NotNull(builder2.ToPrinter());
            Assert.IsNull(builder2.ToParser());

            var formatter = builder2.ToFormatter();

            Assert.AreEqual("1-2", formatter.Print(StandardPeriod));
            Assert.Throws<NotSupportedException>(()=>formatter.Parse("1-3"));
        }

        #endregion
    }
}
