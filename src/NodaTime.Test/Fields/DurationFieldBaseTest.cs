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
using System.Linq;
using NodaTime.Fields;
using NUnit.Framework;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class DurationFieldBaseTest
    {
        [Test]
        public void Constructor_WithInvalidType_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new StubDurationFieldBase((DurationFieldType)(-1)));
        }

        [Test]
        public void Constructor_WithValidType_RemembersType()
        {
            DurationFieldBase field = new StubDurationFieldBase(DurationFieldType.HalfDays);
            Assert.AreEqual(DurationFieldType.HalfDays, field.FieldType);
        }

        [Test]
        public void IsSupported_ReturnsTrue()
        {
            DurationFieldBase field = new StubDurationFieldBase();
            Assert.IsTrue(field.IsSupported);
        }

        [Test]
        public void GetDurationWithoutLocalInstant_UsesUnitTicks()
        {
            DurationFieldBase field = new StubDurationFieldBase();
            Assert.AreEqual(1230L, field.GetDuration(10).Ticks);
        }

        [Test]
        public void GetDurationWithoutLocalInstant_ThrowsOnOverflow()
        {
            DurationFieldBase field = new StubDurationFieldBase();
            Assert.Throws<OverflowException>(() => field.GetDuration(long.MaxValue));
        }

        [Test]
        public void GetValueWithoutLocalInstant_UsesUnitTicks()
        {
            DurationFieldBase field = new StubDurationFieldBase();
            Assert.AreEqual(9, field.GetValue(new Duration(1200L)));
        }

        [Test]
        public void GetValueWithoutLocalInstant_ThrowsOnOverflow()
        {
            DurationFieldBase field = new StubDurationFieldBase();
            Assert.Throws<OverflowException>(() => field.GetValue(new Duration(long.MaxValue)));
        }

        [Test]
        public void GetInt64ValueWithoutLocalInstant_UsesUnitTicks()
        {
            DurationFieldBase field = new StubDurationFieldBase();
            Assert.AreEqual(long.MaxValue / 123, field.GetInt64Value(new Duration(long.MaxValue)));
        }

        [Test]
        public void IsTypeValid_AllEnumValues_AreValid()
        {
            foreach (DurationFieldType type in Enum.GetValues(typeof(DurationFieldType)))
            {
                Assert.IsTrue(DurationFieldBase.IsTypeValid(type));
            }
        }

        [Test]
        public void IsTypeValid_ValuesOutOfRange_AreInvalid()
        {
            Assert.IsFalse(DurationFieldBase.IsTypeValid((DurationFieldType)(-1)));
            DurationFieldType max = Enum.GetValues(typeof(DurationFieldType))
                .Cast<DurationFieldType>().Max();
            Assert.IsFalse(DurationFieldBase.IsTypeValid(max + 1));
        }

        private class StubDurationFieldBase : DurationFieldBase
        {
            internal StubDurationFieldBase() : base(DurationFieldType.Seconds)
            {
            }

            internal StubDurationFieldBase(DurationFieldType fieldType)
                : base(fieldType)
            {
            }

            public override bool IsPrecise
            {
                get { return true; }
            }

            public override long UnitTicks
            {
                get { return 123; }
            }

            public override long GetInt64Value(Duration duration, LocalInstant localInstant)
            {
                throw new System.NotImplementedException();
            }

            public override Duration GetDuration(long value, LocalInstant localInstant)
            {
                throw new System.NotImplementedException();
            }

            public override LocalInstant Add(LocalInstant localInstant, int value)
            {
                return new LocalInstant(localInstant.Ticks + value * UnitTicks);
            }

            public override LocalInstant Add(LocalInstant localInstant, long value)
            {
                return new LocalInstant(localInstant.Ticks + value * UnitTicks);
            }

            public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
