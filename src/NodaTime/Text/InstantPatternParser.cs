// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using NodaTime.Utility;
using System.Text;

namespace NodaTime.Text
{
    /// <summary>
    /// Pattern parsing support for <see cref="Instant" />.
    /// </summary>
    /// <remarks>
    /// Supported standard patterns:
    /// <list type="bullet">
    ///   <item><description>g: general; the UTC ISO-8601 instant in the style uuuu-MM-ddTHH:mm:ssZ</description></item>
    /// </list>
    /// </remarks>
    internal sealed class InstantPatternParser : IPatternParser<Instant>
    {
        private const string GeneralPatternText = "uuuu'-'MM'-'dd'T'HH':'mm':'ss'Z'";
        internal const string BeforeMinValueText = "StartOfTime";
        internal const string AfterMaxValueText = "EndOfTime";

        public IPattern<Instant> ParsePattern([NotNull] string patternText, NodaFormatInfo formatInfo)
        {
            Preconditions.CheckNotNull(patternText, nameof(patternText));
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(TextErrorMessages.FormatStringEmpty);
            }
            if (patternText.Length == 1)
            {
                switch (patternText)
                {
                    case "g": // Simplest way of handling the general pattern...
                        patternText = GeneralPatternText;
                        break;
                    default:
                        throw new InvalidPatternException(TextErrorMessages.UnknownStandardFormat, patternText, typeof(Instant));
                }
            }

            IPattern<LocalDateTime> localResult = formatInfo.LocalDateTimePatternParser.ParsePattern(patternText);
            return new LocalDateTimePatternAdapter(localResult);
        }

        // This not only converts between LocalDateTime and Instant; it also handles infinity.
        private sealed class LocalDateTimePatternAdapter : IPattern<Instant>
        {
            private readonly IPattern<LocalDateTime> pattern;

            internal LocalDateTimePatternAdapter(IPattern<LocalDateTime> pattern)
            {
                this.pattern = pattern;
            }

            public string Format(Instant value) =>
                // We don't need to be able to parse before-min/after-max values, but it's convenient to be
                // able to format them - mostly for the sake of testing (but also for ZoneInterval).
                value.IsValid ? pattern.Format(value.InUtc().LocalDateTime)
                    : value == Instant.BeforeMinValue ? BeforeMinValueText
                    : AfterMaxValueText;

            public StringBuilder AppendFormat(Instant value, [NotNull] StringBuilder builder) =>
                pattern.AppendFormat(value.InUtc().LocalDateTime, builder);

            public ParseResult<Instant> Parse(string text) =>
                pattern.Parse(text).Convert(local => new Instant(local.Date.DaysSinceEpoch, local.NanosecondOfDay));
        }
    }
}
