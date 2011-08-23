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

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Indicates the results of parsing a pattern, including failure.
    /// </summary>
    internal class PatternParseResult<T>
    {
        private readonly IParsedPattern<T> value;
        private readonly NodaFunc<Exception> exceptionProvider;
        private readonly bool continueWithMultiple;

        private PatternParseResult(NodaFunc<Exception> exceptionProvider, bool continueWithMultiple)
        {
            this.exceptionProvider = exceptionProvider;
            this.continueWithMultiple = continueWithMultiple;
        }

        private PatternParseResult(IParsedPattern<T> value)
        {
            this.value = value;
        }

        internal ParseResult<T> ToParseResult()
        {
            if (exceptionProvider == null)
            {
                throw new InvalidOperationException("ToParseResult called on successful pattern parse");
            }
            return ParseResult<T>.ForInvalidFormat(exceptionProvider);
        }

        internal IParsedPattern<T> GetResultOrThrow()
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
        internal bool TryGetResult(out IParsedPattern<T> result)
        {
            bool success = exceptionProvider == null;
            result = success ? value : null;
            return success;
        }

        internal static PatternParseResult<T> ForValue(IParsedPattern<T> value)
        {
            return new PatternParseResult<T>(value);
        }

        internal bool Success { get { return exceptionProvider == null; } }

        internal bool ContinueAfterErrorWithMultipleFormats { get { return continueWithMultiple; } }

        #region Factory methods and readonly static fields
        internal static PatternParseResult<T> ForInvalidFormat(string formatString, params object[] parameters)
        {
            return ForInvalidFormat(() => new InvalidPatternException(string.Format(CultureInfo.CurrentCulture, formatString, parameters)));
        }

        internal static PatternParseResult<T> ForInvalidFormat(NodaFunc<Exception> exceptionProvider)
        {
            return new PatternParseResult<T>(exceptionProvider, false);
        }

        internal static PatternParseResult<T> ArgumentNull(string parameter)
        {
            return new PatternParseResult<T>(() => new ArgumentNullException(parameter), false);
        }

        internal static PatternParseResult<T> EscapeAtEndOfString = ForInvalidFormat(Resources.Parse_EscapeAtEndOfString);

        internal static PatternParseResult<T> MissingEndQuote(char closeQuote)
        {
            return ForInvalidFormat(Resources.Parse_MissingEndQuote, closeQuote);
        }

        internal static PatternParseResult<T> RepeatCountExceeded(char patternCharacter, int maximumCount)
        {
            return ForInvalidFormat(Resources.Parse_RepeatCountExceeded, patternCharacter, maximumCount);
        }

        internal static PatternParseResult<T> DoubleAssigment(char patternCharacter)
        {
            return ForInvalidFormat(Resources.Parse_DoubleAssignment, patternCharacter);
        }

        internal static readonly PatternParseResult<T> FormatStringEmpty = ForInvalidFormat(Resources.Parse_FormatStringEmpty);

        internal static PatternParseResult<T> FormatInvalid(string format)
        {
            return ForInvalidFormat(Resources.Parse_FormatInvalid, format);
        }

        internal static readonly PatternParseResult<T> EmptyFormatsArray = ForInvalidFormat(Resources.Parse_EmptyFormatsArray);

        internal static readonly PatternParseResult<T> FormatElementInvalid = ForInvalidFormat(Resources.Parse_FormatElementInvalid);

        internal static readonly PatternParseResult<T> PercentDoubled = ForInvalidFormat(Resources.Parse_PercentDoubled);

        internal static readonly PatternParseResult<T> PercentAtEndOfString = ForInvalidFormat(Resources.Parse_PercentAtEndOfString);

        internal static readonly PatternParseResult<T> QuotedStringMismatch = ForInvalidFormat(Resources.Parse_QuotedStringMismatch);

        internal static PatternParseResult<T> UnknownStandardFormat(char patternCharacter, Type type)
        {
            return ForInvalidFormat(Resources.Parse_UnknownStandardFormat, patternCharacter, type);
        }

        internal static PatternParseResult<T> PrecisionNotSupported(string pattern, Type type)
        {
            return ForInvalidFormat(Resources.Parse_PrecisionNotSupported, pattern, type);
        }

        internal static PatternParseResult<T> StandardFormatWhitespace(string pattern, Type type)
        {
            return ForInvalidFormat(Resources.Parse_StandardFormatWhitespace, pattern, type);
        }

        internal static readonly PatternParseResult<T> Hour12PatternNotSupported = ForInvalidFormat(Resources.Parse_Hour12PatternNotSupported, typeof(T).FullName);

        internal static readonly PatternParseResult<T> NoMatchingFormat = ForInvalidFormat(Resources.Parse_NoMatchingFormat);

        internal static readonly PatternParseResult<T> MissingSign = ForInvalidFormat(Resources.Parse_MissingSign);

        internal static PatternParseResult<T> UnexpectedEndOfString(string pattern)
        {
            return ForInvalidFormat(Resources.Parse_UnexpectedEndOfString, pattern);
        }
        #endregion
    }
}
