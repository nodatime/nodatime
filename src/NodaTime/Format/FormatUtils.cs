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
using System.Text;
using System.IO;
namespace NodaTime.Format
{
    /// <summary>
    /// Utility methods used by formatters.
    /// FormatUtils is thread-safe and immutable.
    /// </summary>
    internal static class FormatUtils
    {
        private const int LENGTH_OF_SAMPLE_TEXT = 32;
        private const int LENGTH_OF_DOTS = 3;
        private const string DOTS = "...";

        static readonly double LOG_10 = Math.Log(10);

        /// <summary>
        /// Converts an integer to a string, prepended with a variable amount of '0'
        /// pad characters, and appends it to the given buffer.
        /// </summary>
        /// <remarks>
        /// This method is optimized for converting small values to strings.
        /// </remarks>
        /// <param name="builder">Receives integer converted to a string</param>
        /// <param name="value">Value to convert to a string</param>
        /// <param name="size">Minumum amount of digits to append</param>
        internal static void AppendPaddedInteger(StringBuilder builder, int value, int size)
        {
            if (value < 0)
            {
                builder.Append('-');
                if (value != int.MinValue)
                    value = -value;
                else
                {
                    for (; size > 10; size--)
                        builder.Append('0');
                    builder.Append("" + -(long)int.MinValue);
                }
            }

            if (value < 10)
            {
                for (; size > 1; size--)
                    builder.Append('0');
                builder.Append((char)(value + '0'));
            }
            else if (value < 100)
            {
                // Calculate value div/mod by 10 without using two expensive
                // division operations. (2 ^ 27) / 10 = 13421772. Add one to
                // value to correct rounding error.
                int d = ((value + 1) * 13421772) >> 27;
                for (; size > 2; size--)
                    builder.Append('0');
                builder.Append((char)(d + '0'));
                // Append remainder by calculating (value - d * 10).
                builder.Append((char)(value - (d << 3) - (d << 1) + '0'));
            }
            else
            {
                int digits;
                if (value < 1000)
                    digits = 3;
                else if (value < 10000)
                    digits = 4;
                else
                    digits = (int)(Math.Log(value) / LOG_10) + 1;

                for (; size > digits; size--)
                    builder.Append('0');

                builder.Append(value);
            }
        }

        /// <summary>
        /// Converts an integer to a string, and appends it to the given buffer.
        /// </summary>
        /// <remarks>
        /// This method is optimized for converting small values to strings.
        /// </remarks>
        /// <param name="builder">Receives integer converted to a string</param>
        /// <param name="value">Value to convert to a string</param>
        internal static void AppendUnpaddedInteger(StringBuilder builder, int value)
        {
            if (value < 0)
            {
                builder.Append('-');
                if (value != int.MinValue)
                    value = -value;
                else
                    builder.Append("" + -(long)int.MinValue);
            }

            if (value < 10)
                builder.Append((char)(value + '0'));
            else if (value < 100)
            {
                // Calculate value div/mod by 10 without using two expensive
                // division operations. (2 ^ 27) / 10 = 13421772. Add one to
                // value to correct rounding error.
                int d = ((value + 1) * 13421772) >> 27;
                builder.Append((char)(d + '0'));
                // Append remainder by calculating (value - d * 10).
                builder.Append((char)(value - (d << 3) - (d << 1) + '0'));
            }
            else
                builder.Append(value);
        }

        /// <summary>
        /// Converts an integer to a string, and writes it to the given writer.
        /// </summary>
        /// <remarks>
        /// This method is optimized for converting small values to strings.
        /// </remarks>
        /// <param name="writer">Receives integer converted to a string</param>
        /// <param name="value">Value to convert to a string</param>
        internal static void WriteUnpaddedInteger(TextWriter writer, int value)
        {
            if (value < 0)
            {
                writer.Write('-');
                if (value != int.MinValue)
                    value = -value;
                else
                    writer.Write("" + -(long)int.MinValue);
            }

            if (value < 10)
                writer.Write((char)(value + '0'));
            else if (value < 100)
            {
                // Calculate value div/mod by 10 without using two expensive
                // division operations. (2 ^ 27) / 10 = 13421772. Add one to
                // value to correct rounding error.
                int d = ((value + 1) * 13421772) >> 27;
                writer.Write((char)(d + '0'));
                // Append remainder by calculating (value - d * 10).
                writer.Write((char)(value - (d << 3) - (d << 1) + '0'));
            }
            else
                writer.Write(value);
        }

        /// <summary>
        /// Converts an integer to a string, prepended with a variable amount of '0'
        /// pad characters, and writes it to the given writer.
        /// </summary>
        /// <remarks>
        /// This method is optimized for converting small values to strings.
        /// </remarks>
        /// <param name="builder">Receives integer converted to a string</param>
        /// <param name="value">Value to convert to a string</param>
        /// <param name="size">Minumum amount of digits to append</param>
        internal static void WritePaddedInteger(TextWriter writer, int value, int size)
        {
            if (value < 0)
            {
                writer.Write('-');
                if (value != int.MinValue)
                    value = -value;
                else
                {
                    for (; size > 10; size--)
                        writer.Write('0');
                    writer.Write("" + -(long)int.MinValue);
                }
            }

            if (value < 10)
            {
                for (; size > 1; size--)
                    writer.Write('0');
                writer.Write((char)(value + '0'));
            }
            else if (value < 100)
            {
                // Calculate value div/mod by 10 without using two expensive
                // division operations. (2 ^ 27) / 10 = 13421772. Add one to
                // value to correct rounding error.
                int d = ((value + 1) * 13421772) >> 27;
                for (; size > 2; size--)
                    writer.Write('0');
                writer.Write((char)(d + '0'));
                // Append remainder by calculating (value - d * 10).
                writer.Write((char)(value - (d << 3) - (d << 1) + '0'));
            }
            else
            {
                int digits;
                if (value < 1000)
                    digits = 3;
                else if (value < 10000)
                    digits = 4;
                else
                    digits = (int)(Math.Log(value) / LOG_10) + 1;

                for (; size > digits; size--)
                    writer.Write('0');

                writer.Write(value);
            }
        }
        internal static int CalculateDigitCount(long value)
        {
            if (value < 0)
            {
                if (value != long.MinValue)
                    return CalculateDigitCount(-value) + 1;
                else
                    return 20;
            }

            return
                value < 10 ? 1 :
                value < 100 ? 2 :
                value < 1000 ? 3 :
                value < 10000 ? 4 :
                (int)(Math.Log(value) / LOG_10 + 1);

        }

        internal static string CreateErrorMessage(string text, int errorPosition)
        {
            int sampleLen = errorPosition + LENGTH_OF_SAMPLE_TEXT;
            String sampleText;
            if (text.Length <= sampleLen + LENGTH_OF_DOTS)
                sampleText = text;
            else
                sampleText = text.Substring(0, sampleLen) + DOTS;

            if(errorPosition <=0)
                return "Invalid format: \"" + sampleText + '"';

            if (errorPosition >= text.Length)
                return "Invalid format: \"" + sampleText + "\" is too short";

            return "Invalid format: \"" + sampleText + "\" is malformed at \"" +
                sampleText.Substring(errorPosition) + '"';
        }
    }
}
