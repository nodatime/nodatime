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
using NodaTime.Format;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public partial class PeriodFormatterTest
    {
        [Test]
        public void FromPrinter_InitMembers_ForNotNullPrinter()
        {
            var sut = PeriodFormatter.FromPrinter(printer);

            Assert.That(sut.Printer, Is.SameAs(printer));
            Assert.That(sut.IsPrinter, Is.True);

            Assert.That(sut.Parser, Is.Null);
            Assert.That(sut.IsParser, Is.False);

            Assert.That(sut.Provider, Is.Null);
        }

        [Test]
        public void FromPrinter_ThrowsArgumentNull_ForNullPrinter()
        {
            Assert.That(() => PeriodFormatter.FromPrinter(null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void FromParser_InitMembers_ForNotNullParser()
        {
            var sut = PeriodFormatter.FromParser(parser);

            Assert.That(sut.Parser, Is.SameAs(parser));
            Assert.That(sut.IsParser, Is.True);

            Assert.That(sut.Printer, Is.Null);
            Assert.That(sut.IsPrinter, Is.False);

            Assert.That(sut.Provider, Is.Null);
        }

        [Test]
        public void FromParser_ThrowsArgumentNull_ForNullParser()
        {
            Assert.That(() => PeriodFormatter.FromParser(null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void FromPrinterAndParser_InitMembers_ForNotNullPrinterAndParser()
        {
            var sut = PeriodFormatter.FromPrinterAndParser(printer, parser);

            Assert.That(sut.Printer, Is.SameAs(printer));
            Assert.That(sut.IsPrinter, Is.True);

            Assert.That(sut.Parser, Is.SameAs(parser));
            Assert.That(sut.IsParser, Is.True);

            Assert.That(sut.Provider, Is.Null);
        }

        [Test]
        public void FromPrinterAndParser_ThrowsArgumentNull_ForNullPrinter()
        {
            Assert.That(() => PeriodFormatter.FromPrinterAndParser(null, parser), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void FromPrinterAndParser_ThrowsArgumentNull_ForNullParser()
        {
            Assert.That(() => PeriodFormatter.FromPrinterAndParser(printer, null), Throws.InstanceOf<ArgumentNullException>());
        }

    }
}
