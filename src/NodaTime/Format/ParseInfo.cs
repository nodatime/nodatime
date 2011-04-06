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
using NodaTime.Globalization;
using NodaTime.Properties;
#endregion

namespace NodaTime.Format
{
    internal abstract class ParseInfo
    {
        internal ParseInfo(NodaFormatInfo formatInfo, bool throwImmediate)
            : this(formatInfo, throwImmediate, DateTimeParseStyles.None)
        {
        }

        internal ParseInfo(NodaFormatInfo formatInfo, bool throwImmediate, DateTimeParseStyles parseStyles)
        {
            FormatInfo = formatInfo;
            Failure = ParseFailureKind.None;
            ThrowImmediate = throwImmediate;
            AllowInnerWhite = (parseStyles & DateTimeParseStyles.AllowInnerWhite) != DateTimeParseStyles.None;
            AllowLeadingWhite = (parseStyles & DateTimeParseStyles.AllowLeadingWhite) != DateTimeParseStyles.None;
            AllowTrailingWhite = (parseStyles & DateTimeParseStyles.AllowTrailingWhite) != DateTimeParseStyles.None;
            FailureMessageParameters = new object[0];
        }

        internal bool ThrowImmediate { get; private set; }
        internal ParseFailureKind Failure { get; private set; }
        internal string FailureArgumentName { get; private set; }
        internal string FailureMessage { get; private set; }
        internal object[] FailureMessageParameters { get; private set; }
        internal bool Failed { get { return Failure != ParseFailureKind.None; } }
        internal bool AllowInnerWhite { get; private set; }
        internal bool AllowLeadingWhite { get; private set; }
        internal bool AllowTrailingWhite { get; private set; }
        internal NodaFormatInfo FormatInfo { get; private set; }

        protected virtual string DoubleAssignmentMessage { get { return Resources.Parse_DefaultDoubleAssignment; } }

        internal Exception GetFailureException()
        {
            switch (Failure)
            {
                case ParseFailureKind.None:
                    return null;
                case ParseFailureKind.ArgumentNull:
                    return new ArgumentNullException(FailureArgumentName, FailureMessage);
                case ParseFailureKind.Format:
                case ParseFailureKind.ParseEscapeAtEndOfString:
                case ParseFailureKind.ParseDoubleAssigment:
                case ParseFailureKind.ParseMissingEndQuote:
                case ParseFailureKind.ParseRepeatCountExceeded:
                case ParseFailureKind.ParseCannotParseValue:
                case ParseFailureKind.ParseValueStringEmpty:
                case ParseFailureKind.ParseFormatStringEmpty:
                case ParseFailureKind.ParseFormatInvalid:
                case ParseFailureKind.ParseFormatElementInvalid:
                case ParseFailureKind.ParseEmptyFormatsArray:
                case ParseFailureKind.ParseExtraValueCharacters:
                case ParseFailureKind.ParsePercentDoubled:
                case ParseFailureKind.ParsePercentAtEndOfString:
                case ParseFailureKind.ParseQuotedStringMismatch:
                case ParseFailureKind.ParseEscapedCharacterMismatch:
                case ParseFailureKind.ParseMissingDecimalSeparator:
                case ParseFailureKind.ParseTimeSeparatorMismatch:
                case ParseFailureKind.ParseMismatchedNumber:
                case ParseFailureKind.ParseMismatchedSpace:
                case ParseFailureKind.ParseMismatchedCharacter:
                case ParseFailureKind.ParseUnknownStandardFormat:
                case ParseFailureKind.Parse12HourPatternNotSupported:
                    return new ParseException(Failure, FailureMessage);
                default:
                    string message = string.Format(Resources.Parse_UnknownFailure, Failure) + Environment.NewLine + FailureMessage;
                    return new InvalidOperationException(message); // TODO: figure out what exception to throw here.
            }
        }

        private bool FailBasic(ParseFailureKind kind, string message, params object[] parameters)
        {
            Failure = kind;
            FailureMessageParameters = parameters;
            FailureMessage = string.Format(message, parameters);
            return CheckImmediate();
        }

        internal bool SetFormatError(string message, params object[] parameters)
        {
            Failure = ParseFailureKind.Format;
            FailureMessageParameters = parameters;
            FailureMessage = string.Format(message, parameters);
            return CheckImmediate();
        }

        internal bool FailArgumentNull(string argumentName)
        {
            Failure = ParseFailureKind.ArgumentNull;
            FailureMessage = Resources.Noda_ArgumentNull;
            FailureArgumentName = argumentName;
            return CheckImmediate();
        }

        private bool CheckImmediate()
        {
            if (ThrowImmediate)
            {
                var exception = GetFailureException();
                if (exception != null)
                {
                    throw exception;
                }
            }
            return false;
        }

        /// <summary>
        ///   Assigns the new value if the current value is not set.
        /// </summary>
        /// <remarks>
        ///   When parsing an object by pattern a particular value (e.g. hours) can only be set once, or if set
        ///   more than once it should be set to the same value. This method checks thatthe value has not been
        ///   previously set or if it has that the new value is the same as the old one. If the new value is
        ///   different than the old one then a failure is set.
        /// </remarks>
        /// <typeparam name = "T">The base type of the values.</typeparam>
        /// <param name = "currentValue">The current value.</param>
        /// <param name = "newValue">The new value.</param>
        /// <param name = "patternCharacter">The pattern character for the error message if any.</param>
        /// <returns><c>true</c> if the current value is not set or if the current value equals the new value, <c>false</c> otherwise.</returns>
        internal bool AssignNewValue<T>(ref T? currentValue, T newValue, char patternCharacter) where T : struct
        {
            if (currentValue == null || currentValue.Value.Equals(newValue))
            {
                currentValue = newValue;
                return true;
            }
            return FailDoubleAssigment(patternCharacter);
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

        internal bool FailParseCannotParseValue(string value, string typeName, string format)
        {
            return FailBasic(ParseFailureKind.ParseCannotParseValue, Resources.Parse_CannotParseValue, value, typeName, format);
        }

        internal bool FailDoubleAssigment(char patternCharacter)
        {
            return FailBasic(ParseFailureKind.ParseDoubleAssigment, DoubleAssignmentMessage, patternCharacter);
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

        internal bool FailParseUnknownStandardFormat(char patternCharacter, string typeName)
        {
            return FailBasic(ParseFailureKind.ParseUnknownStandardFormat, Resources.Parse_UnknownStandardFormat, patternCharacter, typeName);
        }

        internal bool FailParse12HourPatternNotSupported(string typeName)
        {
            return FailBasic(ParseFailureKind.Parse12HourPatternNotSupported, Resources.Parse_12HourPatternNotSupported, typeName);
        }

    }
}