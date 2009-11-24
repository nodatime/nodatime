#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    /// Contains helper methods for parsing the TZFB files.
    /// </summary>
    internal class ParserHelper
    {
        private const int MillisecondsPerSecond = 1000;
        private const int MillisecondsPerMinute = 60 * MillisecondsPerSecond;
        private const int MillisecondsPerHour = 60 * MillisecondsPerMinute;

        /// <summary>
        /// Parses the year.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The parsed year.</returns>
        public static int ParseYear(String text, int defaultValue)
        {
            text = text.ToLowerInvariant();
            if (text == "minimum" || text == "min") {
                return Int32.MinValue;
            }
            else if (text == "maximum" || text == "max") {
                return Int32.MaxValue;
            }
            else if (text == "only") {
                return defaultValue;
            }
            return Int32.Parse(text, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses an optional value. If the string value is "-" then null is returned otherwise the
        /// input string is returned.
        /// </summary>
        /// <param name="text">The value to parse.</param>
        /// <returns>The input string or null.</returns>
        /// <exception cref="ArgumentNullException">If the text is null.</exception>
        public static string ParseOptional(String text)
        {
            if (text == null) {
                throw new ArgumentNullException("value cannot be null");
            }
            return text == "-" ? null : text;
        }

        /// <summary>
        /// Formats the optional.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string FormatOptional(string value)
        {
            if (value == null) {
                return "-";
            }
            return value;
        }

        /// <summary>
        /// Parses a time offset string into an integer number of milliseconds.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>an integer number of milliseconds</returns>
        /// <exception cref="ArgumentNullException">If the text is null.</exception>
        public static int ParseOffset(string text)
        {
            if (text == null) {
                throw new ArgumentNullException("value cannot be null");
            }
            string[] parts = Regex.Split(text, ":", RegexOptions.CultureInvariant | RegexOptions.Compiled);
            if (parts.Length > 3) {
                throw new FormatException("Offset has too many colon separated parts (max of 3 allowed): " + text);
            }
            int milliseconds = ConvertHourToMilliseconds(parts[0]);
            if (parts.Length > 1) {
                milliseconds += ConvertMinuteToMilliseconds(parts[1]);
                if (parts.Length > 2) {
                    milliseconds += ConvertSecondsWithFractionalToMilliseconds(parts[2]);
                }
            }
            return milliseconds;
        }

        /// <summary>
        /// Converts an hour string to its integer value.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>The hour in the range [-23, 23].</returns>
        /// <exception cref="ArgumentNullException">If the text is null.</exception>
        /// <exception cref="FormatException">If the text is not a valid integer in the range [-23, 23].</exception>
        internal static int ConvertHourToMilliseconds(string text)
        {
            int value = Int32.Parse(text, NumberStyles.Integer, CultureInfo.InvariantCulture);
            if (value < -23 || value > 23) {
                throw new FormatException("hours out of valid range of [-23, 23]: " + value);
            }
            return value * MillisecondsPerHour;
        }

        /// <summary>
        /// Converts a minute string to its integer value.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>The minute in the range [0, 59].</returns>
        /// <exception cref="ArgumentNullException">If the text is null.</exception>
        /// <exception cref="FormatException">If the text is not a valid integer in the range [0, 59].</exception>
        internal static int ConvertMinuteToMilliseconds(string text)
        {
            int value = Int32.Parse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture);
            if (value < 0 || value > 59) {
                throw new FormatException("hours out of valid range of [0, 59]: " + value);
            }
            return value * MillisecondsPerMinute;
        }

        /// <summary>
        /// Converts a second string to its double value.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>The second in the range [0, 60).</returns>
        /// <exception cref="ArgumentNullException">If the text is null.</exception>
        /// <exception cref="FormatException">If the text is not a valid integer in the range [0, 60).</exception>
        internal static int ConvertSecondsWithFractionalToMilliseconds(string text)
        {
            double number = Double.Parse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            if (number < 0.0 || number >= 60.0) {
                throw new FormatException("seconds out of valid range of [0, 60): " + number);
            }
            int value = (int)(number * MillisecondsPerSecond);
            return value;
        }

        /// <summary>
        /// Parses the given text for a positive integer or zero integer. Leading and trailing white space is ignored.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>A positive or zero integer.</returns>
        /// <exception cref="ArgumentNullException">If the text is null.</exception>
        /// <exception cref="FormatException">If the text is not a valid positive or zero integer.</exception>
        internal static int ParsePositiveInteger(string text)
        {
            return Int32.Parse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses the given text for a positive integer or zero integer. Leading and trailing white space is ignored.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="defaultValue">The default value to use if the number cannot be parsed.</param>
        /// <returns>A positive or zero integer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the defaultValue is not positive or zero.</exception>
        /// <exception cref="FormatException">If the text is not a valid positive or zero integer.</exception>
        internal static int ParsePositiveInteger(string text, int defaultValue)
        {
            if (defaultValue < 0) {
                throw new ArgumentOutOfRangeException("defaultValue", "defaultValue must be a positive integer or zero.");
            }
            int value = defaultValue;
            if (text != null) {
                if (!Int32.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out value)) {
                    value = defaultValue;
                }
            }
            return value;
        }

        /// <summary>
        /// Parses the given text for an integer. Leading and trailing white space is ignored.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>An integer.</returns>
        /// <exception cref="ArgumentNullException">If the text is null.</exception>
        /// <exception cref="FormatException">If the text is not a valid integer.</exception>
        internal static int ParseInteger(string text)
        {
            return Int32.Parse(text, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses the given text for an integer. Leading and trailing white space is ignored.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="defaultValue">The default value to use if the number cannot be parsed.</param>
        /// <returns>An integer.</returns>
        /// <exception cref="FormatException">If the text is not a valid integer.</exception>
        internal static int ParseInteger(string text, int defaultValue)
        {
            int value = defaultValue;
            if (text != null) {
                if (!Int32.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) {
                    value = defaultValue;
                }
            }
            return value;
        }

        /// <summary>
        /// Formats the given millisecons offset as a string parsable by ParseOffset().
        /// </summary>
        /// <param name="milliseconds">The milliseconds to format.</param>
        /// <returns>The formatted string</returns>
        internal static string FormatOffset(int milliseconds)
        {
            StringBuilder builder = new StringBuilder();
            if (milliseconds < 0) {
                builder.Append("-");
                milliseconds = -milliseconds;
            }
            int hours = milliseconds / MillisecondsPerHour;
            milliseconds -= (hours * MillisecondsPerHour);
            builder.Append(hours.ToString("D", CultureInfo.InvariantCulture));
            int minutes = milliseconds / MillisecondsPerMinute;
            milliseconds -= (minutes * MillisecondsPerMinute);
            builder.Append(":");
            builder.Append(minutes.ToString("D2", CultureInfo.InvariantCulture));
            if (milliseconds > 0) {
                int seconds = milliseconds / MillisecondsPerSecond;
                milliseconds -= (seconds * MillisecondsPerSecond);
                builder.Append(":");
                builder.Append(seconds.ToString("D2", CultureInfo.InvariantCulture));
                if (milliseconds > 0) {
                    builder.Append(".");
                    builder.Append(milliseconds.ToString("D3", CultureInfo.InvariantCulture));
                }
            }
            return builder.ToString();
        }
    }
}
