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
        public void SimpleAffix_CalculatesLength()
        {
            const string affixText = "Hi";
            var literal = new PeriodFormatterBuilder.SimpleAffix(affixText);

            var actualLength = literal.CalculatePrintedLength(0);

            Assert.That(actualLength, Is.EqualTo(affixText.Length));
        }

        [Test]
        public void SimpleAffix_PrintsTextToTextWriter()
        {
            const string affixText = "T";
            var literal = new PeriodFormatterBuilder.SimpleAffix(affixText);
            var writer = new StringWriter();

            literal.PrintTo(writer, 0);

            Assert.That(writer.ToString(), Is.EqualTo(affixText));
        }

        [Test]
        public void SimpleAffix_ParseMovePostision_GivenZeroPosition()
        {
            const string affixText = "abc";
            var literal = new PeriodFormatterBuilder.SimpleAffix(affixText);
            var writer = new StringWriter();

            var position = literal.Parse(affixText, 0);

            Assert.That(position, Is.EqualTo(affixText.Length));
        }

        [Test]
        public void SimpleAffix_ParseMovePostision_GivenLargeText()
        {
            const string affixText = "abc";
            const string periodString = "abcdefg";

            var literal = new PeriodFormatterBuilder.SimpleAffix(affixText);
            var writer = new StringWriter();

            var position = literal.Parse(periodString, 0);

            Assert.That(position, Is.EqualTo(affixText.Length));
        }

        [Test]
        public void SimpleAffix_ParseMovePostision_IgnoreCase()
        {
            const string affixText = "abc";
            const string periodString = "aBc";

            var literal = new PeriodFormatterBuilder.SimpleAffix(affixText);
            var writer = new StringWriter();

            var position = literal.Parse(periodString, 0);

            Assert.That(position, Is.EqualTo(affixText.Length));
        }

        [Test]
        public void SimpleAffix_ParseMovePostision_GivenNonZeroPosition()
        {
            const string affixText = "abc";
            const string periodString = "OOabc";

            var literal = new PeriodFormatterBuilder.SimpleAffix(affixText);
            var writer = new StringWriter();

            var position = literal.Parse(periodString, 2);

            Assert.That(position, Is.EqualTo(periodString.Length));
        }

        [Test]
        public void SimpleAffix_ParseReturnsFailurePositionComplement_GivenNotMatchedString()
        {
            const string affixText = "abc";
            const string periodString = "OOOZabc";

            var literal = new PeriodFormatterBuilder.SimpleAffix(affixText);
            var writer = new StringWriter();

            var position = literal.Parse(periodString, 3);

            Assert.That(position, Is.EqualTo(~3));
        }

        [Test]
        public void SimpleAffix_ScanFindPosition_GivenCorrectCharsInFrontOfAffix()
        {
            const string affixText = "abc";
            const string periodString = "+-123.,abc";

            var literal = new PeriodFormatterBuilder.SimpleAffix(affixText);
            var writer = new StringWriter();

            var position = literal.Scan(periodString, 0);

            Assert.That(position, Is.EqualTo(7));
        }

        [Test]
        public void SimpleAffix_ScanReturnFaiuledPosition_GivenIncorrectCharsInFrontOfAffix()
        {
            const string affixText = "abc";
            const string periodString = "XyZabc";

            var literal = new PeriodFormatterBuilder.SimpleAffix(affixText);
            var writer = new StringWriter();

            var position = literal.Scan(periodString, 0);

            Assert.That(position, Is.EqualTo(~0));
        }

        [Test]
        public void PluralAffix_CalculatesLength_GivenSingularValue()
        {
            const string singularText = "Foo";
            const string pluralText = "Foos";

            var literal = new PeriodFormatterBuilder.PluralAffix(singularText, pluralText);
            var actualLength = literal.CalculatePrintedLength(1);

            Assert.That(actualLength, Is.EqualTo(singularText.Length));
        }

        [Test]
        public void PluralAffix_CalculatesLength_GivenPluralValue()
        {
            const string singularText = "Foo";
            const string pluralText = "Foos";

            var literal = new PeriodFormatterBuilder.PluralAffix(singularText, pluralText);
            var actualLength = literal.CalculatePrintedLength(3);

            Assert.That(actualLength, Is.EqualTo(pluralText.Length));
        }

        [Test]
        public void PluralAffix_PrintsTextToTextWriter_GivenSingularValue()
        {
            const string singularText = "Foo";
            const string pluralText = "Foos";
            var literal = new PeriodFormatterBuilder.PluralAffix(singularText, pluralText);
            var writer = new StringWriter();

            literal.PrintTo(writer, 1);

            Assert.That(writer.ToString(), Is.EqualTo(singularText));
        }

        [Test]
        public void PluralAffix_PrintsTextToTextWriter_GivenPluralValue()
        {
            const string singularText = "Foo";
            const string pluralText = "Foos";
            var literal = new PeriodFormatterBuilder.PluralAffix(singularText, pluralText);
            var writer = new StringWriter();

            literal.PrintTo(writer, 4);

            Assert.That(writer.ToString(), Is.EqualTo(pluralText));
        }

        [Test]
        public void PluralAffix_ParseMovePostision_GivenLessSingularTextAndZeroPosition()
        {
            const string singularText = "Foo";
            const string pluralText = "Foos";
            var literal = new PeriodFormatterBuilder.PluralAffix(singularText, pluralText);
            var writer = new StringWriter();

            var position = literal.Parse(singularText, 0);

            Assert.That(position, Is.EqualTo(singularText.Length));
        }

        [Test]
        public void PluralAffix_ParseMovePostision_GivenLargePluralTextAndZeroPosition()
        {
            const string singularText = "Foo";
            const string pluralText = "Foos";
            var literal = new PeriodFormatterBuilder.PluralAffix(singularText, pluralText);
            var writer = new StringWriter();

            var position = literal.Parse(pluralText, 0);

            Assert.That(position, Is.EqualTo(pluralText.Length));
        }

        [Test]
        public void PluralAffix_ParseMovePostision_GivenLargeSingularTextAndZeroPosition()
        {
            const string singularText = "Baaar";
            const string pluralText = "Bars";
            var literal = new PeriodFormatterBuilder.PluralAffix(singularText, pluralText);
            var writer = new StringWriter();

            var position = literal.Parse(singularText, 0);

            Assert.That(position, Is.EqualTo(singularText.Length));
        }

        [Test]
        public void PluralAffix_ParseMovePostision_GivenLessPluralTextAndZeroPosition()
        {
            const string singularText = "Baaar";
            const string pluralText = "Bars";
            var literal = new PeriodFormatterBuilder.PluralAffix(singularText, pluralText);
            var writer = new StringWriter();

            var position = literal.Parse(pluralText, 0);

            Assert.That(position, Is.EqualTo(pluralText.Length));
        }


        [Test]
        public void AppendPrefixSimple_ThrowsArgumentNull_ForNullPrefixText()
        {
            Assert.Throws<ArgumentNullException>(() => builder.AppendPrefix(null));
        }

        [Test]
        public void AppendPrefixSimple_ThrowsArgumentNull_ForEmptyPrefixText()
        {
            Assert.Throws<ArgumentNullException>(() => builder.AppendPrefix(String.Empty));
        }

        [Test]
        public void AppendPrefixPlural_ThrowsArgumentNull_ForNullSingularText()
        {
            Assert.Throws<ArgumentNullException>(() => builder.AppendPrefix(null, "notnull"));
        }

        [Test]
        public void AppendPrefixPlural_ThrowsArgumentNull_ForEmptySingularText()
        {
            Assert.Throws<ArgumentNullException>(() => builder.AppendPrefix(String.Empty, "notnull"));
        }

        [Test]
        public void AppendPrefixPlural_ThrowsArgumentNull_ForNullPluralText()
        {
            Assert.Throws<ArgumentNullException>(() => builder.AppendPrefix("notnull", null));
        }

        [Test]
        public void AppendPrefixPlural_ThrowsArgumentNull_ForEmptyPluralText()
        {
            Assert.Throws<ArgumentNullException>(() => builder.AppendPrefix("notnull", String.Empty));
        }
    }
}
