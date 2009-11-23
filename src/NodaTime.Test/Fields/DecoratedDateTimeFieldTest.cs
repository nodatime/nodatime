using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodaTime.Fields;
using NUnit.Framework;

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
            Assert.Throws<ArgumentException>(() => new SimpleDecoratedDateTimeField(
                UnsupportedDateTimeField.GetInstance(DateTimeFieldType.MinuteOfDay, UnsupportedDurationField.Minutes),
                DateTimeFieldType.SecondOfMinute));
        }

        [Test]
        public void WrappedField()
        {
            IDateTimeField field = CreateSampleField();
            var decorated = new SimpleDecoratedDateTimeField(field, field.FieldType);
            Assert.AreSame(field, decorated.WrappedField);
        }

        [Test]
        public void FieldType_IsNotDelegated()
        {
            IDateTimeField field = CreateSampleField();
            var decorated = new SimpleDecoratedDateTimeField(field, DateTimeFieldType.YearOfEra);
            Assert.AreEqual(DateTimeFieldType.YearOfEra, decorated.FieldType);
        }

        [Test]
        public void Delegation()
        {
            LocalInstant when1 = new LocalInstant(12345L);
            LocalInstant when2 = new LocalInstant(98765L);
            Duration duration = new Duration(10000L);
            // Just a smattering
            AssertDelegated(x => x.GetDifference(when1, when2));
            AssertDelegated(x => x.GetValue(when1));
            AssertDelegated(x => x.GetInt64Value(when1));
            AssertDelegated(x => x.GetMaximumValue(when1));
            AssertDelegated(x => x.GetMaximumValue());
            AssertDelegated(x => x.SetValue(when1, 100));
            AssertDelegated(x => x.RoundFloor(when1));
        }

        private static void AssertDelegated<T>(Func<IDateTimeField, T> func)
        {
            IDateTimeField field = CreateSampleField();
            var decorated = new SimpleDecoratedDateTimeField(field, DateTimeFieldType.YearOfEra);
            Assert.AreEqual(func(field), func(decorated));
        }

        private static IDateTimeField CreateSampleField()
        {
            return new PreciseDateTimeField(DateTimeFieldType.TickOfMillisecond, 
                TicksDurationField.Instance, PreciseDurationField.Milliseconds);            
        }

        private class SimpleDecoratedDateTimeField : DecoratedDateTimeField
        {
            internal SimpleDecoratedDateTimeField(IDateTimeField wrappedField, DateTimeFieldType fieldType)
                : base(wrappedField, fieldType)
            {
            }
        }
    }
}
