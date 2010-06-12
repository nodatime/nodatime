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
using System.Globalization;
using NodaTime.Calendars;
using NodaTime.Format;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    [TestFixture]
    public partial class DateTimeFormatterTest
    {
        private DateTimePrinterMock printer;
        private DateTimeParserMock parser;

        private IFormatProvider provider1;
        private IFormatProvider provider2;

        private ICalendarSystem calendar1;
        private ICalendarSystem calendar2;

        private IDateTimeZone zone1;
        private IDateTimeZone zone2;

        private int? pivotYear1;
        private int? pivotYear2;

        private DateTimeFormatter fullFormatterWithOffset;
        private DateTimeFormatter fullFormatterWithoutOffset;

        [SetUp]
        public void Init()
        {
            printer = new DateTimePrinterMock();
            parser = new DateTimeParserMock();

            provider1 = CultureInfo.InvariantCulture;
            provider2 = CultureInfo.CreateSpecificCulture("ar");

            calendar1 = IsoCalendarSystem.Instance;
            calendar2 = null;

            zone1 = DateTimeZones.Utc;
            zone2 = DateTimeZones.ForId("Europe/London");

            pivotYear1 = 55;
            pivotYear2 = 45;

            fullFormatterWithoutOffset =
                new DateTimeFormatter(printer, parser).WithProvider(provider1).WithCalendar(calendar1).WithZone(zone1).WithPivotYear(pivotYear1);

            fullFormatterWithOffset = fullFormatterWithoutOffset.WithOffsetParsed();
        }
    }
}