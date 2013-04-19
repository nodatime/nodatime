// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime;
using NodaTime.TzdbCompiler.Tzdb;
using NUnit.Framework;
using System;

namespace ZoneInfoCompiler.Test.Tzdb
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
