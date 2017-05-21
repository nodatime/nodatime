// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Fields;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Fields
{
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
            var startTime = LocalTimePattern.ExtendedIso.Parse(start).Value;
            var expectedEndTime = LocalTimePattern.ExtendedIso.Parse(expectedEnd).Value;

            int extraDays = 0;
            Assert.AreEqual(expectedEndTime, SampleField.Add(startTime, units, ref extraDays));
            Assert.AreEqual(expectedExtraDays, extraDays);
        }

        [Test]
        [TestCase(Duration.MaxDays * NodaConstants.HoursPerDay - 1, Description = "Using BigInteger")]
        [TestCase(1, Description = "Using long")]
        [TestCase(Duration.MinDays * NodaConstants.HoursPerDay + 1, Description = "Using BigInteger")]
        [TestCase(-1, Description = "Using long")]
        public void GetUnitsInDuration(int hours)
        {
            var duration = Duration.FromHours(hours) + Duration.FromMinutes(30 * Math.Sign(hours));
            Assert.AreEqual(hours, TimePeriodField.Hours.GetUnitsInDuration(duration));
        }

        [Test]
        [TestCase(Duration.MaxDays * NodaConstants.HoursPerDay - 1, Description = "Using BigInteger")]
        [TestCase(1, Description = "Using long")]
        [TestCase(Duration.MinDays * NodaConstants.HoursPerDay, Description = "Using BigInteger (negative)")]
        [TestCase(-1, Description = "Using long (positive)")]
        public void ToDuration(int hours)
        {
            var actual = TimePeriodField.Hours.ToDuration(hours);
            Assert.AreEqual(Duration.FromHours(hours), actual);
        }
    }
}
