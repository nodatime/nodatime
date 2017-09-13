// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Text;
using NodaTime.Calendars;
using NodaTime.Globalization;
#if NET45
using System.Diagnostics.CodeAnalysis;
#endif

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Common methods used when parsing dates - these are used from both LocalDateTimePatternParser
    /// and LocalDatePatternParser.
    /// </summary>
    internal static class DatePatternHelper
    {
        /// <summary>
        /// Creates a character handler for the year-of-era specifier (y).
        /// </summary>
        internal static CharacterHandler<TResult, TBucket> CreateYearOfEraHandler<TResult, TBucket>
            (Func<TResult, int> yearGetter, Action<TBucket, int> setter)
            where TBucket : ParseBucket<TResult>
        {
            return (pattern, builder) =>
            {
                int count = pattern.GetRepeatCount(4);
                builder.AddField(PatternFields.YearOfEra, pattern.Current);
                switch (count)
                {
                    case 2:
                        builder.AddParseValueAction(2, 2, 'y', 0, 99, setter);
                        // Force the year into the range 0-99.
                        builder.AddFormatLeftPad(2, value => ((yearGetter(value) % 100) + 100) % 100,
                            assumeNonNegative: true,
                            assumeFitsInCount: true);
                        // Just remember that we've set this particular field. We can't set it twice as we've already got the YearOfEra flag set.
                        builder.AddField(PatternFields.YearTwoDigits, pattern.Current);
                        break;
                    case 4:
                        // Left-pad to 4 digits when formatting; parse exactly 4 digits.
                        builder.AddParseValueAction(4, 4, 'y', 1, 9999, setter);
                        builder.AddFormatLeftPad(4, yearGetter,
                            assumeNonNegative: false,
                            assumeFitsInCount: true);
                        break;
                    default:
                        throw new InvalidPatternException(TextErrorMessages.InvalidRepeatCount, pattern.Current, count);
                }
            };
        }

        /// <summary>
        /// Creates a character handler for the month-of-year specifier (M).
        /// </summary>
        internal static CharacterHandler<TResult, TBucket> CreateMonthOfYearHandler<TResult, TBucket>
            (Func<TResult, int> numberGetter, Action<TBucket, int> textSetter, Action<TBucket, int> numberSetter)
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
                        builder.AddParseValueAction(count, 2, pattern.Current, 1, 99, numberSetter);
                        builder.AddFormatLeftPad(count, numberGetter, assumeNonNegative: true, assumeFitsInCount: count == 2);
                        break;
                    case 3:
                    case 4:
                        field = PatternFields.MonthOfYearText;
                        var format = builder.FormatInfo;
                        IList<string> nonGenitiveTextValues = count == 3 ? format.ShortMonthNames : format.LongMonthNames;
                        IList<string> genitiveTextValues = count == 3 ? format.ShortMonthGenitiveNames : format.LongMonthGenitiveNames;
                        if (nonGenitiveTextValues == genitiveTextValues)
                        {
                            builder.AddParseLongestTextAction(pattern.Current, textSetter, format.CompareInfo, nonGenitiveTextValues);
                        }
                        else
                        {
                            builder.AddParseLongestTextAction(pattern.Current, textSetter, format.CompareInfo,
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
        private sealed class MonthFormatActionHolder<TResult, TBucket> : SteppedPatternBuilder<TResult, TBucket>.IPostPatternParseFormatAction
            where TBucket : ParseBucket<TResult>
        {
            private readonly int count;
            private readonly NodaFormatInfo formatInfo;
            private readonly Func<TResult, int> getter;

            internal MonthFormatActionHolder(NodaFormatInfo formatInfo, int count, Func<TResult, int> getter)
            {
                this.count = count;
                this.formatInfo = formatInfo;
                this.getter = getter;
            }

#if NET45
            [ExcludeFromCodeCoverage]
#endif
            internal void DummyMethod(TResult value, StringBuilder builder)
            {
                // This method is never called. We use it to create a delegate with a target that implements
                // IPostPatternParseFormatAction. There's no test for this throwing.
                throw new InvalidOperationException("This method should never be called");
            }

            public Action<TResult, StringBuilder> BuildFormatAction(PatternFields finalFields)
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
            (Func<TResult, int> dayOfMonthGetter, Func<TResult, int> dayOfWeekGetter,
             Action<TBucket, int> dayOfMonthSetter, Action<TBucket, int> dayOfWeekSetter)
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
                        builder.AddFormatLeftPad(count, dayOfMonthGetter, assumeNonNegative: true, assumeFitsInCount: count == 2);
                        break;
                    case 3:
                    case 4:
                        field = PatternFields.DayOfWeek;
                        var format = builder.FormatInfo;
                        IList<string> textValues = count == 3 ? format.ShortDayNames : format.LongDayNames;
                        builder.AddParseLongestTextAction(pattern.Current, dayOfWeekSetter, format.CompareInfo, textValues);
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
            (Func<TResult, Era> eraFromValue, Func<TBucket, LocalDatePatternParser.LocalDateParseBucket> dateBucketFromBucket)
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
            (Func<TResult, CalendarSystem> getter, Action<TBucket, CalendarSystem> setter)
            where TBucket : ParseBucket<TResult>
        {
            return (pattern, builder) =>
            {
                builder.AddField(PatternFields.Calendar, pattern.Current);

                builder.AddParseAction((cursor, bucket) =>
                {
                    foreach (var id in CalendarSystem.Ids)
                    {
                        if (cursor.Match(id))
                        {
                            setter(bucket, CalendarSystem.ForId(id));
                            return null;
                        }
                    }
                    return ParseResult<TResult>.NoMatchingCalendarSystem(cursor);
                });
                builder.AddFormatAction((value, sb) => sb.Append(getter(value).Id));
            };
        }    
    }
}
