// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Simple implementation of <see cref="IPattern{T}"/> which simply calls delegates for
    /// parsing and formatting.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class SimplePattern<T> : IPattern<T>
    {
        private readonly NodaFunc<string, ParseResult<T>> parser;
        private readonly NodaFunc<T, string> formatter;

        internal SimplePattern(NodaFunc<string, ParseResult<T>> parser, NodaFunc<T, string> formatter)
        {
            this.parser = parser;
            this.formatter = formatter;
        }

        public ParseResult<T> Parse(string text)
        {
            return parser(text);
        }

        public string Format(T value)
        {
            return formatter(value);
        }
    }
}