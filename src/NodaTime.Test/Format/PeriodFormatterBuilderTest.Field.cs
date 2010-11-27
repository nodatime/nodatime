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
            private bool rejectSignedValues = false;
            private PeriodFormatterBuilder.FieldFormatter[] fieldFormatters = new PeriodFormatterBuilder.FieldFormatter[11];

            public int MinimumPrintedDigits { get; set; }
            public int MaximumParsedDigits { get; set; }

            public PeriodFormatterBuilder.FormatterDurationFieldType FieldType { get; set; }
            public PeriodFormatterBuilder.PrintZeroSetting PrintZero { get; set; }
            public PeriodFormatterBuilder.IPeriodFieldAffix Prefix { get; set; }
            public PeriodFormatterBuilder.IPeriodFieldAffix Suffix { get; set; }

            public FieldFormatterBuilder()
            {
                MinimumPrintedDigits = 1;
                MaximumParsedDigits = 10;
                FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.Days;
                PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyLast;
            }

            public PeriodFormatterBuilder.FieldFormatter Build()
            {
                var fieldFormatter = new PeriodFormatterBuilder.FieldFormatter(MinimumPrintedDigits, PrintZero, MaximumParsedDigits, rejectSignedValues,
                                                                               FieldType, fieldFormatters, Prefix, Suffix);
                fieldFormatters[(int)FieldType] = fieldFormatter;
                return fieldFormatter;
            }
        }

        #region FieldFormatter

        #region Print
        [Test]
        public void FieldFormatter_PrintsFieldValue()
        {
            var fieldFormatter = new FieldFormatterBuilder().Build();
            var writer = new StringWriter();
            var period = Period.FromDays(42);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("42"));
        }
        #endregion

        #region Print:MinimumPrintedDigits
        private object[] PrintMinimumDigitsTestData = {
            new TestCaseData(5, Period.FromDays(42), "00042").SetName("5;42 => 00042"),
            new TestCaseData(3, Period.FromDays(4256), "4256").SetName("3;4256 => 4256"),
            new TestCaseData(0, Period.FromDays(42), "42").SetName("0;42 => 42(neutral)"),
            new TestCaseData(-2, Period.FromDays(42), "42").SetName("-2;42 => 42(neutral)"),
            new TestCaseData(5, Period.FromDays(-42), "-00042").SetName("5;-42 => -00042"),
        };

        [Test]
        [TestCaseSource("PrintMinimumDigitsTestData")]
        public void FieldFormatter_PrintsPaddedFieldValue_WithMinimumDigitsSet(int mimimumDigits, Period periodValue, string periodText)
        {
            var fieldFormatter = new FieldFormatterBuilder() { MinimumPrintedDigits = mimimumDigits }.Build();
            var writer = new StringWriter();

            fieldFormatter.PrintTo(writer, periodValue, null);

            Assert.That(writer.ToString(), Is.EqualTo(periodText));
        }
        #endregion

        #region Print:Supported/Unsupported field type
        [Test]
        public void FieldFormatter_PrintsNothing_IfFieldTypeIsNotSupported()
        {
            var fieldFormatter = new FieldFormatterBuilder().Build();

            var writer = new StringWriter();
            var period = Years.One;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo(""));
        }

        [Test]
        public void FieldFormatter_PrintsValue_IfFieldTypeIsNotSupportedButPrintAlwaysSet()
        {
            var fieldFormatter = new FieldFormatterBuilder() { PrintZero = PeriodFormatterBuilder.PrintZeroSetting.Always, }.Build();
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
            var fieldFormatter = new FieldFormatterBuilder() { PrintZero = PeriodFormatterBuilder.PrintZeroSetting.Never }.Build();

            var writer = new StringWriter();
            var period = Days.Zero;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo(""));
        }
        #endregion

        #region Print:Prefix/Suffix
        [Test]
        public void FieldFormatter_PrintsFieldValueWithPrefix_IfPrefixIsSpecified()
        {
            var fieldFormatter = new FieldFormatterBuilder() { Prefix = new PeriodFormatterBuilder.SimpleAffix("Day:") }.Build();
            var writer = new StringWriter();
            var period = Period.FromDays(42);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("Day:42"));
        }

        [Test]
        public void FieldFormatter_PrintsFieldValueWithSuffix_IfSuffixIsSpecified()
        {
            var fieldFormatter = new FieldFormatterBuilder() { Suffix = new PeriodFormatterBuilder.SimpleAffix("days") }.Build();
            var writer = new StringWriter();
            var period = Period.FromDays(42);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("42days"));
        }

        [Test]
        public void FieldFormatter_PrintsNothing_IfFieldTypeIsNotSupportedButPrefixAndSuffixExist()
        {
            var fieldFormatter =
                new FieldFormatterBuilder() { Prefix = new PeriodFormatterBuilder.SimpleAffix("Day:"), Suffix = new PeriodFormatterBuilder.SimpleAffix("days") }
                    .Build();

            var writer = new StringWriter();
            var period = Years.One;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo(""));
        }
        #endregion

        #region Print:SecondsMilliseconds
        [Test]
        public void FieldFormatter_PrintsPaddedCombinedValue_ForSecondsMillisecondsFieldTypeAndMillisecondsIsZero()
        {
            var fieldFormatter = new FieldFormatterBuilder() { FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(1).WithMilliseconds(0);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("1.000"));
        }

        [Test]
        public void FieldFormatter_PrintsPaddedCombinedValue_ForSecondsMillisecondsFieldTypeAndMillisecondsLessThan1000()
        {
            var fieldFormatter = new FieldFormatterBuilder() { FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(1).WithMilliseconds(2);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("1.002"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndMillisecondsGreaterThan1000()
        {
            var fieldFormatter = new FieldFormatterBuilder() { FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(1).WithMilliseconds(2345);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("3.345"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndNegativeSecondsAndMilliseconds()
        {
            var fieldFormatter = new FieldFormatterBuilder() { FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(-1).WithMilliseconds(-2);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("-1.002"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndNegativeSecondsButPositiveMillisecondsZeroSeconds()
        {
            var fieldFormatter = new FieldFormatterBuilder() { FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(-1).WithMilliseconds(2);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("-0.998"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndNegativeSecondsButPositiveMillisecondsNonZeroSeconds()
        {
            var fieldFormatter = new FieldFormatterBuilder() { FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(-7).WithMilliseconds(5);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("-6.995"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndPositiveSecondsButNegativeMilliseconds()
        {
            var fieldFormatter = new FieldFormatterBuilder() { FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(1).WithMilliseconds(-2);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0.998"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndZeroSecondsAndNotZeroMilliseconds()
        {
            var fieldFormatter = new FieldFormatterBuilder() { FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds }.Build();
            var writer = new StringWriter();
            var period = Period.FromMilliseconds(12);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0.012"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndUnsupportedSeconds()
        {
            var fieldFormatter = new FieldFormatterBuilder() { FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds }.Build();
            var writer = new StringWriter();
            var period = new Period(0, 0, 0, 0, 0, 0, 0, 2345, PeriodType.Milliseconds);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("2.345"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndUnsupportedMilliSeconds()
        {
            var fieldFormatter = new FieldFormatterBuilder() { FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds }.Build();
            var writer = new StringWriter();
            var period = new Period(0, 0, 0, 0, 0, 0, 6, 0, PeriodType.Seconds);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("6.000"));
        }

        [Test]
        public void FieldFormatter_PrintsNothing_ForSecondsMillisecondsFieldTypeAndBothUnsupported()
        {
            var fieldFormatter = new FieldFormatterBuilder() { FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds }.Build();
            var writer = new StringWriter();
            var period = Years.Three;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo(""));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsFieldTypeAndZeroPeriod()
        {
            var fieldFormatter = new FieldFormatterBuilder() { FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds }.Build();
            var writer = new StringWriter();
            var period = Period.Zero;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0.000"));
        }

        [Test]
        public void FieldFormatter_PrintsOnlySeconds_ForSecondsMillisecondsOptionalFieldTypeAndMillisecondsIsZero()
        {
            var fieldFormatter =
                new FieldFormatterBuilder() { FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMillisecondsOptional }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(1).WithMilliseconds(0);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("1"));
        }

        [Test]
        public void FieldFormatter_PrintsCombinedValue_ForSecondsMillisecondsOptionalFieldTypeAndMillisecondsIsNotZero()
        {
            var fieldFormatter =
                new FieldFormatterBuilder() { FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMillisecondsOptional }.Build();
            var writer = new StringWriter();
            var period = Period.FromSeconds(1).WithMilliseconds(234);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("1.234"));
        }
        #endregion

        #region Print:RarelyLast
        [Test]
        public void FieldFormatter_PrintsValue_IfFieldTypeIsSupportedAndPeriodIsZeroAndPrintsRarelyLastAndNoOtherFormattersExists()
        {
            var fieldFormatter = new FieldFormatterBuilder() { PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyLast }.Build();

            var writer = new StringWriter();
            var period = Days.Zero;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void FieldFormatter_PrintsValue_IfFieldTypeIsSupportedAndPeriodIsZeroAndPrintsRarelyLastAndOtherFormattersExistsButNotSupported()
        {
            var builder = new FieldFormatterBuilder() { PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyLast };

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
            var builder = new FieldFormatterBuilder() { PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyLast };

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
            var builder = new FieldFormatterBuilder() { PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyLast };

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
            var fieldFormatter = new FieldFormatterBuilder() { PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyLast }.Build();

            var writer = new StringWriter();
            var period = Period.FromDays(0).WithHours(1);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo(""));
        }
        #endregion

        #region Print:RarelyFirst
        public void FieldFormatter_PrintsValue_IfFieldTypeIsSupportedAndPeriodIsZeroAndPrintsRarelyFirstAndNoOtherFormattersExists()
        {
            var fieldFormatter = new FieldFormatterBuilder() { PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyFirst }.Build();

            var writer = new StringWriter();
            var period = Days.Zero;

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void FieldFormatter_PrintsValue_IfFieldTypeIsSupportedAndPeriodIsZeroAndPrintsRarelyFirstAndOtherFormattersExistsButNotSupported()
        {
            var builder = new FieldFormatterBuilder() { PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyFirst };

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
            var builder = new FieldFormatterBuilder() { PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyFirst };

            var fieldFormatter = builder.Build();
            builder.FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.Hours;
            builder.Build();

            var writer = new StringWriter();
            var period = Period.FromDays(0).WithHours(0);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void FieldFormatter_PrintsNothing_IfFieldTypeIsSupportedAndPeriodIsZeroAndPrintsRarelyFirstAndOtherFormattersExistsAndIsSupportedButGreaterByType
            ()
        {
            var builder = new FieldFormatterBuilder() { PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyFirst };

            var fieldFormatter = builder.Build();
            builder.FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.Years;
            builder.Build();

            var writer = new StringWriter();
            var period = Period.FromDays(0).WithYears(0);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo(""));
        }

        [Test]
        public void
            FieldFormatter_PrintsNothing_IfFieldTypeIsSupportedAndPeriodIsNotZeroAndPrintsRarelyFirstAndOtherFormattersExistsAndIsSupportedButGreaterByType()
        {
            var fieldFormatter = new FieldFormatterBuilder() { PrintZero = PeriodFormatterBuilder.PrintZeroSetting.RarelyFirst }.Build();

            var writer = new StringWriter();
            var period = Period.FromDays(0).WithYears(1);

            fieldFormatter.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo(""));
        }
        #endregion

        #region Parse
        [Test]
        public void FieldFormatter_ParsesText()
        {
            var fieldFormatter = new FieldFormatterBuilder().Build();
            var periodText = "25";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.EqualTo(2));
            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.FromDays(25)));
        }

        [Test]
        public void FieldFormatter_ParsesText_AndStopAtFirstNonDigitSymbol()
        {
            var fieldFormatter = new FieldFormatterBuilder().Build();
            var periodText = "123a";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.EqualTo(3));
            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.FromDays(123)));
        }

        [Test]
        public void FieldFormatter_Fails_IfNoDigitsWithinTheText()
        {
            var fieldFormatter = new FieldFormatterBuilder().Build();
            var periodText = "abc";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.LessThan(0));
            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.Zero));
        }

        [Test]
        public void FieldFormatter_ParsesText_WithZeroChar()
        {
            var fieldFormatter = new FieldFormatterBuilder().Build();
            var periodText = "0";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.EqualTo(1));
            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.FromDays(0)));
        }

        [Test]
        public void FieldFormatter_SkipsItself_IfFieldTypeIsUnsupported()
        {
            var fieldFormatter = new FieldFormatterBuilder().Build();
            var periodText = "25";
            var builder = new PeriodBuilder(PeriodType.Minutes);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.EqualTo(0));
            Assert.That(builder.ToPeriod(), Is.EqualTo(new Period(new int[] { 0 }, PeriodType.Minutes)));
        }

        [Test]
        public void FieldFormatter_Fail_IfFieldTypeIsUnsupportedAndPrintZeroAlways()
        {
            var fieldFormatter = new FieldFormatterBuilder() { PrintZero = PeriodFormatterBuilder.PrintZeroSetting.Always }.Build();
            var periodText = "25";
            var builder = new PeriodBuilder(PeriodType.Minutes);

            Assert.That(() => fieldFormatter.Parse(periodText, 0, builder, null), Throws.InstanceOf<NotSupportedException>());
        }
        #endregion

        #region Parse:Position/MaximumParsedDigits
        [Test]
        public void FieldFormatter_ParsesText_LimitedByPositionAndMaximumParsedDigits()
        {
            var fieldFormatter = new FieldFormatterBuilder() { MaximumParsedDigits = 3 }.Build();
            var periodText = "123456789";
            int startPosition = 4;
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, startPosition, builder, null);

            Assert.That(newPosition, Is.EqualTo(7));
            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.FromDays(567)));
        }

        [Test]
        public void FieldFormatter_PassThrough_IfPositionNotLessThanTextLength()
        {
            var fieldFormatter = new FieldFormatterBuilder().Build();
            var periodText = "123";
            int startPosition = 4;

            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, startPosition, builder, null);

            Assert.That(newPosition, Is.EqualTo(4));
        }

        [Test]
        public void FieldFormatter_Fails_IfPositionNotLessThanTextLengthAndPrintZeroAlwaysSet()
        {
            var fieldFormatter = new FieldFormatterBuilder() { PrintZero = PeriodFormatterBuilder.PrintZeroSetting.Always }.Build();
            var periodText = "123";
            int startPosition = 4;

            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, startPosition, builder, null);

            Assert.That(newPosition, Is.LessThan(0));
        }
        #endregion

        #region Parse:Prefix/Suffix
        [Test]
        public void FieldFormatter_ParsesPrefixAndValue_IfPrefixIsSpecified()
        {
            var fieldFormatter = new FieldFormatterBuilder() { Prefix = new PeriodFormatterBuilder.SimpleAffix("Day:") }.Build();
            var periodText = "Day:25";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.EqualTo(periodText.Length));
            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.FromDays(25)));
        }

        [Test]
        public void FieldFormatter_ParsesValueAndSuffix_IfSuffixIsSpecified()
        {
            var fieldFormatter = new FieldFormatterBuilder() { Suffix = new PeriodFormatterBuilder.SimpleAffix(" days") }.Build();
            var periodText = "25 days";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.EqualTo(periodText.Length));
            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.FromDays(25)));
        }

        [Test]
        public void FieldFormatter_ParsesPrefixAndValueAndSuffix_IfPrefixAndSuffixAreSpecified()
        {
            var fieldFormatter =
                new FieldFormatterBuilder()
                { Prefix = new PeriodFormatterBuilder.SimpleAffix("Day:"), Suffix = new PeriodFormatterBuilder.SimpleAffix(" days") }.Build();
            var periodText = "Day:47 days";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.EqualTo(11));
            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.FromDays(47)));
        }

        [Test]
        public void FieldFormatter_PassThrough_IfPrefixIsSpecifiedButNotInText()
        {
            var fieldFormatter = new FieldFormatterBuilder() { Prefix = new PeriodFormatterBuilder.SimpleAffix("Day:") }.Build();
            var periodText = "47 days";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.EqualTo(0));
            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.Zero));
        }

        [Test]
        public void FieldFormatter_PassThrough_IfSuffixIsSpecifiedButNotInText()
        {
            var fieldFormatter = new FieldFormatterBuilder() { Suffix = new PeriodFormatterBuilder.SimpleAffix(" days") }.Build();
            var periodText = "47blah-blah";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.EqualTo(0));
            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.Zero));
        }

        [Test]
        public void FieldFormatter_Fails_IfPrefixIsSpecifiedButNotInTextAndPrintZeroAlwaysSet()
        {
            var fieldFormatter =
                new FieldFormatterBuilder()
                { Prefix = new PeriodFormatterBuilder.SimpleAffix("Day:"), PrintZero = PeriodFormatterBuilder.PrintZeroSetting.Always }.Build();
            var periodText = "47 days";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.LessThan(0));
            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.Zero));
        }

        [Test]
        public void FieldFormatter_Fails_IfTextContainsPrefixButNotValue()
        {
            var fieldFormatter = new FieldFormatterBuilder() { Prefix = new PeriodFormatterBuilder.SimpleAffix("Day:"), }.Build();
            var periodText = "Day:";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.LessThan(0));
            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.Zero));
        }

        [Test]
        public void FieldFormatter_Fails_IfSuffixIsSpecifiedButNotInTextAndPrintZeroAlwaysSet()
        {
            var fieldFormatter =
                new FieldFormatterBuilder()
                { Suffix = new PeriodFormatterBuilder.SimpleAffix(" days"), PrintZero = PeriodFormatterBuilder.PrintZeroSetting.Always }.Build();
            var periodText = "47blah-blah";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.LessThan(0));
            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.Zero));
        }

        [Test]
        public void FieldFormatter_Fails_IfPrefixAndSuffixAreSpecifiedAndPrefixIsParsedButSuffixIsNot()
        {
            var fieldFormatter =
                new FieldFormatterBuilder()
                { Prefix = new PeriodFormatterBuilder.SimpleAffix("Day:"), Suffix = new PeriodFormatterBuilder.SimpleAffix(" days") }.Build();
            var periodText = "Day:47 oops";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.LessThan(0));
            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.Zero));
        }
        #endregion

        #region Parse:Signs
        [Test]
        public void FieldFormatter_ParsesText_WithLeadingPlusSign()
        {
            var fieldFormatter = new FieldFormatterBuilder().Build();
            var periodText = "+25";
            var builder = new PeriodBuilder(PeriodType.Standard);

            fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.FromDays(25)));
        }

        [Test]
        public void FieldFormatter_Fail_WithLeadingPlusSignAndNoDigitFollow()
        {
            var fieldFormatter = new FieldFormatterBuilder().Build();
            var periodText = "+$25";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.LessThan(0));
        }

        [Test]
        public void FieldFormatter_Fail_WithLeadingPlusSignAndMaximumParsedDigitsExceed()
        {
            var fieldFormatter = new FieldFormatterBuilder() { MaximumParsedDigits = 1 }.Build();
            var periodText = "+100";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.LessThan(0));
        }

        [Test]
        public void FieldFormatter_ParsesText_WithLeadingMinusSign()
        {
            var fieldFormatter = new FieldFormatterBuilder().Build();
            var periodText = "-25";
            var builder = new PeriodBuilder(PeriodType.Standard);

            fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.FromDays(-25)));
        }

        [Test]
        public void FieldFormatter_ParsesText_WithLeadingMinusSignAndMaximumParsedDigitsIsUnaffected()
        {
            var fieldFormatter = new FieldFormatterBuilder() { MaximumParsedDigits = 2 }.Build();
            var periodText = "-12345678";
            var builder = new PeriodBuilder(PeriodType.Standard);

            fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(builder.ToPeriod(), Is.EqualTo(Period.FromDays(-12)));
        }

        [Test]
        public void FieldFormatter_Fail_WithLeadingMinusSignAndNoDigitFollow()
        {
            var fieldFormatter = new FieldFormatterBuilder().Build();
            var periodText = "-+25";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.LessThan(0));
        }

        [Test]
        public void FieldFormatter_Fail_WithLeadingMinusSignAndMaximumParsedDigitsExceed()
        {
            var fieldFormatter = new FieldFormatterBuilder() { MaximumParsedDigits = 1 }.Build();
            var periodText = "-200";
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.LessThan(0));
        }
        #endregion

        #region Parse: Seconds/Milliseconds
        private object[] ParseSecondsMillisecondsTestData = {
            new TestCaseData("1", Period.FromSeconds(1)).SetName("1"),
            new TestCaseData("2.", Period.FromSeconds(2)).SetName("2."), new TestCaseData("3.4", Period.FromSeconds(3).WithMilliseconds(400)).SetName("3.4"),
            new TestCaseData("5.67", Period.FromSeconds(5).WithMilliseconds(670)).SetName("5.67"),
            new TestCaseData("8.901", Period.FromSeconds(8).WithMilliseconds(901)).SetName("8.901"),
            new TestCaseData("1.234567", Period.FromSeconds(1).WithMilliseconds(234)).SetName("1.234567")
        };

        [Test]
        [TestCaseSource("ParseSecondsMillisecondsTestData")]
        public void FieldFormatter_ParsesText_ForSecondsMillisecondsFieldType(string periodText, Period result)
        {
            var fieldFormatter = new FieldFormatterBuilder() { FieldType = PeriodFormatterBuilder.FormatterDurationFieldType.SecondsMilliseconds }.Build();
            var builder = new PeriodBuilder(PeriodType.Standard);

            int newPosition = fieldFormatter.Parse(periodText, 0, builder, null);

            Assert.That(newPosition, Is.EqualTo(periodText.Length));
            Assert.That(builder.ToPeriod(), Is.EqualTo(result));
        }
        #endregion

        #endregion
    }
}