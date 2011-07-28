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

        private static readonly double Log10 = Math.Log(10);

        private const char UnicodeReplacementCharacter = '\ufffd';

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
                    writer.Write(-(long)int.MinValue);
                    return;
                }
            }

            if (value < 10)
            {
                writer.Write((char)(value + '0'));
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
        /// <param name="size">Minumum amount of significant digits to append(negative sign is not counted)</param>
        internal static void WritePaddedInteger(TextWriter writer, int value, int size)
        {
            if (size <= 1)
            {
                WriteUnpaddedInteger(writer, value);
                return;
            }

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
                int digits = value < 1000 ? 3 : value < 10000 ? 4 : (int)(Math.Log(value) / Log10) + 1;

                for (; size > digits; size--)
                {
                    writer.Write('0');
                }

                writer.Write(value);
            }
        }

        internal static int ParseTwoDigits(String text, int position)
        {
            int value = text[position] - '0';
            return ((value << 3) + (value << 1)) + text[position + 1] - '0';
        }

        internal static int ParseDigits(String text, int position, int length)
        {
            if (length >= 10)
            {
                // Since value may exceed max, use stock parser which checks for this.
                return Int32.Parse(text.Substring(position, position + length), CultureInfo.InvariantCulture);
            }
            if (length <= 0)
            {
                return 0;
            }

            int value = text[position++];
            length--;
            bool negative;
            if (value == '-')
            {
                if (length < 0)
                {
                    return 0;
                }
                negative = true;
                value = text[position++];
                length--;
            }
            else
            {
                negative = false;
            }
            value -= '0';
            while (length-- > 0)
            {
                value = ((value << 3) + (value << 1)) + text[position++] - '0';
            }
            return negative ? -value : value;
        }

        internal static string CreateErrorMessage(string text, int errorPosition)
        {
            int sampleLen = errorPosition + LengthOfSampleText;
            string sampleText = text.Length <= sampleLen + LengthOfDots ? text : text.Substring(0, sampleLen) + Dots;

            string prefix = "Invalid format: \"" + sampleText + '"';

            return errorPosition <= 0
                       ? prefix
                       : errorPosition >= text.Length ? prefix + " is too short" : prefix + " is malformed at \"" + sampleText.Substring(errorPosition) + '"';
        }

        internal static int MatchSubstring(string targetString, int startAt, string textToFind)
        {
            if (String.IsNullOrEmpty(textToFind))
            {
                return startAt;
            }

            if (targetString.Length < textToFind.Length + startAt)
            {
                return ~startAt;
            }

            string targetSubString = targetString.Substring(startAt, textToFind.Length);

            return targetSubString.Equals(textToFind, StringComparison.OrdinalIgnoreCase) ? startAt + textToFind.Length : ~startAt;
        }

        internal static int MatchChar(string targetString, int position, char value)
        {
            if (position >= targetString.Length)
            {
                return ~position;
            }

            char a = targetString[position];
            char b = value;
            if (a == b || Char.ToUpperInvariant(a) == Char.ToUpperInvariant(b))
            {
                return position + 1;
            }

            return ~position;
        }

        internal static void WriteUnknownString(TextWriter writer)
        {
            writer.Write(UnicodeReplacementCharacter);
        }

        internal static void WriteUnknownString(TextWriter writer, int len)
        {
            for (int i = len; --i >= 0;)
            {
                WriteUnknownString(writer);
            }
        }
    }
}