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

namespace NodaTime.Test.Format
{
    public partial class PeriodFormatterBuilderTest
    {
        #region Singular prefix

        [Test]
        public void AppendPrefixNull_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => builder.AppendPrefix(null));
        }

        [Test]
        public void AppendPrefixBeforeYears_BuildsCorrectPrinter_For1YearsStandardPeriod()
        {
            var formatter = builder.AppendPrefix("Years:").AppendYears().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("Years:1", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixBeforeYears_BuildsCorrectPrinter_ForZeroYearsStandardPeriod()
        {
            var formatter = builder.AppendPrefix("Years:").AppendYears().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("Years:0", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixBeforeMonths_BuildsCorrectPrinter_For2MOnthsStandardPeriod()
        {
            var formatter = builder.AppendPrefix("Months:").AppendMonths().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("Months:2", printedValue);
            Assert.AreEqual(8, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixBeforeMonths_BuildsCorrectPrinter_ForZeroMonthsStandardPeriod()
        {
            var formatter = builder.AppendPrefix("Months:").AppendMonths().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("Months:0", printedValue);
            Assert.AreEqual(8, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region Plural prefix

        [Test]
        public void AppendPrefixPluralNull_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => builder.AppendPrefix("prefix", null));
            Assert.Throws<ArgumentNullException>(() => builder.AppendPrefix(null, "prefix"));
            Assert.Throws<ArgumentNullException>(() => builder.AppendPrefix(null, null));

        }

        [Test]
        public void AppendPrefixPluralBeforeYears_BuildsCorrectPrinter_ForSingularYearsStandardPeriod()
        {
            var formatter = builder.AppendPrefix("Year:", "Years:").AppendYears().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("Year:1", printedValue);
            Assert.AreEqual(6, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixPluralBeforeYears_BuildsCorrectPrinter_ForPluralYearsStandardPeriod()
        {
            var formatter = builder.AppendPrefix("Year:", "Years:").AppendYears().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("Years:0", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixPluralBeforeHours_BuildsCorrectPrinter_For5HoursStandardPeriod()
        {
            var formatter = builder.AppendPrefix("Hour:", "Hours:").AppendMinutes().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("Hours:6", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixPluralBeforeHours_BuildsCorrectPrinter_ForZeroHoursStandardPeriod()
        {
            var formatter = builder.AppendPrefix("Hour:", "Hours:").AppendMinutes().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("Hours:0", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }


        #endregion

        #region Singular Suffix

        [Test]
        public void AppendSuffixNull_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => builder.AppendSuffix(null));
        }

        [Test]
        public void AppendSuffixWithoutFieldBefore_ThrowsInvalidOperation()
        {
            Assert.Throws<InvalidOperationException>(() => builder.AppendSuffix(" years"));
        }

        [Test]
        public void AppendSuffixAfterYears_BuildsCorrectPrinter_For1YearsStandardPeriod()
        {
            var formatter = builder.AppendYears().AppendSuffix(" years").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("1 years", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendSuffixAfterYears_BuildsCorrectPrinter_ForZeroYearsStandardPeriod()
        {
            var formatter = builder.AppendYears().AppendSuffix(" years").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0 years", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendSuffixAfterHours_BuildsCorrectPrinter_For5HoursStandardPeriod()
        {
            var formatter = builder.AppendHours().AppendSuffix(" hours").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("5 hours", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendSuffixAfterHours_BuildsCorrectPrinter_ForZeroHoursStandardPeriod()
        {
            var formatter = builder.AppendHours().AppendSuffix(" hours").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0 hours", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region Plural Suffix

        [Test]
        public void AppendSuffixPluralNull_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => builder.AppendSuffix("_", null));
            Assert.Throws<ArgumentNullException>(() => builder.AppendSuffix(null, "_"));
            Assert.Throws<ArgumentNullException>(() => builder.AppendSuffix(null, null));
        }

        [Test]
        public void AppendSuffixPluralWithoutFieldBefore_ThrowsInvalidOperation()
        {
            Assert.Throws<InvalidOperationException>(() => builder.AppendSuffix("_", "_"));
        }

        [Test]
        public void AppendSuffixPluralAfterYears_BuildsCorrectPrinter_For1YearsStandardPeriod()
        {
            var formatter = builder.AppendYears().AppendSuffix(" year", " years").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("1 year", printedValue);
            Assert.AreEqual(6, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendSuffixPluralAfterYears_BuildsCorrectPrinter_ForZeroYearsStandardPeriod()
        {
            var formatter = builder.AppendYears().AppendSuffix(" year", " years").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0 years", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendSuffixPluralAfterHours_BuildsCorrectPrinter_For5HoursStandardPeriod()
        {
            var formatter = builder.AppendHours().AppendSuffix(" hour", " hours").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("5 hours", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendSuffixPluralAfterHours_BuildsCorrectPrinter_ForZeroHoursStandardPeriod()
        {
            var formatter = builder.AppendHours().AppendSuffix(" hour", " hours").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("0 hours", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

        #region Prefix And Suffix Together

        [Test]
        public void AppendPrefixAndSuffixOnYears_BuildsCorrectPrinter_For1YearsStandardPeriod()
        {
            var formatter = builder.AppendPrefix("P").AppendYears().AppendSuffix("Y").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(StandardPeriod);

            Assert.AreEqual("P1Y", printedValue);
            Assert.AreEqual(3, printer.CalculatePrintedLength(StandardPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(StandardPeriod, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixAndSuffixOnYears_BuildsCorrectPrinter_ForZeroYearsStandardPeriod()
        {
            var formatter = builder.AppendPrefix("P").AppendYears().AppendSuffix("Y").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(zeroPeriod);

            Assert.AreEqual("P0Y", printedValue);
            Assert.AreEqual(3, printer.CalculatePrintedLength(zeroPeriod, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(zeroPeriod, int.MaxValue, null));
        }

        #endregion

    }
}
