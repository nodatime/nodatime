// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using System.Numerics;

namespace NodaTime.Test
{
    partial class DurationTest
    {
        // TODO: Remove these and just have using static NodaTime.NodaConstants
        // when the Mono-latest compiler works properly
        const long NanosecondsPerDay = NodaConstants.NanosecondsPerDay;
        const long NanosecondsPerHour = NodaConstants.NanosecondsPerHour;
        const long NanosecondsPerMinute = NodaConstants.NanosecondsPerMinute;
        const long NanosecondsPerSecond = NodaConstants.NanosecondsPerSecond;
        const long NanosecondsPerMillisecond = NodaConstants.NanosecondsPerMillisecond;

        [Test]
        public void Zero()
        {
            Duration test = Duration.Zero;
            Assert.AreEqual(0, test.Ticks);
        }

        private static void TestFactoryMethod(Func<long, Duration> method, long value, long nanosecondsPerUnit)
        {
            Duration duration = method(value);
            BigInteger expectedNanoseconds = (BigInteger)value * nanosecondsPerUnit;
            Assert.AreEqual(duration.ToBigIntegerNanoseconds(), expectedNanoseconds);
        }

        private static void TestFactoryMethod(Func<int, Duration> method, int value, long nanosecondsPerUnit)
        {
            Duration duration = method(value);
            BigInteger expectedNanoseconds = (BigInteger) value * nanosecondsPerUnit;
            Assert.AreEqual(duration.ToBigIntegerNanoseconds(), expectedNanoseconds);
        }

        [Test]
        [TestCase(-100), TestCase(-1), TestCase(0), TestCase(1), TestCase(100)]
        public void FromDays(int days)
        {
            TestFactoryMethod(Duration.FromDays, days, NanosecondsPerDay);
        }

        [Test]
        [TestCase(-100), TestCase(-1), TestCase(0), TestCase(1), TestCase(100)]
        public void FromHours(int hours)
        {
            TestFactoryMethod(Duration.FromHours, hours, NanosecondsPerHour);
        }

        [Test]
        [TestCase(int.MinValue - 100L), TestCase(-100), TestCase(-1), TestCase(0)]
        [TestCase(1), TestCase(100), TestCase(int.MaxValue + 100L)]
        public void FromMinutes(long minutes)
        {
            TestFactoryMethod(Duration.FromMinutes, minutes, NanosecondsPerMinute);
        }

        [Test]
        [TestCase(int.MinValue - 100L), TestCase(-100), TestCase(-1), TestCase(0)]
        [TestCase(1), TestCase(100), TestCase(int.MaxValue + 100L)]
        public void FromSeconds(long seconds)
        {
            TestFactoryMethod(Duration.FromSeconds, seconds, NanosecondsPerSecond);
        }

        [Test]
        [TestCase(int.MinValue - 100L), TestCase(-100), TestCase(-1), TestCase(0)]
        [TestCase(1), TestCase(100), TestCase(int.MaxValue + 100L)]
        public void FromMilliseconds(long milliseconds)
        {
            TestFactoryMethod(Duration.FromMilliseconds, milliseconds, NanosecondsPerMillisecond);
        }

        [Test]
        public void FromAndToTimeSpan()
        {
            TimeSpan timeSpan = TimeSpan.FromHours(3) + TimeSpan.FromSeconds(2) + TimeSpan.FromTicks(1);
            Duration duration = Duration.FromHours(3) + Duration.FromSeconds(2) + Duration.FromTicks(1);
            Assert.AreEqual(duration, Duration.FromTimeSpan(timeSpan));
            Assert.AreEqual(timeSpan, duration.ToTimeSpan());

            Duration maxDuration = Duration.FromTicks(Int64.MaxValue);
            Assert.AreEqual(maxDuration, Duration.FromTimeSpan(TimeSpan.MaxValue));
            Assert.AreEqual(TimeSpan.MaxValue, maxDuration.ToTimeSpan());
            Duration minDuration = Duration.FromTicks(Int64.MinValue);

            Assert.AreEqual(minDuration, Duration.FromTimeSpan(TimeSpan.MinValue));
            Assert.AreEqual(TimeSpan.MinValue, minDuration.ToTimeSpan());
        }

        [Test]
        public void FromNanoSeconds_Int64()
        {
            Assert.AreEqual(Duration.OneDay - Duration.Epsilon, Duration.FromNanoseconds(NanosecondsPerDay - 1L));
            Assert.AreEqual(Duration.OneDay, Duration.FromNanoseconds(NanosecondsPerDay));
            Assert.AreEqual(Duration.OneDay + Duration.Epsilon, Duration.FromNanoseconds(NanosecondsPerDay + 1L));

            Assert.AreEqual(-Duration.OneDay - Duration.Epsilon, Duration.FromNanoseconds(-NanosecondsPerDay - 1L));
            Assert.AreEqual(-Duration.OneDay, Duration.FromNanoseconds(-NanosecondsPerDay));
            Assert.AreEqual(-Duration.OneDay + Duration.Epsilon, Duration.FromNanoseconds(-NanosecondsPerDay + 1L));
        }

        [Test]
        public void FromNanoSeconds_BigInteger()
        {
            Assert.AreEqual(Duration.OneDay - Duration.Epsilon, Duration.FromNanoseconds(NanosecondsPerDay - BigInteger.One));
            Assert.AreEqual(Duration.OneDay, Duration.FromNanoseconds(NanosecondsPerDay + BigInteger.Zero));
            Assert.AreEqual(Duration.OneDay + Duration.Epsilon, Duration.FromNanoseconds(NanosecondsPerDay + BigInteger.One));

            Assert.AreEqual(-Duration.OneDay - Duration.Epsilon, Duration.FromNanoseconds(-NanosecondsPerDay - BigInteger.One));
            Assert.AreEqual(-Duration.OneDay, Duration.FromNanoseconds(-NanosecondsPerDay + BigInteger.Zero));
            Assert.AreEqual(-Duration.OneDay + Duration.Epsilon, Duration.FromNanoseconds(-NanosecondsPerDay + BigInteger.One));
        }
    }
}
