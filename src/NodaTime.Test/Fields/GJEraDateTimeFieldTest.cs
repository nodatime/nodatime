// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using NodaTime.Calendars;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class GJEraDateTimeFieldTest
    {
        [Test]
        public void YearOfEraPreserved()
        {
            var start = new LocalDate(Era.Common, 1980, 6, 19).AtMidnight().LocalInstant;
            var bc = CalendarSystem.Iso.Fields.Era.SetValue(start, 0);
            Assert.AreEqual(1980, CalendarSystem.Iso.Fields.YearOfEra.GetValue(bc));
            var backAgain = CalendarSystem.Iso.Fields.Era.SetValue(bc, 1);
            Assert.AreEqual(start, backAgain);
        }
    }
}
