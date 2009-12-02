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
using System.Text;
using NodaTime.Periods;
using System.Globalization;
using System.IO;

namespace NodaTime.Test
{
    public partial class PeriodFormatterTest
    {
        [Test]
        public void PrintToBuilderNonPrinter()
        {
            var sutDefault = new PeriodFormatter(null, null, daysPeriodType);
            var sb = new StringBuilder();
            var period = Days.Five;

            Assert.Throws<NotSupportedException>(() => sutDefault.PrintTo(sb, period));
        }

        [Test]
        public void PrintToBuilderNullPeriod()
        {
            var sutDefault = new PeriodFormatter(null, printer, daysPeriodType);
            var sb = new StringBuilder();

            Assert.Throws<ArgumentNullException>(() => sutDefault.PrintTo(sb, null));
        }

        [Test]
        public void PrintToBuilder()
        {
            var sutDefault = new PeriodFormatter(null, printer, daysPeriodType).WithProvider(provider);
            var sb = new StringBuilder();
            var period = Days.Five;

            sutDefault.PrintTo(sb, period);

            Assert.IsTrue(printer.PrintToBuilderCalled);
            Assert.AreSame(sb, printer.PrintToBuilderArgument);
            Assert.AreSame(period, printer.PrintToBuilderToPeriodArgument);
            Assert.AreSame(provider, printer.PrintToBuilderProviderArgument);

        }

        [Test]
        public void PrintToWriterNonPrinter()
        {
            var sutDefault = new PeriodFormatter(null, null, daysPeriodType);
            var sw = new StringWriter();
            var period = Days.Five;

            Assert.Throws<NotSupportedException>(() => sutDefault.PrintTo(sw, period));
        }

        [Test]
        public void PrintToWriterNullPeriod()
        {
            var sutDefault = new PeriodFormatter(null, printer, daysPeriodType);
            var sw = new StringWriter();

            Assert.Throws<ArgumentNullException>(() => sutDefault.PrintTo(sw, null));
        }

        [Test]
        public void PrintToWriter()
        {
            var sutDefault = new PeriodFormatter(null, printer, daysPeriodType).WithProvider(provider);
            var sw = new StringWriter();
            var period = Days.Five;

            sutDefault.PrintTo(sw, period);

            Assert.IsTrue(printer.PrintToWriterCalled);
            Assert.AreSame(sw, printer.PrintToWriterArgument);
            Assert.AreSame(period, printer.PrintToWriterPeriodArgument);
            Assert.AreSame(provider, printer.PrintToWriterProviderArgument);

        }

        [Test]
        public void PrintNonPrinter()
        {
            var sutDefault = new PeriodFormatter(null, null, daysPeriodType);
            var period = Days.Five;

            Assert.Throws<NotSupportedException>(() => sutDefault.Print(period));
        }

        [Test]
        public void PrintNullPeriod()
        {
            var sutDefault = new PeriodFormatter(null, printer, daysPeriodType);

            Assert.Throws<ArgumentNullException>(() => sutDefault.Print(null));
        }

        [Test]
        public void Print()
        {
            var sutDefault = new PeriodFormatter(null, printer, daysPeriodType).WithProvider(provider);
            var period = Days.Five;

            var text = sutDefault.Print(period);

            Assert.IsTrue(printer.CalculatePrintedLengthCalled);
            Assert.AreSame(period, printer.CalculatePrintedLengthPeriodArgument);
            Assert.AreSame(provider, printer.CalculatePrintedLengthProviderArgument);
            Assert.IsTrue(printer.PrintToBuilderCalled);
            Assert.AreSame(provider, printer.PrintToBuilderProviderArgument);
            Assert.AreSame(period, printer.PrintToBuilderToPeriodArgument);
            Assert.IsNotNull(text);
        }

        [Test]
        public void ParseIntoNonParser()
        {
            var sutDefault = new PeriodFormatter(null, null, daysPeriodType);
            IPeriod result;
            Assert.Throws<NotSupportedException>(() => sutDefault.ParseInto("_", 0, out result));
        }

        [Test]
        public void ParseInto()
        {
            var sutDefault = new PeriodFormatter(parser, null, daysPeriodType).WithProvider(provider);
            var text = "_";
            var position = 42;
            IPeriod result;

            sutDefault.ParseInto(text, position, out result);

            Assert.IsTrue(parser.ParseIntoCalled);
            Assert.AreEqual(text, parser.ParseIntoStringArgument);
            Assert.AreEqual(position, parser.ParseIntoPositionArgument);
            Assert.AreSame(provider, parser.ParseIntoProviderArgument);
            Assert.IsNotNull(result);
        }

        [Test]
        public void ParseNonParser()
        {
            var sutDefault = new PeriodFormatter(null, null, daysPeriodType);

            Assert.Throws<NotSupportedException>(() => sutDefault.Parse("_"));
        }

        [Test]
        public void ParseIntoFullParse()
        {
            var sutDefault = new PeriodFormatter(parser, null, daysPeriodType).WithProvider(provider);
            parser.ParseIntoPositionToReturn = 10;
            var text = "_";

            IPeriod result = sutDefault.Parse(text);

            Assert.IsTrue(parser.ParseIntoCalled);
            Assert.AreEqual(text, parser.ParseIntoStringArgument);
            Assert.AreSame(provider, parser.ParseIntoProviderArgument);
            Assert.AreEqual(0, parser.ParseIntoPositionArgument);
        }

        [Test]
        public void ParseIntoFailedParse()
        {
            var sutDefault = new PeriodFormatter(parser, null, daysPeriodType).WithProvider(provider);
            parser.ParseIntoPositionToReturn = -1;
            var text = "_";

            Assert.Throws<ArgumentException>(() => sutDefault.Parse(text));

            Assert.IsTrue(parser.ParseIntoCalled);
            Assert.AreEqual(text, parser.ParseIntoStringArgument);
            Assert.AreSame(provider, parser.ParseIntoProviderArgument);
            Assert.AreEqual(0, parser.ParseIntoPositionArgument);
        }

        [Test]
        public void ParseIntoPartialParse()
        {
            var sutDefault = new PeriodFormatter(parser, null, daysPeriodType).WithProvider(provider);
            parser.ParseIntoPositionToReturn = 2;
            var text = "123456789";

            Assert.Throws<ArgumentException>(() => sutDefault.Parse(text));

            Assert.IsTrue(parser.ParseIntoCalled);
            Assert.AreEqual(text, parser.ParseIntoStringArgument);
            Assert.AreSame(provider, parser.ParseIntoProviderArgument);
            Assert.AreEqual(0, parser.ParseIntoPositionArgument);
        }
    }
}
