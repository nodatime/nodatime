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
using NodaTime.Periods;

namespace NodaTime.Test.Format
{
    public partial class PeriodFormatterBuilderTest
    {
        #region Years

        [Test]
        public void AppendYears_BuildsCorrectPrinter_For1YearStandardPeriod()
        {
            var formatter = builder.AppendYears().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("1", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));            
        }

        [Test]
        public void AppendYears_BuildsCorrectParser_For1YearStandardPeriod()
        {
            var formatter = builder.AppendYears().ToFormatter();

            var parsedPeriod = formatter.Parse("1");

            Assert.AreEqual(Period.FromYears(1), parsedPeriod);
        }

        [Test]
        public void AppendYears_BuildsCorrectPrinter_ForZeroYearStandardPeriod()
        {
            var formatter = builder.AppendYears().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region Months

        [Test]
        public void AppendMonths_BuildsCorrectPrinter_For2MonthStandardPeriod()
        {
            var formatter = builder.AppendMonths().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("2", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendMonths_BuildsCorrectPrinter_ForzeroMonthStandardPeriod()
        {
            var formatter = builder.AppendMonths().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region Weeks

        [Test]
        public void AppendWeeks_BuildsCorrectPrinter_For3WeeksStandardPeriod()
        {
            var formatter = builder.AppendWeeks().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("3", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendWeeks_BuildsCorrectPrinter_ForZeroWeeksStandardPeriod()
        {
            var formatter = builder.AppendWeeks().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region Days

        [Test]
        public void AppendDays_BuildsCorrectPrinter_For4DaysStandardPeriod()
        {
            var formatter = builder.AppendDays().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("4", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendDays_BuildsCorrectPrinter_ForZeroDaysStandardPeriod()
        {
            var formatter = builder.AppendDays().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region Hours

        [Test]
        public void AppendHours_BuildsCorrectPrinter_For5HoursStandardPeriod()
        {
            var formatter = builder.AppendHours().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("5", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendHours_BuildsCorrectPrinter_ForZeroHoursStandardPeriod()
        {
            var formatter = builder.AppendHours().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region Minutes

        [Test]
        public void AppendMinutes_BuildsCorrectPrinter_For6MinutesStandardPeriod()
        {
            var formatter = builder.AppendMinutes().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("6", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendMinutes_BuildsCorrectPrinter_ForZeroMinutesStandardPeriod()
        {
            var formatter = builder.AppendMinutes().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region Seconds

        [Test]
        public void AppendSeconds_BuildsCorrectPrinter_For7SecondsStandardPeriod()
        {
            var formatter = builder.AppendSeconds().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("7", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendSeconds_BuildsCorrectPrinter_ForZeroSecondsStandardPeriod()
        {
            var formatter = builder.AppendSeconds().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region Milliseconds

        [Test]
        public void AppendMilliseconds_BuildsCorrectPrinter_For8MillisecondsStandardPeriod()
        {
            var formatter = builder.AppendMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("8", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendMilliseconds_BuildsCorrectPrinter_ForZeroMillisecondsStandardPeriod()
        {
            var formatter = builder.AppendMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendMilliseconds3Digit_BuildsCorrectPrinter_For8MillisecondsStandardPeriod()
        {
            var formatter = builder.AppendMillis3Digit().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("008", printedValue);
            Assert.AreEqual(3, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendMilliseconds3Digit_BuildsCorrectPrinter_ForZeroMillisecondsStandardPeriod()
        {
            var formatter = builder.AppendMillis3Digit().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("000", printedValue);
            Assert.AreEqual(3, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region SecondsWithMilliseconds

        [Test]
        public void AppendSecondsWithMilliseconds_BuildsCorrectPrinter_For7SecondsStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, 7, 0);
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("7.000", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithMilliseconds_BuildsCorrectPrinter_For7Seconds1MillisecondStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, 7, 1);
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("7.001", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithMilliseconds_BuildsCorrectPrinter_For7Seconds999MillisecondStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, 7, 999);
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("7.999", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithMilliseconds_BuildsCorrectPrinter_For7Seconds1000MillisecondStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, 7, 1000);
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("8.000", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithMilliseconds_BuildsCorrectPrinter_For7Seconds1001MillisecondStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, 7, 1001);
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("8.001", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithMilliseconds_BuildsCorrectPrinter_For7SecondsMinus1MillisecondStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, 7, -1);
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("6.999", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithMilliseconds_BuildsCorrectPrinter_ForMinus7Seconds1MillisecondStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, -7, 1);
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("-6.999", printedValue);
            Assert.AreEqual(6, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithMilliseconds_BuildsCorrectPrinter_ForMinus7SecondsMinus1MillisecondStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, -7, -1);
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("-7.001", printedValue);
            Assert.AreEqual(6, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithMilliseconds_BuildsCorrectPrinter_For0Seconds0MillisecondStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, 0, 0);
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("0.000", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        #endregion

        #region SecondsWithOptionalMilliseconds

        [Test]
        public void AppendSecondsWithOptionalMilliseconds_BuildsCorrectPrinter_For7SecondsStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, 7, 0);
            var formatter = builder.AppendSecondsWithOptionalMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("7", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithOptionalMilliseconds_BuildsCorrectPrinter_For7Seconds1MillisecondsStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, 7, 1);
            var formatter = builder.AppendSecondsWithOptionalMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("7.001", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithOptionalMilliseconds_BuildsCorrectPrinter_For7Seconds999MillisecondsStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, 7, 999);
            var formatter = builder.AppendSecondsWithOptionalMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("7.999", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithOptionalMilliseconds_BuildsCorrectPrinter_For7Seconds1000MillisecondsStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, 7, 1000);
            var formatter = builder.AppendSecondsWithOptionalMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("8", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithOptionalMilliseconds_BuildsCorrectPrinter_For7Seconds1001MillisecondsStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, 7, 1001);
            var formatter = builder.AppendSecondsWithOptionalMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("8.001", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithOptionalMilliseconds_BuildsCorrectPrinter_For7SecondsMinus1MillisecondsStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, 7, -1);
            var formatter = builder.AppendSecondsWithOptionalMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("6.999", printedValue);
            Assert.AreEqual(5, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithOptionalMilliseconds_BuildsCorrectPrinter_ForMinus7Seconds1MillisecondsStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, -7, 1);
            var formatter = builder.AppendSecondsWithOptionalMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("-6.999", printedValue);
            Assert.AreEqual(6, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithOptionalMilliseconds_BuildsCorrectPrinter_ForMinus7SecondsMinus1MillisecondsStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, -7, -1);
            var formatter = builder.AppendSecondsWithOptionalMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("-7.001", printedValue);
            Assert.AreEqual(6, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        [Test]
        public void AppendSecondsWithOptionalMilliseconds_BuildsCorrectPrinter_ForZeroSecondsZeroMillisecondsStandardPeriod()
        {
            Period p = new Period(0, 0, 0, 0, 0, 0, 0, 0);
            var formatter = builder.AppendSecondsWithOptionalMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(p);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(p, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(p, int.MaxValue, null));
        }

        #endregion
    }
}
