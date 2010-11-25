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
    public class PreciseDateTimeFieldTest
    {
        [Test]
        public void Constructor_WithNullRangeField_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PreciseDateTimeField(DateTimeFieldType.SecondOfMinute, new FakeDurationField(1, true), null));
        }

        [Test]
        public void Constructor_WithTooSmallRangeField_ThrowsArgumentException()
        {
            // Effectively like "seconds per second" - effective range = 1
            Assert.Throws<ArgumentException>(
                () => new PreciseDateTimeField(DateTimeFieldType.SecondOfMinute, new FakeDurationField(1, true), new FakeDurationField(1, true)));
        }

        [Test]
        public void Constructor_WithImprecise_RangeField_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => new PreciseDateTimeField(DateTimeFieldType.SecondOfMinute, new FakeDurationField(1, true), new FakeDurationField(60, false)));
        }

        [Test]
        public void FieldType()
        {
            PreciseDateTimeField field = new PreciseDateTimeField(DateTimeFieldType.SecondOfMinute, new FakeDurationField(1, true),
                                                                  new FakeDurationField(60, true));
            Assert.AreEqual(DateTimeFieldType.SecondOfMinute, field.FieldType);
        }

        [Test]
        public void IsSupported()
        {
            PreciseDateTimeField field = CreateSecondOfMinuteField();
            Assert.IsTrue(field.IsSupported);
        }

        [Test]
        public void GetInt64Value()
        {
            PreciseDateTimeField field = CreateSecondOfMinuteField();
            Assert.AreEqual(0, field.GetInt64Value(new LocalInstant(0L)));
            Assert.AreEqual(1, field.GetInt64Value(new LocalInstant(60L)));
            Assert.AreEqual(2, field.GetInt64Value(new LocalInstant(123L)));
        }

        [Test]
        public void Add_WithInt32Value()
        {
            MockCountingDurationField.int32Additions = 0;
            DateTimeFieldBase field = CreateSecondOfMinuteField();
            Assert.AreEqual(61L, field.Add(new LocalInstant(1L), 1).Ticks);
            Assert.AreEqual(1, MockCountingDurationField.int32Additions);
        }

        [Test]
        public void Add_WithInt64Value()
        {
            MockCountingDurationField.int64Additions = 0;
            DateTimeFieldBase field = CreateSecondOfMinuteField();
            Assert.AreEqual(61L, field.Add(new LocalInstant(1L), 1L).Ticks);
            Assert.AreEqual(1, MockCountingDurationField.int64Additions);
        }

        [Test]
        public void GetDifference_DelegatesToDurationField()
        {
            MockCountingDurationField.differences = 0;
            DateTimeFieldBase field = CreateSecondOfMinuteField();
            Assert.AreEqual(30, field.GetDifference(new LocalInstant(0), new LocalInstant(0)));
            Assert.AreEqual(1, MockCountingDurationField.differences);
        }

        [Test]
        public void GetInt64Difference_DelegatesToDurationField()
        {
            MockCountingDurationField.differences64 = 0;
            DateTimeFieldBase field = CreateSecondOfMinuteField();
            Assert.AreEqual(30L, field.GetInt64Difference(new LocalInstant(0), new LocalInstant(0)));
            Assert.AreEqual(1, MockCountingDurationField.differences64);
        }

        [Test]
        public void SetValue()
        {
            DateTimeFieldBase field = CreateSecondOfMinuteField();
            Assert.AreEqual(0, field.SetValue(new LocalInstant(120L), 0).Ticks);
            Assert.AreEqual(29 * 60, field.SetValue(new LocalInstant(120L), 29).Ticks);
        }

        [Test]
        public void IsLeap_DefaultsToFalse()
        {
            DateTimeFieldBase field = CreateSecondOfMinuteField();
            Assert.IsFalse(field.IsLeap(new LocalInstant(0L)));
        }

        [Test]
        public void GetLeapAmount_DefaultsTo0()
        {
            DateTimeFieldBase field = CreateSecondOfMinuteField();
            Assert.AreEqual(0L, field.GetLeapAmount(new LocalInstant(0L)));
        }

        public void LeapDurationField_DefaultsToNull()
        {
            DateTimeFieldBase field = CreateSecondOfMinuteField();
            Assert.IsNull(field.LeapDurationField);
        }

        [Test]
        public void GetMinimumValue_DefaultsTo0()
        {
            DateTimeFieldBase field = CreateSecondOfMinuteField();
            Assert.AreEqual(0L, field.GetMinimumValue());
        }

        [Test]
        public void RoundFloor()
        {
            DateTimeFieldBase field = CreateSecondOfMinuteField();
            Assert.AreEqual(-120L, field.RoundFloor(new LocalInstant(-61L)).Ticks);
            Assert.AreEqual(-60L, field.RoundFloor(new LocalInstant(-60L)).Ticks);
            Assert.AreEqual(-60L, field.RoundFloor(new LocalInstant(-59L)).Ticks);
            Assert.AreEqual(-60L, field.RoundFloor(new LocalInstant(-1L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(new LocalInstant(1L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundFloor(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundCeiling()
        {
            DateTimeFieldBase field = CreateSecondOfMinuteField();
            Assert.AreEqual(-60L, field.RoundCeiling(new LocalInstant(-61L)).Ticks);
            Assert.AreEqual(-60L, field.RoundCeiling(new LocalInstant(-60L)).Ticks);
            Assert.AreEqual(0L, field.RoundCeiling(new LocalInstant(-59L)).Ticks);
            Assert.AreEqual(0L, field.RoundCeiling(new LocalInstant(-1L)).Ticks);
            Assert.AreEqual(0L, field.RoundCeiling(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(new LocalInstant(1L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundHalfFloor()
        {
            DateTimeFieldBase field = CreateSecondOfMinuteField();
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundHalfCeiling()
        {
            DateTimeFieldBase field = CreateSecondOfMinuteField();
            Assert.AreEqual(0L, field.RoundHalfCeiling(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfCeiling(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundHalfEven()
        {
            DateTimeFieldBase field = CreateSecondOfMinuteField();
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
            DateTimeFieldBase field = CreateSecondOfMinuteField();
            Assert.AreEqual(0L, field.Remainder(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(29L, field.Remainder(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(30L, field.Remainder(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(31L, field.Remainder(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(0L, field.Remainder(new LocalInstant(60L)).Ticks);
        }

        private static PreciseDateTimeField CreateSecondOfMinuteField()
        {
            return new PreciseDateTimeField(DateTimeFieldType.SecondOfMinute, new MockCountingDurationField(DurationFieldType.Seconds, 60),
                                            new MockCountingDurationField(DurationFieldType.Minutes, 60 * 60));
        }
    }
}