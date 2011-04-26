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

#region usings
using System;
using System.Globalization;
using NodaTime.Globalization;
using NodaTime.Utility;
#endregion

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
                throw new FormatException("Invalid format string: precision not allowed");
            }
            char formatChar = Char.ToLowerInvariant(format[0]);
            if (Char.IsWhiteSpace(formatChar))
            {
                throw new FormatException("Invalid format string: format cannot contain whitespace");
            }
            switch (formatChar)
            {
                case 'g':
                    return new FormatterG(formatProvider);
                case 'd':
                    return new FormatterD(formatProvider);
                case 'n':
                    return new FormatterN(formatProvider);
                default:
                    throw new FormatException("Invalid format string: unknown flag");
            }
        }

        #region Nested type: FormatterD
        private sealed class FormatterD : AbstractNodaFormatter<Instant>
        {
            public FormatterD(IFormatProvider formatProvider)
                : base(formatProvider)
            {
            }

            public override string Format(Instant value, IFormatProvider formatProvider)
            {
                var formatInfo = NodaFormatInfo.GetInstance(formatProvider);
                return value.Ticks.ToString("D", formatInfo);
            }

            public override INodaFormatter<Instant> WithFormatProvider(IFormatProvider formatProvider)
            {
                return new FormatterD(formatProvider);
            }
        }
        #endregion

        #region Nested type: FormatterG
        private sealed class FormatterG : AbstractNodaFormatter<Instant>
        {
            public FormatterG(IFormatProvider formatProvider)
                : base(formatProvider)
            {
            }

            public override string Format(Instant value, IFormatProvider formatProvider)
            {
                if (value.Ticks == Instant.MinValue.Ticks)
                {
                    return Instant.BeginningOfTimeLabel;
                }
                if (value.Ticks == Instant.MaxValue.Ticks)
                {
                    return Instant.EndOfTimeLabel;
                }
                var utc = SystemConversions.InstantToDateTime(value);
                return string.Format(CultureInfo.InvariantCulture, "{0}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}Z", utc.Year, utc.Month, utc.Day,
                                     utc.Hour, utc.Minute, utc.Second);
            }

            public override INodaFormatter<Instant> WithFormatProvider(IFormatProvider formatProvider)
            {
                return new FormatterG(formatProvider);
            }
        }
        #endregion

        #region Nested type: FormatterN
        private sealed class FormatterN : AbstractNodaFormatter<Instant>
        {
            public FormatterN(IFormatProvider formatProvider)
                : base(formatProvider)
            {
            }

            public override string Format(Instant value, IFormatProvider formatProvider)
            {
                var formatInfo = NodaFormatInfo.GetInstance(formatProvider);
                return value.Ticks.ToString("N0", formatInfo);
            }

            public override INodaFormatter<Instant> WithFormatProvider(IFormatProvider formatProvider)
            {
                return new FormatterN(formatProvider);
            }
        }
        #endregion
    }
}