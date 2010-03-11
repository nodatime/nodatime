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

using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public partial class DateTimeFormatterBuilderTest
    {
        #region AppendLiteralChar

        [Test]
        public void AppendLiteralChar_PrinterEstimateLengthAs1Always()
        {
            var printer = builder
                .AppendLiteral('q')
                .ToPrinter();

            Assert.That(printer.EstimatedPrintedLength, Is.EqualTo(1));
        }

        [Test]
        public void AppendLiteralChar_PrintsChar()
        {
            var dt = new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc);
            var formatter = builder
                .AppendLiteral('q')
                .ToFormatter();

            var printedValue = formatter.Print(dt);

            Assert.That(printedValue, Is.EqualTo("q"));
        }

        [Test]
        public void AppendLiteralChar_ParserEstimateLengthAs1Always()
        {
            var parser = builder
                .AppendLiteral('z')
                .ToParser();

            Assert.That(parser.EstimatedParsedLength, Is.EqualTo(1));
        }

        [Test]
        public void AppendLiteralChar_ParsesChar()
        {
            var formatter = builder
                .AppendLiteral('z')
                .ToFormatter();

            formatter.Parse("z");
        }

        [Test]
        public void AppendLiteralChar_ParsesChar_CaseInsensitive()
        {
            var formatter = builder
                .AppendLiteral('z')
                .ToFormatter();

            formatter.Parse("Z");
        }

        [Test]
        public void AppendLiteralChar_Throws_DifferentChar()
        {
            var formatter = builder
                .AppendLiteral('z')
                .ToFormatter();

            Assert.Throws<ArgumentException>(() => formatter.Parse("s"));
        }

        #endregion

        #region AppendLiteralString

        [Test]
        public void AppendLiteralString_ReturnsThis_ForEmptyString()
        {
            var newBuilder = builder.AppendLiteral("hi");

            Assert.That(newBuilder, Is.SameAs(builder));
        }

        [Test]
        public void AppendLiteralString_PrinterEstimateLengthAsStringLength()
        {
            var literal = "hi";
            var printer = builder
                .AppendLiteral(literal)
                .ToPrinter();

            Assert.That(printer.EstimatedPrintedLength, Is.EqualTo(literal.Length));
        }

        [Test]
        public void AppendLiteralString_PrintsString()
        {
            var dt = new ZonedDateTime(2004, 6, 9, 10, 20, 30, 40, DateTimeZones.Utc);
            var literal = "hi";
            var formatter = builder
                .AppendLiteral(literal)
                .ToFormatter();

            var printedValue = formatter.Print(dt);

            Assert.That(printedValue, Is.EqualTo(literal));
        }

        [Test]
        public void AppendLiteralString_ParserEstimateLengthAsStringLength()
        {
            var literal = "hi";
            var parser = builder
                .AppendLiteral(literal)
                .ToParser();

            Assert.That(parser.EstimatedParsedLength, Is.EqualTo(literal.Length));
        }

        [Test]
        public void AppendLiteralString_ParsesString()
        {
            var literal = "hi";
            var formatter = builder
                .AppendLiteral(literal)
                .ToFormatter();

            formatter.Parse(literal);
        }

        [Test]
        public void AppendLiteralString_ParsesString_IgnoreCase()
        {
            var formatter = builder
                .AppendLiteral("hi")
                .ToFormatter();

            formatter.Parse("Hi");
        }

        [Test]
        public void AppendLiteralString_ParsesString_Partially()
        {
            var formatter = builder
                .AppendLiteral("hi")
                .AppendLiteral(", people!")
                .ToFormatter();

            formatter.Parse("Hi, people!");
        }

        [Test]
        public void AppendLiteralString_Trows_DifferentString()
        {
            var formatter = builder
                .AppendLiteral("hi")
                .ToFormatter();

            Assert.Throws<ArgumentException>(() => formatter.Parse("by"));
        }

        #endregion

    }
}
