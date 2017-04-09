// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Text;
using NodaTime.Globalization;
using NodaTime.Text;
using NodaTime.Text.Patterns;
using NUnit.Framework;
using System.Collections.Generic;

namespace NodaTime.Test.Text.Patterns
{
    /// <summary>
    /// Tests for SteppedPatternBuilder, often using OffsetPatternParser as this is known
    /// to use SteppedPatternBuilder.
    /// </summary>
    public class SteppedPatternBuilderTest
    {
        private static readonly IPartialPattern<Offset> SimpleOffsetPattern =
            (IPartialPattern<Offset>)new OffsetPatternParser().ParsePattern("HH:mm", NodaFormatInfo.InvariantInfo);

        [Test]
        public void ParsePartial_ValidInMiddle()
        {
            var value = new ValueCursor("x17:30y");
            value.MoveNext();
            value.MoveNext();
            // Start already looking at the value to parse
            Assert.AreEqual('1', value.Current);
            var result = SimpleOffsetPattern.ParsePartial(value);
            Assert.AreEqual(Offset.FromHoursAndMinutes(17, 30), result.Value);
            // Finish just after the value
            Assert.AreEqual('y', value.Current);
        }

        [Test]
        public void ParsePartial_ValidAtEnd()
        {
            var value = new ValueCursor("x17:30");
            value.MoveNext();
            value.MoveNext();
            var result = SimpleOffsetPattern.ParsePartial(value);
            Assert.AreEqual(Offset.FromHoursAndMinutes(17, 30), result.Value);
            // Finish just after the value, which in this case is at the end.
            Assert.AreEqual(TextCursor.Nul, value.Current);
        }

        [Test]
        public void Parse_Partial_Invalid()
        {
            var value = new ValueCursor("x17:y");
            value.MoveNext();
            value.MoveNext();
            var result = SimpleOffsetPattern.ParsePartial(value);
            Assert.Throws<UnparsableValueException>(() => result.GetValueOrThrow());
        }

        [Test]
        public void AppendFormat()
        {
            var builder = new StringBuilder("x");
            var offset = Offset.FromHoursAndMinutes(17, 30);
            SimpleOffsetPattern.AppendFormat(offset, builder);
            Assert.AreEqual("x17:30", builder.ToString());
        }

        [Test]
        [TestCase("aBaB", true)]
        [TestCase("aBAB", false)] // Case-sensitive
        [TestCase("<aBaB", false)] // < is reserved
        [TestCase("aBaB>", false)] // > is reserved
        public void UnhandledLiteral(string text, bool valid)
        {
            CharacterHandler<LocalDate, SampleBucket> handler = delegate { };
            var handlers = new Dictionary<char, CharacterHandler<LocalDate, SampleBucket>>
            {
                { 'a', handler },
                { 'B', handler }
            };
            var builder = new SteppedPatternBuilder<LocalDate, SampleBucket>(NodaFormatInfo.InvariantInfo, () => new SampleBucket());
            if (valid)
            {
                builder.ParseCustomPattern(text, handlers);
            }
            else
            {
                Assert.Throws<InvalidPatternException>(() => builder.ParseCustomPattern(text, handlers));
            }
        }

        private class SampleBucket : ParseBucket<LocalDate>
        {
            internal override ParseResult<LocalDate> CalculateValue(PatternFields usedFields, string value)
            {
                throw new NotImplementedException();
            }
        }
    }
}
