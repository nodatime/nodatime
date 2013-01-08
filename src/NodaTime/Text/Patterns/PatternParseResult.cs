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
    internal sealed class PatternParseResult<T>
    {
        private readonly IPattern<T> value;
        private readonly NodaFunc<Exception> exceptionProvider;

        private PatternParseResult(NodaFunc<Exception> exceptionProvider)
        {
            this.exceptionProvider = exceptionProvider;
        }

        private PatternParseResult(IPattern<T> value)
        {
            this.value = value;
        }

        internal ParseResult<T> ToParseResult()
        {
            if (exceptionProvider == null)
            {
                throw new InvalidOperationException("ToParseResult called on successful pattern parse");
            }
            return ParseResult<T>.ForException(exceptionProvider);
        }

        internal IPattern<T> GetResultOrThrow()
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
        internal bool TryGetResult(out IPattern<T> result)
        {
            bool success = exceptionProvider == null;
            result = success ? value : null;
            return success;
        }

        internal static PatternParseResult<T> ForValue(IPattern<T> value)
        {
            return new PatternParseResult<T>(value);
        }

        internal bool Success { get { return exceptionProvider == null; } }

        /// <summary>
        /// Returns a new result with the target type. This result must be a failure.
        /// </summary>
        internal PatternParseResult<TTarget> WithResultType<TTarget>()
        {
            if (Success)
            {
                throw new InvalidOperationException("Can't change type of a success result");
            }
            return new PatternParseResult<TTarget>(exceptionProvider);
        }

        #region Factory methods and readonly static fields
        internal static PatternParseResult<T> ForInvalidFormat(string formatString, params object[] parameters)
        {
            return ForInvalidFormat(() => new InvalidPatternException(string.Format(CultureInfo.CurrentCulture, formatString, parameters)));
        }

        internal static PatternParseResult<T> ForInvalidFormat(NodaFunc<Exception> exceptionProvider)
        {
            return new PatternParseResult<T>(exceptionProvider);
        }

        internal static PatternParseResult<T> ArgumentNull(string parameter)
        {
            return new PatternParseResult<T>(() => new ArgumentNullException(parameter));
        }

        internal static PatternParseResult<T> EscapeAtEndOfString = ForInvalidFormat(Messages.Parse_EscapeAtEndOfString);

        internal static PatternParseResult<T> MissingEndQuote(char closeQuote)
        {
            return ForInvalidFormat(Messages.Parse_MissingEndQuote, closeQuote);
        }

        internal static PatternParseResult<T> RepeatCountExceeded(char patternCharacter, int maximumCount)
        {
            return ForInvalidFormat(Messages.Parse_RepeatCountExceeded, patternCharacter, maximumCount);
        }

        internal static PatternParseResult<T> DoubleAssigment(char patternCharacter)
        {
            return ForInvalidFormat(Messages.Parse_DoubleAssignment, patternCharacter);
        }

        internal static readonly PatternParseResult<T> FormatStringEmpty = ForInvalidFormat(Messages.Parse_FormatStringEmpty);

        internal static PatternParseResult<T> FormatInvalid(string format)
        {
            return ForInvalidFormat(Messages.Parse_FormatInvalid, format);
        }

        internal static readonly PatternParseResult<T> EmptyFormatsArray = ForInvalidFormat(Messages.Parse_EmptyFormatsArray);
        internal static readonly PatternParseResult<T> FormatElementInvalid = ForInvalidFormat(Messages.Parse_FormatElementInvalid);
        internal static readonly PatternParseResult<T> PercentDoubled = ForInvalidFormat(Messages.Parse_PercentDoubled);
        internal static readonly PatternParseResult<T> PercentAtEndOfString = ForInvalidFormat(Messages.Parse_PercentAtEndOfString);
        internal static readonly PatternParseResult<T> QuotedStringMismatch = ForInvalidFormat(Messages.Parse_QuotedStringMismatch);
        internal static readonly PatternParseResult<T> CalendarAndEra = ForInvalidFormat(Messages.Parse_CalendarAndEra);

        internal static PatternParseResult<T> UnknownStandardFormat(char patternCharacter)
        {
            return ForInvalidFormat(Messages.Parse_UnknownStandardFormat, patternCharacter, typeof(T));
        }

        internal static PatternParseResult<T> PrecisionNotSupported(string pattern)
        {
            return ForInvalidFormat(Messages.Parse_PrecisionNotSupported, pattern, typeof(T));
        }

        internal static PatternParseResult<T> StandardFormatWhitespace(string pattern)
        {
            return ForInvalidFormat(Messages.Parse_StandardFormatWhitespace, pattern, typeof(T));
        }

        internal static readonly PatternParseResult<T> Hour12PatternNotSupported = ForInvalidFormat(Messages.Parse_Hour12PatternNotSupported, typeof(T).FullName);

        internal static readonly PatternParseResult<T> NoMatchingFormat = ForInvalidFormat(Messages.Parse_NoMatchingFormat);

        internal static readonly PatternParseResult<T> MissingSign = ForInvalidFormat(Messages.Parse_MissingSign);

        internal static readonly PatternParseResult<T> EraDesignatorWithoutYearOfEra = ForInvalidFormat(Messages.Parse_EraWithoutYearOfEra);

        internal static PatternParseResult<T> UnexpectedEndOfString(string pattern)
        {
            return ForInvalidFormat(Messages.Parse_UnexpectedEndOfString, pattern);
        }

        internal static PatternParseResult<T> RepeatedFieldInPattern(char field)
        {
            return ForInvalidFormat(Messages.Parse_RepeatedFieldInPattern, field);
        }
        #endregion
    }
}
