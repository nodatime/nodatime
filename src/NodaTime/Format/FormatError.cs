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
using System.Runtime.Serialization;
using NodaTime.Properties;

namespace NodaTime.Format
{
    // TODO: Remove this class (including FormatError.UnparsableValueException) when the pattern and value parsing has been separated.
    internal static class FormatError
    {
        internal static FormatException PositiveSignInvalid()
        {
            return new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_PositiveSignInvalid));
        }

        internal static FormatException EscapeAtEndOfString()
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_EscapeAtEndOfString));
        }

        internal static FormatException MissingEndQuote(char closeQuote)
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_MissingEndQuote, closeQuote));
        }

        internal static FormatException RepeatCountExceeded(char patternCharacter, int maximumCount)
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_RepeatCountExceeded, patternCharacter, maximumCount));
        }

        internal static FormatException CannotParseValue(string value, Type type, string format)
        {
            return new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_CannotParseValue, value, type, format));
        }

        internal static FormatException DoubleAssigment(char patternCharacter)
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_DoubleAssignment, patternCharacter));
        }

        internal static FormatException ValueStringEmpty()
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_ValueStringEmpty));
        }

        internal static FormatException FormatStringEmpty()
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_FormatStringEmpty));
        }

        internal static FormatException FormatInvalid(string format)
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_FormatInvalid, format));
        }

        internal static FormatException EmptyFormatsArray()
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_EmptyFormatsArray));
        }

        internal static FormatException FormatElementInvalid()
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_FormatElementInvalid));
        }

        internal static FormatException ExtraValueCharacters(string remainder)
        {
            return new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_ExtraValueCharacters, remainder));
        }

        internal static FormatException PercentDoubled()
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_PercentDoubled));
        }

        internal static FormatException PercentAtEndOfString()
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_PercentAtEndOfString));
        }

        internal static FormatException QuotedStringMismatch()
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_QuotedStringMismatch));
        }

        internal static FormatException EscapedCharacterMismatch(char patternCharacter)
        {
            return new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_EscapedCharacterMismatch, patternCharacter));
        }

        internal static FormatException MissingDecimalSeparator()
        {
            return new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_MissingDecimalSeparator));
        }

        internal static FormatException TimeSeparatorMismatch()
        {
            return new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_TimeSeparatorMismatch));
        }

        internal static FormatException MismatchedNumber(string pattern)
        {
            return new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_MismatchedNumber, pattern));
        }

        internal static FormatException MismatchedSpace()
        {
            return new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_MismatchedSpace));
        }

        internal static FormatException MismatchedCharacter(char patternCharacter)
        {
            return new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_MismatchedCharacter, patternCharacter));
        }

        internal static FormatException UnknownStandardFormat(char patternCharacter, Type type)
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_UnknownStandardFormat, patternCharacter, type));
        }

        internal static FormatException PrecisionNotSupported(string pattern, Type type)
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_PrecisionNotSupported, pattern, type));
        }

        internal static FormatException StandardFormatWhitespace(string pattern, Type type)
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_StandardFormatWhitespace, pattern, type));
        }

        internal static FormatException Hour12PatternNotSupported(Type type)
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_Hour12PatternNotSupported, type.FullName));
        }

        internal static FormatException NoMatchingFormat()
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_NoMatchingFormat));
        }

        internal static FormatException ValueOutOfRange(object value, Type type)
        {
            return new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_ValueOutOfRange, value, type));
        }

        internal static FormatException MissingSign()
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_MissingSign));
        }

        internal static FormatException UnexpectedEndOfString(string pattern)
        {
            return new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_UnexpectedEndOfString, pattern));
        }
    }
}
