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
    public class PreciseDurationDateTimeFieldTest
    {
        [Test]
        public void Constructor_GivenNullDurationField_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new StubPreciseDurationDateTimeField(null));
        }

        [Test]
        public void Constructor_GivenZeroTicksDurationField_ThrowsArgumentException()
        {
            IDurationField badField = new FakeDurationField(0, true);
            Assert.Throws<ArgumentException>(() => new StubPreciseDurationDateTimeField(badField));
        }

        [Test]
        public void Constructor_GivenImpreciseDurationField_ThrowsArgumentException()
        {
            IDurationField badField = new FakeDurationField(1, false);
            Assert.Throws<ArgumentException>(() => new StubPreciseDurationDateTimeField(badField));
        }

        [Test]
        public void FieldType_ReturnsTypePassedToConstructor()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(DateTimeFieldType.SecondOfMinute, field.FieldType);
        }

        [Test]
        public void IsLenient_ReturnsFalse()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.IsFalse(field.IsLenient);
        }

        [Test]
        public void SetValue()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0, field.SetValue(LocalInstant.FromTicks(120L), 0).Ticks);
            Assert.AreEqual(29 * 60, field.SetValue(LocalInstant.FromTicks(120L), 29).Ticks);
        }

        [Test]
        public void IsLeap_DefaultsToFalse()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.IsFalse(field.IsLeap(LocalInstant.FromTicks(0L)));
        }

        [Test]
        public void GetLeapAmount_DefaultsTo0()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.GetLeapAmount(LocalInstant.FromTicks(0L)));
        }

        public void LeapDurationField_DefaultsToNull()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.IsNull(field.LeapDurationField);
        }

        [Test]
        public void GetMinimumValue_DefaultsTo0()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.GetMinimumValue());
        }

        [Test]
        public void RoundFloor()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(-120L, field.RoundFloor(LocalInstant.FromTicks(-61L)).Ticks);
            Assert.AreEqual(-60L, field.RoundFloor(LocalInstant.FromTicks(-60L)).Ticks);
            Assert.AreEqual(-60L, field.RoundFloor(LocalInstant.FromTicks(-59L)).Ticks);
            Assert.AreEqual(-60L, field.RoundFloor(LocalInstant.FromTicks(-1L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(LocalInstant.FromTicks(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(LocalInstant.FromTicks(1L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(LocalInstant.FromTicks(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(LocalInstant.FromTicks(30L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(LocalInstant.FromTicks(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundFloor(LocalInstant.FromTicks(60L)).Ticks);
        }

        [Test]
        public void RoundCeiling()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(-60L, field.RoundCeiling(LocalInstant.FromTicks(-61L)).Ticks);
            Assert.AreEqual(-60L, field.RoundCeiling(LocalInstant.FromTicks(-60L)).Ticks);
            Assert.AreEqual(0L, field.RoundCeiling(LocalInstant.FromTicks(-59L)).Ticks);
            Assert.AreEqual(0L, field.RoundCeiling(LocalInstant.FromTicks(-1L)).Ticks);
            Assert.AreEqual(0L, field.RoundCeiling(LocalInstant.FromTicks(0L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(LocalInstant.FromTicks(1L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(LocalInstant.FromTicks(29L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(LocalInstant.FromTicks(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(LocalInstant.FromTicks(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(LocalInstant.FromTicks(60L)).Ticks);
        }

        [Test]
        public void RoundHalfFloor()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.RoundHalfFloor(LocalInstant.FromTicks(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(LocalInstant.FromTicks(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(LocalInstant.FromTicks(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(LocalInstant.FromTicks(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(LocalInstant.FromTicks(60L)).Ticks);
        }

        [Test]
        public void RoundHalfCeiling()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.RoundHalfCeiling(LocalInstant.FromTicks(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfCeiling(LocalInstant.FromTicks(29L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(LocalInstant.FromTicks(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(LocalInstant.FromTicks(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(LocalInstant.FromTicks(60L)).Ticks);
        }

        [Test]
        public void RoundHalfEven()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.RoundHalfEven(LocalInstant.FromTicks(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfEven(LocalInstant.FromTicks(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfEven(LocalInstant.FromTicks(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfEven(LocalInstant.FromTicks(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfEven(LocalInstant.FromTicks(60L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfEven(LocalInstant.FromTicks(89L)).Ticks);
            Assert.AreEqual(120L, field.RoundHalfEven(LocalInstant.FromTicks(90L)).Ticks);
            Assert.AreEqual(120L, field.RoundHalfEven(LocalInstant.FromTicks(91L)).Ticks);
        }

        [Test]
        public void Remainder()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.Remainder(LocalInstant.FromTicks(0L)).Ticks);
            Assert.AreEqual(29L, field.Remainder(LocalInstant.FromTicks(29L)).Ticks);
            Assert.AreEqual(30L, field.Remainder(LocalInstant.FromTicks(30L)).Ticks);
            Assert.AreEqual(31L, field.Remainder(LocalInstant.FromTicks(31L)).Ticks);
            Assert.AreEqual(0L, field.Remainder(LocalInstant.FromTicks(60L)).Ticks);
            Assert.AreEqual(57L, field.Remainder(LocalInstant.FromTicks(-63L)).Ticks);
        }

        private class StubPreciseDurationDateTimeField : PreciseDurationDateTimeField
        {
            internal StubPreciseDurationDateTimeField(IDurationField unit) : base(DateTimeFieldType.SecondOfMinute, unit)
            {
            }

            internal StubPreciseDurationDateTimeField() : base(DateTimeFieldType.SecondOfMinute, new MockCountingDurationField(DurationFieldType.Seconds))
            {
            }

            public override long GetInt64Value(LocalInstant localInstant)
            {
                return localInstant.Ticks / 60L;
            }

            public override IDurationField RangeDurationField { get { return new MockCountingDurationField(DurationFieldType.Minutes); } }

            public override long GetMaximumValue()
            {
                return 59;
            }
        }

        // Class allowing us to simulate bad precision/ticks for constructor testing
    }
}