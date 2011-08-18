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
    ///   Provides a <see cref="INodaFormatter{T}" /> factory for generating <see cref="Duration" />
    ///   formatters base on the format string.
    /// </summary>
    internal static class DurationFormatter
    {
        internal static readonly INodaFormatter<Duration> InvariantGeneralFormatter = new DurationGeneralFormatter(CultureInfo.InvariantCulture);
        private static readonly INodaFormatter<Duration> InvariantShortFormatter = new DurationShortFormatter(CultureInfo.InvariantCulture);
        private static readonly INodaFormatter<Duration> InvariantLongFormatter = new DurationLongFormatter(CultureInfo.InvariantCulture);

        private const string LongPattern = "{0}PT{1:D}H{2:D2}M{3:D2}.{4:D3}S";
        private const string SecondsPattern = "{0}PT{1:D}H{2:D2}M{3:D2}S";
        private const string ShortPattern = "{0}PT{1:D}H{2:D2}M";
        private const string HoursPattern = "{0}PT{1:D}H";

        /// <summary>
        ///   Gets the formatter for the given format string.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <returns>The <see cref="INodaFormatter{T}" /> corresponding to the format string.</returns>
        /// <exception cref="FormatException">format is invalid or not supported.</exception>
        internal static INodaFormatter<Duration> GetFormatter(string format)
        {
            if (string.IsNullOrEmpty(format) || format == "G" || format == "g")
            {
                return InvariantGeneralFormatter;
            }
            if (format == "S" || format == "s")
            {
                return InvariantShortFormatter;
            }
            if (format == "L" || format == "l")
            {
                return InvariantLongFormatter;
            }
            throw new FormatException("Duration does not support the '" + format + "' format");
        }

        /// <summary>
        ///   Provides an implementation of <see cref="INodaFormatter{T}" /> that formats <see cref="Offset" />
        ///   objects in the general format.
        /// </summary>
        private class DurationGeneralFormatter : AbstractNodaFormatter<Duration>
        {
            internal DurationGeneralFormatter(IFormatProvider formatProvider) : base(formatProvider)
            {
            }

            /// <summary>
            ///   Overridden in subclasses to provides the actual formatting implementation.
            /// </summary>
            /// <param name="value">The value to format. This can be <c>null</c> if T is a reference type.</param>
            /// <returns>The formatted string.</returns>
            public override string Format(Duration value)
            {
                bool negative = value.Ticks < 0;
                long millisecondsValue = value.Ticks;
                if (millisecondsValue == Int64.MinValue)
                {
                    millisecondsValue = Int64.MaxValue;
                }
                else if (negative)
                {
                    millisecondsValue = -millisecondsValue;
                }
                long hours = millisecondsValue / NodaConstants.MillisecondsPerHour;
                long minutes = (millisecondsValue % NodaConstants.MillisecondsPerHour) / NodaConstants.MillisecondsPerMinute;
                long seconds = (millisecondsValue % NodaConstants.MillisecondsPerMinute) / NodaConstants.MillisecondsPerSecond;
                millisecondsValue = millisecondsValue % NodaConstants.MillisecondsPerSecond;
                string sign = negative ? "-" : "+";
                string pattern;
                if (millisecondsValue != 0)
                {
                    pattern = LongPattern;
                }
                else if (seconds != 0)
                {
                    pattern = SecondsPattern;
                }
                else if (minutes != 0)
                {
                    pattern = ShortPattern;
                }
                else
                {
                    pattern = HoursPattern;
                }
                // TODO: Use the actual IFormatProvider? (Need to check pattern...)
                return string.Format(CultureInfo.InvariantCulture, pattern, sign, hours, minutes, seconds, millisecondsValue);
            }

            public override INodaFormatter<Duration> WithFormatProvider(IFormatProvider formatProvider)
            {
                return formatProvider == FormatProvider ? this : new DurationGeneralFormatter(formatProvider);
            }
        }

        /// <summary>
        ///   Provides an implementation of <see cref="INodaFormatter{T}" /> that formats <see cref="Offset" />
        ///   objects in the general format.
        /// </summary>
        private class DurationLongFormatter : AbstractNodaFormatter<Duration>
        {
            internal DurationLongFormatter(IFormatProvider formatProvider)
                : base(formatProvider)
            {
            }

            /// <summary>
            ///   Overridden in subclasses to provides the actual formatting implementation.
            /// </summary>
            /// <param name="value">The value to format. This can be <c>null</c> if T is a reference type.</param>
            /// <returns>The formatted string.</returns>
            public override string Format(Duration value)
            {
                bool negative = value.Ticks < 0;
                long millisecondsValue = value.Ticks;
                if (millisecondsValue == Int64.MinValue)
                {
                    millisecondsValue = Int64.MaxValue;
                }
                else if (negative)
                {
                    millisecondsValue = -millisecondsValue;
                }
                long hours = millisecondsValue / NodaConstants.MillisecondsPerHour;
                long minutes = (millisecondsValue % NodaConstants.MillisecondsPerHour) / NodaConstants.MillisecondsPerMinute;
                long seconds = (millisecondsValue % NodaConstants.MillisecondsPerMinute) / NodaConstants.MillisecondsPerSecond;
                millisecondsValue = millisecondsValue % NodaConstants.MillisecondsPerSecond;
                string sign = negative ? "-" : "+";
                // TODO: Use the actual IFormatProvider? (Need to check pattern...)
                return string.Format(CultureInfo.InvariantCulture, LongPattern, sign, hours, minutes, seconds, millisecondsValue);
            }

            public override INodaFormatter<Duration> WithFormatProvider(IFormatProvider formatProvider)
            {
                return formatProvider == FormatProvider ? this : new DurationLongFormatter(formatProvider);
            }
        }

        /// <summary>
        ///   Provides an implementation of <see cref="INodaFormatter{T}" /> that formats <see cref="Offset" />
        ///   objects in the general format.
        /// </summary>
        private class DurationShortFormatter : AbstractNodaFormatter<Duration>
        {
            internal DurationShortFormatter(IFormatProvider formatProvider)
                : base(formatProvider)
            {
            }

            /// <summary>
            ///   Overridden in subclasses to provides the actual formatting implementation.
            /// </summary>
            /// <param name="value">The value to format. This can be <c>null</c> if T is a reference type.</param>
            /// <returns>The formatted string.</returns>
            public override string Format(Duration value)
            {
                bool negative = value.Ticks < 0;
                long millisecondsValue = value.Ticks;
                if (millisecondsValue == Int64.MinValue)
                {
                    millisecondsValue = Int64.MaxValue;
                }
                else if (negative)
                {
                    millisecondsValue = -millisecondsValue;
                }
                long hours = millisecondsValue / NodaConstants.MillisecondsPerHour;
                long minutes = (millisecondsValue % NodaConstants.MillisecondsPerHour) / NodaConstants.MillisecondsPerMinute;
                string sign = negative ? "-" : "+";
                // TODO: Use the actual IFormatProvider? (Need to check pattern...)
                return string.Format(CultureInfo.InvariantCulture, ShortPattern, sign, hours, minutes);
            }

            public override INodaFormatter<Duration> WithFormatProvider(IFormatProvider formatProvider)
            {
                return formatProvider == FormatProvider ? this : new DurationShortFormatter(formatProvider);
            }
        }
    }
}