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
using NodaTime.Format;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public partial class PeriodFormatterBuilderTest
    {
        [Test]
        public void Literal_CalculatesLength()
        {
            const string literalText = "Hi";
            var literal = new PeriodFormatterBuilder.Literal(literalText);

            var actualLength = literal.CalculatePrintedLength(null, null);

            Assert.That(actualLength, Is.EqualTo(literalText.Length));
        }

        [Test]
        public void Literal_ReturnsFieldsCountAlwaysZero()
        {
            const string literalText = "Hi";
            var literal = new PeriodFormatterBuilder.Literal(literalText);

            var actualFieldsCount = literal.CountFieldsToPrint(null, 0, null);
            Assert.That(actualFieldsCount, Is.EqualTo(0));
        }

        [Test]
        public void Literal_PrintsTextToTextWriter()
        {
            const string literalText = "T";
            var literal = new PeriodFormatterBuilder.Literal(literalText);
            var writer = new StringWriter();

            literal.PrintTo(writer, null, null);

            Assert.That(writer.ToString(), Is.EqualTo(literalText));
        }

        [Test]
        public void Literal_ParseMovePostision_GivenZeroPosition()
        {
            const string literalText = "abc";
            var literal = new PeriodFormatterBuilder.Literal(literalText);
            var writer = new StringWriter();

            var position = literal.Parse(literalText, 0, null, null);

            Assert.That(position, Is.EqualTo(literalText.Length));
        }

        [Test]
        public void Literal_ParseMovePostision_GivenLargeText()
        {
            const string literalText = "abc";
            const string periodString = "abcdefg";

            var literal = new PeriodFormatterBuilder.Literal(literalText);
            var writer = new StringWriter();

            var position = literal.Parse(periodString, 0, null, null);

            Assert.That(position, Is.EqualTo(literalText.Length));
        }

        [Test]
        public void Literal_ParseMovePostision_IgnoreCase()
        {
            const string literalText = "abc";
            const string periodString = "aBc";

            var literal = new PeriodFormatterBuilder.Literal(literalText);
            var writer = new StringWriter();

            var position = literal.Parse(periodString, 0, null, null);

            Assert.That(position, Is.EqualTo(literalText.Length));
        }

        [Test]
        public void Literal_ParseMovePostision_GivenNonZeroPosition()
        {
            const string literalText = "abc";
            const string periodString = "OOabc";

            var literal = new PeriodFormatterBuilder.Literal(literalText);
            var writer = new StringWriter();

            var position = literal.Parse(periodString, 2, null, null);

            Assert.That(position, Is.EqualTo(periodString.Length));
        }

        [Test]
        public void Literal_ParseReturnsFailurePositionComplement_GivenNotMatchedString()
        {
            const string literalText = "abc";
            const string periodString = "OOOZ";

            var literal = new PeriodFormatterBuilder.Literal(literalText);
            var writer = new StringWriter();

            var position = literal.Parse(periodString, 3, null, null);

            Assert.That(position, Is.EqualTo(~3));
        }


        [Test]
        public void AppendLiteral_ThrowsArgumentNull_ForNullTextArg()
        {
            Assert.Throws<ArgumentNullException>(() => builder.AppendLiteral(null));
        }

        [Test]
        public void AppendLiteral_ThrowsArgumentNull_ForEmptyTextArg()
        {
            Assert.Throws<ArgumentNullException>(() => builder.AppendLiteral(String.Empty));
        }

        [Test]
        public void AppendLiteral_ThrowsInvalidOperation_ForBuilderWithPrefix()
        {
            Assert.Throws<InvalidOperationException>(() => builder.AppendPrefix("prefix").AppendLiteral("literal"));
        }

        [Test]
        public void AppendLiteral_BuildsCorrectPrinter_ForStandardPeriod()
        {
            const string literalText = "HELLO";

            var formatter = builder
                .AppendLiteral(literalText)
                .ToFormatter();

            var printedValue = formatter.Print(standardPeriodFull);

            Assert.That(printedValue, Is.EqualTo(literalText));
        }

    }
}
