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
            var formatter =
                builder.AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("1-2-3-4", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void ZeroDefault_BuildsCorrectPrinter_ForEmptyStandardPeriod()
        {
            var formatter =
                builder.AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("---0", printedValue);
            Assert.AreEqual(4, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void ZeroDefault_BuildsCorrectPrinter_ForEmptyStandardPeriod2YearsField()
        {
            var formatter = builder.AppendYears().AppendLiteral("-").AppendYears().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("-0", printedValue);
            Assert.AreEqual(2, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void ZeroDefault_BuildsCorrectPrinter_ForFullYearDayTimePeriod()
        {
            var formatter =
                builder.AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(yearDayTimePeriodFull);

            Assert.AreEqual("1---4", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(yearDayTimePeriodFull, null));
            Assert.AreEqual(2, printer.CountFieldsToPrint(yearDayTimePeriodFull, int.MaxValue, null));
        }

        [Test]
        public void ZeroDefault_BuildsCorrectPrinter_ForEmptyYearDayTimePeriod()
        {
            var formatter =
                builder.AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(yearDayTimePeriodEmpty);

            Assert.AreEqual("---0", printedValue);
            Assert.AreEqual(4, printer.CalculatePrintedLength(yearDayTimePeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(yearDayTimePeriodEmpty, int.MaxValue, null));
        }
        #endregion

        #region ZeroRarelyLast
        [Test]
        public void ZeroRarelyLast_PrintsYearsMonthsWeeksDays_ForFullStandardPeriod()
        {
            var formatter =
                builder.PrintZeroRarelyLast().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("1-2-3-4", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyLast_PrintsYearsMonthsWeeksDays_ForEmptyStandardPeriod()
        {
            var formatter =
                builder.PrintZeroRarelyLast().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("---0", printedValue);
            Assert.AreEqual(4, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyLast_PrintsYearsMonthsWeeksDays_ForEmptyYearDayTimePeriod()
        {
            var formatter =
                builder.PrintZeroRarelyLast().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(yearDayTimePeriodEmpty);

            Assert.AreEqual("---0", printedValue);
            Assert.AreEqual(4, printer.CalculatePrintedLength(yearDayTimePeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(yearDayTimePeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyLast_PrintsYearsMonthsWeeksDays_ForFullYearDayTimePeriod()
        {
            var formatter =
                builder.PrintZeroRarelyLast().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(yearDayTimePeriodFull);

            Assert.AreEqual("1---4", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(yearDayTimePeriodFull, null));
            Assert.AreEqual(2, printer.CountFieldsToPrint(yearDayTimePeriodFull, int.MaxValue, null));
        }
        #endregion

        #region ZeroRarelyFirst
        [Test]
        public void ZeroRarelyFirst_PrintsYearsMonthsWeeksDays_ForFullStandardPeriod()
        {
            var formatter =
                builder.PrintZeroRarelyFirst().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("1-2-3-4", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsYearsMonthsWeeksDays_ForEmptyStandardPeriod()
        {
            var formatter =
                builder.PrintZeroRarelyFirst().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0---", printedValue);
            Assert.AreEqual(4, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsYearsMonthsWeeksDays_ForFullYearDayTimePeriod()
        {
            var formatter =
                builder.PrintZeroRarelyFirst().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(yearDayTimePeriodFull);

            Assert.AreEqual("1---4", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(yearDayTimePeriodFull, null));
            Assert.AreEqual(2, printer.CountFieldsToPrint(yearDayTimePeriodFull, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsYearsMonthsWeeksDays_ForEmptyYearDayTimePeriod()
        {
            var formatter =
                builder.PrintZeroRarelyFirst().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(yearDayTimePeriodEmpty);

            Assert.AreEqual("0---", printedValue);
            Assert.AreEqual(4, printer.CalculatePrintedLength(yearDayTimePeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(yearDayTimePeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsYears_ForEmptyStandardPeriod()
        {
            var formatter = builder.PrintZeroRarelyFirst().AppendYears().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsMonths_ForEmptyStandardPeriod()
        {
            var formatter = builder.PrintZeroRarelyFirst().AppendMonths().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsWeeks_ForEmptyStandardPeriod()
        {
            var formatter = builder.PrintZeroRarelyFirst().AppendWeeks().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsHours_ForEmptyStandardPeriod()
        {
            var formatter = builder.PrintZeroRarelyFirst().AppendHours().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsMinutes_ForEmptyStandardPeriod()
        {
            var formatter = builder.PrintZeroRarelyFirst().AppendMinutes().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void ZeroRarelyFirst_PrintsSeconds_ForEmptyStandardPeriod()
        {
            var formatter = builder.PrintZeroRarelyFirst().AppendSeconds().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }
        #endregion

        #region ZeroIfSupported
        [Test]
        public void ZeroIfSupported_PrintsYearsMonthsWeeksDays_ForFullStandardPeriod()
        {
            var formatter =
                builder.PrintZeroIfSupported().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("1-2-3-4", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void ZeroIfSupported_PrintsYearsMonthsWeeksDays_ForEmptyStandardPeriod()
        {
            var formatter =
                builder.PrintZeroIfSupported().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0-0-0-0", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void ZeroIfSupported_PrintsYearsMonthsWeeksDays_ForFullYearDayTimePeriod()
        {
            var formatter =
                builder.PrintZeroIfSupported().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(yearDayTimePeriodFull);

            Assert.AreEqual("1---4", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(yearDayTimePeriodFull, null));
            Assert.AreEqual(2, printer.CountFieldsToPrint(yearDayTimePeriodFull, int.MaxValue, null));
        }

        [Test]
        public void ZeroIfSupported_PrintsYearsMonthsWeeksDays_ForEmptyYearDayTimePeriod()
        {
            var formatter =
                builder.PrintZeroIfSupported().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(yearDayTimePeriodEmpty);

            Assert.AreEqual("0---0", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(yearDayTimePeriodEmpty, null));
            Assert.AreEqual(2, printer.CountFieldsToPrint(yearDayTimePeriodEmpty, int.MaxValue, null));
        }
        #endregion

        #region ZeroAlways
        [Test]
        public void ZeroAlways_PrintsYearsMonthsWeeksDays_ForFullStandardPeriod()
        {
            var formatter =
                builder.PrintZeroAlways().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("1-2-3-4", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void ZeroAlways_PrintsYearsMonthsWeeksDays_ForEmptyStandardPeriod()
        {
            var formatter =
                builder.PrintZeroAlways().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0-0-0-0", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void ZeroAlways_PrintsYearsMonthsWeeksDays_ForFullYearDayTimePeriod()
        {
            var formatter =
                builder.PrintZeroAlways().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(yearDayTimePeriodFull);

            Assert.AreEqual("1-0-0-4", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(yearDayTimePeriodFull, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(yearDayTimePeriodFull, int.MaxValue, null));
        }

        [Test]
        public void ZeroAlways_PrintsYearsMonthsWeeksDays_ForEmptyYearDayTimePeriod()
        {
            var formatter =
                builder.PrintZeroAlways().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(yearDayTimePeriodEmpty);

            Assert.AreEqual("0-0-0-0", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(yearDayTimePeriodEmpty, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(yearDayTimePeriodEmpty, int.MaxValue, null));
        }
        #endregion

        #region ZeroNever
        [Test]
        public void ZeroNever_PrintsYearsMonthsWeeksDays_ForFullStandardPeriod()
        {
            var formatter =
                builder.PrintZeroNever().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("1-2-3-4", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(4, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void ZeroNever_PrintsYearsMonthsWeeksDays_ForEmptyStandardPeriod()
        {
            var formatter =
                builder.PrintZeroNever().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("---", printedValue);
            Assert.AreEqual(3, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(0, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void ZeroNever_PrintsYearsMonthsWeeksDays_ForFullYearDayTimePeriod()
        {
            var formatter =
                builder.PrintZeroNever().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(yearDayTimePeriodFull);

            Assert.AreEqual("1---4", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(yearDayTimePeriodFull, null));
            Assert.AreEqual(2, printer.CountFieldsToPrint(yearDayTimePeriodFull, int.MaxValue, null));
        }

        [Test]
        public void ZeroNever_PrintsYearsMonthsWeeksDays_ForEmptyYearDayTimePeriod()
        {
            var formatter =
                builder.PrintZeroNever().AppendYears().AppendLiteral("-").AppendMonths().AppendLiteral("-").AppendWeeks().AppendLiteral("-").AppendDays().
                    ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(yearDayTimePeriodEmpty);

            Assert.AreEqual("---", printedValue);
            Assert.AreEqual(3, printer.CalculatePrintedLength(yearDayTimePeriodEmpty, null));
            Assert.AreEqual(0, printer.CountFieldsToPrint(yearDayTimePeriodEmpty, int.MaxValue, null));
        }
        #endregion
    }
}