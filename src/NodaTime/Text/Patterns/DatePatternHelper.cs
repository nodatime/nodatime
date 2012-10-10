#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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
                // TODO(Post-V1): Handle parsing and formatting negative years.
                PatternParseResult<TResult> failure = null;
                int count = pattern.GetRepeatCount(5, ref failure);
                if (failure != null)
                {
                    return failure;
                }
                failure = builder.AddField(PatternFields.Year, pattern.Current);
                if (failure != null)
                {
                    return failure;
                }
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
                return null;

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
                PatternParseResult<TResult> failure = null;
                int count = pattern.GetRepeatCount(4, ref failure);
                if (failure != null)
                {
                    return failure;
                }
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
                            builder.AddParseTextAction(pattern.Current, textSetter, format.CultureInfo.CompareInfo, nonGenitiveTextValues);
                        }
                        else
                        {
                            // TODO(Post-V1): Make this more robust. The genitive text values come first here because in some cultures they
                            // are longer than the non-genitive forms - we want to match the longest substring. (We're not doing any backtracking...)
                            // This will fail to do the right thing if we get into the opposite situation.
                            builder.AddParseTextAction(pattern.Current, textSetter, format.CultureInfo.CompareInfo,
                                                        genitiveTextValues, nonGenitiveTextValues);
                        }

                        // Hack: see below
                        builder.AddFormatAction(new MonthFormatActionHolder<TResult, TBucket>(format, count, numberGetter).DummyMethod);
                        break;
                    default:
                        throw new InvalidOperationException("Invalid count!");
                }
                failure = builder.AddField(field, pattern.Current);
                if (failure != null)
                {
                    return failure;
                }
                return null;
            };
        }

        // Hacky way of building an action which depends on the final set of pattern fields to determine whether to format a month
        // using the genitive form or not.
        internal class MonthFormatActionHolder<TResult, TBucket> : SteppedPatternBuilder<TResult, TBucket>.IPostPatternParseFormatAction
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
                PatternParseResult<TResult> failure = null;
                int count = pattern.GetRepeatCount(4, ref failure);
                if (failure != null)
                {
                    return failure;
                }
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
                        builder.AddParseTextAction(pattern.Current, dayOfWeekSetter, format.CultureInfo.CompareInfo, textValues);
                        builder.AddFormatAction((value, sb) => sb.Append(textValues[dayOfWeekGetter(value)]));
                        break;
                    default:
                        throw new InvalidOperationException("Invalid count!");
                }
                failure = builder.AddField(field, pattern.Current);
                if (failure != null)
                {
                    return failure;
                }
                return null;
            };
        }

        /// <summary>
        /// Creates a character handler for the era specifier (g).
        /// </summary>
        internal static CharacterHandler<TResult, TBucket> CreateEraHandler<TResult, TBucket>
            (CalendarSystem calendar, NodaFunc<TResult, Era> getter, NodaAction<TBucket, int> setter)
            where TBucket : ParseBucket<TResult>
        {
            return (pattern, builder) =>
            {
                PatternParseResult<TResult> failure = null;
                pattern.GetRepeatCount(2, ref failure);
                if (failure != null)
                {
                    return failure;
                }
                failure = builder.AddField(PatternFields.Era, pattern.Current);
                if (failure != null)
                {
                    return failure;
                }
                var formatInfo = builder.FormatInfo;
                // Note: currently the count is ignored. More work needed to determine whether abbreviated era names should be used for just "g".

                builder.AddParseAction((cursor, bucket) =>
                {
                    var compareInfo = formatInfo.CultureInfo.CompareInfo;
                    var eras = calendar.Eras;
                    for (int i = 0; i < eras.Count; i++)
                    {
                        foreach (string eraName in formatInfo.GetEraNames(eras[i]))
                        {
                            if (cursor.MatchCaseInsensitive(eraName, compareInfo))
                            {
                                setter(bucket, i);
                                return null;
                            }
                        }
                    }
                    return ParseResult<TResult>.MismatchedText('g');
                });
                builder.AddFormatAction((value, sb) => sb.Append(formatInfo.GetEraPrimaryName(getter(value))));
                return null;
            };
        }
    }
}
