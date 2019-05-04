// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Demo
{
    public class OffsetTimeDemo
    {
        [Test]
        public void Construction()
        {
            OffsetTime offsetTime = Snippet.For(new OffsetTime(
                new LocalTime(15, 20, 48),
                Offset.FromHours(3)));

            Assert.AreEqual(new LocalTime(15, 20, 48), offsetTime.TimeOfDay);
            Assert.AreEqual(Offset.FromHours(3), offsetTime.Offset);
        }

        [Test]
        public void WithOffset()
        {
            OffsetTime original = new OffsetTime(
                new LocalTime(15, 20, 48),
                Offset.FromHours(3));

            OffsetTime updated = Snippet.For(
                original.WithOffset(Offset.FromHours(-3)));

            Assert.AreEqual(original.TimeOfDay, updated.TimeOfDay);
            Assert.AreEqual(Offset.FromHours(-3), updated.Offset);
        }

        [Test]
        public void With()
        {
            OffsetTime original = new OffsetTime(
                new LocalTime(15, 20, 48),
                Offset.FromHours(3));

            OffsetTime updated = Snippet.For(
                original.With(x => x.PlusHours(5)));

            Assert.AreEqual(new LocalTime(20, 20, 48), updated.TimeOfDay);
            Assert.AreEqual(original.Offset, updated.Offset);
        }
    }
}
