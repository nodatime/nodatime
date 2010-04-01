#region Copyright and license information

// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
using NodaTime.Partials;
using NodaTime.Periods;

using NUnit.Framework;

namespace NodaTime.Test.Partials
{
    [TestFixture]
    public class PartialTest
    {
        private static readonly ICalendarSystem isoCalendar = IsoCalendarSystem.Instance;
        private static readonly ICalendarSystem gregorianCalendar = GregorianCalendarSystem.Default;
        private static readonly DateTimeFieldType[] hourMinuteFieldTypes = new[] { DateTimeFieldType.HourOfDay, DateTimeFieldType.MinuteOfHour };
        private static readonly DateTimeFieldType[] monthDayFieldTypes = new[] { DateTimeFieldType.MonthOfYear, DateTimeFieldType.DayOfMonth };
        private Partial twentyPastTen;
        private Partial twentyPastTenCopy;
        private Partial twentyPastFifteen;
        private Partial december31;
        private Partial midnight;
        private Partial oneToMidnight;

        [SetUp]
        public void SetUp()
        {
            twentyPastTen = CreateHourMinPartial(10, 20, isoCalendar);
            twentyPastTenCopy = CreateHourMinPartial(10, 20, isoCalendar);
            twentyPastFifteen = CreateHourMinPartial(15, 20, gregorianCalendar);
            midnight = CreateHourMinPartial(0, 0, isoCalendar);
            oneToMidnight = CreateHourMinPartial(23, 59, isoCalendar);
            december31 = CreateMonthDayPartial(12, 31, isoCalendar);
        }

        [TearDown]
        public void TearDown()
        {
            AssertImmutability();
        }

        private void AssertImmutability()
        {
            AssertCalendar(isoCalendar, twentyPastTen);
            AssertHourMinute(10, 20, twentyPastTen);

            AssertCalendar(isoCalendar, twentyPastTenCopy);
            AssertHourMinute(10, 20, twentyPastTenCopy);

            AssertCalendar(gregorianCalendar, twentyPastFifteen);
            AssertHourMinute(15, 20, twentyPastFifteen);

            AssertCalendar(isoCalendar, midnight);
            AssertHourMinute(0, 0, midnight);

            AssertCalendar(isoCalendar, midnight);
            AssertHourMinute(0, 0, midnight);

            AssertCalendar(isoCalendar, oneToMidnight);
            AssertHourMinute(23, 59, oneToMidnight);

            AssertCalendar(isoCalendar, december31);
            AssertMonthDay(12, 31, december31);
        }

        private static void AssertCalendar(ICalendarSystem calendar, Partial partial)
        {
            Assert.AreEqual(calendar, @partial.Calendar);
        }

        private static void AssertMonthDay(int month, int day, Partial monthDayPartial)
        {
            CollectionAssert.AreEqual(monthDayFieldTypes, monthDayPartial.GetFieldTypes());
            CollectionAssert.AreEqual(new[] { month, day }, monthDayPartial.GetValues());
        }

        private static void AssertHourMinute(int hour, int minute, Partial hourMinPartial)
        {
            CollectionAssert.AreEqual(hourMinuteFieldTypes, hourMinPartial.GetFieldTypes());
            CollectionAssert.AreEqual(new[] { hour, minute }, hourMinPartial.GetValues());
        }

        private static Partial CreateHourMinPartial(int hour, int min, ICalendarSystem calendarSystem)
        {
            return new Partial(hourMinuteFieldTypes, new[] { hour, min }, calendarSystem);
        }

        private static Partial CreateMonthDayPartial(int month, int day, ICalendarSystem calendarSystem)
        {
            return new Partial(monthDayFieldTypes, new[] { month, day }, calendarSystem);
        }

        [Test]
        public void Get_SupportedField()
        {
            Assert.AreEqual(10, twentyPastTen.Get(DateTimeFieldType.HourOfDay), "hour of day");
            Assert.AreEqual(20, twentyPastTen.Get(DateTimeFieldType.MinuteOfHour), "minute of hour");
        }

        [Test]
        public void Get_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => twentyPastTen.Get(null));
        }

        [Test]
        public void Get_NotSupportedField_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => twentyPastTen.Get(DateTimeFieldType.SecondOfMinute));
        }

        [Test]
        public void Size()
        {
            Assert.AreEqual(2, twentyPastTen.Size);
        }

        [Test]
        public void GetFieldType_ValidIndex()
        {
            Assert.AreSame(DateTimeFieldType.HourOfDay, twentyPastTen.GetFieldType(0), "index 0, hour of day");
            Assert.AreSame(DateTimeFieldType.MinuteOfHour, twentyPastTen.GetFieldType(1), "index 1, minute of hour");
        }

        [Test]
        public void GetFieldType_InvalidIndex_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => twentyPastTen.GetFieldType(-1), "index -1");
            Assert.Throws<ArgumentOutOfRangeException>(() => twentyPastTen.GetFieldType(42), "index 42");
        }

        [Test]
        public void GetFieldTypes()
        {
            var fieldTypes = twentyPastTen.GetFieldTypes();
            CollectionAssert.AreEqual(hourMinuteFieldTypes, fieldTypes);
        }

        [Test]
        public void GetField_ValidIndex()
        {
            Assert.AreSame(isoCalendar.Fields.HourOfDay, twentyPastTen.GetField(0), "index 0, hour of day");
            Assert.AreSame(isoCalendar.Fields.MinuteOfHour, twentyPastTen.GetField(1), "index 1, minute of hour");
        }

        [Test]
        public void GetField_InvalidIndex_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => twentyPastTen.GetField(-1), "index -1");
            Assert.Throws<ArgumentOutOfRangeException>(() => twentyPastTen.GetField(42), "index 42");
        }

        [Test]
        public void GetFields()
        {
            var fields = twentyPastTen.GetFields();
            var expectedFields = new[] { isoCalendar.Fields.HourOfDay, isoCalendar.Fields.MinuteOfHour };
            CollectionAssert.AreEqual(expectedFields, fields);
        }

        [Test]
        public void GetValue_ValidIndex()
        {
            Assert.AreEqual(10, twentyPastTen.GetValue(0), "index 0");
            Assert.AreEqual(20, twentyPastTen.GetValue(1), "index 1");
        }

        [Test]
        public void GetValue_InvalidIndex_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => twentyPastTen.GetValue(-1), "index -1");
            Assert.Throws<ArgumentOutOfRangeException>(() => twentyPastTen.GetValue(42), "index 42");
        }

        [Test]
        public void GetValues()
        {
            var values = twentyPastTen.GetValues();
            CollectionAssert.AreEqual(new[] { 10, 20 }, values);
        }

        [Test]
        public void IsSupported_SupportedField_IsTrue()
        {
            Assert.True(twentyPastTen.IsSupported(DateTimeFieldType.HourOfDay), "hour of day");
            Assert.True(twentyPastTen.IsSupported(DateTimeFieldType.MinuteOfHour), "minute of hour");
        }

        [Test]
        public void IsSupported_NotSupportedField_IsFalse()
        {
            Assert.False(twentyPastTen.IsSupported(DateTimeFieldType.SecondOfMinute), "second of minute");
            Assert.False(twentyPastTen.IsSupported(DateTimeFieldType.TickOfDay), "tick of day");
            Assert.False(twentyPastTen.IsSupported(DateTimeFieldType.DayOfMonth), "day of month");
        }

        [Test]
        public void Equality()
        {
            TestHelper.TestEqualsClass<IPartial>(twentyPastTen, twentyPastTenCopy, twentyPastFifteen);
            TestHelper.TestEqualsClass<IPartial>(twentyPastTen, twentyPastTenCopy, december31);
        }

        [Test]
        public void Comparison_SameFieldTypes()
        {
            TestHelper.TestCompareToClass<IPartial>(twentyPastTen, twentyPastTenCopy, twentyPastFifteen);
        }

        [Test]
        public void Comparison_DifferentFieldTypes_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => december31.CompareTo(twentyPastTen));
        }

        [Test]
        public void WithCalendar_FieldsStillValid()
        {
            Partial december31Gregorian = december31.WithCalendar(gregorianCalendar);
            Assert.AreEqual(gregorianCalendar, december31Gregorian.Calendar);
            CollectionAssert.AreEqual(december31.GetValues(), december31Gregorian.GetValues());
            CollectionAssert.AreEqual(december31.GetFieldTypes(), december31Gregorian.GetFieldTypes());
        }

        [Test, Ignore("Need more calendars")]
        public void WithCalendar_FieldsNoLongerValid_Throws()
        {
            //Assert.Throws<ArgumentException>(() => december31.WithCalendar(ethiopianCalendar));
        }

        [Test]
        public void WithCalendar_SameCalendar_ReturnsSame()
        {
            Partial copy = twentyPastTen.WithCalendar(isoCalendar);
            Assert.AreSame(twentyPastTen, copy);
        }

        [Test]
        public void WithCalendar_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => twentyPastTen.WithCalendar(null));
        }

        [Test]
        public void With_SupportedField()
        {
            Partial tenToEleven = twentyPastTen.With(DateTimeFieldType.MinuteOfHour, 50);
            AssertHourMinute(10, 50, tenToEleven);
        }

        [Test]
        public void With_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => twentyPastTen.With(null, 42));
        }

        [Test]
        public void With_NotSupportedField_AddsFieldOrdered1()
        {
            Partial withDay = twentyPastTen.With(DateTimeFieldType.DayOfMonth, 15);
            var fieldTypes = new[] { DateTimeFieldType.DayOfMonth, DateTimeFieldType.HourOfDay, DateTimeFieldType.MinuteOfHour };
            CollectionAssert.AreEqual(fieldTypes, withDay.GetFieldTypes());
            CollectionAssert.AreEqual(new[] { 15, 10, 20 }, withDay.GetValues());
        }

        [Test]
        public void With_NotSupportedField_AddsFieldOrdered2()
        {
            Partial withMinuteOfDay = twentyPastTen.With(DateTimeFieldType.MinuteOfDay, 15);
            var fieldTypes = new[] { DateTimeFieldType.HourOfDay, DateTimeFieldType.MinuteOfDay, DateTimeFieldType.MinuteOfHour };
            CollectionAssert.AreEqual(fieldTypes, withMinuteOfDay.GetFieldTypes());
            CollectionAssert.AreEqual(new[] { 10, 15, 20 }, withMinuteOfDay.GetValues());
        }

        [Test]
        public void With_NotSupportedField_AddsFieldOrdered3()
        {
            Partial withSeconds = twentyPastTen.With(DateTimeFieldType.SecondOfMinute, 15);
            var fieldTypes = new[] { DateTimeFieldType.HourOfDay, DateTimeFieldType.MinuteOfHour, DateTimeFieldType.SecondOfMinute };
            CollectionAssert.AreEqual(fieldTypes, withSeconds.GetFieldTypes());
            CollectionAssert.AreEqual(new[] { 10, 20, 15 }, withSeconds.GetValues());
        }

        [Test]
        public void With_SameFieldAndValue_ReturnsSame()
        {
            Partial copy = twentyPastTen.With(DateTimeFieldType.HourOfDay, 10);
            Assert.AreSame(twentyPastTen, copy);
        }

        [Test]
        public void With_OutOfRange_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => twentyPastTen.With(DateTimeFieldType.MinuteOfHour, 70));
        }

        [Test]
        public void Without_NotSupportedField_ReturnsSame()
        {
            Partial copy = twentyPastTen.Without(DateTimeFieldType.Year);
            Assert.AreSame(twentyPastTen, copy);
        }

        [Test]
        public void Without_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => twentyPastTen.Without(null));
        }

        [Test]
        public void Without_SupportedField_RemovesField()
        {
            Partial minutesOnly = twentyPastTen.Without(DateTimeFieldType.HourOfDay);
            Assert.AreEqual(1, minutesOnly.Size);
            Assert.AreEqual(DateTimeFieldType.MinuteOfHour, minutesOnly.GetFieldType(0));
            Assert.AreEqual(20, minutesOnly.GetValue(0));
        }

        [Test]
        public void Without_LastField()
        {
            Partial hoursOnly = new Partial(new[] { DateTimeFieldType.HourOfDay }, new[] { 10 }, isoCalendar);
            Partial nothing = hoursOnly.Without(DateTimeFieldType.HourOfDay);
            Assert.AreEqual(0, nothing.Size);
            Assert.False(nothing.IsSupported(DateTimeFieldType.HourOfDay));
        }

        [Test]
        public void WithField_SupportedField()
        {
            Partial twentyPastNoon = twentyPastTen.WithField(DateTimeFieldType.HourOfDay, 12);
            AssertHourMinute(12, 20, twentyPastNoon);
        }

        [Test]
        public void WithField_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => twentyPastTen.WithField(null, 42));
        }

        [Test]
        public void WithField_NotSupportedField_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => twentyPastTen.WithField(DateTimeFieldType.DayOfMonth, 6));
        }

        [Test]
        public void WithField_SameValue_ReturnsSame()
        {
            Partial copy = twentyPastTen.WithField(DateTimeFieldType.HourOfDay, 10);
            Assert.AreSame(twentyPastTen, copy);
        }

        [Test]
        public void WithFieldAdded_SupportedField()
        {
            Partial tenToEleven = twentyPastTen.WithFieldAdded(DurationFieldType.Minutes, 30);
            AssertHourMinute(10, 50, tenToEleven);
        }

        [Test]
        public void WithFieldAdded_Zero_ReturnsSame()
        {
            var copy = twentyPastTen.WithFieldAdded(DurationFieldType.Hours, 0);
            Assert.AreSame(twentyPastTen, copy);
        }

        [Test]
        public void WithFieldAdded_NotSupportedField_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => twentyPastTen.WithFieldAdded(DurationFieldType.Days, 6));
        }

        [Test]
        public void WithFieldAdded_OverflowLargestField_Throws()
        {
            Assert.Throws<ArgumentException>(() => twentyPastTen.WithFieldAdded(DurationFieldType.Hours, 16));
            //TODO: Why does this return January 1 instead of throwing due to overflow?
            // Commented out to remain compliant with Joda Time behavior
            //Assert.Throws<ArgumentException>(() => december31.WithFieldAdded(DurationFieldType.Days, 1));
        }

        [Test]
        public void WithFieldAdded_OverflowNonLargestField_Carries()
        {
            var elevenOClock = twentyPastTen.WithFieldAdded(DurationFieldType.Minutes, 40);
            AssertHourMinute(11, 0, elevenOClock);
        }

        [Test]
        public void WithFieldAdded_UnderflowLargestField_Throws()
        {
            Assert.Throws<ArgumentException>(() => twentyPastTen.WithFieldAdded(DurationFieldType.Hours, -11));
            Assert.Throws<ArgumentException>(() => midnight.WithFieldAdded(DurationFieldType.Minutes, -1));
        }

        [Test]
        public void WithFieldAdded_UnderflowNonLargestField_Carries()
        {
            Partial fiveToTen = twentyPastTen.WithFieldAdded(DurationFieldType.Minutes, -25);
            AssertHourMinute(9, 55, fiveToTen);
        }
        
        [Test]
        public void WithFieldAddWrapped_SupportedField()
        {
            Partial twentyPastNoon = twentyPastTen.WithFieldAddWrapped(DurationFieldType.Hours, 2);
            AssertHourMinute(12, 20, twentyPastNoon);
        }

        [Test]
        public void WithFieldAddWrapped_NotSupportedField_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => twentyPastTen.WithFieldAddWrapped(DurationFieldType.Days, 1));
        }

        [Test]
        public void WithFieldAddWrapped_Zero_ReturnsSame()
        {
            Partial copy = twentyPastTen.WithFieldAddWrapped(DurationFieldType.Hours, 0);
            Assert.AreSame(twentyPastTen, copy);
        }

        [Test]
        public void WithFieldAddWrapped_OverflowLargestField_Wraps()
        {
            Partial twentyPastTwo = twentyPastTen.WithFieldAddWrapped(DurationFieldType.Hours, 16);
            AssertHourMinute(2, 20, twentyPastTwo);
            Partial onePastMidnight = oneToMidnight.WithFieldAddWrapped(DurationFieldType.Minutes, 2);
            AssertHourMinute(0, 1, onePastMidnight);
        }

        [Test]
        public void WithFieldAddWrapped_OverflowNonLargestField_Carries()
        {
            Partial tenPastEleven = twentyPastTen.WithFieldAddWrapped(DurationFieldType.Minutes, 50);
            AssertHourMinute(11, 10, tenPastEleven);
        }

        [Test]
        public void WithFieldAddWrapped_UnderflowLargestField_Wraps()
        {
            Partial twentyPastTwentyTwo = twentyPastTen.WithFieldAddWrapped(DurationFieldType.Hours, -12);
            AssertHourMinute(22, 20, twentyPastTwentyTwo);
            Partial twoToMidnight = midnight.WithFieldAddWrapped(DurationFieldType.Minutes, -2);
            AssertHourMinute(23, 58, twoToMidnight);
        }

        [Test]
        public void WithFieldAddWrapped_UnderflowNonLargestField_Carries()
        {
            Partial halfPastNine = twentyPastTen.WithFieldAddWrapped(DurationFieldType.Minutes, -50);
            AssertHourMinute(9, 30, halfPastNine);
        }

        [Test]
        public void WithPeriodAdded_IgnoresNotSupportedFields()
        {
            Period oneDayOneHourFiveMinutes = Period.FromDays(1).WithHours(1).WithMinutes(5);
            Partial halfPastNoon = twentyPastTen.WithPeriodAdded(oneDayOneHourFiveMinutes, 2);
            AssertHourMinute(12, 30, halfPastNoon);

            Partial tenPastEight = twentyPastTen.WithPeriodAdded(oneDayOneHourFiveMinutes, -2);
            AssertHourMinute(8, 10, tenPastEight);
        }

        [Test]
        public void WithPeriodAdded_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => twentyPastTen.WithPeriodAdded(null, 42));
        }

        [Test]
        public void WithPeriodAdded_ZeroFactor_ReturnsSame()
        {
            Period oneDayOneHourFiveMinutes = Period.FromDays(1).WithHours(1).WithMinutes(5);
            Partial copy2 = twentyPastTen.WithPeriodAdded(oneDayOneHourFiveMinutes, 0);
            Assert.AreSame(twentyPastTen, copy2);
        }

        [Test]
        public void Plus_IgnoresNotSupportedFields()
        {
            Period oneDayTwoHoursTenMinutes = Period.FromDays(1).WithHours(2).WithMinutes(10);
            Partial halfPastNoon = twentyPastTen.Plus(oneDayTwoHoursTenMinutes);
            AssertHourMinute(12, 30, halfPastNoon);
        }

        [Test]
        public void Plus_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => twentyPastTen.Plus(null));
        }

        [Test]
        public void Minus_IgnoresNotSupportedFields()
        {
            Period oneDayTwoHoursTenMinutes = Period.FromDays(1).WithHours(2).WithMinutes(10);
            Partial tenPastEight = twentyPastTen.Minus(oneDayTwoHoursTenMinutes);
            AssertHourMinute(8, 10, tenPastEight);
        }

        [Test]
        public void Minus_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => twentyPastTen.Minus(null));
        }

        /* TODO: Why does Partial take a ICalendarSystem instead of a Chronology?
         * ToDateTime (probably should be renamed to ToZonedDateTime) depends on this
    //-----------------------------------------------------------------------
    public void testToDateTime_RI() {
        Partial base = createHourMinPartial(COPTIC_PARIS);
        DateTime dt = new DateTime(0L); // LONDON zone
        assertEquals("1970-01-01T01:00:00.000+01:00", dt.toString());
        
        DateTime test = base.toDateTime(dt);
        check(base, 10, 20);
        assertEquals("1970-01-01T01:00:00.000+01:00", dt.toString());
        assertEquals("1970-01-01T10:20:00.000+01:00", test.toString());
    }

    public void testToDateTime_nullRI() {
        Partial base = createHourMinPartial(1, 2, ISO_UTC);
        DateTimeUtils.setCurrentMillisFixed(TEST_TIME2);
        
        DateTime test = base.toDateTime((ReadableInstant) null);
        check(base, 1, 2);
        assertEquals("1970-01-02T01:02:07.008+01:00", test.toString());
    }
*/
    }
}