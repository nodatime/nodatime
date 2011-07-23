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

using System;
using System.Globalization;

namespace NodaTime.Format
{
    /// <summary>
    ///   Provides a <see cref = "FormatterBase{T}" /> factory for generating <see cref = "Offset" />
    ///   formatters base on the format string.
    /// </summary>
    internal static class OffsetFormatter
    {
        internal static readonly FormatterBase<Offset> GeneralFormatter = new OffsetGeneralFormatter();
        private static readonly FormatterBase<Offset> ShortFormatter = new OffsetShortFormatter();
        private static readonly FormatterBase<Offset> LongFormatter = new OffsetLongFormatter();
        private static readonly FormatterBase<Offset> IsoFormatter = new OffsetIsoFormatter();

        private const string LongPatternPt = "{0}PT{1:D}H{2:D2}M{3:D2}.{4:D3}S";
        private const string SecondsPatternPt = "{0}PT{1:D}H{2:D2}M{3:D2}S";
        private const string ShortPatternPt = "{0}PT{1:D}H{2:D2}M";
        private const string HoursPatternPt = "{0}PT{1:D}H";

        private const string MillisecondsPattern = "{0}{1:D}:{2:D2}:{3:D2}.{4:D3}";
        private const string SecondsPattern = "{0}{1:D}:{2:D2}:{3:D2}";
        private const string MinutesPattern = "{0}{1:D}:{2:D2}";
        private const string HoursPattern = "{0}{1:D}";

        /// <summary>
        ///   Gets the formatter for the given format string.
        /// </summary>
        /// <param name = "format">The format string.</param>
        /// <returns>The <see cref = "FormatterBase{T}" /> corresponding to the format string.</returns>
        /// <exception cref = "FormatException">format is invalid or not supported.</exception>
        internal static FormatterBase<Offset> GetFormatter(string format)
        {
            if (string.IsNullOrEmpty(format) || format == "G" || format == "g")
            {
                return GeneralFormatter;
            }
            if (format == "S" || format == "s")
            {
                return ShortFormatter;
            }
            if (format == "L" || format == "l")
            {
                return LongFormatter;
            }
            if (format == "I" || format == "i")
            {
                return IsoFormatter;
            }
            throw new FormatException("Offset does not support the '" + format + "' format");
        }

        /// <summary>
        ///   Provides an implementation of <see cref = "FormatterBase{T}" /> that formats <see cref = "Offset" />
        ///   objects in the general format.
        /// </summary>
        private class OffsetGeneralFormatter : FormatterBase<Offset>
        {
            /// <summary>
            ///   Overridden in subclasses to provides the actual formatting implementation.
            /// </summary>
            /// <param name = "value">The value to format. This can be <c>null</c> if T is a reference type.</param>
            /// <param name = "formatProvider">The format provider to use. This will never be <c>null</c>.</param>
            /// <returns>The formatted string.</returns>
            protected override string FormatValue(Offset value, IFormatProvider formatProvider)
            {
                bool negative = value.Milliseconds < 0;
                int millisecondsValue = negative ? -value.Milliseconds : value.Milliseconds;
                int hours = millisecondsValue / NodaConstants.MillisecondsPerHour;
                int minutes = (millisecondsValue % NodaConstants.MillisecondsPerHour) / NodaConstants.MillisecondsPerMinute;
                int seconds = (millisecondsValue % NodaConstants.MillisecondsPerMinute) / NodaConstants.MillisecondsPerSecond;
                millisecondsValue = millisecondsValue % NodaConstants.MillisecondsPerSecond;
                string sign = negative ? "-" : "+";
                string pattern;
                if (millisecondsValue != 0)
                {
                    pattern = LongPatternPt;
                }
                else if (seconds != 0)
                {
                    pattern = SecondsPatternPt;
                }
                else if (minutes != 0)
                {
                    pattern = ShortPatternPt;
                }
                else
                {
                    pattern = HoursPatternPt;
                }
                return string.Format(CultureInfo.InvariantCulture, pattern, sign, hours, minutes, seconds, millisecondsValue);
            }
        }

        /// <summary>
        ///   Provides an implementation of <see cref = "FormatterBase{T}" /> that formats <see cref = "Offset" />
        ///   objects in the general format.
        /// </summary>
        private class OffsetIsoFormatter : FormatterBase<Offset>
        {
            /// <summary>
            ///   Overridden in subclasses to provides the actual formatting implementation.
            /// </summary>
            /// <param name = "value">The value to format. This can be <c>null</c> if T is a reference type.</param>
            /// <param name = "formatProvider">The format provider to use. This will never be <c>null</c>.</param>
            /// <returns>The formatted string.</returns>
            protected override string FormatValue(Offset value, IFormatProvider formatProvider)
            {
                bool negative = value.Milliseconds < 0;
                int millisecondsValue = negative ? -value.Milliseconds : value.Milliseconds;
                int hours = millisecondsValue / NodaConstants.MillisecondsPerHour;
                int minutes = (millisecondsValue % NodaConstants.MillisecondsPerHour) / NodaConstants.MillisecondsPerMinute;
                int seconds = (millisecondsValue % NodaConstants.MillisecondsPerMinute) / NodaConstants.MillisecondsPerSecond;
                millisecondsValue = millisecondsValue % NodaConstants.MillisecondsPerSecond;
                string sign = negative ? "-" : "+";
                string pattern;
                if (millisecondsValue != 0)
                {
                    pattern = MillisecondsPattern;
                }
                else if (seconds != 0)
                {
                    pattern = SecondsPattern;
                }
                else if (minutes != 0)
                {
                    pattern = MinutesPattern;
                }
                else
                {
                    pattern = HoursPattern;
                }
                return string.Format(CultureInfo.InvariantCulture, pattern, sign, hours, minutes, seconds, millisecondsValue);
            }
        }

        /// <summary>
        ///   Provides an implementation of <see cref = "FormatterBase{T}" /> that formats <see cref = "Offset" />
        ///   objects in the general format.
        /// </summary>
        private class OffsetLongFormatter : FormatterBase<Offset>
        {
            /// <summary>
            ///   Overridden in subclasses to provides the actual formatting implementation.
            /// </summary>
            /// <param name = "value">The value to format. This can be <c>null</c> if T is a reference type.</param>
            /// <param name = "formatProvider">The format provider to use. This will never be <c>null</c>.</param>
            /// <returns>The formatted string.</returns>
            protected override string FormatValue(Offset value, IFormatProvider formatProvider)
            {
                bool negative = value.Milliseconds < 0;
                int millisecondsValue = negative ? -value.Milliseconds : value.Milliseconds;
                int hours = millisecondsValue / NodaConstants.MillisecondsPerHour;
                int minutes = (millisecondsValue % NodaConstants.MillisecondsPerHour) / NodaConstants.MillisecondsPerMinute;
                int seconds = (millisecondsValue % NodaConstants.MillisecondsPerMinute) / NodaConstants.MillisecondsPerSecond;
                millisecondsValue = millisecondsValue % NodaConstants.MillisecondsPerSecond;
                string sign = negative ? "-" : "+";
                return string.Format(CultureInfo.InvariantCulture, LongPatternPt, sign, hours, minutes, seconds, millisecondsValue);
            }
        }

        /// <summary>
        ///   Provides an implementation of <see cref = "FormatterBase{T}" /> that formats <see cref = "Offset" />
        ///   objects in the general format.
        /// </summary>
        private class OffsetShortFormatter : FormatterBase<Offset>
        {
            /// <summary>
            ///   Overridden in subclasses to provides the actual formatting implementation.
            /// </summary>
            /// <param name = "value">The value to format. This can be <c>null</c> if T is a reference type.</param>
            /// <param name = "formatProvider">The format provider to use. This will never be <c>null</c>.</param>
            /// <returns>The formatted string.</returns>
            protected override string FormatValue(Offset value, IFormatProvider formatProvider)
            {
                bool negative = value.Milliseconds < 0;
                int millisecondsValue = negative ? -value.Milliseconds : value.Milliseconds;
                int hours = millisecondsValue / NodaConstants.MillisecondsPerHour;
                int minutes = (millisecondsValue % NodaConstants.MillisecondsPerHour) / NodaConstants.MillisecondsPerMinute;
                string sign = negative ? "-" : "+";
                return string.Format(CultureInfo.InvariantCulture, ShortPatternPt, sign, hours, minutes);
            }
        }
    }
}