#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
using NodaTime.Format;
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    [TestFixture]
    public partial class PeriodFormatterBuilderTest
    {
        private PeriodFormatterBuilder builder;

        private Period standardPeriodEmpty;
        private Period standardPeriodFull;

        private Period timePeriod;
        private Period datePeriod;

        private Period yearDayTimePeriodEmpty;
        private Period yearDayTimePeriodFull;

        [SetUp]
        public void Init()
        {
            builder = new PeriodFormatterBuilder();

            standardPeriodEmpty = new Period(0, 0, 0, 0, 0, 0, 0, 0);
            standardPeriodFull = new Period(1, 2, 3, 4, 5, 6, 7, 8);

            timePeriod = new Period(0, 0, 0, 0, 5, 6, 7, 8);
            datePeriod = new Period(1, 2, 3, 4, 0, 0, 0, 0);

            yearDayTimePeriodEmpty = new Period(0, 0, 0, 0, 0, 0, 0, 0, PeriodType.YearDayTime);
            yearDayTimePeriodFull = new Period(1, 0, 0, 4, 5, 6, 7, 8, PeriodType.YearDayTime);
        }

        [Test]
        public void AppendFormatter_ThrowsArgumentNull_ForNullFormatterArgument()
        {
            Assert.That(() => new PeriodFormatterBuilder().Append(null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void AppendFormatter_ThrowsInvalidOperation_IfAppendedJustAfterPrefix()
        {
            var builder = new PeriodFormatterBuilder().AppendPrefix("prefix");

            var baseFormatter = new PeriodFormatterBuilder().AppendYears().AppendLiteral("-").ToFormatter();

            Assert.That(() => builder.Append(baseFormatter), Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void AppendFormatter_MergesFormatters()
        {
            var baseFormatter = new PeriodFormatterBuilder().AppendYears().AppendLiteral("-").ToFormatter();

            var formatter = new PeriodFormatterBuilder().Append(baseFormatter).AppendYears().ToFormatter();
            var periodText = "1-1";

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.That(printedValue, Is.EqualTo(periodText));
            Assert.That(printer.CalculatePrintedLength(standardPeriodFull, null), Is.EqualTo(periodText.Length));
            Assert.That(printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null), Is.EqualTo(2));

            var periodValue = formatter.Parse(periodText);
            Assert.That(periodValue, Is.EqualTo(Period.FromYears(1)));
        }

        [Test]
        public void AppendPrinterParser_Throws_IfBothNull()
        {
            Assert.Throws<ArgumentException>(() => new PeriodFormatterBuilder().Append(null, null));
        }

        [Test]
        public void AppendPrinterParser_ThrowsInvalidOperation_IfAppendedJustAfterPrefix()
        {
            var builder = new PeriodFormatterBuilder().AppendPrefix("prefix");

            var baseFormatter = new PeriodFormatterBuilder().AppendYears().AppendLiteral("-").ToFormatter();

            Assert.That(() => builder.Append(baseFormatter.Printer, baseFormatter.Parser), Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void AppendPrinterParser_BuildsOnlyPrinter_IfParserIsNull()
        {
            var printer = new PeriodFormatterBuilder().AppendYears().AppendLiteral("-").ToPrinter();

            var builder2 = new PeriodFormatterBuilder().Append(printer, null).AppendMonths();

            var formatter = builder2.ToFormatter();

            Assert.That(formatter.Print(standardPeriodFull), Is.EqualTo("1-2"));
            Assert.Throws<NotSupportedException>(() => formatter.Parse("1-3"));
        }

        [Test]
        public void AppendPrinterParser_BuildsOnlyParser_IfPrinterIsNull()
        {
            var parser = new PeriodFormatterBuilder().AppendYears().AppendLiteral("-").ToParser();

            var builder2 = new PeriodFormatterBuilder().Append(null, parser).AppendMonths();

            var formatter = builder2.ToFormatter();

            Assert.That(formatter.Parse("1-2"), Is.EqualTo(Period.FromYears(1).WithMonths(2)));
            Assert.Throws<NotSupportedException>(() => formatter.Print(standardPeriodFull));
        }
    }
}