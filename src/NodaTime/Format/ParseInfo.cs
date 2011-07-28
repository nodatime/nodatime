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

using NodaTime.Globalization;
using System;

namespace NodaTime.Format
{
    /// <summary>
    ///   Provides a container for the interim parsed pieces of values.
    /// </summary>
    internal class ParseInfo
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "ParseInfo" /> class.
        /// </summary>
        /// <param name = "formatProvider">The format info.</param>
        internal ParseInfo(IFormatProvider formatProvider)
            : this(formatProvider, DateTimeParseStyles.None)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ParseInfo" /> class.
        /// </summary>
        /// <param name = "formatProvider">The format info.</param>
        /// <param name = "parseStyles">The parse styles.</param>
        internal ParseInfo(IFormatProvider formatProvider, DateTimeParseStyles parseStyles)
        {
            FormatProvider = formatProvider;
            FormatInfo = NodaFormatInfo.GetInstance(formatProvider);
            AllowInnerWhite = (parseStyles & DateTimeParseStyles.AllowInnerWhite) != DateTimeParseStyles.None;
            AllowLeadingWhite = (parseStyles & DateTimeParseStyles.AllowLeadingWhite) != DateTimeParseStyles.None;
            AllowTrailingWhite = (parseStyles & DateTimeParseStyles.AllowTrailingWhite) != DateTimeParseStyles.None;
        }

        internal IFormatProvider FormatProvider { get; set; }

        /// <summary>
        ///   Gets a value indicating whether inner white space is allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if inner white is allowed; otherwise, <c>false</c>.
        /// </value>
        internal bool AllowInnerWhite { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether leading white space is allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if leading white space is allowed; otherwise, <c>false</c>.
        /// </value>
        internal bool AllowLeadingWhite { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether trailing white space is allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if trailing white space is allowed; otherwise, <c>false</c>.
        /// </value>
        internal bool AllowTrailingWhite { get; private set; }

        /// <summary>
        ///   Gets the format info object that controls the parsing of the object.
        /// </summary>
        internal NodaFormatInfo FormatInfo { get; private set; }

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
        internal static bool AssignNewValue<T>(ref T? currentValue, T newValue, char patternCharacter) where T : struct
        {
            if (currentValue == null || currentValue.Value.Equals(newValue))
            {
                currentValue = newValue;
                return true;
            }
            throw FormatError.DoubleAssigment(patternCharacter);
        }
    }
}