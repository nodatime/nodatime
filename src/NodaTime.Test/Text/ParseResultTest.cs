#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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

using NUnit.Framework;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public class ParseResultTest
    {
        [Test]
        public void Value_Success()
        {
            ParseResult<int> result = ParseResult<int>.ForValue(5);
            Assert.AreEqual(5, result.Value);
        }

        [Test]
        public void Value_Failure()
        {
            ParseResult<int> result = ParseResult<int>.ForInvalidValue("text");
            Assert.Throws<UnparsableValueException>(() => result.Value.GetHashCode());
        }

        [Test]
        public void GetValueOrThrow_Success()
        {
            ParseResult<int> result = ParseResult<int>.ForValue(5);
            Assert.AreEqual(5, result.GetResultOrThrow());
        }

        [Test]
        public void GetValueOrThrow_Failure()
        {
            ParseResult<int> result = ParseResult<int>.ForInvalidValue("text");
            Assert.Throws<UnparsableValueException>(() => result.GetResultOrThrow());
        }

        [Test]
        public void TryGetValue_Success()
        {
            ParseResult<int> result = ParseResult<int>.ForValue(5);
            int actual;
            Assert.IsTrue(result.TryGetResult(-1, out actual));
            Assert.AreEqual(5, actual);
        }

        [Test]
        public void TryGetValue_Failure()
        {
            ParseResult<int> result = ParseResult<int>.ForInvalidValue("text");
            int actual;
            Assert.IsFalse(result.TryGetResult(-1, out actual));
            Assert.AreEqual(-1, actual);
        }
    }
}
