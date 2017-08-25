// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NUnit.Framework;
using System;
using System.Linq;

namespace NodaTime.Test.Text
{
    public class CompositePatternBuilderPatternTest
    {
        // See https://github.com/nodatime/nodatime/issues/607
        [Test]
        [TestCase("2017-02-23T16:40:50.123456789")]
        [TestCase("2017-02-23T16:40:50.123")]
        [TestCase("2017-02-23T16:40:50")]
        [TestCase("2017-02-23T16:40")]
        public void IsoPattern(string text)
        {
            // We assert that the text round-trips. If it does, it's
            // reasonable to assume it parsed correctly...
            var shortPattern = LocalDateTimePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm");
            var pattern = new CompositePatternBuilder<LocalDateTime>
            {
                { LocalDateTimePattern.ExtendedIso, _ => true },
                { shortPattern, ldt => ldt.Second == 0 && ldt.NanosecondOfSecond == 0 }
            }.Build();
            var value = pattern.Parse(text).Value;
            string formatted = pattern.Format(value);
            Assert.AreEqual(text, formatted);
        }

        [Test]
        public void Format_NoValidPattern()
        {
            var pattern = new CompositePatternBuilder<LocalDate>
            {
                { LocalDatePattern.Iso, _ => false },
                { LocalDatePattern.CreateWithInvariantCulture("yyyy"), _ => false },
            }.Build();
            Assert.Throws<FormatException>(() => pattern.Format(new LocalDate(2017, 1, 1)));
        }

        [Test]
        public void Parse()
        {
            var pattern = new CompositePatternBuilder<LocalDate>
            {
                { LocalDatePattern.Iso, _ => true },
                { LocalDatePattern.CreateWithInvariantCulture("yyyy"), _ => false },
            }.Build();
            Assert.IsTrue(pattern.Parse("2017-03-20").Success);
            Assert.IsFalse(pattern.Parse("2017-03").Success);
            Assert.IsTrue(pattern.Parse("2017").Success);
        }

        [Test]
        public void Build_Empty()
        {
            var pattern = new CompositePatternBuilder<LocalDate>();
            Assert.Throws<InvalidOperationException>(() => pattern.Build());
        }

        [Test]
        public void Enumerators()
        {
            var pattern1 = LocalDatePattern.Iso;
            var pattern2 = LocalDatePattern.CreateWithInvariantCulture("yyyy");

            var builder = new CompositePatternBuilder<LocalDate>
            {
                { pattern1, _ => true },
                { pattern2, _ => false },
            };

            CollectionAssert.AreEqual(new[] { pattern1, pattern2 }, builder.ToList());
            CollectionAssert.AreEqual(new[] { pattern1, pattern2 }, builder.OfType<LocalDatePattern>().ToList());
        }
    }
}
