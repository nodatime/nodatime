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
            Assert.Throws<ArgumentOutOfRangeException>(() => new StubDurationField((DurationFieldType)(-1)));
        }

        [Test]
        public void Constructor_WithValidType_RemembersType()
        {
            DurationField field = new StubDurationField(DurationFieldType.HalfDays);
            Assert.AreEqual(DurationFieldType.HalfDays, field.FieldType);
        }

        [Test]
        public void IsSupported_ReturnsTrue()
        {
            DurationField field = new StubDurationField();
            Assert.IsTrue(field.IsSupported);
        }

        [Test]
        public void GetDurationWithoutLocalInstant_UsesUnitTicks()
        {
            DurationField field = new StubDurationField();
            Assert.AreEqual(1230L, field.GetDuration(10).Ticks);
        }

        [Test]
        public void GetDurationWithoutLocalInstant_ThrowsOnOverflow()
        {
            DurationField field = new StubDurationField();
            Assert.Throws<OverflowException>(() => field.GetDuration(long.MaxValue));
        }

        [Test]
        public void GetValueWithoutLocalInstant_UsesUnitTicks()
        {
            DurationField field = new StubDurationField();
            Assert.AreEqual(9, field.GetValue(new Duration(1200L)));
        }

        [Test]
        public void GetValueWithoutLocalInstant_ThrowsOnOverflow()
        {
            DurationField field = new StubDurationField();
            Assert.Throws<OverflowException>(() => field.GetValue(new Duration(long.MaxValue)));
        }

        [Test]
        public void GetInt64ValueWithoutLocalInstant_UsesUnitTicks()
        {
            DurationField field = new StubDurationField();
            Assert.AreEqual(long.MaxValue / 123, field.GetInt64Value(new Duration(long.MaxValue)));
        }

        [Test]
        public void IsTypeValid_AllEnumValues_AreValid()
        {
            foreach (DurationFieldType type in Enum.GetValues(typeof(DurationFieldType)))
            {
                Assert.IsTrue(DurationField.IsTypeValid(type));
            }
        }

        [Test]
        public void IsTypeValid_ValuesOutOfRange_AreInvalid()
        {
            Assert.IsFalse(DurationField.IsTypeValid((DurationFieldType)(-1)));
            DurationFieldType max = Enum.GetValues(typeof(DurationFieldType)).Cast<DurationFieldType>().Max();
            Assert.IsFalse(DurationField.IsTypeValid(max + 1));
        }

        private class StubDurationField : DurationField
        {
            internal StubDurationField() : base(DurationFieldType.Seconds)
            {
            }

            internal StubDurationField(DurationFieldType fieldType) : base(fieldType)
            {
            }

            internal override bool IsSupported { get { return true; } }

            internal override bool IsPrecise { get { return true; } }

            internal override long UnitTicks { get { return 123; } }

            internal override long GetInt64Value(Duration duration, LocalInstant localInstant)
            {
                throw new NotImplementedException();
            }

            internal override Duration GetDuration(long value, LocalInstant localInstant)
            {
                throw new NotImplementedException();
            }

            internal override LocalInstant Add(LocalInstant localInstant, int value)
            {
                return new LocalInstant(localInstant.Ticks + value * UnitTicks);
            }

            internal override LocalInstant Add(LocalInstant localInstant, long value)
            {
                return new LocalInstant(localInstant.Ticks + value * UnitTicks);
            }

            internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
            {
                throw new NotImplementedException();
            }
        }
    }
}