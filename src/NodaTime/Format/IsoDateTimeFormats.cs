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

using NodaTime.Fields;
using NodaTime.TimeZones;

namespace NodaTime.Format
{
    /// <summary>
    /// Original name: ISODateTimeFormat.
    /// This has a non-private constructor in Joda Time, but it's not clear
    /// to me why, given that it only has static methods...
    /// </summary>
    public static class IsoDateTimeFormats
    {
        // year element (yyyy)
        private static readonly DateTimeFormatter ye = new DateTimeFormatterBuilder().AppendYear(4, 9).ToFormatter();

        // monthOfYear element (-MM)
        private static readonly DateTimeFormatter mye = new DateTimeFormatterBuilder().AppendLiteral('-').AppendMonthOfYear(2).ToFormatter();

        // dayOfMonth element (-dd)
        private static readonly DateTimeFormatter dme = new DateTimeFormatterBuilder().AppendLiteral('-').AppendDayOfMonth(2).ToFormatter();

        // weekyear element (xxxx)
        private static readonly DateTimeFormatter we = new DateTimeFormatterBuilder().AppendWeekYear(4, 9).ToFormatter();

        // weekOfWeekyear element (-ww)
        private static readonly DateTimeFormatter wwe = new DateTimeFormatterBuilder().AppendLiteral("-W").AppendWeekOfWeekYear(2).ToFormatter();

        // dayOfWeek element (-ee)
        private static readonly DateTimeFormatter dwe = new DateTimeFormatterBuilder().AppendLiteral('-').AppendDayOfWeek(1).ToFormatter();

        // dayOfYear element (-DDD)
        private static readonly DateTimeFormatter dye = new DateTimeFormatterBuilder().AppendLiteral('-').AppendDayOfYear(3).ToFormatter();

        // hourOfDay element (HH)
        private static readonly DateTimeFormatter hde = new DateTimeFormatterBuilder().AppendHourOfDay(2).ToFormatter();

        // minuteOfHour element (:mm)
        private static readonly DateTimeFormatter mhe = new DateTimeFormatterBuilder().AppendLiteral(':').AppendMinuteOfHour(2).ToFormatter();

        // secondOfMinute element (:ss)
        private static readonly DateTimeFormatter sme = new DateTimeFormatterBuilder().AppendLiteral(':').AppendSecondOfMinute(2).ToFormatter();

        // fractionOfSecond element (.SSSSSSSSS)
        private static readonly DateTimeFormatter fse = new DateTimeFormatterBuilder().AppendLiteral('.').AppendFractionOfSecond(3, 9).ToFormatter();

        // zone offset element
        private static readonly DateTimeFormatter ze = new DateTimeFormatterBuilder().AppendTimeZoneOffset("Z", true, 2, 4).ToFormatter();

        // literal 'T' element
        private static readonly DateTimeFormatter lte = new DateTimeFormatterBuilder().AppendLiteral('T').ToFormatter();

        // year month
        private static readonly DateTimeFormatter ym = new DateTimeFormatterBuilder().Append(ye).Append(mye).ToFormatter();

        // year month day
        private static readonly DateTimeFormatter ymd = new DateTimeFormatterBuilder().Append(ye).Append(mye).Append(dme).ToFormatter();

        // weekyear week
        private static readonly DateTimeFormatter ww = new DateTimeFormatterBuilder().Append(we).Append(wwe).ToFormatter();

        // weekyear week day
        private static readonly DateTimeFormatter wwd = new DateTimeFormatterBuilder().Append(we).Append(wwe).Append(dwe).ToFormatter();

        // hour minute
        private static readonly DateTimeFormatter hm = new DateTimeFormatterBuilder().Append(hde).Append(mhe).ToFormatter();

        // hour minute second
        private static readonly DateTimeFormatter hms = new DateTimeFormatterBuilder().Append(hde).Append(mhe).Append(sme).ToFormatter();

        // hour minute second milliseconds
        private static readonly DateTimeFormatter hmsl =
            new DateTimeFormatterBuilder().Append(hde).Append(mhe).Append(sme).AppendLiteral('.').AppendFractionOfSecond(3, 3).ToFormatter();

        // hour minute second fraction
        private static readonly DateTimeFormatter hmsf = new DateTimeFormatterBuilder().Append(hde).Append(mhe).Append(sme).Append(fse).ToFormatter();

        // date hour
        private static readonly DateTimeFormatter dh = new DateTimeFormatterBuilder().Append(ymd).Append(lte).Append(hde).ToFormatter();

        // date hour minute
        private static readonly DateTimeFormatter dhm = new DateTimeFormatterBuilder().Append(ymd).Append(lte).Append(hm).ToFormatter();

        // date hour minute second
        private static readonly DateTimeFormatter dhms = new DateTimeFormatterBuilder().Append(ymd).Append(lte).Append(hms).ToFormatter();

        // date hour minute second milliseconds
        private static readonly DateTimeFormatter dhmsl = new DateTimeFormatterBuilder().Append(ymd).Append(lte).Append(hmsl).ToFormatter();

        // date hour minute second fraction
        private static readonly DateTimeFormatter dhmsf = new DateTimeFormatterBuilder().Append(ymd).Append(lte).Append(hmsf).ToFormatter();

        // time
        private static readonly DateTimeFormatter t = new DateTimeFormatterBuilder().Append(hmsf).Append(ze).ToFormatter();

        // time no milliseconds
        private static readonly DateTimeFormatter tx = new DateTimeFormatterBuilder().Append(hms).Append(ze).ToFormatter();

        // time no milliseconds
        private static readonly DateTimeFormatter tnz = new DateTimeFormatterBuilder().Append(hmsf).ToFormatter();

        // Ttime
        private static readonly DateTimeFormatter tt = new DateTimeFormatterBuilder().Append(lte).Append(t).ToFormatter();

        // Ttime no milliseconds
        private static readonly DateTimeFormatter ttx = new DateTimeFormatterBuilder().Append(lte).Append(tx).ToFormatter();

        // date time
        private static readonly DateTimeFormatter dt = new DateTimeFormatterBuilder().Append(ymd).Append(tt).ToFormatter();

        // date time no milliseconds
        private static readonly DateTimeFormatter dtx = new DateTimeFormatterBuilder().Append(ymd).Append(ttx).ToFormatter();

        // week date time
        private static readonly DateTimeFormatter wdt = new DateTimeFormatterBuilder().Append(wwd).Append(tt).ToFormatter();

        // week date time no milliseconds
        private static readonly DateTimeFormatter wdtx = new DateTimeFormatterBuilder().Append(wwd).Append(ttx).ToFormatter();

        // ordinal date(same as yd)
        private static readonly DateTimeFormatter od = new DateTimeFormatterBuilder().Append(ye).Append(dye).ToFormatter();

        // ordinal date time
        private static readonly DateTimeFormatter odt = new DateTimeFormatterBuilder().Append(od).Append(tt).ToFormatter();

        // ordinal date time no milliseconds
        private static readonly DateTimeFormatter odtx = new DateTimeFormatterBuilder().Append(od).Append(ttx).ToFormatter();

        // basic date
        private static readonly DateTimeFormatter bd =
            new DateTimeFormatterBuilder().AppendYear(4, 4).AppendFixedDecimal(DateTimeFieldType.MonthOfYear, 2).AppendFixedDecimal(
                DateTimeFieldType.DayOfMonth, 2).ToFormatter();

        // basic time
        private static readonly DateTimeFormatter bt =
            new DateTimeFormatterBuilder().AppendFixedDecimal(DateTimeFieldType.HourOfDay, 2).AppendFixedDecimal(DateTimeFieldType.MinuteOfHour, 2).
                AppendFixedDecimal(DateTimeFieldType.SecondOfMinute, 2).AppendLiteral('.').AppendFractionOfSecond(3, 9).AppendTimeZoneOffset("Z", false, 2, 2).
                ToFormatter();

        // basic time no milliseconds
        private static readonly DateTimeFormatter btx =
            new DateTimeFormatterBuilder().AppendFixedDecimal(DateTimeFieldType.HourOfDay, 2).AppendFixedDecimal(DateTimeFieldType.MinuteOfHour, 2).
                AppendFixedDecimal(DateTimeFieldType.SecondOfMinute, 2).AppendTimeZoneOffset("Z", false, 2, 2).ToFormatter();

        // basic time no zone
        private static readonly DateTimeFormatter btnz =
            new DateTimeFormatterBuilder().AppendFixedDecimal(DateTimeFieldType.HourOfDay, 2).AppendFixedDecimal(DateTimeFieldType.MinuteOfHour, 2).
                AppendFixedDecimal(DateTimeFieldType.SecondOfMinute, 2).AppendLiteral('.').AppendFractionOfSecond(3, 9).ToFormatter();

        // basic Ttime
        private static readonly DateTimeFormatter btt = new DateTimeFormatterBuilder().Append(lte).Append(bt).ToFormatter();

        // basic Ttime no milliseconds
        private static readonly DateTimeFormatter bttx = new DateTimeFormatterBuilder().Append(lte).Append(btx).ToFormatter();

        // basic date time
        private static readonly DateTimeFormatter bdt = new DateTimeFormatterBuilder().Append(bd).Append(btt).ToFormatter();

        // basic date time no milliseconds
        private static readonly DateTimeFormatter bdtx = new DateTimeFormatterBuilder().Append(bd).Append(bttx).ToFormatter();

        // basic ordinal date
        private static readonly DateTimeFormatter bod =
            new DateTimeFormatterBuilder().AppendYear(4, 4).AppendFixedDecimal(DateTimeFieldType.DayOfYear, 3).ToFormatter();

        // basic ordinal date time
        private static readonly DateTimeFormatter bodt = new DateTimeFormatterBuilder().Append(bod).Append(btt).ToFormatter();

        // basic ordinal date time
        private static readonly DateTimeFormatter bodtx = new DateTimeFormatterBuilder().Append(bod).Append(bttx).ToFormatter();

        // basic week date
        private static readonly DateTimeFormatter bwd =
            new DateTimeFormatterBuilder().AppendWeekYear(4, 4).AppendLiteral('W').AppendFixedDecimal(DateTimeFieldType.WeekOfWeekYear, 2).AppendFixedDecimal(
                DateTimeFieldType.DayOfWeek, 1).ToFormatter();

        // basic week date time
        private static readonly DateTimeFormatter bwdt = new DateTimeFormatterBuilder().Append(bwd).Append(btt).ToFormatter();

        // basic week date time no milliseconds
        private static readonly DateTimeFormatter bwdtx = new DateTimeFormatterBuilder().Append(bwd).Append(bttx).ToFormatter();

        // date parser element
        private static readonly DateTimeFormatter dpe =
            new DateTimeFormatterBuilder().Append(null,
                                                  new[]
                                                  {
                                                      new DateTimeFormatterBuilder().Append(ye).AppendOptional(
                                                          new DateTimeFormatterBuilder().Append(mye).AppendOptional(dme.Parser).ToParser()).ToParser(),
                                                      new DateTimeFormatterBuilder().Append(we).Append(wwe).AppendOptional(dwe.Parser).ToParser(),
                                                      new DateTimeFormatterBuilder().Append(ye).Append(dye).ToParser()
                                                  }).ToFormatter();

        // decimal point parser
        private static readonly IDateTimeParser dec =
            new DateTimeFormatterBuilder().Append(null,
                                                  new[]
                                                  {
                                                      new DateTimeFormatterBuilder().AppendLiteral('.').ToParser(),
                                                      new DateTimeFormatterBuilder().AppendLiteral(',').ToParser()
                                                  }).ToParser();

        // time parser element
        private static readonly DateTimeFormatter tpe = new DateTimeFormatterBuilder()
            // time-element
            .Append(hde).Append(null, new[]
                                      {
                                          new DateTimeFormatterBuilder()
                                          // minute-element
                                          .Append(mhe).Append(null, new[]
                                                                    {
                                                                        new DateTimeFormatterBuilder()
                                                                        // second-element
                                                                        .Append(sme)
                                                                        // second fraction
                                                                        .AppendOptional(
                                                                            new DateTimeFormatterBuilder().Append(dec).AppendFractionOfSecond(1, 9).ToParser()).
                                                                        ToParser(), // minute fraction
                                                                        new DateTimeFormatterBuilder().Append(dec).AppendFractionOfMinute(1, 9).ToParser(), null
                                                                    }).ToParser(),
                                          // hour fraction
                                          new DateTimeFormatterBuilder().Append(dec).AppendFractionOfHour(1, 9).ToParser(), null
                                      }).ToFormatter();

        // offset parser
        private static readonly IDateTimeParser offset = new DateTimeFormatterBuilder().AppendLiteral('T').Append(ze).ToParser();

        // date parser
        private static readonly DateTimeFormatter dp = new DateTimeFormatterBuilder().Append(dpe).AppendOptional(offset).ToFormatter();

        // local date parser
        private static readonly DateTimeFormatter ldp = dpe.WithZone(DateTimeZone.Utc);

        // time parser
        private static readonly DateTimeFormatter tp =
            new DateTimeFormatterBuilder().AppendOptional(lte.Parser).Append(tpe).AppendOptional(ze.Parser).ToFormatter();

        // local time parser
        private static readonly DateTimeFormatter ltp =
            new DateTimeFormatterBuilder().AppendOptional(lte.Parser).Append(tpe).ToFormatter().WithZone(DateTimeZone.Utc);

        /// <summary>
        /// Gets a formatter for a four digit year. (yyyy)
        /// </summary>
        public static DateTimeFormatter Year { get { return ye; } }

        /// <summary>
        /// Gets a formatter for a four digit year and two digit month of year. 
        /// (yyyy-MM)
        /// </summary>
        public static DateTimeFormatter YearMonth { get { return ym; } }

        /// <summary>
        /// Gets a formatter for a four digit year, two digit month of year, and
        /// two digit day of month. 
        /// (yyyy-MM-dd)
        /// </summary>
        public static DateTimeFormatter YearMonthDay { get { return ymd; } }

        /// <summary>
        /// Gets a formatter for a four digit weekyear. 
        /// (xxxx)
        /// </summary>
        public static DateTimeFormatter WeekYear { get { return we; } }

        /// <summary>
        /// Gets a formatter for a for a four digit weekyear and two digit week of weekyear. 
        /// (xxxx-'W'ww)
        /// </summary>
        public static DateTimeFormatter WeekYearWeek { get { return ww; } }

        /// <summary>
        /// Gets a formatter for a for a four digit weekyear, two digit week of
        /// weekyear, and one digit day of week.
        /// (xxxx-'W'ww-e)
        /// </summary>
        public static DateTimeFormatter WeekYearWeekDay { get { return wwd; } }

        /// <summary>
        /// Gets a formatter for a two digit hour of day.
        /// (HH)
        /// </summary>
        public static DateTimeFormatter Hour { get { return hde; } }

        /// <summary>
        /// Gets a formatter for a two digit hour of day and two digit minute of hour.
        /// (HH:mm)
        /// </summary>
        public static DateTimeFormatter HourMinute { get { return hm; } }

        /// <summary>
        /// Gets a formatter for a two digit hour of day, two digit minute of
        /// hour, and two digit second of minute.
        /// (HH:mm:ss)
        /// </summary>
        public static DateTimeFormatter HourMinuteSecond { get { return hms; } }

        /// <summary>
        /// Gets a formatter for a two digit hour of day, two digit minute of
        /// hour, two digit second of minute, and three digit fraction of
        /// second (HH:mm:ss.SSS). Parsing will parse up to 3 fractional second digits.
        /// (HH:mm:ss.SSS)
        /// </summary>
        public static DateTimeFormatter HourMinuteSecondMilliseconds { get { return hmsl; } }

        /// <summary>
        /// Gets a formatter for a two digit hour of day, two digit minute of
        /// hour, two digit second of minute, and three digit fraction of
        /// second (HH:mm:ss.SSS). Parsing will parse up to 9 fractional second
        /// digits, throwing away all except the first three.
        /// (HH:mm:ss.SSS)
        /// </summary>
        public static DateTimeFormatter HourMinuteSecondFraction { get { return hmsf; } }

        /// <summary>
        /// Gets a formatter for a full date as four digit year, two digit month
        /// of year, and two digit day of month.
        /// (yyyy-MM-dd)
        /// </summary>
        public static DateTimeFormatter Date { get { return ymd; } }

        /// <summary>
        /// Gets a formatter that combines a full date and two digit hour of day.
        /// (yyyy-MM-dd'T'HH)
        /// </summary>
        public static DateTimeFormatter DateHour { get { return dh; } }

        /// <summary>
        /// Gets a formatter that combines a full date, two digit hour of day,
        /// and two digit minute of hour.
        /// (yyyy-MM-dd'T'HH:mm)
        /// </summary>
        public static DateTimeFormatter DateHourMinute { get { return dhm; } }

        /// <summary>
        /// Gets a formatter that combines a full date, two digit hour of day,
        /// two digit minute of hour, and two digit second of minute.
        /// (yyyy-MM-dd'T'HH:mm:ss)
        /// </summary>
        public static DateTimeFormatter DateHourMinuteSecond { get { return dhms; } }

        /// <summary>
        /// Gets a formatter that combines a full date, two digit hour of day,
        /// two digit minute of hour, two digit second of minute, and three digit
        /// fraction of second (yyyy-MM-dd'T'HH:mm:ss.SSS). Parsing will parse up
        /// to 3 fractional second digits.
        /// (yyyy-MM-dd'T'HH:mm:ss.SSS)
        /// </summary>
        public static DateTimeFormatter DateHourMinuteSecondMilliseconds { get { return dhmsl; } }

        /// <summary>
        /// Gets a formatter that combines a full date, two digit hour of day,
        /// two digit minute of hour, two digit second of minute, and three digit
        /// fraction of second (yyyy-MM-dd'T'HH:mm:ss.SSS). Parsing will parse up
        /// to 9 fractional second digits, throwing away all except the first three.
        /// (yyyy-MM-dd'T'HH:mm:ss.SSS)
        /// </summary>
        public static DateTimeFormatter DateHourMinuteSecondFraction { get { return dhmsl; } }

        /// <summary>
        /// Gets a formatter for a two digit hour of day, two digit minute of
        /// hour, two digit second of minute, three digit fraction of second, and
        /// time zone offset.
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HH:mm' for non-zero.
        /// (HH:mm:ss.SSSZZ)
        /// </summary>
        public static DateTimeFormatter Time { get { return t; } }

        /// <summary>
        /// Gets a formatter for a two digit hour of day, two digit minute of
        /// hour, two digit second of minute, and time zone offset (HH:mm:ssZZ).
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HH:mm' for non-zero.
        /// (HH:mm:ssZZ)
        /// </summary>
        public static DateTimeFormatter TimeNoMilliseconds { get { return tx; } }

        /// <summary>
        /// Gets a formatter for a two digit hour of day, two digit minute of
        /// hour, two digit second of minute, and three digit fraction of second.
        /// (HH:mm:ss.SSS)
        /// </summary>
        public static DateTimeFormatter TimeNoZone { get { return tnz; } }

        /// <summary>
        /// Gets a formatter for a two digit hour of day, two digit minute of
        /// hour, two digit second of minute, three digit fraction of second, and
        /// time zone offset prefixed by 'T'.
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HH:mm' for non-zero.
        /// ('T'HH:mm:ss.SSSZZ)
        /// </summary>
        public static DateTimeFormatter TTime { get { return tt; } }

        /// <summary>
        /// Gets a formatter for a two digit hour of day, two digit minute of
        /// hour, two digit second of minute, and time zone offset prefixed by 'T'.
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HH:mm' for non-zero.
        /// ('T'HH:mm:ssZZ)
        /// </summary>
        public static DateTimeFormatter TTimeNoMilliseconds { get { return ttx; } }

        /// <summary>
        /// Gets a formatter that combines a full date and time, separated by a 'T'
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HH:mm' for non-zero.
        /// (yyyy-MM-dd'T'HH:mm:ss.SSSZZ)
        /// </summary>
        public static DateTimeFormatter DateTime { get { return dt; } }

        /// <summary>
        /// Gets a formatter that combines a full date and time without milliseconds,
        /// separated by a 'T'
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HH:mm' for non-zero.
        /// (yyyy-MM-dd'T'HH:mm:ssZZ)
        /// </summary>
        public static DateTimeFormatter DateTimeNoMilliseconds { get { return dtx; } }

        /// <summary>
        /// Gets a formatter for a full ordinal date, using a four
        /// digit year and three digit dayOfYear.
        /// (yyyy-DDD)
        /// </summary>
        public static DateTimeFormatter OrdinalDate { get { return od; } }

        /// <summary>
        /// Gets a formatter for a full ordinal date and time, using a four
        /// digit year and three digit dayOfYear (yyyy-DDD'T'HH:mm:ss.SSSZZ).
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HH:mm' for non-zero.
        /// (yyyy-DDD'T'HH:mm:ss.SSSZZ)
        /// </summary>
        public static DateTimeFormatter OrdinalDateTime { get { return odt; } }

        /// <summary>
        /// Gets a formatter for a full ordinal date and time without millis,
        /// using a four digit year and three digit dayOfYear (yyyy-DDD'T'HH:mm:ss.SSSZZ).
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HH:mm' for non-zero.
        /// (yyyy-DDD'T'HH:mm:ssZZ)
        /// </summary>
        public static DateTimeFormatter OrdinalDateTimeNoMilliseconds { get { return odtx; } }

        /// <summary>
        /// Gets a formatter for a full date as four digit weekyear, two digit
        /// week of weekyear, and one digit day of week
        /// (xxxx-'W'ww-e)
        /// </summary>
        public static DateTimeFormatter WeekDate { get { return wwd; } }

        /// <summary>
        /// Gets a formatter that combines a full weekyear date and time,
        /// separated by a 'T'
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HH:mm' for non-zero.
        /// (xxxx-'W'ww-e'T'HH:mm:ss.SSSZZ)
        /// </summary>
        public static DateTimeFormatter WeekDateTime { get { return wdt; } }

        /// <summary>
        /// Gets a formatter that combines a full weekyear date and time without millis,
        /// separated by a 'T'
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HH:mm' for non-zero.
        /// (xxxx-'W'ww-e'T'HH:mm:ssZZ)
        /// </summary>
        public static DateTimeFormatter WeekDateTimeNoMilliseconds { get { return wdtx; } }

        /// <summary>
        /// Gets a formatter for a full date as four digit year, two digit
        /// month of year, and two digit day of month.
        /// (yyyyMMdd)
        /// </summary>
        public static DateTimeFormatter BasicDate { get { return bd; } }

        /// <summary>
        /// Gets a formatter for a two digit hour of day, two digit minute
        /// of hour, two digit second of minute, three digit millis, and time zone offset.
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HHmm' for non-zero.
        /// (HHmmss.SSSZ)
        /// </summary>
        public static DateTimeFormatter BasicTime { get { return bt; } }

        /// <summary>
        /// Gets a formatter for a two digit hour of day, two digit minute
        /// of hour, two digit second of minute, and time zone offset.
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HHmm' for non-zero.
        /// (HHmmssZ)
        /// </summary>
        public static DateTimeFormatter BasicTimeNoMilliseconds { get { return btx; } }

        /// <summary>
        /// Gets a formatter for a two digit hour of day, two digit minute
        /// of hour, two digit second of minute, and three digit millis.
        /// (HHmmss.SSS)
        /// </summary>
        public static DateTimeFormatter BasicTimeNoZone { get { return btnz; } }

        /// <summary>
        /// Gets a formatter for a two digit hour of day, two digit minute
        /// of hour, two digit second of minute, three digit millis, and time zone
        /// offset prefixed by 'T'.
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HHmm' for non-zero.
        /// ('T'HHmmss.SSSZ)
        /// </summary>
        public static DateTimeFormatter BasicTTime { get { return btt; } }

        /// <summary>
        /// Gets a formatter for a two digit hour of day, two digit minute
        /// of hour, two digit second of minute, three digit millis, and time zone
        /// offset prefixed by 'T'.
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HHmm' for non-zero.
        /// ('T'HHmmss.SSSZ)
        /// </summary>
        public static DateTimeFormatter BasicTTimeNoMilliseconds { get { return bttx; } }

        /// <summary>
        /// Gets a formatter that combines a basic date and time, separated by a 'T'
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HHmm' for non-zero.
        /// (yyyyMMdd'T'HHmmss.SSSZ)
        /// </summary>
        public static DateTimeFormatter BasicDateTime { get { return bdt; } }

        /// <summary>
        /// Gets a formatter that combines a basic date and time without millis,
        /// separated by a 'T'
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HHmm' for non-zero.
        /// (yyyyMMdd'T'HHmmssZ)
        /// </summary>
        public static DateTimeFormatter BasicDateTimeNoMilliseconds { get { return bdtx; } }

        /// <summary>
        /// Gets a formatter for a full ordinal date, using a four
        /// digit year and three digit dayOfYear.
        /// (yyyyDDD)
        /// </summary>
        public static DateTimeFormatter BasicOrdinalDate { get { return bod; } }

        /// <summary>
        /// Gets a formatter for a full ordinal date and time, using a four
        /// digit year and three digit dayOfYear
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HHmm' for non-zero.
        /// (yyyyDDD'T'HHmmss.SSSZ)
        /// </summary>
        public static DateTimeFormatter BasicOrdinalDateTime { get { return bodt; } }

        /// <summary>
        /// Gets a formatter for a full ordinal date and time without millis,
        /// using a four digit year and three digit dayOfYear.
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HHmm' for non-zero.
        /// (yyyyDDD'T'HHmmssZ)
        /// </summary>
        public static DateTimeFormatter BasicOrdinalDateTimeNoMilliseconds { get { return bodtx; } }

        /// <summary>
        /// Gets a formatter for a full date as four digit weekyear, two
        /// igit week of weekyear, and one digit day of week
        /// (xxxx'W'wwe)
        /// </summary>
        public static DateTimeFormatter BasicWeekDate { get { return bwd; } }

        /// <summary>
        /// Gets a formatter that combines a basic weekyear date and time, separated by a 'T'
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HHmm' for non-zero.
        /// (xxxx'W'wwe'T'HHmmss.SSSZ)
        /// </summary>
        public static DateTimeFormatter BasicWeekDateTime { get { return bwdt; } }

        /// <summary>
        /// Gets a formatter that combines a basic weekyear date and time
        /// without millis, separated by a 'T'.
        /// The time zone offset is 'Z' for zero, and of the form '\u00b1HHmm' for non-zero.
        /// (xxxx'W'wwe'T'HHmmssZ)
        /// </summary>
        public static DateTimeFormatter BasicWeekDateTimeNoMilliseconds { get { return bwdtx; } }

        /// <summary>
        /// Gets a generic ISO date parser for parsing dates.
        /// It accepts formats described by the following syntax:
        /// date-element      = std-date-element | ord-date-element | week-date-element
        /// std-date-element  = yyyy ['-' MM ['-' dd]]
        /// ord-date-element  = yyyy ['-' DDD]
        /// week-date-element = xxxx '-W' ww ['-' e]
        /// </summary>
        public static DateTimeFormatter DateElementParser { get { return dpe; } }

        /// <summary>
        /// Gets a generic ISO time parser. It accepts formats described by
        /// the following syntax:
        /// time-element   = HH [minute-element] | [fraction]
        /// minute-element = ':' mm [second-element] | [fraction]
        /// second-element = ':' ss [fraction]
        /// fraction       = ('.' | ',') digit+
        /// </summary>
        public static DateTimeFormatter TimeElementParser { get { return tpe; } }

        /// <summary>
        /// Gets a generic ISO date parser for parsing dates with a possible zone.
        /// It accepts formats described by the following syntax:
        /// date              = date-element ['T' offset]
        /// date-element      = std-date-element | ord-date-element | week-date-element
        /// std-date-element  = yyyy ['-' MM ['-' dd]]
        /// ord-date-element  = yyyy ['-' DDD]
        /// week-date-element = xxxx '-W' ww ['-' e]
        /// offset            = 'Z' | (('+' | '-') HH [':' mm [':' ss [('.' | ',') SSS]]])
        /// </summary>
        public static DateTimeFormatter DateParser { get { return dp; } }

        /// <summary>
        /// Gets a generic ISO date parser for parsing local dates.
        /// This parser is initialised with the local (UTC) time zone.
        /// It accepts formats described by the following syntax:
        /// date-element      = std-date-element | ord-date-element | week-date-element
        /// std-date-element  = yyyy ['-' MM ['-' dd]]
        /// ord-date-element  = yyyy ['-' DDD]
        /// week-date-element = xxxx '-W' ww ['-' e]
        /// </summary>
        public static DateTimeFormatter LocalDateParser { get { return ldp; } }

        /// <summary>
        /// Gets a generic ISO time parser for parsing times with a possible zone.
        /// It accepts formats described by the following syntax:
        /// time           = ['T'] time-element [offset]
        /// time-element   = HH [minute-element] | [fraction]
        /// minute-element = ':' mm [second-element] | [fraction]
        /// second-element = ':' ss [fraction]
        /// fraction       = ('.' | ',') digit+
        /// offset         = 'Z' | (('+' | '-') HH [':' mm [':' ss [('.' | ',') SSS]]])
        /// </summary>
        public static DateTimeFormatter TimeParser { get { return tp; } }

        /// <summary>
        /// Gets a generic ISO time parser for parsing local times.
        /// This parser is initialised with the local (UTC) time zone.
        /// It accepts formats described by the following syntax:
        /// time           = ['T'] time-element
        /// time-element   = HH [minute-element] | [fraction]
        /// minute-element = ':' mm [second-element] | [fraction]
        /// second-element = ':' ss [fraction]
        /// fraction       = ('.' | ',') digit+
        /// </summary>
        public static DateTimeFormatter LocalTimeParser { get { return ltp; } }
    }
}