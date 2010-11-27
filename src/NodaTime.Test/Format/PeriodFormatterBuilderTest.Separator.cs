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

using System.IO;
using NodaTime.Format;
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public partial class PeriodFormatterBuilderTest
    {
        private class SeparatorBuilder
        {
            private string text;
            private string finalText;
            private string[] variants;

            public SeparatorBuilder()
            {
                text = "A";
                finalText = "AA";
                variants = null;
                UseBefore = true;
                UseAfter = true;
            }

            public bool UseBefore { get; set; }

            public bool UseAfter { get; set; }

            public PeriodFormatterBuilder.Separator Build()
            {
                PeriodFormatterBuilder.FieldFormatter[] fieldFormatters = new PeriodFormatterBuilder.FieldFormatter[2];
                var beforeFormatter = new PeriodFormatterBuilder.FieldFormatter(-1, PeriodFormatterBuilder.PrintZeroSetting.RarelyLast, 10, false,
                                                                                PeriodFormatterBuilder.FormatterDurationFieldType.Years, fieldFormatters, null,
                                                                                null);
                var afterFormatter = new PeriodFormatterBuilder.FieldFormatter(-1, PeriodFormatterBuilder.PrintZeroSetting.RarelyLast, 10, false,
                                                                               PeriodFormatterBuilder.FormatterDurationFieldType.Months, fieldFormatters, null,
                                                                               null);
                fieldFormatters[0] = beforeFormatter;
                fieldFormatters[1] = afterFormatter;

                var separator = new PeriodFormatterBuilder.Separator(text, finalText, variants, beforeFormatter, beforeFormatter, UseBefore, UseAfter);
                separator.Finish(afterFormatter, afterFormatter);
                return separator;
            }
        }

        private object[] SeparatorPrintTestData = {
            new TestCaseData(true, true, new Period(1, 2, 0, 0, 0, 0, 0, 0), "1AA2").SetName("before:true; after:true; bothFieldsArePrinted"),
            new TestCaseData(true, true, new Period(0, 0, 0, 3, 0, 0, 0, 0, PeriodType.Days), "").SetName("before:true; after:true; bothFieldAreNotPrinted"),
            new TestCaseData(true, true, new Period(0, 4, 0, 0, 0, 0, 0, 0, PeriodType.Months), "4").SetName("before:true; after:true; afterFieldIsOnlyPrinted")
            ,
            new TestCaseData(true, true, new Period(5, 0, 0, 0, 0, 0, 0, 0, PeriodType.Years), "5").SetName("before:true; after:true; beforeFieldIsOnlyPrinted")
            , new TestCaseData(true, false, new Period(1, 2, 0, 0, 0, 0, 0, 0), "1A2").SetName("before:true; after:false; bothFieldsArePrinted"),
            new TestCaseData(true, false, new Period(0, 0, 0, 3, 0, 0, 0, 0, PeriodType.Days), "").SetName("before:true; after:false; bothFieldAreNotPrinted"),
            new TestCaseData(true, false, new Period(0, 4, 0, 0, 0, 0, 0, 0, PeriodType.Months), "4").SetName(
                "before:true; after:false; afterFieldIsOnlyPrinted"),
            new TestCaseData(true, false, new Period(5, 0, 0, 0, 0, 0, 0, 0, PeriodType.Years), "5A").SetName(
                "before:true; after:false; beforeFieldIsOnlyPrinted"),
            new TestCaseData(false, true, new Period(1, 2, 0, 0, 0, 0, 0, 0), "1AA2").SetName("before:false; after:true; bothFieldsArePrinted"),
            new TestCaseData(false, true, new Period(0, 0, 0, 3, 0, 0, 0, 0, PeriodType.Days), "").SetName("before:false; after:true; bothFieldAreNotPrinted"),
            new TestCaseData(false, true, new Period(0, 4, 0, 0, 0, 0, 0, 0, PeriodType.Months), "AA4").SetName(
                "before:false; after:true; afterFieldIsOnlyPrinted"),
            new TestCaseData(false, true, new Period(5, 0, 0, 0, 0, 0, 0, 0, PeriodType.Years), "5").SetName(
                "before:false; after:true; beforeFieldIsOnlyPrinted"),
            new TestCaseData(false, false, new Period(1, 2, 0, 0, 0, 0, 0, 0), "12").SetName("before:false; after:false; bothFieldsArePrinted"),
            new TestCaseData(false, false, new Period(0, 0, 0, 3, 0, 0, 0, 0, PeriodType.Days), "").SetName("before:false; after:false; bothFieldAreNotPrinted")
            ,
            new TestCaseData(false, false, new Period(0, 4, 0, 0, 0, 0, 0, 0, PeriodType.Months), "4").SetName(
                "before:false; after:false; afterFieldIsOnlyPrinted"),
            new TestCaseData(false, false, new Period(5, 0, 0, 0, 0, 0, 0, 0, PeriodType.Years), "5").SetName(
                "before:false; after:false; beforeFieldIsOnlyPrinted"),
        };

        [Test]
        [TestCaseSource("SeparatorPrintTestData")]
        public void Separator_Prints(bool useBefore, bool useAfter, IPeriod period, string periodText)
        {
            var separator = new SeparatorBuilder() { UseBefore = useBefore, UseAfter = useAfter }.Build();
            var writer = new StringWriter();

            separator.PrintTo(writer, period, null);

            Assert.That(writer.ToString(), Is.EqualTo(periodText));
        }

        //[Test]
        //public void AppendSeparatorBetweenYearsAndHours_BuildsCorrectPrinter_ForStandardPeriod()
        //{
        //    var formatter = builder.AppendYears().AppendSeparator("T").AppendHours().ToFormatter();

        //    var printer = formatter.Printer;
        //    var printedValue = formatter.Print(standardPeriodFull);

        //    Assert.AreEqual("1T5", printedValue);
        //    Assert.AreEqual(3, printer.CalculatePrintedLength(standardPeriodFull, null));
        //    Assert.AreEqual(2, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        //}

        //[Test]
        //public void AppendSeparatorBetweenYearsAndHours_BuildsCorrectPrinter_ForTimePeriod()
        //{
        //    var formatter = builder.AppendYears().AppendSeparator("T").AppendHours().ToFormatter();

        //    var printer = formatter.Printer;
        //    var printedValue = formatter.Print(timePeriod);

        //    Assert.AreEqual("5", printedValue);
        //    Assert.AreEqual(1, printer.CalculatePrintedLength(timePeriod, null));
        //    Assert.AreEqual(1, printer.CountFieldsToPrint(timePeriod, int.MaxValue, null));
        //}

        //[Test]
        //public void AppendSeparatorBetweenYearsAndHours_BuildsCorrectPrinter_ForDatePeriod()
        //{
        //    var formatter = builder.AppendYears().AppendSeparator("T").AppendHours().ToFormatter();

        //    var printer = formatter.Printer;
        //    var printedValue = formatter.Print(datePeriod);

        //    Assert.AreEqual("1", printedValue);
        //    Assert.AreEqual(1, printer.CalculatePrintedLength(datePeriod, null));
        //    Assert.AreEqual(1, printer.CountFieldsToPrint(datePeriod, int.MaxValue, null));
        //}

        //[Test]
        //public void AppendSeparatorBetweenYearsAndHours_ParsesTo1yesr5MonthsStandardPeriod_FromFieldsWithSeparator()
        //{
        //    var formatter = builder.AppendYears().AppendSeparator("T").AppendHours().ToFormatter();

        //    var period = formatter.Parse("1T5");

        //    Assert.AreEqual(Period.FromYears(1).WithHours(5), period);
        //}

        //#region FinalText

        //[Test]
        //public void AppendSeparatorFinalText_BuildsCorrectPrinter_ForStandardPeriod()
        //{
        //    var formatter = builder
        //        .AppendYears().AppendSeparator(", ", " and ")
        //        .AppendHours().AppendSeparator(", ", " and ")
        //        .AppendMinutes().AppendSeparator(", ", " and ")
        //        .ToFormatter();

        //    var printer = formatter.Printer;
        //    var printedValue = formatter.Print(standardPeriodFull);

        //    Assert.AreEqual("1, 5 and 6", printedValue);
        //    Assert.AreEqual(10, printer.CalculatePrintedLength(standardPeriodFull, null));
        //    Assert.AreEqual(3, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        //}

        //[Test]
        //public void AppendSeparatorFinalText_BuildsCorrectPrinter_ForTimePeriod()
        //{
        //    var formatter = builder
        //        .AppendYears().AppendSeparator(", ", " and ")
        //        .AppendHours().AppendSeparator(", ", " and ")
        //        .AppendMinutes().AppendSeparator(", ", " and ")
        //        .ToFormatter();

        //    var printer = formatter.Printer;
        //    var printedValue = formatter.Print(timePeriod);

        //    Assert.AreEqual("5 and 6", printedValue);
        //    Assert.AreEqual(7, printer.CalculatePrintedLength(timePeriod, null));
        //    Assert.AreEqual(2, printer.CountFieldsToPrint(timePeriod, int.MaxValue, null));
        //}

        //[Test]
        //public void AppendSeparatorFinalText_BuildsCorrectPrinter_ForDatePeriod()
        //{
        //    var formatter = builder
        //        .AppendYears().AppendSeparator(", ", " and ")
        //        .AppendHours().AppendSeparator(", ", " and ")
        //        .AppendMinutes().AppendSeparator(", ", " and ")
        //        .ToFormatter();

        //    var printer = formatter.Printer;
        //    var printedValue = formatter.Print(datePeriod);

        //    Assert.AreEqual("1", printedValue);
        //    Assert.AreEqual(1, printer.CalculatePrintedLength(datePeriod, null));
        //    Assert.AreEqual(1, printer.CountFieldsToPrint(datePeriod, int.MaxValue, null));
        //}

        //#endregion

        //#region FieldsAfter

        //[Test]
        //public void AppendSeparatorIfFieldsAfter_BuildsCorrectPrinter_ForStandardPeriod()
        //{
        //    var formatter = builder
        //        .AppendYears().AppendSeparatorIfFieldsAfter("T")
        //        .AppendHours()
        //        .ToFormatter();

        //    var printer = formatter.Printer;
        //    var printedValue = formatter.Print(standardPeriodFull);

        //    Assert.AreEqual("1T5", printedValue);
        //    Assert.AreEqual(3, printer.CalculatePrintedLength(standardPeriodFull, null));
        //    Assert.AreEqual(2, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        //}

        //[Test]
        //public void AppendSeparatorIfFieldsAfter_BuildsCorrectPrinter_ForTimePeriod()
        //{
        //    var formatter = builder
        //        .AppendYears().AppendSeparatorIfFieldsAfter("T")
        //        .AppendHours()
        //        .ToFormatter();

        //    var printer = formatter.Printer;
        //    var printedValue = formatter.Print(timePeriod);

        //    Assert.AreEqual("T5", printedValue);
        //    Assert.AreEqual(2, printer.CalculatePrintedLength(timePeriod, null));
        //    Assert.AreEqual(1, printer.CountFieldsToPrint(timePeriod, int.MaxValue, null));
        //}

        //[Test]
        //public void AppendSeparatorIfFieldsAfter_BuildsCorrectPrinter_ForDatePeriod()
        //{
        //    var formatter = builder
        //        .AppendYears().AppendSeparatorIfFieldsAfter("T")
        //        .AppendHours()
        //        .ToFormatter();

        //    var printer = formatter.Printer;
        //    var printedValue = formatter.Print(datePeriod);

        //    Assert.AreEqual("1", printedValue);
        //    Assert.AreEqual(1, printer.CalculatePrintedLength(datePeriod, null));
        //    Assert.AreEqual(1, printer.CountFieldsToPrint(datePeriod, int.MaxValue, null));
        //}

        //#endregion

        //#region FieldsBefore

        //[Test]
        //public void AppendSeparatorIfFieldsBefore_BuildsCorrectPrinter_ForStandardPeriod()
        //{
        //    var formatter = builder
        //        .AppendYears().AppendSeparatorIfFieldsBefore("T")
        //        .AppendHours()
        //        .ToFormatter();

        //    var printer = formatter.Printer;
        //    var printedValue = formatter.Print(standardPeriodFull);

        //    Assert.AreEqual("1T5", printedValue);
        //    Assert.AreEqual(3, printer.CalculatePrintedLength(standardPeriodFull, null));
        //    Assert.AreEqual(2, printer.CountFieldsToPrint(standardPeriodFull, int.MaxValue, null));
        //}

        //[Test]
        //public void AppendSeparatorIfFieldsBefore_BuildsCorrectPrinter_ForTimePeriod()
        //{
        //    var formatter = builder
        //        .AppendYears().AppendSeparatorIfFieldsBefore("T")
        //        .AppendHours()
        //        .ToFormatter();

        //    var printer = formatter.Printer;
        //    var printedValue = formatter.Print(timePeriod);

        //    Assert.AreEqual("5", printedValue);
        //    Assert.AreEqual(1, printer.CalculatePrintedLength(timePeriod, null));
        //    Assert.AreEqual(1, printer.CountFieldsToPrint(timePeriod, int.MaxValue, null));
        //}

        //[Test]
        //public void AppendSeparatorIfFieldsBefore_BuildsCorrectPrinter_ForDatePeriod()
        //{
        //    var formatter = builder
        //        .AppendYears().AppendSeparatorIfFieldsBefore("T")
        //        .AppendHours()
        //        .ToFormatter();

        //    var printer = formatter.Printer;
        //    var printedValue = formatter.Print(datePeriod);

        //    Assert.AreEqual("1T", printedValue);
        //    Assert.AreEqual(2, printer.CalculatePrintedLength(datePeriod, null));
        //    Assert.AreEqual(1, printer.CountFieldsToPrint(datePeriod, int.MaxValue, null));
        //}

        //#endregion
    }
}