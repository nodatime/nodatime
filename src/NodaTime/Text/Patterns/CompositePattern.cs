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

using System.Collections.Generic;

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Composite pattern which parses by trying several parse patterns in turn, and formats
    /// by calling a delegate (which may have come from another <see cref="IPattern{T}"/> to start with).
    /// </summary>
    internal class CompositePattern<T> : IPattern<T>
    {
        private List<IPattern<T>> parsePatterns;
        private NodaFunc<T, string> formatter;

        internal CompositePattern(IEnumerable<IPattern<T>> parsePatterns, IPattern<T> formatPattern)
        {
            this.parsePatterns = new List<IPattern<T>>(parsePatterns);
            this.formatter = formatPattern.Format;
        }

        internal CompositePattern(IEnumerable<IPattern<T>> parsePatterns, NodaFunc<T, string> formatter)
        {
            this.parsePatterns = new List<IPattern<T>>(parsePatterns);
            this.formatter = formatter;
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
            return ParseResult<T>.NoMatchingFormat;
        }

        public string Format(T value)
        {
            return formatter(value);
        }
    }

}
