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
using NUnit.Framework;
using NodaTime.Format;

namespace NodaTime.Test.Format
{
    public partial class PeriodFormatterTest
    {

        [Test]
        public void Construct_FromNotNullPrinter()
        {
            var sut = PeriodFormatter.FromPrinter(printer);
            Assert.IsNull(sut.Parser);
            Assert.AreEqual(printer, sut.Printer);
            Assert.IsNull(sut.Provider);

            Assert.IsTrue(sut.IsPrinter);
            Assert.IsFalse(sut.IsParser);
        }

        [Test]
        public void Construct_FromNullPrinter()
        {
            Assert.Throws<ArgumentNullException>(() => PeriodFormatter.FromPrinter(null));
        }

        [Test]
        public void Construct_FromNotNullParser()
        {
            var sut = PeriodFormatter.FromParser(parser);
            Assert.AreEqual(parser, sut.Parser);
            Assert.IsNull(sut.Printer);
            Assert.IsNull(sut.Provider);

            Assert.IsFalse(sut.IsPrinter);
            Assert.IsTrue(sut.IsParser);
        }

        [Test]
        public void Construct_FromNullParser()
        {
            Assert.Throws<ArgumentNullException>(() => PeriodFormatter.FromParser(null));
        }

        [Test]
        public void Construct_FromPrinterAndParser()
        {
            var sut = PeriodFormatter.FromPrinterAndParser(printer, parser);
            Assert.AreEqual(parser, sut.Parser);
            Assert.AreEqual(printer, sut.Printer);
            Assert.IsNull(sut.Provider);

            Assert.IsTrue(sut.IsPrinter);
            Assert.IsTrue(sut.IsParser);
        }

        [Test]
        public void Construct_FromNotNullPrinterAndNullParser()
        {
            Assert.Throws<ArgumentNullException>(() => PeriodFormatter.FromPrinterAndParser(printer, null));
        }

        [Test]
        public void Construct_FromNullPrinterAndNotNullParser()
        {
            Assert.Throws<ArgumentNullException>(() => PeriodFormatter.FromPrinterAndParser(null, parser));
        }

    }
}
