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
using NodaTime.Utility;
using NodaTime.Globalization;

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
        }

        private bool ThrowImmediate { get; set; }
        private ParseFailureKind Failure { get; set; }
        private string FailureArgumentName { get; set; }
        private object FailureMessageFormatArgument { get; set; }
        private string FailureMessageId { get; set; }
        internal bool Failed { get { return Failure != ParseFailureKind.None; } }
        internal bool AllowInnerWhite { get; private set; }
        internal bool AllowLeadingWhite { get; private set; }
        internal bool AllowTrailingWhite { get; private set; }
        internal NodaFormatInfo FormatInfo { get; private set; }

        protected virtual string DoubleAssignmentKey { get { return "Format_DefaultDoubleAsignment"; } } // TODO: Use correct message key

        internal Exception GetFailureException()
        {
            switch (Failure)
            {
                case ParseFailureKind.None:
                    break;
                case ParseFailureKind.ArgumentNull:
                    return new ArgumentNullException(FailureArgumentName, ResourceHelper.GetMessage(FailureMessageId));
                case ParseFailureKind.Format:
                    return new FormatException(ResourceHelper.GetMessage(FailureMessageId));
                case ParseFailureKind.FormatWithParameter:
                    return new FormatException(ResourceHelper.GetMessage(FailureMessageId, FailureMessageFormatArgument));
                default:
                    return new InvalidOperationException(ResourceHelper.GetMessage("Unknown_failure", Failure)); // TODO: Use correct message key
            }
            return null;
        }

        internal bool SetFailure(ParseFailureKind failure, string failureMessageId)
        {
            return SetFailure(failure, failureMessageId, null, null);
        }

        internal bool SetFailure(ParseFailureKind failure, string failureMessageId, object failureMessageFormatArgument)
        {
            return SetFailure(failure, failureMessageId, failureMessageFormatArgument, null);
        }

        internal bool SetFailure(ParseFailureKind failure, string failureMessageId, object failureMessageFormatArgument, string failureArgumentName)
        {
            Failure = failure;
            FailureMessageId = failureMessageId;
            FailureMessageFormatArgument = failureMessageFormatArgument;
            FailureArgumentName = failureArgumentName;
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
            return SetFailure(ParseFailureKind.FormatWithParameter, DoubleAssignmentKey, patternCharacter);
        }
    }

    #region Nested type: ParseFailureKind
    internal enum ParseFailureKind
    {
        None,
        ArgumentNull,
        Format,
        FormatWithParameter
    }
    #endregion
}