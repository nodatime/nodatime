// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text;
using JetBrains.Annotations;
using NodaTime.Annotations;

namespace NodaTime.Text
{
    /// <summary>
    /// Generic interface supporting parsing and formatting. Parsing always results in a 
    /// <see cref="ParseResult{T}"/> which can represent success or failure.
    /// </summary>
    /// <remarks>
    /// Idiomatic text handling in Noda Time involves creating a pattern once and reusing it multiple
    /// times, rather than specifying the pattern text repeatedly. All patterns are immutable and thread-safe,
    /// and include the culture used for localization purposes.
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
        [NotNull] ParseResult<T> Parse([SpecialNullHandling] string text);

        /// <summary>
        /// Formats the given value as text according to the rules of this pattern.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>The value formatted according to this pattern.</returns>
        [NotNull] string Format(T value);

        /// <summary>
        /// Formats the given value as text according to the rules of this pattern,
        /// appending to the given <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="builder">The <c>StringBuilder</c> to append to.</param>
        /// <returns>The builder passed in as <paramref name="builder"/>.</returns>
        [NotNull] StringBuilder AppendFormat(T value, [NotNull] StringBuilder builder);
    }
}
