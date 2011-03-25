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
using System.Text;

namespace NodaTime.Format
{
    /// <summary>
    ///   Provides helper methods for formatting values using pattern strings.
    /// </summary>
    internal static class FormatHelper
    {
        /// <summary>
        ///   The maximum number of characters allowed for padded values.
        /// </summary>
        internal const int MaximumPaddingLength = 16;

        private static readonly string[] FixedNumberFormats = new[]
                                                              {
                                                                  "0", "00", "000", "0000", "00000", "000000", "0000000", "00000000", "000000000", "0000000000",
                                                                  "00000000000", "000000000000", "0000000000000", "00000000000000", "000000000000000",
                                                                  "0000000000000000"
                                                              };

        /// <summary>
        ///   Formats a sign character.
        /// </summary>
        /// <param name = "value">The <see cref = "ISignedValue" /> value.</param>
        /// <param name = "required">if set to <c>true</c> the sin will always be output, otherwise only if the value is negative.</param>
        /// <param name = "outputBuffer">The output buffer to add the sign to.</param>
        internal static void FormatSign(ISignedValue value, bool required, StringBuilder outputBuffer)
        {
            if (required || value.IsNegative)
            {
                outputBuffer.Append(value.Sign);
            }
        }

        /// <summary>
        ///   Formats the given value left padded with zeros.
        /// </summary>
        /// <remarks>
        ///   Left pads with zeros the value into a field of <paramref name = "length" /> characters. If the value
        ///   is longer than <paramref name = "length" />, the entire value is formatted.
        /// </remarks>
        /// <param name = "value">The value to format.</param>
        /// <param name = "length">The length to fill.</param>
        /// <param name = "outputBuffer">The output buffer to add the digits to.</param>
        /// <exception cref = "FormatException">if too many characters are requested. <see cref = "MaximumPaddingLength" />.</exception>
        internal static void LeftPad(int value, int length, StringBuilder outputBuffer)
        {
            if (length > MaximumPaddingLength)
            {
                throw new FormatException("Too many digits");
            }
            var digits = new char[MaximumPaddingLength];
            int pos = MaximumPaddingLength;
            int num = value;
            do
            {
                digits[--pos] = "0123456789"[num % 10];
                num /= 10;
            } while (num != 0 && pos > 0);
            while ((MaximumPaddingLength - pos) < length)
            {
                digits[--pos] = '0';
            }
            outputBuffer.Append(digits, pos, MaximumPaddingLength - pos);
        }

        /// <summary>
        ///   Formats the given value right padded with zeros.
        /// </summary>
        /// <param name = "value">The value to format.</param>
        /// <param name = "length">The length to fill.</param>
        /// <param name = "scale">The scale of the value i.e. the number of significant digits is the range of the value.</param>
        /// <param name = "outputBuffer">The output buffer to add the digits to.</param>
        internal static void RightPad(int value, int length, int scale, StringBuilder outputBuffer)
        {
            if (scale < 1)
            {
                throw new FormatException("scale < 1");
            }
            if (length > scale)
            {
                throw new FormatException("length > scale");
            }
            long relevantDigits = value;
            relevantDigits /= (long)Math.Pow(10.0, (scale - length));
            outputBuffer.Append(((int)relevantDigits).ToString(FixedNumberFormats[length - 1], CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///   Formats the given value right padded with zeros. The rightmost zeros are truncated.  If the entire value is truncated then
        ///   the preceeding decimal separater is also removed.
        /// </summary>
        /// <param name = "value">The value to format.</param>
        /// <param name = "length">The length to fill.</param>
        /// <param name = "scale">The scale of the value i.e. the number of significant digits is the range of the value.</param>
        /// <param name = "decimalSeparator">The decimal separator for this culture.</param>
        /// <param name = "outputBuffer">The output buffer to add the digits to.</param>
        internal static void RightPadTruncate(int value, int length, int scale, string decimalSeparator, StringBuilder outputBuffer)
        {
            if (scale < 1)
            {
                throw new FormatException("scale < 1");
            }
            if (length > scale)
            {
                throw new FormatException("length > scale");
            }
            long relevantDigits = value;
            relevantDigits /= (long)Math.Pow(10.0, (scale - length));
            int relevantLength = length;
            while (relevantLength > 0)
            {
                if ((relevantDigits % 10L) != 0L)
                {
                    break;
                }
                relevantDigits /= 10L;
                relevantLength--;
            }
            if (relevantLength > 0)
            {
                outputBuffer.Append(((int)relevantDigits).ToString(FixedNumberFormats[relevantLength - 1], CultureInfo.InvariantCulture));
            }
            else if (outputBuffer.Length > 0 && outputBuffer.ToString().EndsWith(decimalSeparator))
            {
                var decimalSeparatorLength = decimalSeparator.Length;
                outputBuffer.Remove(outputBuffer.Length - decimalSeparatorLength, decimalSeparatorLength);
            }
        }
    }
}