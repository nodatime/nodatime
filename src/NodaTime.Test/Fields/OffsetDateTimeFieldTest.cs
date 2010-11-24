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
using NodaTime.Calendars;
using NodaTime.Fields;
using NUnit.Framework;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class OffsetDateTimeFieldTest
    {
        [Test]
        public void Constructor_WithoutFieldType_HardCodedProperties()
        {
            OffsetDateTimeField field = new OffsetDateTimeField(IsoCalendarSystem.Instance.Fields.SecondOfMinute, 3);
            Assert.AreEqual(DateTimeFieldType.SecondOfMinute, field.FieldType);
            Assert.IsTrue(field.IsSupported);
            Assert.IsFalse(field.IsLenient);
            Assert.IsFalse(field.IsLeap(LocalInstant.FromTicks(0)));
            Assert.AreEqual(0, field.GetLeapAmount(LocalInstant.FromTicks(0)));
            Assert.IsNull(field.LeapDurationField);
        }

        [Test]
        public void Constructor_WithNullField_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new OffsetDateTimeField(null, 3));
        }

        [Test]
        public void Constructor_WithZeroOffset_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new OffsetDateTimeField(IsoCalendarSystem.Instance.Fields.SecondOfMinute, 0));
        }

        [Test]
        public void Constructor_WithUnsupportedField_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => new OffsetDateTimeField(UnsupportedDateTimeField.GetInstance(DateTimeFieldType.SecondOfMinute, UnsupportedDurationField.Seconds), 3));
        }

        [Test]
        public void Constructor_WithSpecificFieldType()
        {
            OffsetDateTimeField field = new OffsetDateTimeField(IsoCalendarSystem.Instance.Fields.SecondOfMinute, DateTimeFieldType.SecondOfDay, 3);
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
            Assert.Throws<ArgumentNullException>(() => new OffsetDateTimeField(IsoCalendarSystem.Instance.Fields.SecondOfMinute, null, 3));
        }

        [Test]
        public void Constructor_WithSpecificFieldTypeButZeroOffset_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new OffsetDateTimeField(IsoCalendarSystem.Instance.Fields.SecondOfMinute, DateTimeFieldType.SecondOfDay, 0));
        }

        [Test]
        public void GetValue_AddsOffset()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(0 + 3, field.GetValue(LocalInstant.FromTicks(0)));
            Assert.AreEqual(6 + 3, field.GetValue(LocalInstant.FromTicks(6 * NodaConstants.TicksPerSecond)));
        }

        [Test]
        public void GetInt64Value_AddsOffset()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(0 + 3, field.GetInt64Value(LocalInstant.FromTicks(0)));
            Assert.AreEqual(6 + 3, field.GetInt64Value(LocalInstant.FromTicks(6 * NodaConstants.TicksPerSecond)));
        }

        [Test]
        public void Add_Int32_DelegatesToWrappedField()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(NodaConstants.TicksPerSecond, field.Add(LocalInstant.FromTicks(0), 1).Ticks);
        }

        [Test]
        public void Add_Int64_DelegatesToWrappedField()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(NodaConstants.TicksPerSecond, field.Add(LocalInstant.FromTicks(0), 1L).Ticks);
        }

        [Test]
        public void GetInt64Difference_DelegatesToWrappedField()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(-21L,
                            field.GetInt64Difference(LocalInstant.FromTicks(20 * NodaConstants.TicksPerSecond), LocalInstant.FromTicks(41 * NodaConstants.TicksPerSecond)));
        }

        [Test]
        public void GetDifference_DelegatesToWrappedField()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(-21L, field.GetDifference(LocalInstant.FromTicks(20 * NodaConstants.TicksPerSecond), LocalInstant.FromTicks(41 * NodaConstants.TicksPerSecond)));
        }

        [Test]
        public void SetValue_AdjustsByOffset()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(31200000L, field.SetValue(LocalInstant.FromTicks(21200000L), 6).Ticks);
            Assert.AreEqual(261200000L, field.SetValue(LocalInstant.FromTicks(21200000L), 29).Ticks);
            // Note the wrapping here
            Assert.AreEqual(571200000L, field.SetValue(LocalInstant.FromTicks(21200000L), 60).Ticks);
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
            Assert.AreEqual(3, field.GetMinimumValue(LocalInstant.FromTicks(0)));
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
            Assert.AreEqual(62, field.GetMaximumValue(LocalInstant.FromTicks(0)));
        }

        [Test]
        public void RoundFloor()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(-2 * NodaConstants.TicksPerSecond, field.RoundFloor(LocalInstant.FromTicks(-1001 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(-1 * NodaConstants.TicksPerSecond, field.RoundFloor(LocalInstant.FromTicks(-1000 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(-1 * NodaConstants.TicksPerSecond, field.RoundFloor(LocalInstant.FromTicks(-999 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(-1 * NodaConstants.TicksPerSecond, field.RoundFloor(LocalInstant.FromTicks(-1 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundFloor(LocalInstant.FromTicks(0)).Ticks);
            Assert.AreEqual(0, field.RoundFloor(LocalInstant.FromTicks(1 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundFloor(LocalInstant.FromTicks(499 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundFloor(LocalInstant.FromTicks(500 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundFloor(LocalInstant.FromTicks(501 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundFloor(LocalInstant.FromTicks(1000 * NodaConstants.TicksPerMillisecond)).Ticks);
        }

        [Test]
        public void RoundCeiling_DelegatesToWrappedField()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(-1 * NodaConstants.TicksPerSecond, field.RoundCeiling(LocalInstant.FromTicks(-1001 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(-1 * NodaConstants.TicksPerSecond, field.RoundCeiling(LocalInstant.FromTicks(-1000 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundCeiling(LocalInstant.FromTicks(-999 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundCeiling(LocalInstant.FromTicks(-1 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundCeiling(LocalInstant.FromTicks(0)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundCeiling(LocalInstant.FromTicks(1 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundCeiling(LocalInstant.FromTicks(499 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundCeiling(LocalInstant.FromTicks(500 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundCeiling(LocalInstant.FromTicks(501 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundCeiling(LocalInstant.FromTicks(1000 * NodaConstants.TicksPerMillisecond)).Ticks);
        }

        [Test]
        public void RoundHalfFloor_DelegatesToWrappedField()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(0, field.RoundHalfFloor(LocalInstant.FromTicks(0)).Ticks);
            Assert.AreEqual(0, field.RoundHalfFloor(LocalInstant.FromTicks(499 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundHalfFloor(LocalInstant.FromTicks(500 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfFloor(LocalInstant.FromTicks(501 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfFloor(LocalInstant.FromTicks(1000 * NodaConstants.TicksPerMillisecond)).Ticks);
        }

        [Test]
        public void RoundHalfCeiling_DelegatesToWrappedField()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(0, field.RoundHalfCeiling(LocalInstant.FromTicks(0)).Ticks);
            Assert.AreEqual(0, field.RoundHalfCeiling(LocalInstant.FromTicks(499 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfCeiling(LocalInstant.FromTicks(500 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfCeiling(LocalInstant.FromTicks(501 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfCeiling(LocalInstant.FromTicks(1000 * NodaConstants.TicksPerMillisecond)).Ticks);
        }

        [Test]
        public void RoundHalfEven_DelegatesToWrappedField()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(0, field.RoundHalfEven(LocalInstant.FromTicks(0)).Ticks);
            Assert.AreEqual(0, field.RoundHalfEven(LocalInstant.FromTicks(499 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.RoundHalfEven(LocalInstant.FromTicks(500 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfEven(LocalInstant.FromTicks(501 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfEven(LocalInstant.FromTicks(1000 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, field.RoundHalfEven(LocalInstant.FromTicks(1499 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(2 * NodaConstants.TicksPerSecond, field.RoundHalfEven(LocalInstant.FromTicks(1500 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(2 * NodaConstants.TicksPerSecond, field.RoundHalfEven(LocalInstant.FromTicks(1501 * NodaConstants.TicksPerMillisecond)).Ticks);
        }

        [Test]
        public void Remainder_DelegatesToWrappedField()
        {
            OffsetDateTimeField field = GetSampleField();
            Assert.AreEqual(0, field.Remainder(LocalInstant.FromTicks(0)).Ticks);
            Assert.AreEqual(499 * NodaConstants.TicksPerMillisecond, field.Remainder(LocalInstant.FromTicks(499 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(500 * NodaConstants.TicksPerMillisecond, field.Remainder(LocalInstant.FromTicks(500 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(501 * NodaConstants.TicksPerMillisecond, field.Remainder(LocalInstant.FromTicks(501 * NodaConstants.TicksPerMillisecond)).Ticks);
            Assert.AreEqual(0, field.Remainder(LocalInstant.FromTicks(1000 * NodaConstants.TicksPerMillisecond)).Ticks);
        }

        /// <summary>
        /// Helper method to avoid having to repeat all of this every time
        /// </summary>
        private static OffsetDateTimeField GetSampleField()
        {
            return new OffsetDateTimeField(IsoCalendarSystem.Instance.Fields.SecondOfMinute, 3);
        }
    }
}