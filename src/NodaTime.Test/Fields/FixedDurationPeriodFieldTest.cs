// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class FixedDurationPeriodFieldTest
    {
        // Just a sample really.
        private static readonly IPeriodField SampleField = FixedDurationPeriodField.Milliseconds;

        [Test]
        public void Add()
        {
            LocalInstant when = new LocalInstant(567L);
            Assert.AreEqual(567L, SampleField.Add(when, 0L).Ticks);
            Assert.AreEqual(567L + 12340000L, SampleField.Add(when, 1234L).Ticks);
            Assert.AreEqual(567L - 12340000L, SampleField.Add(when, -1234L).Ticks);
        }

        [Test]
        public void Add_ThrowsOnOverflow()
        {
            Assert.Throws<OverflowException>(() => SampleField.Add(new LocalInstant(long.MaxValue), 1));
            Assert.Throws<OverflowException>(() => SampleField.Add(new LocalInstant(0), long.MaxValue));
        }

        [Test]
        public void Subtract()
        {
            Assert.AreEqual(0L, SampleField.Subtract(new LocalInstant(1), new LocalInstant(0)));
            Assert.AreEqual(567L, SampleField.Subtract(new LocalInstant(5670000L), new LocalInstant(0)));
            Assert.AreEqual(567L - 1234L, SampleField.Subtract(new LocalInstant(5670000L), new LocalInstant(12340000L)));
            Assert.AreEqual(567L + 1234L, SampleField.Subtract(new LocalInstant(5670000L), new LocalInstant(-12340000L)));
            Assert.AreEqual(int.MaxValue + 1L, SampleField.Subtract(new LocalInstant(int.MaxValue * 10000L), new LocalInstant(-10000L)));
        }

        [Test]
        public void Subtract_ThrowsOnOverflow()
        {
            Assert.Throws<OverflowException>(() => FixedDurationPeriodField.Ticks.Subtract(new LocalInstant(long.MaxValue), new LocalInstant(-1L)));
        }

        [Test]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public void Add_OverflowOnMultiplyOnDays(int days)
        {
            Assert.Throws<OverflowException>(() => FixedDurationPeriodField.Days.Add(new LocalInstant(0L), days));
        }

        [Test]
        public void Add_OverflowOnAddOrSubtract()
        {
            Assert.Throws<OverflowException>(() => FixedDurationPeriodField.Ticks.Add(new LocalInstant(long.MaxValue), 1L));
            Assert.Throws<OverflowException>(() => FixedDurationPeriodField.Ticks.Add(new LocalInstant(long.MinValue), -1L));
        }

        [Test]
        public void Add_MinLongTicks()
        {
            // Without a bit of care, this would fail.
            long minTicks = FixedDurationPeriodField.Ticks.Add(new LocalInstant(0), long.MinValue).Ticks;
            Assert.AreEqual(long.MinValue, minTicks);
        }
    }
}