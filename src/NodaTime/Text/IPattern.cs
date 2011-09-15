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

namespace NodaTime.Text
{
    /// <summary>
    /// Generic interface supporting parsing and formatting. Parsing always results in a 
    /// <see cref="ParseResult{T}"/> which can represent success or failure.
    /// </summary>
    /// <remarks>
    /// Idiomatic text handling in Noda Time involves creating a pattern once and reusing it multiple
    /// times, rather than specifying the pattern text repeatedly. All patterns are immutable and thread-safe,
    /// and include the <see cref="NodaFormatInfo"/> used for localization purposes.
    /// </remarks>
    /// <typeparam name="T">Type of value to parse or format.</typeparam>
    public interface IPattern<T>
    {
        /// <summary>
        /// Parses the given text value according to the rules of this pattern.
        /// </summary>
        /// <remarks>
        /// This method never throws an exception (barring a bug in Noda Time itself). Even errors such as
        /// the argument being null are wrapped in a parse result.
        /// </remarks>
        /// <param name="text">The text value to parse.</param>
        /// <returns>The result of parsing, which may be successful or unsuccessful.</returns>
        ParseResult<T> Parse(string text);

        /// <summary>
        /// Formats the given value as text according to the rules of this pattern.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>The value formatted according to this pattern.</returns>
        string Format(T value);
    }
}
