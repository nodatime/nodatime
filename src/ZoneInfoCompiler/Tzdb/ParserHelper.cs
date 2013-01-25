// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using NodaTime.Utility;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    ///   Contains helper methods for parsing the TZFB files.
    /// </summary>
    internal static class ParserHelper
    {
        /// <summary>
        ///   Converts an hour string to its long value.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>The hour in the range [-23, 23].</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
        /// <exception cref="FormatException">If the text is not a valid integer in the range [-23, 23].</exception>
        internal static long ConvertHourToTicks(string text)
        {
            int value = Int32.Parse(text, NumberStyles.Integer, CultureInfo.InvariantCulture);
            if (value < -23 || value > 23)
            {
                throw new FormatException("hours out of valid range of [-23, 23]: " + value);
            }
            return value * NodaConstants.TicksPerHour;
        }

        /// <summary>
        ///   Converts a minute string to its long value.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>The minute in the range [0, 59].</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
        /// <exception cref="FormatException">If the text is not a valid integer in the range [0, 59].</exception>
        internal static long ConvertMinuteToTicks(string text)
        {
            int value = Int32.Parse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture);
            if (value < 0 || value > 59)
            {
                throw new FormatException("minutes out of valid range of [0, 59]: " + value);
            }
            return value * NodaConstants.TicksPerMinute;
        }

        /// <summary>
        ///   Converts a second string to its double value.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>The second in the range [0, 60).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
        /// <exception cref="FormatException">If the text is not a valid integer in the range [0, 60).</exception>
        internal static long ConvertSecondsWithFractionalToTicks(string text)
        {
            double number = Double.Parse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowDecimalPoint,
                                         CultureInfo.InvariantCulture);
            if (number < 0.0 || number >= 60.0)
            {
                throw new FormatException("seconds out of valid range of [0, 60): " + number);
            }
            long value = (long)(number * NodaConstants.MillisecondsPerSecond) * NodaConstants.TicksPerMillisecond;
            return value;
        }

        /// <summary>
        /// Formats the optional.
        /// </summary>
        /// <param name="value">The value.</param>
        public static string FormatOptional(string value)
        {
            return value ?? "-";
        }

        /// <summary>
        ///   Parses the given text for an integer. Leading and trailing white space is ignored.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="defaultValue">The default value to use if the number cannot be parsed.</param>
        /// <returns>An integer.</returns>
        /// <exception cref="FormatException">If the text is not a valid integer.</exception>
        internal static int ParseInteger(string text, int defaultValue)
        {
            int value = defaultValue;
            if (text != null)
            {
                if (!Int32.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                {
                    value = defaultValue;
                }
            }
            return value;
        }

        /// <summary>
        ///   Parses a time offset string into an integer number of ticks.
        /// </summary>
        /// <param name="text">The value to parse.</param>
        /// <returns>an integer number of ticks</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
        public static Offset ParseOffset(string text)
        {
            Preconditions.CheckNotNull(text, "text");
            int sign = 1;
            if (text.StartsWith("-", StringComparison.Ordinal))
            {
                sign = -1;
                text = text.Substring(1);
            }
            var parts = Regex.Split(text, ":", RegexOptions.CultureInvariant | RegexOptions.Compiled);
            if (parts.Length > 3)
            {
                throw new FormatException("Offset has too many colon separated parts (max of 3 allowed): " + text);
            }
            long ticks = ConvertHourToTicks(parts[0]);
            if (parts.Length > 1)
            {
                ticks += ConvertMinuteToTicks(parts[1]);
                if (parts.Length > 2)
                {
                    ticks += ConvertSecondsWithFractionalToTicks(parts[2]);
                }
            }
            ticks = ticks * sign;
            return Offset.FromTicks(ticks);
        }

        /// <summary>
        ///   Parses an optional value. If the string value is "-" then null is returned otherwise the
        ///   input string is returned.
        /// </summary>
        /// <param name="text">The value to parse.</param>
        /// <returns>The input string or null.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
        public static string ParseOptional(String text)
        {
            Preconditions.CheckNotNull(text, "text");
            return text == "-" ? null : text;
        }

        /// <summary>
        ///   Parses the year.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The parsed year.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        public static int ParseYear(String text, int defaultValue)
        {
            text = text.ToLowerInvariant();
            switch (text)
            {
                case "min":
                case "minimum":
                    return Int32.MinValue;
                case "max":
                case "maximum":
                    return Int32.MaxValue;
                case "only":
                    return defaultValue;
                default:
                    return Int32.Parse(text, CultureInfo.InvariantCulture);
            }
        }
    }
}
