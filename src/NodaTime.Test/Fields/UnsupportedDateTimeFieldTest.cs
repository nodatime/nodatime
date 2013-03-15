// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class UnsupportedDateTimeFieldTest
    {
        [Test]
        public void UnsupportedOperations_ThrowNotSupportedException()
        {
            LocalInstant when = new LocalInstant();
            AssertUnsupported(x => x.GetInt64Value(when));
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
            DateTimeField field = UnsupportedDateTimeField.MonthOfYear;
            Assert.IsFalse(field.IsSupported);
        }

        private static void AssertUnsupported(Action<DateTimeField> action)
        {
            DateTimeField field = UnsupportedDateTimeField.MonthOfYear;
            Assert.Throws<NotSupportedException>(() => action(field));
        }
    }
}