// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

#if !NETCORE

using System;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    public class BclDateTimeZoneTest
    {
        private static readonly ReadOnlyCollection<TimeZoneInfo> BclZones =
            TestHelper.IsRunningOnMono ? GetSafeSystemTimeZones() : TimeZoneInfo.GetSystemTimeZones();

        private static ReadOnlyCollection<TimeZoneInfo> GetSafeSystemTimeZones() =>
             TimeZoneInfo.GetSystemTimeZones()
                // Filter time zones that have a rule involving Feb 29th, to avoid https://bugzilla.xamarin.com/show_bug.cgi?id=54468
                .Where(ZoneRulesDontReferToLeapDays)
                // Recreate all time zones from their rules, to avoid https://bugzilla.xamarin.com/show_bug.cgi?id=54480
                // Arguably this is invalid, but it means we're testing that Noda Time can do as well as it can feasibly
                // do based on the rules.
                .Select(RecreateZone)
                .ToList()
                .AsReadOnly();

        private static bool ZoneRulesDontReferToLeapDays(TimeZoneInfo zone) =>
            !zone.GetAdjustmentRules().Any(rule => TransitionRefersToLeapDay(rule.DaylightTransitionStart) ||
                                                   TransitionRefersToLeapDay(rule.DaylightTransitionEnd));

        private static bool TransitionRefersToLeapDay(TimeZoneInfo.TransitionTime transition) =>
            transition.IsFixedDateRule && transition.Month == 2 && transition.Day == 29;

        private static TimeZoneInfo RecreateZone(TimeZoneInfo zone) =>
            TimeZoneInfo.CreateCustomTimeZone(zone.Id, zone.BaseUtcOffset, zone.DisplayName, zone.StandardName,
                zone.DisplayName, zone.GetAdjustmentRules());

        [Test]
        [TestCaseSource(nameof(BclZones))]
        [Category("BrokenOnMonoLinux")]
        public void AllZoneTransitions(TimeZoneInfo windowsZone)
        {
            var nodaZone = BclDateTimeZone.FromTimeZoneInfo(windowsZone);

            Instant instant = Instant.FromUtc(1800, 1, 1, 0, 0);
            Instant end = Instant.FromUtc(2050, 1, 1, 0, 0);

            while (instant < end)
            {
                ValidateZoneEquality(instant - Duration.Epsilon, nodaZone, windowsZone);
                ValidateZoneEquality(instant, nodaZone, windowsZone);
                instant = nodaZone.GetZoneInterval(instant).RawEnd;
            }
        }

        [Test]
        [TestCaseSource(nameof(BclZones))]
        public void DisplayName(TimeZoneInfo windowsZone)
        {
            var nodaZone = BclDateTimeZone.FromTimeZoneInfo(windowsZone);
            Assert.AreEqual(windowsZone.DisplayName, nodaZone.DisplayName);
        }

        /// <summary>
        /// This test catches situations where the Noda Time representation doesn't have all the
        /// transitions it should; AllZoneTransitions may pass not spot times when we *should* have
        /// a transition, because it only uses the transitions it knows about. Instead, here we
        /// check each day between 1st January 1950 and 1st January 2050. We use midnight UTC, but
        /// this is arbitrary. The choice of checking once a week is just practical - it's a relatively
        /// slow test, mostly because TimeZoneInfo is slow.
        /// </summary>
        [Test]
        [TestCaseSource(nameof(BclZones))]
        [Category("BrokenOnMonoLinux")]
        public void AllZonesEveryWeek(TimeZoneInfo windowsZone)
        {
            ValidateZoneEveryWeek(windowsZone);
        }

        // This demonstrates bug 115.
        [Test]
        public void Namibia()
        {
            String bclId = "Namibia Standard Time";
            try
            {
                ValidateZoneEveryWeek(TimeZoneInfo.FindSystemTimeZoneById(bclId));
            }
            catch (TimeZoneNotFoundException)
            {
                // This may occur on Mono, for example.
                Assert.Ignore("Test assumes existence of BCL zone with ID: " + bclId);
            }
        }

        [Test]
        [TestCaseSource(nameof(BclZones))]
        public void AllZonesStartAndEndOfTime(TimeZoneInfo windowsZone)
        {
            var nodaZone = BclDateTimeZone.FromTimeZoneInfo(windowsZone);
            var firstInterval = nodaZone.GetZoneInterval(Instant.MinValue);
            Assert.IsFalse(firstInterval.HasStart);
            var lastInterval = nodaZone.GetZoneInterval(Instant.MaxValue);
            Assert.IsFalse(lastInterval.HasEnd);
        }

        private void ValidateZoneEveryWeek(TimeZoneInfo windowsZone)
        {
            var nodaZone = BclDateTimeZone.FromTimeZoneInfo(windowsZone);

            Instant instant = Instant.FromUtc(1950, 1, 1, 0, 0);
            Instant end = Instant.FromUtc(2050, 1, 1, 0, 0);

            while (instant < end)
            {
                ValidateZoneEquality(instant, nodaZone, windowsZone);
                instant += Duration.OneWeek;
            }
        }

        [Test]
        public void ForSystemDefault()
        {
            // Assume that the local time zone doesn't change between two calls...
            TimeZoneInfo local = TimeZoneInfo.Local;
            BclDateTimeZone nodaLocal1 = BclDateTimeZone.ForSystemDefault();
            BclDateTimeZone nodaLocal2 = BclDateTimeZone.ForSystemDefault();
            // Check it's actually the right zone
            Assert.AreSame(local, nodaLocal1.OriginalZone);
            // Check it's cached
            Assert.AreSame(nodaLocal1, nodaLocal2);
        }

        [Test]
        public void DateTimeMinValueStartRuleExtendsToBeginningOfTime()
        {
            var rules = new[]
            {
                // Rule for the whole of time, with DST of 1 hour commencing on March 1st
                // and ending on September 1st.
                TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(
                    DateTime.MinValue, DateTime.MaxValue.Date, TimeSpan.FromHours(1),
                    TimeZoneInfo.TransitionTime.CreateFixedDateRule(DateTime.MinValue, 3, 1),
                    TimeZoneInfo.TransitionTime.CreateFixedDateRule(DateTime.MinValue, 9, 1))
            };
            var bclZone = TimeZoneInfo.CreateCustomTimeZone("custom", baseUtcOffset: TimeSpan.Zero,
                displayName: "DisplayName", standardDisplayName: "Standard",
                daylightDisplayName: "Daylight",
                adjustmentRules: rules);
            var nodaZone = BclDateTimeZone.FromTimeZoneInfo(bclZone);
            // Standard time in February BC 101
            Assert.AreEqual(Offset.Zero, nodaZone.GetUtcOffset(Instant.FromUtc(-100, 2, 1, 0, 0)));
            // Daylight time in July BC 101
            Assert.AreEqual(Offset.FromHours(1), nodaZone.GetUtcOffset(Instant.FromUtc(-100, 7, 1, 0, 0)));
            // Standard time in October BC 101
            Assert.AreEqual(Offset.Zero, nodaZone.GetUtcOffset(Instant.FromUtc(-100, 10, 1, 0, 0)));
        }

        [Test]
        public void AwkwardLeapYears()
        {
            // This mimics the data on Mono on Linux for Europe/Malta, where there's a BCL adjustment rule for
            // each rule for quite a long time. One of those years is 1948, and the daylight transition is Feburary
            // 29th. That then fails when we try to build a ZoneInterval at the end of that year.
            // See https://github.com/nodatime/nodatime/issues/743 for more details. We've simplified this to just
            // a single rule here...

            // Amusingly, trying to reproduce the test on Mono with a custom time zone causes Mono to throw -
            // quite possibly due to the same root cause that we're testing we've fixed in Noda Time.
            // See https://bugzilla.xamarin.com/attachment.cgi?id=21192&action=edit
            if (TestHelper.IsRunningOnMono)
            {
                Assert.Ignore("Test skipped on Mono");
            }

            var rules = new[]
            {
                TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(
                    dateStart: new DateTime(1948, 1, 1),
                    dateEnd: new DateTime(1949, 1, 1).AddDays(-1),
                    daylightDelta: TimeSpan.FromHours(1),
                    daylightTransitionStart: TimeZoneInfo.TransitionTime.CreateFixedDateRule(timeOfDay: new DateTime(1, 1, 1, 2, 0, 0), month: 2, day: 29),
                    daylightTransitionEnd: TimeZoneInfo.TransitionTime.CreateFixedDateRule(timeOfDay: new DateTime(1, 1, 1, 3, 0, 0), month: 10, day: 3))
            };

            var bclZone = TimeZoneInfo.CreateCustomTimeZone("Europe/Malta", TimeSpan.Zero, "Malta", "Standard", "Daylight", rules);
            var nodaZone = BclDateTimeZone.FromTimeZoneInfo(bclZone);

            var expectedTransition1 = Instant.FromUtc(1948, 2, 29, 2, 0, 0);
            var expectedTransition2 = Instant.FromUtc(1948, 10, 3, 2, 0, 0); // 3am local time

            var zoneIntervalBefore = nodaZone.GetZoneInterval(Instant.FromUtc(1947, 1, 1, 0, 0));
            Assert.AreEqual(
                new ZoneInterval("Standard", Instant.BeforeMinValue, expectedTransition1, Offset.Zero, Offset.Zero),
                zoneIntervalBefore);

            var daylightZoneInterval = nodaZone.GetZoneInterval(Instant.FromUtc(1948, 6, 1, 0, 0));
            Assert.AreEqual(
                new ZoneInterval("Daylight", expectedTransition1, expectedTransition2, Offset.FromHours(1), Offset.FromHours(1)),
                daylightZoneInterval);

            var zoneIntervalAfter = nodaZone.GetZoneInterval(Instant.FromUtc(1949, 1, 1, 0, 0));
            Assert.AreEqual(
                new ZoneInterval("Standard", expectedTransition2, Instant.AfterMaxValue, Offset.Zero, Offset.Zero),
                zoneIntervalAfter);
        }

        [Test]
        public void LocalZoneIsNull()
        {
            var systemZone = TimeZoneInfo.CreateCustomTimeZone("Normal zone", TimeSpan.Zero, "Display", "Standard");
            using (TimeZoneInfoReplacer.Replace(null, systemZone))
            {
                Assert.Throws<InvalidOperationException>(() => BclDateTimeZone.ForSystemDefault());
            }
        }

#if NET451
        [Test]
        public void FakeDaylightSavingTime()
        {
            // Linux time zones on Mono can have a strange situation with a "0 savings" adjustment rule to represent
            // "we want to change standard time but we can't".
            // See https://github.com/nodatime/nodatime/issues/746
            // Normally the odd rule would only be in place for a year, but it's simplest to just make it all the time.
            // We go into daylight savings at midday on March 10th, and out again at midday on September 25.

            // We should be able to use DateTime.MaxValue for dateEnd, but not in .NET 4.5 apparently.
            var rule = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(DateTime.MinValue, new DateTime(9999, 12, 31), TimeSpan.Zero,
                TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 12, 0, 0), 3, 10),
                TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 12, 0, 0), 9, 25));
            var bclZone = TimeZoneInfo.CreateCustomTimeZone("Nasty", TimeSpan.FromHours(4), "Display", "Standard", "Daylight", new[] { rule });
            var nodaZone = BclDateTimeZone.FromTimeZoneInfo(bclZone);
            var winterInterval = nodaZone.GetZoneInterval(Instant.FromUtc(2017, 2, 1, 0, 0));
            var summerInterval = nodaZone.GetZoneInterval(Instant.FromUtc(2017, 6, 1, 0, 0));

            var expectedWinter = new ZoneInterval("Standard", Instant.FromUtc(2016, 9, 25, 8, 0), Instant.FromUtc(2017, 3, 10, 8, 0), Offset.FromHours(4), Offset.Zero);
            var expectedSummer = new ZoneInterval("Daylight", Instant.FromUtc(2017, 3, 10, 8, 0), Instant.FromUtc(2017, 9, 25, 8, 0), Offset.FromHours(4), Offset.FromHours(1));

            Assert.AreEqual(expectedWinter, winterInterval);
            Assert.AreEqual(expectedSummer, summerInterval);
        }
#endif

        private void ValidateZoneEquality(Instant instant, DateTimeZone nodaZone, TimeZoneInfo windowsZone)
        {
            // The BCL is basically broken (up to and including .NET 4.5.1 at least) around its interpretation
            // of its own data around the new year. See http://codeblog.jonskeet.uk/2014/09/30/the-mysteries-of-bcl-time-zone-data/
            // for details. We're not trying to emulate this behaviour.
            // It's a lot *better* for .NET 4.6, 
            var utc = instant.InUtc();
            if ((utc.Month == 12 && utc.Day == 31) || (utc.Month == 1 && utc.Day == 1))
            {
                return;
            }

            var interval = nodaZone.GetZoneInterval(instant);

            // Check that the zone interval really represents a transition. It could be a change in
            // wall offset, name, or the split between standard time and daylight savings for the interval.
            if (interval.RawStart != Instant.BeforeMinValue)
            {
                var previousInterval = nodaZone.GetZoneInterval(interval.Start - Duration.Epsilon);
                Assert.AreNotEqual(new {interval.WallOffset, interval.Name, interval.StandardOffset},
                    new {previousInterval.WallOffset, previousInterval.Name, previousInterval.StandardOffset},
                    "Non-transition from {0} to {1}", previousInterval, interval);
            }
            var nodaOffset = interval.WallOffset;
            var windowsOffset = windowsZone.GetUtcOffset(instant.ToDateTimeUtc());
            Assert.AreEqual(windowsOffset, nodaOffset.ToTimeSpan(), "Incorrect offset at {0} in interval {1}", instant, interval);
            var bclDaylight = windowsZone.IsDaylightSavingTime(instant.ToDateTimeUtc());
            Assert.AreEqual(bclDaylight, interval.Savings != Offset.Zero,
                "At {0}, BCL IsDaylightSavingTime={1}; Noda savings={2}",
                instant, bclDaylight, interval.Savings);
        }
    }
}

#endif
