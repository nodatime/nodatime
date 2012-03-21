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

using System.Text;
using NodaTime.Globalization;
using NodaTime.Properties;
using NodaTime.Text.Patterns;
using NodaTime.Utility;

namespace NodaTime.Text
{
    /// <summary>
    /// Represents a pattern for parsing and formatting <see cref="Period"/> values.
    /// </summary>
    public class PeriodPattern : IPattern<Period>
    {
        /// <summary>
        /// Pattern which uses the normal ISO format for all the supported ISO
        /// fields, but extends the time part with "s" for milliseconds and "t" for ticks.
        /// No normalization is carried out, and a period may contain weeks as well as years, months and days.
        /// Each element may also be negative, independently of other elements. This pattern round-trips its
        /// values: a parse/format cycle will produce an identical period, including units.
        /// </summary>
        public static readonly PeriodPattern RoundtripPattern = new PeriodPattern(new RoundtripPatternImpl());
        /// <summary>
        /// A "normalizing" pattern which abides by the ISO-8601 duration format as far as possible.
        /// Weeks are added to the number of days (after multiplying by 7). Time units are normalized
        /// (extending into days where necessary), and fractions of seconds are represented within the
        /// seconds part. Unlike ISO-8601, which pattern allows for negative values within a period.
        /// </summary>
        public static readonly PeriodPattern NormalizingIsoPattern = new PeriodPattern(new NormalizingIsoPatternImpl());

        private readonly IPattern<Period> pattern;

        private PeriodPattern(IPattern<Period> pattern)
        {
            this.pattern = Preconditions.CheckNotNull(pattern, "pattern");
        }

        /// <summary>
        /// Parses the given text value according to the rules of this pattern.
        /// </summary>
        /// <remarks>
        /// This method never throws an exception (barring a bug in Noda Time itself). Even errors such as
        /// the argument being null are wrapped in a parse result.
        /// </remarks>
        /// <param name="text">The text value to parse.</param>
        /// <returns>The result of parsing, which may be successful or unsuccessful.</returns>
        public ParseResult<Period> Parse(string text)
        {
            return pattern.Parse(text);
        }

        /// <summary>
        /// Formats the given period as text according to the rules of this pattern.
        /// </summary>
        /// <param name="value">The period to format.</param>
        /// <returns>The period formatted according to this pattern.</returns>
        public string Format(Period value)
        {
            return pattern.Format(value);
        }

        private static void AppendValue(StringBuilder builder, PeriodUnits unitToCheck, long value, string suffix)
        {
            // Avoid having a load of conditions in the calling code by checking here
            if (unitToCheck == 0)
            {
                return;
            }
            FormatHelper.FormatInvariant(value, builder);
            builder.Append(suffix);
        }

        private static ParseResult<Period> InvalidUnit(char unitCharacter)
        {
            return ParseResult<Period>.ForInvalidValue(Messages.Parse_InvalidUnitSpecifier, unitCharacter);
        }

        private static ParseResult<Period> RepeatedUnit(char unitCharacter)
        {
            return ParseResult<Period>.ForInvalidValue(Messages.Parse_RepeatedUnitSpecifier, unitCharacter);
        }

        private static ParseResult<Period> MisplacedUnit(char unitCharacter)
        {
            return ParseResult<Period>.ForInvalidValue(Messages.Parse_MisplacedUnitSpecifier, unitCharacter);
        }

        private static readonly ParseResult<Period> EmptyPeriod = ParseResult<Period>.ForInvalidValue(Messages.Parse_EmptyPeriod);

        private class RoundtripPatternImpl : IPattern<Period>
        {            
            public ParseResult<Period> Parse(string value)
            {
                if (value == null)
                {
                    return ParseResult<Period>.ArgumentNull("value");
                }
                if (value.Length == 0)
                {
                    return ParseResult<Period>.ValueStringEmpty;
                }

                ValueCursor valueCursor = new ValueCursor(value);
                
                valueCursor.MoveNext();
                if (valueCursor.Current != 'P')
                {
                    return ParseResult<Period>.MismatchedCharacter('P');
                }
                bool inDate = true;
                PeriodBuilder builder = new PeriodBuilder();
                PeriodUnits unitsSoFar = 0;
                while (valueCursor.MoveNext())
                {
                    long unitValue;
                    if (inDate && valueCursor.Current == 'T')
                    {
                        inDate = false;
                        continue;
                    }
                    var failure = valueCursor.ParseInt64<Period>(out unitValue);
                    if (failure != null)
                    {
                        return failure;
                    }
                    if (valueCursor.Length == valueCursor.Index)
                    {
                        return ParseResult<Period>.EndOfString;
                    }
                    // Various failure cases:
                    // - Repeated unit (e.g. P1M2M)
                    // - Time unit is in date part (e.g. P5M)
                    // - Date unit is in time part (e.g. PT1D)
                    // - Unit is in incorrect order (e.g. P5D1Y)
                    // - Unit is invalid (e.g. P5J)
                    // - Unit is missing (e.g. P5)
                    PeriodUnits unit;
                    switch (valueCursor.Current)
                    {
                        case 'Y':
                            unit = PeriodUnits.Years;
                            builder.Years = unitValue;
                            break;
                        case 'M':
                            if (inDate)
                            {
                                unit = PeriodUnits.Months;
                                builder.Months = unitValue;
                            }
                            else
                            {
                                unit = PeriodUnits.Minutes;
                                builder.Minutes = unitValue;                                
                            }
                            break;
                        case 'W':
                            unit = PeriodUnits.Weeks;
                            builder.Weeks = unitValue;
                            break;
                        case 'D':
                            unit = PeriodUnits.Days;
                            builder.Days = unitValue;
                            break;
                        case 'H':
                            unit = PeriodUnits.Hours;
                            builder.Hours = unitValue;
                            break;
                        case 'S':
                            unit = PeriodUnits.Seconds;
                            builder.Seconds = unitValue;
                            break;
                        case 's':
                            unit = PeriodUnits.Milliseconds;
                            builder.Milliseconds = unitValue;
                            break;
                        case 't':
                            unit = PeriodUnits.Ticks;
                            builder.Ticks = unitValue;
                            break;
                        default:
                            return InvalidUnit(valueCursor.Current);
                    }
                    if ((unit & unitsSoFar) != 0)
                    {
                        return RepeatedUnit(valueCursor.Current);
                    }

                    // This handles putting months before years, for example. Less significant units
                    // have higher integer representations.
                    if (unit < unitsSoFar)
                    {
                        return MisplacedUnit(valueCursor.Current);
                    }
                    // The result of checking "there aren't any time units in this unit" should be
                    // equal to "we're still in the date part".
                    if ((unit & PeriodUnits.AllTimeUnits) == 0 != inDate)
                    {
                        return MisplacedUnit(valueCursor.Current);
                    }
                    unitsSoFar |= unit;
                }
                if (unitsSoFar == 0)
                {
                    return EmptyPeriod;
                }
                return ParseResult<Period>.ForValue(builder.Build());
            }

            public string Format(Period value)
            {
                Preconditions.CheckNotNull(value, "value");
                StringBuilder builder = new StringBuilder("P");
                AppendValue(builder, value.Units & PeriodUnits.Years, value.Years, "Y");
                AppendValue(builder, value.Units & PeriodUnits.Months, value.Months, "M");
                AppendValue(builder, value.Units & PeriodUnits.Weeks, value.Weeks, "W");
                AppendValue(builder, value.Units & PeriodUnits.Days, value.Days, "D");
                if ((value.Units & PeriodUnits.AllTimeUnits) != 0)
                {
                    builder.Append("T");
                    AppendValue(builder, value.Units & PeriodUnits.Hours, value.Hours, "H");
                    AppendValue(builder, value.Units & PeriodUnits.Minutes, value.Minutes, "M");
                    AppendValue(builder, value.Units & PeriodUnits.Seconds, value.Seconds, "S");
                    AppendValue(builder, value.Units & PeriodUnits.Milliseconds, value.Milliseconds, "s");
                    AppendValue(builder, value.Units & PeriodUnits.Ticks, value.Ticks, "t");
                }
                return builder.ToString();
            }
        }

        private class NormalizingIsoPatternImpl : IPattern<Period>
        {

            public ParseResult<Period> Parse(string text)
            {
                throw new System.NotImplementedException();
            }

            public string Format(Period value)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
