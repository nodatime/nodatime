// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class UnsupportedPeriodFieldTest
    {
        [Test]
        public void ConstantProperties_ReturnExpectedValues()
        {
            PeriodField field = UnsupportedPeriodField.Seconds;
            Assert.IsFalse(field.IsSupported);
            Assert.IsTrue(field.IsFixedLength);
            Assert.AreEqual(0, field.UnitTicks);
        }

        [Test]
        public void CachedValuesAreSingletons()
        {
            PeriodField field1 = UnsupportedPeriodField.Seconds;
            PeriodField field2 = UnsupportedPeriodField.ForFieldType(PeriodFieldType.Seconds);
            PeriodField field3 = UnsupportedPeriodField.ForFieldType(PeriodFieldType.Seconds);

            Assert.AreSame(field1, field2);
            Assert.AreSame(field1, field3);
        }

        [Test]
        public void UnsupportedOperations_ThrowNotSupportedException()
        {
            LocalInstant when = new LocalInstant();
            Duration duration = new Duration();

            AssertUnsupported(x => x.Add(when, 0));
            AssertUnsupported(x => x.Add(when, 0L));
            AssertUnsupported(x => x.GetDifference(when, when));
            AssertUnsupported(x => x.GetDuration(10));
            AssertUnsupported(x => x.GetDuration(10, when));
            AssertUnsupported(x => x.GetInt64Difference(when, when));
            AssertUnsupported(x => x.GetInt64Value(duration, when));
        }

        private static void AssertUnsupported(Action<UnsupportedPeriodField> action)
        {
            Assert.Throws<NotSupportedException>(() => action(UnsupportedPeriodField.Seconds));
        }
    }
}