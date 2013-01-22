// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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
