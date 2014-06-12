// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Text;

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Composite pattern which parses by trying several parse patterns in turn, and formats
    /// by calling a delegate (which may have come from another <see cref="IPattern{T}"/> to start with).
    /// </summary>
    internal sealed class CompositePattern<T> : IPartialPattern<T>
    {
        private readonly List<IPartialPattern<T>> parsePatterns;
        private readonly Func<T, IPartialPattern<T>> formatPatternPicker;

        internal CompositePattern(IEnumerable<IPartialPattern<T>> parsePatterns, Func<T, IPartialPattern<T>> formatPatternPicker)
        {
            this.parsePatterns = new List<IPartialPattern<T>>(parsePatterns);
            this.formatPatternPicker = formatPatternPicker;
        }

        public ParseResult<T> Parse(string text)
        {
            foreach (IPattern<T> pattern in parsePatterns)
            {
                ParseResult<T> result = pattern.Parse(text);
                if (result.Success || !result.ContinueAfterErrorWithMultipleFormats)
                {
                    return result;
                }
            }
            return ParseResult<T>.NoMatchingFormat(new ValueCursor(text));
        }

        public string Format(T value)
        {
            return formatPatternPicker(value).Format(value);
        }

        public ParseResult<T> ParsePartial(ValueCursor cursor)
        {
            int index = cursor.Index;
            foreach (IPartialPattern<T> pattern in parsePatterns)
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

        public void FormatPartial(T value, StringBuilder builder)
        {
            formatPatternPicker(value).FormatPartial(value, builder);
        }
    }

}
