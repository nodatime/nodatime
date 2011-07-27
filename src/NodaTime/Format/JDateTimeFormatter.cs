using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NodaTime.Format
{
    /*
    public static class JDateTimeFormatter
    {
        // Fields
        internal static char[] allStandardFormats = new char[] { 
        'd', 'D', 'f', 'F', 'g', 'G', 'm', 'M', 'o', 'O', 'r', 'R', 's', 't', 'T', 'u', 
        'U', 'y', 'Y'
     };
        //private const int DEFAULT_ALL_DATETIMES_SIZE = 0x84;
        internal static string[] fixedNumberFormats = new string[] { "0", "00", "000", "0000", "00000", "000000", "0000000" };
        internal const int MaxSecondsFractionDigits = 7;
        internal static readonly TimeSpan NullOffset = TimeSpan.MinValue;
        internal const string RoundtripDateTimeUnfixed = "yyyy'-'MM'-'ddTHH':'mm':'ss zzz";
        internal const string RoundtripFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK";

        // Methods
        private static string ExpandPredefinedFormat(string format, ref DateTime dateTime, ref DateTimeFormatInfo dtfi, ref TimeSpan offset)
        {
            switch (format[0])
            {
                case 'o':
                case 'O':
                    dtfi = DateTimeFormatInfo.InvariantInfo;
                    goto Label_0160;

                case 'r':
                case 'R':
                    if (offset != NullOffset)
                    {
                        dateTime -= offset;
                    }
                    else if (dateTime.Kind == DateTimeKind.Local)
                    {
                        InvalidFormatForLocal(format, dateTime);
                    }
                    dtfi = DateTimeFormatInfo.InvariantInfo;
                    goto Label_0160;

                case 's':
                    dtfi = DateTimeFormatInfo.InvariantInfo;
                    goto Label_0160;

                case 'u':
                    if (offset == NullOffset)
                    {
                        if (dateTime.Kind == DateTimeKind.Local)
                        {
                            InvalidFormatForLocal(format, dateTime);
                        }
                        break;
                    }
                    dateTime -= offset;
                    break;

                case 'U':
                    if (offset != NullOffset)
                    {
                        throw new FormatException("Format_InvalidString");
                    }
                    dtfi = (DateTimeFormatInfo)dtfi.Clone();
                    if (dtfi.Calendar.GetType() != typeof(GregorianCalendar))
                    {
                        dtfi.Calendar = new GregorianCalendar();
                    }
                    dateTime = dateTime.ToUniversalTime();
                    goto Label_0160;

                default:
                    goto Label_0160;
            }
            dtfi = DateTimeFormatInfo.InvariantInfo;
        Label_0160:
            format = GetRealFormat(format, dtfi);
            return format;
        }

        internal static string Format(DateTime dateTime, string format, DateTimeFormatInfo dtfi)
        {
            return Format(dateTime, format, dtfi, NullOffset);
        }

        internal static string Format(DateTime dateTime, string format, DateTimeFormatInfo dtfi, TimeSpan offset)
        {
            if (string.IsNullOrEmpty(format))
            {
                bool flag = false;
                if (dateTime.Ticks < 0xc92a69c000L)
                {
                    switch (dtfi.Calendar.ID)
                    {
                        case 3:
                        case 4:
                        case 6:
                        case 8:
                        case 13:
                        case 0x17:
                            flag = true;
                            dtfi = DateTimeFormatInfo.InvariantInfo;
                            break;
                    }
                }
                if (offset == NullOffset)
                {
                    format = flag ? "s" : "G";
                }
                else if (flag)
                {
                    format = "yyyy'-'MM'-'ddTHH':'mm':'ss zzz";
                }
                else
                {
                    format = dtfi.DateTimeOffsetPattern;
                }
            }
            if (format.Length == 1)
            {
                format = ExpandPredefinedFormat(format, ref dateTime, ref dtfi, ref offset);
            }
            return FormatCustomized(dateTime, format, dtfi, offset);
        }

        private static string FormatCustomized(DateTime dateTime, string format, DateTimeFormatInfo dtfi, TimeSpan offset)
        {
            int num2;
            Calendar calendar = dtfi.Calendar;
            var outputBuffer = new StringBuilder();
            bool flag = calendar.ID == 8;
            bool timeOnly = true;
            for (int i = 0; i < format.Length; i += num2)
            {
                int num4;
                int dayOfMonth;
                int month;
                int year;
                char patternChar = format[i];
                switch (patternChar)
                {
                    case 'F':
                    case 'f':
                        break;

                    case 'H':
                            num2 = ParseRepeatPattern(format, i, patternChar);
                            FormatDigits(outputBuffer, dateTime.Hour, num2);
                            continue;
                    case ':':
                            outputBuffer.Append(dtfi.TimeSeparator);
                            num2 = 1;
                            continue;
                    case '/':
                            outputBuffer.Append(dtfi.DateSeparator);
                            num2 = 1;
                            continue;
                    case '%':
                            num4 = ParseNextChar(format, i);
                            if ((num4 < 0) || (num4 == 0x25))
                            {
                                throw new FormatException("Format_InvalidString");
                            }
                            char ch3 = (char)num4;
                            outputBuffer.Append(FormatCustomized(dateTime, ch3.ToString(), dtfi, offset));
                            num2 = 2;
                            continue;
                    case '\'':
                    case '"':
                            var result = new StringBuilder();
                            num2 = ParseQuoteString(format, i, result);
                            outputBuffer.Append(result);
                            continue;
                    case 'K':
                            num2 = 1;
                            FormatCustomizedRoundripTimeZone(dateTime, offset, outputBuffer);
                            continue;
                    case 'M':
                        num2 = ParseRepeatPattern(format, i, patternChar);
                        month = calendar.GetMonth(dateTime);
                        if (num2 > 2)
                        {
                            goto Label_03D3;
                        }
                        if (!flag)
                        {
                            goto Label_03C7;
                        }
                        HebrewFormatDigits(outputBuffer, month);
                        goto Label_042E;

                    case '\\':
                        {
                            num4 = ParseNextChar(format, i);
                            if (num4 < 0)
                            {
                                throw new FormatException(Environment.GetResourceString("Format_InvalidString"));
                            }
                            outputBuffer.Append((char)num4);
                            num2 = 2;
                            continue;
                        }
                    case 'd':
                        num2 = ParseRepeatPattern(format, i, patternChar);
                        if (num2 > 2)
                        {
                            goto Label_037F;
                        }
                        dayOfMonth = calendar.GetDayOfMonth(dateTime);
                        if (!flag)
                        {
                            goto Label_0373;
                        }
                        HebrewFormatDigits(outputBuffer, dayOfMonth);
                        goto Label_0399;

                    case 'g':
                        {
                            num2 = ParseRepeatPattern(format, i, patternChar);
                            outputBuffer.Append(dtfi.GetEraName(calendar.GetEra(dateTime)));
                            continue;
                        }
                    case 'h':
                        {
                            num2 = ParseRepeatPattern(format, i, patternChar);
                            int num3 = dateTime.Hour % 12;
                            if (num3 == 0)
                            {
                                num3 = 12;
                            }
                            FormatDigits(outputBuffer, num3, num2);
                            continue;
                        }
                    case 's':
                        {
                            num2 = ParseRepeatPattern(format, i, patternChar);
                            FormatDigits(outputBuffer, dateTime.Second, num2);
                            continue;
                        }
                    case 't':
                        {
                            num2 = ParseRepeatPattern(format, i, patternChar);
                            if (num2 != 1)
                            {
                                goto Label_0327;
                            }
                            if (dateTime.Hour >= 12)
                            {
                                goto Label_02FE;
                            }
                            if (dtfi.AMDesignator.Length >= 1)
                            {
                                outputBuffer.Append(dtfi.AMDesignator[0]);
                            }
                            continue;
                        }
                    case 'm':
                        {
                            num2 = ParseRepeatPattern(format, i, patternChar);
                            FormatDigits(outputBuffer, dateTime.Minute, num2);
                            continue;
                        }
                    case 'y':
                        year = calendar.GetYear(dateTime);
                        num2 = ParseRepeatPattern(format, i, patternChar);
                        if (!dtfi.HasForceTwoDigitYears)
                        {
                            goto Label_0466;
                        }
                        FormatDigits(outputBuffer, year, (num2 <= 2) ? num2 : 2);
                        goto Label_04B5;

                    case 'z':
                        {
                            num2 = ParseRepeatPattern(format, i, patternChar);
                            FormatCustomizedTimeZone(dateTime, offset, format, num2, timeOnly, outputBuffer);
                            continue;
                        }
                    default:
                        goto Label_05A4;
                }
                num2 = ParseRepeatPattern(format, i, patternChar);
                if (num2 > 7)
                {
                    throw new FormatException(Environment.GetResourceString("Format_InvalidString"));
                }
                long num5 = dateTime.Ticks % 0x989680L;
                num5 /= (long)Math.Pow(10.0, (double)(7 - num2));
                if (patternChar == 'f')
                {
                    outputBuffer.Append(((int)num5).ToString(fixedNumberFormats[num2 - 1], CultureInfo.InvariantCulture));
                    continue;
                }
                int num6 = num2;
                while (num6 > 0)
                {
                    if ((num5 % 10L) != 0L)
                    {
                        break;
                    }
                    num5 /= 10L;
                    num6--;
                }
                if (num6 > 0)
                {
                    outputBuffer.Append(((int)num5).ToString(fixedNumberFormats[num6 - 1], CultureInfo.InvariantCulture));
                }
                else if ((outputBuffer.Length > 0) && (outputBuffer[outputBuffer.Length - 1] == '.'))
                {
                    outputBuffer.Remove(outputBuffer.Length - 1, 1);
                }
                continue;
            Label_02FE:
                if (dtfi.PMDesignator.Length >= 1)
                {
                    outputBuffer.Append(dtfi.PMDesignator[0]);
                }
                continue;
            Label_0327:
                outputBuffer.Append((dateTime.Hour < 12) ? dtfi.AMDesignator : dtfi.PMDesignator);
                continue;
            Label_0373:
                FormatDigits(outputBuffer, dayOfMonth, num2);
                goto Label_0399;
            Label_037F:
                int num8 = (int)calendar.GetDayOfWeek(dateTime);
                outputBuffer.Append(FormatDayOfWeek(num8, num2, dtfi));
            Label_0399:
                timeOnly = false;
                continue;
            Label_03C7:
                FormatDigits(outputBuffer, month, num2);
                goto Label_042E;
            Label_03D3:
                if (flag)
                {
                    outputBuffer.Append(FormatHebrewMonthName(dateTime, month, num2, dtfi));
                }
                else if (((dtfi.FormatFlags & DateTimeFormatFlags.UseGenitiveMonth) != DateTimeFormatFlags.None) && (num2 >= 4))
                {
                    outputBuffer.Append(dtfi.internalGetMonthName(month, IsUseGenitiveForm(format, i, num2, 'd') ? MonthNameStyles.Genitive : MonthNameStyles.Regular, false));
                }
                else
                {
                    outputBuffer.Append(FormatMonth(month, num2, dtfi));
                }
            Label_042E:
                timeOnly = false;
                continue;
            Label_0466:
                if (calendar.ID == 8)
                {
                    HebrewFormatDigits(outputBuffer, year);
                }
                else if (num2 <= 2)
                {
                    FormatDigits(outputBuffer, year % 100, num2);
                }
                else
                {
                    string str = "D" + num2;
                    outputBuffer.Append(year.ToString(str, CultureInfo.InvariantCulture));
                }
            Label_04B5:
                timeOnly = false;
                continue;
            Label_05A4:
                outputBuffer.Append(patternChar);
                num2 = 1;
            }
            return outputBuffer.ToString();
        }

        private static void FormatCustomizedRoundripTimeZone(DateTime dateTime, TimeSpan offset, StringBuilder result)
        {
            if (offset == NullOffset)
            {
                switch (dateTime.Kind)
                {
                    case DateTimeKind.Utc:
                        result.Append("Z");
                        return;

                    case DateTimeKind.Local:
                        offset = TimeZoneInfo.Local.GetUtcOffset(dateTime, TimeZoneInfoOptions.NoThrowOnInvalidTime);
                        goto Label_0043;
                }
                return;
            }
        Label_0043:
            if (offset >= TimeSpan.Zero)
            {
                result.Append('+');
            }
            else
            {
                result.Append('-');
                offset = offset.Negate();
            }
            result.AppendFormat(CultureInfo.InvariantCulture, "{0:00}:{1:00}", new object[] { offset.Hours, offset.Minutes });
        }

        private static void FormatCustomizedTimeZone(DateTime dateTime, TimeSpan offset, string format, int tokenLen, bool timeOnly, StringBuilder result)
        {
            if (offset == NullOffset)
            {
                if (timeOnly && (dateTime.Ticks < 0xc92a69c000L))
                {
                    offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now, TimeZoneInfoOptions.NoThrowOnInvalidTime);
                }
                else if (dateTime.Kind == DateTimeKind.Utc)
                {
                    InvalidFormatForUtc(format, dateTime);
                    dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
                    offset = TimeZoneInfo.Local.GetUtcOffset(dateTime, TimeZoneInfoOptions.NoThrowOnInvalidTime);
                }
                else
                {
                    offset = TimeZoneInfo.Local.GetUtcOffset(dateTime, TimeZoneInfoOptions.NoThrowOnInvalidTime);
                }
            }
            if (offset >= TimeSpan.Zero)
            {
                result.Append('+');
            }
            else
            {
                result.Append('-');
                offset = offset.Negate();
            }
            if (tokenLen <= 1)
            {
                result.AppendFormat(CultureInfo.InvariantCulture, "{0:0}", new object[] { offset.Hours });
            }
            else
            {
                result.AppendFormat(CultureInfo.InvariantCulture, "{0:00}", new object[] { offset.Hours });
                if (tokenLen >= 3)
                {
                    result.AppendFormat(CultureInfo.InvariantCulture, ":{0:00}", new object[] { offset.Minutes });
                }
            }
        }

        private static string FormatDayOfWeek(int dayOfWeek, int repeat, DateTimeFormatInfo dtfi)
        {
            if (repeat == 3)
            {
                return dtfi.GetAbbreviatedDayName((DayOfWeek)dayOfWeek);
            }
            return dtfi.GetDayName((DayOfWeek)dayOfWeek);
        }

        [SecuritySafeCritical, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal static void FormatDigits(StringBuilder outputBuffer, int value, int len)
        {
            FormatDigits(outputBuffer, value, len, false);
        }

        [SecuritySafeCritical]
        internal static unsafe void FormatDigits(StringBuilder outputBuffer, int value, int len, bool overrideLengthLimit)
    {
        if (!overrideLengthLimit && (len > 2))
        {
            len = 2;
        }
        char* chPtr = (char*) stackalloc byte[(((IntPtr) 0x10) * 2)];
        char* chPtr2 = chPtr + 0x10;
        int num = value;
        do
        {
            *(--chPtr2) = (char) ((num % 10) + 0x30);
            num /= 10;
        }
        while ((num != 0) && (chPtr2 > chPtr));
        int valueCount = (int) ((long) (((chPtr + 0x10) - chPtr2) / 2));
        while ((valueCount < len) && (chPtr2 > chPtr))
        {
            *(--chPtr2) = '0';
            valueCount++;
        }
        outputBuffer.Append(chPtr2, valueCount);
    }

        private static string FormatHebrewMonthName(DateTime time, int month, int repeatCount, DateTimeFormatInfo dtfi)
        {
            if (dtfi.Calendar.IsLeapYear(dtfi.Calendar.GetYear(time)))
            {
                return dtfi.internalGetMonthName(month, MonthNameStyles.LeapYear, repeatCount == 3);
            }
            if (month >= 7)
            {
                month++;
            }
            if (repeatCount == 3)
            {
                return dtfi.GetAbbreviatedMonthName(month);
            }
            return dtfi.GetMonthName(month);
        }

        private static string FormatMonth(int month, int repeatCount, DateTimeFormatInfo dtfi)
        {
            if (repeatCount == 3)
            {
                return dtfi.GetAbbreviatedMonthName(month);
            }
            return dtfi.GetMonthName(month);
        }

        internal static string[] GetAllDateTimes(DateTime dateTime, DateTimeFormatInfo dtfi)
        {
            List<string> list = new List<string>(0x84);
            for (int i = 0; i < allStandardFormats.Length; i++)
            {
                string[] strArray = GetAllDateTimes(dateTime, allStandardFormats[i], dtfi);
                for (int j = 0; j < strArray.Length; j++)
                {
                    list.Add(strArray[j]);
                }
            }
            string[] array = new string[list.Count];
            list.CopyTo(0, array, 0, list.Count);
            return array;
        }

        internal static string[] GetAllDateTimes(DateTime dateTime, char format, DateTimeFormatInfo dtfi)
        {
            string[] allDateTimePatterns = null;
            string[] strArray2 = null;
            switch (format)
            {
                case 'D':
                case 'F':
                case 'G':
                case 'M':
                case 'T':
                case 'd':
                case 'f':
                case 'g':
                case 'Y':
                case 'm':
                case 't':
                case 'y':
                    allDateTimePatterns = dtfi.GetAllDateTimePatterns(format);
                    strArray2 = new string[allDateTimePatterns.Length];
                    for (int i = 0; i < allDateTimePatterns.Length; i++)
                    {
                        strArray2[i] = Format(dateTime, allDateTimePatterns[i], dtfi);
                    }
                    return strArray2;

                case 'O':
                case 'R':
                case 'o':
                case 'r':
                case 's':
                case 'u':
                    return new string[] { Format(dateTime, new string(new char[] { format }), dtfi) };

                case 'U':
                    {
                        DateTime time = dateTime.ToUniversalTime();
                        allDateTimePatterns = dtfi.GetAllDateTimePatterns(format);
                        strArray2 = new string[allDateTimePatterns.Length];
                        for (int j = 0; j < allDateTimePatterns.Length; j++)
                        {
                            strArray2[j] = Format(time, allDateTimePatterns[j], dtfi);
                        }
                        return strArray2;
                    }
            }
            throw new FormatException(Environment.GetResourceString("Format_InvalidString"));
        }

        internal static string GetRealFormat(string format, DateTimeFormatInfo dtfi)
        {
            switch (format[0])
            {
                case 'D':
                    return dtfi.LongDatePattern;

                case 'F':
                    return dtfi.FullDateTimePattern;

                case 'G':
                    return dtfi.GeneralLongTimePattern;

                case 'M':
                case 'm':
                    return dtfi.MonthDayPattern;

                case 'O':
                case 'o':
                    return "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK";

                case 'R':
                case 'r':
                    return dtfi.RFC1123Pattern;

                case 'T':
                    return dtfi.LongTimePattern;

                case 'U':
                    return dtfi.FullDateTimePattern;

                case 'd':
                    return dtfi.ShortDatePattern;

                case 'f':
                    return (dtfi.LongDatePattern + " " + dtfi.ShortTimePattern);

                case 'g':
                    return dtfi.GeneralShortTimePattern;

                case 'Y':
                case 'y':
                    return dtfi.YearMonthPattern;

                case 's':
                    return dtfi.SortableDateTimePattern;

                case 't':
                    return dtfi.ShortTimePattern;

                case 'u':
                    return dtfi.UniversalSortableDateTimePattern;
            }
            throw new FormatException(Environment.GetResourceString("Format_InvalidString"));
        }

        private static void HebrewFormatDigits(StringBuilder outputBuffer, int digits)
        {
            outputBuffer.Append(HebrewNumber.ToString(digits));
        }

        internal static void InvalidFormatForLocal(string format, DateTime dateTime)
        {
        }

        [SecuritySafeCritical]
        internal static void InvalidFormatForUtc(string format, DateTime dateTime)
        {
            Mda.DateTimeInvalidLocalFormat();
        }

        private static bool IsUseGenitiveForm(string format, int index, int tokenLen, char patternToMatch)
        {
            int num2 = 0;
            int num = index - 1;
            while ((num >= 0) && (format[num] != patternToMatch))
            {
                num--;
            }
            if (num >= 0)
            {
                while ((--num >= 0) && (format[num] == patternToMatch))
                {
                    num2++;
                }
                if (num2 <= 1)
                {
                    return true;
                }
            }
            num = index + tokenLen;
            while ((num < format.Length) && (format[num] != patternToMatch))
            {
                num++;
            }
            if (num < format.Length)
            {
                num2 = 0;
                while ((++num < format.Length) && (format[num] == patternToMatch))
                {
                    num2++;
                }
                if (num2 <= 1)
                {
                    return true;
                }
            }
            return false;
        }

        internal static int ParseNextChar(string format, int pos)
        {
            if (pos >= (format.Length - 1))
            {
                return -1;
            }
            return format[pos + 1];
        }

        internal static int ParseQuoteString(string format, int pos, StringBuilder result)
        {
            int length = format.Length;
            int num2 = pos;
            char ch = format[pos++];
            bool flag = false;
            while (pos < length)
            {
                char ch2 = format[pos++];
                if (ch2 == ch)
                {
                    flag = true;
                    break;
                }
                if (ch2 == '\\')
                {
                    if (pos >= length)
                    {
                        throw new FormatException(Environment.GetResourceString("Format_InvalidString"));
                    }
                    result.Append(format[pos++]);
                }
                else
                {
                    result.Append(ch2);
                }
            }
            if (!flag)
            {
                throw new FormatException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Format_BadQuote"), new object[] { ch }));
            }
            return (pos - num2);
        }

        internal static int ParseRepeatPattern(string format, int pos, char patternChar)
        {
            int length = format.Length;
            int num2 = pos + 1;
            while ((num2 < length) && (format[num2] == patternChar))
            {
                num2++;
            }
            return (num2 - pos);
        }
    }
    */
}

