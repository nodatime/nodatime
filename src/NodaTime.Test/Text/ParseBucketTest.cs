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

using NUnit.Framework;
using NodaTime.Text;
using NodaTime.Text.Patterns;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public class ParseBucketTest
    {
        [Test]
        public void IsUsed_NoMatch()
        {
            Assert.IsFalse(ParseBucket<Offset>.IsFieldUsed(PatternFields.Hours12 | PatternFields.Minutes, PatternFields.Hours24));
        }

        [Test]
        public void IsUsed_SingleValueMatch()
        {
            Assert.IsTrue(ParseBucket<Offset>.IsFieldUsed(PatternFields.Hours24, PatternFields.Hours24));
        }

        [Test]
        public void IsFieldUsed_MultiValueMatch()
        {
            Assert.IsTrue(ParseBucket<Offset>.IsFieldUsed(PatternFields.Hours24 | PatternFields.Minutes, PatternFields.Hours24));
        }

        [Test]
        public void AllAreUsed_NoMatch()
        {
            Assert.IsFalse(ParseBucket<Offset>.AreAllFieldsUsed(PatternFields.Hours12 | PatternFields.Minutes,
                                                                PatternFields.Hours24 | PatternFields.Seconds));
        }

        [Test]
        public void AllAreUsed_PartialMatch()
        {
            Assert.IsFalse(ParseBucket<Offset>.AreAllFieldsUsed(PatternFields.Hours12 | PatternFields.Minutes,
                                                                PatternFields.Hours12 | PatternFields.Seconds));
        }

        [Test]
        public void AllAreUsed_CompleteMatch()
        {
            Assert.IsTrue(ParseBucket<Offset>.AreAllFieldsUsed(PatternFields.Hours12 | PatternFields.Minutes,
                                                               PatternFields.Hours12 | PatternFields.Minutes));
        }

        [Test]
        public void AllAreUsed_CompleteMatchWithMore()
        {
            Assert.IsTrue(ParseBucket<Offset>.IsFieldUsed(PatternFields.Hours24 | PatternFields.Minutes | PatternFields.Hours12,
                                                          PatternFields.Hours24 | PatternFields.Minutes));
        }
    }
}
