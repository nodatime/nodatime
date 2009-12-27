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

using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public partial class PeriodFormatterBuilderTest
    {
        #region ZeroDefault

        [Test]
        public void ZeroDefault_BuildsCorrectPrinter_ForStandardPeriod()
        {
            var formatter = builder
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("1-2-3-4", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroDefault_BuildsCorrectPrinter_ForEmptyYearDayTimePeriod()
        {
            var formatter = builder
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(emptyYearDayTimePeriod);

            Assert.AreEqual("---0", printedValue);
            Assert.AreEqual(4, printer.CalculatePrintedLength(emptyYearDayTimePeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(emptyYearDayTimePeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroDefault_BuildsCorrectPrinter_ForFullYearDayTimePeriod()
        {
            var formatter = builder
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(fullYearDayTimePeriod);

            Assert.AreEqual("1---4", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(fullYearDayTimePeriod, null));
            Assert.AreEqual(2, printer.CountFieldsToPrint(fullYearDayTimePeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroDefault_BuildsCorrectPrinter_ForEmptyStandardPeriod()
        {
            var formatter = builder
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("---0", printedValue);
            Assert.AreEqual(4, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroDefault_BuildsCorrectPrinter_ForEmptyStandardPeriod2YearsField()
        {
            var formatter = builder
                .AppendYears().AppendLiteral("-")
                .AppendYears()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("-0", printedValue);
            Assert.AreEqual(2, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region ZeroRarelyLast

        [Test]
        public void ZeroRarelyLast_PrintsYearsMonthsWeeksDays_ForFullStandardPeriod()
        {
            var formatter = builder
                .PrintZeroRarelyLast()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("1-2-3-4", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyLast_PrintsYearsMonthsWeeksDays_ForEmptyYearDayTimePeriod()
        {
            var formatter = builder
                .PrintZeroRarelyLast()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(emptyYearDayTimePeriod);

            Assert.AreEqual("---0", printedValue);
            Assert.AreEqual(4, printer.CalculatePrintedLength(emptyYearDayTimePeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(emptyYearDayTimePeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyLast_PrintsYearsMonthsWeeksDays_ForFullYearDayTimePeriod()
        {
            var formatter = builder
                .PrintZeroRarelyLast()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(fullYearDayTimePeriod);

            Assert.AreEqual("1---4", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(fullYearDayTimePeriod, null));
            Assert.AreEqual(2, printer.CountFieldsToPrint(fullYearDayTimePeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyLast_PrintsYearsMonthsWeeksDays_ForEmptyStandardPeriod()
        {
            var formatter = builder
                .PrintZeroRarelyLast()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("---0", printedValue);
            Assert.AreEqual(4, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region ZeroRarelyFirst

        [Test]
        public void ZeroRarelyFirst_PrintsYearsMonthsWeeksDays_ForFullStandardPeriod()
        {
            var formatter = builder
                .PrintZeroRarelyFirst()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("1-2-3-4", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsYearsMonthsWeeksDays_ForEmptyYearDayTimePeriod()
        {
            var formatter = builder
                .PrintZeroRarelyFirst()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(emptyYearDayTimePeriod);

            Assert.AreEqual("0---", printedValue);
            Assert.AreEqual(4, printer.CalculatePrintedLength(emptyYearDayTimePeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(emptyYearDayTimePeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsYearsMonthsWeeksDays_ForFullYearDayTimePeriod()
        {
            var formatter = builder
                .PrintZeroRarelyFirst()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(fullYearDayTimePeriod);

            Assert.AreEqual("1---4", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(fullYearDayTimePeriod, null));
            Assert.AreEqual(2, printer.CountFieldsToPrint(fullYearDayTimePeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsYearsMonthsWeeksDays_ForEmptyStandardPeriod()
        {
            var formatter = builder
                .PrintZeroRarelyFirst()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0---", printedValue);
            Assert.AreEqual(4, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsYears_ForEmptyStandardPeriod()
        {
            var formatter = builder
                .PrintZeroRarelyFirst()
                .AppendYears()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsMonths_ForEmptyStandardPeriod()
        {
            var formatter = builder
                .PrintZeroRarelyFirst()
                .AppendMonths()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsWeeks_ForEmptyStandardPeriod()
        {
            var formatter = builder
                .PrintZeroRarelyFirst()
                .AppendWeeks()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsHours_ForEmptyStandardPeriod()
        {
            var formatter = builder
                .PrintZeroRarelyFirst()
                .AppendHours()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsMinutes_ForEmptyStandardPeriod()
        {
            var formatter = builder
                .PrintZeroRarelyFirst()
                .AppendMinutes()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsSeconds_ForEmptyStandardPeriod()
        {
            var formatter = builder
                .PrintZeroRarelyFirst()
                .AppendSeconds()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region ZeroIfSupported

        [Test]
        public void ZeroIfSupported_PrintsYearsMonthsWeeksDays_ForFullStandardPeriod()
        {
            var formatter = builder
                .PrintZeroIfSupported()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("1-2-3-4", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroIfSupported_PrintsYearsMonthsWeeksDays_ForEmptyYearDayTimePeriod()
        {
            var formatter = builder
                .PrintZeroIfSupported()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(emptyYearDayTimePeriod);

            Assert.AreEqual("0---0", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(emptyYearDayTimePeriod, null));
            Assert.AreEqual(2, printer.CountFieldsToPrint(emptyYearDayTimePeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroIfSupported_PrintsYearsMonthsWeeksDays_ForFullYearDayTimePeriod()
        {
            var formatter = builder
                .PrintZeroIfSupported()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(fullYearDayTimePeriod);

            Assert.AreEqual("1---4", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(fullYearDayTimePeriod, null));
            Assert.AreEqual(2, printer.CountFieldsToPrint(fullYearDayTimePeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroIfSupported_PrintsYearsMonthsWeeksDays_ForEmptyStandardPeriod()
        {
            var formatter = builder
                .PrintZeroIfSupported()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0-0-0-0", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region ZeroAlways

        [Test]
        public void ZeroAlways_PrintsYearsMonthsWeeksDays_ForFullStandardPeriod()
        {
            var formatter = builder
                .PrintZeroAlways()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("1-2-3-4", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroAlways_PrintsYearsMonthsWeeksDays_ForEmptyYearDayTimePeriod()
        {
            var formatter = builder
                .PrintZeroAlways()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(emptyYearDayTimePeriod);

            Assert.AreEqual("0-0-0-0", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(emptyYearDayTimePeriod, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(emptyYearDayTimePeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroAlways_PrintsYearsMonthsWeeksDays_ForFullYearDayTimePeriod()
        {
            var formatter = builder
                .PrintZeroAlways()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(fullYearDayTimePeriod);

            Assert.AreEqual("1-0-0-4", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(fullYearDayTimePeriod, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(fullYearDayTimePeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroAlways_PrintsYearsMonthsWeeksDays_ForEmptyStandardPeriod()
        {
            var formatter = builder
                .PrintZeroAlways()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0-0-0-0", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region ZeroNever

        [Test]
        public void ZeroNever_PrintsYearsMonthsWeeksDays_ForFullStandardPeriod()
        {
            var formatter = builder
                .PrintZeroNever()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("1-2-3-4", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroNever_PrintsYearsMonthsWeeksDays_ForEmptyYearDayTimePeriod()
        {
            var formatter = builder
                .PrintZeroNever()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(emptyYearDayTimePeriod);

            Assert.AreEqual("---", printedValue);
            Assert.AreEqual(3, printer.CalculatePrintedLength(emptyYearDayTimePeriod, null));
            Assert.AreEqual(0, printer.CountFieldsToPrint(emptyYearDayTimePeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroNever_PrintsYearsMonthsWeeksDays_ForFullYearDayTimePeriod()
        {
            var formatter = builder
                .PrintZeroNever()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(fullYearDayTimePeriod);

            Assert.AreEqual("1---4", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(fullYearDayTimePeriod, null));
            Assert.AreEqual(2, printer.CountFieldsToPrint(fullYearDayTimePeriod, int.MaxValue, null));
        }

        [Test]
        public void ZeroNever_PrintsYearsMonthsWeeksDays_ForEmptyStandardPeriod()
        {
            var formatter = builder
                .PrintZeroNever()
                .AppendYears().AppendLiteral("-")
                .AppendMonths().AppendLiteral("-")
                .AppendWeeks().AppendLiteral("-")
                .AppendDays()
                .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("---", printedValue);
            Assert.AreEqual(3, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(0, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion
    }
}
