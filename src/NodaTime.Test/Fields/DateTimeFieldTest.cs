// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    // TODO(Post-V1): Refactor a lot of these tests: there's a lot of duplication down the hierarchy.
    [TestFixture]
    public class DateTimeFieldTest
    {
        [Test]
        public void Constructor_WithInvalidType_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentNullException>(() => new StubDateTimeField(null));
        }

        [Test]
        public void Constructor_WithValidType_RemembersType()
        {
            DateTimeField field = new StubDateTimeField(DateTimeFieldType.WeekYear);
            Assert.AreEqual(DateTimeFieldType.WeekYear, field.FieldType);
        }

        [Test]
        public void IsSupported_DefaultsToTrue()
        {
            DateTimeField field = new StubDateTimeField();
            Assert.IsTrue(field.IsSupported);
        }

        #region Values
        [Test]
        public void GetValue_DelegatesToGetInt64Value()
        {
            var field = new StubDateTimeField();
            var arg = new LocalInstant(60);

            field.GetValue(arg);

            Assert.That(field.GetInt64ValueWasCalled, Is.True);
            Assert.That(field.GetInt64ValueArg, Is.EqualTo(arg));
        }

        [Test]
        public void SetValue_InvalidFromSmaller_Adjusts()
        {
            // Field will adjust *smaller* units to make things valid
            LocalInstant jan30th = new LocalDate(2001, 1, 30).AtMidnight().LocalInstant;
            LocalInstant actual = CalendarSystem.Iso.Fields.MonthOfYear.SetValue(jan30th, 2);
            LocalInstant expected = new LocalDate(2001, 2, 28).AtMidnight().LocalInstant;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SetValue_InvalidFromLarger_Throws()
        {
            // Field cannot adjust *larger* units
            LocalInstant feb20th = new LocalDate(2001, 2, 20).AtMidnight().LocalInstant;
            Assert.Throws<ArgumentOutOfRangeException>(() => CalendarSystem.Iso.Fields.DayOfMonth.SetValue(feb20th, 30));
        }
        #endregion

        #region Ranges
        [Test]
        public void GetMinimumValue_OnStub_DefaultsTo0()
        {
            var field = new StubDateTimeField();
            Assert.AreEqual(0L, field.GetMinimumValue());
            Assert.That(field.GetMinWasCalled, Is.True);
        }

        [Test]
        public void GetMinimumValueForInstant_DelegatesToAbsolute()
        {
            var field = new StubDateTimeField();
            Assert.AreEqual(0L, field.GetMinimumValue(new LocalInstant(0)));
            Assert.That(field.GetMinWasCalled, Is.True);
        }

        [Test]
        public void GetMaximumValue_OnStub_DefaultsTo59()
        {
            var field = new StubDateTimeField();
            Assert.AreEqual(59L, field.GetMaximumValue());
            Assert.That(field.GetMaxWasCalled, Is.True);
        }

        [Test]
        public void GetMaximumValueForInstant_DelegatesToAbsolute()
        {
            var field = new StubDateTimeField();
            Assert.AreEqual(59L, field.GetMaximumValue(new LocalInstant(0)));
            Assert.That(field.GetMaxWasCalled, Is.True);
        }
        #endregion

        #region Rounding
        [Test]
        public void RoundFloor_OnStub_RoundsTo60()
        {
            DateTimeField field = new StubDateTimeField();
            Assert.AreEqual(0L, field.RoundFloor(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundFloor(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundCeiling()
        {
            DateTimeField field = new StubDateTimeField();
            Assert.AreEqual(0L, field.RoundCeiling(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundHalfFloor()
        {
            DateTimeField field = new StubDateTimeField();
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundHalfCeiling()
        {
            DateTimeField field = new StubDateTimeField();
            Assert.AreEqual(0L, field.RoundHalfCeiling(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfCeiling(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(new LocalInstant(60L)).Ticks);
        }

        [Test]
        public void RoundHalfEven()
        {
            DateTimeField field = new StubDateTimeField();
            Assert.AreEqual(0L, field.RoundHalfEven(new LocalInstant(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfEven(new LocalInstant(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfEven(new LocalInstant(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfEven(new LocalInstant(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfEven(new LocalInstant(60L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfEven(new LocalInstant(89L)).Ticks);
            Assert.AreEqual(120L, field.RoundHalfEven(new LocalInstant(90L)).Ticks);
            Assert.AreEqual(120L, field.RoundHalfEven(new LocalInstant(91L)).Ticks);
        }
        #endregion

        private class StubDateTimeField : DateTimeField
        {
            internal StubDateTimeField(DateTimeFieldType type) : base(type, new MockCountingPeriodField(PeriodFieldType.Seconds))
            {
            }

            internal StubDateTimeField()
                : this(DateTimeFieldType.SecondOfMinute)
            {
            }

            public bool GetInt64ValueWasCalled;
            public LocalInstant GetInt64ValueArg;

            internal override long GetInt64Value(LocalInstant localInstant)
            {
                GetInt64ValueWasCalled = true;
                GetInt64ValueArg = localInstant;

                return localInstant.Ticks / 60L;
            }

            internal override LocalInstant SetValue(LocalInstant localInstant, long value)
            {
                return localInstant;
            }

            public bool GetMaxWasCalled;

            internal override long GetMaximumValue()
            {
                GetMaxWasCalled = true;
                return 59;
            }

            public bool GetMinWasCalled;

            internal override long GetMinimumValue()
            {
                GetMinWasCalled = true;
                return 0;
            }

            internal override LocalInstant RoundFloor(LocalInstant localInstant)
            {
                return new LocalInstant((localInstant.Ticks / 60L) * 60L);
            }
        }
    }
}