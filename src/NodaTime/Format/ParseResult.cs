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
        /// the default value of T or the successful parse result value.
        /// </summary>
        internal bool TryGetResult(out T result)
        {
            result = value;
            return exceptionProvider == null;
        }

        internal static ParseResult<T> ForValue(T value)
        {
            return new ParseResult<T>(value);
        }

        internal bool Success { get { return exceptionProvider == null; } }

        internal bool ContinueAfterErrorWithMultipleFormats { get { return continueWithMultiple; } }

        #region Factory methods and readonly static fields
        internal static ParseResult<T> ForInvalidFormat(NodaFunc<Exception> exceptionProvider)
        {
            return new ParseResult<T>(exceptionProvider, false);
        }

        private static ParseResult<T> ForInvalidValue(NodaFunc<Exception> exceptionProvider)
        {
            return new ParseResult<T>(exceptionProvider, true);
        }

        internal static ParseResult<T> ArgumentNull(string parameter)
        {
            return new ParseResult<T>(() => new ArgumentNullException(parameter), false);
        }

        internal static readonly ParseResult<T> PositiveSignInvalid =
            ForInvalidValue(() => new FormatError.FormatValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_PositiveSignInvalid)));

        internal static ParseResult<T> EscapeAtEndOfString =
            ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_EscapeAtEndOfString)));

        internal static ParseResult<T> MissingEndQuote(char closeQuote)
        {
            return ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_MissingEndQuote, closeQuote)));
        }

        internal static ParseResult<T> RepeatCountExceeded(char patternCharacter, int maximumCount)
        {
            return
                ForInvalidValue(
                    () =>
                    new FormatError.FormatValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_RepeatCountExceeded, patternCharacter,
                                                                       maximumCount)));
        }

        internal static ParseResult<T> CannotParseValue(string value, Type type, string format)
        {
            return
                ForInvalidValue(
                    () => new FormatError.FormatValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_CannotParseValue, value, type, format)));
        }

        internal static ParseResult<T> DoubleAssigment(char patternCharacter)
        {
            return ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_DoubleAssignment, patternCharacter)));
        }

        internal static readonly ParseResult<T> ValueStringEmpty =
            ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_ValueStringEmpty)));

        internal static readonly ParseResult<T> FormatStringEmpty =
            ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_FormatStringEmpty)));

        internal static ParseResult<T> FormatInvalid(string format)
        {
            return ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_FormatInvalid, format)));
        }

        internal static readonly ParseResult<T> EmptyFormatsArray =
            ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_EmptyFormatsArray)));

        internal static readonly ParseResult<T> FormatElementInvalid =
            ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_FormatElementInvalid)));

        internal static ParseResult<T> ExtraValueCharacters(string remainder)
        {
            return
                ForInvalidValue(
                    () => new FormatError.FormatValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_ExtraValueCharacters, remainder)));
        }

        internal static readonly ParseResult<T> PercentDoubled =
            ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_PercentDoubled)));

        internal static readonly ParseResult<T> PercentAtEndOfString =
            ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_PercentAtEndOfString)));

        internal static readonly ParseResult<T> QuotedStringMismatch =
            ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_QuotedStringMismatch)));

        internal static ParseResult<T> EscapedCharacterMismatch(char patternCharacter)
        {
            return
                ForInvalidValue(
                    () =>
                    new FormatError.FormatValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_EscapedCharacterMismatch, patternCharacter)));
        }

        internal static ParseResult<T> MissingDecimalSeparator =
            ForInvalidValue(() => new FormatError.FormatValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_MissingDecimalSeparator)));

        internal static ParseResult<T> TimeSeparatorMismatch =
            ForInvalidValue(() => new FormatError.FormatValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_TimeSeparatorMismatch)));

        internal static ParseResult<T> MismatchedNumber(string pattern)
        {
            return
                ForInvalidValue(() => new FormatError.FormatValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_MismatchedNumber, pattern)));
        }

        internal static ParseResult<T> MismatchedSpace =
            ForInvalidValue(() => new FormatError.FormatValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_MismatchedSpace)));

        internal static ParseResult<T> MismatchedCharacter(char patternCharacter)
        {
            return
                ForInvalidValue(
                    () => new FormatError.FormatValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_MismatchedCharacter, patternCharacter)));
        }

        internal static ParseResult<T> UnknownStandardFormat(char patternCharacter, Type type)
        {
            return
                ForInvalidFormat(
                    () => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_UnknownStandardFormat, patternCharacter, type)));
        }

        internal static ParseResult<T> PrecisionNotSupported(string pattern, Type type)
        {
            return ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_PrecisionNotSupported, pattern, type)));
        }

        internal static ParseResult<T> StandardFormatWhitespace(string pattern, Type type)
        {
            return
                ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_StandardFormatWhitespace, pattern, type)));
        }

        internal static ParseResult<T> Hour12PatternNotSupported(Type type)
        {
            return
                ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_Hour12PatternNotSupported, type.FullName)));
        }

        internal static readonly ParseResult<T> NoMatchingFormat =
            ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_NoMatchingFormat)));

        internal static ParseResult<T> ValueOutOfRange(object value, Type type)
        {
            return
                ForInvalidValue(
                    () => new FormatError.FormatValueException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_ValueOutOfRange, value, type)));
        }

        internal static readonly ParseResult<T> MissingSign =
            ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_MissingSign)));

        internal static ParseResult<T> UnexpectedEndOfString(string pattern)
        {
            return ForInvalidFormat(() => new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Parse_UnexpectedEndOfString, pattern)));
        }
        #endregion
    }
}
