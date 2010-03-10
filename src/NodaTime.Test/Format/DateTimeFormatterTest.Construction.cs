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

using NodaTime.Format;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public partial class DateTimeFormatterTest
    {
        [Test]
        public void Constructor_InitDefaultFields()
        {
            var sut = new DateTimeFormatter(null, null);

            Assert.That(sut.Provider, Is.Null);
            Assert.That(sut.IsOffsetParsed, Is.False);
            Assert.That(sut.Calendar, Is.Null);
            Assert.That(sut.Zone, Is.Null);
            Assert.That(sut.PivotYear, Is.Null);
        }

        [Test]
        public void Constructor_InitOnlyPrinter_FromNotNullPrinterAndNullParser()
        {
            var sut = new DateTimeFormatter(printer, null);

            Assert.That(sut.Printer, Is.SameAs(printer));
            Assert.That(sut.IsPrinter, Is.True);

            Assert.That(sut.Parser, Is.Null);
            Assert.That(sut.IsParser, Is.False);

        }

        [Test]
        public void Constructor_InitOnlyParser_FromNullPrinterAndNotNullParser()
        {
            var sut = new DateTimeFormatter(null, parser);

            Assert.That(sut.Printer, Is.Null);
            Assert.That(sut.IsPrinter, Is.False);

            Assert.That(sut.Parser, Is.SameAs(parser));
            Assert.That(sut.IsParser, Is.True);
        }

        [Test]
        public void Constructor_InitBothPrinterAndParser_FromNotNullPrinterAndNotNullParser()
        {
            var sut = new DateTimeFormatter(printer, parser);

            Assert.That(sut.Printer, Is.SameAs(printer));
            Assert.That(sut.IsPrinter, Is.True);

            Assert.That(sut.Parser, Is.SameAs(parser));
            Assert.That(sut.IsParser, Is.True);
        }

        [Test]
        public void Constructor_InitNothing_FromNullPrinterAndNullParser()
        {
            var sut = new DateTimeFormatter(null, null);

            Assert.That(sut.Printer, Is.Null);
            Assert.That(sut.IsPrinter, Is.False);

            Assert.That(sut.Parser, Is.Null);
            Assert.That(sut.IsParser, Is.False);
        }
    }
}
