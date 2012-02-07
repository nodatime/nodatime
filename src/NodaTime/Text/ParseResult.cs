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

namespace NodaTime.Text
{
    /// <summary>
    /// The result of a parse operation. 
    /// </summary>
    /// <typeparam name="T">The type which was parsed, such as a <see cref="LocalDateTime"/>.</typeparam>
    public sealed class ParseResult<T>
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

        /// <summary>
        /// Returns the value from the parse operation if it was successful, or throws an exception indicating the parse failure
        /// otherwise.
        /// </summary>
        /// <remarks>
        /// This method is exactly equivalent to calling the <see cref="GetValueOrThrow"/> method, but is terser if the code is
        /// already clear that it will throw if the parse failed.
        /// </remarks>
        /// <returns>The result of the parsing operation if it was successsful.</returns>
        public T Value { get { return GetValueOrThrow(); } }

        /// <summary>
        /// Returns the value from the parse operation if it was successful, or throws an exception indicating the parse failure
        /// otherwise.
        /// </summary>
        /// <remarks>
        /// This method is exactly equivalent to fetching the <see cref="Value"/> property, but more explicit in terms of throwing
        /// an exception on failure.
        /// </remarks>
        /// <returns>The result of the parsing operation if it was successsful.</returns>
        public T GetValueOrThrow()
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
        /// <param name="failureValue">The "default" value to set in <paramref name="result"/> if parsing failed.</param>
        /// <param name="result">The parameter to store the parsed value in on success.</param>
        /// <returns>True if this parse result was successful, or false otherwise.</returns>
        public bool TryGetValue(T failureValue, out T result)
        {
            bool success = exceptionProvider == null;
            result = success ? value : failureValue;
            return success;
        }

        /// <summary>
        /// Indicates whether the parse operation was successful.
        /// </summary>
        /// <remarks>
        /// This returns True if and only if fetching the value with the <see cref="Value"/> property will return with no exception.
        /// </remarks>
        public bool Success { get { return exceptionProvider == null; } }

        internal bool ContinueAfterErrorWithMultipleFormats { get { return continueWithMultiple; } }

        /// <summary>
        /// Converts this result to a new target type, either by executing the given projection
        /// for a success result, or propagating the exception provider for failure.
        /// </summary>
        internal ParseResult<TTarget> Convert<TTarget>(NodaFunc<T, TTarget> projection)
        {
            return Success ? ParseResult<TTarget>.ForValue(projection(Value))
                : new ParseResult<TTarget>(exceptionProvider, continueWithMultiple);
        }

        /// <summary>
        /// Converts this result to a new target type by propagating the exception provider.
        /// This parse result must already be an error result.
        /// </summary>
        internal ParseResult<TTarget> ConvertError<TTarget>()
        {
            if (Success)
            {
                throw new InvalidOperationException("ConvertError should not be called on a successful parse result");
            }
            return new ParseResult<TTarget>(exceptionProvider, continueWithMultiple);
        }

        #region Factory methods and readonly static fields
        // TODO: Expose this and following method?
        internal static ParseResult<T> ForValue(T value)
        {
            return new ParseResult<T>(value);
        }

        internal static ParseResult<T> ForException(NodaFunc<Exception> exceptionProvider)
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

        internal static readonly ParseResult<T> PositiveSignInvalid = ForInvalidValue(Messages.Parse_PositiveSignInvalid);

        internal static ParseResult<T> CannotParseValue(string value, string format)
        {
            return ForInvalidValue(Messages.Parse_CannotParseValue, value, typeof(T), format);
        }

        // Special case: it's a fault with the value, but we still don't want to continue with multiple patterns.
        internal static readonly ParseResult<T> ValueStringEmpty =
            new ParseResult<T>(() => new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, Messages.Parse_ValueStringEmpty)), false);

        internal static ParseResult<T> ExtraValueCharacters(string remainder)
        {
            return ForInvalidValue(Messages.Parse_ExtraValueCharacters, remainder);
        }

        internal static readonly ParseResult<T> QuotedStringMismatch = ForInvalidValue(Messages.Parse_QuotedStringMismatch);

        internal static ParseResult<T> EscapedCharacterMismatch(char patternCharacter)
        {
            return ForInvalidValue(Messages.Parse_EscapedCharacterMismatch, patternCharacter);
        }

        internal static ParseResult<T> TimeSeparatorMismatch = ForInvalidValue(Messages.Parse_TimeSeparatorMismatch);
        internal static ParseResult<T> DateSeparatorMismatch = ForInvalidValue(Messages.Parse_DateSeparatorMismatch);

        internal static ParseResult<T> MismatchedNumber(string pattern)
        {
            return ForInvalidValue(Messages.Parse_MismatchedNumber, pattern);
        }
        
        internal static ParseResult<T> MismatchedCharacter(char patternCharacter)
        {
            return ForInvalidValue(Messages.Parse_MismatchedCharacter, patternCharacter);
        }

        internal static ParseResult<T> MismatchedText(char field)
        {
            return ForInvalidValue(Messages.Parse_MismatchedText, field);
        }

        internal static readonly ParseResult<T> NoMatchingFormat = ForInvalidValue(Messages.Parse_NoMatchingFormat);

        internal static ParseResult<T> ValueOutOfRange(object value)
        {
            return ForInvalidValue(Messages.Parse_ValueOutOfRange, value, typeof(T));
        }

        internal static readonly ParseResult<T> MissingSign = ForInvalidValue(Messages.Parse_MissingSign);
        internal static readonly ParseResult<T> MissingAmPmDesignator = ForInvalidValue(Messages.Parse_MissingAmPmDesignator);

        internal static ParseResult<T> FieldValueOutOfRange(int value, char field)
        {
            return ForInvalidValue(Messages.Parse_FieldValueOutOfRange, value, field, typeof(T));
        }

        /// <summary>
        /// Two fields (e.g. "hour of day" and "hour of half day") were mutually inconsistent.
        /// </summary>
        internal static ParseResult<T> InconsistentValues(char field1, char field2)
        {
            return ForInvalidValue(Messages.Parse_InconsistentValues2, field1, field2, typeof(T));
        }

        /// <summary>
        /// The month of year is inconsistent between the text and numeric specifications.
        /// We can't use InconsistentValues for this as the pattern character is the same in both cases.
        /// </summary>
        internal static readonly ParseResult<T> InconsistentMonthValues = ForInvalidValue(Messages.Parse_InconsistentMonthTextValue);

        /// <summary>
        /// The day of month is inconsistent with the day of week value.
        /// We can't use InconsistentValues for this as the pattern character is the same in both cases.
        /// </summary>
        internal static readonly ParseResult<T> InconsistentDayOfWeekTextValue = ForInvalidValue(Messages.Parse_InconsistentDayOfWeekTextValue);

        internal static ParseResult<T> YearOfEraOutOfRange(int value, int eraIndex, CalendarSystem calendar)
        {
            return ForInvalidValue(Messages.Parse_YearOfEraOutOfRange, value, calendar.Eras[eraIndex].Name, calendar.Name);
        }

        internal static ParseResult<T> MonthOutOfRange(int month, int year)
        {
            return ForInvalidValue(Messages.Parse_MonthOutOfRange, month, year);
        }

        internal static ParseResult<T> DayOfMonthOutOfRange(int day, int month, int year)
        {
            return ForInvalidValue(Messages.Parse_DayOfMonthOutOfRange, day, month, year);
        }
        #endregion
    }
}
