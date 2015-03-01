// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class LocalInstantTest
    {
        [Test]
        public void Equality()
        {
            LocalInstant equal = new LocalInstant(1, 100L);
            LocalInstant different1 = new LocalInstant(1, 200L);
            LocalInstant different2 = new LocalInstant(2, 100L);

            TestHelper.TestEqualsStruct(equal, equal, different1);
            TestHelper.TestOperatorEquality(equal, equal, different1);

            TestHelper.TestEqualsStruct(equal, equal, different2);
            TestHelper.TestOperatorEquality(equal, equal, different2);
        }        

        [Test]
        public void MinusOffset_Zero_IsNeutralElement()
        {
            Instant sampleInstant = new Instant(1, 23456L);
            LocalInstant sampleLocalInstant = new LocalInstant(1, 23456L);
            Assert.AreEqual(sampleInstant, sampleLocalInstant.Minus(Offset.Zero));
            Assert.AreEqual(sampleInstant, sampleLocalInstant.MinusZeroOffset());
        }
    }
}
