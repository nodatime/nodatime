// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class FixedLengthPeriodDateTimeFieldTest
    {
        [Test]
        public void Constructor_GivenNullPeriodField_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new StubFixedLengthPeriodDateTimeField(null));
        }

        [Test]
        public void Constructor_GivenUnsupportedPeriodField_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new StubFixedLengthPeriodDateTimeField(UnsupportedPeriodField.Hours));
        }

        [Test]
        public void Constructor_GivenVariableLengthPeriodField_ThrowsArgumentException()
        {
            PeriodField badField = new FakePeriodField(1, false);
            Assert.Throws<ArgumentException>(() => new StubFixedLengthPeriodDateTimeField(badField));
        }

        [Test]
        public void FieldType_ReturnsTypePassedToConstructor()
        {
            DateTimeField field = new StubFixedLengthPeriodDateTimeField();
            Assert.AreEqual(DateTimeFieldType.SecondOfMinute, field.FieldType);
        }

        [Test]
        public void SetValue()
        {
            DateTimeField field = new StubFixedLengthPeriodDateTimeField();
            Assert.AreEqual(0, field.SetValue(new LocalInstant(120L), 0).Ticks);
            Assert.AreEqual(29 * 60, field.SetValue(new LocalInstant(120L), 29).Ticks);
        }

        [Test]
        public void GetMinimumValue_DefaultsTo0()
        {
            DateTimeField field = new StubFixedLengthPeriodDateTimeField();
            Assert.AreEqual(0L, field.GetMinimumValue());
        }

        [Test]
        public void RoundFloor()
        {
            DateTimeField field = new StubFixedLengthPeriodDateTimeField();
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
            DateTimeField field = new StubFixedLengthPeriodDateTimeField();
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

        private class StubFixedLengthPeriodDateTimeField : FixedLengthPeriodDateTimeField
        {
            internal StubFixedLengthPeriodDateTimeField(PeriodField unit) : base(DateTimeFieldType.SecondOfMinute, unit)
            {
            }

            internal StubFixedLengthPeriodDateTimeField() : base(DateTimeFieldType.SecondOfMinute, new MockCountingPeriodField(PeriodFieldType.Seconds))
            {
            }

            internal override long GetInt64Value(LocalInstant localInstant)
            {
                return localInstant.Ticks / 60L;
            }

            internal override long GetMaximumValue()
            {
                return 59;
            }
        }

        // Class allowing us to simulate bad precision/ticks for constructor testing
    }
}