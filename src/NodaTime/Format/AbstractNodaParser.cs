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

namespace NodaTime.Format
{
    /// <summary>
    /// Base class providing simple support for the various
    ///  Parse/TryParse/ParseExact/TryParseExact overloads provided by individual
    /// types.
    /// </summary>
    // TODO: Rename to indicate that this is different to INodaParser? It's the "BCL style" of parsing
    // rather than the "build a parser with a single pattern" approach.
    internal abstract class AbstractNodaParser<T>
    {
        private readonly string[] allFormats;
        private readonly T failureValue;

        protected AbstractNodaParser(string[] allFormats, T failureValue)
        {
            this.allFormats = allFormats;
            this.failureValue = failureValue;
        }

        internal T Parse(string value, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            return ParseExact(value, allFormats, formatInfo, styles);
        }

        internal T ParseExact(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            return ParseSingle(value, format, formatInfo, styles).GetResultOrThrow();
        }

        internal T ParseExact(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            return ParseMultiple(value, formats, formatInfo, styles).GetResultOrThrow();
        }

        internal bool TryParse(string value, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out T result)
        {
            return TryParseExact(value, allFormats, formatInfo, styles, out result);
        }

        internal bool TryParseExact(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out T result)
        {
            return ParseSingle(value, format, formatInfo, styles).TryGetResult(failureValue, out result);
        }

        internal bool TryParseExact(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out T result)
        {
            return ParseMultiple(value, formats, formatInfo, styles).TryGetResult(failureValue, out result);
        }

        protected virtual ParseResult<T> ParseMultiple(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            if (formats == null)
            {
                return ParseResult<T>.ArgumentNull("formats");
            }
            if (formats.Length == 0)
            {
                return ParseResult<T>.EmptyFormatsArray;
            }

            foreach (string format in formats)
            {
                ParseResult<T> result = ParseSingle(value, format, formatInfo, styles);
                if (result.Success)
                {
                    return result;
                }
                if (!result.ContinueAfterErrorWithMultipleFormats)
                {
                    return result;
                }
            }
            return ParseResult<T>.NoMatchingFormat;
        }

        protected abstract ParseResult<T> ParseSingle(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles);
    }
}
