// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using System.Text;

namespace NodaTime.Text
{
    /// <summary>
    ///   Provides helper methods for formatting values using pattern strings.
    /// </summary>
    internal static class FormatHelper
    {
        /// <summary>
        /// The maximum number of characters allowed for padded values.
        /// </summary>
        internal const int MaximumPaddingLength = 16;

        /// <summary>
        /// Maximum number of digits in a (positive) long.
        /// </summary>
        internal const int MaximumInt64Length = 19;

        private static readonly string[] FixedNumberFormats = new[]
                                                              {
                                                                  "0", "00", "000", "0000", "00000", "000000", "0000000", "00000000", "000000000", "0000000000",
                                                                  "00000000000", "000000000000", "0000000000000", "00000000000000", "000000000000000",
                                                                  "0000000000000000"
                                                              };

        /// <summary>
        /// Formats the given value left padded with zeros.
        /// </summary>
        /// <remarks>
        /// Left pads with zeros the value into a field of <paramref name = "length" /> characters. If the value
        /// is longer than <paramref name = "length" />, the entire value is formatted. If the value is negative,
        /// it is preceded by "-" but this does not count against the length.
        /// </remarks>
        /// <param name="value">The value to format.</param>
        /// <param name="length">The length to fill.</param>
        /// <param name="outputBuffer">The output buffer to add the digits to.</param>
        /// <exception cref="FormatException">if too many characters are requested. <see cref="MaximumPaddingLength" />.</exception>
        internal static void LeftPad(int value, int length, StringBuilder outputBuffer)
        {
            if (length > MaximumPaddingLength)
            {
                throw new FormatException("Too many digits");
            }
            if (value < 0)
            {
                outputBuffer.Append('-');
                // Special case, as we can't use Math.Abs.
                if (value == int.MinValue)
                {
                    if (length > 10)
                    {
                        outputBuffer.Append("000000".Substring(16 - length));
                    }
                    outputBuffer.Append("2147483648");
                    return;
                }
                LeftPad(Math.Abs(value), length, outputBuffer);
                return;
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
        /// Formats the given value right padded with zeros.
        /// Note: current usage means this never has to cope with negative numbers.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="length">The length to fill.</param>
        /// <param name="scale">The scale of the value i.e. the number of significant digits is the range of the value.</param>
        /// <param name="outputBuffer">The output buffer to add the digits to.</param>
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
        /// Formats the given value right padded with zeros. The rightmost zeros are truncated.  If the entire value is truncated then
        /// the preceeding decimal separater is also removed.
        /// Note: current usage means this never has to cope with negative numbers.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="length">The length to fill.</param>
        /// <param name="scale">The scale of the value i.e. the number of significant digits is the range of the value.</param>
        /// <param name="decimalSeparator">The decimal separator for this culture.</param>
        /// <param name="outputBuffer">The output buffer to add the digits to.</param>
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
            else if (outputBuffer.Length > 0 && outputBuffer.ToString().EndsWith(decimalSeparator, StringComparison.CurrentCulture))
            {
                var decimalSeparatorLength = decimalSeparator.Length;
                outputBuffer.Remove(outputBuffer.Length - decimalSeparatorLength, decimalSeparatorLength);
            }
        }

        /// <summary>
        /// Formats the given value using the invariant culture, with no truncation or padding.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="outputBuffer">The output buffer to add the digits to.</param>
        internal static void FormatInvariant(long value, StringBuilder outputBuffer)
        {
            if (value == 0)
            {
                outputBuffer.Append('0');
                return;
            }
            if (value == long.MinValue)
            {
                outputBuffer.Append("-9223372036854775808");
                return;
            }
            if (value < 0)
            {
                outputBuffer.Append('-');
                FormatInvariant(-value, outputBuffer);
                return;
            }

            var digits = new char[MaximumInt64Length];
            int pos = MaximumInt64Length;
            do
            {
                digits[--pos] = "0123456789"[(int)(value % 10)];
                value /= 10;
            } while (value != 0);
            outputBuffer.Append(digits, pos, MaximumInt64Length - pos);
        }
    }
}