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
using NodaTime.Format;
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public partial class PeriodFormatterTest
    {
        #region PrintToStringBuilder

        [Test]
        public void PrintToStringBuilder_ThrowsUnsupported_IfNonPrinter()
        {
            var sutDefault = PeriodFormatter.FromParser(parser);
            var sb = new StringBuilder();
            var period = Days.Five;

            Assert.Throws<NotSupportedException>(() => sutDefault.PrintTo(sb, period));
        }

        [Test]
        public void PrintToStringBuilder_ThrowsArgumentNull_ForNullPeriod()
        {
            var sutDefault = PeriodFormatter.FromPrinter(printer);
            var sb = new StringBuilder();

            Assert.Throws<ArgumentNullException>(() => sutDefault.PrintTo(sb, null));
        }

        [Test]
        public void PrintToStringBuilder_ThrowsArgumentNull_ForNullStringBuilder()
        {
            var sutDefault = PeriodFormatter.FromPrinter(printer);
            StringBuilder sb = null;

            Assert.Throws<ArgumentNullException>(() => sutDefault.PrintTo(sb, Days.Five));
        }

        [Test]
        public void PrintToStringBuilder_DelegatesToPrinter()
        {
            var sutDefault = PeriodFormatter.FromPrinter(printer).WithProvider(provider1);
            var sb = new StringBuilder();
            var period = Days.Five;

            sutDefault.PrintTo(sb, period);

            Assert.That(printer.PrintToWriterCalled, Is.True);
            Assert.That(printer.PrintToWriterPeriodArgument, Is.SameAs(period));
            Assert.That(printer.PrintToWriterProviderArgument, Is.SameAs(provider1));
        }

        #endregion

        #region PrintToTextWriter

        [Test]
        public void PrintToTextWriter_ThrowsUnsupported_IfNonPrinter()
        {
            var sutDefault = PeriodFormatter.FromParser(parser);
            var sw = new StringWriter();
            var period = Days.Five;

            Assert.Throws<NotSupportedException>(() => sutDefault.PrintTo(sw, period));
        }

        [Test]
        public void PrintToTextWriter_ThrowsArgumentNull_ForNullPeriod()
        {
            var sutDefault = PeriodFormatter.FromPrinter(printer);
            var sw = new StringWriter();

            Assert.Throws<ArgumentNullException>(() => sutDefault.PrintTo(sw, null));
        }

        [Test]
        public void PrintToTextWriter_ThrowsArgumentNull_ForNullTextWriter()
        {
            var sutDefault = PeriodFormatter.FromPrinter(printer);
            TextWriter sw = null;

            Assert.Throws<ArgumentNullException>(() => sutDefault.PrintTo(sw, Years.Two));
        }

        [Test]
        public void PrintToTextWriter_DelegatesToPrinter()
        {
            var sutDefault = PeriodFormatter.FromPrinter(printer).WithProvider(provider1);
            var sw = new StringWriter();
            var period = Days.Five;

            sutDefault.PrintTo(sw, period);

            Assert.That(printer.PrintToWriterCalled, Is.True);
            Assert.That(printer.PrintToWriterArgument, Is.SameAs(sw));
            Assert.That(printer.PrintToWriterPeriodArgument, Is.SameAs(period));
            Assert.That(printer.PrintToWriterProviderArgument, Is.SameAs(provider1));
        }

        #endregion

        #region Print

        [Test]
        public void Printr_ThrowsUnsupported_IfNonPrinter()
        {
            var sutDefault = PeriodFormatter.FromParser(parser);
            var period = Days.Five;

            Assert.Throws<NotSupportedException>(() => sutDefault.Print(period));
        }

        [Test]
        public void Print_ThrowsArgumentNull_ForNullPeriod()
        {
            var sutDefault = PeriodFormatter.FromPrinter(printer);

            Assert.Throws<ArgumentNullException>(() => sutDefault.Print(null));
        }

        [Test]
        public void Print_DelegatesToPrinter()
        {
            var sutDefault = PeriodFormatter.FromPrinter(printer).WithProvider(provider1);
            var period = Days.Five;

            var text = sutDefault.Print(period);

            Assert.That(printer.CalculatePrintedLengthCalled, Is.True);
            Assert.That(printer.CalculatePrintedLengthPeriodArgument, Is.SameAs(period));
            Assert.That(printer.CalculatePrintedLengthProviderArgument, Is.SameAs(provider1));

            Assert.That(printer.PrintToWriterCalled, Is.True);
            Assert.That(printer.PrintToWriterPeriodArgument, Is.SameAs(period));
            Assert.That(printer.PrintToWriterProviderArgument, Is.SameAs(provider1));

            Assert.That(text, Is.Not.Null);
        }

        #endregion

        #region Parse

        [Test]
        public void Parse_ThrowsUnsupported_IfNonParser()
        {
            var sutDefault = PeriodFormatter.FromPrinter(printer);
            Assert.Throws<NotSupportedException>(() => sutDefault.Parse("_"));
        }

        [Test]
        public void Parse_ThrowsArgumentNull_ForNullText()
        {
            var sutDefault = PeriodFormatter.FromParser(parser);

            Assert.Throws<ArgumentNullException>(() => sutDefault.Parse(null));
        }

        [Test]
        public void Parse_DelegatesToParser()
        {
            var sutDefault = PeriodFormatter.FromParser(parser).WithProvider(provider1);
            var text = "_";
            var position = 0;

            var result = sutDefault.Parse(text);

            Assert.That(parser.ParseCalled, Is.True);
            Assert.That(parser.ParseStringArgument, Is.EqualTo(text));
            Assert.That(parser.ParsePositionArgument, Is.EqualTo(position));
            Assert.That(parser.ParseProviderArgument, Is.SameAs(provider1));
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Parse_ThrowsFormat_IfParsingWasFailed()
        {
            var sutDefault = PeriodFormatter.FromParser(parser).WithProvider(provider1);
            parser.ParsePositionToReturn = -1;
            var text = "_";

            Assert.Throws<FormatException>(() => sutDefault.Parse(text));
        }

        [Test]
        public void Parse_ThrowsFormat_IfParsingWasNotReachTheEndOfTheText()
        {
            var sutDefault = PeriodFormatter.FromParser(parser).WithProvider(provider1);
            parser.ParsePositionToReturn = 2;
            var text = "123456789";

            Assert.Throws<FormatException>(() => sutDefault.Parse(text));
        }

        #endregion
    }
}
