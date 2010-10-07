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
            DurationField badField = new FakeDurationField(0, true);
            Assert.Throws<ArgumentException>(() => new StubPreciseDurationDateTimeField(badField));
        }

        [Test]
        public void Constructor_GivenImpreciseDurationField_ThrowsArgumentException()
        {
            DurationField badField = new FakeDurationField(1, false);
            Assert.Throws<ArgumentException>(() => new StubPreciseDurationDateTimeField(badField));
        }

        [Test]
        public void FieldType_ReturnsTypePassedToConstructor()
        {
            DateTimeField field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(DateTimeFieldType.SecondOfMinute, field.FieldType);
        }

        [Test]
        public void IsLenient_ReturnsFalse()
        {
            DateTimeField field = new StubPreciseDurationDateTimeField();
            Assert.IsFalse(field.IsLenient);
        }

        [Test]
        public void SetValue()
        {
            DateTimeField field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0, field.SetValue(new LocalInstant(120L), 0).Ticks);
            Assert.AreEqual(29 * 60, field.SetValue(new LocalInstant(120L), 29).Ticks);
        }

        [Test]
        public void IsLeap_DefaultsToFalse()
        {
            DateTimeField field = new StubPreciseDurationDateTimeField();
            Assert.IsFalse(field.IsLeap(new LocalInstant(0L)));
        }

        [Test]
        public void GetLeapAmount_DefaultsTo0()
        {
            DateTimeField field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.GetLeapAmount(new LocalInstant(0L)));
        }

        public void LeapDurationField_DefaultsToNull()
        {
            DateTimeField field = new StubPreciseDurationDateTimeField();
            Assert.IsNull(field.LeapDurationField);
        }

        [Test]
        public void GetMinimumValue_DefaultsTo0()
        {
            DateTimeField field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.GetMinimumValue());
        }

        [Test]
        public void RoundFloor()
        {
            DateTimeField field = new StubPreciseDurationDateTimeField();
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
            DateTimeField field = new StubPreciseDurationDateTimeField();
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
            DateTimeField field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundHalfCeiling()
        {
            DateTimeField field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.RoundHalfCeiling(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfCeiling(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundHalfEven()
        {
            DateTimeField field = new StubPreciseDurationDateTimeField();
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
            DateTimeField field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.Remainder(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(29L, field.Remainder(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(30L, field.Remainder(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(31L, field.Remainder(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(0L, field.Remainder(new LocalInstant(60L)).Ticks);
            Assert.AreEqual(57L, field.Remainder(new LocalInstant(-63L)).Ticks);
        }

        private class StubPreciseDurationDateTimeField : PreciseDurationDateTimeField
        {
            internal StubPreciseDurationDateTimeField(DurationField unit) : base(DateTimeFieldType.SecondOfMinute, unit)
            {
            }

            internal StubPreciseDurationDateTimeField() : base(DateTimeFieldType.SecondOfMinute, new MockCountingDurationField(DurationFieldType.Seconds))
            {
            }

            public override long GetInt64Value(LocalInstant localInstant)
            {
                return localInstant.Ticks / 60L;
            }

            public override DurationField RangeDurationField { get { return new MockCountingDurationField(DurationFieldType.Minutes); } }

            public override long GetMaximumValue()
            {
                return 59;
            }
        }

        // Class allowing us to simulate bad precision/ticks for constructor testing
    }
}