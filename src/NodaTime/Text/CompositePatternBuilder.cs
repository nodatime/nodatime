// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodaTime.Text
{
    /// <summary>
    /// A builder for composite patterns.
    /// </summary>
    /// <remarks>
    /// A composite pattern is a combination of multiple patterns. When parsing, these are checked
    /// in the order in which they are added to the builder with the <see cref="Add(IPattern{T}, Func{T, bool})"/>
    /// method, by trying to parse and seeing if the result is a successful one. When formatting,
    /// the patterns are checked in the reverse order, using the predicate provided along with the pattern
    /// when calling <c>Add</c>. The intention is that patterns are added in "most precise first" order,
    /// and the predicate should indicate whether it can fully represent the given value - so the "less precise"
    /// (and therefore usually shorter) pattern can be used first.
    /// </remarks>
    /// <typeparam name="T">The type of value to be parsed or formatted by the resulting pattern.</typeparam>
    /// <threadsafety>
    /// This type is mutable, and should not be used between multiple threads. The patterns created
    /// by the <see cref="Build"/> method are immutable and can be used between multiple threads, assuming
    /// that each component (both pattern and predicate) is also immutable.
    /// </threadsafety>
    [Mutable]
    public sealed class CompositePatternBuilder<T> : IEnumerable<IPattern<T>>
    {
        private readonly List<IPattern<T>> patterns = new List<IPattern<T>>();
        private readonly List<Func<T, bool>> formatPredicates = new List<Func<T, bool>>();

        /// <summary>
        /// Constructs a new instance which initially has no component patterns. At least one component
        /// pattern must be added before <see cref="Build"/> is called.
        /// </summary>
        public CompositePatternBuilder()
        {
        }

        /// <summary>
        /// Adds a component pattern to this builder.
        /// </summary>
        /// <param name="pattern">The component pattern to use as part of the eventual composite pattern.</param>
        /// <param name="formatPredicate">A predicate to determine whether or not this pattern is suitable for
        /// formatting the given value.</param>
        public void Add([NotNull] IPattern<T> pattern, [NotNull] Func<T, bool> formatPredicate)
        {
            patterns.Add(Preconditions.CheckNotNull(pattern, nameof(pattern)));
            formatPredicates.Add(Preconditions.CheckNotNull(formatPredicate, nameof(formatPredicate)));
        }

        /// <summary>
        /// Builds a composite pattern from this builder. Further changes to this builder
        /// will have no impact on the returned pattern.
        /// </summary>
        /// <exception cref="InvalidOperationException">No component patterns have been added.</exception>
        /// <returns>A pattern using the patterns added to this builder.</returns>
        [NotNull] public IPattern<T> Build()
        {
            Preconditions.CheckState(patterns.Count != 0, "A composite pattern must have at least one component pattern.");
            return new CompositePattern(patterns, formatPredicates);
        }

        internal IPartialPattern<T> BuildAsPartial()
        {
            Preconditions.DebugCheckState(patterns.All(p => p is IPartialPattern<T>), "All patterns should be partial");
            return (IPartialPattern<T>) Build();
        }

        /// <inheritdoc />
        IEnumerator<IPattern<T>> IEnumerable<IPattern<T>>.GetEnumerator() => patterns.GetEnumerator();
        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => patterns.GetEnumerator();

        private sealed class CompositePattern : IPartialPattern<T>
        {
            private readonly IPattern<T>[] patterns;
            private readonly Func<T, bool>[] formatPredicates;

            internal CompositePattern(List<IPattern<T>> patterns, List<Func<T, bool>> formatPredicates)
            {
                this.patterns = patterns.ToArray();
                this.formatPredicates = formatPredicates.ToArray();
            }

            public ParseResult<T> Parse([SpecialNullHandling] string text)
            {
                foreach (IPattern<T> pattern in patterns)
                {
                    ParseResult<T> result = pattern.Parse(text);
                    if (result.Success || !result.ContinueAfterErrorWithMultipleFormats)
                    {
                        return result;
                    }
                }
                return ParseResult<T>.NoMatchingFormat(new ValueCursor(text));
            }

            public ParseResult<T> ParsePartial(ValueCursor cursor)
            {
                int index = cursor.Index;
                foreach (IPartialPattern<T> pattern in patterns)
                {
                    cursor.Move(index);
                    ParseResult<T> result = pattern.ParsePartial(cursor);
                    if (result.Success || !result.ContinueAfterErrorWithMultipleFormats)
                    {
                        return result;
                    }
                }
                cursor.Move(index);
                return ParseResult<T>.NoMatchingFormat(cursor);
            }

            public string Format(T value) => FindFormatPattern(value).Format(value);

            public StringBuilder AppendFormat(T value, [NotNull] StringBuilder builder) =>
                FindFormatPattern(value).AppendFormat(value, builder);

            private IPattern<T> FindFormatPattern(T value)
            {
                for (int i = formatPredicates.Length - 1; i >= 0; i--)
                {
                    if (formatPredicates[i](value))
                    {
                        return patterns[i];
                    }
                }
                throw new FormatException("Composite pattern was unable to format value using any of the provided patterns.");
            }
        }
    }
}
