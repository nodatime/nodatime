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
using NodaTime.Properties;
#endregion

namespace NodaTime.Format
{
    public class ParseException : FormatException
    {
        public ParseException()
        {
            Kind = ParseFailureKind.None;
        }

        public ParseException(ParseFailureKind kind, string message)
            : base(message)
        {
            Kind = kind;
        }

        public ParseException(ParseFailureKind kind, string message, Exception innerException)
            : base(message, innerException)
        {
            Kind = kind;
        }

        public ParseFailureKind Kind { get; private set; }

        private static ParseException MakeException(ParseFailureKind kind, string message, params object[] parameters)
        {
            var text = string.Format(message, parameters);
            return new ParseException(kind, text);
        }

        private static ParseException MakeTypeException(ParseFailureKind kind, string message, params object[] parameters)
        {
            var text = string.Format(message, parameters);
            return new ParseTypeFormatException(kind, text);
        }

        internal static ParseException PositiveSignInvalid()
        {
            return MakeException(ParseFailureKind.PositiveSignInvalid, Resources.Parse_PositiveSignInvalid);
        }

        internal static ParseException EscapeAtEndOfString()
        {
            return MakeTypeException(ParseFailureKind.EscapeAtEndOfString, Resources.Parse_EscapeAtEndOfString);
        }

        internal static ParseException MissingEndQuote(char closeQuote)
        {
            return MakeTypeException(ParseFailureKind.MissingEndQuote, Resources.Parse_MissingEndQuote, closeQuote);
        }

        internal static ParseException RepeatCountExceeded(char patternCharacter, int maximumCount)
        {
            return MakeException(ParseFailureKind.RepeatCountExceeded, Resources.Parse_RepeatCountExceeded, patternCharacter, maximumCount);
        }

        internal static ParseException CannotParseValue(string value, Type type, string format)
        {
            return MakeException(ParseFailureKind.CannotParseValue, Resources.Parse_CannotParseValue, value, type.FullName, format);
        }

        internal static ParseException DoubleAssigment(char patternCharacter)
        {
            return MakeTypeException(ParseFailureKind.DoubleAssigment, Resources.Parse_DoubleAssignment, patternCharacter);
        }

        internal static ParseException ValueStringEmpty()
        {
            return MakeTypeException(ParseFailureKind.ValueStringEmpty, Resources.Parse_ValueStringEmpty);
        }

        internal static ParseException FormatStringEmpty()
        {
            return MakeTypeException(ParseFailureKind.FormatStringEmpty, Resources.Parse_FormatStringEmpty);
        }

        internal static ParseException FormatInvalid(string format)
        {
            return MakeTypeException(ParseFailureKind.FormatInvalid, Resources.Parse_FormatInvalid, format);
        }

        internal static ParseException EmptyFormatsArray()
        {
            return MakeTypeException(ParseFailureKind.EmptyFormatsArray, Resources.Parse_EmptyFormatsArray);
        }

        internal static ParseException FormatElementInvalid()
        {
            return MakeTypeException(ParseFailureKind.FormatElementInvalid, Resources.Parse_FormatElementInvalid);
        }

        internal static ParseException ExtraValueCharacters(string remainder)
        {
            return MakeException(ParseFailureKind.ExtraValueCharacters, Resources.Parse_ExtraValueCharacters, remainder);
        }

        internal static ParseException PercentDoubled()
        {
            return MakeTypeException(ParseFailureKind.PercentDoubled, Resources.Parse_PercentDoubled);
        }

        internal static ParseException PercentAtEndOfString()
        {
            return MakeTypeException(ParseFailureKind.PercentAtEndOfString, Resources.Parse_PercentAtEndOfString);
        }

        internal static ParseException QuotedStringMismatch()
        {
            return MakeTypeException(ParseFailureKind.QuotedStringMismatch, Resources.Parse_QuotedStringMismatch);
        }

        internal static ParseException EscapedCharacterMismatch(char patternCharacter)
        {
            return MakeException(ParseFailureKind.EscapedCharacterMismatch, Resources.Parse_EscapedCharacterMismatch, patternCharacter);
        }

        internal static ParseException MissingDecimalSeparator()
        {
            return MakeException(ParseFailureKind.MissingDecimalSeparator, Resources.Parse_MissingDecimalSeparator);
        }

        internal static ParseException TimeSeparatorMismatch()
        {
            return MakeException(ParseFailureKind.TimeSeparatorMismatch, Resources.Parse_TimeSeparatorMismatch);
        }

        internal static ParseException MismatchedNumber(string pattern)
        {
            return MakeException(ParseFailureKind.MismatchedNumber, Resources.Parse_MismatchedNumber, pattern);
        }

        internal static ParseException MismatchedSpace()
        {
            return MakeException(ParseFailureKind.MismatchedSpace, Resources.Parse_MismatchedSpace);
        }

        internal static ParseException MismatchedCharacter(char patternCharacter)
        {
            return MakeException(ParseFailureKind.MismatchedCharacter, Resources.Parse_MismatchedCharacter, patternCharacter);
        }

        internal static ParseException UnknownStandardFormat(char patternCharacter, Type type)
        {
            return MakeTypeException(ParseFailureKind.UnknownStandardFormat, Resources.Parse_UnknownStandardFormat, patternCharacter, type.FullName);
        }

        internal static ParseException PrecisionNotSupported(string pattern, Type type)
        {
            return MakeTypeException(ParseFailureKind.PrecisionNotSupported, Resources.Parse_PrecisionNotSupported, pattern, type.FullName);
        }

        internal static ParseException StandardFormatWhitespace(string pattern, Type type)
        {
            return MakeTypeException(ParseFailureKind.StandardFormatWhitespace, Resources.Parse_StandardFormatWhitespace, pattern, type.FullName);
        }

        internal static ParseException Hour12PatternNotSupported(Type type)
        {
            return MakeTypeException(ParseFailureKind.Hour12PatternNotSupported, Resources.Parse_Hour12PatternNotSupported, type.FullName);
        }

        internal static ParseException NoMatchingFormat()
        {
            return MakeException(ParseFailureKind.NoMatchingFormat, Resources.Parse_NoMatchingFormat);
        }

        internal static ParseException ValueOutOfRange(object value, Type type)
        {
            return MakeException(ParseFailureKind.ValueOutOfRange, Resources.Parse_ValueOutOfRange, value, type.FullName);
        }

        internal static ParseException MissingSign()
        {
            return MakeTypeException(ParseFailureKind.MissingSign, Resources.Parse_MissingSign);
        }

        #region Nested type: ParseTypeFormatException
        internal class ParseTypeFormatException : ParseException
        {
            public ParseTypeFormatException()
            {
            }

            public ParseTypeFormatException(ParseFailureKind kind, string message)
                : base(kind, message)
            {
            }
        }
        #endregion
    }
}
