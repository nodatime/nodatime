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
    ///   Provides a <see cref = "FormatterBase{T}" /> factory for generating <see cref = "Instant" />
    ///   formatters base on the format string.
    /// </summary>
    internal static class InstantFormatter
    {
        internal static readonly FormatterBase<Instant> GeneralFormatter = new InstantGeneralFormatter();
        private static readonly FormatterBase<Instant> TicksFormatterN = new InstantTicksFormatter("N0");
        private static readonly FormatterBase<Instant> TicksFormatterD = new InstantTicksFormatter("D");

        /// <summary>
        ///   Gets the formatter for the given format string.
        /// </summary>
        /// <param name = "format">The format string.</param>
        /// <returns>The <see cref = "FormatterBase{T}" /> corresponding to the format string.</returns>
        /// <exception cref = "FormatException">format is invalid or not supported.</exception>
        internal static FormatterBase<Instant> GetFormatter(string format)
        {
            if (string.IsNullOrEmpty(format) || format == "G" || format == "g")
            {
                return GeneralFormatter;
            }
            if (format == "N" || format == "n")
            {
                return TicksFormatterN;
            }
            if (format == "D" || format == "d")
            {
                return TicksFormatterD;
            }
            throw new FormatException("Instant does not support the '" + format + "' format");
        }

        /// <summary>
        ///   Provides an implementation of <see cref = "FormatterBase{T}" /> that formats <see cref = "Instant" />
        ///   objects as a number of ticks.
        /// </summary>
        private class InstantTicksFormatter : FormatterBase<Instant>
        {
            private readonly string format;

            /// <summary>
            ///   Initializes a new instance of the <see cref = "InstantTicksFormatter" /> class.
            /// </summary>
            /// <param name = "format">The <see cref = "Int64" /> format string to use.</param>
            public InstantTicksFormatter(string format)
            {
                this.format = format;
            }

            /// <summary>
            ///   Overridden in subclasses to provides the actual formatting implementation.
            /// </summary>
            /// <param name = "value">The value to format. This can be <c>null</c> if T is a reference type.</param>
            /// <param name = "formatProvider">The format provider to use. This will never be <c>null</c>.</param>
            /// <returns>The formatted string.</returns>
            protected override string FormatValue(Instant value, IFormatProvider formatProvider)
            {
                return value.Ticks.ToString(format, formatProvider);
            }
        }

        /// <summary>
        ///   Provides an implementation of <see cref = "FormatterBase{T}" /> that formats <see cref = "Instant" />
        ///   objects in the general format.
        /// </summary>
        private class InstantGeneralFormatter : FormatterBase<Instant>
        {
            /// <summary>
            ///   Overridden in subclasses to provides the actual formatting implementation.
            /// </summary>
            /// <param name = "value">The value to format. This can be <c>null</c> if T is a reference type.</param>
            /// <param name = "formatProvider">The format provider to use. This will never be <c>null</c>.</param>
            /// <returns>The formatted string.</returns>
            protected override string FormatValue(Instant value, IFormatProvider formatProvider)
            {
                if (value.Ticks == Instant.MinValue.Ticks)
                {
                    return Instant.BeginningOfTimeLabel;
                }
                if (value.Ticks == Instant.MaxValue.Ticks)
                {
                    return Instant.EndOfTimeLabel;
                }

                // TODO: Use LocalDateTime formatting when available
                var utc = new LocalDateTime(new LocalInstant(value.Ticks));
                // We have to use the invariant culture rather than the passed in provider
                // because the ISO8601 is culture invariant.
                return string.Format(CultureInfo.InvariantCulture, "{0}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}Z", utc.Year, utc.MonthOfYear, utc.DayOfMonth,
                                     utc.HourOfDay, utc.MinuteOfHour, utc.SecondOfMinute);
            }
        }
    }
}