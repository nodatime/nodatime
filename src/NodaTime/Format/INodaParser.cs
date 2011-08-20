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

namespace NodaTime.Format
{
    /// <summary>
    /// Provides an interface for value parsers in the Noda Time package. The parsers
    /// provided by Noda Time are all immutable and thread-safe.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Different implementations expose specific configuration options, such as which calendar
    /// system to use when parsing a value. These are not exposed on this interface, as they're
    /// not common to all parsers.
    /// </para>
    /// <para>
    /// Noda Time supports the common .NET model of overloaded ToString, Parse and TryParse methods,
    /// but using this type and <see cref="INodaFormatter{T}"/> allows formatting patterns and 
    /// options to be set one and then reused.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type to parse.</typeparam>
    // TODO: Is it useful to have a FormatProvider here?
    public interface INodaParser<T>
    {
        /// <summary>
        /// Gets the format provider use by this parse to parse values.
        /// </summary>
        /// <value>
        /// The format provider.
        /// </value>
        IFormatProvider FormatProvider { get; }

        /// <summary>
        /// Parses the given text using the <see cref="IFormatProvider"/> given when the parser
        /// was constructed. This does NOT use the current thread <see cref="IFormatProvider"/>.
        /// </summary>
        /// <exception cref="FormatException">If the text cannot be parsed.</exception>
        T Parse(string text);

        /// <summary>
        /// Attempts to parse the given text, but does not throw an exception if parsing fails.
        /// Instead, the return value indicates success or failure.
        /// </summary>
        bool TryParse(string text, out T value);

        /// <summary>
        /// Returns a new copy of this parser that uses the given <see cref="IFormatProvider"/> for
        /// formatting instead of the one that this parser uses.
        /// </summary>
        /// <param name="formatProvider">The format provider to use.</param>
        /// <returns>A new copy of this parser using the given <see cref="IFormatProvider"/>.</returns>
        INodaParser<T> WithFormatProvider(IFormatProvider formatProvider);
    }
}
