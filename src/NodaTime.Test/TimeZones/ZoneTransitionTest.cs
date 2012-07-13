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
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class ZoneTransitionTest
    {
        [Test]
        public void Construct_NullName_Exception()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new ZoneTransition(NodaConstants.UnixEpoch, null, Offset.Zero, Offset.Zero));
        }

        [Test]
        public void Construct_Normal()
        {
            const string name = "abc";
            var actual = new ZoneTransition(NodaConstants.UnixEpoch, name, Offset.Zero, Offset.Zero);
            Assert.AreEqual(NodaConstants.UnixEpoch, actual.Instant, "Instant");
            Assert.AreEqual(name, actual.Name, "GetName");
            Assert.AreEqual(Offset.Zero, actual.WallOffset, "WallOffset");
            Assert.AreEqual(Offset.Zero, actual.StandardOffset, "StandardOffset");
        }

        [Test]
        public void IEquatableIComparable_Tests()
        {
            var value = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var equalValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var greaterValue = new ZoneTransition(Instant.MaxValue, "abc", Offset.Zero, Offset.Zero);

            TestHelper.TestEqualsClass(value, equalValue, greaterValue);
            TestHelper.TestCompareToClass(value, equalValue, greaterValue);
        }

        [Test]
        public void IsTransitionFrom_null_true()
        {
            var value = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            Assert.True(value.IsTransitionFrom(null));
        }

        [Test]
        public void IsTransitionFrom_identity_false()
        {
            var value = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            Assert.False(value.IsTransitionFrom(value));
        }

        [Test]
        public void IsTransitionFrom_equalObject_false()
        {
            var newValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void ISTransitionFrom_unequalStandardOffset_false()
        {
            var newValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.MaxValue, Offset.Zero);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_unequalSavings_false()
        {
            var newValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.MaxValue);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_unequalName_false()
        {
            var newValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(NodaConstants.UnixEpoch, "qwe", Offset.Zero, Offset.Zero);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_earlierInstant_false()
        {
            var newValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(NodaConstants.UnixEpoch + Duration.Epsilon, "abc", Offset.Zero, Offset.Zero);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_earlierInstantAndUnequalStandardOffset_false()
        {
            var newValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(NodaConstants.UnixEpoch + Duration.Epsilon, "abc", Offset.MaxValue, Offset.Zero);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_earlierInstantAndUnequalSavings_false()
        {
            var newValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(NodaConstants.UnixEpoch + Duration.Epsilon, "abc", Offset.Zero, Offset.MaxValue);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_earlierInstantAndUnequalName_false()
        {
            var newValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(NodaConstants.UnixEpoch + Duration.Epsilon, "qwe", Offset.Zero, Offset.Zero);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_laterInstant_false()
        {
            var newValue = new ZoneTransition(NodaConstants.UnixEpoch + Duration.Epsilon, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.Zero);
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_laterInstantAndUnequalStandardOffset_true()
        {
            var newValue = new ZoneTransition(NodaConstants.UnixEpoch + Duration.Epsilon, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.MaxValue, Offset.Zero);
            Assert.True(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_laterInstantAndUnequalSavings_true()
        {
            var newValue = new ZoneTransition(NodaConstants.UnixEpoch + Duration.Epsilon, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.MaxValue);
            Assert.True(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_laterInstantAndEqualButOppositeStandardAndSavings_false()
        {
            var newValue = new ZoneTransition(NodaConstants.UnixEpoch + Duration.Epsilon, "abc", Offset.FromHours(1), Offset.Zero);
            var oldValue = new ZoneTransition(NodaConstants.UnixEpoch, "abc", Offset.Zero, Offset.FromHours(1));
            Assert.False(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_laterInstantAndUnequalName_true()
        {
            var newValue = new ZoneTransition(NodaConstants.UnixEpoch + Duration.Epsilon, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(NodaConstants.UnixEpoch, "qwe", Offset.Zero, Offset.Zero);
            Assert.True(newValue.IsTransitionFrom(oldValue));
        }

        [Test]
        public void IsTransitionFrom_laterInstantAndUnequalNameAndSavings_true()
        {
            var newValue = new ZoneTransition(NodaConstants.UnixEpoch + Duration.Epsilon, "abc", Offset.Zero, Offset.Zero);
            var oldValue = new ZoneTransition(NodaConstants.UnixEpoch, "qwe", Offset.Zero, Offset.MaxValue);
            Assert.True(newValue.IsTransitionFrom(oldValue));
        }
    }
}
