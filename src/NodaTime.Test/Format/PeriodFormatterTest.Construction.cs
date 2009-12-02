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

namespace NodaTime.Test
{
    public partial class PeriodFormatterTest
    {
        [Test]
        public void InitWithNullPrinterAndParser()
        {
            var sut = new PeriodFormatter(null, null, daysPeriodType);
            Assert.IsNull(sut.Printer);            
            Assert.IsNull(sut.Parser);
            Assert.IsNull(sut.Provider);
            Assert.AreEqual(daysPeriodType, sut.PeriodType);

            Assert.IsFalse(sut.IsPrinter);
            Assert.IsFalse(sut.IsParser);
        }

        [Test]
        public void InitWithNotNullPrinterAndNullParser()
        {
            var sut = new PeriodFormatter(null, printer, daysPeriodType);
            Assert.IsNull(sut.Parser);
            Assert.AreEqual(printer, sut.Printer);
            Assert.IsNull(sut.Provider);
            Assert.AreEqual(daysPeriodType, sut.PeriodType);

            Assert.IsTrue(sut.IsPrinter);
            Assert.IsFalse(sut.IsParser);
        }

        [Test]
        public void InitWithNullPrinterAndNotNullParser()
        {
            var sut = new PeriodFormatter(parser, null, daysPeriodType);
            Assert.AreEqual(parser, sut.Parser);
            Assert.IsNull(sut.Printer);
            Assert.IsNull(sut.Provider);
            Assert.AreEqual(daysPeriodType, sut.PeriodType);

            Assert.IsFalse(sut.IsPrinter);
            Assert.IsTrue(sut.IsParser);
        }

        [Test]
        public void InitWithNotNullPrinterAndParser()
        {
            var sut = new PeriodFormatter(parser, printer, daysPeriodType);
            Assert.AreEqual(parser, sut.Parser);
            Assert.AreEqual(printer, sut.Printer);
            Assert.IsNull(sut.Provider);
            Assert.AreEqual(daysPeriodType, sut.PeriodType);

            Assert.IsTrue(sut.IsPrinter);
            Assert.IsTrue(sut.IsParser);
        }

    }
}
