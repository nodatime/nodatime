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

namespace NodaTime.Test
{
    [TestFixture]
    public class IntervalTest
    {
        private static readonly Instant SampleStart = new Instant(-300);
        private static readonly Instant SampleEnd = new Instant(400);

        [Test]
        public void Construction_Success()
        {
            var interval = new Interval(SampleStart, SampleEnd);
            Assert.AreEqual(SampleStart, interval.Start);
            Assert.AreEqual(SampleEnd, interval.End);
        }

        [Test]
        public void Construction_EqualStartAndEnd()
        {
            var interval = new Interval(SampleStart, SampleStart);
            Assert.AreEqual(SampleStart, interval.Start);
            Assert.AreEqual(SampleStart, interval.End);
            Assert.AreEqual(new Duration(0), interval.Duration);
        }

        [Test]
        public void Construction_EndBeforeStart()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Interval(SampleEnd, SampleStart));
        }

        [Test]
        public void Equals()
        {
            TestHelper.TestEqualsStruct(
                new Interval(SampleStart, SampleEnd),
                new Interval(SampleStart, SampleEnd),
                new Interval(Instant.UnixEpoch, SampleEnd));
        }

        [Test]
        public void Operators()
        {
            TestHelper.TestOperatorEquality(
                new Interval(SampleStart, SampleEnd),
                new Interval(SampleStart, SampleEnd),
                new Interval(Instant.UnixEpoch, SampleEnd));
        }

        [Test]
        public void Duration()
        {
            var interval = new Interval(SampleStart, SampleEnd);
            Assert.AreEqual(new Duration(700), interval.Duration);
        }
    }
}
