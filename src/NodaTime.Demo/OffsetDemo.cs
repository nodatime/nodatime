// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using System;

namespace NodaTime.Demo
{
    public class OffsetDemo
    {
        [Test]
        public void ConstructionFromHours()
        {
            Offset offset = Snippet.For(Offset.FromHours(1));
            Assert.AreEqual(3600, offset.Seconds);
        }

        [Test]
        public void ConstructionFromHoursAndMinutes()
        {
            Offset offset = Snippet.For(Offset.FromHoursAndMinutes(1, 1));
            Assert.AreEqual(3660, offset.Seconds);
        }

        [Test]
        public void ConstructionFromSeconds()
        {
            Offset offset = Snippet.For(Offset.FromSeconds(450));
            Assert.AreEqual(450, offset.Seconds);
        }

        [Test]
        public void ConstructionFromMilliseconds()
        {
            Offset offset = Snippet.For(Offset.FromMilliseconds(1200));
            Assert.AreEqual(1, offset.Seconds);
            Assert.AreEqual(1000, offset.Milliseconds);
        }

        [Test]
        public void ConstructionFromTicks()
        {
            Offset offset = Snippet.For(Offset.FromTicks(15_000_000));
            Assert.AreEqual(10_000_000, offset.Ticks);
            Assert.AreEqual(1, offset.Seconds);
        }

        [Test]
        public void ConstructionFromNanoseconds()
        {
            Offset offset = Snippet.For(Offset.FromNanoseconds(1_200_000_000));
            Assert.AreEqual(1, offset.Seconds);
            Assert.AreEqual(1_000_000_000, offset.Nanoseconds);
        }

        [Test]
        public void ConstructionFromTimeSpan()
        {
            var timespan = TimeSpan.FromHours(1.5);
            Offset offset = Snippet.For(Offset.FromTimeSpan(timespan));
            Assert.AreEqual(5400, offset.Seconds);
        }

        [Test]
        public void Add()
        {
            var lefHandOffset = Offset.FromHours(5);
            var rightHandOffset = Offset.FromHours(6);

            var result = Snippet.For(Offset.Add(lefHandOffset, rightHandOffset));

            Assert.AreEqual(result, lefHandOffset + rightHandOffset);
        }

        [Test]
        public void Substract()
        {
            var lefHandOffset = Offset.FromHours(7);
            var rightHandOffset = Offset.FromHours(5);

            var result = Snippet.For(Offset.Subtract(lefHandOffset, rightHandOffset));

            Assert.AreEqual(result, lefHandOffset - rightHandOffset);
        }

        [Test]
        public void CompareTo()
        {
            var smallerOffset = Offset.FromHours(3);
            var largerOffset = Offset.FromHours(5);
            var equalOffset = Offset.FromHours(5);

            var lessThan = smallerOffset.CompareTo(largerOffset);
            var equalTo = largerOffset.CompareTo(equalOffset);
            var largerThan = largerOffset.CompareTo(smallerOffset);

            Assert.Less(lessThan, 0);
            Assert.AreEqual(equalTo, 0);
            Assert.Greater(largerThan, 0);
        }

        [Test]
        public void Equals()
        {
            var equalOffset1 = Offset.FromHoursAndMinutes(1, 30);
            var equalOffset2 = Offset.FromHoursAndMinutes(1, 30);
            var inequalOffset = Offset.FromHours(2);

            var isEqual = equalOffset1.Equals(equalOffset2);
            var isInequal = equalOffset1.Equals(inequalOffset);

            Assert.IsTrue(isEqual);
            Assert.IsFalse(isInequal);
        }

        [Test]
        public void Max()
        {
            var smallerOffset = Offset.FromHours(3);
            var largerOffset = Offset.FromHours(5);

            var result = Snippet.For(Offset.Max(smallerOffset, largerOffset));

            Assert.AreEqual(largerOffset, result);
        }

        [Test]
        public void Min()
        {
            var smallerOffset = Offset.FromHours(3);
            var largerOffset = Offset.FromHours(5);

            var result = Snippet.For(Offset.Min(smallerOffset, largerOffset));

            Assert.AreEqual(smallerOffset, result);
        }

        [Test]
        public void Negate()
        {
            var offsetToNegate = Offset.FromHours(2);

            var result = Snippet.For(Offset.Negate(offsetToNegate));

            Assert.AreEqual(result, -offsetToNegate);
        }

        [Test]
        public void Plus()
        {
            var offset = Offset.FromSeconds(100);
            var offset2 = Offset.FromSeconds(150);
            var expected = Offset.FromSeconds(250);

            var actual = Snippet.For(offset.Plus(offset2));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Minus()
        {
            var offset = Offset.FromSeconds(100);
            var offset2 = Offset.FromSeconds(120);
            var expected = Offset.FromSeconds(-20);

            var actual = Snippet.For(offset.Minus(offset2));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToTimeSpan()
        {
            var offset = Offset.FromSeconds(120);
            var actual = Snippet.For(offset.ToTimeSpan());
            var expected = TimeSpan.FromSeconds(120);

            Assert.AreEqual(expected, actual);
        }
    }
}