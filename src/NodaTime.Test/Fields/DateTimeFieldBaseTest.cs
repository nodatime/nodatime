#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
    // TODO: Refactor a lot of these tests: there's a lot of duplication down the hierarchy.
    [TestFixture]
    public class DateTimeFieldBaseTest
    {
        [Test]
        public void Constructor_WithInvalidType_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new StubDateTimeFieldBase((DateTimeFieldType)(-1)));
        }

        [Test]
        public void Constructor_WithValidType_RemembersType()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase(DateTimeFieldType.Weekyear);
            Assert.AreEqual(DateTimeFieldType.Weekyear, field.FieldType);
        }

        [Test]
        public void IsSupported_DefaultsToTrue()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.IsTrue(field.IsSupported);
        }

        // TODO: Isn't this testing the stub code rather than DateTimeFieldBase?
        [Test]
        public void GetValue_OnStub_DividesBy60()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0, field.GetValue(new LocalInstant(0)));
            Assert.AreEqual(1, field.GetValue(new LocalInstant(60)));
            Assert.AreEqual(2, field.GetValue(new LocalInstant(123)));
        }

        [Test]
        public void AddInt32_DelegatesToDurationField()
        {
            MockCountingDurationField.int32Additions = 0;
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(61, field.Add(new LocalInstant(1), 1).Ticks);
            Assert.AreEqual(1, MockCountingDurationField.int32Additions);
        }

        [Test]
        public void AddInt64_DelegatesToDurationField()
        {
            MockCountingDurationField.int64Additions = 0;
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(61, field.Add(new LocalInstant(1), 1L).Ticks);
            Assert.AreEqual(1, MockCountingDurationField.int64Additions);
        }

        [Test]
        public void GetDifference_DelegatesToDurationFieldGetInt64Difference()
        {
            MockCountingDurationField.differences = 0;
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(30, field.GetDifference(new LocalInstant(), new LocalInstant()));
            Assert.AreEqual(1, MockCountingDurationField.differences);
        }

        [Test]
        public void GetInt64Difference_DelegatesToDurationFieldGetInt64Difference()
        {
            MockCountingDurationField.differences = 0;
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(30, field.GetInt64Difference(new LocalInstant(), new LocalInstant()));
            Assert.AreEqual(1, MockCountingDurationField.differences);
        }

        [Test]
        public void Set_OnStub_Adds1000()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(1000, field.SetValue(new LocalInstant(0), 0).Ticks);
            Assert.AreEqual(1029, field.SetValue(new LocalInstant(0), 29).Ticks);
        }

        [Test]
        public void IsLeap_DefaultsToFalse()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.IsFalse(field.IsLeap(new LocalInstant(0)));
        }

        [Test]
        public void GetLeapAmount_DefaultsTo0()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0, field.GetLeapAmount(new LocalInstant(0)));
        }

        [Test]
        public void LeapDurationField_DefaultsToNull()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.IsNull(field.LeapDurationField);
        }

        [Test]
        public void GetMinimumValue_OnStub_DefaultsTo0()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.GetMinimumValue());
        }
                
        [Test]
        public void GetMinimumValueForInstant_OnStub_DefaultsTo0()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.GetMinimumValue(new LocalInstant(0)));
        }

        [Test]
        public void GetMaximumValue_OnStub_DefaultsTo59()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(59L, field.GetMaximumValue());
        }

        [Test]
        public void GetMaximumValueForInstant_OnStub_DefaultsTo59()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(59L, field.GetMaximumValue(new LocalInstant(0)));
        }

        [Test]
        public void RoundFloor_OnStub_RoundsTo60()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.RoundFloor(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundFloor(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundCeiling()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.RoundCeiling(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundHalfFloor()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundHalfCeiling()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.RoundHalfCeiling(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfCeiling(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundHalfEven()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.RoundHalfEven(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfEven(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfEven(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfEven(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfEven(new LocalInstant(60L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfEven(new LocalInstant(89L)).Ticks);
            Assert.AreEqual(120L, field.RoundHalfEven(new LocalInstant(90L)).Ticks);
            Assert.AreEqual(120L, field.RoundHalfEven(new LocalInstant(91L)).Ticks);
        }

        [Test]
        public void Remainder()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.Remainder(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(29L, field.Remainder(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(30L, field.Remainder(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(31L, field.Remainder(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(0L, field.Remainder(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void IsTypeValid_AllEnumValues_AreValid()
        {
            foreach (DateTimeFieldType type in Enum.GetValues(typeof(DateTimeFieldType)))
            {
                Assert.IsTrue(DateTimeFieldBase.IsTypeValid(type));
            }
        }

        [Test]
        public void IsTypeValid_ValuesOutOfRange_AreInvalid()
        {
            Assert.IsFalse(DateTimeFieldBase.IsTypeValid((DateTimeFieldType) (-1)));
            DateTimeFieldType max = Enum.GetValues(typeof(DateTimeFieldType))
                .Cast<DateTimeFieldType>().Max();
            Assert.IsFalse(DateTimeFieldBase.IsTypeValid(max + 1));
        }

        private class StubDateTimeFieldBase : DateTimeFieldBase
        {
            private int maxValue;

            internal StubDateTimeFieldBase(DateTimeFieldType type)
                : base(type)
            {
            }

            internal StubDateTimeFieldBase()
                : this(59)
            {
            }

            internal StubDateTimeFieldBase(int maxValue)
                : base(DateTimeFieldType.SecondOfMinute)
            {
                this.maxValue = maxValue;
            }

            public override int GetValue(LocalInstant instant)
            {
                return (int)(instant.Ticks / 60L);
            }

            public override long GetInt64Value(LocalInstant localInstant)
            {
                return localInstant.Ticks / 60L;
            }

            public override LocalInstant SetValue(LocalInstant localInstant, long value)
            {
                return new LocalInstant(1000 + value);
            }

            public override DurationField DurationField
            {
                get { return new MockCountingDurationField(DurationFieldType.Seconds); }
            }

            public override DurationField RangeDurationField
            {
                get { return new MockCountingDurationField(DurationFieldType.Minutes);  }
            }

            public override long GetMaximumValue()
            {
                return 59;
            }

            public override long GetMinimumValue()
            {
                return 0;
            }

            public override LocalInstant RoundFloor(LocalInstant localInstant)
            {
                return new LocalInstant((localInstant.Ticks / 60L) * 60L);
            }

            public override bool IsLenient
            {
                get { return false; }
            }
        }
    }
}
