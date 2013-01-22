// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class DecoratedDateTimeFieldTest
    {
        [Test]
        public void Constructor_WithNullWrappedField_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SimpleDecoratedDateTimeField(null, DateTimeFieldType.SecondOfMinute));
        }

        [Test]
        public void Constructor_WithUnsupportedWrappedField_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => new SimpleDecoratedDateTimeField(UnsupportedDateTimeField.MinuteOfDay, DateTimeFieldType.SecondOfMinute));
        }

        [Test]
        public void WrappedField()
        {
            DateTimeField field = CreateSampleField();
            var decorated = new SimpleDecoratedDateTimeField(field, field.FieldType);
            Assert.AreSame(field, decorated.WrappedField);
        }

        [Test]
        public void FieldType_IsNotDelegated()
        {
            DateTimeField field = CreateSampleField();
            var decorated = new SimpleDecoratedDateTimeField(field, DateTimeFieldType.YearOfEra);
            Assert.AreEqual(DateTimeFieldType.YearOfEra, decorated.FieldType);
        }

        [Test]
        public void Delegation()
        {
            LocalInstant when = new LocalInstant(12345L);
            // Just a smattering
            AssertDelegated(x => x.GetValue(when));
            AssertDelegated(x => x.GetInt64Value(when));
            AssertDelegated(x => x.GetMaximumValue(when));
            AssertDelegated(x => x.GetMaximumValue());
            AssertDelegated(x => x.SetValue(when, 100));
            AssertDelegated(x => x.RoundFloor(when));
        }

        private static void AssertDelegated<T>(Func<DateTimeField, T> func)
        {
            DateTimeField field = CreateSampleField();
            var decorated = new SimpleDecoratedDateTimeField(field, DateTimeFieldType.YearOfEra);
            Assert.AreEqual(func(field), func(decorated));
        }

        private static DateTimeField CreateSampleField()
        {
            return new FixedLengthDateTimeField(DateTimeFieldType.TickOfMillisecond, TicksPeriodField.Instance, FixedLengthPeriodField.Milliseconds);
        }

        private class SimpleDecoratedDateTimeField : DecoratedDateTimeField
        {
            internal SimpleDecoratedDateTimeField(DateTimeField wrappedField, DateTimeFieldType fieldType) : base(wrappedField, fieldType)
            {
            }
        }
    }
}