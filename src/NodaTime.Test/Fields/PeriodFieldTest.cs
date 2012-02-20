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
using System.Linq;
using NUnit.Framework;
using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class PeriodFieldTest
    {
        [Test]
        public void Constructor_WithInvalidType_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new StubPeriodField((PeriodFieldType)(-1)));
        }

        [Test]
        public void Constructor_WithValidType_RemembersType()
        {
            PeriodField field = new StubPeriodField(PeriodFieldType.HalfDays);
            Assert.AreEqual(PeriodFieldType.HalfDays, field.FieldType);
        }

        [Test]
        public void IsSupported_ReturnsTrue()
        {
            PeriodField field = new StubPeriodField();
            Assert.IsTrue(field.IsSupported);
        }

        [Test]
        public void GetDurationWithoutLocalInstant_UsesUnitTicks()
        {
            PeriodField field = new StubPeriodField();
            Assert.AreEqual(1230L, field.GetDuration(10).TotalTicks);
        }

        [Test]
        public void GetDurationWithoutLocalInstant_ThrowsOnOverflow()
        {
            PeriodField field = new StubPeriodField();
            Assert.Throws<OverflowException>(() => field.GetDuration(long.MaxValue));
        }

        [Test]
        public void GetValueWithoutLocalInstant_UsesUnitTicks()
        {
            PeriodField field = new StubPeriodField();
            Assert.AreEqual(9, field.GetValue(new Duration(1200L)));
        }

        [Test]
        public void GetValueWithoutLocalInstant_ThrowsOnOverflow()
        {
            PeriodField field = new StubPeriodField();
            Assert.Throws<OverflowException>(() => field.GetValue(new Duration(long.MaxValue)));
        }

        [Test]
        public void GetInt64ValueWithoutLocalInstant_UsesUnitTicks()
        {
            PeriodField field = new StubPeriodField();
            Assert.AreEqual(long.MaxValue / 123, field.GetInt64Value(new Duration(long.MaxValue)));
        }

        [Test]
        public void IsTypeValid_AllEnumValues_AreValid()
        {
            foreach (PeriodFieldType type in Enum.GetValues(typeof(PeriodFieldType)))
            {
                Assert.IsTrue(PeriodField.IsTypeValid(type));
            }
        }

        [Test]
        public void IsTypeValid_ValuesOutOfRange_AreInvalid()
        {
            Assert.IsFalse(PeriodField.IsTypeValid((PeriodFieldType)(-1)));
            PeriodFieldType max = Enum.GetValues(typeof(PeriodFieldType)).Cast<PeriodFieldType>().Max();
            Assert.IsFalse(PeriodField.IsTypeValid(max + 1));
        }

        private class StubPeriodField : PeriodField
        {
            internal StubPeriodField() : this(PeriodFieldType.Seconds)
            {
            }

            internal StubPeriodField(PeriodFieldType fieldType) : base(fieldType, 123, true, true)
            {
            }

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