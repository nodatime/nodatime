// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text;
using NodaTime.Annotations;
using NodaTime.Properties;
using NodaTime.Utility;

namespace NodaTime.Text
{
    /// <summary>
    /// Represents a pattern for parsing and formatting <see cref="Period"/> values.
    /// </summary>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class PeriodPattern : IPattern<Period>
    {
        /// <summary>
        /// Pattern which uses the normal ISO format for all the supported ISO
        /// fields, but extends the time part with "s" for milliseconds and "t" for ticks.
        /// No normalization is carried out, and a period may contain weeks as well as years, months and days.
        /// Each element may also be negative, independently of other elements. This pattern round-trips its
        /// values: a parse/format cycle will produce an identical period, including units.
        /// </summary>
        public static PeriodPattern RoundtripPattern { get; } = new PeriodPattern(new RoundtripPatternImpl());

        /// <summary>
        /// A "normalizing" pattern which abides by the ISO-8601 duration format as far as possible.
        /// Weeks are added to the number of days (after multiplying by 7). Time units are normalized
        /// (extending into days where necessary), and fractions of seconds are represented within the
        /// seconds part. Unlike ISO-8601, which pattern allows for negative values within a period.
        /// </summary>
        /// <remarks>
        /// Note that normalizing the period when formatting will cause an <see cref="System.OverflowException"/>
        /// if the period contains more than <see cref="System.Int64.MaxValue"/> ticks when the
        /// combined weeks/days/time portions are considered. Such a period could never
        /// be useful anyway, however.
        /// </remarks>
        public static PeriodPattern NormalizingIsoPattern { get; } = new PeriodPattern(new NormalizingIsoPatternImpl());

        private readonly IPattern<Period> pattern;

        private PeriodPattern(IPattern<Period> pattern)
        {
            this.pattern = Preconditions.CheckNotNull(pattern, nameof(pattern));
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
        public ParseResult<Period> Parse(string text) => pattern.Parse(text);

        /// <summary>
        /// Formats the given period as text according to the rules of this pattern.
        /// </summary>
        /// <param name="value">The period to format.</param>
        /// <returns>The period formatted according to this pattern.</returns>
        public string Format(Period value) => pattern.Format(value);

        /// <summary>
        /// Formats the given value as text according to the rules of this pattern,
        /// appending to the given <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="builder">The <c>StringBuilder</c> to append to.</param>
        /// <returns>The builder passed in as <paramref name="builder"/>.</returns>
        public StringBuilder AppendFormat(Period value, StringBuilder builder) => pattern.AppendFormat(value, builder);

        private static void AppendValue(StringBuilder builder, long value, string suffix)
        {
            // Avoid having a load of conditions in the calling code by checking here
            if (value == 0)
            {
                return;
            }
            FormatHelper.FormatInvariant(value, builder);
            builder.Append(suffix);
        }

        private static ParseResult<Period> InvalidUnit(ValueCursor cursor, char unitCharacter) => ParseResult<Period>.ForInvalidValue(cursor, Messages.Parse_InvalidUnitSpecifier, unitCharacter);

        private static ParseResult<Period> RepeatedUnit(ValueCursor cursor, char unitCharacter) => ParseResult<Period>.ForInvalidValue(cursor, Messages.Parse_RepeatedUnitSpecifier, unitCharacter);

        private static ParseResult<Period> MisplacedUnit(ValueCursor cursor, char unitCharacter) => ParseResult<Period>.ForInvalidValue(cursor, Messages.Parse_MisplacedUnitSpecifier, unitCharacter);

        private sealed class RoundtripPatternImpl : IPattern<Period>
        {            
            public ParseResult<Period> Parse(string text)
            {
                if (text == null)
                {
                    return ParseResult<Period>.ArgumentNull("text");
                }
                if (text.Length == 0)
                {
                    return ParseResult<Period>.ValueStringEmpty;
                }

                ValueCursor valueCursor = new ValueCursor(text);
                
                valueCursor.MoveNext();
                if (valueCursor.Current != 'P')
                {
                    return ParseResult<Period>.MismatchedCharacter(valueCursor, 'P');
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
                        return ParseResult<Period>.EndOfString(valueCursor);
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
                        case 'Y': unit = PeriodUnits.Years; break;
                        case 'M': unit = inDate ? PeriodUnits.Months : PeriodUnits.Minutes; break;
                        case 'W': unit = PeriodUnits.Weeks; break;
                        case 'D': unit = PeriodUnits.Days; break;
                        case 'H': unit = PeriodUnits.Hours; break;
                        case 'S': unit = PeriodUnits.Seconds; break;
                        case 's': unit = PeriodUnits.Milliseconds; break;
                        case 't': unit = PeriodUnits.Ticks; break;
                        default: return InvalidUnit(valueCursor, valueCursor.Current);
                    }
                    if ((unit & unitsSoFar) != 0)
                    {
                        return RepeatedUnit(valueCursor, valueCursor.Current);
                    }

                    // This handles putting months before years, for example. Less significant units
                    // have higher integer representations.
                    if (unit < unitsSoFar)
                    {
                        return MisplacedUnit(valueCursor, valueCursor.Current);
                    }
                    // The result of checking "there aren't any time units in this unit" should be
                    // equal to "we're still in the date part".
                    if ((unit & PeriodUnits.AllTimeUnits) == 0 != inDate)
                    {
                        return MisplacedUnit(valueCursor, valueCursor.Current);
                    }
                    builder[unit] = unitValue;
                    unitsSoFar |= unit;
                }
                return ParseResult<Period>.ForValue(builder.Build());
            }

            public string Format(Period value) => AppendFormat(value, new StringBuilder()).ToString();

            public StringBuilder AppendFormat(Period value, StringBuilder builder)
            {
                Preconditions.CheckNotNull(value, nameof(value));
                Preconditions.CheckNotNull(builder, nameof(builder));
                builder.Append("P");
                AppendValue(builder, value.Years, "Y");
                AppendValue(builder, value.Months, "M");
                AppendValue(builder, value.Weeks, "W");
                AppendValue(builder, value.Days, "D");
                if (value.HasTimeComponent)
                {
                    builder.Append("T");
                    AppendValue(builder, value.Hours, "H");
                    AppendValue(builder, value.Minutes, "M");
                    AppendValue(builder, value.Seconds, "S");
                    AppendValue(builder, value.Milliseconds, "s");
                    AppendValue(builder, value.Ticks, "t");
                }
                return builder;
            }
        }

        private sealed class NormalizingIsoPatternImpl : IPattern<Period>
        {
            // TODO: Tidy this up a *lot*.
            public ParseResult<Period> Parse(string text)
            {
                if (text == null)
                {
                    return ParseResult<Period>.ArgumentNull("text");
                }
                if (text.Length == 0)
                {
                    return ParseResult<Period>.ValueStringEmpty;
                }

                ValueCursor valueCursor = new ValueCursor(text);

                valueCursor.MoveNext();
                if (valueCursor.Current != 'P')
                {
                    return ParseResult<Period>.MismatchedCharacter(valueCursor, 'P');
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
                    bool negative = valueCursor.Current == '-';
                    var failure = valueCursor.ParseInt64<Period>(out unitValue);
                    if (failure != null)
                    {
                        return failure;
                    }
                    if (valueCursor.Length == valueCursor.Index)
                    {
                        return ParseResult<Period>.EndOfString(valueCursor);
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
                        case 'Y': unit = PeriodUnits.Years; break;
                        case 'M': unit = inDate ? PeriodUnits.Months : PeriodUnits.Minutes; break;
                        case 'W': unit = PeriodUnits.Weeks; break;
                        case 'D': unit = PeriodUnits.Days; break;
                        case 'H': unit = PeriodUnits.Hours; break;
                        case 'S': unit = PeriodUnits.Seconds; break;
                        case ',':
                        case '.': unit = PeriodUnits.Ticks; break; // Special handling below
                        default: return InvalidUnit(valueCursor, valueCursor.Current);
                    }
                    if ((unit & unitsSoFar) != 0)
                    {
                        return RepeatedUnit(valueCursor, valueCursor.Current);
                    }

                    // This handles putting months before years, for example. Less significant units
                    // have higher integer representations.
                    if (unit < unitsSoFar)
                    {
                        return MisplacedUnit(valueCursor, valueCursor.Current);
                    }

                    // The result of checking "there aren't any time units in this unit" should be
                    // equal to "we're still in the date part".
                    if ((unit & PeriodUnits.AllTimeUnits) == 0 != inDate)
                    {
                        return MisplacedUnit(valueCursor, valueCursor.Current);
                    }

                    // Seen a . or , which need special handling.
                    if (unit == PeriodUnits.Ticks)
                    {
                        // Check for already having seen seconds, e.g. PT5S0.5
                        if ((unitsSoFar & PeriodUnits.Seconds) != 0)
                        {
                            return MisplacedUnit(valueCursor, valueCursor.Current);
                        }
                        builder.Seconds = unitValue;

                        if (!valueCursor.MoveNext())
                        {
                            return ParseResult<Period>.MissingNumber(valueCursor);
                        }
                        int totalTicks;
                        // Can cope with at most 9999999 ticks
                        if (!valueCursor.ParseFraction(7, 7, out totalTicks, false))
                        {
                            return ParseResult<Period>.MissingNumber(valueCursor);
                        }
                        // Use whether or not the seconds value was negative (even if 0)
                        // as the indication of whether this value is negative.
                        if (negative)
                        {
                            totalTicks = -totalTicks;
                        }
                        builder.Milliseconds = (totalTicks / NodaConstants.TicksPerMillisecond) % NodaConstants.MillisecondsPerSecond;
                        builder.Ticks = totalTicks % NodaConstants.TicksPerMillisecond;

                        if (valueCursor.Current != 'S')
                        {
                            return ParseResult<Period>.MismatchedCharacter(valueCursor, 'S');
                        }
                        if (valueCursor.MoveNext())
                        {
                            return ParseResult<Period>.ExpectedEndOfString(valueCursor);
                        }
                        return ParseResult<Period>.ForValue(builder.Build());
                    }

                    builder[unit] = unitValue;
                    unitsSoFar |= unit;
                }
                if (unitsSoFar == 0)
                {
                    return ParseResult<Period>.ForInvalidValue(valueCursor, Messages.Parse_EmptyPeriod);
                }
                return ParseResult<Period>.ForValue(builder.Build());
            }

            public string Format(Period value) => AppendFormat(value, new StringBuilder()).ToString();

            public StringBuilder AppendFormat(Period value, StringBuilder builder)
            {
                Preconditions.CheckNotNull(value, nameof(value));
                Preconditions.CheckNotNull(builder, nameof(builder));
                value = value.Normalize();
                // Always ensure we've got *some* unit; arbitrarily pick days.
                if (value.Equals(Period.Zero))
                {
                    builder.Append("P0D");
                    return builder;
                }
                builder.Append("P");
                AppendValue(builder, value.Years, "Y");
                AppendValue(builder, value.Months, "M");
                AppendValue(builder, value.Weeks, "W");
                AppendValue(builder, value.Days, "D");
                if (value.HasTimeComponent)
                {
                    builder.Append("T");
                    AppendValue(builder, value.Hours, "H");
                    AppendValue(builder, value.Minutes, "M");
                    long ticks = value.Milliseconds * NodaConstants.TicksPerMillisecond + value.Ticks;
                    long seconds = value.Seconds;
                    if (ticks != 0 || seconds != 0)
                    {
                        if (ticks < 0 || seconds < 0)
                        {
                            builder.Append("-");
                            ticks = -ticks;
                            seconds = -seconds;
                        }
                        FormatHelper.FormatInvariant(seconds, builder);
                        if (ticks != 0)
                        {
                            builder.Append(".");
                            FormatHelper.AppendFractionTruncate((int)ticks, 7, 7, builder);
                        }
                        builder.Append("S");
                    }
                }
                return builder;
            }
        }
    }
}
