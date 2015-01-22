// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using NodaTime.Annotations;
using NodaTime.Calendars;
using NodaTime.Properties;

namespace NodaTime.Text
{
    /// <summary>
    /// The result of a parse operation.
    /// </summary>
    /// <typeparam name="T">The type which was parsed, such as a <see cref="LocalDateTime"/>.</typeparam>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class ParseResult<T>
    {
        private readonly T value;
        private readonly Func<Exception> exceptionProvider;
        internal bool ContinueAfterErrorWithMultipleFormats { get; }

        private ParseResult(Func<Exception> exceptionProvider, bool continueWithMultiple)
        {
            this.exceptionProvider = exceptionProvider;
            this.ContinueAfterErrorWithMultipleFormats = continueWithMultiple;
        }

        private ParseResult(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value from the parse operation if it was successful, or throws an exception indicating the parse failure
        /// otherwise.
        /// </summary>
        /// <remarks>
        /// This method is exactly equivalent to calling the <see cref="GetValueOrThrow"/> method, but is terser if the code is
        /// already clear that it will throw if the parse failed.
        /// </remarks>
        /// <value>The result of the parsing operation if it was successful.</value>
        public T Value => GetValueOrThrow();

        /// <summary>
        /// Gets an exception indicating the cause of the parse failure.
        /// </summary>
        /// <remarks>This property is typically used to wrap parse failures in higher level exceptions.</remarks>
        /// <value>The exception indicating the cause of the parse failure.</value>
        /// <exception cref="InvalidOperationException">The parse operation succeeded.</exception>
        public Exception Exception
        {
            get
            {
                if (exceptionProvider == null)
                {
                    throw new InvalidOperationException("Parse operation succeeded, so no exception is available");
                }
                return exceptionProvider();
            }
        }

        /// <summary>
        /// Gets the value from the parse operation if it was successful, or throws an exception indicating the parse failure
        /// otherwise.
        /// </summary>
        /// <remarks>
        /// This method is exactly equivalent to fetching the <see cref="Value"/> property, but more explicit in terms of throwing
        /// an exception on failure.
        /// </remarks>
        /// <returns>The result of the parsing operation if it was successful.</returns>
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
        /// <value>true if the parse operation was successful; otherwise false.</value>
        public bool Success => exceptionProvider == null;

        /// <summary>
        /// Converts this result to a new target type, either by executing the given projection
        /// for a success result, or propagating the exception provider for failure.
        /// </summary>
        internal ParseResult<TTarget> Convert<TTarget>(Func<T, TTarget> projection) =>
            Success ? ParseResult<TTarget>.ForValue(projection(Value))
                    : new ParseResult<TTarget>(exceptionProvider, ContinueAfterErrorWithMultipleFormats);

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
            return new ParseResult<TTarget>(exceptionProvider, ContinueAfterErrorWithMultipleFormats);
        }

        #region Factory methods and readonly static fields
        // TODO: Expose this and following method? What about continueOnMultiple?
        internal static ParseResult<T> ForValue(T value) => new ParseResult<T>(value);

        internal static ParseResult<T> ForException(Func<Exception> exceptionProvider) => new ParseResult<T>(exceptionProvider, false);

        internal static ParseResult<T> ForInvalidValue(ValueCursor cursor, string formatString, params object[] parameters)
        {
            return ForInvalidValue(() =>
            {
                // Format the message which is specific to the kind of parse error.
                string detailMessage = string.Format(CultureInfo.CurrentCulture, formatString, parameters);
                // Format the overall message, containing the parse error and the value itself.
                string overallMessage = string.Format(CultureInfo.CurrentCulture, Messages.Parse_UnparsableValue, detailMessage, cursor);
                return new UnparsableValueException(overallMessage);
            });
        }

        internal static ParseResult<T> ForInvalidValuePostParse(string text, string formatString, params object[] parameters)
        {
            return ForInvalidValue(() =>
            {
                // Format the message which is specific to the kind of parse error.
                string detailMessage = string.Format(CultureInfo.CurrentCulture, formatString, parameters);
                // Format the overall message, containing the parse error and the value itself.
                string overallMessage = string.Format(CultureInfo.CurrentCulture, Messages.Parse_UnparsableValuePostParse, detailMessage, text);
                return new UnparsableValueException(overallMessage);
            });
        }

        private static ParseResult<T> ForInvalidValue(Func<Exception> exceptionProvider) => new ParseResult<T>(exceptionProvider, true);

        internal static ParseResult<T> ArgumentNull(string parameter) => new ParseResult<T>(() => new ArgumentNullException(parameter), false);

        internal static ParseResult<T> PositiveSignInvalid(ValueCursor cursor) => ForInvalidValue(cursor, Messages.Parse_PositiveSignInvalid);

        internal static ParseResult<T> CannotParseValue(ValueCursor cursor, string format) => ForInvalidValue(cursor, Messages.Parse_CannotParseValue, typeof(T), format);

        // Special case: it's a fault with the value, but we still don't want to continue with multiple patterns.
        // Also, there's no point in including the text.
        internal static readonly ParseResult<T> ValueStringEmpty =
            new ParseResult<T>(() => new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, Messages.Parse_ValueStringEmpty)), false);

        internal static ParseResult<T> ExtraValueCharacters(ValueCursor cursor, string remainder) => ForInvalidValue(cursor, Messages.Parse_ExtraValueCharacters, remainder);

        internal static ParseResult<T> QuotedStringMismatch(ValueCursor cursor) => ForInvalidValue(cursor, Messages.Parse_QuotedStringMismatch);

        internal static ParseResult<T> EscapedCharacterMismatch(ValueCursor cursor, char patternCharacter) => ForInvalidValue(cursor, Messages.Parse_EscapedCharacterMismatch, patternCharacter);

        internal static ParseResult<T> EndOfString(ValueCursor cursor) => ForInvalidValue(cursor, Messages.Parse_EndOfString);

        internal static ParseResult<T> TimeSeparatorMismatch(ValueCursor cursor) => ForInvalidValue(cursor, Messages.Parse_TimeSeparatorMismatch);

        internal static ParseResult<T> DateSeparatorMismatch(ValueCursor cursor) => ForInvalidValue(cursor, Messages.Parse_DateSeparatorMismatch);

        internal static ParseResult<T> MissingNumber(ValueCursor cursor) => ForInvalidValue(cursor, Messages.Parse_MissingNumber);

        internal static ParseResult<T> UnexpectedNegative(ValueCursor cursor) => ForInvalidValue(cursor, Messages.Parse_UnexpectedNegative);

        /// <summary>
        /// This isn't really an issue with the value so much as the pattern... but the result is the same.
        /// </summary>
        internal static readonly ParseResult<T> FormatOnlyPattern =
            new ParseResult<T>(() => new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, Messages.Parse_FormatOnlyPattern)), true);

        internal static ParseResult<T> MismatchedNumber(ValueCursor cursor, string pattern) => ForInvalidValue(cursor, Messages.Parse_MismatchedNumber, pattern);

        internal static ParseResult<T> MismatchedCharacter(ValueCursor cursor, char patternCharacter) => ForInvalidValue(cursor, Messages.Parse_MismatchedCharacter, patternCharacter);

        internal static ParseResult<T> MismatchedText(ValueCursor cursor, char field) => ForInvalidValue(cursor, Messages.Parse_MismatchedText, field);

        internal static ParseResult<T> NoMatchingFormat(ValueCursor cursor) => ForInvalidValue(cursor, Messages.Parse_NoMatchingFormat);

        internal static ParseResult<T> ValueOutOfRange(ValueCursor cursor, object value) => ForInvalidValue(cursor, Messages.Parse_ValueOutOfRange, value, typeof(T));

        internal static ParseResult<T> MissingSign(ValueCursor cursor) => ForInvalidValue(cursor, Messages.Parse_MissingSign);

        internal static ParseResult<T> MissingAmPmDesignator(ValueCursor cursor) => ForInvalidValue(cursor, Messages.Parse_MissingAmPmDesignator);

        internal static ParseResult<T> NoMatchingCalendarSystem(ValueCursor cursor) => ForInvalidValue(cursor, Messages.Parse_NoMatchingCalendarSystem);

        internal static ParseResult<T> NoMatchingZoneId(ValueCursor cursor) => ForInvalidValue(cursor, Messages.Parse_NoMatchingZoneId);

        internal static ParseResult<T> InvalidHour24(string text) => ForInvalidValuePostParse(text, Messages.Parse_InvalidHour24);

        internal static ParseResult<T> FieldValueOutOfRange(ValueCursor cursor, int value, char field) =>
            ForInvalidValue(cursor, Messages.Parse_FieldValueOutOfRange, value, field, typeof(T));

        internal static ParseResult<T> FieldValueOutOfRangePostParse(string text, int value, char field) =>
            ForInvalidValuePostParse(text, Messages.Parse_FieldValueOutOfRange, value, field, typeof(T));

        /// <summary>
        /// Two fields (e.g. "hour of day" and "hour of half day") were mutually inconsistent.
        /// </summary>
        internal static ParseResult<T> InconsistentValues(string text, char field1, char field2) =>
            ForInvalidValuePostParse(text, Messages.Parse_InconsistentValues2, field1, field2, typeof(T));

        /// <summary>
        /// The month of year is inconsistent between the text and numeric specifications.
        /// We can't use InconsistentValues for this as the pattern character is the same in both cases.
        /// </summary>
        internal static ParseResult<T> InconsistentMonthValues(string text) => ForInvalidValuePostParse(text, Messages.Parse_InconsistentMonthTextValue);

        /// <summary>
        /// The day of month is inconsistent with the day of week value.
        /// We can't use InconsistentValues for this as the pattern character is the same in both cases.
        /// </summary>
        internal static ParseResult<T> InconsistentDayOfWeekTextValue(string text) => ForInvalidValuePostParse(text, Messages.Parse_InconsistentDayOfWeekTextValue);

        /// <summary>
        /// We'd expected to get to the end of the string now, but we haven't.
        /// </summary>
        internal static ParseResult<T> ExpectedEndOfString(ValueCursor cursor) => ForInvalidValue(cursor, Messages.Parse_ExpectedEndOfString);

        internal static ParseResult<T> YearOfEraOutOfRange(string text, int value, Era era, CalendarSystem calendar) =>
            ForInvalidValuePostParse(text, Messages.Parse_YearOfEraOutOfRange, value, era.Name, calendar.Name);

        internal static ParseResult<T> MonthOutOfRange(string text, int month, int year) => ForInvalidValuePostParse(text, Messages.Parse_MonthOutOfRange, month, year);

        internal static ParseResult<T> DayOfMonthOutOfRange(string text, int day, int month, int year) => ForInvalidValuePostParse(text, Messages.Parse_DayOfMonthOutOfRange, day, month, year);

        internal static ParseResult<T> InvalidOffset(string text) => ForInvalidValuePostParse(text, Messages.Parse_InvalidOffset);

        internal static ParseResult<T> SkippedLocalTime(string text) => ForInvalidValuePostParse(text, Messages.Parse_SkippedLocalTime);

        internal static ParseResult<T> AmbiguousLocalTime(string text) => ForInvalidValuePostParse(text, Messages.Parse_AmbiguousLocalTime);

        #endregion
    }
}
