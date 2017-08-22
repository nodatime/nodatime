// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Globalization;
using System.Text;
using NodaTime.Annotations;
using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using NodaTime.Utility;
using JetBrains.Annotations;

namespace NodaTime.Text
{
    /// <summary>
    /// Represents a pattern for parsing and formatting <see cref="Instant"/> values.
    /// </summary>
    /// <threadsafety>
    /// When used with a read-only <see cref="CultureInfo" />, this type is immutable and instances
    /// may be shared freely between threads. We recommend only using read-only cultures for patterns, although this is
    /// not currently enforced.
    /// </threadsafety>
    [Immutable] // Well, assuming an immutable culture...
    public sealed class InstantPattern : IPattern<Instant>
    {
        /// <summary>
        /// Gets the general pattern, which always uses an invariant culture. The general pattern represents
        /// an instant as a UTC date/time in ISO-8601 style "uuuu-MM-ddTHH:mm:ss'Z'".
        /// </summary>
        /// <value>The general pattern, which always uses an invariant culture.</value>
        [NotNull] public static InstantPattern General => Patterns.GeneralPatternImpl;

        /// <summary>
        /// Gets an invariant instant pattern which is ISO-8601 compatible, providing up to 9 decimal places
        /// of sub-second accuracy. (These digits are omitted when unnecessary.)
        /// This corresponds to the text pattern "uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFF'Z'".
        /// </summary>
        /// <value>An invariant instant pattern which is ISO-8601 compatible, providing up to 9 decimal places
        /// of sub-second accuracy.</value>
        [NotNull] public static InstantPattern ExtendedIso => Patterns.ExtendedIsoPatternImpl;

        private const string DefaultFormatPattern = "g";

        internal static readonly PatternBclSupport<Instant> BclSupport = new PatternBclSupport<Instant>(DefaultFormatPattern, fi => fi.InstantPatternParser);

        /// <summary>
        /// Class whose existence is solely to avoid type initialization order issues, most of which stem
        /// from needing NodaFormatInfo.InvariantInfo...
        /// </summary>
        private static class Patterns
        {
            internal static readonly InstantPattern ExtendedIsoPatternImpl = CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFF'Z'");
            internal static readonly InstantPattern GeneralPatternImpl = CreateWithInvariantCulture("uuuu-MM-ddTHH:mm:ss'Z'");
        }

        private readonly IPattern<Instant> pattern;

        /// <summary>
        /// Gets the pattern text for this pattern, as supplied on creation.
        /// </summary>
        /// <value>The pattern text for this pattern, as supplied on creation.</value>
        [NotNull] public string PatternText { get; }

        private InstantPattern(string patternText, IPattern<Instant> pattern)
        {
            PatternText = patternText;
            this.pattern = pattern;
        }

        /// <summary>
        /// Parses the given text value according to the rules of this pattern.
        /// </summary>
        /// <remarks>
        /// This method never throws an exception (barring a bug in Noda Time itself). Even errors such as
        /// the argument being null are wrapped in a parse result.
        /// </remarks>
        /// <param name="text">The text value to parse.</param>
        /// <returns>The result of parsing, which may be successful or unsuccessful.</returns>
        [NotNull] public ParseResult<Instant> Parse([SpecialNullHandling] string text) => pattern.Parse(text);

        /// <summary>
        /// Formats the given instant as text according to the rules of this pattern.
        /// </summary>
        /// <param name="value">The instant to format.</param>
        /// <returns>The instant formatted according to this pattern.</returns>
        [NotNull] public string Format(Instant value) => pattern.Format(value);

        /// <summary>
        /// Formats the given value as text according to the rules of this pattern,
        /// appending to the given <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="builder">The <c>StringBuilder</c> to append to.</param>
        /// <returns>The builder passed in as <paramref name="builder"/>.</returns>
        [NotNull] public StringBuilder AppendFormat(Instant value, [NotNull] StringBuilder builder) => pattern.AppendFormat(value, builder);

        /// <summary>
        /// Creates a pattern for the given pattern text and format info.
        /// </summary>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="formatInfo">The format info to use in the pattern</param>
        /// <returns>A pattern for parsing and formatting instants.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        private static InstantPattern Create([NotNull] string patternText, [NotNull] NodaFormatInfo formatInfo)
        {
            Preconditions.CheckNotNull(patternText, nameof(patternText));
            Preconditions.CheckNotNull(formatInfo, nameof(formatInfo));
            var pattern = formatInfo.InstantPatternParser.ParsePattern(patternText);
            return new InstantPattern(patternText, pattern);
        }

        /// <summary>
        /// Creates a pattern for the given pattern text and culture.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="cultureInfo">The culture to use in the pattern</param>
        /// <returns>A pattern for parsing and formatting instants.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        [NotNull] public static InstantPattern Create([NotNull] string patternText, [NotNull] CultureInfo cultureInfo) =>
            Create(patternText, NodaFormatInfo.GetFormatInfo(cultureInfo));

        /// <summary>
        /// Creates a pattern for the given pattern text in the current thread's current culture.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options. Note that the current culture
        /// is captured at the time this method is called - it is not captured at the point of parsing
        /// or formatting values.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <returns>A pattern for parsing and formatting instants.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        [NotNull] public static InstantPattern CreateWithCurrentCulture([NotNull] string patternText) =>
            Create(patternText, NodaFormatInfo.CurrentInfo);

        /// <summary>
        /// Creates a pattern for the given pattern text in the invariant culture.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <returns>A pattern for parsing and formatting instants.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        [NotNull] public static InstantPattern CreateWithInvariantCulture([NotNull] string patternText) =>
            Create(patternText, NodaFormatInfo.InvariantInfo);

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// localization information.
        /// </summary>
        /// <param name="formatInfo">The localization information to use in the new pattern.</param>
        /// <returns>A new pattern with the given localization information.</returns>
        private InstantPattern WithFormatInfo([NotNull] NodaFormatInfo formatInfo) => Create(PatternText, formatInfo);

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// culture.
        /// </summary>
        /// <param name="cultureInfo">The culture to use in the new pattern.</param>
        /// <returns>A new pattern with the given culture.</returns>
        [NotNull] public InstantPattern WithCulture([NotNull] CultureInfo cultureInfo) =>
            WithFormatInfo(NodaFormatInfo.GetFormatInfo(cultureInfo));
    }
}
