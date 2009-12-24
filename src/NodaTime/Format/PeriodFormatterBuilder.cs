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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NodaTime.Fields;
namespace NodaTime.Format
{
    /// <summary>
    /// Factory that creates complex instances of PeriodFormatter via method calls.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Period formatting is performed by the <see cref="PeriodFormatter"/> class.
    /// Three classes provide factory methods to create formatters, and this is one.
    /// The others are <see cref="PeriodFormaterFactory"/> and <see cref="ISOPeriodFormatterFactory"/>.
    /// </para>
    /// <para>
    /// PeriodFormatterBuilder is used for constructing formatters which are then
    /// used to print or parse. The formatters are built by appending specific fields
    /// or other formatters to an instance of this builder.
    /// </para>
    /// <para>
    /// For example, a formatter that prints years and months, like "15 years and 8 months",
    /// can be constructed as follows:
    /// <code>
    /// * PeriodFormatter yearsAndMonths = new PeriodFormatterBuilder()
    ///     .printZeroAlways()
    ///     .appendYears()
    ///     .appendSuffix(" year", " years")
    ///     .appendSeparator(" and ")
    ///     .printZeroRarely()
    ///     .appendMonths()
    ///     .appendSuffix(" month", " months")
    ///     .toFormatter();
    /// </code>
    /// </para>
    /// <para>
    /// PeriodFormatterBuilder itself is mutable and not thread-safe, but the
    /// formatters that it builds are thread-safe and immutable.
    /// </para>
    /// </remarks>
    public class PeriodFormatterBuilder
    {
        #region Private classes

        enum PrintZeroSetting
        {
            RarelyFirst = 1,
            RarelyLast,
            IfSupported,
            Always,
            Never
        }

        /// <summary>
        /// Defines a formatted field's prefix or suffix text.
        /// This can be used for fields such as 'n hours' or 'nH' or 'Hour:n'.
        /// </summary>
        interface IPeriodFieldAffix
        {
            int CalculatePrintedLength(int value);

            void PrintTo(StringBuilder stringBuilder, int value);

            void PrintTo(TextWriter textWriter, int value);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="periodString"></param>
            /// <param name="position"></param>
            /// <returns>new position after parsing affix, or ~position of failure</returns>
            int Parse(string periodString, int position);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="periodstring"></param>
            /// <param name="position"></param>
            /// <returns>position where affix starts, or original ~position if not found</returns>
            int Scan(string periodstring, int position);
        }

        /// <summary>
        /// Implements an affix where the text does not vary by the amount.
        /// </summary>
        class SimpleAffix : IPeriodFieldAffix
        {
            private readonly string text;

            public SimpleAffix(string text)
            {
                this.text = text;
            }

                
            #region IPeriodFieldAffix Members

            public int CalculatePrintedLength(int value)
            {
                return text.Length;
            }

            public void PrintTo(StringBuilder stringBuilder, int value)
            {
                stringBuilder.Append(text);
            }

            public void PrintTo(TextWriter textWriter, int value)
            {
                textWriter.Write(text);
            }

            public int Parse(string periodString, int position)
            {
                string periodSubString = periodString.Substring(position, text.Length);
                if (periodSubString.Equals(text, StringComparison.OrdinalIgnoreCase))
                    return position + text.Length;
                else
                    return ~position;
            }

            public int Scan(string periodstring, int position)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        /// <summary>
        /// Implements an affix where the text varies by the amount of the field.
        /// Only singular (1) and plural (not 1) are supported.
        /// </summary>
        class PluralAffix : IPeriodFieldAffix
        {
            private readonly string singularText;
            private readonly string pluralText;

            public PluralAffix(string singularText, string pluralText)
            {
                this.singularText = singularText;
                this.pluralText = pluralText;
            }

            #region IPeriodFieldAffix Members

            public int CalculatePrintedLength(int value)
            {
                return (value == 1 ? singularText : pluralText).Length;
            }

            public void PrintTo(StringBuilder stringBuilder, int value)
            {
                stringBuilder.Append(value == 1 ? singularText : pluralText);
            }

            public void PrintTo(TextWriter textWriter, int value)
            {
                textWriter.Write(value == 1 ? singularText : pluralText);
            }

            public int Parse(string periodString, int position)
            {
                string firstToCheck;
                string secondToCheck;

                if (singularText.Length > pluralText.Length)
                {
                    firstToCheck = singularText;
                    secondToCheck = pluralText;
                }
                else
                {
                    firstToCheck = pluralText;
                    secondToCheck = singularText;
                }

                if (FindText(periodString, position, firstToCheck))
                    return position + firstToCheck.Length;
                if (FindText(periodString, position, secondToCheck))
                    return position + secondToCheck.Length;

                return ~position;
            }

            bool FindText(string targetString, int startAt, string textToFind)
            {
                string targetSubString = targetString.Substring(startAt, textToFind.Length);

                return targetSubString.Equals(textToFind, StringComparison.OrdinalIgnoreCase);
            }

            public int Scan(string periodstring, int position)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        /// <summary>
        /// Builds a composite affix by merging two other affix implementations.
        /// </summary>
        class CompositeAffix : IPeriodFieldAffix
        {
            readonly IPeriodFieldAffix left;
            readonly IPeriodFieldAffix right;

            public CompositeAffix(IPeriodFieldAffix left, IPeriodFieldAffix right)
            {
                this.left = left;
                this.right = right;
            }

            #region IPeriodFieldAffix Members

            public int CalculatePrintedLength(int value)
            {
                return left.CalculatePrintedLength(value)
                    + right.CalculatePrintedLength(value);
            }

            public void PrintTo(StringBuilder stringBuilder, int value)
            {
                left.PrintTo(stringBuilder, value);
                right.PrintTo(stringBuilder, value);
            }

            public void PrintTo(TextWriter textWriter, int value)
            {
                left.PrintTo(textWriter, value);
                right.PrintTo(textWriter, value);
            }

            public int Parse(string periodString, int position)
            {
                position = left.Parse(periodString, position);
                if (position >= 0)
                    position = right.Parse(periodString, position);

                return position;
            }

            public int Scan(string periodString, int position)
            {
                position = left.Scan(periodString, position);
                if (position >= 0)
                    position = right.Scan(periodString, position);

                return position;
            }

            #endregion
        }

        /// <summary>
        /// Handles a simple literal piece of text.
        /// </summary>
        class Literal : IPeriodPrinter, IPeriodParser
        {
            public static readonly Literal Empty = new Literal(String.Empty);

            static Literal() { }

            private readonly string text;

            public Literal(string text)
            {
                this.text = text;
            }


            #region IPeriodPrinter Members

            public int CalculatePrintedLength(IPeriod period, IFormatProvider provider)
            {
                return text.Length;
            }

            public int CountFieldsToPrint(IPeriod period, int stopAt, IFormatProvider provider)
            {
                return 0;
            }

            public void PrintTo(StringBuilder stringBuilder, IPeriod period, IFormatProvider provider)
            {
                stringBuilder.Append(text);
            }

            public void PrintTo(TextWriter textWriter, IPeriod period, IFormatProvider provider)
            {
                textWriter.Write(text);
            }

            #endregion

            #region IPeriodParser Members

            public int Parse(string periodString, int position, PeriodBuilder builder,IFormatProvider provider)
            {
                throw new NotImplementedException();
                //string periodSubString = periodString.Substring(position, text.Length);
                //if (periodSubString.Equals(text, StringComparison.OrdinalIgnoreCase))
                //    return position + text.Length;
                //else
                //    return ~position;
            }

            #endregion
        }

        /// <summary>
        /// Formats the numeric value of a field, potentially with prefix/suffix.
        /// </summary>
        class FieldFormatter : IPeriodPrinter, IPeriodParser
        {
            private readonly int minPrintedDigits;
            private readonly PrintZeroSetting printZero;
            private readonly int maxParsedDigits;
            private readonly bool rejectSignedValues;

            private readonly DurationFieldType fieldType;
            private readonly FieldFormatter[] fieldFormatters;
            private readonly IPeriodFieldAffix prefix;
            private readonly IPeriodFieldAffix suffix;

            public FieldFormatter(int minPrintedDigits, PrintZeroSetting printZero, int maxParsedDigits
                , bool rejectSignedValues, DurationFieldType fieldType, FieldFormatter[] fieldFormatters
                , IPeriodFieldAffix prefix, IPeriodFieldAffix suffix)
            {
                this.minPrintedDigits = minPrintedDigits;
                this.printZero = printZero;
                this.maxParsedDigits = maxParsedDigits;
                this.rejectSignedValues = rejectSignedValues;
                this.fieldType = fieldType;
                this.fieldFormatters = fieldFormatters;
                this.prefix = prefix;
                this.suffix = suffix;

            }

            public FieldFormatter(FieldFormatter inittialFieldFormatter, IPeriodFieldAffix suffix)
            {
                this.minPrintedDigits = inittialFieldFormatter.minPrintedDigits;
                this.printZero = inittialFieldFormatter.printZero;
                this.maxParsedDigits = inittialFieldFormatter.maxParsedDigits;
                this.rejectSignedValues = inittialFieldFormatter.rejectSignedValues;
                this.fieldType = inittialFieldFormatter.fieldType;
                this.fieldFormatters = inittialFieldFormatter.fieldFormatters;
                this.prefix = inittialFieldFormatter.prefix;
                if (inittialFieldFormatter.suffix != null)
                    this.suffix = new CompositeAffix(inittialFieldFormatter.suffix, suffix);
                else
                    this.suffix = suffix;
            }

            public DurationFieldType FieldType { get { return fieldType; } }

            #region IPeriodPrinter Members

            public int CalculatePrintedLength(IPeriod period, IFormatProvider provider)
            {
                var fieldValue = GetFieldValue(period);
                if (fieldValue == long.MaxValue)
                    return 0;

                int digitCount = Math.Max(minPrintedDigits, FormatUtils.CalculateDigitCount(fieldValue));

                var intVlaue = (int)fieldValue;
                if (prefix != null)
                    digitCount += prefix.CalculatePrintedLength(intVlaue);
                if (suffix != null)
                    digitCount += suffix.CalculatePrintedLength(intVlaue);

                return digitCount;
            }

            public int CountFieldsToPrint(IPeriod period, int stopAt, IFormatProvider provider)
            {
                if (stopAt < 0)
                    return 0;

                if (printZero == PrintZeroSetting.Always || GetFieldValue(period) != long.MaxValue)
                    return 1;

                return 0;
            }

            public void PrintTo(StringBuilder stringBuilder, IPeriod period, IFormatProvider provider)
            {
                var fieldValue = GetFieldValue(period);
                if (fieldValue == long.MaxValue)
                    return;

                var intVlaue = (int)fieldValue;
                if (prefix != null)
                    prefix.PrintTo(stringBuilder, intVlaue);

                int minDigits = minPrintedDigits;
                if (minDigits <= 1)
                {
                    FormatUtils.AppendUnpaddedInteger(stringBuilder, intVlaue);
                }
                else
                {
                    FormatUtils.AppendPaddedInteger(stringBuilder, intVlaue, minDigits);
                }

                if (suffix != null)
                    suffix.PrintTo(stringBuilder, intVlaue);
            }

            public void PrintTo(TextWriter textWriter, IPeriod period, IFormatProvider provider)
            {
                var fieldValue = GetFieldValue(period);
                if (fieldValue == long.MaxValue)
                    return;

                var intVlaue = (int)fieldValue;
                if (prefix != null)
                    prefix.PrintTo(textWriter, intVlaue);

                int minDigits = minPrintedDigits;
                if (minDigits <= 1)
                {
                    FormatUtils.WriteUnpaddedInteger(textWriter, intVlaue);
                }
                else
                {
                    FormatUtils.WritePaddedInteger(textWriter, intVlaue, minDigits);
                }

                if (suffix != null)
                    suffix.PrintTo(textWriter, intVlaue);
            }

            #endregion

            #region IPeriodParser Members

            public int Parse(string periodString, int position, PeriodBuilder builder, IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            #endregion

            bool IsZero(IPeriod period) 
            {
                for (int i = 0, isize = period.Size; i < isize; i++) 
                {
                    if (period.GetValue(i) != 0) 
                        return false;
                }
                return true;
            }

            long GetFieldValue(IPeriod period)
            {
                long value;

                if (printZero != PrintZeroSetting.Always && !period.IsSupported(fieldType))
                    return long.MaxValue;
                else
                    value = period.Get(fieldType);

                // determine if period is zero and this is the last field
                if (value == 0)
                {
                    switch (printZero)
                    {
                        case PrintZeroSetting.Never:
                            return long.MaxValue;
                        case PrintZeroSetting.RarelyLast:
                            if (IsZero(period) && fieldFormatters[(int)fieldType] == this)
                            {
                                for (int i = (int)fieldType + 1; i < 12; i++)
                                {
                                    if (period.IsSupported(fieldType) && fieldFormatters[i] != null)
                                    {
                                        return long.MaxValue;
                                    }
                                }
                            }
                            else
                                return long.MaxValue;
                            break;
                        case PrintZeroSetting.RarelyFirst:
                            if (IsZero(period) && fieldFormatters[(int)fieldType] == this)
                            {
                                int i = Math.Min((int)fieldType, 8);  // line split out for IBM JDK
                                i--;                              // see bug 1660490
                                for (; i >= 0 && i <= 13; i--)
                                {
                                    if (period.IsSupported(fieldType) && fieldFormatters[i] != null)
                                    {
                                        return long.MaxValue;
                                    }
                                }
                            }
                            else
                                return long.MaxValue;
                            break;
                    }
                }
                return value;
            }
        }

        /// <summary>
        ///  Composite implementation that merges other fields to create a full pattern.
        /// </summary>
        class Composite : IPeriodParser, IPeriodPrinter
        {
            private readonly IPeriodParser[] periodParsers;
            private readonly IPeriodPrinter[] periodPrinters;

            public Composite(ArrayList printerList, ArrayList parserList)
            {
                if (printerList.Count <= 0)
                    periodPrinters = null;
                else
                    periodPrinters = (IPeriodPrinter[])printerList.ToArray(typeof(IPeriodPrinter));

                if (parserList.Count <= 0)
                    periodParsers = null;
                else
                    periodParsers = (IPeriodParser[])parserList.ToArray(typeof(IPeriodParser));

            }

            public IPeriodParser[] Parsers { get { return periodParsers; } }
            public IPeriodPrinter[] Printers { get { return periodPrinters; } }

            #region IPeriodParser Members

            public int Parse(string periodString, int position, PeriodBuilder builder, IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IPeriodPrinter Members

            public int CalculatePrintedLength(IPeriod period, IFormatProvider provider)
            {
                int sum = 0;

                for (int i = periodPrinters.Length; i > 0; --i)
                    sum += periodPrinters[i-1].CalculatePrintedLength(period, provider);

                return sum;
            }

            public int CountFieldsToPrint(IPeriod period, int stopAt, IFormatProvider provider)
            {
                int sum = 0;

                for (int i = periodPrinters.Length; sum < stopAt && i > 0; --i)
                    sum += periodPrinters[i-1].CountFieldsToPrint(period, Int32.MaxValue, provider);

                return sum;
            }

            public void PrintTo(StringBuilder stringBuilder, IPeriod period, IFormatProvider provider)
            {
                foreach (var printer in periodPrinters)
                    printer.PrintTo(stringBuilder, period, provider);
            }

            public void PrintTo(TextWriter textWriter, IPeriod period, IFormatProvider provider)
            {
                foreach (var printer in periodPrinters)
                    printer.PrintTo(textWriter, period, provider);
            }

            #endregion
        }

        /// <summary>
        /// Handles a separator, that splits the fields into multiple parts.
        /// For example, the 'T' in the ISO8601 standard.
        /// </summary>
        class Separator : IPeriodPrinter, IPeriodParser
        {
            readonly string text;
            readonly string finalText;
            readonly string[] parsedForms;
            readonly bool useBefore;
            readonly bool useAfter;

            readonly IPeriodPrinter beforePrinter;
            IPeriodPrinter afterPrinter;
            readonly IPeriodParser beforeParser;
            IPeriodParser afterParser;

            public Separator(string text, string finalText, string[] variants,
                IPeriodPrinter beforePrinter, IPeriodParser beforeParser,
                bool useBefore, bool useAfter)
            {
                this.text = text;
                this.finalText = finalText;

                if ((finalText == null || text.Equals(finalText, StringComparison.Ordinal))
                    && (variants == null || variants.Length == 0))
                {
                    parsedForms = new string[] { text };
                }
                {
                    //filter unique strings
                    var uniqueStrings = new Dictionary<string, string>();
                    if(!uniqueStrings.ContainsKey(text))
                        uniqueStrings.Add(text, text);
                    if(!uniqueStrings.ContainsKey(finalText))
                        uniqueStrings.Add(finalText, finalText);
                    if(variants != null)
                        foreach (var variant in variants)
                            uniqueStrings.Add(variant, variant);
                    parsedForms = new string[uniqueStrings.Keys.Count];
                    uniqueStrings.Keys.CopyTo(parsedForms, 0);

                    //sort in revered order
                    Array.Sort(parsedForms, (first, second) => second.CompareTo(first));

                }

                this.beforePrinter = beforePrinter;
                this.beforeParser = beforeParser;
                this.useBefore = useBefore;
                this.useAfter = useAfter;
            }

            public Separator Finish(IPeriodPrinter afterPrinter, IPeriodParser afterParser)
            {
                this.afterPrinter = afterPrinter;
                this.afterParser = afterParser;

                return this;
            }

            #region IPeriodPrinter Members

            public int CalculatePrintedLength(IPeriod period, IFormatProvider provider)
            {
                int sum = beforePrinter.CalculatePrintedLength(period, provider)
                    + afterPrinter.CalculatePrintedLength(period, provider);

                if (useBefore)
                {
                    if (beforePrinter.CountFieldsToPrint(period, 1, provider) > 0)
                    {
                        if (useAfter)
                        {
                            int afterCount = afterPrinter.CountFieldsToPrint(period, 2, provider);
                            if (afterCount > 0)
                                sum += ((afterCount > 1) ? text : finalText).Length;
                        }
                        else
                            sum += text.Length;
                    }
                }
                else if (useAfter && afterPrinter.CountFieldsToPrint(period, 1, provider) > 0)
                    sum += text.Length;

                return sum;
            }

            public int CountFieldsToPrint(IPeriod period, int stopAt, IFormatProvider provider)
            {
                int sum = beforePrinter.CountFieldsToPrint(period, stopAt, provider);
                if (sum < stopAt)
                {
                    sum += afterPrinter.CountFieldsToPrint(period, stopAt, provider);
                }
                return sum;
            }

            public void PrintTo(StringBuilder stringBuilder, IPeriod period, IFormatProvider provider)
            {
                beforePrinter.PrintTo(stringBuilder, period, provider);
                if (useBefore)
                {
                    if (beforePrinter.CountFieldsToPrint(period, 1, provider) > 0)
                    {
                        if (useAfter)
                        {
                            int afterCount = afterPrinter.CountFieldsToPrint(period, 2, provider);
                            if (afterCount > 0)
                            {
                                stringBuilder.Append(afterCount > 1 ? text : finalText);
                            }
                        }
                        else
                        {
                            stringBuilder.Append(text);
                        }
                    }
                }
                else if (useAfter && afterPrinter.CountFieldsToPrint(period, 1, provider) > 0)
                {
                    stringBuilder.Append(text);
                }
                afterPrinter.PrintTo(stringBuilder, period, provider);
            }

            public void PrintTo(TextWriter textWriter, IPeriod period, IFormatProvider provider)
            {
                beforePrinter.PrintTo(textWriter, period, provider);
                if (useBefore)
                {
                    if (beforePrinter.CountFieldsToPrint(period, 1, provider) > 0)
                    {
                        if (useAfter)
                        {
                            int afterCount = afterPrinter.CountFieldsToPrint(period, 2, provider);
                            if (afterCount > 0)
                            {
                                textWriter.Write(afterCount > 1 ? text : finalText);
                            }
                        }
                        else
                        {
                           textWriter.Write(text);
                        }
                    }
                }
                else if (useAfter && afterPrinter.CountFieldsToPrint(period, 1, provider) > 0)
                {
                    textWriter.Write(text);
                }
                afterPrinter.PrintTo(textWriter, period, provider);
            }

            #endregion

            #region IPeriodParser Members

            public int Parse(string periodString, int position, PeriodBuilder builder, IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodFormatterBuilder"/> class 
        /// with default values.
        /// </summary>
        public PeriodFormatterBuilder()
        {
            Clear();
        }

        /// <summary>
        /// Clears out all the appended elements, allowing this builder to be reused.
        /// </summary>
        public void Clear()
        {
            minimumPrintedDigits = 1;
            printZero = PrintZeroSetting.RarelyLast;
            maximumParsedDigits = 10;
            rejectSignedValues = false;
            prefix = null;
            if (elementPairs == null)
                elementPairs = new List<object>();
            else
                elementPairs.Clear();

            notParser = false;
            notPrinter = false;
            filedFormatters = new FieldFormatter[13];

        }

        #region Options

        PrintZeroSetting printZero;
        int maximumParsedDigits;
        int minimumPrintedDigits;
        bool rejectSignedValues;

        /// <summary>
        /// Always print zero values for the next and following appended fields,
        /// even if the period doesn't support it. The parser requires values for
        /// fields that always print zero.
        /// </summary>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder PrintZeroAlways()
        {
            printZero = PrintZeroSetting.Always;
            return this;
        }

        /// <summary>
        /// Never print zero values for the next and following appended fields,
        /// unless no fields would be printed. If no fields are printed, the printer
        /// forces the last "printZeroRarely" field to print a zero.
        /// </summary>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder PrintZeroNever()
        {
            printZero = PrintZeroSetting.Never;
            return this;
        }

        /// <summary>
        /// Print zero values for the next and following appened fields only if the
        /// period supports it.
        /// </summary>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder PrintZeroIfSupported()
        {
            printZero = PrintZeroSetting.IfSupported;
            return this;
        }

        /// <summary>
        /// Never print zero values for the next and following appended fields,
        /// nless no fields would be printed. If no fields are printed, the printer
        /// forces the first "printZeroRarely" field to print a zero.
        /// </summary>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder PrintZeroRarelyFirst()
        {
            printZero = PrintZeroSetting.RarelyFirst;
            return this;
        }

        /// <summary>
        /// Never print zero values for the next and following appended fields,
        /// nless no fields would be printed. If no fields are printed, the printer
        /// forces the last "printZeroRarely" field to print a zero.
        /// </summary>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder PrintZeroRarelyLast()
        {
            printZero = PrintZeroSetting.RarelyLast;
            return this;
        }

        /// <summary>
        /// Set the minimum digits printed for the next and following appended
        /// fields. By default, the minimum digits printed is one. If the field value
        /// is zero, it is not printed unless a printZero rule is applied.
        /// </summary>
        /// <param name="minDigits">The minimum digits value</param>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder MinimumPrintedDigits(int minDigits)
        {
            minimumPrintedDigits = minDigits;
            return this;
        }

        /// <summary>
        /// Set the maximum digits parsed for the next and following appended
        /// fields. By default, the maximum digits parsed is ten.
        /// </summary>
        /// <param name="maxDigits">The maximum digits value</param>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder MaximumParsedDigits(int maxDigits)
        {
            maximumParsedDigits = maxDigits;
            return this;
        }

        /// <summary>
        /// Reject signed values when parsing the next and following appended fields.
        /// </summary>
        /// <param name="reject">Set true to reject, false otherwise</param>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder RejectSignedValues(bool reject)
        {
            rejectSignedValues = reject;
            return this;
        }

        #endregion

        #region Append

        List<object> elementPairs;
        IPeriodFieldAffix prefix;
        bool notPrinter;
        bool notParser;
        FieldFormatter[] filedFormatters;

        void ClearPrefix()
        {
            if (prefix != null)
                throw new InvalidOperationException("Prefix not followed by field");

            prefix = null;
        }

        PeriodFormatterBuilder AppendImpl(IPeriodPrinter printer, IPeriodParser parser)
        {
            elementPairs.Add(printer);
            elementPairs.Add(parser);
            notPrinter |= (printer == null);
            notParser |= (parser == null);

            return this;
        }

        /// <summary>
        /// Appends another formatter.
        /// </summary>
        /// <param name="formatter"></param>
        /// <exception cref="ArgumentNullException"> If formatter is null</exception>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder Append(PeriodFormatter formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException("formatter", "No formatter supplied");

            ClearPrefix();
            return AppendImpl(formatter.Printer, formatter.Parser);
        }

        /// <summary>
        /// Appends a printer parser pair.
        /// <remarks>
        /// Either the printer or the parser may be null, in which case the builder will
        /// be unable to produce a parser or printer repectively.
        /// </remarks>
        /// </summary>
        /// <exception cref="ArgumentException">If both the printer and parser are null</exception>
        /// <param name="printer">Appends a printer to the builder, null if printing is not supported</param>
        /// <param name="parser">Appends a parser to the builder, null if parsing is not supported</param>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder Append(IPeriodPrinter printer, IPeriodParser parser)
        {
            if (printer == null && parser == null)
                throw new ArgumentException("No printer or parser supplied");

            ClearPrefix();
            return AppendImpl(printer, parser);
        }

        #region Fields

        PeriodFormatterBuilder AppendField(DurationFieldType fieldType)
        {
            return AppendField(fieldType, minimumPrintedDigits);
        }

        PeriodFormatterBuilder AppendField(DurationFieldType fieldType, int minDigits)
        {
            FieldFormatter newFieldFormatter = new FieldFormatter(minDigits, printZero, maximumParsedDigits
                , rejectSignedValues, fieldType, filedFormatters, prefix, null);
            filedFormatters[(int)fieldType] = newFieldFormatter;
            prefix = null;

            return AppendImpl(newFieldFormatter, newFieldFormatter);
        }

        /// <summary>
        /// Instructs the printer to emit specific text, and the parser to expect it.
        /// The parser is case-insensitive.
        /// </summary>
        /// <exception cref="ArgumentNullException">If text is null</exception>
        /// <param name="text">The text of the literal to append</param>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder AppendLiteral(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text", "Literal must not be null");

            ClearPrefix();
            Literal newLiteral = new Literal(text);
            return AppendImpl(newLiteral, newLiteral);

        }

        /// <summary>
        /// Instruct the printer to emit an integer years field, if supported.
        /// </summary>
        /// <remarks>
        /// The number of printed and parsed digits can be controlled using
        /// <see cref="PeriodFormatterBuilder.MinimumPrintedDigits(int minDigits)"/>
        /// and <see cref="PeriodFormatterBuilder. MaximumParsedDigits(int maxDigits)"/>
        /// </remarks>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder AppendYears()
        {
            return AppendField(DurationFieldType.Years);
        }

        /// <summary>
        /// Instruct the printer to emit an integer months field, if supported.
        /// </summary>
        /// <remarks>
        /// The number of printed and parsed digits can be controlled using
        /// <see cref="PeriodFormatterBuilder.MinimumPrintedDigits(int minDigits)"/>
        /// and <see cref="PeriodFormatterBuilder. MaximumParsedDigits(int maxDigits)"/>
        /// </remarks>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder AppendMonths()
        {
            return AppendField(DurationFieldType.Months);
        }

        /// <summary>
        /// Instruct the printer to emit an integer weeks field, if supported.
        /// </summary>
        /// <remarks>
        /// The number of printed and parsed digits can be controlled using
        /// <see cref="PeriodFormatterBuilder.MinimumPrintedDigits(int minDigits)"/>
        /// and <see cref="PeriodFormatterBuilder. MaximumParsedDigits(int maxDigits)"/>
        /// </remarks>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder AppendWeeks()
        {
            return AppendField(DurationFieldType.Weeks);
        }

        /// <summary>
        /// Instruct the printer to emit an integer days field, if supported.
        /// </summary>
        /// <remarks>
        /// The number of printed and parsed digits can be controlled using
        /// <see cref="PeriodFormatterBuilder.MinimumPrintedDigits(int minDigits)"/>
        /// and <see cref="PeriodFormatterBuilder. MaximumParsedDigits(int maxDigits)"/>
        /// </remarks>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder AppendDays()
        {
            return AppendField(DurationFieldType.Days);
        }

        /// <summary>
        /// Instruct the printer to emit an integer hours field, if supported.
        /// </summary>
        /// <remarks>
        /// The number of printed and parsed digits can be controlled using
        /// <see cref="PeriodFormatterBuilder.MinimumPrintedDigits(int minDigits)"/>
        /// and <see cref="PeriodFormatterBuilder. MaximumParsedDigits(int maxDigits)"/>
        /// </remarks>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder AppendHours()
        {
            return AppendField(DurationFieldType.Hours);
        }

        /// <summary>
        /// Instruct the printer to emit an integer minutes field, if supported.
        /// </summary>
        /// <remarks>
        /// The number of printed and parsed digits can be controlled using
        /// <see cref="PeriodFormatterBuilder.MinimumPrintedDigits(int minDigits)"/>
        /// and <see cref="PeriodFormatterBuilder. MaximumParsedDigits(int maxDigits)"/>
        /// </remarks>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder AppendMinutes()
        {
            return AppendField(DurationFieldType.Minutes);
        }

        /// <summary>
        /// Instruct the printer to emit an integer seconds field, if supported.
        /// </summary>
        /// <remarks>
        /// The number of printed and parsed digits can be controlled using
        /// <see cref="PeriodFormatterBuilder.MinimumPrintedDigits(int minDigits)"/>
        /// and <see cref="PeriodFormatterBuilder. MaximumParsedDigits(int maxDigits)"/>
        /// </remarks>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder AppendSeconds()
        {
            return AppendField(DurationFieldType.Seconds);
        }


        /// <summary>
        /// Instruct the printer to emit a combined seconds and millis field, if supported.
        /// </summary>
        /// <remarks>
        /// The millis will overflow into the seconds if necessary.
        /// The millis are always output.
        /// The number of printed and parsed digits can be controlled using
        /// <see cref="PeriodFormatterBuilder.MinimumPrintedDigits(int minDigits)"/>
        /// and <see cref="PeriodFormatterBuilder. MaximumParsedDigits(int maxDigits)"/>
        /// </remarks>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder AppendSecondsWithMillis()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Instruct the printer to emit a combined seconds and millis field, if supported.
        /// </summary>
        /// <remarks>
        /// The millis will overflow into the seconds if necessary.
        /// The millis are only output if non-zero.
        /// The number of printed and parsed digits can be controlled using
        /// <see cref="PeriodFormatterBuilder.MinimumPrintedDigits(int minDigits)"/>
        /// and <see cref="PeriodFormatterBuilder. MaximumParsedDigits(int maxDigits)"/>
        /// </remarks>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder AppendSecondsWithOptionalMillis()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Instruct the printer to emit an integer millis field, if supported.
        /// </summary>
        /// <remarks>
        /// The number of printed and parsed digits can be controlled using
        /// <see cref="PeriodFormatterBuilder.MinimumPrintedDigits(int minDigits)"/>
        /// and <see cref="PeriodFormatterBuilder. MaximumParsedDigits(int maxDigits)"/>
        /// </remarks>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder AppendMillis()
        {
            return AppendField(DurationFieldType.Milliseconds);
        }

        /// <summary>
        /// Instruct the printer to emit an integer millis field, if supported.
        /// </summary>
        /// <remarks>
        /// The number of printed and parsed digits can be controlled using
        /// <see cref="PeriodFormatterBuilder. MaximumParsedDigits(int maxDigits)"/>
        /// </remarks>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder AppendMillis3Digit()
        {
            return AppendField(DurationFieldType.Milliseconds, 3);
        }

        #endregion

        #region Prefix

        PeriodFormatterBuilder AppendPrefix(IPeriodFieldAffix prefix)
        {
            if (prefix == null)
                throw new ArgumentNullException();

            if (this.prefix != null)
                prefix = new CompositeAffix(this.prefix, prefix);

            this.prefix = prefix;

            return this;
        }

        /// <summary>
        /// Append a field prefix which applies only to the next appended field. If
        /// the field is not printed, neither is the prefix.
        /// </summary>
        /// <param name="text">Text to print before field only if field is printed</param>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder AppendPrefix(string text)
        {
            if (text == null)
                throw new ArgumentNullException();
            return AppendPrefix(new SimpleAffix(text));
        }

        /// <summary>
        /// Append a field prefix which applies only to the next appended field. If
        /// the field is not printed, neither is the prefix.
        /// </summary>
        /// <param name="text">Text to print before field only if field is printed</param>
        /// <returns>This PeriodFormatterBuilder</returns>
        public PeriodFormatterBuilder AppendPrefix(string singularText, string pluralText)
        {
            if (singularText == null || pluralText == null)
                throw new ArgumentNullException();
            return AppendPrefix(new  PluralAffix(singularText, pluralText));
        }

        #endregion

        #region Suffix

        PeriodFormatterBuilder AppendSuffix(IPeriodFieldAffix suffix)
        {
            object originalPrinter = default(object);
            object orgiginalParser = default(object);

            if (elementPairs.Count > 0)
            {
                originalPrinter = elementPairs[elementPairs.Count - 2];
                orgiginalParser = elementPairs[elementPairs.Count - 1];
            }

            var originalFormatter = originalPrinter as FieldFormatter;

            if (originalPrinter == null || orgiginalParser == null
                || originalPrinter != orgiginalParser
                || originalFormatter == null)
            {
                throw new InvalidOperationException("No field to apply suffix to");
            }

            ClearPrefix();
            var newfieldFormater = new FieldFormatter(originalFormatter, suffix);
            elementPairs[elementPairs.Count - 2] = newfieldFormater;
            elementPairs[elementPairs.Count - 1] = newfieldFormater;
            filedFormatters[(int)newfieldFormater.FieldType] = newfieldFormater;

            return this;
        }

        /// <summary>
        /// Append a field suffix which applies only to the last appended field. If
        /// the field is not printed, neither is the suffix.
        /// </summary>
        /// <param name="text">Text to print after field only if field is printed</param>
        /// <returns>This PeriodFormatterBuilder</returns>
        /// <exception cref="InvalidOperationException">If no field exists to append to</exception>
        public PeriodFormatterBuilder AppendSuffix(string text)
        {
            if (text == null)
                throw new ArgumentNullException();

            return AppendSuffix(new SimpleAffix(text));
        }

        /// <summary>
        /// Append a field suffix which applies only to the last appended field. If
        /// the field is not printed, neither is the suffix.
        /// </summary>
        /// <param name="singularText">Text to print if field value is one</param>
        /// <param name="pluralText">Text to print if field value is not one</param>
        /// <returns>This PeriodFormatterBuilder</returns>
        /// <remarks>
        /// During parsing, the singular and plural versions are accepted whether or
        /// ot the actual value matches plurality.
        /// </remarks>
        /// <exception cref="InvalidOperationException">If no field exists to append to</exception>
        public PeriodFormatterBuilder AppendSuffix(string singularText, string pluralText)
        {
            if (singularText == null || pluralText == null)
                throw new ArgumentNullException();
            return AppendSuffix(new PluralAffix(singularText, pluralText));

        }

        #endregion

        #region Separator

        PeriodFormatterBuilder AppendSeparator(string text, string finalText, string[] variants,
            bool useBefore, bool useAfter)
        {
            if (text == null || finalText == null)
                throw new ArgumentNullException();

            ClearPrefix();

            //optimize zero formatter case
            if (elementPairs.Count == 0)
            {
                if (useAfter && useBefore == false)
                {
                    Separator newSepaator = new Separator(text, finalText, variants
                        , Literal.Empty, Literal.Empty, useBefore, useAfter);
                    AppendImpl(newSepaator, newSepaator);
                }
                return this;
            }

            //find the last separator added
            int i;
            Separator lastSeparator = default(Separator);
            for (i = elementPairs.Count; --i >= 0; )
            {
                if ((lastSeparator = elementPairs[i] as Separator) != null)
                {
                    break;
                }
                i--;    //element pairs
            }

            //merge formatters
            if (lastSeparator != null && (i + 1) == elementPairs.Count)
                throw new InvalidOperationException("Cannot have two adjacent separators");
            else
            {
                var afterSeparator = new object[elementPairs.Count - i - 1];
                Array.Copy(elementPairs.ToArray(),i+1,afterSeparator,0,elementPairs.Count - i -1);
                elementPairs.RemoveRange(i + 1, elementPairs.Count - i - 1);
                var objects = CreateComposite(afterSeparator);

                Separator separator = new Separator(
                         text, finalText, variants,
                         (IPeriodPrinter)objects[0], (IPeriodParser)objects[1],
                         useBefore, useAfter);
                elementPairs.Add(separator);
                elementPairs.Add(separator);
            }
            return this;
        }

        /// <summary>
        /// Append a separator, which is output if fields are printed both before
        /// and after the separator.
        /// </summary>
        /// <param name="text">The text to use as a separator</param>
        /// <param name="finalText">The text used if this is the final separator to be printed</param>
        /// <param name="variants">Set of text values which are also acceptable when parsed</param>
        /// <returns>This PeriodFormatterBuilder</returns>
        /// <remarks>
        /// This method changes the separator depending on whether it is the last separator
        /// to be output.
        /// <para>
        /// For example,
        /// <code>
        /// builder.AppendDays().AppendSeparator(",", "&").AppendHours().AppendSeparator(",", "&").AppendMinutes()
        /// </code>
        /// will output '1,2&3' if all three fields are output, '1&2' if two fields are output
        /// and '1' if just one field is output.
        /// </para>
        /// <para>
        /// The text will be parsed case-insensitively.
        /// </para>
        /// <para>
        /// Note: appending a separator discontinues any further work on the latest
        /// appended field.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">If this separator follows a previous one</exception>
        public PeriodFormatterBuilder AppendSeparator(string text, string finalText, string[] variants)
        {
            return AppendSeparator(text, finalText, variants, true, true);
        }

        /// <summary>
        /// Append a separator, which is output if fields are printed both before
        /// and after the separator.
        /// </summary>
        /// <param name="text">The text to use as a separator</param>
        /// <param name="finalText">The text used if this is the final separator to be printed</param>
        /// <returns>This PeriodFormatterBuilder</returns>
        /// <remarks>
        /// This method changes the separator depending on whether it is the last separator
        /// to be output.
        /// <para>
        /// For example,
        /// <code>
        /// builder.AppendDays().AppendSeparator(",", "&").AppendHours().AppendSeparator(",", "&").AppendMinutes()
        /// </code>
        /// will output '1,2&3' if all three fields are output, '1&2' if two fields are output
        /// and '1' if just one field is output.
        /// </para>
        /// <para>
        /// The text will be parsed case-insensitively.
        /// </para>
        /// <para>
        /// Note: appending a separator discontinues any further work on the latest
        /// appended field.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">If this separator follows a previous one</exception>
        public PeriodFormatterBuilder AppendSeparator(string text, string finalText)
        {
            return AppendSeparator(text, finalText, null, true, true);
        }

        /// <summary>
        /// Append a separator, which is output only if fields are printed before the separator.
        /// </summary>
        /// <param name="text">The text to use as a separator</param>
        /// <returns>This PeriodFormatterBuilder</returns>
        /// <remarks>
        /// <para>
        /// For example,
        /// <code>
        /// builder.AppendDays().AppendSeparatorIfFieldsBefore(",").appendHours()
        /// </code>
        /// will only output the comma if the days fields is output.
        /// </para>
        /// <para>
        /// The text will be parsed case-insensitively.
        /// </para>
        /// <para>
        /// Note: appending a separator discontinues any further work on the latest
        /// appended field.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">If this separator follows a previous one</exception>
        public PeriodFormatterBuilder AppendSeparatorIfFieldsBefore(string text)
        {
            return AppendSeparator(text, text, null, true, false);
        }

        /// <summary>
        /// Append a separator, which is output only if fields are printed after the separator.
        /// </summary>
        /// <param name="text">The text to use as a separator</param>
        /// <returns>This PeriodFormatterBuilder</returns>
        /// <remarks>
        /// <para>
        /// For example,
        /// <code>
        /// builder.appendDays().appendSeparatorIfFieldsAfter(",").appendHours()
        /// </code>
        /// will only output the comma if the hours fields is output.
        /// </para>
        /// <para>
        /// The text will be parsed case-insensitively.
        /// </para>
        /// <para>
        /// Note: appending a separator discontinues any further work on the latest
        /// appended field.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">If this separator follows a previous one</exception>
        public PeriodFormatterBuilder AppendSeparatorIfFieldsAfter(string text)
        {
            return AppendSeparator(text, text, null, false, true);
        }

        /// <summary>
        /// Append a separator, which is output if fields are printed both before
        /// </summary>
        /// <param name="text">The text to use as a separator</param>
        /// <returns>This PeriodFormatterBuilder</returns>
        /// <remarks>
        /// <para>
        /// For example,
        /// <code>
        /// builder.appendDays().appendSeparator(",").appendHours()
        /// </code>
        /// will only output the comma if both the days and hours fields are output.
        /// </para>
        /// <para>
        /// The text will be parsed case-insensitively.
        /// </para>
        /// <para>
        /// Note: appending a separator discontinues any further work on the latest
        /// appended field.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">If this separator follows a previous one</exception>
        public PeriodFormatterBuilder AppendSeparator(string text)
        {
            return AppendSeparator(text, text, null, true, true);
        }

        #endregion

        #region Composition

        static object[] CreateComposite(IList elementPairs)
        {
            switch (elementPairs.Count)
            {
                case 0:
                    return new Object[] { Literal.Empty, Literal.Empty };
                case 1:
                    return new Object[] { elementPairs[0], elementPairs[1] };
                default:
                    var printers = new ArrayList();
                    var parsers = new ArrayList();
                    Decompose(elementPairs, printers, parsers);
                    Composite comp = new Composite(printers, parsers);
                    return new Object[] { comp, comp };
            }
        }

        static void Decompose(IList elementPairs, ArrayList printerList, ArrayList parserList)
        {
            for (int i = 0; i < elementPairs.Count; i++ )
            {
                var firstItem = elementPairs[i];
                if (firstItem is IPeriodPrinter)
                    if (firstItem is Composite)
                        printerList.AddRange(((Composite)firstItem).Printers);
                    else
                        printerList.Add(firstItem);
                ++i;
                var secondItem = elementPairs[i];
                if (secondItem is IPeriodParser)
                    if (secondItem is Composite)
                        parserList.AddRange(((Composite)secondItem).Parsers);
                    else
                        parserList.Add(secondItem);
            }
        }

        static PeriodFormatter ToFormatter(List<Object> elementPairs, bool notPrinter, bool notParser)
        {
            if (notPrinter && notParser)
            {
                throw new InvalidOperationException("Builder has created neither a printer nor a parser");
            }
            int size = elementPairs.Count;
            if (size >= 2 && elementPairs[0] is Separator)
            {
                Separator sep = (Separator)elementPairs[0];
                var elementsAfterSeparator = new List<Object>(size - 2);
                for (int i = 0; i < size - 2; i++)
                    elementsAfterSeparator.Add(elementPairs[i + 2]);

                PeriodFormatter f = ToFormatter(elementsAfterSeparator, notPrinter, notParser);
                sep = sep.Finish(f.Printer, f.Parser);
                return PeriodFormatter.FromPrinterAndParser(sep, sep);
            }
            Object[] comp = CreateComposite(elementPairs);
            if (notPrinter)
                return PeriodFormatter.FromParser((IPeriodParser)comp[1]);
            else if (notParser)
                return PeriodFormatter.FromPrinter((IPeriodPrinter)comp[0]);
            else
                return PeriodFormatter.FromPrinterAndParser((IPeriodPrinter)comp[0], (IPeriodParser)comp[1]);
        }

        /// <summary>
        /// Constructs a PeriodFormatter using all the appended elements.
        /// </summary>
        /// <returns>The newly created formatter</returns>
        /// <remarks>
        /// <para>
        /// This is the main method used by applications at the end of the build
        /// process to create a usable formatter.
        /// </para>
        /// <para>
        /// Subsequent changes to this builder do not affect the returned formatter.
        /// </para>
        /// <para>
        /// The returned formatter may not support both printing and parsing.
        /// The methods <see cref="PeriodFormatter.IsPrinter"/> and
        /// <see cref="PeriodFormatter.IsParser"/> will help you determine the state
        /// of the formatter.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">If the builder can produce neither a printer nor a parser</exception>
        public PeriodFormatter ToFormatter()
        {
            var newPeriodFormatter = ToFormatter(elementPairs, notPrinter, notParser);
            filedFormatters = (FieldFormatter[])filedFormatters.Clone();

            return newPeriodFormatter;
        }

        /// <summary>
        /// Internal method to create a IPeriodPrinter instance using all the
        /// appended elements.
        /// </summary>
        /// <returns>The newly created printer, null if builder cannot create a printer</returns>
        /// <remarks>
        /// <para>
        /// Most applications will not use this method.
        /// If you want a printer in an application, call <see cref="ToFormatter()"/>
        /// and just use the printing API.
        /// </para>
        /// <para>
        /// Subsequent changes to this builder do not affect the returned printer.
        /// </para>
        /// </remarks>
        public IPeriodPrinter ToPrinter()
        {
            if (notPrinter)
                return null;

            return ToFormatter().Printer;
        }

        /// <summary>
        /// Internal method to create a IPeriodParser instance using all the
        /// appended elements.
        /// </summary>
        /// <returns>The newly created parser, null if builder cannot create a parser</returns>
        /// <remarks>
        /// <para>
        /// Most applications will not use this method.
        /// If you want a parser in an application, call <see cref="ToFormatter()"/>
        /// and just use the parsing API.
        /// </para>
        /// <para>
        /// Subsequent changes to this builder do not affect the returned parser.
        /// </para>
        /// </remarks>
        public IPeriodParser ToParser()
        {
            if (notParser)
                return null;

            return ToFormatter().Parser;
        }
        #endregion

        #endregion
    }
}
