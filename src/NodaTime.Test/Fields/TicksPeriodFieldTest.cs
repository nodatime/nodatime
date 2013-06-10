// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class TicksPeriodFieldTest
    {
        [Test]
        public void FieldType()
        {
            Assert.AreEqual(PeriodFieldType.Ticks, TicksPeriodField.Instance.FieldType);
        }

        public void IsSupported()
        {
            Assert.IsTrue(TicksPeriodField.Instance.IsSupported);
        }

        public void IsFixedLength()
        {
            Assert.IsTrue(TicksPeriodField.Instance.IsFixedLength);
        }

        [Test]
        public void UnitTicks()
        {
            Assert.AreEqual(1, TicksPeriodField.Instance.UnitTicks);
        }

        [Test]
        public void Add_Int32Value()
        {
            Assert.AreEqual(567L, TicksPeriodField.Instance.Add(new LocalInstant(567L), 0).Ticks);
            Assert.AreEqual(567L + 1234L, TicksPeriodField.Instance.Add(new LocalInstant(567L), 1234).Ticks);
            Assert.AreEqual(567L - 1234L, TicksPeriodField.Instance.Add(new LocalInstant(567L), -1234).Ticks);
        }

        [Test]
        public void Add_Int32Value_Overflows()
        {
            Assert.Throws<OverflowException>(() => TicksPeriodField.Instance.Add(new LocalInstant(long.MaxValue), 1));
        }

        [Test]
        public void Add_Int64Value()
        {
            Assert.AreEqual(567L, TicksPeriodField.Instance.Add(new LocalInstant(567L), 0L).Ticks);
            Assert.AreEqual(567L + 1234L, TicksPeriodField.Instance.Add(new LocalInstant(567L), 1234L).Ticks);
            Assert.AreEqual(567L - 1234L, TicksPeriodField.Instance.Add(new LocalInstant(567L), -1234L).Ticks);
        }

        [Test]
        public void Add_Int64Value_Overflows()
        {
            Assert.Throws<OverflowException>(() => TicksPeriodField.Instance.Add(new LocalInstant(long.MaxValue), 1L));
        }

        [Test]
        public void GetInt64Difference()
        {
            Assert.AreEqual(567L, TicksPeriodField.Instance.GetInt64Difference(new LocalInstant(567L), new LocalInstant(0L)));
            Assert.AreEqual(567L - 1234L, TicksPeriodField.Instance.GetInt64Difference(new LocalInstant(567L), new LocalInstant(1234L)));
            Assert.AreEqual(567L + 1234L, TicksPeriodField.Instance.GetInt64Difference(new LocalInstant(567L), new LocalInstant(-1234L)));
            Assert.AreEqual(long.MaxValue - 1, TicksPeriodField.Instance.GetInt64Difference(new LocalInstant(long.MaxValue), new LocalInstant(1L)));
        }

        [Test]
        public void GetInt64Difference_Overflows()
        {
            Assert.Throws<OverflowException>(() => TicksPeriodField.Instance.GetInt64Difference(new LocalInstant(long.MaxValue), new LocalInstant(-1L)));
        }
    }
}