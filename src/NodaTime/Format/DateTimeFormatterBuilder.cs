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
        #region Private classes
        internal class CharacterLiteral : IDateTimePrinter, IDateTimeParser
        {
            private readonly char value;

            public CharacterLiteral(char value)
            {
                this.value = value;
            }

            public int EstimatedPrintedLength { get { return 1; } }

            public void PrintTo(TextWriter writer, LocalInstant instant, CalendarSystem calendarSystem, Offset timezoneOffset, DateTimeZone dateTimeZone,
                                IFormatProvider provider)
            {
                writer.Write(value);
            }

            public void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider)
            {
                writer.Write(value);
            }

            public int EstimatedParsedLength { get { return 1; } }

            public int ParseInto(DateTimeParserBucket bucket, string text, int position)
            {
                return FormatUtils.MatchChar(text, position, value);
            }
        }

        internal class StringLiteral : IDateTimePrinter, IDateTimeParser
        {
            private readonly string value;

            public StringLiteral(string value)
            {
                this.value = value;
            }

            public int EstimatedPrintedLength { get { return value.Length; } }

            public void PrintTo(TextWriter writer, LocalInstant instant, CalendarSystem calendarSystem, Offset timezoneOffset, DateTimeZone dateTimeZone,
                                IFormatProvider provider)
            {
                writer.Write(value);
            }

            public void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider)
            {
                writer.Write(value);
            }

            public int EstimatedParsedLength { get { return value.Length; } }

            public int ParseInto(DateTimeParserBucket bucket, string text, int position)
            {
                return FormatUtils.MatchSubstring(text, position, value);
            }
        }

        private class TextField : IDateTimePrinter, IDateTimeParser
        {
            private readonly DateTimeFieldType fieldType;
            private readonly bool isShort;

            public TextField(DateTimeFieldType fieldType, bool isShort)
            {
                this.fieldType = fieldType;
                this.isShort = isShort;
            }

            public int EstimatedPrintedLength { get { return isShort ? 6 : 20; } }

            public void PrintTo(TextWriter writer, LocalInstant instant, CalendarSystem calendarSystem, Offset timezoneOffset, DateTimeZone dateTimeZone,
                                IFormatProvider provider)
            {
                try
                {
                    writer.Write(Print(instant, calendarSystem, provider));
                }
                catch (SystemException)
                {
                    FormatUtils.WriteUnknownString(writer);
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
                    FormatUtils.WriteUnknownString(writer);
                }
            }

            private String Print(LocalInstant instant, CalendarSystem calendar, IFormatProvider provider)
            {
                DateTimeField field = fieldType.GetField(calendar);

                return isShort ? field.GetAsShortText(instant, provider) : field.GetAsText(instant, provider);
            }

            private String Print(IPartial partial, IFormatProvider provider)
            {
                if (partial.IsSupported(fieldType))
                {
                    DateTimeField field = fieldType.GetField(partial.Calendar);

                    return isShort ? field.GetAsShortText(partial, provider) : field.GetAsText(partial, provider);
                }
                else
                {
                    return "\ufffd";
                }
            }

            public int EstimatedParsedLength { get { return EstimatedPrintedLength; } }

            private static readonly Dictionary<IFormatProvider, Dictionary<DateTimeFieldType, Object[]>> cParseCache =
                new Dictionary<IFormatProvider, Dictionary<DateTimeFieldType, Object[]>>();

            public int ParseInto(DateTimeParserBucket bucket, string text, int position)
            {
                IFormatProvider provider = bucket.Provider;
                // handle languages which might have non ASCII A-Z or punctuation
                // bug 1788282
                Dictionary<string, string> validValues = null;
                int maxLength = 0;
                lock (cParseCache)
                {
                    var innerMap = cParseCache[provider];
                    if (innerMap == null)
                    {
                        innerMap = new Dictionary<DateTimeFieldType, Object[]>();
                        cParseCache[provider] = innerMap;
                    }
                    var array = innerMap[fieldType];
                    if (array == null)
                    {
                        validValues = new Dictionary<string, string>(32);

                        //TODO: I don't know how to handle this at the moment(Dmitry)

                        //ZonedDateTime dt = new ZonedDateTime();
                        //Property property = dt.property(iFieldType);
                        //int min = property.getMinimumValueOverall();
                        //int max = property.getMaximumValueOverall();
                        //if (max - min > 32) {  // protect against invalid fields
                        //    return ~position;
                        //}
                        //maxLength = property.getMaximumTextLength(locale);
                        //for (int i = min; i <= max; i++) {
                        //    property.set(i);
                        //    validValues.add(property.getAsShortText(locale));
                        //    validValues.add(property.getAsShortText(locale).toLowerCase(locale));
                        //    validValues.add(property.getAsShortText(locale).toUpperCase(locale));
                        //    validValues.add(property.getAsText(locale));
                        //    validValues.add(property.getAsText(locale).toLowerCase(locale));
                        //    validValues.add(property.getAsText(locale).toUpperCase(locale));
                        //}
                        //if ("en".equals(locale.getLanguage()) && iFieldType == DateTimeFieldType.era()) {
                        //    // hack to support for parsing "BCE" and "CE" if the language is English
                        //    validValues.add("BCE");
                        //    validValues.add("bce");
                        //    validValues.add("CE");
                        //    validValues.add("ce");
                        //    maxLength = 3;
                        //}
                        array = new Object[] { validValues, maxLength };
                        innerMap[fieldType] = array;
                    }
                    else
                    {
                        validValues = (Dictionary<string, string>)array[0];
                        maxLength = (int)array[1];
                    }
                }
                // match the longest string first using our knowledge of the max length
                int limit = Math.Min(text.Length, position + maxLength);
                for (int i = limit; i > position; i--)
                {
                    String match = text.Substring(position, i);
                    if (validValues.ContainsKey(match))
                    {
                        bucket.SaveField(fieldType, match, provider);
                        return i;
                    }
                }
                return ~position;
            }
        }

        private abstract class NumberFormatter : IDateTimeParser
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

            public int EstimatedParsedLength { get { return maxParsedDigits; } }

            public virtual int ParseInto(DateTimeParserBucket bucket, string text, int position)
            {
                int limit = Math.Min(maxParsedDigits, text.Length - position);

                bool negative = false;
                int length = 0;
                while (length < limit)
                {
                    char c = text[position + length];
                    if (length == 0 && (c == '-' || c == '+') && signed)
                    {
                        negative = c == '-';

                        // Next character must be a digit.
                        if (length + 1 >= limit || (c = text[position + length + 1]) < '0' || c > '9')
                        {
                            break;
                        }

                        if (negative)
                        {
                            length++;
                        }
                        else
                        {
                            // Skip the '+' for parseInt to succeed.
                            position++;
                        }
                        // Expand the limit to disregard the sign character.
                        limit = Math.Min(limit + 1, text.Length - position);
                        continue;
                    }
                    if (c < '0' || c > '9')
                    {
                        break;
                    }
                    length++;
                }

                if (length == 0)
                {
                    return ~position;
                }

                int value;
                if (length >= 9)
                {
                    // Since value may exceed integer limits, use stock parser
                    // which checks for this.
                    value = Int32.Parse(text.Substring(position, position += length));
                }
                else
                {
                    int i = position;
                    if (negative)
                    {
                        i++;
                    }
                    try
                    {
                        value = text[i++] - '0';
                    }
                    catch (IndexOutOfRangeException)
                    {
                        return ~position;
                    }
                    position += length;
                    while (i < position)
                    {
                        value = ((value << 3) + (value << 1)) + text[i++] - '0';
                    }
                    if (negative)
                    {
                        value = -value;
                    }
                }

                bucket.SaveField(fieldType, value);
                return position;
            }
        }

        private class UnpaddedNumber : NumberFormatter, IDateTimePrinter
        {
            public UnpaddedNumber(DateTimeFieldType fieldType, int maxParsedDigits, bool signed) : base(fieldType, maxParsedDigits, signed)
            {
            }

            public int EstimatedPrintedLength { get { return MaxParsedDigits; } }

            public void PrintTo(TextWriter writer, LocalInstant instant, CalendarSystem calendarSystem, Offset timezoneOffset, DateTimeZone dateTimeZone,
                                IFormatProvider provider)
            {
                try
                {
                    int value = FieldType.GetField(calendarSystem).GetValue(instant);

                    FormatUtils.WriteUnpaddedInteger(writer, value);
                }
                catch (SystemException)
                {
                    FormatUtils.WriteUnknownString(writer);
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
                    FormatUtils.WriteUnknownString(writer);
                }
            }
        }

        private class PaddedNumber : NumberFormatter, IDateTimePrinter
        {
            private readonly int minPrintedDigits;

            public PaddedNumber(DateTimeFieldType fieldType, int maxParsedDigits, int minPrintedDigits, bool signed) : base(fieldType, maxParsedDigits, signed)
            {
                this.minPrintedDigits = minPrintedDigits;
            }

            public int EstimatedPrintedLength { get { return MaxParsedDigits; } }

            public void PrintTo(TextWriter writer, LocalInstant instant, CalendarSystem calendarSystem, Offset timezoneOffset, DateTimeZone dateTimeZone,
                                IFormatProvider provider)
            {
                try
                {
                    int value = FieldType.GetField(calendarSystem).GetValue(instant);
                    FormatUtils.WritePaddedInteger(writer, value, minPrintedDigits);
                }
                catch (SystemException)
                {
                    FormatUtils.WriteUnknownString(writer);
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
                    FormatUtils.WriteUnknownString(writer);
                }
            }
        }

        private class FixedNumber : PaddedNumber
        {
            public FixedNumber(DateTimeFieldType fieldType, int numDigits, bool signed) : base(fieldType, numDigits, numDigits, signed)
            {
            }

            public override int ParseInto(DateTimeParserBucket bucket, string text, int position)
            {
                int newPos = base.ParseInto(bucket, text, position);
                if (newPos < 0)
                {
                    return newPos;
                }
                int expectedPos = position + MaxParsedDigits;
                if (newPos != expectedPos)
                {
                    if (Signed)
                    {
                        char c = text[position];
                        if (c == '-' || c == '+')
                        {
                            expectedPos++;
                        }
                    }
                    if (newPos > expectedPos)
                    {
                        // The failure is at the position of the first extra digit.
                        return ~(expectedPos + 1);
                    }
                    else if (newPos < expectedPos)
                    {
                        // The failure is at the position where the next digit should be.
                        return ~newPos;
                    }
                }
                return newPos;
            }
        }

        private class TwoDigitYear : IDateTimePrinter, IDateTimeParser
        {
            private readonly DateTimeFieldType fieldType;
            private readonly int pivot;
            private readonly bool lenient;

            public TwoDigitYear(DateTimeFieldType fieldType, int pivot, bool lenient)
            {
                this.fieldType = fieldType;
                this.pivot = pivot;
                this.lenient = lenient;
            }

            public int EstimatedPrintedLength { get { return 2; } }

            public void PrintTo(TextWriter writer, LocalInstant instant, CalendarSystem calendarSystem, Offset timezoneOffset, DateTimeZone dateTimeZone,
                                IFormatProvider provider)
            {
                int year = GetTwoDigitYear(instant, calendarSystem);
                WriteYear(writer, year);
            }

            public void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider)
            {
                int year = GetTwoDigitYear(partial);
                WriteYear(writer, year);
            }

            private int GetTwoDigitYear(LocalInstant instant, CalendarSystem calendarSystem)
            {
                try
                {
                    int year = fieldType.GetField(calendarSystem).GetValue(instant);
                    if (year < 0)
                    {
                        year = -year;
                    }
                    return year % 100;
                }
                catch (SystemException)
                {
                    return -1;
                }
            }

            private int GetTwoDigitYear(IPartial partial)
            {
                if (partial.IsSupported(fieldType))
                {
                    try
                    {
                        int year = partial.Get(fieldType);
                        if (year < 0)
                        {
                            year = -year;
                        }
                        return year % 100;
                    }
                    catch (SystemException)
                    {
                    }
                }
                return -1;
            }

            private void WriteYear(TextWriter writer, int year)
            {
                if (year < 0)
                {
                    writer.Write('\ufffd');
                    writer.Write('\ufffd');
                }
                else
                {
                    FormatUtils.WritePaddedInteger(writer, year, 2);
                }
            }

            public int EstimatedParsedLength { get { return lenient ? 4 : 2; } }

            public int ParseInto(DateTimeParserBucket bucket, string text, int position)
            {
                int limit = text.Length - position;

                if (!lenient)
                {
                    limit = Math.Min(2, limit);
                    if (limit < 2)
                    {
                        return ~position;
                    }
                }
                else
                {
                    bool hasSignChar = false;
                    bool negative = false;
                    int length = 0;
                    while (length < limit)
                    {
                        char c = text[position + length];
                        if (length == 0 && (c == '-' || c == '+'))
                        {
                            hasSignChar = true;
                            negative = c == '-';
                            if (negative)
                            {
                                length++;
                            }
                            else
                            {
                                // Skip the '+' for parseInt to succeed.
                                position++;
                                limit--;
                            }
                            continue;
                        }
                        if (c < '0' || c > '9')
                        {
                            break;
                        }
                        length++;
                    }

                    if (length == 0)
                    {
                        return ~position;
                    }

                    if (hasSignChar || length != 2)
                    {
                        int value;
                        if (length >= 9)
                        {
                            // Since value may exceed integer limits, use stock
                            // parser which checks for this.
                            value = Int32.Parse(text.Substring(position, position += length));
                        }
                        else
                        {
                            int i = position;
                            if (negative)
                            {
                                i++;
                            }
                            try
                            {
                                value = text[i++] - '0';
                            }
                            catch (IndexOutOfRangeException)
                            {
                                return ~position;
                            }
                            position += length;
                            while (i < position)
                            {
                                value = ((value << 3) + (value << 1)) + text[i++] - '0';
                            }
                            if (negative)
                            {
                                value = -value;
                            }
                        }

                        bucket.SaveField(fieldType, value);
                        return position;
                    }
                }

                int year;
                char cr = text[position];
                if (cr < '0' || cr > '9')
                {
                    return ~position;
                }
                year = cr - '0';
                cr = text[position + 1];
                if (cr < '0' || cr > '9')
                {
                    return ~position;
                }
                year = ((year << 3) + (year << 1)) + cr - '0';

                int pivotLocal = pivot;
                // If the bucket pivot year is non-null, use that when parsing
                if (bucket.PivotYear.HasValue)
                {
                    pivotLocal = bucket.PivotYear.Value;
                }

                int low = pivotLocal - 50;

                int t;
                if (low >= 0)
                {
                    t = low % 100;
                }
                else
                {
                    t = 99 + ((low + 1) % 100);
                }

                year += low + ((year < t) ? 100 : 0) - t;

                bucket.SaveField(fieldType, year);
                return position + 2;
            }
        }

        private class Fraction : IDateTimePrinter, IDateTimeParser
        {
            private readonly DateTimeFieldType fieldType;
            private readonly int minDigits;
            private readonly int maxDigits;

            public Fraction(DateTimeFieldType fieldType, int minDigits, int maxDigits)
            {
                this.fieldType = fieldType;
                if (maxDigits > 18)
                {
                    maxDigits = 18;
                }

                this.minDigits = minDigits;
                this.maxDigits = maxDigits;
            }

            public int EstimatedPrintedLength { get { return maxDigits; } }

            public void PrintTo(TextWriter writer, LocalInstant instant, CalendarSystem calendarSystem, Offset timezoneOffset, DateTimeZone dateTimeZone,
                                IFormatProvider provider)
            {
                Print(writer, instant, calendarSystem);
            }

            public void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider)
            {
                // removed check whether field is supported, as input field is typically
                // secondOfDay which is unsupported by TimeOfDay
                LocalInstant instant = partial.Calendar.SetPartial(partial, LocalInstant.LocalUnixEpoch);
                Print(writer, instant, partial.Calendar);
            }

            private void Print(TextWriter writer, LocalInstant instant, CalendarSystem calendarSystem)
            {
                DateTimeField field = fieldType.GetField(calendarSystem);
                int minDigitsLocal = minDigits;

                Duration fraction;
                try
                {
                    fraction = field.Remainder(instant);
                }
                catch (SystemException)
                {
                    FormatUtils.WriteUnknownString(writer, minDigits);
                    return;
                }

                if (fraction == Duration.Zero)
                {
                    while (--minDigitsLocal >= 0)
                    {
                        writer.Write('0');
                    }

                    return;
                }

                Duration scaled;
                int maxDigits;
                GetFractionData(fraction, field, out scaled, out maxDigits);

                String str = scaled.Ticks.ToString();

                int length = str.Length;
                int digits = maxDigits;
                while (length < digits)
                {
                    writer.Write('0');
                    minDigitsLocal--;
                    digits--;
                }

                if (minDigitsLocal < digits)
                {
                    // Chop off as many trailing zero digits as necessary.
                    while (minDigitsLocal < digits)
                    {
                        if (length <= 1 || str[length - 1] != '0')
                        {
                            break;
                        }
                        digits--;
                        length--;
                    }
                    if (length < str.Length)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            writer.Write(str[i]);
                        }
                    }
                    return;
                }
                writer.Write(str);
            }

            private void GetFractionData(Duration fraction, DateTimeField field, out Duration fractionResult, out int maxDigitsResult)
            {
                long rangeMillis = field.DurationField.UnitTicks;
                long scalar;
                int maxDigitsLocal = maxDigits;
                while (true)
                {
                    switch (maxDigitsLocal)
                    {
                        default:
                            scalar = 1L;
                            break;
                        case 1:
                            scalar = 10L;
                            break;
                        case 2:
                            scalar = 100L;
                            break;
                        case 3:
                            scalar = 1000L;
                            break;
                        case 4:
                            scalar = 10000L;
                            break;
                        case 5:
                            scalar = 100000L;
                            break;
                        case 6:
                            scalar = 1000000L;
                            break;
                        case 7:
                            scalar = 10000000L;
                            break;
                        case 8:
                            scalar = 100000000L;
                            break;
                        case 9:
                            scalar = 1000000000L;
                            break;
                        case 10:
                            scalar = 10000000000L;
                            break;
                        case 11:
                            scalar = 100000000000L;
                            break;
                        case 12:
                            scalar = 1000000000000L;
                            break;
                        case 13:
                            scalar = 10000000000000L;
                            break;
                        case 14:
                            scalar = 100000000000000L;
                            break;
                        case 15:
                            scalar = 1000000000000000L;
                            break;
                        case 16:
                            scalar = 10000000000000000L;
                            break;
                        case 17:
                            scalar = 100000000000000000L;
                            break;
                        case 18:
                            scalar = 1000000000000000000L;
                            break;
                    }
                    if (((rangeMillis * scalar) / scalar) == rangeMillis)
                    {
                        break;
                    }
                    // Overflowed: scale down.
                    maxDigitsLocal--;
                }

                fractionResult = fraction * scalar / rangeMillis;
                maxDigitsResult = maxDigitsLocal;
            }

            public int EstimatedParsedLength { get { return maxDigits; } }

            public int ParseInto(DateTimeParserBucket bucket, string text, int position)
            {
                DateTimeField field = fieldType.GetField(bucket.Calendar);

                int limit = Math.Min(maxDigits, text.Length - position);

                long value = 0;

                //TODO: check this out, JT used millis, NT uses ticks, probably multiply factor by 100
                long n = field.DurationField.UnitTicks * 10;
                int length = 0;
                while (length < limit)
                {
                    char c = text[position + length];
                    if (c < '0' || c > '9')
                    {
                        break;
                    }
                    length++;
                    long nn = n / 10;
                    value += (c - '0') * nn;
                    n = nn;
                }

                value /= 10;

                if (length == 0)
                {
                    return ~position;
                }

                if (value > Int32.MaxValue)
                {
                    return ~position;
                }

                DateTimeField parseField = new PreciseDateTimeField(DateTimeFieldType.MillisecondOfSecond, PreciseDurationField.Milliseconds,
                                                                    field.DurationField);

                bucket.SaveField(parseField, (int)value);

                return position + length;
            }
        }

        private class MatchingParser : IDateTimeParser
        {
            private readonly IDateTimeParser[] parsers;
            private readonly int parsedLengthEstimate;

            public MatchingParser(IDateTimeParser[] parsers)
            {
                int parsedLengthEstimate = 0;
                for (int i = parsers.Length; --i >= 0;)
                {
                    IDateTimeParser parser = parsers[i];
                    if (parser != null)
                    {
                        int len = parser.EstimatedParsedLength;
                        if (len > parsedLengthEstimate)
                        {
                            parsedLengthEstimate = len;
                        }
                    }
                }
                this.parsedLengthEstimate = parsedLengthEstimate;
                this.parsers = parsers;
            }

            public int EstimatedParsedLength { get { return parsedLengthEstimate; } }

            public int ParseInto(DateTimeParserBucket bucket, string text, int position)
            {
                IDateTimeParser[] parsersLocal = parsers;
                int length = parsersLocal.Length;

                var originalState = bucket.SaveState();
                bool isOptional = false;

                int bestValidPos = position;
                Object bestValidState = null;

                int bestInvalidPos = position;

                for (int i = 0; i < length; i++)
                {
                    IDateTimeParser parser = parsersLocal[i];
                    if (parser == null)
                    {
                        // The empty parser wins only if nothing is better.
                        if (bestValidPos <= position)
                        {
                            return position;
                        }
                        isOptional = true;
                        break;
                    }
                    int parsePos = parser.ParseInto(bucket, text, position);
                    if (parsePos >= position)
                    {
                        if (parsePos > bestValidPos)
                        {
                            if (parsePos >= text.Length || (i + 1) >= length || parsers[i + 1] == null)
                            {
                                // Completely parsed text or no more parsers to
                                // check. Skip the rest.
                                return parsePos;
                            }
                            bestValidPos = parsePos;
                            bestValidState = bucket.SaveState();
                        }
                    }
                    else
                    {
                        if (parsePos < 0)
                        {
                            parsePos = ~parsePos;
                            if (parsePos > bestInvalidPos)
                            {
                                bestInvalidPos = parsePos;
                            }
                        }
                    }
                    bucket.RestoreState(originalState);
                }

                if (bestValidPos > position || (bestValidPos == position && isOptional))
                {
                    // Restore the state to the best valid parse.
                    if (bestValidState != null)
                    {
                        bucket.RestoreState(bestValidState);
                    }
                    return bestValidPos;
                }

                return ~bestInvalidPos;
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

            public int EstimatedPrintedLength { get { return printKind == TimeZoneNamePrintKind.ShortName ? 4 : 20; } }

            public void PrintTo(TextWriter writer, LocalInstant instant, CalendarSystem calendarSystem, Offset timezoneOffset, DateTimeZone dateTimeZone,
                                IFormatProvider provider)
            {
                writer.Write(Print(instant - timezoneOffset, dateTimeZone, provider));
            }

            public void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider)
            {
                //no zone info
            }

            private String Print(Instant instant, DateTimeZone displayZone, IFormatProvider provider)
            {
                if (displayZone == null)
                {
                    return ""; // no zone
                }
                //TODO: can't be ported until the required changes in timezone API
                //switch (printKind)
                //{
                //    case TimeZoneNamePrintKind.LongName:
                //        return displayZone.GetName(instant);
                //    case TimeZoneNamePrintKind.ShortName:
                //        return displayZone.getShortName(instant, locale);
                //    case TimeZoneNamePrintKind.Id:
                //        return displayZone.Id;
                //}
                return "";
            }
        }

        private class TimeZoneOffset : IDateTimePrinter, IDateTimeParser
        {
            private readonly string zeroOffsetText;
            private readonly bool showSeparators;
            private readonly int minFields;
            private readonly int maxFields;

            public TimeZoneOffset(string zeroOffsetText, bool showSeparators, int minFields, int maxFields)
            {
                this.zeroOffsetText = zeroOffsetText;
                this.showSeparators = showSeparators;
                if (minFields <= 0 || maxFields < minFields)
                {
                    throw new ArgumentException();
                }
                if (minFields > 4)
                {
                    minFields = maxFields = 4;
                }

                this.minFields = minFields;
                this.maxFields = maxFields;
            }

            public int EstimatedPrintedLength
            {
                get
                {
                    int est = 1 + minFields << 1;
                    if (showSeparators)
                    {
                        est += minFields - 1;
                    }
                    if (zeroOffsetText != null && zeroOffsetText.Length > est)
                    {
                        est = zeroOffsetText.Length;
                    }
                    return est;
                }
            }

            public void PrintTo(TextWriter writer, LocalInstant instant, CalendarSystem calendarSystem, Offset timezoneOffset, DateTimeZone dateTimeZone,
                                IFormatProvider provider)
            {
                if (dateTimeZone == null)
                {
                    return;
                }

                if (timezoneOffset == Offset.Zero && zeroOffsetText != null)
                {
                    writer.Write(zeroOffsetText);
                    return;
                }
                if (timezoneOffset > Offset.Zero)
                {
                    writer.Write('+');
                }
                else
                {
                    writer.Write('-');
                    timezoneOffset = -timezoneOffset;
                }

                int hours = timezoneOffset.Milliseconds / NodaConstants.MillisecondsPerHour;
                FormatUtils.WritePaddedInteger(writer, hours, 2);
                if (maxFields == 1)
                {
                    return;
                }
                timezoneOffset = timezoneOffset - Offset.ForHours(hours);
                if (timezoneOffset == Offset.Zero && minFields <= 1)
                {
                    return;
                }

                int minutes = timezoneOffset.Milliseconds / NodaConstants.MillisecondsPerMinute;
                if (showSeparators)
                {
                    writer.Write(':');
                }
                FormatUtils.WritePaddedInteger(writer, minutes, 2);
                if (maxFields == 2)
                {
                    return;
                }
                timezoneOffset = timezoneOffset - new Offset(minutes * NodaConstants.MillisecondsPerMinute);
                if (timezoneOffset == Offset.Zero && minFields <= 2)
                {
                    return;
                }

                int seconds = timezoneOffset.Milliseconds / NodaConstants.MillisecondsPerSecond;
                if (showSeparators)
                {
                    writer.Write(':');
                }
                FormatUtils.WritePaddedInteger(writer, seconds, 2);
                if (maxFields == 3)
                {
                    return;
                }
                timezoneOffset = timezoneOffset - new Offset(seconds * NodaConstants.MillisecondsPerSecond);
                if (timezoneOffset == Offset.Zero && minFields <= 3)
                {
                    return;
                }

                if (showSeparators)
                {
                    writer.Write('.');
                }
                FormatUtils.WritePaddedInteger(writer, timezoneOffset.Milliseconds, 3);
            }

            public void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider)
            {
                //no zone info
            }

            public int EstimatedParsedLength { get { return EstimatedPrintedLength; } }

            public int ParseInto(DateTimeParserBucket bucket, string text, int position)
            {
                int limit = text.Length - position;

                zeroOffset:
                if (zeroOffsetText != null)
                {
                    if (zeroOffsetText.Length == 0)
                    {
                        // Peek ahead, looking for sign character.
                        if (limit > 0)
                        {
                            char c = text[position];
                            if (c == '-' || c == '+')
                            {
                                goto zeroOffset;
                            }
                        }
                        bucket.Offset = Offset.Zero;
                        return position;
                    }
                    if (FormatUtils.MatchSubstring(text, position, zeroOffsetText) > 0)
                    {
                        bucket.Offset = Offset.Zero;
                        return position + zeroOffsetText.Length;
                    }
                }

                // Format to expect is sign character followed by at least one digit.

                if (limit <= 1)
                {
                    return ~position;
                }

                bool negative;
                char cr = text[position];
                if (cr == '-')
                {
                    negative = true;
                }
                else if (cr == '+')
                {
                    negative = false;
                }
                else
                {
                    return ~position;
                }

                limit--;
                position++;

                // Format following sign is one of:
                //
                // hh
                // hhmm
                // hhmmss
                // hhmmssSSS
                // hh:mm
                // hh:mm:ss
                // hh:mm:ss.SSS

                // First parse hours.

                if (DigitCount(text, position, 2) < 2)
                {
                    // Need two digits for hour.
                    return ~position;
                }

                int offset;

                int hours = FormatUtils.ParseTwoDigits(text, position);
                if (hours > 23)
                {
                    return ~position;
                }

                offset = hours * NodaConstants.MillisecondsPerHour;
                limit -= 2;
                position += 2;

                parse:
                {
                    // Need to decide now if separators are expected or parsing
                    // stops at hour field.

                    if (limit <= 0)
                    {
                        goto parse;
                    }

                    bool expectSeparators;
                    cr = text[position];
                    if (cr == ':')
                    {
                        expectSeparators = true;
                        limit--;
                        position++;
                    }
                    else if (cr >= '0' && cr <= '9')
                    {
                        expectSeparators = false;
                    }
                    else
                    {
                        goto parse;
                    }

                    // Proceed to parse minutes.

                    int count = DigitCount(text, position, 2);
                    if (count == 0 && !expectSeparators)
                    {
                        goto parse;
                    }
                    else if (count < 2)
                    {
                        // Need two digits for minute.
                        return ~position;
                    }

                    int minutes = FormatUtils.ParseTwoDigits(text, position);
                    if (minutes > 59)
                    {
                        return ~position;
                    }
                    offset += minutes * NodaConstants.MillisecondsPerMinute;
                    limit -= 2;
                    position += 2;

                    // Proceed to parse seconds.

                    if (limit <= 0)
                    {
                        goto parse;
                    }

                    if (expectSeparators)
                    {
                        if (text[position] != ':')
                        {
                            goto parse;
                        }
                        limit--;
                        position++;
                    }

                    count = DigitCount(text, position, 2);
                    if (count == 0 && !expectSeparators)
                    {
                        goto parse;
                    }
                    else if (count < 2)
                    {
                        // Need two digits for second.
                        return ~position;
                    }

                    int seconds = FormatUtils.ParseTwoDigits(text, position);
                    if (seconds > 59)
                    {
                        return ~position;
                    }
                    offset += seconds * NodaConstants.MillisecondsPerSecond;
                    limit -= 2;
                    position += 2;

                    // Proceed to parse fraction of second.

                    if (limit <= 0)
                    {
                        goto parse;
                    }

                    if (expectSeparators)
                    {
                        if (text[position] != '.' && text[position] != ',')
                        {
                            goto parse;
                        }
                        limit--;
                        position++;
                    }

                    count = DigitCount(text, position, 3);
                    if (count == 0 && !expectSeparators)
                    {
                        goto parse;
                    }
                    else if (count < 1)
                    {
                        // Need at least one digit for fraction of second.
                        return ~position;
                    }

                    offset += (text[position++] - '0') * 100;
                    if (count > 1)
                    {
                        offset += (text[position++] - '0') * 10;
                        if (count > 2)
                        {
                            offset += text[position++] - '0';
                        }
                    }
                }

                bucket.Offset = new Offset(negative ? -offset : offset);
                return position;
            }

            private int DigitCount(String text, int position, int amount)
            {
                int limit = Math.Min(text.Length - position, amount);
                amount = 0;
                for (; limit > 0; limit--)
                {
                    char c = text[position + amount];
                    if (c < '0' || c > '9')
                    {
                        break;
                    }
                    amount++;
                }
                return amount;
            }
        }

        private class Composite : IDateTimePrinter, IDateTimeParser
        {
            private readonly IDateTimePrinter[] printers;
            private readonly IDateTimeParser[] parsers;

            private readonly int printedLengthEstimate;
            private readonly int parsedLengthEstimate;

            public Composite(List<object> elementPairs)
            {
                var printerList = new List<IDateTimePrinter>();
                var parserList = new List<IDateTimeParser>();

                Decompose(elementPairs, printerList, parserList);

                if (printerList.Count <= 0)
                {
                    printers = null;
                    printedLengthEstimate = 0;
                }
                else
                {
                    int size = printerList.Count;
                    printers = new IDateTimePrinter[size];
                    int printEst = 0;
                    for (int i = 0; i < size; i++)
                    {
                        IDateTimePrinter printer = printerList[i];
                        printEst += printer.EstimatedPrintedLength;
                        printers[i] = printer;
                    }
                    printedLengthEstimate = printEst;
                }

                if (parserList.Count <= 0)
                {
                    parsers = null;
                    parsedLengthEstimate = 0;
                }
                else
                {
                    int size = parserList.Count;
                    parsers = new IDateTimeParser[size];
                    int parseEst = 0;
                    for (int i = 0; i < size; i++)
                    {
                        IDateTimeParser parser = parserList[i];
                        parseEst += parser.EstimatedParsedLength;
                        parsers[i] = parser;
                    }
                    parsedLengthEstimate = parseEst;
                }
            }

            public int EstimatedPrintedLength { get { return printedLengthEstimate; } }

            public void PrintTo(TextWriter writer, LocalInstant instant, CalendarSystem calendarSystem, Offset timezoneOffset, DateTimeZone dateTimeZone,
                                IFormatProvider provider)
            {
                IDateTimePrinter[] elements = printers;
                if (elements == null)
                {
                    throw new NotSupportedException();
                }

                int len = elements.Length;
                for (int i = 0; i < len; i++)
                {
                    elements[i].PrintTo(writer, instant, calendarSystem, timezoneOffset, dateTimeZone, provider);
                }
            }

            public void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider)
            {
                IDateTimePrinter[] elements = printers;
                if (elements == null)
                {
                    throw new NotSupportedException();
                }

                int len = elements.Length;
                for (int i = 0; i < len; i++)
                {
                    elements[i].PrintTo(writer, partial, provider);
                }
            }

            public int EstimatedParsedLength { get { return parsedLengthEstimate; } }

            public int ParseInto(DateTimeParserBucket bucket, string text, int position)
            {
                IDateTimeParser[] elements = parsers;
                if (elements == null)
                {
                    throw new NotSupportedException();
                }

                int len = elements.Length;
                for (int i = 0; i < len && position >= 0; i++)
                {
                    position = elements[i].ParseInto(bucket, text, position);
                }
                return position;
            }

            public bool IsPrinter { get { return printers != null; } }

            public bool IsParser { get { return parsers != null; } }

            private static void Decompose(List<object> elementPairs, List<IDateTimePrinter> printers, List<IDateTimeParser> parsers)
            {
                int size = elementPairs.Count;
                for (int i = 0; i < size; i += 2)
                {
                    var element = elementPairs[i];
                    if (element is IDateTimePrinter)
                    {
                        if (element is Composite)
                        {
                            printers.AddRange(((Composite)element).printers);
                        }
                        else
                        {
                            printers.Add(element as IDateTimePrinter);
                        }
                    }

                    element = elementPairs[i + 1];
                    if (element is IDateTimeParser)
                    {
                        if (element is Composite)
                        {
                            parsers.AddRange(((Composite)element).parsers);
                        }
                        else
                        {
                            parsers.Add(element as IDateTimeParser);
                        }
                    }
                }
            }
        }
        #endregion

        private readonly List<object> elementPairs = new List<object>();
        private object formatter;

        /// <summary>
        /// Clears out all the appended elements, allowing this builder to be
        /// reused.
        /// </summary>
        public void Clear()
        {
            formatter = null;
            elementPairs.Clear();
        }

        #region Append
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

            return AppendPair(formatter.Printer, formatter.Parser);
        }

        /// <summary>
        /// Appends a printer/parser pair.
        /// </summary>
        /// <param name="printer">The printer to add</param>
        /// <param name="parser">The parser to add</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <exception cref="ArgumentNullException">If printer or parser is null</exception>
        internal DateTimeFormatterBuilder Append(IDateTimePrinter printer, IDateTimeParser parser)
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
        internal DateTimeFormatterBuilder Append(IDateTimePrinter printer)
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
        internal DateTimeFormatterBuilder Append(IDateTimeParser parser)
        {
            Guard(parser);

            return AppendPair(null, parser);
        }

        /// <summary>
        /// Appends a printer and a set of matching parsers.
        /// </summary>
        /// <param name="printer">The printer to add</param>
        /// <param name="parsers">The parsers to add</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <remarks>
        /// <para>
        /// When parsing, the first parser in the list is selected for parsing. 
        /// If it fails, the next is chosen, and so on. 
        /// If none of these parsers succeeds, 
        /// then the failed position of the parser that made the greatest progress is returned.
        /// </para>
        /// <para>
        /// Only the printer is optional. In addtion, it is illegal for any but the
        /// last of the parser array elements to be null. If the last element is
        /// null, this represents the empty parser. The presence of an empty parser
        /// indicates that the entire array of parse formats is optional.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException">If any parser element but the last is null</exception>
        internal DateTimeFormatterBuilder Append(IDateTimePrinter printer, IDateTimeParser[] parsers)
        {
            Guard(parsers);

            int length = parsers.Length;
            if (length == 1)
            {
                if (parsers[0] == null)
                {
                    throw new ArgumentException("No parser supplied");
                }
                return AppendPair(printer, parsers[0]);
            }

            IDateTimeParser[] copyOfParsers = new IDateTimeParser[length];
            int i;
            for (i = 0; i < length - 1; i++)
            {
                if ((copyOfParsers[i] = parsers[i]) == null)
                {
                    throw new ArgumentException("Incomplete parser array");
                }
            }
            copyOfParsers[i] = parsers[i];

            return AppendPair(printer, new MatchingParser(copyOfParsers));
        }

        /// <summary>
        /// Appends just a parser element which is optional. With no matching
        /// printer, a printer cannot be built from this DateTimeFormatterBuilder.
        /// </summary>
        /// <param name="parser"></param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <exception cref="ArgumentNullException">if parser is null</exception>
        internal DateTimeFormatterBuilder AppendOptional(IDateTimeParser parser)
        {
            Guard(parser);
            IDateTimeParser[] parsers = new[] { parser, null };
            return AppendPair(null, new MatchingParser(parsers));
        }

        #region Literal, Text
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
        /// Instructs the printer to emit specific text, and the parser to expect it. 
        /// The parser is case-insensitive.
        /// </summary>
        /// <param name="text">The text of the literal to append</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <exception cref="ArgumentNullException">If text argument is null or empty string</exception>
        public DateTimeFormatterBuilder AppendLiteral(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            return text.Length == 1 ? AppendObject(new CharacterLiteral(text[0])) : AppendObject(new StringLiteral(text));
        }

        /// <summary>
        /// Instructs the printer to emit a field value as text, and the
        /// parser to expect text.
        /// </summary>
        /// <param name="fieldType">Type of field to append</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <exception cref="ArgumentNullException">If field type is null</exception>
        internal DateTimeFormatterBuilder AppendText(DateTimeFieldType fieldType)
        {
            Guard(fieldType);

            return AppendObject(new TextField(fieldType, false));
        }

        /// <summary>
        /// Instructs the printer to emit a locale-specific era text (BC/AD), and
        /// the parser to expect it. The parser is case-insensitive.
        /// </summary>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendEraText()
        {
            return AppendText(DateTimeFieldType.Era);
        }

        /// <summary>
        /// Instructs the printer to emit a locale-specific month of year text.
        /// The parser will accept a long or short month of year text. 
        /// The parser is case-insensitive.
        /// </summary>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendMonthOfYearText()
        {
            return AppendText(DateTimeFieldType.MonthOfYear);
        }

        /// <summary>
        /// Instructs the printer to emit a locale-specific day of week text.
        /// The parser will accept a long or short day of week text. 
        /// The parser is case-insensitive.
        /// </summary>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendDayOfWeekText()
        {
            return AppendText(DateTimeFieldType.DayOfWeek);
        }

        /// <summary>
        /// Instructs the printer to emit a locale-specific AM/PM text, and the
        /// parser tp expect it. The parser is case-insensitive.
        /// </summary>
        /// <returns>This DateTimeFormatterBuilder</returns>
        internal DateTimeFormatterBuilder AppendHalfDayOfDayText()
        {
            return AppendText(DateTimeFieldType.HalfDayOfDay);
        }

        /// <summary>
        /// Instructs the printer to emit a field value as short text, and the
        /// parser to expect text.
        /// </summary>
        /// <param name="fieldType">Type of field to append</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <exception cref="ArgumentNullException">If field type is null</exception>
        internal DateTimeFormatterBuilder AppendShortText(DateTimeFieldType fieldType)
        {
            Guard(fieldType);

            return AppendObject(new TextField(fieldType, true));
        }

        /// <summary>
        /// Instructs the printer to emit a locale-specific short month of year text.
        /// The parser will accept a long or short month of year text. 
        /// The parser is case-insensitive.
        /// </summary>
        /// <returns>This DateTimeFormatterBuilder</returns>
        internal DateTimeFormatterBuilder AppendMonthOfYearShortText()
        {
            return AppendShortText(DateTimeFieldType.MonthOfYear);
        }

        /// <summary>
        /// Instructs the printer to emit a locale-specific short day of week text.
        /// The parser will accept a long or short day of week text. 
        /// The parser is case-insensitive.
        /// </summary>
        /// <returns>This DateTimeFormatterBuilder</returns>
        internal DateTimeFormatterBuilder AppendDayOfWeekShortText()
        {
            return AppendText(DateTimeFieldType.DayOfWeek);
        }
        #endregion

        #region Decimal fields
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
        internal DateTimeFormatterBuilder AppendDecimal(DateTimeFieldType fieldType, int minDigits, int maxDigits)
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

            return minDigits <= 1
                       ? AppendObject(new UnpaddedNumber(fieldType, maxDigits, false))
                       : AppendObject(new PaddedNumber(fieldType, maxDigits, minDigits, false));
        }

        /// <summary>
        /// Instructs the printer to emit a field value as a decimal number, and the
        /// parser to expect a signed decimal number.
        /// </summary>
        /// <param name="fieldType">Type of field to append</param>
        /// <param name="minDigits">Minumum number of digits to <i>print</i></param>
        /// <param name="maxDigits">Maximum number of digits to <i>parse</i>, or the estimated
        /// maximum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <exception cref="ArgumentNullException">If field type is null</exception>
        internal DateTimeFormatterBuilder AppendSignedDecimal(DateTimeFieldType fieldType, int minDigits, int maxDigits)
        {
            Guard(fieldType);

            if (maxDigits < minDigits)
            {
                maxDigits = minDigits;
            }
            if (minDigits < 0 || maxDigits <= 0)
            {
                throw new ArgumentException();
            }
            if (minDigits <= 1)
            {
                return AppendObject(new UnpaddedNumber(fieldType, maxDigits, true));
            }
            else
            {
                return AppendObject(new PaddedNumber(fieldType, maxDigits, minDigits, true));
            }
        }

        /// <summary>
        /// Instructs the printer to emit a field value as a fixed-width decimal
        /// number (smaller numbers will be left-padded with zeros), and the parser
        /// to expect an unsigned decimal number with the same fixed width.
        /// </summary>
        /// <param name="fieldType">Type of field to append</param>
        /// <param name="numDigits">The exact number of digits to parse or print, except if
        /// printed value requires more digits</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <exception cref="ArgumentNullException">If field type is null</exception>
        /// <exception cref="ArgumentException">if <code>numDigits &lt;= 0</code></exception>
        internal DateTimeFormatterBuilder AppendFixedDecimal(DateTimeFieldType fieldType, int numDigits)
        {
            Guard(fieldType);

            if (numDigits <= 0)
            {
                throw new ArgumentException("Illegal number of digits: " + numDigits);
            }
            return AppendObject(new FixedNumber(fieldType, numDigits, false));
        }

        /// <summary>
        /// Instructs the printer to emit a field value as a fixed-width decimal
        /// number (smaller numbers will be left-padded with zeros), and the parser
        /// to expect an signed decimal number with the same fixed width.
        /// </summary>
        /// <param name="fieldType">Type of field to append</param>
        /// <param name="numDigits">The exact number of digits to parse or print, except if
        /// printed value requires more digits</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <exception cref="ArgumentNullException">If field type is null</exception>
        /// <exception cref="ArgumentException">if <code>numDigits &lt;= 0</code></exception>
        internal DateTimeFormatterBuilder AppendFixedSignedDecimal(DateTimeFieldType fieldType, int numDigits)
        {
            Guard(fieldType);

            if (numDigits <= 0)
            {
                throw new ArgumentException("Illegal number of digits: " + numDigits);
            }
            return AppendObject(new FixedNumber(fieldType, numDigits, true));
        }

        /// <summary>
        /// Instructs the printer to emit a numeric century of era field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to <i>print</i></param>
        /// <param name="maxDigits">Maximum number of digits to <i>parse</i>, or the estimated
        /// maximum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendCenturyOfEra(int minDigits, int maxDigits)
        {
            return AppendSignedDecimal(DateTimeFieldType.CenturyOfEra, minDigits, maxDigits);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric year of era field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to <i>print</i></param>
        /// <param name="maxDigits">Maximum number of digits to <i>parse</i>, or the estimated
        /// maximum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendYearOfEra(int minDigits, int maxDigits)
        {
            return AppendDecimal(DateTimeFieldType.YearOfEra, minDigits, maxDigits);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric year of century field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to <i>print</i></param>
        /// <param name="maxDigits">Maximum number of digits to <i>parse</i>, or the estimated
        /// maximum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendYearOfCentury(int minDigits, int maxDigits)
        {
            return AppendDecimal(DateTimeFieldType.YearOfCentury, minDigits, maxDigits);
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
            return AppendSignedDecimal(DateTimeFieldType.Year, minDigits, maxDigits);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric month of year field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendMonthOfYear(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.MonthOfYear, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric week of weekyear field.
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
        public DateTimeFormatterBuilder AppendWeekYear(int minDigits, int maxDigits)
        {
            return AppendSignedDecimal(DateTimeFieldType.WeekYear, minDigits, maxDigits);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric day of year field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendDayOfYear(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.DayOfYear, minDigits, 3);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric day of month field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendDayOfMonth(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.DayOfMonth, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric day of week field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendDayOfWeek(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.DayOfWeek, minDigits, 1);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric hour of day field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendHourOfDay(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.HourOfDay, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric clock hour of day field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendClockHourOfDay(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.ClockHourOfDay, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric hour of halfday field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendHourOfHalfDay(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.HourOfHalfDay, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric clock hour of halfday field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendClockHourOfHalfDay(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.ClockHourOfHalfDay, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric minute of day field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendMinuteOfDay(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.MinuteOfDay, minDigits, 4);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric minute of hour field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendMinuteOfHour(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.MinuteOfHour, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric second of day field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendSecondOfDay(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.SecondOfDay, minDigits, 5);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric second of minute field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendSecondOfMinute(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.SecondOfMinute, minDigits, 2);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric milliseconds of day field.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendMillisecondsOfDay(int minDigits)
        {
            return AppendDecimal(DateTimeFieldType.MillisecondOfDay, minDigits, 8);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric milliseconds of second field.
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
        #endregion

        #region Two digit year
        /// <summary>
        /// Instructs the printer to emit a numeric year field which always prints
        /// two digits.
        /// </summary>
        /// <param name="pivot">Pivot year to use when parsing</param>
        /// <param name="lenientParse">When true, if digit count is not two, it is treated
        /// as an absolute year</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <remarks>
        /// A pivot year is used during parsing to determine the range
        /// of supported years as <code>(pivot - 50) .. (pivot + 49)</code>. If
        /// parse is instructed to be lenient and the digit count is not two, it is
        /// treated as an absolute year. With lenient parsing, specifying a positive
        /// or negative sign before the year also makes it absolute.        
        /// </remarks>
        public DateTimeFormatterBuilder AppendTwoDigitYear(int pivot, bool lenientParse)
        {
            return AppendObject(new TwoDigitYear(DateTimeFieldType.Year, pivot, lenientParse));
        }

        /// <summary>
        /// Instructs the printer to emit a numeric year field which always prints
        /// and parses two digits.
        /// </summary>
        /// <param name="pivot">Pivot year to use when parsing</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <remarks>
        ///  A pivot year is used during parsing to determine
        ///  the range of supported years as <code>(pivot - 50) .. (pivot + 49)</code>.
        ///  
        ///  pivot   supported range   00 is   20 is   40 is   60 is   80 is
        ///  ---------------------------------------------------------------
        ///  1950      1900..1999      1900    1920    1940    1960    1980
        ///  1975      1925..2024      2000    2020    1940    1960    1980
        ///  2000      1950..2049      2000    2020    2040    1960    1980
        ///  2025      1975..2074      2000    2020    2040    2060    1980
        ///  2050      2000..2099      2000    2020    2040    2060    2080
        /// </remarks>
        public DateTimeFormatterBuilder AppendTwoDigitYear(int pivot)
        {
            return AppendTwoDigitYear(pivot, false);
        }

        /// <summary>
        /// Instructs the printer to emit a numeric week year field which always prints
        /// two digits.
        /// </summary>
        /// <param name="pivot">Pivot week year to use when parsing</param>
        /// <param name="lenientParse">When true, if digit count is not two, it is treated
        /// as an absolute week year</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <remarks>
        /// A pivot year is used during parsing to determine the range
        /// of supported years as <code>(pivot - 50) .. (pivot + 49)</code>. If
        /// parse is instructed to be lenient and the digit count is not two, it is
        /// treated as an absolute week year. With lenient parsing, specifying a positive
        /// or negative sign before the week year also makes it absolute.        
        /// </remarks>
        public DateTimeFormatterBuilder AppendTwoDigitWeekYear(int pivot, bool lenientParse)
        {
            return AppendObject(new TwoDigitYear(DateTimeFieldType.WeekYear, pivot, lenientParse));
        }

        /// <summary>
        /// Instructs the printer to emit a numeric week year field which always prints
        /// and parses two digits.
        /// </summary>
        /// <param name="pivot">Pivot week year to use when parsing</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <remarks>
        ///  A pivot week year is used during parsing to determine
        ///  the range of supported years as <code>(pivot - 50) .. (pivot + 49)</code>.
        ///  
        ///  pivot   supported range   00 is   20 is   40 is   60 is   80 is
        ///  ---------------------------------------------------------------
        ///  1950      1900..1999      1900    1920    1940    1960    1980
        ///  1975      1925..2024      2000    2020    1940    1960    1980
        ///  2000      1950..2049      2000    2020    2040    1960    1980
        ///  2025      1975..2074      2000    2020    2040    2060    1980
        ///  2050      2000..2099      2000    2020    2040    2060    2080
        /// </remarks>
        public DateTimeFormatterBuilder AppendTwoDigitWeekYear(int pivot)
        {
            return AppendTwoDigitWeekYear(pivot, false);
        }
        #endregion

        #region Fraction
        /// <summary>
        /// Instructs the printer to emit a remainder of time as a decimal fraction, sans decimal point.
        /// </summary>
        /// <param name="fieldType">Type of field to append</param>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <param name="maxDigits">Maximum number of digits to print or parse</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        /// <example>
        /// If the field is specified as minuteOfHour and the time is 12:30:45, 
        /// the value printed is 75. A decimal point is implied, so the fraction is 0.75,
        /// or three-quarters of a minute.
        /// </example>
        internal DateTimeFormatterBuilder AppendFraction(DateTimeFieldType fieldType, int minDigits, int maxDigits)
        {
            Guard(fieldType);

            if (maxDigits < minDigits)
            {
                maxDigits = minDigits;
            }
            if (minDigits < 0 || maxDigits <= 0)
            {
                throw new ArgumentException();
            }

            return AppendObject(new Fraction(fieldType, minDigits, maxDigits));
        }

        /// <summary>
        /// Instructs the printer to emit a remainder of time as a decimal fraction, sans decimal point.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <param name="maxDigits">Maximum number of digits to print or parse</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendFractionOfSecond(int minDigits, int maxDigits)
        {
            return AppendFraction(DateTimeFieldType.SecondOfDay, minDigits, maxDigits);
        }

        /// <summary>
        /// Instructs the printer to emit a remainder of time as a decimal fraction, sans decimal point.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <param name="maxDigits">Maximum number of digits to print or parse</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendFractionOfMinute(int minDigits, int maxDigits)
        {
            return AppendFraction(DateTimeFieldType.MinuteOfDay, minDigits, maxDigits);
        }

        /// <summary>
        /// Instructs the printer to emit a remainder of time as a decimal fraction, sans decimal point.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <param name="maxDigits">Maximum number of digits to print or parse</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendFractionOfHour(int minDigits, int maxDigits)
        {
            return AppendFraction(DateTimeFieldType.HourOfDay, minDigits, maxDigits);
        }

        /// <summary>
        /// Instructs the printer to emit a remainder of time as a decimal fraction, sans decimal point.
        /// </summary>
        /// <param name="minDigits">Minumum number of digits to print</param>
        /// <param name="maxDigits">Maximum number of digits to print or parse</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendFractionOfDay(int minDigits, int maxDigits)
        {
            return AppendFraction(DateTimeFieldType.DayOfYear, minDigits, maxDigits);
        }
        #endregion

        #region TimeZone
        /// <summary>
        /// Instructs the printer to emit a locale-specific time zone name. 
        /// A parser cannot be created from this builder 
        /// if a time zone name is appended.
        /// </summary>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendTimeZoneName()
        {
            return Append(new TimeZoneName(TimeZoneNamePrintKind.LongName), (IDateTimeParser)null);
        }

        /// <summary>
        /// Instructs the printer to emit a short locale-specific time zone name. 
        /// A parser cannot be created from this builder 
        /// if a time zone name is appended.
        /// </summary>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendTimeZoneShortName()
        {
            return Append(new TimeZoneName(TimeZoneNamePrintKind.ShortName), (IDateTimeParser)null);
        }

        /// <summary>
        /// Instructs the printer to emit the identifier of the time zone. 
        /// This field cannot currently be parsed. 
        /// </summary>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendTimeZoneId()
        {
            return Append(new TimeZoneName(TimeZoneNamePrintKind.Id), (IDateTimeParser)null);
        }

        /// <summary>
        /// Instructs the printer to emit text and numbers to display time zone
        /// offset from UTC. A parser will use the parsed time zone offset to adjust
        /// the datetime.
        /// </summary>
        /// <param name="zeroOffsetText">Text to use if time zone offset is zero. If
        /// null, offset is always shown.</param>
        /// <param name="showSeparators">If true, prints ':' separator before minute and
        /// second field and prints '.' separator before fraction field.</param>
        /// <param name="minFields">Minimum number of fields to print, stopping when no
        /// more precision is required. 1=hours, 2=minutes, 3=seconds, 4=fraction</param>
        /// <param name="maxFields">Maximum number of fields to print</param>
        /// <returns>This DateTimeFormatterBuilder</returns>
        public DateTimeFormatterBuilder AppendTimeZoneOffset(String zeroOffsetText, bool showSeparators, int minFields, int maxFields)
        {
            return AppendObject(new TimeZoneOffset(zeroOffsetText, showSeparators, minFields, maxFields));
        }
        #endregion

        #endregion

        #region Composition
        /// <summary>
        /// Returns true if toPrinter can be called without throwing an
        /// NotSupportedException.
        /// </summary>
        /// <returns>true if a printer can be built</returns>
        public bool CanBuildPrinter()
        {
            return IsPrinter(GetFormatter());
        }

        /// <summary>
        /// Returns true if toParser can be called without throwing an
        /// NotSupportedException.
        /// </summary>
        /// <returns>true if a parser can be built</returns>
        public bool CanBuildParser()
        {
            return IsParser(GetFormatter());
        }

        /// <summary>
        /// Returns true if toFormatter can be called without throwing an
        /// NotSupportedException.
        /// </summary>
        /// <returns>true if a formatter can be built</returns>
        public bool CanBuildFormatter()
        {
            return IsFormatter(GetFormatter());
        }

        /// <summary>
        /// Internal method to create a IDateTimePrinter instance using all the
        /// appended elements.
        /// </summary>
        /// <returns>Constructed IDateTimePrinter</returns>
        /// <remarks>
        /// <para>
        /// Most applications will not use this method.
        /// If you want a printer in an application, call <see cref="ToFormatter"/>
        /// and just use the printing API.
        /// </para>
        /// <para>
        /// Subsequent changes to this builder do not affect the returned printer.
        /// </para>
        /// </remarks>
        /// <exception cref="NotSupportedException">If printing is not supported</exception>
        internal IDateTimePrinter ToPrinter()
        {
            var f = GetFormatter();
            if (IsPrinter(f))
            {
                return (IDateTimePrinter)f;
            }
            throw new NotSupportedException("Printing is not supported");
        }

        /// <summary>
        /// Internal method to create a DateTimeParser instance using all the
        /// appended elements.
        /// </summary>
        /// <returns>Constructed IDateTimeParser</returns>
        /// <remarks>
        /// <para>
        /// Most applications will not use this method.
        /// If you want a parser in an application, call <see cref="ToFormatter"/>
        /// and just use the parsing API.
        /// </para>
        /// <para>
        /// Subsequent changes to this builder do not affect the returned parser.
        /// </para>
        /// </remarks>
        /// <exception cref="NotSupportedException">If parsing is not supported</exception>
        internal IDateTimeParser ToParser()
        {
            var f = GetFormatter();
            if (IsParser(f))
            {
                return (IDateTimeParser)f;
            }
            throw new NotSupportedException("Parsing is not supported");
        }

        /// <summary>
        /// Constructs a DateTimeFormatter using all the appended elements.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This is the main method used by applications at the end of the build
        /// process to create a usable formatter.
        /// <para>
        /// Subsequent changes to this builder do not affect the returned formatter.
        /// </para> 
        /// <para>
        /// The returned formatter may not support both printing and parsing.
        /// </para>
        /// <para>
        /// The methods <see cref="IsPrinter"/> and
        /// <see cref="IsParser"/> will help you determine the state
        /// of the formatter.
        /// </para>
        /// </remarks>
        /// <exception cref="NotSupportedException">if neither printing nor parsing is supported</exception>
        public DateTimeFormatter ToFormatter()
        {
            Object f = GetFormatter();
            IDateTimePrinter printer = null;
            if (IsPrinter(f))
            {
                printer = (IDateTimePrinter)f;
            }
            IDateTimeParser parser = null;
            if (IsParser(f))
            {
                parser = (IDateTimeParser)f;
            }
            if (printer != null || parser != null)
            {
                return new DateTimeFormatter(printer, parser);
            }
            throw new NotSupportedException("Both printing and parsing not supported");
        }

        private bool IsPrinter(Object @object)
        {
            if (@object is IDateTimePrinter)
            {
                if (@object is Composite)
                {
                    return ((Composite)@object).IsPrinter;
                }
                return true;
            }
            return false;
        }

        private bool IsParser(Object @object)
        {
            if (@object is IDateTimeParser)
            {
                if (@object is Composite)
                {
                    return ((Composite)@object).IsParser;
                }
                return true;
            }
            return false;
        }

        private bool IsFormatter(Object @object)
        {
            return IsPrinter(@object) || IsParser(@object);
        }

        private Object GetFormatter()
        {
            Object f = formatter;

            if (f == null)
            {
                if (elementPairs.Count == 2)
                {
                    Object printer = elementPairs[0];
                    Object parser = elementPairs[1];

                    if (printer != null)
                    {
                        if (printer == parser || parser == null)
                        {
                            f = printer;
                        }
                    }
                    else
                    {
                        f = parser;
                    }
                }

                if (f == null)
                {
                    f = new Composite(elementPairs);
                }

                formatter = f;
            }

            return f;
        }
        #endregion

        #region Guards
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

        private static void Guard(IDateTimeParser[] parsers)
        {
            if (parsers == null)
            {
                throw new ArgumentNullException("parsers");
            }
        }
        #endregion
    }
}