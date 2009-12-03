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
    /// The others are <see cref="PeriodFormaterFactory"/> and <see cref="ISOPeriodFormatterfactory"/>.
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

            public int ParseInto(string periodString, int position, IFormatProvider provider, out IPeriod period)
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

            #region IPeriodPrinter Members

            public int CalculatePrintedLength(IPeriod period, IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public int CountFieldsToPrint(IPeriod period, int stopAt, IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public void PrintTo(StringBuilder stringBuilder, IPeriod period, IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public void PrintTo(TextWriter textWriter, IPeriod period, IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IPeriodParser Members

            public int ParseInto(string periodString, int position, IFormatProvider provider, out IPeriod period)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        /// <summary>
        ///  Composite implementation that merges other fields to create a full pattern.
        /// </summary>
        class Composite : IPeriodParser, IPeriodPrinter
        {
            private readonly IPeriodParser[] periodParsers;
            private readonly IPeriodPrinter[] periodPrinters;

            public Composite(IPeriodParser[] parsers, IPeriodPrinter[] printers)
            {
                periodParsers = parsers;
                periodPrinters = printers;

            }

            #region IPeriodParser Members

            public int ParseInto(string periodString, int position, IFormatProvider provider, out IPeriod period)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IPeriodPrinter Members

            public int CalculatePrintedLength(IPeriod period, IFormatProvider provider)
            {
                int sum = 0;

                for (int i = periodPrinters.Length; i >= 0; --i)
                    sum += periodPrinters[i].CalculatePrintedLength(period, provider);

                return sum;
            }

            public int CountFieldsToPrint(IPeriod period, int stopAt, IFormatProvider provider)
            {
                int sum = 0;

                for (int i = periodPrinters.Length; sum < stopAt && i >= 0; --i)
                    sum += periodPrinters[i].CountFieldsToPrint(period, Int32.MaxValue, provider);

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

        #endregion
    }
}
