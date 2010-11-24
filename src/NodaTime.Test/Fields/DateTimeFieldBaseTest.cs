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
    // TODO: Refactor a lot of these tests: there's a lot of duplication down the hierarchy.
    [TestFixture]
    public class DateTimeFieldBaseTest
    {
        [Test]
        public void Constructor_WithInvalidType_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentNullException>(() => new StubDateTimeFieldBase(null));
        }

        [Test]
        public void Constructor_WithValidType_RemembersType()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase(DateTimeFieldType.WeekYear);
            Assert.AreEqual(DateTimeFieldType.WeekYear, field.FieldType);
        }

        [Test]
        public void IsSupported_DefaultsToTrue()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.IsTrue(field.IsSupported);
        }

        #region Values
        [Test]
        public void GetValue_DelegatesToGetInt64Value()
        {
            var field = new StubDateTimeFieldBase();
            var arg = LocalInstant.FromTicks(60);

            field.GetValue(arg);

            Assert.That(field.GetInt64ValueWasCalled, Is.True);
            Assert.That(field.GetInt64ValueArg, Is.EqualTo(arg));
        }

        [Test]
        public void AddInt32_DelegatesToDurationField()
        {
            MockCountingDurationField.int32Additions = 0;
            var instantArg = LocalInstant.FromTicks(1);
            var valueArg = 1;
            var field = new StubDateTimeFieldBase();

            field.Add(instantArg, valueArg);

            Assert.That(MockCountingDurationField.int32Additions, Is.EqualTo(1));
            Assert.That(MockCountingDurationField.AddInstantArg, Is.EqualTo(instantArg));
            Assert.That(MockCountingDurationField.AddValueArg, Is.EqualTo(valueArg));
        }

        [Test]
        public void AddInt64_DelegatesToDurationField()
        {
            MockCountingDurationField.int64Additions = 0;
            var instantArg = LocalInstant.FromTicks(2);
            var valueArg = 5L;
            var field = new StubDateTimeFieldBase();

            field.Add(instantArg, valueArg);

            Assert.That(MockCountingDurationField.int64Additions, Is.EqualTo(1));
            Assert.That(MockCountingDurationField.Add64InstantArg, Is.EqualTo(instantArg));
            Assert.That(MockCountingDurationField.Add64ValueArg, Is.EqualTo(valueArg));
        }

        [Test]
        public void GetDifference_DelegatesToDurationFieldGetDifference()
        {
            MockCountingDurationField.differences = 0;
            var firstInstant = LocalInstant.FromTicks(2);
            var secondInstant = LocalInstant.FromTicks(3);
            var field = new StubDateTimeFieldBase();

            field.GetDifference(firstInstant, secondInstant);

            Assert.That(MockCountingDurationField.differences, Is.EqualTo(1));
            Assert.That(MockCountingDurationField.DiffFirstArg, Is.EqualTo(firstInstant));
            Assert.That(MockCountingDurationField.DiffSecondArg, Is.EqualTo(secondInstant));
        }

        [Test]
        public void GetInt64Difference_DelegatesToDurationFieldGetInt64Difference()
        {
            MockCountingDurationField.differences64 = 0;
            var firstInstant = LocalInstant.FromTicks(4);
            var secondInstant = LocalInstant.FromTicks(5);
            var field = new StubDateTimeFieldBase();

            field.GetInt64Difference(firstInstant, secondInstant);

            Assert.That(MockCountingDurationField.differences64, Is.EqualTo(1));
            Assert.That(MockCountingDurationField.Diff64FirstArg, Is.EqualTo(firstInstant));
            Assert.That(MockCountingDurationField.Diff64SecondArg, Is.EqualTo(secondInstant));
        }
        #endregion

        #region Leap
        [Test]
        public void IsLeap_DefaultsToFalse()
        {
            var field = new StubDateTimeFieldBase();
            Assert.IsFalse(field.IsLeap(LocalInstant.FromTicks(0)));
        }

        [Test]
        public void GetLeapAmount_DefaultsTo0()
        {
            var field = new StubDateTimeFieldBase();
            Assert.AreEqual(0, field.GetLeapAmount(LocalInstant.FromTicks(0)));
        }

        [Test]
        public void LeapDurationField_DefaultsToNull()
        {
            var field = new StubDateTimeFieldBase();
            Assert.IsNull(field.LeapDurationField);
        }
        #endregion

        #region Ranges
        [Test]
        public void GetMinimumValue_OnStub_DefaultsTo0()
        {
            var field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.GetMinimumValue());
            Assert.That(field.GetMinWasCalled, Is.True);
        }

        [Test]
        public void GetMinimumValueForInstant_DelegatesToAbsolute()
        {
            var field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.GetMinimumValue(LocalInstant.FromTicks(0)));
            Assert.That(field.GetMinWasCalled, Is.True);
        }

        [Test]
        public void GetMaximumValue_OnStub_DefaultsTo59()
        {
            var field = new StubDateTimeFieldBase();
            Assert.AreEqual(59L, field.GetMaximumValue());
            Assert.That(field.GetMaxWasCalled, Is.True);
        }

        [Test]
        public void GetMaximumValueForInstant_DelegatesToAbsolute()
        {
            var field = new StubDateTimeFieldBase();
            Assert.AreEqual(59L, field.GetMaximumValue(LocalInstant.FromTicks(0)));
            Assert.That(field.GetMaxWasCalled, Is.True);
        }
        #endregion

        #region Rounding
        [Test]
        public void RoundFloor_OnStub_RoundsTo60()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.RoundFloor(LocalInstant.FromTicks(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(LocalInstant.FromTicks(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(LocalInstant.FromTicks(30L)).Ticks);
            Assert.AreEqual(0L, field.RoundFloor(LocalInstant.FromTicks(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundFloor(LocalInstant.FromTicks(60L)).Ticks);
        }

        [Test]
        public void RoundCeiling()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.RoundCeiling(LocalInstant.FromTicks(0L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(LocalInstant.FromTicks(29L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(LocalInstant.FromTicks(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(LocalInstant.FromTicks(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundCeiling(LocalInstant.FromTicks(60L)).Ticks);
        }

        [Test]
        public void RoundHalfFloor()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.RoundHalfFloor(LocalInstant.FromTicks(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(LocalInstant.FromTicks(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfFloor(LocalInstant.FromTicks(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(LocalInstant.FromTicks(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfFloor(LocalInstant.FromTicks(60L)).Ticks);
        }

        [Test]
        public void RoundHalfCeiling()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.RoundHalfCeiling(LocalInstant.FromTicks(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfCeiling(LocalInstant.FromTicks(29L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(LocalInstant.FromTicks(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(LocalInstant.FromTicks(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfCeiling(LocalInstant.FromTicks(60L)).Ticks);
        }

        [Test]
        public void RoundHalfEven()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.RoundHalfEven(LocalInstant.FromTicks(0L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfEven(LocalInstant.FromTicks(29L)).Ticks);
            Assert.AreEqual(0L, field.RoundHalfEven(LocalInstant.FromTicks(30L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfEven(LocalInstant.FromTicks(31L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfEven(LocalInstant.FromTicks(60L)).Ticks);
            Assert.AreEqual(60L, field.RoundHalfEven(LocalInstant.FromTicks(89L)).Ticks);
            Assert.AreEqual(120L, field.RoundHalfEven(LocalInstant.FromTicks(90L)).Ticks);
            Assert.AreEqual(120L, field.RoundHalfEven(LocalInstant.FromTicks(91L)).Ticks);
        }

        [Test]
        public void Remainder()
        {
            DateTimeFieldBase field = new StubDateTimeFieldBase();
            Assert.AreEqual(0L, field.Remainder(LocalInstant.FromTicks(0L)).Ticks);
            Assert.AreEqual(29L, field.Remainder(LocalInstant.FromTicks(29L)).Ticks);
            Assert.AreEqual(30L, field.Remainder(LocalInstant.FromTicks(30L)).Ticks);
            Assert.AreEqual(31L, field.Remainder(LocalInstant.FromTicks(31L)).Ticks);
            Assert.AreEqual(0L, field.Remainder(LocalInstant.FromTicks(60L)).Ticks);
        }
        #endregion

        private class StubDateTimeFieldBase : DateTimeFieldBase
        {
            internal StubDateTimeFieldBase(DateTimeFieldType type) : base(type)
            {
            }

            internal StubDateTimeFieldBase() : base(DateTimeFieldType.SecondOfMinute)
            {
            }

            public bool GetInt64ValueWasCalled;
            public LocalInstant GetInt64ValueArg;

            public override long GetInt64Value(LocalInstant localInstant)
            {
                GetInt64ValueWasCalled = true;
                GetInt64ValueArg = localInstant;

                return localInstant.Ticks / 60L;
            }

            public override LocalInstant SetValue(LocalInstant localInstant, long value)
            {
                return localInstant;
            }

            public override IDurationField DurationField { get { return new MockCountingDurationField(DurationFieldType.Seconds); } }

            public override IDurationField RangeDurationField { get { return new MockCountingDurationField(DurationFieldType.Minutes); } }

            public bool GetMaxWasCalled;

            public override long GetMaximumValue()
            {
                GetMaxWasCalled = true;
                return 59;
            }

            public bool GetMinWasCalled;

            public override long GetMinimumValue()
            {
                GetMinWasCalled = true;
                return 0;
            }

            public override LocalInstant RoundFloor(LocalInstant localInstant)
            {
                return LocalInstant.FromTicks((localInstant.Ticks / 60L) * 60L);
            }

            public override bool IsLenient { get { return false; } }
        }
    }
}