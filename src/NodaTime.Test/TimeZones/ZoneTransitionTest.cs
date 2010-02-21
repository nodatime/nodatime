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
            Instant instant = new Instant(Instant.MinValue.Ticks + oneHour.AsTicks());
            var actual = new ZoneTransition(instant, name, minusTwoHours, minusTwoHours);
            Assert.AreEqual(instant, actual.Instant, "Instant");
            Assert.AreEqual(minusOneHour, actual.StandardOffset, "StandardOffset");
            Assert.AreEqual(Offset.Zero, actual.Savings, "Savings");
        }

        [Test]
        public void Construct_EndOfTime_Truncated()
        {
            string name = "abc";
            Instant instant = new Instant(Instant.MaxValue.Ticks + minusOneHour.AsTicks());
            var actual = new ZoneTransition(instant, name, threeHours, threeHours);
            Assert.AreEqual(instant, actual.Instant, "Instant");
            Assert.AreEqual(oneHour, actual.StandardOffset, "StandardOffset");
            Assert.AreEqual(Offset.Zero, actual.Savings, "Savings");
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

        [Test]
        public void IsTransitionFrom_null_true()
        {
            var value = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            Assert.True(value.IsTransitionFrom(null));
        }

        [Test]
        public void IsTransitionFrom_identity_false()
        {
            var value = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            Assert.False(value.IsTransitionFrom(value));
        }

        [Test]
        public void IsTransitionFrom_equalObject_false()
        {
            var newValue = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void ISTransitionFrom_unequalStandardOffset_false()
        {
            var newValue = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.MaxValue, Offset.Zero);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_unequalSavings_false()
        {
            var newValue = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.MaxValue);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_unequalName_false()
        {
            var newValue = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(Instant.UnixEpoch, "qwe", Offset.Zero, Offset.Zero);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_earlierInstant_false()
        {
            var newValue = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(Instant.UnixEpoch + Duration.One, "abc", Offset.Zero, Offset.Zero);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_earlierInstantAndUnequalStandardOffset_false()
        {
            var newValue = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(Instant.UnixEpoch + Duration.One, "abc", Offset.MaxValue, Offset.Zero);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_earlierInstantndUnequalSavings_false()
        {
            var newValue = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(Instant.UnixEpoch + Duration.One, "abc", Offset.Zero, Offset.MaxValue);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_earlierInstantAndUnequalName_false()
        {
            var newValue = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(Instant.UnixEpoch + Duration.One, "qwe", Offset.Zero, Offset.Zero);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_laterInstant_false()
        {
            var newValue = new ZoneTransition(Instant.UnixEpoch + Duration.One, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_laterInstantAndUnequalStandardOffset_false()
        {
            var newValue = new ZoneTransition(Instant.UnixEpoch + Duration.One, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.MaxValue, Offset.Zero);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_laterInstantAndUnequalSavings_true()
        {
            var newValue = new ZoneTransition(Instant.UnixEpoch + Duration.One, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(Instant.UnixEpoch, "abc", Offset.Zero, Offset.MaxValue);
            Assert.True(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_laterInstantAndUnequalName_true()
        {
            var newValue = new ZoneTransition(Instant.UnixEpoch + Duration.One, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(Instant.UnixEpoch, "qwe", Offset.Zero, Offset.Zero);
            Assert.True(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_laterInstantAndUnequalNameAndSavings_true()
        {
            var newValue = new ZoneTransition(Instant.UnixEpoch + Duration.One, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(Instant.UnixEpoch, "qwe", Offset.Zero, Offset.MaxValue);
            Assert.True(newValue.IsTransitionFrom(oldValue));
        }
    }
}
