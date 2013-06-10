// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class ScaledDurationTest
    {
        private readonly ScaledPeriodField sample = new ScaledPeriodField(TicksPeriodField.Instance, PeriodFieldType.Minutes, 90);
        private readonly LocalInstant localInstant = new LocalInstant(567L);

        [Test]
        public void Constructor_WithNullField_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ScaledPeriodField(null, PeriodFieldType.Minutes, 10));
        }

        [Test]
        public void Constructor_WithUnsupportedField_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ScaledPeriodField(UnsupportedPeriodField.Milliseconds, PeriodFieldType.Minutes, 10));
        }

        [Test]
        public void Constructor_WithInvalidFieldType_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ScaledPeriodField(TicksPeriodField.Instance, (PeriodFieldType)(-1), 10));
        }

        [Test]
        public void Constructor_WithScaleOfZero_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ScaledPeriodField(TicksPeriodField.Instance, PeriodFieldType.Minutes, 0));
        }

        [Test]
        public void Constructor_WithScaleOfOne_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ScaledPeriodField(TicksPeriodField.Instance, PeriodFieldType.Minutes, 1));
        }

        [Test]
        public void SimpleProperties()
        {
            Assert.AreEqual(PeriodFieldType.Minutes, sample.FieldType);
            Assert.IsTrue(sample.IsSupported);
            Assert.IsTrue(sample.IsFixedLength);
        }

        [Test]
        public void UnitTicks()
        {
            Assert.AreEqual(90, sample.UnitTicks);
        }

        [Test]
        public void GetDuration_WithLocalInstant()
        {
            Assert.AreEqual(0L, sample.GetDuration(0, localInstant).Ticks);
            Assert.AreEqual(1234 * 90L, sample.GetDuration(1234, localInstant).Ticks);
            Assert.AreEqual(-1234 * 90L, sample.GetDuration(-1234, localInstant).Ticks);
            Assert.AreEqual(int.MaxValue * 90L, sample.GetDuration(int.MaxValue, localInstant).Ticks);
        }

        [Test]
        public void GetDuration_WithLocalInstantCausingOverflow()
        {
            Assert.Throws<OverflowException>(() => sample.GetDuration(long.MaxValue, localInstant));
        }

        [Test]
        public void Add_WithInt32()
        {
            Assert.AreEqual(567L, sample.Add(localInstant, 0).Ticks);
            Assert.AreEqual(567L + 1234L * 90L, sample.Add(localInstant, 1234).Ticks);
            Assert.AreEqual(567L - 1234L * 90L, sample.Add(localInstant, -1234).Ticks);
        }

        [Test]
        public void Add_WithInt32CausingOverflow()
        {
            Assert.Throws<OverflowException>(() => sample.Add(new LocalInstant(long.MaxValue), 1));
        }

        [Test]
        public void Add_WithInt64()
        {
            Assert.AreEqual(567L, sample.Add(localInstant, 0L).Ticks);
            Assert.AreEqual(567L + 1234L * 90L, sample.Add(localInstant, 1234L).Ticks);
            Assert.AreEqual(567L - 1234L * 90L, sample.Add(localInstant, -1234L).Ticks);
        }

        [Test]
        public void Add_WithInt64CausingOverflow()
        {
            Assert.Throws<OverflowException>(() => sample.Add(new LocalInstant(long.MaxValue), 1L));
        }

        [Test]
        public void GetDifference()
        {
            Assert.AreEqual(0, sample.GetDifference(new LocalInstant(1L), new LocalInstant(0L)));
            Assert.AreEqual(567, sample.GetDifference(new LocalInstant(567 * 90L), new LocalInstant(0L)));
            Assert.AreEqual(567 - 1234, sample.GetDifference(new LocalInstant(567 * 90L), new LocalInstant(1234 * 90L)));
            Assert.AreEqual(567 + 1234, sample.GetDifference(new LocalInstant(567 * 90L), new LocalInstant(-1234 * 90L)));
        }

        [Test]
        public void GetDifference_WithOverflow()
        {
            Assert.Throws<OverflowException>(() => sample.GetDifference(new LocalInstant(long.MaxValue), new LocalInstant(-1L)));
        }

        [Test]
        public void GetInt64Difference()
        {
            Assert.AreEqual(0L, sample.GetInt64Difference(new LocalInstant(1L), new LocalInstant(0L)));
            Assert.AreEqual(567L, sample.GetInt64Difference(new LocalInstant(567 * 90L), new LocalInstant(0L)));
            Assert.AreEqual(567L - 1234L, sample.GetInt64Difference(new LocalInstant(567 * 90L), new LocalInstant(1234 * 90L)));
            Assert.AreEqual(567L + 1234L, sample.GetInt64Difference(new LocalInstant(567 * 90L), new LocalInstant(-1234 * 90L)));
        }

        [Test]
        public void GetInt64Difference_WithOverflow()
        {
            Assert.Throws<OverflowException>(() => sample.GetInt64Difference(new LocalInstant(long.MaxValue), new LocalInstant(-1L)));
        }
    }
}