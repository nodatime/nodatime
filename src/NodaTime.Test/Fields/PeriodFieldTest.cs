// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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
            Assert.AreEqual(1230L, field.GetDuration(10).Ticks);
        }

        [Test]
        public void GetDurationWithoutLocalInstant_ThrowsOnOverflow()
        {
            PeriodField field = new StubPeriodField();
            Assert.Throws<OverflowException>(() => field.GetDuration(long.MaxValue));
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