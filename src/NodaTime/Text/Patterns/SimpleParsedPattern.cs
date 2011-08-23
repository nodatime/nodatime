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

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Simple implementation of <see cref="IParsedPattern{T}"/> which simply calls delegates for
    /// parsing and formatting.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class SimpleParsedPattern<T> : IParsedPattern<T>
    {
        private readonly NodaFunc<string, ParseResult<T>> parser;
        private readonly NodaFunc<T, string> formatter;

        internal SimpleParsedPattern(NodaFunc<string, ParseResult<T>> parser, NodaFunc<T, string> formatter)
        {
            this.parser = parser;
            this.formatter = formatter;
        }

        public ParseResult<T> Parse(string value)
        {
            return parser(value);
        }

        public string Format(T value)
        {
            return formatter(value);
        }
    }
}