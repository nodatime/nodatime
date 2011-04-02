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
        }

        internal bool ThrowImmediate { get; private set; }
        internal ParseFailureKind Failure { get; private set; }
        internal string FailureArgumentName { get; private set; }
        internal string FailureMessage { get; private set; }
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
                    return new FormatException(FailureMessage);
                default:
                    string message = string.Format(Resources.Parse_UnknownFailure, Failure) + Environment.NewLine + FailureMessage;
                    return new InvalidOperationException(message); // TODO: figure out what exception to throw here.
            }
        }

        internal bool SetFormatError(string message, params object[] parameters)
        {
            Failure = ParseFailureKind.ArgumentNull;
            FailureMessage = string.Format(message, parameters);
            return CheckImmediate();
        }

        internal bool SetArgumentNull(string argumentName)
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
            return SetFormatError(DoubleAssignmentMessage, patternCharacter);
        }
    }

    #region Nested type: ParseFailureKind
    internal enum ParseFailureKind
    {
        None,
        ArgumentNull,
        Format,
    }
    #endregion
}