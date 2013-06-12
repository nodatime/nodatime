// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Linq;
using System.Reflection;
using NodaTime.Calendars;
using NodaTime.Fields;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    [TestFixture]
    public class TimeOfDayCalculatorTest
    {
        [Test]
        [TestCase("2013-06-12T15:17:08.1234567")]
        [TestCase("1000-06-12T00:52:59.1234567")]
        [TestCase("1000-06-12T12:52:59.1234567")]
        public void DateTimeFields(string text)
        {
            // Use LocalDateTime rather than LocalTime in order to check we can handle LocalInstants before the unix epoch.
            LocalInstant localInstant = LocalDateTimePattern.ExtendedIsoPattern.Parse(text).Value.LocalInstant;

            FieldSet isoFields = CalendarSystem.Iso.Fields;
            FieldSet timeCalculatorFields = TimeOfDayCalculator.TimeFields;
            foreach (var property in typeof(FieldSet).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                                     .Where(p => p.PropertyType == typeof(DateTimeField))
                                                     .Where(p => ((DateTimeField)p.GetValue(timeCalculatorFields, null)).IsSupported))
            {
                var isoField = (DateTimeField)property.GetValue(isoFields, null);
                var testField = (DateTimeField)property.GetValue(timeCalculatorFields, null);
                //Assert.AreEqual(isoField.GetValue(localInstant), testField.GetValue(localInstant));
                Assert.AreEqual(isoField.GetInt64Value(localInstant), testField.GetInt64Value(localInstant));
            }
        }
    }
}
