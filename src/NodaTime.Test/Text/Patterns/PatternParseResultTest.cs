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
using NUnit.Framework;
using NodaTime.Text;
using NodaTime.Text.Patterns;

namespace NodaTime.Test.Text.Patterns
{
    [TestFixture]
    public class PatternParseResultTest
    {
        [Test]
        public void Success()
        {
            DummyPattern pattern = new DummyPattern();
            PatternParseResult<int> parseResult = PatternParseResult<int>.ForValue(pattern);
            Assert.IsTrue(parseResult.Success);
            Assert.AreSame(pattern, parseResult.GetResultOrThrow());

            IPattern<int> result;
            Assert.IsTrue(parseResult.TryGetResult(out result));
            Assert.AreSame(pattern, result);
        }

        [Test]
        public void ToParseResult_InvalidForSuccess()
        {
            DummyPattern pattern = new DummyPattern();
            PatternParseResult<int> parseResult = PatternParseResult<int>.ForValue(pattern);
            Assert.Throws<InvalidOperationException>(() => parseResult.ToParseResult());
        }

        private class DummyPattern : IPattern<int>
        {
            public ParseResult<int> Parse(string text)
            {
                return ParseResult<int>.ForValue(5);
            }

            public string Format(int value)
            {
                return value.ToString();
            }
        }

    }
}
