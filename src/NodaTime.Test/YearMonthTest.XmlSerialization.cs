// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NodaTime.Test
{
    public partial class YearMonthTest
    {
        [Test]
        public void XmlSerialization_Iso()
        {
            var value = new YearMonth(2013, 4);
            TestHelper.AssertXmlRoundtrip(value, "<value>2013-04</value>");
        }

        [Test]
        public void XmlSerialization_NonIso()
        {
            var value = new YearMonth(2013, 4, CalendarSystem.Julian);
            TestHelper.AssertXmlRoundtrip(value, "<value calendar=\"Julian\">2013-04</value>");
        }

        [Test]
        [TestCase("<value calendar=\"Rubbish\">2013-06</value>", typeof(KeyNotFoundException), Description = "Unknown calendar system")]
        [TestCase("<value>2013-15</value>", typeof(UnparsableValueException), Description = "Invalid month")]
        public void XmlSerialization_Invalid(string xml, Type expectedExceptionType)
        {
            TestHelper.AssertXmlInvalid<YearMonth>(xml, expectedExceptionType);
        }
    }
}
