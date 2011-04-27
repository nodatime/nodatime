using System;
using NodaTime.Properties;

namespace NodaTime.Format
{
    internal class ParseErrorInfo
    {
        internal ParseErrorInfo(IFormatProvider formatProvider)
            : this(formatProvider, true)
        {
        }

        internal ParseErrorInfo(IFormatProvider formatProvider, bool throwImmediate)
        {
            FormatProvider = formatProvider;
            ThrowImmediate = throwImmediate;
        }

        internal IFormatProvider FormatProvider { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether we throw immediately upon a failure.
        /// </summary>
        /// <value>
        ///   <c>true</c> if we throw immediate; otherwise, <c>false</c>.
        /// </value>
        internal bool ThrowImmediate { get; set; }

        /// <summary>
        ///   Gets the failure type.
        /// </summary>
        internal ParseFailureKind Failure { get; private set; }

        /// <summary>
        ///   Gets the name of the failure argument name if the failure is <see cref = "ParseFailureKind.ArgumentNull" />.
        /// </summary>
        /// <value>
        ///   The name of the failure argument.
        /// </value>
        internal string FailureArgumentName { get; private set; }

        /// <summary>
        ///   Gets the failure message.
        /// </summary>
        internal string FailureMessage { get; private set; }

        /// <summary>
        ///   Gets the failure message parameters which are replaced in the failure message.
        /// </summary>
        internal object[] FailureMessageParameters { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether a failure has occurred.
        /// </summary>
        /// <value>
        ///   <c>true</c> if failed; otherwise, <c>false</c>.
        /// </value>
        internal bool Failed { get { return Failure != ParseFailureKind.None; } }

        /// <summary>
        ///   Clears the failure information.
        /// </summary>
        internal void ClearFail()
        {
            Failure = ParseFailureKind.None;
            FailureMessage = null;
            FailureMessageParameters = new object[0];
            FailureArgumentName = null;
        }

        /// <summary>
        ///   Gets the failure exception object if a failure has occurred.
        /// </summary>
        /// <returns>An <see cref = "Exception" /> subclass or null.</returns>
        internal Exception GetFailureException()
        {
            switch (Failure)
            {
                case ParseFailureKind.None:
                    return null;
                case ParseFailureKind.ArgumentNull:
                    return new ArgumentNullException(FailureArgumentName, FailureMessage);
                default:
                    return new ParseException(Failure, FailureMessage);
            }
        }

        /// <summary>
        ///   Sets the failure information.
        /// </summary>
        /// <param name = "kind">The failure kind.</param>
        /// <param name = "message">The failure message.</param>
        /// <param name = "parameters">The optional failure parameters.</param>
        /// <returns><c>false</c> indicating an error.</returns>
        private bool FailBasic(ParseFailureKind kind, string message, params object[] parameters)
        {
            Failure = kind;
            FailureMessageParameters = parameters;
            FailureMessage = string.Format(FormatProvider, message, parameters);
            return CheckImmediate();
        }

        /// <summary>
        ///   Reports an arugment null failure.
        /// </summary>
        /// <param name = "argumentName">Name of the argument.</param>
        /// <returns><c>false</c> indicating an error.</returns>
        internal bool FailArgumentNull(string argumentName)
        {
            Failure = ParseFailureKind.ArgumentNull;
            FailureMessage = Resources.Noda_ArgumentNull;
            FailureArgumentName = argumentName;
            return CheckImmediate();
        }

        /// <summary>
        ///   If the <see cref = "ThrowImmediate" /> flag is true and a failure has been registered an
        ///   exception is thrown, otherwise returns a value indicating whether a failure has occurred.
        /// </summary>
        /// <returns></returns>
        internal bool CheckImmediate()
        {
            if (ThrowImmediate)
            {
                var exception = GetFailureException();
                if (exception != null)
                {
                    throw exception;
                }
            }
            return !Failed;
        }

        internal bool FailParseEscapeAtEndOfString()
        {
            return FailBasic(ParseFailureKind.EscapeAtEndOfString, Resources.Parse_EscapeAtEndOfString);
        }

        internal bool FailParseMissingEndQuote(char closeQuote)
        {
            return FailBasic(ParseFailureKind.MissingEndQuote, Resources.Parse_MissingEndQuote, closeQuote);
        }

        internal bool FailParseRepeatCountExceeded(char patternCharacter, int maximumCount)
        {
            return FailBasic(ParseFailureKind.RepeatCountExceeded, Resources.Parse_RepeatCountExceeded, patternCharacter, maximumCount);
        }

        internal bool FailParseCannotParseValue(string value, Type type, string format)
        {
            return FailBasic(ParseFailureKind.CannotParseValue, Resources.Parse_CannotParseValue, value, type.FullName, format);
        }

        internal bool FailDoubleAssigment(char patternCharacter)
        {
            return FailBasic(ParseFailureKind.DoubleAssigment, Resources.Parse_DoubleAssignment, patternCharacter);
        }

        internal bool FailParseValueStringEmpty()
        {
            return FailBasic(ParseFailureKind.ValueStringEmpty, Resources.Parse_ValueStringEmpty);
        }

        internal bool FailParseFormatStringEmpty()
        {
            return FailBasic(ParseFailureKind.FormatStringEmpty, Resources.Parse_FormatStringEmpty);
        }

        internal bool FailParseFormatInvalid(string format)
        {
            return FailBasic(ParseFailureKind.FormatInvalid, Resources.Parse_FormatInvalid, format);
        }

        internal bool FailParseEmptyFormatsArray()
        {
            return FailBasic(ParseFailureKind.EmptyFormatsArray, Resources.Parse_EmptyFormatsArray);
        }

        internal bool FailParseFormatElementInvalid()
        {
            return FailBasic(ParseFailureKind.FormatElementInvalid, Resources.Parse_FormatElementInvalid);
        }

        internal bool FailParseExtraValueCharacters(string remainder)
        {
            return FailBasic(ParseFailureKind.ExtraValueCharacters, Resources.Parse_ExtraValueCharacters, remainder);
        }

        internal bool FailParsePercentDoubled()
        {
            return FailBasic(ParseFailureKind.PercentDoubled, Resources.Parse_PercentDoubled);
        }

        internal bool FailParsePercentAtEndOfString()
        {
            return FailBasic(ParseFailureKind.PercentAtEndOfString, Resources.Parse_PercentAtEndOfString);
        }

        internal bool FailParseQuotedStringMismatch()
        {
            return FailBasic(ParseFailureKind.QuotedStringMismatch, Resources.Parse_QuotedStringMismatch);
        }

        internal bool FailParseEscapedCharacterMismatch(char patternCharacter)
        {
            return FailBasic(ParseFailureKind.EscapedCharacterMismatch, Resources.Parse_EscapedCharacterMismatch, patternCharacter);
        }

        internal bool FailParseMissingDecimalSeparator()
        {
            return FailBasic(ParseFailureKind.MissingDecimalSeparator, Resources.Parse_MissingDecimalSeparator);
        }

        internal bool FailParseTimeSeparatorMismatch()
        {
            return FailBasic(ParseFailureKind.TimeSeparatorMismatch, Resources.Parse_TimeSeparatorMismatch);
        }

        internal bool FailParseMismatchedNumber(string pattern)
        {
            return FailBasic(ParseFailureKind.MismatchedNumber, Resources.Parse_MismatchedNumber, pattern);
        }

        internal bool FailParseMismatchedSpace()
        {
            return FailBasic(ParseFailureKind.MismatchedSpace, Resources.Parse_MismatchedSpace);
        }

        internal bool FailParseMismatchedCharacter(char patternCharacter)
        {
            return FailBasic(ParseFailureKind.MismatchedCharacter, Resources.Parse_MismatchedCharacter, patternCharacter);
        }

        internal bool FailParseUnknownStandardFormat(char patternCharacter, Type type)
        {
            return FailBasic(ParseFailureKind.UnknownStandardFormat, Resources.Parse_UnknownStandardFormat, patternCharacter, type.FullName);
        }

        internal bool FailParsePrecisionNotSupported(string pattern, Type type)
        {
            return FailBasic(ParseFailureKind.PrecisionNotSupported, Resources.Parse_PrecisionNotSupported, pattern, type.FullName);
        }

        internal bool FailParseStandardFormatWhitespace(string pattern, Type type)
        {
            return FailBasic(ParseFailureKind.StandardFormatWhitespace, Resources.Parse_StandardFormatWhitespace, pattern, type.FullName);
        }

        internal bool FailParse12HourPatternNotSupported(Type type)
        {
            return FailBasic(ParseFailureKind.Hour12PatternNotSupported, Resources.Parse_Hour12PatternNotSupported, type.FullName);
        }

        internal bool FailParseNoMatchingFormat()
        {
            return FailBasic(ParseFailureKind.NoMatchingFormat, Resources.Parse_NoMatchingFormat);
        }

        internal bool FailParseValueOutOfRange(object value, Type type)
        {
            return FailBasic(ParseFailureKind.ValueOutOfRange, Resources.Parse_ValueOutOfRange, value, type.FullName);
        }

        internal bool FailParseMissingSign()
        {
            return FailBasic(ParseFailureKind.MissingSign, Resources.Parse_MissingSign);
        }

        internal bool FailParsePositiveSignInvalid()
        {
            return FailBasic(ParseFailureKind.PositiveSignInvalid, Resources.Parse_PositiveSignInvalid);
        }
    }
}