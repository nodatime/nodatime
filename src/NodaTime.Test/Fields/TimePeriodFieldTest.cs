// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Fields;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class TimePeriodFieldTest
    {
        // Just a sample really.
        private static readonly TimePeriodField SampleField = TimePeriodField.Milliseconds;

        [Test]
        [TestCase("00:00:00.567", 0, "00:00:00.567", 0)]
        [TestCase("00:00:00.567", 1, "00:00:00.568", 0)]
        [TestCase("00:00:00.567001", 1, "00:00:00.568001", 0)]
        [TestCase("00:00:00.567", -567, "00:00:00.000", 0)]
        [TestCase("00:00:00.567", -568, "23:59:59.999", -1)]
        [TestCase("23:59:59.000", 1000, "00:00:00.000", 1)]
        public void Add(string start, long units, string expectedEnd, int expectedExtraDays)
        {
            var startTime = LocalTimePattern.ExtendedIsoPattern.Parse(start).Value;
            var expectedEndTime = LocalTimePattern.ExtendedIsoPattern.Parse(expectedEnd).Value;

            int extraDays = 0;
            Assert.AreEqual(expectedEndTime, SampleField.Add(startTime, units, ref extraDays));
            Assert.AreEqual(expectedExtraDays, extraDays);
        }

        [Test]
        [TestCase("00:00:01.000", "00:00:00.000", 1000)]
        [TestCase("23:59:59.000", "00:00:00.000", NodaConstants.MillisecondsPerDay - 1000)]
        [TestCase("00:00:01.0000", "00:00:00.9999", 0)]
        public void Subtract(string minuend, string subtrahend, long expected)
        {
            var minuendTime = LocalTimePattern.ExtendedIsoPattern.Parse(minuend).Value;
            var subtrahendTime = LocalTimePattern.ExtendedIsoPattern.Parse(subtrahend).Value;
            Assert.AreEqual(expected, SampleField.Subtract(minuendTime, subtrahendTime));
            Assert.AreEqual(-expected, SampleField.Subtract(subtrahendTime, minuendTime));
        }
    }
}
