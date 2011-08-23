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

namespace NodaTime.Text
{
    /// <summary>
    /// Styles applied when parsing values.
    /// </summary>
    [Flags]
    public enum ParseStyles
    {
        /// <summary>
        /// Default formatting options must be used.
        /// </summary>
        None = DateTimeStyles.None,
        /// <summary>
        /// Leading white-space characters must be ignored during parsing, except if they occur in the specified format patterns.
        /// </summary>
        AllowInnerWhite = DateTimeStyles.AllowInnerWhite,
        /// <summary>
        /// Trailing white-space characters must be ignored during parsing, except if they occur in the specified format patterns.
        /// </summary>
        AllowLeadingWhite = DateTimeStyles.AllowLeadingWhite,
        /// <summary>
        /// Extra white-space characters in the middle of the string must be ignored during parsing, except if they occur in the specified format patterns.
        /// </summary>
        AllowTrailingWhite = DateTimeStyles.AllowTrailingWhite,
        /// <summary>
        /// Extra white-space characters anywhere in the string must be ignored during parsing, except if they occur in the specified format patterns.
        /// This value is a combination of the AllowLeadingWhite, AllowTrailingWhite, and AllowInnerWhite values.
        /// </summary>
        AllowWhiteSpaces = DateTimeStyles.AllowWhiteSpaces,
    }
}
