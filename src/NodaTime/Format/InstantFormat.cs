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
using System.Globalization;
using NodaTime.Utility;

namespace NodaTime.Format
{
    internal static class InstantFormat
    {
        /// <summary>
        ///   Handles common default processing and parameter validation for simple formatting.
        /// </summary>
        /// <param name = "value">The value to format.</param>
        /// <param name = "format">The format string. If <c>null</c> or empty defaults to "g".</param>
        /// <param name = "formatProvider">The <see cref = "IFormatProvider" /> to use. If <c>null</c> the thread's current culture is used.</param>
        /// <exception cref = "FormatException"></exception>
        /// <returns>The value formatted as a string.</returns>
        internal static string Format(Instant value, string format, IFormatProvider formatProvider)
        {
            var formatter = MakeFormatter(format, formatProvider);
            return formatter.Format(value);
        }

        internal static INodaFormatter<Instant> MakeFormatter(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "G";
            }
            if (format.Length > 1)
            {
                throw FormatError.PrecisionNotSupported(format, typeof(Instant));
            }
            char formatChar = Char.ToLowerInvariant(format[0]);
            if (Char.IsWhiteSpace(formatChar))
            {
                throw FormatError.StandardFormatWhitespace(format, typeof(Instant));
            }
            switch (formatChar)
            {
                case 'g':
                    return new InstantFormatterG(formatProvider);
                case 'd':
                    return new InstantFormatterD(formatProvider);
                case 'n':
                    return new InstantFormatterN(formatProvider);
                default:
                    throw FormatError.UnknownStandardFormat(formatChar, typeof(Instant));
            }
        }

        #region Nested type: InstantFormatterD
        private sealed class InstantFormatterD : AbstractNodaFormatter<Instant>
        {
            internal InstantFormatterD(IFormatProvider formatProvider) : base(formatProvider)
            {
            }

            public override string Format(Instant value)
            {
                return value.Ticks.ToString("D", FormatProvider);
            }

            public override INodaFormatter<Instant> WithFormatProvider(IFormatProvider formatProvider)
            {
                return new InstantFormatterD(formatProvider);
            }
        }
        #endregion

        #region Nested type: InstantFormatterG
        private sealed class InstantFormatterG : AbstractNodaFormatter<Instant>
        {
            internal InstantFormatterG(IFormatProvider formatProvider) : base(formatProvider)
            {
            }

            public override string Format(Instant value)
            {
                if (value.Ticks == Instant.MinValue.Ticks)
                {
                    return Instant.BeginningOfTimeLabel;
                }
                if (value.Ticks == Instant.MaxValue.Ticks)
                {
                    return Instant.EndOfTimeLabel;
                }
                DateTime utc = value.ToDateTimeUtc();
                return string.Format(CultureInfo.InvariantCulture, "{0}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}Z", utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute,
                                     utc.Second);
            }

            public override INodaFormatter<Instant> WithFormatProvider(IFormatProvider formatProvider)
            {
                return new InstantFormatterG(formatProvider);
            }
        }
        #endregion

        #region Nested type: InstantFormatterN
        /// <summary>
        /// A Noda formatter implementation for the N pattern (ticks as a group separated number).
        /// </summary>
        private sealed class InstantFormatterN : AbstractNodaFormatter<Instant>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InstantFormatterN"/> class.
            /// </summary>
            /// <param name="formatProvider">The format provider.</param>
            internal InstantFormatterN(IFormatProvider formatProvider) : base(formatProvider)
            {
            }

            public override string Format(Instant value)
            {
                return value.Ticks.ToString("N0", FormatProvider);
            }

            public override INodaFormatter<Instant> WithFormatProvider(IFormatProvider formatProvider)
            {
                return new InstantFormatterN(formatProvider);
            }
        }
        #endregion
    }
}