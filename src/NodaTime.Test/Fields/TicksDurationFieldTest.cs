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
using NodaTime.Fields;
using NUnit.Framework;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class TicksDurationFieldTest
    {
        [Test]
        public void FieldType()
        {
            Assert.AreEqual(DurationFieldType.Ticks, TicksDurationField.Instance.FieldType);
        }

        public void IsSupported()
        {
            Assert.IsTrue(TicksDurationField.Instance.IsSupported);
        }

        public void IsPrecise()
        {
            Assert.IsTrue(TicksDurationField.Instance.IsPrecise);
        }

        [Test]
        public void UnitTicks()
        {
            Assert.AreEqual(1, TicksDurationField.Instance.UnitTicks);
        }

        [Test]
        public void GetInt64Value()
        {
            Assert.AreEqual(0L, TicksDurationField.Instance.GetInt64Value(new Duration(0L)));
            Assert.AreEqual(1234L, TicksDurationField.Instance.GetInt64Value(new Duration(1234L)));
            Assert.AreEqual(-1234L, TicksDurationField.Instance.GetInt64Value(new Duration(-1234L)));
            Assert.AreEqual(int.MaxValue + 1L, TicksDurationField.Instance.GetInt64Value(new Duration(int.MaxValue + 1L)));
        }

        [Test]
        public void GetInt32Value()
        {
            Assert.AreEqual(0, TicksDurationField.Instance.GetValue(new Duration(0L)));
            Assert.AreEqual(1234, TicksDurationField.Instance.GetValue(new Duration(1234L)));
            Assert.AreEqual(-1234, TicksDurationField.Instance.GetValue(new Duration(-1234L)));
        }

        [Test]
        public void GetInt64Value_WithLocalInstant()
        {
            LocalInstant when = new LocalInstant(56789L);
            Assert.AreEqual(0L, TicksDurationField.Instance.GetInt64Value(new Duration(0L), when));
            Assert.AreEqual(1234L, TicksDurationField.Instance.GetInt64Value(new Duration(1234L), when));
            Assert.AreEqual(-1234L, TicksDurationField.Instance.GetInt64Value(new Duration(-1234L), when));
            Assert.AreEqual(int.MaxValue + 1L, TicksDurationField.Instance.GetInt64Value(new Duration(int.MaxValue + 1L), when));
        }

        [Test]
        public void GetInt32Value_Overflows()
        {
            Assert.Throws<OverflowException>(() => TicksDurationField.Instance.GetValue(new Duration(int.MaxValue + 1L)));
        }

        [Test]
        public void GetDuration()
        {
            Assert.AreEqual(0L, TicksDurationField.Instance.GetDuration(0).Ticks);
            Assert.AreEqual(1234L, TicksDurationField.Instance.GetDuration(1234).Ticks);
            Assert.AreEqual(-1234L, TicksDurationField.Instance.GetDuration(-1234).Ticks);
        }

        [Test]
        public void GetDuration_WithLocalInstant()
        {
            LocalInstant when = new LocalInstant(56789L);
            Assert.AreEqual(0L, TicksDurationField.Instance.GetDuration(0, when).Ticks);
            Assert.AreEqual(1234L, TicksDurationField.Instance.GetDuration(1234, when).Ticks);
            Assert.AreEqual(-1234L, TicksDurationField.Instance.GetDuration(-1234, when).Ticks);
        }

        [Test]
        public void Add_Int32Value()
        {
            Assert.AreEqual(567L, TicksDurationField.Instance.Add(new LocalInstant(567L), 0).Ticks);
            Assert.AreEqual(567L + 1234L, TicksDurationField.Instance.Add(new LocalInstant(567L), 1234).Ticks);
            Assert.AreEqual(567L - 1234L, TicksDurationField.Instance.Add(new LocalInstant(567L), -1234).Ticks);
        }

        [Test]
        public void Add_Int32Value_Overflows()
        {
            Assert.Throws<OverflowException>(() =>
                TicksDurationField.Instance.Add(new LocalInstant(long.MaxValue), 1));
        }

        [Test]
        public void Add_Int64Value()
        {
            Assert.AreEqual(567L, TicksDurationField.Instance.Add(new LocalInstant(567L), 0L).Ticks);
            Assert.AreEqual(567L + 1234L, TicksDurationField.Instance.Add(new LocalInstant(567L), 1234L).Ticks);
            Assert.AreEqual(567L - 1234L, TicksDurationField.Instance.Add(new LocalInstant(567L), -1234L).Ticks);
        }

        [Test]
        public void Add_Int64Value_Overflows()
        {
            Assert.Throws<OverflowException>(() =>
                TicksDurationField.Instance.Add(new LocalInstant(long.MaxValue), 1L));
        }

        [Test]
        public void GetInt32Difference()
        {
            Assert.AreEqual(567, TicksDurationField.Instance.GetDifference(new LocalInstant(567L), new LocalInstant(0L)));
            Assert.AreEqual(567 - 1234, TicksDurationField.Instance.GetDifference(new LocalInstant(567L), new LocalInstant(1234L)));
            Assert.AreEqual(567 + 1234, TicksDurationField.Instance.GetDifference(new LocalInstant(567L), new LocalInstant(-1234L)));
        }

        [Test]
        public void GetInt32Difference_Overflows()
        {
            Assert.Throws<OverflowException>(() =>
                TicksDurationField.Instance.GetDifference(new LocalInstant(long.MaxValue), new LocalInstant(1L)));
        }

        [Test]
        public void GetInt64Difference()
        {
            Assert.AreEqual(567L, TicksDurationField.Instance.GetInt64Difference(new LocalInstant(567L), new LocalInstant(0L)));
            Assert.AreEqual(567L - 1234L, TicksDurationField.Instance.GetInt64Difference(new LocalInstant(567L), new LocalInstant(1234L)));
            Assert.AreEqual(567L + 1234L, TicksDurationField.Instance.GetInt64Difference(new LocalInstant(567L), new LocalInstant(-1234L)));
            Assert.AreEqual(long.MaxValue - 1, TicksDurationField.Instance.GetInt64Difference(new LocalInstant(long.MaxValue), new LocalInstant(1L)));
        }

        [Test]
        public void GetInt64Difference_Overflows()
        {
            Assert.Throws<OverflowException>(() =>
                TicksDurationField.Instance.GetInt64Difference(new LocalInstant(long.MaxValue), new LocalInstant(-1L)));
        }
    }
}
