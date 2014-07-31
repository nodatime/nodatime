// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using NodaTime.Globalization;
using NodaTime.Properties;
using NodaTime.Text.Patterns;
using NodaTime.Utility;

namespace NodaTime.Text
{
    /// <summary>
    /// Pattern parsing support for <see cref="Instant" />.
    /// </summary>
    /// <remarks>
    /// Supported standard patterns:
    /// <list type="bullet">
    ///   <item><description>g: general; the UTC ISO-8601 instant in the style yyyy-MM-ddTHH:mm:ssZ</description></item>
    /// </list>
    /// </remarks>
    internal sealed class InstantPatternParser : IPatternParser<Instant>
    {
        private const string GeneralPatternText = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";

        public IPattern<Instant> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            Preconditions.CheckNotNull(patternText, "patternText");
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(Messages.Parse_FormatStringEmpty);
            }
            if (patternText.Length == 1)
            {
                switch (patternText)
                {
                    case "g": // Simplest way of handling the general pattern...
                        patternText = GeneralPatternText;
                        break;
                    default:
                        throw new InvalidPatternException(Messages.Parse_UnknownStandardFormat, patternText, typeof(Instant));
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

            public string Format(Instant value)
            {
                return pattern.Format(value.InUtc().LocalDateTime);
            }

            public ParseResult<Instant> Parse(string text)
            {
                return pattern.Parse(text).Convert(local => new Instant(local.Date.DaysSinceEpoch, local.NanosecondOfDay));
            }
        }
    }
}
