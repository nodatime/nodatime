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
using System.IO;
using NodaTime.Format;
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public partial class PeriodFormatterBuilderTest
    {
        private class FieldFormatterBuilder
        {
            private int maximumParsedDigits = 10;
            private bool rejectSignedValues = false;
            private PeriodFormatterBuilder.FieldFormatter[] fieldFormatters = new PeriodFormatterBuilder.FieldFormatter[11];

            public int MinimumPrintedDigits { get; set; }
            public PeriodFormatterBuilder.FormatterDurationFieldType FieldType { get; set; }
            public PeriodFormatterBuilder.PrintZeroSetting PrintZero { get; set; }
            public PeriodFormatterBuilder.IPeriodFieldAffix Prefix { get; set; }
            public PeriodFormatterBuilder.IPeriodFieldAffix Suffix { get; set; }

            public FieldFormatterBuilder()
            {
                MinimumPrintedDigits = 1;
                FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.Days;
                PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyLast;
            }

            public PeriodFormatterBuilder.FieldFormatter Build()
            {
                var fieldFormatter = new PeriodFormatterBuilder.FieldFormatter(MinimumPrintedDigits, PrintZero, maximumParsedDigits, rejectSignedValues, FieldType, fieldFormatters, Prefix, Suffix);
                fieldFormatters[(int)FieldType] = fieldFormatter;
                return fieldFormatter;
            }
        }

        #region FieldFormatter

        [Test]
        public void FieldFormatter_PrintsFieldValue()
        {
            var fieldFormatter = new FieldFormatterBuilder().Build();
            var writer = new StringWriter();
            var period = Period.FromDays(42);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("42"));
        }

        #region MinimumPrintedDigits

        [Test]
        public void FieldFormatter_PrintsPaddedFieldValue_WithMinimumDigitsSet()
        {
            var fieldFormatter = new FieldFormatterBuilder()
                {
                    MinimumPrintedDigits = 5
                }.Build();
            var writer = new StringWriter();
            var period = Period.FromDays(42);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("00042"));
        }

        [Test]
        public void FieldFormatter_PrintsUnpaddedFieldValue_WithMinimumDigitsIsZero()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                MinimumPrintedDigits = 0
            }.Build();
            var writer = new StringWriter();
            var period = Period.FromDays(42);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("42"));
        }

        [Test]
        public void FieldFormatter_PrintsUnpaddedFieldValue_WithNegativeMinimumDigits()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                MinimumPrintedDigits = -2
            }.Build();
            var writer = new StringWriter();
            var period = Period.FromDays(42);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("42"));
        }

        [Test]
        public void FieldFormatter_PrintsPaddedFieldValue_WithMinimumDigitsSetAndNegativeFieldValue()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                MinimumPrintedDigits = 5
            }.Build();
            var writer = new StringWriter();
            var period = Period.FromDays(-42);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("-00042"));
        }

        #endregion

        #region Supported/Unsupported field type

        [Test]
        public void FieldFormatter_PrintsNothing_IfFieldTypeIsNotSupported()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                Prefix = new PeriodFormatterBuilder.SimpleAffix("Day:"),
                Suffix = new PeriodFormatterBuilder.SimpleAffix("days")
            }
                                    .Build();

            var writer = new StringWriter();
            var period = Years.One;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo(""));
        }

        [Test]
        public void FieldFormatter_PrintsValue_IfFieldTypeIsNotSupportedButPrintAlwaysSet()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                PrintZero = PeriodFormatterBuilder.PrintZeroSetting.Always,
            }
                                    .Build();
            var writer = new StringWriter();
            var period = Years.One;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void FieldFormatter_PrintsFieldValue_IfFieldTypeIsSupportedAndZero()
        {
            var fieldFormatter = new FieldFormatterBuilder().Build();

            var writer = new StringWriter();
            var period = Days.Zero;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void FieldFormatter_PrintsNothing_IfFieldTypeIsSupportedAndValueIsZeroAndPrintsNeverSet()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                PrintZero = PeriodFormatterBuilder.PrintZeroSetting.Never
            }.Build();

            var writer = new StringWriter();
            var period = Days.Zero;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo(""));
        }

        #endregion

        #region Prefix/Suffix

        [Test]
        public void FieldFormatter_PrintsFieldValueWithPrefix_IfPrefixIsSpecified()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                Prefix = new PeriodFormatterBuilder.SimpleAffix("Day:")
            }
                                    .Build();
            var writer = new StringWriter();
            var period = Period.FromDays(42);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("Day:42"));
        }

        [Test]
        public void FieldFormatter_PrintsFieldValueWithSuffix_IfSuffixIsSpecified()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                Suffix = new PeriodFormatterBuilder.SimpleAffix("days")
            }
                                    .Build();
            var writer = new StringWriter();
            var period = Period.FromDays(42);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("42days"));
        }


        #endregion

        #region SecondsMilliseconds

        [Test]
        public void FieldFormatter_PrintsPaddedCombinedValue_ForSecondsMillisecondsFieldTypeAndMillisecondsIsZero()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds
            }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(1).WithMilliseconds(0);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("1.000"));
        }

        [Test]
        public void FieldFormatter_PrintsPaddedCombinedValue_ForSecondsMillisecondsFieldTypeAndMillisecondsLessThan1000()
        {
            var fieldFormatter = new FieldFormatterBuilder()
                {
                    FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds
                }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(1).WithMilliseconds(2);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("1.002"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndMillisecondsGreaterThan1000()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds
            }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(1).WithMilliseconds(2345);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("3.345"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndNegativeSecondsAndMilliseconds()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds
            }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(-1).WithMilliseconds(-2);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("-1.002"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndNegativeSecondsButPositiveMillisecondsZeroSeconds()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds
            }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(-1).WithMilliseconds(2);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("-0.998"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndNegativeSecondsButPositiveMillisecondsNonZeroSeconds()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds
            }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(-7).WithMilliseconds(5);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("-6.995"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndPositiveSecondsButNegativeMilliseconds()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds
            }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(1).WithMilliseconds(-2);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0.998"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndZeroSecondsAndNotZeroMilliseconds()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds
            }.Build();
            var writer = new StringWriter();
            var period = Period.FromMilliseconds(12);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0.012"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndUnsupportedSeconds()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds
            }.Build();
            var writer = new StringWriter();
            var period = new Period(0, 0, 0, 0, 0, 0, 0, 2345, PeriodType.Milliseconds);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("2.345"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndUnsupportedMilliSeconds()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds
            }.Build();
            var writer = new StringWriter();
            var period = new Period(0, 0, 0, 0, 0, 0, 6, 0, PeriodType.Seconds);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("6.000"));
        }

        [Test]
        public void FieldFormatter_PrintsNothing_ForSecondsMillisecondsFieldTypeAndBothUnsupported()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds
            }.Build();
            var writer = new StringWriter();
            var period = Years.Three;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo(""));
        }


        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndZeroPeriod()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds
            }.Build();
            var writer = new StringWriter();
            var period = Period.Zero;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0.000"));
        }

        [Test]
        public void FieldFormatter_PrintsOnlySeconds_ForSecondsMillisecondsOptionalFieldTypeAndMillisecondsIsZero()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMillisecondsOptional
            }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(1).WithMilliseconds(0);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("1"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsOptionalFieldTypeAndMillisecondsIsNotZero()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMillisecondsOptional
            }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(1).WithMilliseconds(234);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("1.234"));
        }

        #endregion

        #region RarelyLast

        [Test]
        public void FieldFormatter_PrintsValue_IfFieldTypeIsSupportedAndPeriodIsZeroAndPrintsRarelyLastAndNoOtherFormattersExists()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyLast
            }.Build();

            var writer = new StringWriter();
            var period = Days.Zero;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void FieldFormatter_PrintsValue_IfFieldTypeIsSupportedAndPeriodIsZeroAndPrintsRarelyLastAndOtherFormattersExistsButNotSupported()
        {
            var builder = new FieldFormatterBuilder()
            {
                PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyLast
            };

            var fieldFormatter = builder.Build();
            builder.FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.Hours;
            builder.Build();

            var writer = new StringWriter();
            var period = Days.Zero;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void FieldFormatter_PrintsValue_IfFieldTypeIsSupportedAndPeriodIsZeroAndPrintsRarelyLastAndOtherFormattersExistsAndIsSupportedAndGreaterByType()
        {
            var builder = new FieldFormatterBuilder()
            {
                PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyLast
            };

            var fieldFormatter = builder.Build();
            builder.FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.Years;
            builder.Build();

            var writer = new StringWriter();
            var period = Period.FromDays(0).WithYears(0);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void FieldFormatter_PrintsNothing_IfFieldTypeIsSupportedAndPeriodIsZeroAndPrintsRarelyLastAndOtherFormattersExistsAndIsSupportedButLessByType()
        {
            var builder = new FieldFormatterBuilder()
            {
                PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyLast
            };

            var fieldFormatter = builder.Build();
            builder.FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.Hours;
            builder.Build();

            var writer = new StringWriter();
            var period = Period.FromDays(0).WithHours(0);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo(""));
        }

        [Test]
        public void FieldFormatter_PrintsNothing_IfValueIsZeroAndPrintRarelyLastButPeriodIsNotZero()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyLast
            }.Build();

            var writer = new StringWriter();
            var period = Period.FromDays(0).WithHours(1);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo(""));
        }

        #endregion

        #region RarelyFirst

        public void FieldFormatter_PrintsValue_IfFieldTypeIsSupportedAndPeriodIsZeroAndPrintsRarelyFirstAndNoOtherFormattersExists()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyFirst
            }.Build();

            var writer = new StringWriter();
            var period = Days.Zero;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void FieldFormatter_PrintsValue_IfFieldTypeIsSupportedAndPeriodIsZeroAndPrintsRarelyFirstAndOtherFormattersExistsButNotSupported()
        {
            var builder = new FieldFormatterBuilder()
            {
                PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyFirst
            };

            var fieldFormatter = builder.Build();
            builder.FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.Years;
            builder.Build();

            var writer = new StringWriter();
            var period = Days.Zero;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void FieldFormatter_PrintsValue_IfFieldTypeIsSupportedAndPeriodIsZeroAndPrintsRarelyFirstAndOtherFormattersExistsAndIsSupportedAndLessByType()
        {
            var builder = new FieldFormatterBuilder()
            {
                PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyFirst
            };

            var fieldFormatter = builder.Build();
            builder.FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.Hours;
            builder.Build();

            var writer = new StringWriter();
            var period = Period.FromDays(0).WithHours(0);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void FieldFormatter_PrintsNothing_IfFieldTypeIsSupportedAndPeriodIsZeroAndPrintsRarelyFirstAndOtherFormattersExistsAndIsSupportedButGreaterByType()
        {
            var builder = new FieldFormatterBuilder()
            {
                PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyFirst
            };

            var fieldFormatter = builder.Build();
            builder.FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.Years;
            builder.Build();

            var writer = new StringWriter();
            var period = Period.FromDays(0).WithYears(0);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo(""));
        }

        [Test]
        public void FieldFormatter_PrintsNothing_IfFieldTypeIsSupportedAndPeriodIsNotZeroAndPrintsRarelyFirstAndOtherFormattersExistsAndIsSupportedButGreaterByType()
        {
            var fieldFormatter = new FieldFormatterBuilder()
            {
                PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyFirst
            }.Build();

            var writer = new StringWriter();
            var period = Period.FromDays(0).WithYears(1);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo(""));
        }

        #endregion

        #endregion

        #region Years

        [Test]
        public void AppendYears_Prints1_For1YearStandardPeriod()
        {
            var formatter = builder.AppendYears().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("1", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));            
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

            Assert.Throws<FormatException>(() => formatter.Parse("--1"));
        }

        [Test]
        public void AppendYears_Prints0_ForZeroYearStandardPeriod()
        {
            var formatter = builder.AppendYears().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
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
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("2", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
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
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
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
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("3", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
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
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
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
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("4", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
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
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
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
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("5", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
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
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
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
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("6", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
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
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
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
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("7", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
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
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("0", printedValue);
            Assert.AreEqual(1, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
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
        public void AppendMilliseconds_Parses8_To8MillisecondsStandardPeriod()
        {
            var formatter = builder.AppendMillis().ToFormatter();

            var parsedPeriod = formatter.Parse("8");

            Assert.AreEqual(Period.FromMilliseconds(8), parsedPeriod);
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
            var printedValue = formatter.Print(standardPeriodFull);

            Assert.AreEqual("008", printedValue);
            Assert.AreEqual(3, printer.CalculatePrintedLength(standardPeriodFull, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        }

        [Test]
        public void AppendMilliseconds3Digit_Prints000_ForZeroMillisecondsStandardPeriod()
        {
            var formatter = builder.AppendMillis3Digit().ToFormatter();

            var printer = formatter.Printer;
            var printedValue = formatter.Print(standardPeriodEmpty);

            Assert.AreEqual("000", printedValue);
            Assert.AreEqual(3, printer.CalculatePrintedLength(standardPeriodEmpty, null));
            Assert.AreEqual(1, printer.CountFieldsToPrint(standardPeriodEmpty, int.MaxValue, null));
        }

        #endregion

        #region SecondsWithMilliseconds

        [Test]
        public void AppendSecondsWithMilliseconds_BuildsCorrectParser_To7SecondsStandardPeriod()
        {
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var parsedPeriod = formatter.Parse("7.000");

            Assert.AreEqual(Period.FromSeconds(7), parsedPeriod);
        }

        [Test]
        public void AppendSecondsWithMilliseconds_BuildsCorrectParser_To7Seconds1MillisecondsStandardPeriod()
        {
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var parsedPeriod = formatter.Parse("7.001");

            Assert.AreEqual(Period.FromSeconds(7).WithMilliseconds(1), parsedPeriod);
        }

        [Test]
        public void AppendSecondsWithMilliseconds_BuildsCorrectParser_To7Seconds999MillisecondsStandardPeriod()
        {
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var parsedPeriod = formatter.Parse("7.999");

            Assert.AreEqual(Period.FromSeconds(7).WithMilliseconds(999), parsedPeriod);
        }

        [Test]
        public void AppendSecondsWithMilliseconds_BuildsCorrectParser_To7Seconds100MillisecondsStandardPeriod()
        {
            var formatter = builder.AppendSecondsWithMillis().ToFormatter();

            var parsedPeriod = formatter.Parse("7.1000");

            //only 3 digits are considering when parsing milliseconds
            Assert.AreEqual(Period.FromSeconds(7).WithMilliseconds(100), parsedPeriod);
        }

        #endregion
    }
}
