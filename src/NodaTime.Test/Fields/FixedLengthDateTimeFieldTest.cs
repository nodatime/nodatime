// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class FixedLengthDateTimeFieldTest
    {
        [Test]
        public void Constructor_WithNullRangeField_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new FixedLengthDateTimeField(DateTimeFieldType.MinuteOfHour, new FakePeriodField(1, true), null));
        }

        [Test]
        public void Constructor_WithTooSmallRangeField_ThrowsArgumentException()
        {
            // Effectively like "seconds per second" - effective range = 1
            Assert.Throws<ArgumentException>(
                () => new FixedLengthDateTimeField(DateTimeFieldType.MinuteOfHour, new FakePeriodField(1, true), new FakePeriodField(1, true)));
        }

        [Test]
        public void Constructor_WithVariableLength_RangeField_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => new FixedLengthDateTimeField(DateTimeFieldType.MinuteOfHour, new FakePeriodField(1, true), new FakePeriodField(60, false)));
        }

        [Test]
        public void FieldType()
        {
            FixedLengthDateTimeField field = new FixedLengthDateTimeField(DateTimeFieldType.MinuteOfHour, new FakePeriodField(1, true),
                                                                  new FakePeriodField(60, true));
            Assert.AreEqual(DateTimeFieldType.MinuteOfHour, field.FieldType);
        }

        [Test]
        public void IsSupported()
        {
            FixedLengthDateTimeField field = CreateMinuteOfHourField();
            Assert.IsTrue(field.IsSupported);
        }

        [Test]
        public void GetInt64Value()
        {
            // Slightly odd LocalInstant in that it's in seconds, not ticks - due to the way the MockCountingPeriodField works.
            FixedLengthDateTimeField field = CreateMinuteOfHourField();
            Assert.AreEqual(0, field.GetInt64Value(new LocalInstant(0L)));
            Assert.AreEqual(1, field.GetInt64Value(new LocalInstant(60L)));
            Assert.AreEqual(2, field.GetInt64Value(new LocalInstant(123L)));
        }

        [Test]
        public void GetInt64Value_Negative()
        {
            // Slightly odd LocalInstant in that it's in seconds, not ticks - due to the way the MockCountingPeriodField works.
            FixedLengthDateTimeField field = CreateMinuteOfHourField();
            Assert.AreEqual(0, field.GetInt64Value(new LocalInstant(0L)));
            Assert.AreEqual(59, field.GetInt64Value(new LocalInstant(-59L)));
            Assert.AreEqual(59, field.GetInt64Value(new LocalInstant(-60L)));
            Assert.AreEqual(58, field.GetInt64Value(new LocalInstant(-61L)));
            Assert.AreEqual(58, field.GetInt64Value(new LocalInstant(-119L)));
            Assert.AreEqual(58, field.GetInt64Value(new LocalInstant(-120L)));
            Assert.AreEqual(57, field.GetInt64Value(new LocalInstant(-121L)));
        }

        [Test]
        public void SetValue()
        {
            DateTimeField field = CreateMinuteOfHourField();
            Assert.AreEqual(0, field.SetValue(new LocalInstant(120L), 0).Ticks);
            Assert.AreEqual(29 * 60, field.SetValue(new LocalInstant(120L), 29).Ticks);
        }

        [Test]
        public void IsLeap_DefaultsToFalse()
        {
            DateTimeField field = CreateMinuteOfHourField();
            Assert.IsFalse(field.IsLeap(new LocalInstant(0L)));
        }

        [Test]
        public void RoundFloor()
        {
            DateTimeField field = CreateMinuteOfHourField();
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
            DateTimeField field = CreateMinuteOfHourField();
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
            DateTimeField field = CreateMinuteOfHourField();
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundHalfCeiling()
        {
            DateTimeField field = CreateMinuteOfHourField();
            Assert.AreEqual(0L, field.RoundHalfCeiling(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfCeiling(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundHalfEven()
        {
            DateTimeField field = CreateMinuteOfHourField();
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
            DateTimeField field = CreateMinuteOfHourField();
            Assert.AreEqual(0L, field.Remainder(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(29L, field.Remainder(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(30L, field.Remainder(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(31L, field.Remainder(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(0L, field.Remainder(new LocalInstant(60L)).Ticks);
        }

        private static FixedLengthDateTimeField CreateMinuteOfHourField()
        {
            return new FixedLengthDateTimeField(DateTimeFieldType.MinuteOfHour,
                new MockCountingPeriodField(PeriodFieldType.Minutes, 60),
                new MockCountingPeriodField(PeriodFieldType.Hours, 60 * 60));
        }
    }
}