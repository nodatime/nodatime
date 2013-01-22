// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Text;
using NodaTime.Calendars;
using NodaTime.Globalization;

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Common methods used when parsing dates - these are used from both LocalDateTimePatternParser
    /// and LocalDatePatternParser.
    /// </summary>
    internal static class DatePatternHelper
    {
        /// <summary>
        /// Creates a character handler for the year specifier (y).
        /// </summary>
        internal static CharacterHandler<TResult, TBucket> CreateYearHandler<TResult, TBucket>
            (NodaFunc<TResult, int> centuryGetter, NodaFunc<TResult, int> yearGetter, NodaAction<TBucket, int> setter)
            where TBucket : ParseBucket<TResult>
        {
            return (pattern, builder) =>
            {
                int count = pattern.GetRepeatCount(5);
                builder.AddField(PatternFields.Year, pattern.Current);
                switch (count)
                {
                    case 1:
                    case 2:
                        builder.AddParseValueAction(count, 2, 'y', -99, 99, setter);
                        builder.AddFormatAction((value, sb) => FormatHelper.LeftPad(centuryGetter(value), count, sb));
                        // Just remember that we've set this particular field. We can't set it twice as we've already got the Year flag set.
                        builder.AddField(PatternFields.YearTwoDigits, pattern.Current);
                        break;
                    case 3:
                        // Maximum value will be determined later.
                        // Three or more digits (ick).
                        builder.AddParseValueAction(count, 5, 'y', -99999, 99999, setter);
                        builder.AddFormatAction((value, sb) => FormatHelper.LeftPad(yearGetter(value), count, sb));
                        break;
                    default:
                        // Maximum value will be determined later.
                        // Note that the *exact* number of digits are required; not just "at least count".
                        builder.AddParseValueAction(count, count, 'y', -99999, 99999, setter);
                        builder.AddFormatAction((value, sb) => FormatHelper.LeftPad(yearGetter(value), count, sb));
                        break;
                }
            };
        }

        /// <summary>
        /// Creates a character handler for the month-of-year specifier (M).
        /// </summary>
        internal static CharacterHandler<TResult, TBucket> CreateMonthOfYearHandler<TResult, TBucket>
            (NodaFunc<TResult, int> numberGetter, NodaAction<TBucket, int> textSetter, NodaAction<TBucket, int> numberSetter)
            where TBucket : ParseBucket<TResult>
        {
            return (pattern, builder) =>
            {
                int count = pattern.GetRepeatCount(4);
                PatternFields field;
                switch (count)
                {
                    case 1:
                    case 2:
                        field = PatternFields.MonthOfYearNumeric;
                        // Handle real maximum value in the bucket
                        builder.AddParseValueAction(count, 2, pattern.Current, 0, 99, numberSetter);
                        builder.AddFormatAction((value, sb) => FormatHelper.LeftPad(numberGetter(value), count, sb));
                        break;
                    case 3:
                    case 4:
                        field = PatternFields.MonthOfYearText;
                        var format = builder.FormatInfo;
                        IList<string> nonGenitiveTextValues = count == 3 ? format.ShortMonthNames : format.LongMonthNames;
                        IList<string> genitiveTextValues = count == 3 ? format.ShortMonthGenitiveNames : format.LongMonthGenitiveNames;
                        if (nonGenitiveTextValues == genitiveTextValues)
                        {
                            builder.AddParseLongestTextAction(pattern.Current, textSetter, format.CultureInfo.CompareInfo, nonGenitiveTextValues);
                        }
                        else
                        {
                            builder.AddParseLongestTextAction(pattern.Current, textSetter, format.CultureInfo.CompareInfo,
                                                        genitiveTextValues, nonGenitiveTextValues);
                        }

                        // Hack: see below
                        builder.AddFormatAction(new MonthFormatActionHolder<TResult, TBucket>(format, count, numberGetter).DummyMethod);
                        break;
                    default:
                        throw new InvalidOperationException("Invalid count!");
                }
                builder.AddField(field, pattern.Current);
            };
        }

        // Hacky way of building an action which depends on the final set of pattern fields to determine whether to format a month
        // using the genitive form or not.
        internal sealed class MonthFormatActionHolder<TResult, TBucket> : SteppedPatternBuilder<TResult, TBucket>.IPostPatternParseFormatAction
            where TBucket : ParseBucket<TResult>
        {
            private readonly int count;
            private readonly NodaFormatInfo formatInfo;
            private readonly NodaFunc<TResult, int> getter;

            internal MonthFormatActionHolder(NodaFormatInfo formatInfo, int count, NodaFunc<TResult, int> getter)
            {
                this.count = count;
                this.formatInfo = formatInfo;
                this.getter = getter;
            }

            internal void DummyMethod(TResult value, StringBuilder builder)
            {
                throw new InvalidOperationException("This method should never be called");
            }

            public NodaAction<TResult, StringBuilder> BuildFormatAction(PatternFields finalFields)
            {
                bool genitive = (finalFields & PatternFields.DayOfMonth) != 0;
                IList<string> textValues = count == 3
                    ? (genitive ? formatInfo.ShortMonthGenitiveNames : formatInfo.ShortMonthNames)
                    : (genitive ? formatInfo.LongMonthGenitiveNames : formatInfo.LongMonthNames);
                return (value, sb) => sb.Append(textValues[getter(value)]);
            }
        }

        /// <summary>
        /// Creates a character handler for the day specifier (d).
        /// </summary>
        internal static CharacterHandler<TResult, TBucket> CreateDayHandler<TResult, TBucket>
            (NodaFunc<TResult, int> dayOfMonthGetter, NodaFunc<TResult, int> dayOfWeekGetter,
             NodaAction<TBucket, int> dayOfMonthSetter, NodaAction<TBucket, int> dayOfWeekSetter)
            where TBucket : ParseBucket<TResult>
        {
            return (pattern, builder) =>
            {
                int count = pattern.GetRepeatCount(4);
                PatternFields field;
                switch (count)
                {
                    case 1:
                    case 2:
                        field = PatternFields.DayOfMonth;
                        // Handle real maximum value in the bucket
                        builder.AddParseValueAction(count, 2, pattern.Current, 1, 99, dayOfMonthSetter);
                        builder.AddFormatAction((value, sb) => FormatHelper.LeftPad(dayOfMonthGetter(value), count, sb));
                        break;
                    case 3:
                    case 4:
                        field = PatternFields.DayOfWeek;
                        var format = builder.FormatInfo;
                        IList<string> textValues = count == 3 ? format.ShortDayNames : format.LongDayNames;
                        builder.AddParseLongestTextAction(pattern.Current, dayOfWeekSetter, format.CultureInfo.CompareInfo, textValues);
                        builder.AddFormatAction((value, sb) => sb.Append(textValues[dayOfWeekGetter(value)]));
                        break;
                    default:
                        throw new InvalidOperationException("Invalid count!");
                }
                builder.AddField(field, pattern.Current);
            };
        }

        /// <summary>
        /// Creates a character handler for the era specifier (g).
        /// </summary>
        internal static CharacterHandler<TResult, TBucket> CreateEraHandler<TResult, TBucket>
            (NodaFunc<TResult, Era> eraFromValue, NodaFunc<TBucket, LocalDatePatternParser.LocalDateParseBucket> dateBucketFromBucket)
            where TBucket : ParseBucket<TResult>
        {
            return (pattern, builder) =>
            {
                pattern.GetRepeatCount(2);
                builder.AddField(PatternFields.Era, pattern.Current);
                var formatInfo = builder.FormatInfo;
                // Note: currently the count is ignored. More work needed to determine whether abbreviated era names should be used for just "g".
                builder.AddParseAction((cursor, bucket) => 
                {
                    var dateBucket = dateBucketFromBucket(bucket);
                    return dateBucket.ParseEra<TResult>(formatInfo, cursor);
                });
                builder.AddFormatAction((value, sb) => sb.Append(formatInfo.GetEraPrimaryName(eraFromValue(value))));
            };
        }

        /// <summary>
        /// Creates a character handler for the calendar specifier (c).
        /// </summary>
        internal static CharacterHandler<TResult, TBucket> CreateCalendarHandler<TResult, TBucket>
            (NodaFunc<TResult, CalendarSystem> getter, NodaAction<TBucket, CalendarSystem> setter)
            where TBucket : ParseBucket<TResult>
        {
            return (pattern, builder) =>
            {
                builder.AddField(PatternFields.Calendar, pattern.Current);

                builder.AddParseAction((cursor, bucket) =>
                {
                    // TODO: Do we really want this to be case-insensitive? It's really an ID, not a culture-specific value.
                    foreach (var id in CalendarSystem.Ids)
                    {
                        if (cursor.MatchCaseInsensitive(id, NodaFormatInfo.InvariantInfo.CultureInfo.CompareInfo, true))
                        {
                            setter(bucket, CalendarSystem.ForId(id));
                            return null;
                        }
                    }
                    return ParseResult<TResult>.NoMatchingCalendarSystem;
                });
                builder.AddFormatAction((value, sb) => sb.Append(getter(value).Id));
            };
        }    
    }
}
