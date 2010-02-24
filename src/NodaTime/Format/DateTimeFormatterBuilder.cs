#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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

using NodaTime.Calendars;
using NodaTime.Fields;

namespace NodaTime.Format
{
    /// <summary>
    /// Original name: DateTimeFormatterBuilder.
    /// </summary>
    public class DateTimeFormatterBuilder
    {
        private const char UnicodeReplacementCharacter = '\ufffd';

        #region Private classes

        private class CharacterLiteral : IDateTimePrinter
        {
            private readonly char value;

            public CharacterLiteral(char value)
            {
                this.value = value;
            }

            public int EstimatedPrintedLength { get { return 1; } }

            public void PrintTo(TextWriter writer, LocalInstant instant, ICalendarSystem calendarSystem, Offset timezoneOffset, IDateTimeZone dateTimeZone, IFormatProvider provider)
            {
                writer.Write(value);
            }

            public void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider)
            {
                writer.Write(value);
            }
        }

        private class StringLiteral : IDateTimePrinter
        {
            private readonly string value;

            public StringLiteral(string value)
            {
                this.value = value;
            }

            public int EstimatedPrintedLength { get { return value.Length; } }

            public void PrintTo(TextWriter writer, LocalInstant instant, ICalendarSystem calendarSystem, Offset timezoneOffset, IDateTimeZone dateTimeZone, IFormatProvider provider)
            {
                writer.Write(value);
            }

            public void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider)
            {
                writer.Write(value);
            }
        }

        private class TextField : IDateTimePrinter
        {
            private readonly DateTimeFieldType fieldType;
            private readonly bool isShort;

            public TextField(DateTimeFieldType fieldType, bool isShort)
            {
                this.fieldType = fieldType;
                this.isShort = isShort;
            }

            public int EstimatedPrintedLength { get { return isShort ? 6 : 20; } }

            public void PrintTo(TextWriter writer, LocalInstant instant, ICalendarSystem calendarSystem, Offset timezoneOffset, IDateTimeZone dateTimeZone, IFormatProvider provider)
            {
                try
                {
                    writer.Write(Print(instant, calendarSystem, provider));
                }
                catch (SystemException)
                {
                    writer.Write(UnicodeReplacementCharacter);
                }
            }

            public void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider)
            {
                try
                {
                    writer.Write(Print(partial, provider));
                }
                catch (SystemException)
                {
                    writer.Write(UnicodeReplacementCharacter);
                }
            }

            private String Print(LocalInstant instant, ICalendarSystem calendar, IFormatProvider provider)
            {
                IDateTimeField field = fieldType.GetField(calendar);

                return isShort ? field.GetAsShortText(instant, provider)
                                : field.GetAsText(instant, provider);
            }

            private String Print(IPartial partial, IFormatProvider provider)
            {
                if (partial.IsSupported(fieldType))
                {
                    IDateTimeField field = fieldType.GetField(partial.Calendar);

                    return isShort ? field.GetAsShortText(partial, provider)
                                    : field.GetAsText(partial, provider);
                }
                else
                {
                    return "\ufffd";
                }
            }
        }

        private abstract class NumberFormatter
        {
            private readonly DateTimeFieldType fieldType;
            private readonly int maxParsedDigits;
            private readonly bool signed;

            protected NumberFormatter(DateTimeFieldType fieldType, int maxParsedDigits, bool signed)
            {
                this.fieldType = fieldType;
                this.maxParsedDigits = maxParsedDigits;
                this.signed = signed;
            }

            protected DateTimeFieldType FieldType { get { return fieldType; } }
            protected int MaxParsedDigits { get { return maxParsedDigits; } }
            protected bool Signed { get { return signed; } }
        }

        private class UnpaddedNumber : NumberFormatter, IDateTimePrinter
        {
            public UnpaddedNumber(DateTimeFieldType fieldType, int maxParsedDigits, bool signed)
                : base(fieldType, maxParsedDigits, signed)
            {

            }

            public int EstimatedPrintedLength { get { return MaxParsedDigits; } }

            public void PrintTo(TextWriter writer, LocalInstant instant, ICalendarSystem calendarSystem, Offset timezoneOffset, IDateTimeZone dateTimeZone, IFormatProvider provider)
            {
                try
                {
                    IDateTimeField field = FieldType.GetField(calendarSystem);
                    FormatUtils.WriteUnpaddedInteger(writer, field.GetValue(instant));
                }
                catch (SystemException)
                {
                    writer.Write(UnicodeReplacementCharacter);
                }
            }

            public void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider)
            {
                try
                {
                    FormatUtils.WriteUnpaddedInteger(writer, partial.Get(FieldType));
                }
                catch (SystemException)
                {
                    writer.Write(UnicodeReplacementCharacter);
                }
            }

        }

        private class PaddedNumber : NumberFormatter, IDateTimePrinter
        {
            private readonly int minPrintedDigits;

            public PaddedNumber(DateTimeFieldType fieldType, int maxParsedDigits,
                int minPrintedDigits, bool signed)
                : base(fieldType, maxParsedDigits, signed)
            {
                this.minPrintedDigits = minPrintedDigits;
            }

            public int EstimatedPrintedLength { get { return MaxParsedDigits; } }

            public void PrintTo(TextWriter writer, LocalInstant instant, ICalendarSystem calendarSystem, Offset timezoneOffset, IDateTimeZone dateTimeZone, IFormatProvider provider)
            {
                try
                {
                    IDateTimeField field = FieldType.GetField(calendarSystem);
                    FormatUtils.WritePaddedInteger(writer, field.GetValue(instant), minPrintedDigits);
                }
                catch (SystemException)
                {
                    writer.Write(UnicodeReplacementCharacter);
                }
            }

            public void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider)
            {
                try
                {
                    FormatUtils.WritePaddedInteger(writer, partial.Get(FieldType), minPrintedDigits);
                }
                catch (SystemException)
                {
                    writer.Write(UnicodeReplacementCharacter);
                }
            }

        }

        private enum TimeZoneNamePrintKind
        {
            LongName,
            ShortName,
            Id
        }

        private class TimeZoneName : IDateTimePrinter
        {
            private readonly TimeZoneNamePrintKind printKind;

            public TimeZoneName(TimeZoneNamePrintKind printKind)
            {
                this.printKind = printKind;
            }

            public int EstimatedPrintedLength 
            { 
                get { return printKind == TimeZoneNamePrintKind.ShortName ? 4 : 20; } 
            }

            public void PrintTo(TextWriter writer, LocalInstant instant, ICalendarSystem calendarSystem, Offset timezoneOffset, IDateTimeZone dateTimeZone, IFormatProvider provider)
            {
                writer.Write(Print(instant - timezoneOffset, dateTimeZone, provider));
            }

            public void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider)
            {
                //no zone info
            }

            private String Print(Instant instant, IDateTimeZone displayZone, IFormatProvider provider)
            {

                if (displayZone == null)
                {
                    return "";  // no zone
                }
                //TODO: can't be ported until the required changes in timezone API
                //switch (printKind)
                //{
                //    case TimeZoneNamePrintKind.LongName:
                //        return displayZone.Name(instant);
                //    case TimeZoneNamePrintKind.ShortName:
                //        return displayZone.getShortName(instant, locale);
                //    case TimeZoneNamePrintKind.Id:
                //        return displayZone.Id;
                //}
                return "";
            }
        }

        #endregion

        private readonly List<object> elementPairs = new List<object>();

        public DateTimeFormatterBuilder()
        {
        }

        private DateTimeFormatterBuilder AppendObject(object @object)
        {
            elementPairs.Add(@object);
            elementPairs.Add(@object);
            return this;
        }

        private DateTimeFormatterBuilder AppendPair(IDateTimePrinter printer, IDateTimeParser parser)
        {
            elementPairs.Add(printer);
            elementPairs.Add(parser);
            return this;
        }

        /// <summary>
        /// Appends another formatter.
        /// </summary>
        /// <param name="formatter">The formatter to add</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <exception cref="ArgumentNullException">If formatter is null</exception>
        public DateTimeFormatterBuilder Append(DateTimeFormatter formatter)
        {
            Guard(formatter);

            return Append(formatter.Printer, formatter.Parser);

        }

        /// <summary>
        /// Appends a printer/parser pair.
        /// </summary>
        /// <param name="printer">The printer to add</param>
        /// <param name="parser">The parser to add</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <exception cref="ArgumentNullException">If printer or parser is null</exception>
        private DateTimeFormatterBuilder Append(IDateTimePrinter printer, IDateTimeParser parser)
        {
            Guard(printer);
            Guard(parser);

            return AppendPair(printer, parser);

        }

        /// <summary>
        /// Appends just a printer. With no matching parser, a parser cannot be
        /// built from this DateTimeFormatterBuilder.
        /// </summary>
        /// <param name="printer">The printer to add</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <exception cref="ArgumentNullException">If printer is null</exception>
        private DateTimeFormatterBuilder Append(IDateTimePrinter printer)
        {
            Guard(printer);

            return AppendPair(printer, null);
        }

        /// <summary>
        /// Appends just a parser. With no matching printer, a printer cannot be
        /// built from this DateTimeFormatterBuilder.
        /// </summary>
        /// <param name="parser">The parser to add</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <exception cref="ArgumentNullException">If parser is null</exception>
        private DateTimeFormatterBuilder Append(IDateTimeParser parser)
        {
            Guard(parser);

            return AppendPair(null, parser);
        }

        /// <summary>
        /// Instructs the printer to emit a specific character, and the parser to
        /// expect it. The parser is case-insensitive.
        /// </summary>
        /// <param name="value">A character value to emit/expect</param>
        /// <returns>this DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendLiteral(char value)
        {
            return AppendObject(new CharacterLiteral(value));
        }

        /// <summary>
        /// Instructs the printer to emit specific text, and the parser to expect
        /// it. The parser is case-insensitive.
        /// </summary>
        /// <param name="text">The text to emit/expect</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendLiteral(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return this;
            }

            return text.Length == 1 ? AppendObject(new CharacterLiteral(text[0]))
                : AppendObject(new StringLiteral(text));
        }

        /// <summary>
        /// Instructs the printer to emit a field value as text, and the
        /// parser to expect text.
        /// </summary>
        /// <param name="fieldType">Type of field to append</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <exception cref="ArgumentNullException">If field type is null</exception>
        public DateTimeFormatterBuilder AppendText(DateTimeFieldType fieldType)
        {
            Guard(fieldType);

            return AppendObject(new TextField(fieldType, false));
        }

        /// <summary>
        /// Instructs the printer to emit a field value as short text, and the
        /// parser to expect text.
        /// </summary>
        /// <param name="fieldType">Type of field to append</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <exception cref="ArgumentNullException">If field type is null</exception>
        public DateTimeFormatterBuilder AppendShortText(DateTimeFieldType fieldType)
        {
            Guard(fieldType);

            return AppendObject(new TextField(fieldType, true));
        }

        /// <summary>
        /// Instructs the printer to emit a field value as a decimal number, and the
        /// parser to expect an unsigned decimal number.
        /// </summary>
        /// <param name="fieldType">Type of field to append</param>
        /// <param name="minDigits">Minumum number of digits to <i>print</i></param>
        /// <param name="maxDigits">Maximum number of digits to <i>parse</i>, or the estimated</param>
        /// maximum number of digits to print
        /// <returns>this DateTimeFormatterBuilder</returns>
        /// <exception cref="ArgumentNullException">If field type is null</exception>
        /// <exception cref="ArgumentException">If minDigits less than zero or maxDigits not greater than zero</exception>
        public DateTimeFormatterBuilder AppendDecimal(DateTimeFieldType fieldType, int minDigits, int maxDigits)
        {
            Guard(fieldType);
                
            if (minDigits < 0 || maxDigits <= 0)
            {
                throw new ArgumentException();
            }

            if (maxDigits < minDigits)
            {
                maxDigits = minDigits;
            }

            return minDigits <= 1 ? AppendObject(new UnpaddedNumber(fieldType, maxDigits, false))
                                : AppendObject(new PaddedNumber(fieldType, maxDigits, minDigits, false));
        }

        /// <summary>
        /// Instructs the printer to emit a numeric millisOfSecond field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <remarks>
        /// This method will append a field that prints a three digit value.
        /// During parsing the value that is parsed is assumed to be three digits.
        /// If less than three digits are present then they will be counted as the
        /// smallest parts of the millisecond. This is probably not what you want
        /// if you are using the field as a fraction. Instead, a fractional
        /// millisecond should be produced using <see cref="AppendFractionOfSecond"/>.
        /// </remarks>
        public DateTimeFormatterBuilder AppendMillisecondsOfSecond(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.MillisecondOfSecond, minDigits, 3);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric millisOfDay field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendMillisecondsOfDay(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.MillisecondOfDay, minDigits, 8);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric secondOfMinute field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendSecondOfMinute(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.SecondOfMinute, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric secondOfDay field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendSecondOfDay(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.SecondOfDay, minDigits, 5);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric minuteOfHour field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendMinuteOfHour(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.MinuteOfHour, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric minuteOfDay field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendMinuteOfDay(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.MinuteOfDay, minDigits, 4);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric hourOfDay field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendHourOfDay(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.HourOfDay, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric clockhourOfDay field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendClockHourOfDay(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.ClockHourOfDay, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric hourOfHalfday field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendHourOfHalfDay(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.HourOfHalfDay, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric clockHourOfHalfday field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendClockHourOfHalfDay(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.ClockHourOfHalfDay, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric dayOfWeek field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendDayOfWeek(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.DayOfWeek, minDigits, 1);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric dayOfMonth field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendDayOfMonth(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.DayOfMonth, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric dayOfYear field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendDayOfYear(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.DayOfYear, minDigits, 3);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric weekOfWeekyear field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendWeekOfWeekYear(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.WeekOfWeekYear, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric weekYear field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <param name="maxDigits">Maximum number of digits to <i>parse</i>, or the estimated</param>
        /// maximum number of digits to print
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendWeekOfWeekYear(int minDigits, int maxDigits)
        {
            return AppendDecimal(DateTimeFieldType.WeekYear, minDigits, maxDigits);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric monthOfYear field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendMonthOfYear(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.MonthOfYear, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric year field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <param name="maxDigits">Maximum number of digits to <i>parse</i>, or the estimated</param>
        /// maximum number of digits to print
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendYear(int minDigits, int maxDigits)
        {
            return AppendDecimal(DateTimeFieldType.Year, minDigits, maxDigits);
        }

        private static void Guard(DateTimeFieldType fieldType)
        {
            if (fieldType == null)
            {
                throw new ArgumentNullException("fieldType");
            }
        }

        private static void Guard(DateTimeFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }
        }

        private static void Guard(IDateTimePrinter printer)
        {
            if (printer == null)
            {
                throw new ArgumentNullException("printer");
            }
        }

        private static void Guard(IDateTimeParser parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }
        }        
    }
}
