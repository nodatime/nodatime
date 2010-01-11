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
using System;

namespace NodaTime.Test.Format
{
    public partial class PeriodFormatterBuilderTest
    {
        #region Years

        [Test]
        public void AppendYears_Prints1_For1YearStandardPeriod()
        {
            var formatter = builder.AppendYears().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("1", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));            
        }

        [Test]
        public void AppendYears_Parse1_To1YearStandardPeriod()
        {
            var formatter = builder.AppendYears().ToFormatter();

            var parsedPeriod = formatter.Parse("1");

            Assert.AreEqual(Period.FromYears(1), parsedPeriod);
        }

        [Test]
        public void AppendYears_ParsePlus1_To1YearStandardPeriod()
        {
            var formatter = builder.AppendYears().ToFormatter();

            var parsedPeriod = formatter.Parse("+1");

            Assert.AreEqual(Period.FromYears(1), parsedPeriod);
        }

        [Test]
        public void AppendYears_ParseMinus1_ToMinus1YearStandardPeriod()
        {
            var formatter = builder.AppendYears().ToFormatter();

            var parsedPeriod = formatter.Parse("-1");

            Assert.AreEqual(Period.FromYears(-1), parsedPeriod);
        }

        [Test]
        public void AppendYears_ParseMinusMinus1_Throws()
        {
            var formatter = builder.AppendYears().ToFormatter();

            Assert.Throws<ArgumentException>(() => formatter.Parse("--1"));
        }

        [Test]
        public void AppendYears_Prints0_ForZeroYearStandardPeriod()
        {
            var formatter = builder.AppendYears().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendYears_Parses0_ToZeroYearStandardPeriod()
        {
            var formatter = builder.AppendYears().ToFormatter();

            var parsedPeriod = formatter.Parse("0");

            Assert.AreEqual(Period.FromYears(0), parsedPeriod);
        }

        #endregion

        #region Months

        [Test]
        public void AppendMonths_Prints2_For2MonthStandardPeriod()
        {
            var formatter = builder.AppendMonths().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("2", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendMonths_Parse2_To2MonthStandardPeriod()
        {
            var formatter = builder.AppendMonths().ToFormatter();

            var parsedPeriod = formatter.Parse("2");

            Assert.AreEqual(Period.FromMonths(2), parsedPeriod);
        }

        [Test]
        public void AppendMonths_ParsePlus2_To2MonthStandardPeriod()
        {
            var formatter = builder.AppendMonths().ToFormatter();

            var parsedPeriod = formatter.Parse("+2");

            Assert.AreEqual(Period.FromMonths(2), parsedPeriod);
        }

        [Test]
        public void AppendMonths_ParseMinus2_ToMinus2MonthStandardPeriod()
        {
            var formatter = builder.AppendMonths().ToFormatter();

            var parsedPeriod = formatter.Parse("-2");

            Assert.AreEqual(Period.FromMonths(-2), parsedPeriod);
        }

        [Test]
        public void AppendMonths_Prints0_ForZeroMonthStandardPeriod()
        {
            var formatter = builder.AppendMonths().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendMonths_Parse0_ToZeroMonthStandardPeriod()
        {
            var formatter = builder.AppendMonths().ToFormatter();

            var parsedPeriod = formatter.Parse("0");

            Assert.AreEqual(Period.FromMonths(0), parsedPeriod);
        }

        #endregion

        #region Weeks

        [Test]
        public void AppendWeeks_Prints3_For3WeeksStandardPeriod()
        {
            var formatter = builder.AppendWeeks().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("3", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendWeeks_Parse3_To3WeeksStandardPeriod()
        {
            var formatter = builder.AppendWeeks().ToFormatter();

            var parsedPeriod = formatter.Parse("3");

            Assert.AreEqual(Period.FromWeeks(3), parsedPeriod);
        }

        [Test]
        public void AppendWeeks_ParsePlus3_To3WeeksStandardPeriod()
        {
            var formatter = builder.AppendWeeks().ToFormatter();

            var parsedPeriod = formatter.Parse("+3");

            Assert.AreEqual(Period.FromWeeks(3), parsedPeriod);
        }

        [Test]
        public void AppendWeeks_ParseMinus3_ToMinus3WeeksStandardPeriod()
        {
            var formatter = builder.AppendWeeks().ToFormatter();

            var parsedPeriod = formatter.Parse("-3");

            Assert.AreEqual(Period.FromWeeks(-3), parsedPeriod);
        }

        [Test]
        public void AppendWeeks_Prints0_ForZeroWeeksStandardPeriod()
        {
            var formatter = builder.AppendWeeks().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendWeeks_Parses0_ToZeroWeeksStandardPeriod()
        {
            var formatter = builder.AppendWeeks().ToFormatter();

            var parsedPeriod = formatter.Parse("0");

            Assert.AreEqual(Period.FromWeeks(0), parsedPeriod);
        }

        #endregion

        #region Days

        [Test]
        public void AppendDays_Prints4_For4DaysStandardPeriod()
        {
            var formatter = builder.AppendDays().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("4", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendDays_Parses4_To4DaystandardPeriod()
        {
            var formatter = builder.AppendDays().ToFormatter();

            var parsedPeriod = formatter.Parse("4");

            Assert.AreEqual(Period.FromDays(4), parsedPeriod);
        }

        [Test]
        public void AppendDays_ParsesPlus4_To4DaystandardPeriod()
        {
            var formatter = builder.AppendDays().ToFormatter();

            var parsedPeriod = formatter.Parse("+4");

            Assert.AreEqual(Period.FromDays(4), parsedPeriod);
        }

        [Test]
        public void AppendDays_ParsesMinus4_ToMinus4DaystandardPeriod()
        {
            var formatter = builder.AppendDays().ToFormatter();

            var parsedPeriod = formatter.Parse("-4");

            Assert.AreEqual(Period.FromDays(-4), parsedPeriod);
        }

        [Test]
        public void AppendDays_Prints0_ForZeroDaysStandardPeriod()
        {
            var formatter = builder.AppendDays().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendDays_Parses0_ToZeroDaystandardPeriod()
        {
            var formatter = builder.AppendDays().ToFormatter();

            var parsedPeriod = formatter.Parse("0");

            Assert.AreEqual(Period.FromDays(0), parsedPeriod);
        }


        #endregion

        #region Hours

        [Test]
        public void AppendHours_Prints5_For5HoursStandardPeriod()
        {
            var formatter = builder.AppendHours().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("5", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendHours_Parses5_To5HourStandardPeriod()
        {
            var formatter = builder.AppendHours().ToFormatter();

            var parsedPeriod = formatter.Parse("5");

            Assert.AreEqual(Period.FromHours(5), parsedPeriod);
        }

        [Test]
        public void AppendHours_ParsesPlus5_To5HourStandardPeriod()
        {
            var formatter = builder.AppendHours().ToFormatter();

            var parsedPeriod = formatter.Parse("+5");

            Assert.AreEqual(Period.FromHours(5), parsedPeriod);
        }

        [Test]
        public void AppendHours_ParsesMinuss5_ToMinus5HourStandardPeriod()
        {
            var formatter = builder.AppendHours().ToFormatter();

            var parsedPeriod = formatter.Parse("-5");

            Assert.AreEqual(Period.FromHours(-5), parsedPeriod);
        }

        [Test]
        public void AppendHours_Prints0_ForZeroHoursStandardPeriod()
        {
            var formatter = builder.AppendHours().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendHours_Parses0_ToZeroHourStandardPeriod()
        {
            var formatter = builder.AppendHours().ToFormatter();

            var parsedPeriod = formatter.Parse("0");

            Assert.AreEqual(Period.FromHours(0), parsedPeriod);
        }

        #endregion

        #region Minutes

        [Test]
        public void AppendMinutes_Prints6_For6MinutesStandardPeriod()
        {
            var formatter = builder.AppendMinutes().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("6", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendMinutes_Parses6_To6MinutesStandardPeriod()
        {
            var formatter = builder.AppendMinutes().ToFormatter();

            var parsedPeriod = formatter.Parse("6");

            Assert.AreEqual(Period.FromMinutes(6), parsedPeriod);
        }

        [Test]
        public void AppendMinutes_ParsesPlus6_ToPlus6MinutesStandardPeriod()
        {
            var formatter = builder.AppendMinutes().ToFormatter();

            var parsedPeriod = formatter.Parse("+6");

            Assert.AreEqual(Period.FromMinutes(6), parsedPeriod);
        }

        [Test]
        public void AppendMinutes_ParsesMinus6_ToMinus6MinutesStandardPeriod()
        {
            var formatter = builder.AppendMinutes().ToFormatter();

            var parsedPeriod = formatter.Parse("-6");

            Assert.AreEqual(Period.FromMinutes(-6), parsedPeriod);
        }

        [Test]
        public void AppendMinutes_Prints0_ForZeroMinutesStandardPeriod()
        {
            var formatter = builder.AppendMinutes().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendMinutes_Parses0_ToZeroMinutesStandardPeriod()
        {
            var formatter = builder.AppendMinutes().ToFormatter();

            var parsedPeriod = formatter.Parse("0");

            Assert.AreEqual(Period.FromMinutes(0), parsedPeriod);
        }

        #endregion

        #region Seconds

        [Test]
        public void AppendSeconds_Prints7_For7SecondsStandardPeriod()
        {
            var formatter = builder.AppendSeconds().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("7", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendSeconds_Parses7_To7SecondsStandardPeriod()
        {
            var formatter = builder.AppendSeconds().ToFormatter();

            var parsedPeriod = formatter.Parse("7");

            Assert.AreEqual(Period.FromSeconds(7), parsedPeriod);
        }

        [Test]
        public void AppendSeconds_ParsesPlus7_To7SecondsStandardPeriod()
        {
            var formatter = builder.AppendSeconds().ToFormatter();

            var parsedPeriod = formatter.Parse("+7");

            Assert.AreEqual(Period.FromSeconds(7), parsedPeriod);
        }

        [Test]
        public void AppendSeconds_ParsesMinus7_ToMinus7SecondsStandardPeriod()
        {
            var formatter = builder.AppendSeconds().ToFormatter();

            var parsedPeriod = formatter.Parse("-7");

            Assert.AreEqual(Period.FromSeconds(-7), parsedPeriod);
        }

        [Test]
        public void AppendSeconds_Prints0_ForZeroSecondsStandardPeriod()
        {
            var formatter = builder.AppendSeconds().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendSeconds_Parses0_ToZeroSecondsStandardPeriod()
        {
            var formatter = builder.AppendSeconds().ToFormatter();

            var parsedPeriod = formatter.Parse("0");

            Assert.AreEqual(Period.FromSeconds(0), parsedPeriod);
        }

        #endregion

        #region Milliseconds

        [Test]
        public void AppendMilliseconds_Prints8_For8MillisecondsStandardPeriod()
        {
            var formatter = builder.AppendMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("8", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendMilliseconds_Parses8_To8MillisecondsStandardPeriod()
        {
            var formatter = builder.AppendMillis().ToFormatter();

            var parsedPeriod = formatter.Parse("8");

            Assert.AreEqual(Period.FromMilliseconds(8), parsedPeriod);
        }

        [Test]
        public void AppendMilliseconds_Prints0_ForZeroMillisecondsStandardPeriod()
        {
            var formatter = builder.AppendMillis().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendMilliseconds_Parses0_ToZeroMillisecondsStandardPeriod()
        {
            var formatter = builder.AppendMillis().ToFormatter();

            var parsedPeriod = formatter.Parse("0");

            Assert.AreEqual(Period.FromMilliseconds(0), parsedPeriod);
        }

        [Test]
        public void AppendMilliseconds3Digit_Prints008_For8MillisecondsStandardPeriod()
        {
            var formatter = builder.AppendMillis3Digit().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("008", printedValue);
            Assert.AreEqual(3, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendMilliseconds3Digit_Prints000_ForZeroMillisecondsStandardPeriod()
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
        public void AppendSecondsWithMilliseconds_BuildsCorrectParser_To7SecondsStandardPeriod()
        {
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var parsedPeriod = formatter.Parse("7.000");

            Assert.AreEqual(Period.FromSeconds(7), parsedPeriod);
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
        public void AppendSecondsWithMilliseconds_BuildsCorrectParser_To7Seconds1MillisecondsStandardPeriod()
        {
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var parsedPeriod = formatter.Parse("7.001");

            Assert.AreEqual(Period.FromSeconds(7).WithMilliseconds(1), parsedPeriod);
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
        public void AppendSecondsWithMilliseconds_BuildsCorrectParser_To7Seconds999MillisecondsStandardPeriod()
        {
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var parsedPeriod = formatter.Parse("7.999");

            Assert.AreEqual(Period.FromSeconds(7).WithMilliseconds(999), parsedPeriod);
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
        public void AppendSecondsWithMilliseconds_BuildsCorrectParser_To7Seconds100MillisecondsStandardPeriod()
        {
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var parsedPeriod = formatter.Parse("7.1000");

            //only 3 digits are considering when parsing milliseconds
            Assert.AreEqual(Period.FromSeconds(7).WithMilliseconds(100), parsedPeriod);
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
