// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class OffsetDateTimeFieldTest
    {
        [Test]
        public void Constructor_WithoutFieldType_HardCodedProperties()
        {
            OffsetDateTimeField field = new OffsetDateTimeField(CalendarSystem.Iso.Fields.SecondOfMinute, 3);
            Assert.AreEqual(DateTimeFieldType.SecondOfMinute, field.FieldType);
            Assert.IsTrue(field.IsSupported);
            Assert.IsFalse(field.IsLeap(new LocalInstant(0)));
        }

        [Test]
        public void Constructor_WithNullField_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new OffsetDateTimeField(null, 3));
        }

        [Test]
        public void Constructor_WithZeroOffset_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new OffsetDateTimeField(CalendarSystem.Iso.Fields.SecondOfMinute, 0));
        }

        [Test]
        public void Constructor_WithUnsupportedField_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => new OffsetDateTimeField(UnsupportedDateTimeField.SecondOfMinute, 3));
        }

        [Test]
        public void Constructor_WithSpecificFieldType()
        {
            OffsetDateTimeField field = new OffsetDateTimeField(CalendarSystem.Iso.Fields.SecondOfMinute, DateTimeFieldType.SecondOfDay, 3);
            Assert.AreEqual(DateTimeFieldType.SecondOfDay, field.FieldType);
        }

        [Test]
        public void Constructor_WithSpecificFieldTypeButNullField_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new OffsetDateTimeField(null, DateTimeFieldType.SecondOfDay, 3));
        }

        [Test]
        public void Constructor_WithSpecificNullFieldType_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new OffsetDateTimeField(CalendarSystem.Iso.Fields.SecondOfMinute, null, 3));
        }

        [Test]
        public void Constructor_WithSpecificFieldTypeButZeroOffset_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new OffsetDateTimeField(CalendarSystem.Iso.Fields.SecondOfMinute, DateTimeFieldType.SecondOfDay, 0));
        }

        [Test]
        public void GetValue_AddsOffset()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(0 + 3, field.GetValue(new LocalInstant(0)));
            Assert.AreEqual(6 + 3, field.GetValue(new LocalInstant(6 * NodaConstants.TicksPerSecond)));
        }

        [Test]
        public void GetInt64Value_AddsOffset()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(0 + 3, field.GetInt64Value(new LocalInstant(0)));
            Assert.AreEqual(6 + 3, field.GetInt64Value(new LocalInstant(6 * NodaConstants.TicksPerSecond)));
        }

        [Test]
        public void SetValue_AdjustsByOffset()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(31200000L, field.SetValue(new LocalInstant(21200000L), 6).Ticks);
            Assert.AreEqual(261200000L, field.SetValue(new LocalInstant(21200000L), 29).Ticks);
            // Note the wrapping here
            Assert.AreEqual(571200000L, field.SetValue(new LocalInstant(21200000L), 60).Ticks);
        }

        [Test]
        public void GetMinimumValue_UsesOffset()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(3, field.GetMinimumValue());
        }

        [Test]
        public void GetMinimumValue_WithLocalInstant_UsesOffset()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(3, field.GetMinimumValue(new LocalInstant(0)));
        }

        [Test]
        public void GetMaximumValue_UsesOffset()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(62, field.GetMaximumValue());
        }

        [Test]
        public void GetMaximumValue_WithLocalInstant_UsesOffset()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(62, field.GetMaximumValue(new LocalInstant(0)));
        }

        [Test]
        public void RoundFloor()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(-2 * NodaConstants.TicksPerSecond, field.RoundFloor(new LocalInstant(-1001 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(-1 * NodaConstants.TicksPerSecond, field.RoundFloor(new LocalInstant(-1000 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(-1 * NodaConstants.TicksPerSecond, field.RoundFloor(new LocalInstant(-999 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(-1 * NodaConstants.TicksPerSecond, field.RoundFloor(new LocalInstant(-1 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundFloor(new LocalInstant(0)).Ticks);
            Assert.AreEqual(0, field.RoundFloor(new LocalInstant(1 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundFloor(new LocalInstant(499 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundFloor(new LocalInstant(500 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundFloor(new LocalInstant(501 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundFloor(new LocalInstant(1000 * NodaConstants.TicksPerMillisecond)).Ticks);
        }

        [Test]
        public void RoundCeiling_DelegatesToWrappedField()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(-1 * NodaConstants.TicksPerSecond, field.RoundCeiling(new LocalInstant(-1001 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(-1 * NodaConstants.TicksPerSecond, field.RoundCeiling(new LocalInstant(-1000 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundCeiling(new LocalInstant(-999 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundCeiling(new LocalInstant(-1 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundCeiling(new LocalInstant(0)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundCeiling(new LocalInstant(1 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundCeiling(new LocalInstant(499 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundCeiling(new LocalInstant(500 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundCeiling(new LocalInstant(501 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundCeiling(new LocalInstant(1000 * NodaConstants.TicksPerMillisecond)).Ticks);
        }

        [Test]
        public void RoundHalfFloor_DelegatesToWrappedField()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(0, field.RoundHalfFloor(new LocalInstant(0)).Ticks);
            Assert.AreEqual(0, field.RoundHalfFloor(new LocalInstant(499 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundHalfFloor(new LocalInstant(500 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfFloor(new LocalInstant(501 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfFloor(new LocalInstant(1000 * NodaConstants.TicksPerMillisecond)).Ticks);
        }

        [Test]
        public void RoundHalfCeiling_DelegatesToWrappedField()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(0, field.RoundHalfCeiling(new LocalInstant(0)).Ticks);
            Assert.AreEqual(0, field.RoundHalfCeiling(new LocalInstant(499 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfCeiling(new LocalInstant(500 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfCeiling(new LocalInstant(501 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfCeiling(new LocalInstant(1000 * NodaConstants.TicksPerMillisecond)).Ticks);
        }

        [Test]
        public void RoundHalfEven_DelegatesToWrappedField()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(0, field.RoundHalfEven(new LocalInstant(0)).Ticks);
            Assert.AreEqual(0, field.RoundHalfEven(new LocalInstant(499 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundHalfEven(new LocalInstant(500 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfEven(new LocalInstant(501 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfEven(new LocalInstant(1000 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfEven(new LocalInstant(1499 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(2 * NodaConstants.TicksPerSecond, field.RoundHalfEven(new LocalInstant(1500 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(2 * NodaConstants.TicksPerSecond, field.RoundHalfEven(new LocalInstant(1501 * NodaConstants.TicksPerMillisecond)).Ticks);
        }

        [Test]
        public void Remainder_DelegatesToWrappedField()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(0, field.Remainder(new LocalInstant(0)).Ticks);
            Assert.AreEqual(499 * NodaConstants.TicksPerMillisecond, field.Remainder(new LocalInstant(499 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(500 * NodaConstants.TicksPerMillisecond, field.Remainder(new LocalInstant(500 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(501 * NodaConstants.TicksPerMillisecond, field.Remainder(new LocalInstant(501 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.Remainder(new LocalInstant(1000 * NodaConstants.TicksPerMillisecond)).Ticks);
        }

        /// <summary>
        /// Helper method to avoid having to repeat all of this every time
        /// </summary>
        private static OffsetDateTimeField GetSampleField()
        {
            return new OffsetDateTimeField(CalendarSystem.Iso.Fields.SecondOfMinute, 3);
        }
    }
}