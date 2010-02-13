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
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public partial class PeriodFormatterBuilderTest
    {
        #region Singular prefix

        [Test]
        public void AppendPrefixNull_ThrowsArgumentNull_ForNullPrefixString()
        {
            Assert.Throws<ArgumentNullException>(() => builder.AppendPrefix(null));
        }

        [Test]
        public void AppendPrefixBeforeYears_BuildsCorrectPrinter_For1YearsStandardPeriod()
        {
            var formatter = builder.AppendPrefix("Years:")
                                    .AppendYears()
                                    .ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("Years:1", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixBeforeYears_BuildsCorrectPrinter_ForZeroYearsStandardPeriod()
        {
            var formatter = builder.AppendPrefix("Years:").AppendYears().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("Years:0", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixBeforeMonths_BuildsCorrectPrinter_For2MonthsStandardPeriod()
        {
            var formatter = builder.AppendPrefix("Months:").AppendMonths().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("Months:2", printedValue);
            Assert.AreEqual(8, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixBeforeMonths_BuildsCorrectPrinter_ForZeroMonthsStandardPeriod()
        {
            var formatter = builder.AppendPrefix("Months:").AppendMonths().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("Months:0", printedValue);
            Assert.AreEqual(8, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixBeforeYears_ParsesTo1YearStandardPeriod_FromPrefixWithFieldString()
        {
            var formatter = builder.AppendPrefix("Years:").AppendYears().ToFormatter();

            var period = formatter.Parse("Years:1");

            Assert.AreEqual(Period.FromYears(1), period);
        }

        [Test]
        public void AppendPrefixBeforeYears_ParseThrowsArgument_ForFieldWithoutPrefixString()
        {
            var formatter = builder.AppendPrefix("Years:").AppendYears().ToFormatter();
            Assert.Throws<ArgumentException>(() => formatter.Parse("1"));
        }

        [Test]
        public void AppendPrefixBeforeYears_ParseThrowsArgument_ForPrefixWithoutFieldString()
        {
            var formatter = builder.AppendPrefix("Years:").AppendYears().ToFormatter();
            Assert.Throws<ArgumentException>(() => formatter.Parse("Years:"));
        }
        [Test]
        public void AppendPrefixBeforeMonths_ParsesTo2MonthsStandardPeriod_FromPrefixWithFieldString()
        {
            var formatter = builder.AppendPrefix("Months:").AppendMonths().ToFormatter();

            var period = formatter.Parse("Months:2");

            Assert.AreEqual(Period.FromMonths(2), period);
        }

        [Test]
        public void AppendPrefixBeforeMonths_ParseThrowsArgument_ForFieldWithoutPrefixString()
        {
            var formatter = builder.AppendPrefix("Months:").AppendMonths().ToFormatter();
            Assert.Throws<ArgumentException>(() => formatter.Parse("2"));
        }

        [Test]
        public void AppendPrefixBeforeMonths_ParseThrowsArgument_ForPrefixWithoutFieldString()
        {
            var formatter = builder.AppendPrefix("Months:").AppendMonths().ToFormatter();
            Assert.Throws<ArgumentException>(() => formatter.Parse("Months:"));
        }

        #endregion

        #region Plural prefix

        [Test]
        public void AppendPrefixPlural_ThrowsArgumentNull_ForAnuNullOfPrefixStrings()
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
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("Year:1", printedValue);
            Assert.AreEqual(6, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixPluralBeforeYears_BuildsCorrectPrinter_ForPluralYearsStandardPeriod()
        {
            var formatter = builder.AppendPrefix("Year:", "Years:").AppendYears().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("Years:0", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixPluralBeforeHours_BuildsCorrectPrinter_For6HoursStandardPeriod()
        {
            var formatter = builder.AppendPrefix("Hour:", "Hours:").AppendMinutes().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("Hours:6", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixPluralBeforeHours_BuildsCorrectPrinter_ForZeroHoursStandardPeriod()
        {
            var formatter = builder.AppendPrefix("Hour:", "Hours:").AppendMinutes().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("Hours:0", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixPluralBeforeYears_ParsesTo1YearStandardPeriod_FromPrefixWithFieldString()
        {
            var formatter = builder.AppendPrefix("Year:", "Years:").AppendYears().ToFormatter();

            var period = formatter.Parse("Year:1");

            Assert.AreEqual(Period.FromYears(1), period);
        }

        [Test]
        public void AppendPrefixPluralBeforeYears_ParsesToZeroYearsStandardPeriod_FromPrefixWithFieldString()
        {
            var formatter = builder.AppendPrefix("Year:", "Years:").AppendYears().ToFormatter();

            var period = formatter.Parse("Years:0");

            Assert.AreEqual(Period.FromYears(0), period);
        }

        #endregion

        #region Singular Suffix

        [Test]
        public void AppendSuffix_ThrowsArgumentNull_ForNullSuffixString()
        {
            Assert.Throws<ArgumentNullException>(() => builder.AppendSuffix(null));
        }

        [Test]
        public void AppendSuffix_ThrowsInvalidOperation_WithoutFieldBefore()
        {
            Assert.Throws<InvalidOperationException>(() => builder.AppendSuffix(" years"));
        }

        [Test]
        public void AppendSuffixAfterYears_BuildsCorrectPrinter_For1YearsStandardPeriod()
        {
            var formatter = builder.AppendYears().AppendSuffix(" years").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("1 years", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void AppendSuffixAfterYears_BuildsCorrectPrinter_ForZeroYearsStandardPeriod()
        {
            var formatter = builder.AppendYears().AppendSuffix(" years").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0 years", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void AppendSuffixAfterHours_BuildsCorrectPrinter_For5HoursStandardPeriod()
        {
            var formatter = builder.AppendHours().AppendSuffix(" hours").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("5 hours", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void AppendSuffixAfterHours_BuildsCorrectPrinter_ForZeroHoursStandardPeriod()
        {
            var formatter = builder.AppendHours().AppendSuffix(" hours").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0 hours", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void AppendSuffixAfterYears_ParsesTo1YearStandardPeriod_FromFieldWithSuffix()
        {
            var formatter = builder.AppendYears().AppendSuffix(" year").ToFormatter();

            var period = formatter.Parse("1 year");

            Assert.AreEqual(Period.FromYears(1), period);
        }

        [Test]
        public void AppendSuffixAfterYears_ParseThrowsArgument_FromFieldWithoutSuffix()
        {
            var formatter = builder.AppendYears().AppendSuffix(" year").ToFormatter();
            Assert.Throws<ArgumentException>(() => formatter.Parse("1"));
        }

        [Test]
        public void AppendSuffixAfterYears_ParseThrowsArgument_FromSuffixWithoutField()
        {
            var formatter = builder.AppendYears().AppendSuffix(" year").ToFormatter();
            Assert.Throws<ArgumentException>(() => formatter.Parse("year"));
        }

        #endregion

        #region Plural Suffix

        [Test]
        public void AppendSuffixPlural_ThrowsArgumentNull_ForAnyNullOfSuffixStrings()
        {
            Assert.Throws<ArgumentNullException>(() => builder.AppendSuffix("_", null));
            Assert.Throws<ArgumentNullException>(() => builder.AppendSuffix(null, "_"));
            Assert.Throws<ArgumentNullException>(() => builder.AppendSuffix(null, null));
        }

        [Test]
        public void AppendSuffixPlural_ThrowsInvalidOperation_WithoutFieldBefore()
        {
            Assert.Throws<InvalidOperationException>(() => builder.AppendSuffix("_", "_"));
        }

        [Test]
        public void AppendSuffixPluralAfterYears_BuildsCorrectPrinter_For1YearsStandardPeriod()
        {
            var formatter = builder.AppendYears().AppendSuffix(" year", " years").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("1 year", printedValue);
            Assert.AreEqual(6, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void AppendSuffixPluralAfterYears_BuildsCorrectPrinter_ForZeroYearsStandardPeriod()
        {
            var formatter = builder.AppendYears().AppendSuffix(" year", " years").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0 years", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void AppendSuffixPluralAfterHours_BuildsCorrectPrinter_For5HoursStandardPeriod()
        {
            var formatter = builder.AppendHours().AppendSuffix(" hour", " hours").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("5 hours", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void AppendSuffixPluralAfterHours_BuildsCorrectPrinter_ForZeroHoursStandardPeriod()
        {
            var formatter = builder.AppendHours().AppendSuffix(" hour", " hours").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0 hours", printedValue);
            Assert.AreEqual(7, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void AppendSuffixPluralAfterYears_ParseTo1YearStandardPeriod_FromFieldWithSuffix()
        {
            var formatter = builder.AppendYears().AppendSuffix(" year", " years").ToFormatter();

            var period = formatter.Parse("1 year");

            Assert.AreEqual(Period.FromYears(1), period);
        }

        [Test]
        public void AppendSuffixPluralAfterYears_ParseThrowsArgument_FromFieldWithoutSuffix()
        {
            var formatter = builder.AppendYears().AppendSuffix(" year", " years").ToFormatter();
            Assert.Throws<ArgumentException>(() => formatter.Parse("1"));
        }

        [Test]
        public void AppendSuffixPluralAfterYears_ParseThrowsArgument_FromSuffixWithoutField()
        {
            var formatter = builder.AppendYears().AppendSuffix(" year", " years").ToFormatter();
            Assert.Throws<ArgumentException>(() => formatter.Parse("year"));
        }
        #endregion

        #region Prefix And Suffix Together

        [Test]
        public void AppendPrefixAndSuffixOnYears_BuildsCorrectPrinter_For1YearsStandardPeriod()
        {
            var formatter = builder.AppendPrefix("P").AppendYears().AppendSuffix("Y").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("P1Y", printedValue);
            Assert.AreEqual(3, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixAndSuffixOnYears_BuildsCorrectPrinter_ForZeroYearsStandardPeriod()
        {
            var formatter = builder.AppendPrefix("P").AppendYears().AppendSuffix("Y").ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("P0Y", printedValue);
            Assert.AreEqual(3, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        [Test]
        public void AppendPrefixAndSuffixOnYears_ParsesTo1YearStandardPeriod_FromFullString()
        {
            var formatter = builder.AppendPrefix("P").AppendYears().AppendSuffix("Y").ToFormatter();

            var period = formatter.Parse("P1Y");

            Assert.AreEqual(Period.FromYears(1), period);
        }

        #endregion

    }
}
