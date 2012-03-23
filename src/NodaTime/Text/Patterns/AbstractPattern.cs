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

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Abstract class to provide common facilities
    /// </summary>
    internal abstract class AbstractPattern<T> : IPattern<T>
    {
        private readonly NodaFormatInfo formatInfo;

        protected NodaFormatInfo FormatInfo { get { return formatInfo; } }

        protected AbstractPattern(NodaFormatInfo formatInfo)
        {
            this.formatInfo = formatInfo;
        }

        /// <summary>
        /// Performs the first part of the parse, validating the value is non-empty before
        /// handing over to ParseImpl for the meat of the work.
        /// </summary>
        public ParseResult<T> Parse(string text)
        {
            if (text == null)
            {
                return ParseResult<T>.ArgumentNull("text");
            }
            if (text.Length == 0)
            {
                return ParseResult<T>.ValueStringEmpty;
            }
            return ParseImpl(text);
        }

        /// <summary>
        /// Overridden by derived classes to parse the given value, which is guaranteed to be 
        /// non-null and non-empty. It will have been trimmed appropriately if the parse style allows leading or trailing whitespace.
        /// </summary>
        protected abstract ParseResult<T> ParseImpl(string value);

        public abstract string Format(T value);
    }
}
