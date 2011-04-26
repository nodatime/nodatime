using System;
using NodaTime.Properties;

namespace NodaTime.Format
{
    internal class ParseErrorInfo
    {
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
            return FailBasic(ParseFailureKind.ParseEscapeAtEndOfString, Resources.Parse_EscapeAtEndOfString);
        }

        internal bool FailParseMissingEndQuote(char closeQuote)
        {
            return FailBasic(ParseFailureKind.ParseMissingEndQuote, Resources.Parse_MissingEndQuote, closeQuote);
        }

        internal bool FailParseRepeatCountExceeded(char patternCharacter, int maximumCount)
        {
            return FailBasic(ParseFailureKind.ParseRepeatCountExceeded, Resources.Parse_RepeatCountExceeded, patternCharacter, maximumCount);
        }

        internal bool FailParseCannotParseValue(string value, Type type, string format)
        {
            return FailBasic(ParseFailureKind.ParseCannotParseValue, Resources.Parse_CannotParseValue, value, type.FullName, format);
        }

        internal bool FailDoubleAssigment(char patternCharacter)
        {
            return FailBasic(ParseFailureKind.ParseDoubleAssigment, Resources.Parse_DoubleAssignment, patternCharacter);
        }

        internal bool FailParseValueStringEmpty()
        {
            return FailBasic(ParseFailureKind.ParseValueStringEmpty, Resources.Parse_ValueStringEmpty);
        }

        internal bool FailParseFormatStringEmpty()
        {
            return FailBasic(ParseFailureKind.ParseFormatStringEmpty, Resources.Parse_FormatStringEmpty);
        }

        internal bool FailParseFormatInvalid(string format)
        {
            return FailBasic(ParseFailureKind.ParseFormatInvalid, Resources.Parse_FormatInvalid, format);
        }

        internal bool FailParseEmptyFormatsArray()
        {
            return FailBasic(ParseFailureKind.ParseEmptyFormatsArray, Resources.Parse_EmptyFormatsArray);
        }

        internal bool FailParseFormatElementInvalid()
        {
            return FailBasic(ParseFailureKind.ParseFormatElementInvalid, Resources.Parse_FormatElementInvalid);
        }

        internal bool FailParseExtraValueCharacters(string remainder)
        {
            return FailBasic(ParseFailureKind.ParseExtraValueCharacters, Resources.Parse_ExtraValueCharacters, remainder);
        }

        internal bool FailParsePercentDoubled()
        {
            return FailBasic(ParseFailureKind.ParsePercentDoubled, Resources.Parse_PercentDoubled);
        }

        internal bool FailParsePercentAtEndOfString()
        {
            return FailBasic(ParseFailureKind.ParsePercentAtEndOfString, Resources.Parse_PercentAtEndOfString);
        }

        internal bool FailParseQuotedStringMismatch()
        {
            return FailBasic(ParseFailureKind.ParseQuotedStringMismatch, Resources.Parse_QuotedStringMismatch);
        }

        internal bool FailParseEscapedCharacterMismatch(char patternCharacter)
        {
            return FailBasic(ParseFailureKind.ParseEscapedCharacterMismatch, Resources.Parse_EscapedCharacterMismatch, patternCharacter);
        }

        internal bool FailParseMissingDecimalSeparator()
        {
            return FailBasic(ParseFailureKind.ParseMissingDecimalSeparator, Resources.Parse_MissingDecimalSeparator);
        }

        internal bool FailParseTimeSeparatorMismatch()
        {
            return FailBasic(ParseFailureKind.ParseTimeSeparatorMismatch, Resources.Parse_TimeSeparatorMismatch);
        }

        internal bool FailParseMismatchedNumber(string pattern)
        {
            return FailBasic(ParseFailureKind.ParseMismatchedNumber, Resources.Parse_MismatchedNumber, pattern);
        }

        internal bool FailParseMismatchedSpace()
        {
            return FailBasic(ParseFailureKind.ParseMismatchedSpace, Resources.Parse_MismatchedSpace);
        }

        internal bool FailParseMismatchedCharacter(char patternCharacter)
        {
            return FailBasic(ParseFailureKind.ParseMismatchedCharacter, Resources.Parse_MismatchedCharacter, patternCharacter);
        }

        internal bool FailParseUnknownStandardFormat(char patternCharacter, Type type)
        {
            return FailBasic(ParseFailureKind.ParseUnknownStandardFormat, Resources.Parse_UnknownStandardFormat, patternCharacter, type.FullName);
        }

        internal bool FailParse12HourPatternNotSupported(Type type)
        {
            return FailBasic(ParseFailureKind.Parse12HourPatternNotSupported, Resources.Parse_12HourPatternNotSupported, type.FullName);
        }

        internal bool FailParseNoMatchingFormat()
        {
            return FailBasic(ParseFailureKind.ParseNoMatchingFormat, Resources.Parse_NoMatchingFormat);
        }

        internal bool FailParseValueOutOfRange(object value, Type type)
        {
            return FailBasic(ParseFailureKind.ParseValueOutOfRange, Resources.Parse_ValueOutOfRange, value, type.FullName);
        }

        internal bool FailParseMissingSign()
        {
            return FailBasic(ParseFailureKind.ParseMissingSign, Resources.Parse_MissingSign);
        }

        internal bool FailParsePositiveSignInvalid()
        {
            return FailBasic(ParseFailureKind.ParsePositiveSignInvalid, Resources.Parse_PositiveSignInvalid);
        }
    }
}