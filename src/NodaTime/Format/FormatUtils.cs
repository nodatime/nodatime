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
        private const int LengthOfSampleText = 32;
        private const int LengthOfDots = 3;
        private const string Dots = "...";

        static readonly double Log10 = Math.Log(10);

        private const char UnicodeReplacementCharacter = '\ufffd';

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
                {
                    value = -value;
                }
                else
                {
                    for (; size > 10; size--)
                    {
                        builder.Append('0');
                    }
                    builder.Append(-(long)int.MinValue);
                    return;
                }
            }

            if (value < 10)
            {
                for (; size > 1; size--)
                {
                    builder.Append('0');
                }
                builder.Append((char)(value + '0'));
            }
            else if (value < 100)
            {
                // Calculate value div/mod by 10 without using two expensive
                // division operations. (2 ^ 27) / 10 = 13421772. Add one to
                // value to correct rounding error.
                int d = ((value + 1) * 13421772) >> 27;
                for (; size > 2; size--)
                {
                    builder.Append('0');
                }
                builder.Append((char)(d + '0'));
                // Append remainder by calculating (value - d * 10).
                builder.Append((char)(value - (d << 3) - (d << 1) + '0'));
            }
            else
            {
                int digits = value < 1000 ? 3
                    : value < 10000 ? 4
                    : (int)(Math.Log(value) / Log10) + 1;

                for (; size > digits; size--)
                {
                    builder.Append('0');
                }

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
                {
                    value = -value;
                }
                else
                {
                    builder.Append(-(long) int.MinValue);
                    return;
                }
            }

            if (value < 10)
            {
                builder.Append((char)(value + '0'));
            }
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
            {
                builder.Append(value);
            }
        }

        //TODO: measure how much this optimized method is faster then .NET intrinsic formatting
        // value.ToString("D");

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
                {
                    value = -value;
                }
                else
                {
                    writer.Write(-(long) int.MinValue);
                    return;
                }
            }

            if (value < 10)
            {
                writer.Write((char) (value + '0'));
            }
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
            {
                writer.Write(value);
            }
        }

        //TODO: measure how much this optimized method is faster then .NET intrinsic formatting
        // value.ToString("D" + size.ToString);

        /// <summary>
        /// Converts an integer to a string, prepended with a variable amount of '0'
        /// pad characters, and writes it to the given writer.
        /// </summary>
        /// <remarks>
        /// This method is optimized for converting small values to strings.
        /// </remarks>
        /// <param name="writer">Receives integer converted to a string</param>
        /// <param name="value">Value to convert to a string</param>
        /// <param name="size">Minumum amount of digits to append</param>
        internal static void WritePaddedInteger(TextWriter writer, int value, int size)
        {
            if (value < 0)
            {
                writer.Write('-');
                if (value != int.MinValue)
                {
                    value = -value;
                }
                else
                {
                    for (; size > 10; size--)
                    {
                        writer.Write('0');
                    }
                    writer.Write(-(long)int.MinValue);
                    return;
                }
            }

            if (value < 10)
            {
                for (; size > 1; size--)
                {
                    writer.Write('0');
                }
                writer.Write((char)(value + '0'));
            }
            else if (value < 100)
            {
                // Calculate value div/mod by 10 without using two expensive
                // division operations. (2 ^ 27) / 10 = 13421772. Add one to
                // value to correct rounding error.
                int d = ((value + 1) * 13421772) >> 27;
                for (; size > 2; size--)
                {
                    writer.Write('0');
                }
                writer.Write((char)(d + '0'));
                // Append remainder by calculating (value - d * 10).
                writer.Write((char)(value - (d << 3) - (d << 1) + '0'));
            }
            else
            {
                int digits = value < 1000 ? 3
                    : value < 10000 ? 4
                    : (int)(Math.Log(value) / Log10) + 1;

                for (; size > digits; size--)
                {
                    writer.Write('0');
                }

                writer.Write(value);
            }
        }

        internal static int CalculateDigitCount(long value)
        {
            if (value < 0)
            {
                return value == long.MinValue ? 20 : CalculateDigitCount(-value) + 1;
            }

            return
                value < 10 ? 1 :
                value < 100 ? 2 :
                value < 1000 ? 3 :
                value < 10000 ? 4 :
                (int)(Math.Log(value) / Log10 + 1);
        }

        internal static int ParseTwoDigits(String text, int position)
        {
            int value = text[position] - '0';
            return ((value << 3) + (value << 1)) + text[position + 1] - '0';
        }

        internal static string CreateErrorMessage(string text, int errorPosition)
        {
            int sampleLen = errorPosition + LengthOfSampleText;
            string sampleText = text.Length <= sampleLen + LengthOfDots ? text
                : text.Substring(0, sampleLen) + Dots;

            string prefix = "Invalid format: \"" + sampleText + '"';

            return errorPosition <= 0 ? prefix
                : errorPosition >= text.Length ? prefix + " is too short"
                : prefix + " is malformed at \"" + sampleText.Substring(errorPosition) + '"';
        }

        internal static int MatchSubstring(string targetString, int startAt, string textToFind)
        {
            if (String.IsNullOrEmpty(textToFind))
            {
                return startAt;
            }

            if (startAt + textToFind.Length > targetString.Length)
            {
                return ~startAt;
            }

            string targetSubString = targetString.Substring(startAt, textToFind.Length);

            return targetSubString.Equals(textToFind, StringComparison.OrdinalIgnoreCase) 
                ? startAt + textToFind.Length
                : ~startAt;
        }

        internal static void WriteUnknownString(TextWriter writer)
        {
            writer.Write(UnicodeReplacementCharacter);
        }

        internal static void WriteUnknownString(TextWriter writer, int len)
        {
            for (int i = len; --i >= 0; )
            {
                writer.Write(UnicodeReplacementCharacter);
            }
        }
    }
}
