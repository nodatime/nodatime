#region Copyright and license information

// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
using NodaTime.TimeZones;
using NUnit.Framework;
using NodaTime.Calendars;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public partial class ZoneTransitionTest
    {
        private Offset threeHours = Offset.Create(3, 0, 0, 0);
        private Offset oneHour = Offset.Create(1, 0, 0, 0);
        private Offset minusOneHour = Offset.Create(-1, 0, 0, 0);
        private Offset minusTwoHours = Offset.Create(-2, 0, 0, 0);

        [Test]
        public void Construct_NullName_Exception()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new ZoneTransition(Instant.UnixEpoch, null, Offset.Zero, Offset.Zero));
        }

        [Test]
        public void Construct_Normal()
        {
            string name = "abc";
            var actual = new ZoneTransition(Instant.UnixEpoch, name, Offset.Zero, Offset.Zero);
            Assert.AreEqual(Instant.UnixEpoch, actual.Instant, "Instant");
            Assert.AreEqual(name, actual.Name, "Name");
            Assert.AreEqual(Offset.Zero, actual.WallOffset, "WallOffset");
            Assert.AreEqual(Offset.Zero, actual.StandardOffset, "StandardOffset");
        }

        [Test]
        public void Construct_BeginningOfTime_Truncated()
        {
            string name = "abc";
            var actual = new ZoneTransition(new Instant(Instant.MinValue.Ticks + oneHour.AsTicks()), name, minusTwoHours, minusTwoHours);
            var expected = new ZoneTransition(new Instant(Instant.MinValue.Ticks + oneHour.AsTicks()), name, minusOneHour, minusOneHour);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Construct_EndOfTime_Truncated()
        {
            string name = "abc";
            var actual = new ZoneTransition(new Instant(Instant.MinValue.Ticks + oneHour.AsTicks()), name, threeHours, threeHours);
            var expected = new ZoneTransition(new Instant(Instant.MinValue.Ticks + oneHour.AsTicks()), name, oneHour, oneHour);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IEquatableIComparable_Tests()
        {
            var value = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var equalValue = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var greaterValue = new ZoneTransition(Instant.MaxValue, "abc", Offset.Zero, Offset.Zero);

            TestHelper.TestEqualsClass(value, equalValue, greaterValue);
            TestHelper.TestCompareToClass(value, equalValue, greaterValue);
        }
    }
}
