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
using System.IO;
using System.Text;
using NodaTime.Calendars;
using NodaTime.Format;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public partial class DateTimeFormatterTest
    {
        #region Printing
        [Test]
        public void PrintToTextWriter_TrowsNotSupported_IfNotPrinter()
        {
            var sut = new DateTimeFormatter(null, parser);
            var sw = new StringWriter();
            var dt = new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, zone1);

            Assert.Throws<NotSupportedException>(() => sut.PrintTo(sw, dt));
        }

        [Test]
        public void PrintToTextWriter_PassDateTimeCalendar_IfNotSetOnFormatter()
        {
            var sut = new DateTimeFormatter(printer, null);
            var sw = new StringWriter();
            var dt = new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, zone1);

            sut.PrintTo(sw, dt);
            Assert.That(printer.Calendar, Is.SameAs(dt.Chronology.Calendar));
        }

        [Test]
        public void PrintToTextWriter_PassDateTimeZone_IfNotSetOnFormatter()
        {
            var sut = new DateTimeFormatter(printer, null);
            var sw = new StringWriter();
            var dt = new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, zone1);

            sut.PrintTo(sw, dt);
            Assert.That(printer.Zone, Is.SameAs(dt.Zone));
        }

        [Test]
        public void PrintToTextWriter_PassFormatterCalendar_IfSetOnFormatter()
        {
            var sut = new DateTimeFormatter(printer, null).WithCalendar(calendar1);
            var sw = new StringWriter();
            var dt = new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, zone1);

            sut.PrintTo(sw, dt);
            Assert.That(printer.Calendar, Is.SameAs(sut.Calendar));
            //TODO:for now we have only one calendar system, uncomment once the any second will be mplemented
            //Assert.That(printer.Calendar, Is.SameAs(dt.Chronology.Calendar));
        }

        [Test]
        public void PrintToTextWriter_PassFormatterZone_IfSetOnFormatter()
        {
            var sut = new DateTimeFormatter(printer, null).WithZone(zone2);
            var sw = new StringWriter();
            var dt = new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, zone1);

            sut.PrintTo(sw, dt);
            Assert.That(printer.Zone, Is.SameAs(sut.Zone));
            Assert.That(printer.Zone, Is.Not.SameAs(dt.Zone));
        }

        [Test]
        public void PrintToTextWriter_PassGivenWriter()
        {
            var sut = new DateTimeFormatter(printer, null);
            var sw = new StringWriter();
            var dt = new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, zone1);

            sut.PrintTo(sw, dt);
            Assert.That(printer.DtWriter, Is.SameAs(sw));
        }

        [Test]
        public void PrintToTextWriter_PassFormatterProvider()
        {
            var sut = new DateTimeFormatter(printer, null).WithProvider(provider1);
            var sw = new StringWriter();
            var dt = new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, zone1);

            sut.PrintTo(sw, dt);
            Assert.That(printer.DtProvider, Is.SameAs(provider1));
        }

        [Test]
        public void PrintToStringBuilder_PassGivenBuilder()
        {
            var sut = new DateTimeFormatter(printer, null);
            var sb = new StringBuilder();
            var dt = new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, zone1);

            sut.PrintTo(sb, dt);
            var actualBuilder = ((StringWriter)printer.DtWriter).GetStringBuilder();
            Assert.That(actualBuilder, Is.SameAs(sb));
        }
        #endregion

        #region Parsing
        [Test]
        public void Parse_TrowsNotSupported_IfNotParser()
        {
            var sut = new DateTimeFormatter(printer, null);
            var text = "2009-01-01";

            Assert.Throws<NotSupportedException>(() => sut.Parse(text));
        }

        [Test]
        public void Parse_PassISOCalendar_IfNotSetOnFormatter()
        {
            var sut = new DateTimeFormatter(null, parser);
            var text = "2009-01-01";

            sut.Parse(text);
            Assert.That(parser.Bucket.Calendar, Is.SameAs(CalendarSystem.Iso));
        }

        [Test]
        public void Parse_PassFormatterCalendar_IfSetOnFormatter()
        {
            var sut = new DateTimeFormatter(null, parser).WithCalendar(calendar1);
            var text = "2009-01-01";

            sut.Parse(text);
            Assert.That(parser.Bucket.Calendar, Is.SameAs(sut.Calendar));
            //TODO:for now we have only one calendar system, uncomment once the any second will be mplemented
            //Assert.That(parser.Bucket.CalendarSystem, Is.Not.SameAs(CalendarSystem.Iso));
        }

        [Test]
        public void Parse_PassUnixEpochAsInitialLocalInstant()
        {
            var sut = new DateTimeFormatter(null, parser);
            var text = "2009-01-01";

            sut.Parse(text);
            Assert.That(parser.Bucket.InitialLocalInstant, Is.EqualTo(LocalInstant.LocalUnixEpoch));
        }

        [Test]
        public void Parse_PassFormatterProvider()
        {
            var sut = new DateTimeFormatter(null, parser).WithProvider(provider1);
            var text = "2009-01-01";

            sut.Parse(text);
            Assert.That(parser.Bucket.Provider, Is.SameAs(provider1));
        }

        [Test]
        public void Parse_PassGivenText()
        {
            var sut = new DateTimeFormatter(null, parser);
            var text = "2009-01-01";

            sut.Parse(text);
            Assert.That(parser.Text, Is.SameAs(text));
        }

        [Test]
        public void Parse_PassZeroPositionToStartParsingFrom()
        {
            var sut = new DateTimeFormatter(null, parser);
            var text = "2009-01-01";

            sut.Parse(text);
            Assert.That(parser.Position, Is.EqualTo(0));
        }
        #endregion
    }
}