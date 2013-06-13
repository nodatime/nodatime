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
        private static readonly IPeriodField Instance = new TicksPeriodField();

        [Test]
        public void Add_Int64Value()
        {
            Assert.AreEqual(567L, Instance.Add(new LocalInstant(567L), 0L).Ticks);
            Assert.AreEqual(567L + 1234L, Instance.Add(new LocalInstant(567L), 1234L).Ticks);
            Assert.AreEqual(567L - 1234L, Instance.Add(new LocalInstant(567L), -1234L).Ticks);
        }

        [Test]
        public void Add_Int64Value_Overflows()
        {
            Assert.Throws<OverflowException>(() => Instance.Add(new LocalInstant(long.MaxValue), 1L));
        }

        [Test]
        public void Subtract()
        {
            Assert.AreEqual(567L, Instance.Subtract(new LocalInstant(567L), new LocalInstant(0L)));
            Assert.AreEqual(567L - 1234L, Instance.Subtract(new LocalInstant(567L), new LocalInstant(1234L)));
            Assert.AreEqual(567L + 1234L, Instance.Subtract(new LocalInstant(567L), new LocalInstant(-1234L)));
            Assert.AreEqual(long.MaxValue - 1, Instance.Subtract(new LocalInstant(long.MaxValue), new LocalInstant(1L)));
        }

        [Test]
        public void GetInt64Difference_Overflows()
        {
            Assert.Throws<OverflowException>(() => Instance.Subtract(new LocalInstant(long.MaxValue), new LocalInstant(-1L)));
        }
    }
}