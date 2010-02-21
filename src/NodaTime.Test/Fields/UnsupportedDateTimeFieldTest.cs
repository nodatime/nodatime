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
    public class UnsupportedDateTimeFieldTest
    {
        [Test]
        public void GetInstance_WithNullDurationField_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                UnsupportedDateTimeField.GetInstance(DateTimeFieldType.SecondOfMinute, null));
        }

        [Test]
        public void GetInstance_WithInvalidDateTimeFieldType_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                UnsupportedDateTimeField.GetInstance(null, UnsupportedDurationField.Years));
        }

        [Test]
        public void GetInstance_Caching()
        {
            DurationField months = UnsupportedDurationField.Months;
            DurationField years = UnsupportedDurationField.Years;

            IDateTimeField field1 = UnsupportedDateTimeField.GetInstance(DateTimeFieldType.MonthOfYear, months);
            IDateTimeField field2 = UnsupportedDateTimeField.GetInstance(DateTimeFieldType.MonthOfYear, months);
            IDateTimeField field3 = UnsupportedDateTimeField.GetInstance(DateTimeFieldType.MonthOfYear, years);
            IDateTimeField field4 = UnsupportedDateTimeField.GetInstance(DateTimeFieldType.MonthOfYear, years);
            IDateTimeField field5 = UnsupportedDateTimeField.GetInstance(DateTimeFieldType.YearOfCentury, years);

            Assert.AreSame(field1, field2);
            Assert.AreNotSame(field2, field3);
            Assert.AreSame(field3, field4);
            Assert.AreNotSame(field4, field5);
        }

        [Test]
        public void GetInstance_ReturnsCorrectValues()
        {
            IDateTimeField field = UnsupportedDateTimeField.GetInstance(
                DateTimeFieldType.MonthOfYear, UnsupportedDurationField.Years);

            Assert.AreEqual(DateTimeFieldType.MonthOfYear, field.FieldType);
            Assert.AreSame(UnsupportedDurationField.Years, field.DurationField);
        }

        [Test]
        public void UnsupportedOperations_ThrowNotSupportedException()
        {
            LocalInstant when = new LocalInstant();
            AssertUnsupported(x => x.GetInt64Value(when));
            AssertUnsupported(x => x.GetLeapAmount(when));
            AssertUnsupported(x => x.GetMaximumValue());
            AssertUnsupported(x => x.GetMaximumValue(when));
            AssertUnsupported(x => x.GetMinimumValue());
            AssertUnsupported(x => x.GetMinimumValue(when));
            AssertUnsupported(x => x.GetValue(when));
            AssertUnsupported(x => x.IsLeap(when));
            AssertUnsupported(x => x.Remainder(when));
            AssertUnsupported(x => x.RoundCeiling(when));
            AssertUnsupported(x => x.RoundFloor(when));
            AssertUnsupported(x => x.RoundHalfCeiling(when));
            AssertUnsupported(x => x.RoundHalfEven(when));
            AssertUnsupported(x => x.RoundHalfFloor(when));
            AssertUnsupported(x => x.SetValue(when, 0L));
        }

        [Test]
        public void ConstantProperties_ReturnExpectedValues()
        {
            IDateTimeField field = UnsupportedDateTimeField.GetInstance(
                DateTimeFieldType.MonthOfYear, UnsupportedDurationField.Years);
            Assert.IsFalse(field.IsLenient);
            Assert.IsFalse(field.IsSupported);
            Assert.IsNull(field.LeapDurationField);
            Assert.IsNull(field.RangeDurationField);
        }

        private static void AssertUnsupported(Action<IDateTimeField> action)
        {
            IDateTimeField field = UnsupportedDateTimeField.GetInstance(DateTimeFieldType.MonthOfYear,
                new MockCountingDurationField(DurationFieldType.Seconds));
            Assert.Throws<NotSupportedException>(() => action(field));
        }

        [Test]
        public void AddInt32_DelegatesToDurationField()
        {
            MockCountingDurationField.int32Additions = 0;
            IDateTimeField field = UnsupportedDateTimeField.GetInstance(
                DateTimeFieldType.MonthOfYear, new MockCountingDurationField(DurationFieldType.Seconds));
            Assert.AreEqual(61, field.Add(new LocalInstant(1), 1).Ticks);
            Assert.AreEqual(1, MockCountingDurationField.int32Additions);
        }

        [Test]
        public void AddInt64_DelegatesToDurationField()
        {
            MockCountingDurationField.int64Additions = 0;
            IDateTimeField field = UnsupportedDateTimeField.GetInstance(
                DateTimeFieldType.MonthOfYear, new MockCountingDurationField(DurationFieldType.Seconds));
            Assert.AreEqual(61, field.Add(new LocalInstant(1), 1L).Ticks);
            Assert.AreEqual(1, MockCountingDurationField.int64Additions);
        }

        [Test]
        public void GetDifference_DelegatesToDurationFieldGetDifference()
        {
            MockCountingDurationField.differences = 0;
            IDateTimeField field = UnsupportedDateTimeField.GetInstance(
                DateTimeFieldType.MonthOfYear, new MockCountingDurationField(DurationFieldType.Seconds));
            Assert.AreEqual(30, field.GetDifference(new LocalInstant(), new LocalInstant()));
            Assert.AreEqual(1, MockCountingDurationField.differences);
        }

        [Test]
        public void GetInt64Difference_DelegatesToDurationFieldGetInt64Difference()
        {
            MockCountingDurationField.differences64 = 0;
            IDateTimeField field = UnsupportedDateTimeField.GetInstance(
                DateTimeFieldType.MonthOfYear, new MockCountingDurationField(DurationFieldType.Seconds));
            Assert.AreEqual(30, field.GetInt64Difference(new LocalInstant(), new LocalInstant()));
            Assert.AreEqual(1, MockCountingDurationField.differences64);
        }

    }
}
