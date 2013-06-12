// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NodaTime.Fields;
using NodaTime.Text;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NodaTime.Test.Calendars
{
    [TestFixture]
    public class CalculatorCalendarSystemTest
    {
        private static readonly CalendarSystem[] GregorianLikeOldCalendarSystems = 
            Enumerable.Range(1, 7)
                      .Select(x => GregorianCalendarSystem.GetInstance(x))
                      .Concat(new[] { GregorianCalendarSystem.IsoHelper.Instance })
                      .ToArray();

        private static readonly CalendarSystem[] GregorianLikeNewCalendarSystems =
            Enumerable.Range(1, 7)
                      .Select(x => new CalculatorCalendarSystem("greg " + x, "ignored", GregorianYearMonthDayCalculator.Instance, x))
                      .Concat(new[] { new CalculatorCalendarSystem("ignored", "ignored", IsoYearMonthDayCalculator.IsoInstance, 4), })
                      .ToArray();

        private static readonly CalendarSystem[] CopticOldCalendarSystems =
            Enumerable.Range(1, 7)
                      .Select(x => CopticCalendarSystem.GetInstance(x))
                      .ToArray();

        private static readonly CalendarSystem[] CopticNewCalendarSystems =
            Enumerable.Range(1, 7)
                      .Select(x => new CalculatorCalendarSystem("coptic " + x, "ignored", CopticYearMonthDayCalculator.CopticInstance, x))
                      .ToArray();

        private static readonly string[] GregorianLikeTestValues =
        {
            "2013-06-12T15:17:08.1234567",
            "-0500-06-12T00:52:59.1234567",
            "-1550-06-12T00:52:59.1234567",
            "1000-06-12T00:52:59.1234567",
            "1972-02-29T12:52:59.1234567",
        };

        private static readonly string[] CopticTestValues =
        {
            "2013-06-12T15:17:08.1234567",
            "1000-06-12T00:52:59.1234567",
            "1972-02-29T12:52:59.1234567",
        };

#pragma warning disable 0414 // Used by tests via reflection - do not remove!
        private static readonly TestCaseData[] GregorianLikeCalendarDateTimeFieldTestData = 
            CreateTestCaseData<DateTimeField>(GregorianLikeOldCalendarSystems, GregorianLikeNewCalendarSystems, GregorianLikeTestValues);

        private static readonly TestCaseData[] GregorianLikeCalendarPeriodFieldTestData =
            CreateTestCaseData<PeriodField>(GregorianLikeOldCalendarSystems, GregorianLikeNewCalendarSystems, GregorianLikeTestValues);

        private static readonly TestCaseData[] GregorianLikeCalendarConstructionTestData =
            (from calendar in GregorianLikeNewCalendarSystems
             from text in GregorianLikeTestValues
             select new TestCaseData(text, calendar).SetName(text + ": " + calendar.Id)).ToArray();

        private static readonly TestCaseData[] CopticCalendarDateTimeFieldTestData =
            CreateTestCaseData<DateTimeField>(CopticOldCalendarSystems, CopticNewCalendarSystems, CopticTestValues);

        private static readonly TestCaseData[] CopticCalendarPeriodFieldTestData =
            CreateTestCaseData<PeriodField>(CopticOldCalendarSystems, CopticNewCalendarSystems, CopticTestValues);

        private static readonly TestCaseData[] CopticCalendarConstructionTestData =
            (from calendar in CopticNewCalendarSystems
             from text in CopticTestValues
             select new TestCaseData(text, calendar).SetName(text + ": " + calendar.Id)).ToArray();

#pragma warning restore 0414

        private static TestCaseData[] CreateTestCaseData<T>(CalendarSystem[] oldCalendarSystems,
            CalendarSystem[] newCalendarSystems, string[] testValues)
        {
            return (from calendarPair in oldCalendarSystems.Zip(newCalendarSystems, (Old, New) => new { Old, New })
                    from property in GetSupportedProperties(calendarPair.New.Fields).Where(p => p.PropertyType == typeof(T))
                    from text in testValues
                    select new TestCaseData(text, calendarPair.Old, calendarPair.New, property)
                                 .SetName(text + ": " + calendarPair.Old.Id + " - " + property.Name)).ToArray();
        }

        private static IEnumerable<PropertyInfo> GetSupportedProperties(FieldSet fields)
        {
            // This is horrible, but should return all supported fields of either type
            return typeof(FieldSet).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                   .Where(p => (bool)p.PropertyType.GetProperty("IsSupported", BindingFlags.Instance | BindingFlags.NonPublic)
                                                                    .GetValue(p.GetValue(fields, null), null));
        }

        [Test]
        [TestCaseSource("GregorianLikeCalendarDateTimeFieldTestData")]
        [TestCaseSource("CopticCalendarDateTimeFieldTestData")]
        public void DateTimeFields(string text, CalendarSystem oldSystem, CalendarSystem newSystem, PropertyInfo property)
        {
            LocalInstant localInstant = LocalDateTimePattern.ExtendedIsoPattern.Parse(text).Value.LocalInstant;

            FieldSet originalFields = oldSystem.Fields;
            FieldSet newFields = newSystem.Fields;

            var oldField = (DateTimeField)property.GetValue(originalFields, null);
            var newField = (DateTimeField)property.GetValue(newFields, null);
            long expectedValue = oldField.GetInt64Value(localInstant);
            long actualValue = newField.GetInt64Value(localInstant);
            Assert.AreEqual(expectedValue, actualValue);
            if (expectedValue >= int.MinValue && expectedValue <= int.MaxValue)
            {
                Assert.AreEqual(oldField.GetInt64Value(localInstant), oldField.GetInt64Value(localInstant));
            }
        }

        [Test]
        [TestCaseSource("GregorianLikeCalendarPeriodFieldTestData")]
        [TestCaseSource("CopticCalendarPeriodFieldTestData")]
        public void PeriodFields(string text, CalendarSystem oldSystem, CalendarSystem newSystem, PropertyInfo property)
        {
            LocalInstant localInstant = LocalDateTimePattern.ExtendedIsoPattern.Parse(text).Value.LocalInstant;

            FieldSet originalFields = oldSystem.Fields;
            FieldSet newFields = newSystem.Fields;

            var oldField = (PeriodField)property.GetValue(originalFields, null);
            var newField = (PeriodField)property.GetValue(newFields, null);

            // 81 values which will cover various boundaries
            for (int i = -40; i <= 40; i++)
            {
                LocalInstant expectedValue = oldField.Add(localInstant, i);
                LocalInstant actualValue = newField.Add(localInstant, i);

                Assert.AreEqual(expectedValue, actualValue, "Error adding {0}", i);

                // The diff won't necessarily by equal to i as arithmetic isn't invertible,
                // but it should be the same as the old field.
                long expectedDiff = oldField.GetInt64Difference(actualValue, localInstant);
                long actualDiff = newField.GetInt64Difference(actualValue, localInstant);
                Assert.AreEqual(expectedDiff, actualDiff, "Error diffing after adding {0}", i);
            }
        }

        [Test]
        [TestCaseSource("GregorianLikeCalendarConstructionTestData")]
        [TestCaseSource("CopticCalendarConstructionTestData")]
        public void Construction(string text, CalendarSystem calendar)
        {
            LocalInstant localInstant = LocalDateTimePattern.ExtendedIsoPattern.Parse(text).Value.LocalInstant;
            LocalDateTime localDateTime = new LocalDateTime(localInstant, calendar);
            LocalInstant localDateInstant = (localDateTime.Date + LocalTime.Midnight).LocalInstant;

            Assert.AreEqual(localDateInstant,
                calendar.GetLocalInstant(localDateTime.Era, localDateTime.YearOfEra, localDateTime.Month, localDateTime.Day));
            Assert.AreEqual(localInstant,
                calendar.GetLocalInstant(localDateTime.Year, localDateTime.Month, localDateTime.Day,
                                         localDateTime.Hour, localDateTime.Minute, localDateTime.Second,
                                         localDateTime.Millisecond, (int) (localDateTime.TickOfSecond % NodaConstants.TicksPerMillisecond)));
            Assert.AreEqual(localInstant,
                calendar.GetLocalInstant(localDateTime.Year, localDateTime.Month, localDateTime.Day, localDateTime.TickOfDay));

            Assert.AreEqual(localDateInstant, calendar.GetLocalInstantFromWeekYearWeekAndDayOfWeek(
                localDateTime.WeekYear, localDateTime.WeekOfWeekYear, localDateTime.IsoDayOfWeek));
        }
    }
}
