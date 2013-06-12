using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using NodaTime.Calendars;
using NodaTime.Fields;
using NodaTime.Text;

namespace NodaTime.Test.Calendars
{
    [TestFixture]
    public class CalculatorCalendarSystemTest
    {
#pragma warning disable 0414 // Used by tests via reflection - do not remove!

        private static readonly CalendarSystem[] GregorianLikeOldCalendarSystems = 
        {
            GregorianCalendarSystem.GetInstance(1),
            GregorianCalendarSystem.GetInstance(2),
            GregorianCalendarSystem.GetInstance(3),
            GregorianCalendarSystem.GetInstance(4),
            GregorianCalendarSystem.GetInstance(5),
            GregorianCalendarSystem.GetInstance(6),
            GregorianCalendarSystem.GetInstance(7),
            GregorianCalendarSystem.IsoHelper.Instance
        };

        private static readonly CalendarSystem[] GregorianLikeNewCalendarSystems = 
        {
            new CalculatorCalendarSystem("new greg 1", "ignored", GregorianYearMonthDayCalculator.Instance, 1),
            new CalculatorCalendarSystem("new greg 2", "ignored", GregorianYearMonthDayCalculator.Instance, 2),
            new CalculatorCalendarSystem("new greg 3", "ignored", GregorianYearMonthDayCalculator.Instance, 3),
            new CalculatorCalendarSystem("new greg 4", "ignored", GregorianYearMonthDayCalculator.Instance, 4),
            new CalculatorCalendarSystem("new greg 5", "ignored", GregorianYearMonthDayCalculator.Instance, 5),
            new CalculatorCalendarSystem("new greg 6", "ignored", GregorianYearMonthDayCalculator.Instance, 6),
            new CalculatorCalendarSystem("new greg 7", "ignored", GregorianYearMonthDayCalculator.Instance, 7),
            new CalculatorCalendarSystem("new iso", "ignored", IsoYearMonthDayCalculator.IsoInstance, 4),
        };

        private static readonly string[] GregorionLikeTestValues =
        {
            "2013-06-12T15:17:08.1234567",
            "-0500-06-12T00:52:59.1234567",
            "-1550-06-12T00:52:59.1234567",
            "1000-06-12T00:52:59.1234567",
            "1972-02-29T12:52:59.1234567",
        };

        private static readonly TestCaseData[] GregorianLikeCalendarDateTimeFieldTestData = 
            (from calendarPair in GregorianLikeOldCalendarSystems.Zip(GregorianLikeNewCalendarSystems, (Old, New) => new { Old, New })
             from property in GetSupportedProperties(calendarPair.New.Fields).Where(p => p.PropertyType == typeof(DateTimeField))
             from text in GregorionLikeTestValues
             select new TestCaseData(text, calendarPair.Old, calendarPair.New, property)
                           .SetName(text + ": " + calendarPair.Old.Id + " - " + property.Name)).ToArray();

        private static readonly TestCaseData[] GregorianLikeCalendarPeriodFieldTestData =
            (from calendarPair in GregorianLikeOldCalendarSystems.Zip(GregorianLikeNewCalendarSystems, (Old, New) => new { Old, New })
             from property in GetSupportedProperties(calendarPair.New.Fields).Where(p => p.PropertyType == typeof(PeriodField))
             from text in GregorionLikeTestValues
             select new TestCaseData(text, calendarPair.Old, calendarPair.New, property)
                           .SetName(text + ": " + calendarPair.Old.Id + " - " + property.Name)).ToArray();
#pragma warning restore 0414

        private static IEnumerable<PropertyInfo> GetSupportedProperties(FieldSet fields)
        {
            // This is horrible, but should return all supported fields of either type
            return typeof(FieldSet).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                   .Where(p => (bool)p.PropertyType.GetProperty("IsSupported", BindingFlags.Instance | BindingFlags.NonPublic)
                                                                    .GetValue(p.GetValue(fields, null), null));
        }

        [Test]
        [TestCaseSource("GregorianLikeCalendarDateTimeFieldTestData")]
        public void GregorianLikeDateTimeFields(string text, CalendarSystem oldSystem, CalendarSystem newSystem, PropertyInfo property)
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
        public void GregorianLikePeriodFields(string text, CalendarSystem oldSystem, CalendarSystem newSystem, PropertyInfo property)
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
    }

}
