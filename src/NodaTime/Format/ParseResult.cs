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
using NodaTime.Properties;

namespace NodaTime.Format
{
    internal class ParseResult<T>
    {
        private readonly T value;
        private readonly NodaFunc<Exception> exceptionProvider;
        private readonly bool continueWithMultiple;

        private ParseResult(NodaFunc<Exception> exceptionProvider, bool continueWithMultiple)
        {
            this.exceptionProvider = exceptionProvider;
            this.continueWithMultiple = continueWithMultiple;
        }

        private ParseResult(T value)
        {
            this.value = value;
        }

        internal T GetResultOrThrow()
        {
            if (exceptionProvider == null)
            {
                return value;
            }
            throw exceptionProvider();
        }

        /// <summary>
        /// Returns the success value, and sets the out parameter to either
        /// the specified failure value of T or the successful parse result value.
        /// </summary>
        internal bool TryGetResult(T failureValue, out T result)
        {
            bool success = exceptionProvider == null;
            result = success ? value : failureValue;
            return success;
        }

        internal static ParseResult<T> ForValue(T value)
        {
            return new ParseResult<T>(value);
        }

        internal bool Success { get { return exceptionProvider == null; } }

        internal bool ContinueAfterErrorWithMultipleFormats { get { return continueWithMultiple; } }

        #region Factory methods and readonly static fields
        internal static ParseResult<T> ForInvalidFormat(string formatString, params object[] parameters)
        {
            return ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, formatString, parameters)));
        }

        internal static ParseResult<T> ForInvalidFormat(NodaFunc<Exception> exceptionProvider)
        {
            return new ParseResult<T>(exceptionProvider, false);
        }

        internal static ParseResult<T> ForInvalidValue(string formatString, params object[] parameters)
        {
            return ForInvalidValue(() => new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, formatString, parameters)));
        }

        private static ParseResult<T> ForInvalidValue(NodaFunc<Exception> exceptionProvider)
        {
            return new ParseResult<T>(exceptionProvider, true);
        }

        internal static ParseResult<T> ArgumentNull(string parameter)
        {
            return new ParseResult<T>(() => new ArgumentNullException(parameter), false);
        }

        internal static readonly ParseResult<T> PositiveSignInvalid = ForInvalidValue(Resources.Parse_PositiveSignInvalid);

        internal static ParseResult<T> EscapeAtEndOfString = ForInvalidFormat(Resources.Parse_EscapeAtEndOfString);

        internal static ParseResult<T> MissingEndQuote(char closeQuote)
        {
            return ForInvalidFormat(Resources.Parse_MissingEndQuote, closeQuote);
        }

        internal static ParseResult<T> RepeatCountExceeded(char patternCharacter, int maximumCount)
        {
            return ForInvalidFormat(Resources.Parse_RepeatCountExceeded, patternCharacter, maximumCount);
        }

        internal static ParseResult<T> CannotParseValue(string value, Type type, string format)
        {
            return ForInvalidValue(Resources.Parse_CannotParseValue, value, type, format);
        }

        internal static ParseResult<T> DoubleAssigment(char patternCharacter)
        {
            return ForInvalidFormat(Resources.Parse_DoubleAssignment, patternCharacter);
        }

        internal static readonly ParseResult<T> ValueStringEmpty = ForInvalidFormat(Resources.Parse_ValueStringEmpty);

        internal static readonly ParseResult<T> FormatStringEmpty = ForInvalidFormat(Resources.Parse_FormatStringEmpty);

        internal static ParseResult<T> FormatInvalid(string format)
        {
            return ForInvalidFormat(Resources.Parse_FormatInvalid, format);
        }

        internal static readonly ParseResult<T> EmptyFormatsArray = ForInvalidFormat(Resources.Parse_EmptyFormatsArray);

        internal static readonly ParseResult<T> FormatElementInvalid = ForInvalidFormat(Resources.Parse_FormatElementInvalid);

        internal static ParseResult<T> ExtraValueCharacters(string remainder)
        {
            return ForInvalidValue(Resources.Parse_ExtraValueCharacters, remainder);
        }

        internal static readonly ParseResult<T> PercentDoubled = ForInvalidFormat(Resources.Parse_PercentDoubled);

        internal static readonly ParseResult<T> PercentAtEndOfString = ForInvalidFormat(Resources.Parse_PercentAtEndOfString);

        internal static readonly ParseResult<T> QuotedStringMismatch = ForInvalidFormat(Resources.Parse_QuotedStringMismatch);

        internal static ParseResult<T> EscapedCharacterMismatch(char patternCharacter)
        {
            return ForInvalidValue(Resources.Parse_EscapedCharacterMismatch, patternCharacter);
        }

        internal static ParseResult<T> MissingDecimalSeparator = ForInvalidValue(Resources.Parse_MissingDecimalSeparator);

        internal static ParseResult<T> TimeSeparatorMismatch = ForInvalidValue(Resources.Parse_TimeSeparatorMismatch);

        internal static ParseResult<T> MismatchedNumber(string pattern)
        {
            return ForInvalidValue(Resources.Parse_MismatchedNumber, pattern);
        }

        internal static ParseResult<T> MismatchedSpace = ForInvalidValue(Resources.Parse_MismatchedSpace);

        internal static ParseResult<T> MismatchedCharacter(char patternCharacter)
        {
            return ForInvalidValue(Resources.Parse_MismatchedCharacter, patternCharacter);
        }

        internal static ParseResult<T> UnknownStandardFormat(char patternCharacter, Type type)
        {
            return ForInvalidFormat(Resources.Parse_UnknownStandardFormat, patternCharacter, type);
        }

        internal static ParseResult<T> PrecisionNotSupported(string pattern, Type type)
        {
            return ForInvalidFormat(Resources.Parse_PrecisionNotSupported, pattern, type);
        }

        internal static ParseResult<T> StandardFormatWhitespace(string pattern, Type type)
        {
            return ForInvalidFormat(Resources.Parse_StandardFormatWhitespace, pattern, type);
        }

        internal static ParseResult<T> Hour12PatternNotSupported(Type type)
        {
            return ForInvalidFormat(Resources.Parse_Hour12PatternNotSupported, type.FullName);
        }

        internal static readonly ParseResult<T> NoMatchingFormat = ForInvalidFormat(Resources.Parse_NoMatchingFormat);

        internal static ParseResult<T> ValueOutOfRange(object value, Type type)
        {
            return ForInvalidValue(Resources.Parse_ValueOutOfRange, value, type);
        }

        internal static readonly ParseResult<T> MissingSign = ForInvalidFormat(Resources.Parse_MissingSign);

        internal static ParseResult<T> UnexpectedEndOfString(string pattern)
        {
            return ForInvalidFormat(Resources.Parse_UnexpectedEndOfString, pattern);
        }
        #endregion
    }
}
