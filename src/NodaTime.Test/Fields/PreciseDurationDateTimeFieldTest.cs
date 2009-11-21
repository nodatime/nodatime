using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodaTime.Fields;
using NUnit.Framework;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class PreciseDurationDateTimeFieldTest
    {
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
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(DateTimeFieldType.SecondOfMinute, field.FieldType);
        }

        [Test]
        public void IsSupported_ReturnsTrue()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.IsTrue(field.IsSupported);
        }

        [Test]
        public void IsLenient_ReturnsFalse()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.IsFalse(field.IsLenient);
        }

        [Test]
        public void GetValue()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0, field.GetValue(new LocalInstant(0)));
            Assert.AreEqual(1, field.GetValue(new LocalInstant(60)));
            Assert.AreEqual(2, field.GetValue(new LocalInstant(123)));
        }

        [Test]
        public void Add_WithInt32Value()
        {
            MockCountingDurationField.int32Additions = 0;
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(61L, field.Add(new LocalInstant(1L), 1).Ticks);
            Assert.AreEqual(1, MockCountingDurationField.int32Additions);
        }

        [Test]
        public void Add_WithInt64Value()
        {
            MockCountingDurationField.int64Additions = 0;
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(61L, field.Add(new LocalInstant(1L), 1L).Ticks);
            Assert.AreEqual(1, MockCountingDurationField.int64Additions);
        }

        [Test]
        public void GetDifference_DelegatesToDurationField()
        {
            MockCountingDurationField.differences = 0;
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(30, field.GetDifference(new LocalInstant(0), new LocalInstant(0)));
            Assert.AreEqual(1, MockCountingDurationField.differences);
        }

        [Test]
        public void GetInt64Difference_DelegatesToDurationField()
        {
            MockCountingDurationField.differences = 0;
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(30L, field.GetInt64Difference(new LocalInstant(0), new LocalInstant(0)));
            Assert.AreEqual(1, MockCountingDurationField.differences);
        }

        [Test]
        public void SetValue()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0, field.SetValue(new LocalInstant(120L), 0).Ticks);
            Assert.AreEqual(29 * 60, field.SetValue(new LocalInstant(120L), 29).Ticks);
        }

        [Test]
        public void IsLeap_DefaultsToFalse()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.IsFalse(field.IsLeap(new LocalInstant(0L)));
        }

        [Test]
        public void GetLeapAmount_DefaultsTo0()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.GetLeapAmount(new LocalInstant(0L)));
        }

        public void LeapDurationField_DefaultsToNull()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.IsNull(field.LeapDurationField);
        }

        [Test]
        public void GetMinimumValue_DefaultsTo0()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.GetMinimumValue());
        }

        [Test]
        public void RoundFloor()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
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
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
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
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(new LocalInstant(60L)).Ticks);
        }


        [Test]
        public void RoundHalfCeiling()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.RoundHalfCeiling(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfCeiling(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundHalfEven()
        {
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
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
            DateTimeFieldBase field = new StubPreciseDurationDateTimeField();
            Assert.AreEqual(0L, field.Remainder(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(29L, field.Remainder(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(30L, field.Remainder(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(31L, field.Remainder(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(0L, field.Remainder(new LocalInstant(60L)).Ticks);
        }

        private class StubPreciseDurationDateTimeField : PreciseDurationDateTimeField
        {
            internal StubPreciseDurationDateTimeField(DurationField unit)
                : base(DateTimeFieldType.SecondOfMinute, unit)
            {
            }

            internal StubPreciseDurationDateTimeField() 
                : base(DateTimeFieldType.SecondOfMinute, new MockCountingDurationField(DurationFieldType.Seconds))
            {
            }


            public override long GetInt64Value(LocalInstant localInstant)
            {
                return localInstant.Ticks / 60L;
            }

            public override DurationField RangeDurationField
            {
                get { return new MockCountingDurationField(DurationFieldType.Minutes); }
            }

            public override long GetMaximumValue()
            {
                return 59;
            }
        }

        // Class allowing us to simulate bad precision/ticks for constructor testing
        private class FakeDurationField : DurationFieldBase
        {
            private readonly long unitTicks;
            private readonly bool precise;

            internal FakeDurationField(long unitTicks, bool precise)
                : base(DurationFieldType.Seconds)
            {
                this.unitTicks = unitTicks;
                this.precise = precise;
            }

            public override bool IsPrecise { get { return precise; } }

            public override long UnitTicks { get { return unitTicks; } }

            public override long GetInt64Value(Duration duration, LocalInstant localInstant)
            {
                return 0;
            }

            public override Duration GetDuration(long value, LocalInstant localInstant)
            {
                return new Duration(0);
            }

            public override LocalInstant Add(LocalInstant localInstant, int value)
            {
                return new LocalInstant();
            }

            public override LocalInstant Add(LocalInstant localInstant, long value)
            {
                return new LocalInstant();
            }

            public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
            {
                return 0;
            }
        }
    }
}
