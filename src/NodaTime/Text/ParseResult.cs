// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using NodaTime.Annotations;
using NodaTime.Calendars;
using JetBrains.Annotations;
using NodaTime.Utility;

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
        [NotNull] public Exception Exception
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
        /// <param name="projection">The projection to apply for the value of this result,
        /// if it's a success result.</param>
        /// <returns>A ParseResult for the target type, either with a value obtained by applying the specified
        /// projection to the value in this result, or with the same error as this result.</returns>
        [NotNull]
        public ParseResult<TTarget> Convert<TTarget>([NotNull] Func<T, TTarget> projection)
        {
            Preconditions.CheckNotNull(projection, nameof(projection));
            return Success
                ? ParseResult<TTarget>.ForValue(projection(Value))
                : new ParseResult<TTarget>(exceptionProvider, ContinueAfterErrorWithMultipleFormats);
        }

        /// <summary>
        /// Converts this result to a new target type by propagating the exception provider.
        /// This parse result must already be an error result.
        /// </summary>
        /// <returns>A ParseResult for the target type, with the same error as this result.</returns>
        [NotNull] public ParseResult<TTarget> ConvertError<TTarget>()
        {
            if (Success)
            {
                throw new InvalidOperationException("ConvertError should not be called on a successful parse result");
            }
            return new ParseResult<TTarget>(exceptionProvider, ContinueAfterErrorWithMultipleFormats);
        }

        #region Factory methods and readonly static fields

        /// <summary>
        /// Produces a ParseResult which represents a successful parse operation.
        /// </summary>
        /// <remarks>When T is a reference type, <paramref name="value"/> should not be null,
        /// but this isn't currently checked.</remarks>
        /// <param name="value">The successfully parsed value.</param>
        /// <returns>A ParseResult representing a successful parsing operation.</returns>
        [NotNull] public static ParseResult<T> ForValue(T value) => new ParseResult<T>(value);

         /// <summary>
         /// Produces a ParseResult which represents a failed parsing operation.
         /// </summary>
         /// <remarks>This method accepts a delegate rather than the exception itself, as creating an
         /// exception can be relatively slow: if the client doesn't need the actual exception, just the information
         /// that the parse failed, there's no point in creating the exception.</remarks>
         /// <param name="exceptionProvider">A delegate that produces the exception representing the error that
         /// caused the parse to fail.</param>
         /// <returns>A ParseResult representing a failed parsing operation.</returns>
         [NotNull] public static ParseResult<T> ForException([NotNull] Func<Exception> exceptionProvider) =>
            new ParseResult<T>(Preconditions.CheckNotNull(exceptionProvider, nameof(exceptionProvider)), false);

        internal static ParseResult<T> ForInvalidValue(ValueCursor cursor, string formatString, params object[] parameters) =>
            ForInvalidValue(() =>
            {
                // Format the message which is specific to the kind of parse error.
                string detailMessage = string.Format(CultureInfo.CurrentCulture, formatString, parameters);
                // Format the overall message, containing the parse error and the value itself.
                string overallMessage = string.Format(CultureInfo.CurrentCulture, TextErrorMessages.UnparsableValue, detailMessage, cursor);
                return new UnparsableValueException(overallMessage);
            });

        internal static ParseResult<T> ForInvalidValuePostParse(string text, string formatString, params object[] parameters) =>
            ForInvalidValue(() =>
            {
                // Format the message which is specific to the kind of parse error.
                string detailMessage = string.Format(CultureInfo.CurrentCulture, formatString, parameters);
                // Format the overall message, containing the parse error and the value itself.
                string overallMessage = string.Format(CultureInfo.CurrentCulture, TextErrorMessages.UnparsableValuePostParse, detailMessage, text);
                return new UnparsableValueException(overallMessage);
            });

        private static ParseResult<T> ForInvalidValue(Func<Exception> exceptionProvider) => new ParseResult<T>(exceptionProvider, true);

        internal static ParseResult<T> ArgumentNull(string parameter) => new ParseResult<T>(() => new ArgumentNullException(parameter), false);

        internal static ParseResult<T> PositiveSignInvalid(ValueCursor cursor) => ForInvalidValue(cursor, TextErrorMessages.PositiveSignInvalid);

        // Special case: it's a fault with the value, but we still don't want to continue with multiple patterns.
        // Also, there's no point in including the text.
        internal static readonly ParseResult<T> ValueStringEmpty =
            new ParseResult<T>(() => new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, TextErrorMessages.ValueStringEmpty)), false);

        internal static ParseResult<T> ExtraValueCharacters(ValueCursor cursor, string remainder) => ForInvalidValue(cursor, TextErrorMessages.ExtraValueCharacters, remainder);

        internal static ParseResult<T> QuotedStringMismatch(ValueCursor cursor) => ForInvalidValue(cursor, TextErrorMessages.QuotedStringMismatch);

        internal static ParseResult<T> EscapedCharacterMismatch(ValueCursor cursor, char patternCharacter) => ForInvalidValue(cursor, TextErrorMessages.EscapedCharacterMismatch, patternCharacter);

        internal static ParseResult<T> EndOfString(ValueCursor cursor) => ForInvalidValue(cursor, TextErrorMessages.EndOfString);

        internal static ParseResult<T> TimeSeparatorMismatch(ValueCursor cursor) => ForInvalidValue(cursor, TextErrorMessages.TimeSeparatorMismatch);

        internal static ParseResult<T> DateSeparatorMismatch(ValueCursor cursor) => ForInvalidValue(cursor, TextErrorMessages.DateSeparatorMismatch);

        internal static ParseResult<T> MissingNumber(ValueCursor cursor) => ForInvalidValue(cursor, TextErrorMessages.MissingNumber);

        internal static ParseResult<T> UnexpectedNegative(ValueCursor cursor) => ForInvalidValue(cursor, TextErrorMessages.UnexpectedNegative);

        /// <summary>
        /// This isn't really an issue with the value so much as the pattern... but the result is the same.
        /// </summary>
        internal static readonly ParseResult<T> FormatOnlyPattern =
            new ParseResult<T>(() => new UnparsableValueException(string.Format(CultureInfo.CurrentCulture, TextErrorMessages.FormatOnlyPattern)), true);

        internal static ParseResult<T> MismatchedNumber(ValueCursor cursor, string pattern) => ForInvalidValue(cursor, TextErrorMessages.MismatchedNumber, pattern);

        internal static ParseResult<T> MismatchedCharacter(ValueCursor cursor, char patternCharacter) => ForInvalidValue(cursor, TextErrorMessages.MismatchedCharacter, patternCharacter);

        internal static ParseResult<T> MismatchedText(ValueCursor cursor, char field) => ForInvalidValue(cursor, TextErrorMessages.MismatchedText, field);

        internal static ParseResult<T> NoMatchingFormat(ValueCursor cursor) => ForInvalidValue(cursor, TextErrorMessages.NoMatchingFormat);

        internal static ParseResult<T> ValueOutOfRange(ValueCursor cursor, object value) => ForInvalidValue(cursor, TextErrorMessages.ValueOutOfRange, value, typeof(T));

        internal static ParseResult<T> MissingSign(ValueCursor cursor) => ForInvalidValue(cursor, TextErrorMessages.MissingSign);

        internal static ParseResult<T> MissingAmPmDesignator(ValueCursor cursor) => ForInvalidValue(cursor, TextErrorMessages.MissingAmPmDesignator);

        internal static ParseResult<T> NoMatchingCalendarSystem(ValueCursor cursor) => ForInvalidValue(cursor, TextErrorMessages.NoMatchingCalendarSystem);

        internal static ParseResult<T> NoMatchingZoneId(ValueCursor cursor) => ForInvalidValue(cursor, TextErrorMessages.NoMatchingZoneId);

        internal static ParseResult<T> InvalidHour24(string text) => ForInvalidValuePostParse(text, TextErrorMessages.InvalidHour24);

        internal static ParseResult<T> FieldValueOutOfRange(ValueCursor cursor, int value, char field) =>
            ForInvalidValue(cursor, TextErrorMessages.FieldValueOutOfRange, value, field, typeof(T));
        internal static ParseResult<T> FieldValueOutOfRange(ValueCursor cursor, long value, char field) =>
            ForInvalidValue(cursor, TextErrorMessages.FieldValueOutOfRange, value, field, typeof(T));

        internal static ParseResult<T> FieldValueOutOfRangePostParse(string text, int value, char field) =>
            ForInvalidValuePostParse(text, TextErrorMessages.FieldValueOutOfRange, value, field, typeof(T));

        /// <summary>
        /// Two fields (e.g. "hour of day" and "hour of half day") were mutually inconsistent.
        /// </summary>
        internal static ParseResult<T> InconsistentValues(string text, char field1, char field2) =>
            ForInvalidValuePostParse(text, TextErrorMessages.InconsistentValues2, field1, field2, typeof(T));

        /// <summary>
        /// The month of year is inconsistent between the text and numeric specifications.
        /// We can't use InconsistentValues for this as the pattern character is the same in both cases.
        /// </summary>
        internal static ParseResult<T> InconsistentMonthValues(string text) => ForInvalidValuePostParse(text, TextErrorMessages.InconsistentMonthTextValue);

        /// <summary>
        /// The day of month is inconsistent with the day of week value.
        /// We can't use InconsistentValues for this as the pattern character is the same in both cases.
        /// </summary>
        internal static ParseResult<T> InconsistentDayOfWeekTextValue(string text) => ForInvalidValuePostParse(text, TextErrorMessages.InconsistentDayOfWeekTextValue);

        /// <summary>
        /// We'd expected to get to the end of the string now, but we haven't.
        /// </summary>
        internal static ParseResult<T> ExpectedEndOfString(ValueCursor cursor) => ForInvalidValue(cursor, TextErrorMessages.ExpectedEndOfString);

        internal static ParseResult<T> YearOfEraOutOfRange(string text, int value, Era era, CalendarSystem calendar) =>
            ForInvalidValuePostParse(text, TextErrorMessages.YearOfEraOutOfRange, value, era.Name, calendar.Name);

        internal static ParseResult<T> MonthOutOfRange(string text, int month, int year) => ForInvalidValuePostParse(text, TextErrorMessages.MonthOutOfRange, month, year);

        internal static ParseResult<T> IsoMonthOutOfRange(string text, int month) => ForInvalidValuePostParse(text, TextErrorMessages.IsoMonthOutOfRange, month);

        internal static ParseResult<T> DayOfMonthOutOfRange(string text, int day, int month, int year) => ForInvalidValuePostParse(text, TextErrorMessages.DayOfMonthOutOfRange, day, month, year);

        internal static ParseResult<T> DayOfMonthOutOfRangeNoYear(string text, int day, int month) => ForInvalidValuePostParse(text, TextErrorMessages.DayOfMonthOutOfRangeNoYear, day, month);

        internal static ParseResult<T> InvalidOffset(string text) => ForInvalidValuePostParse(text, TextErrorMessages.InvalidOffset);

        internal static ParseResult<T> SkippedLocalTime(string text) => ForInvalidValuePostParse(text, TextErrorMessages.SkippedLocalTime);

        internal static ParseResult<T> AmbiguousLocalTime(string text) => ForInvalidValuePostParse(text, TextErrorMessages.AmbiguousLocalTime);

        #endregion
    }
}
