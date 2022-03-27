// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NodaTime.Test.TimeZones
{
    public partial class BclDateTimeZoneTest
    {        
        private static readonly ReadOnlyCollection<NamedWrapper<TimeZoneInfo>> BclZones =
            (TestHelper.IsRunningOnMono ? GetSafeSystemTimeZones() : TimeZoneInfo.GetSystemTimeZones())
            .Select(zone => new NamedWrapper<TimeZoneInfo>(zone, zone.Id))
            .ToList()
            .AsReadOnly();

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

        private static int EndTestYearExclusive =>
#if NET6_0_OR_GREATER
            2050;
#else
            // .NET Core 3.1 on Unix doesn't expose the information we need to determine any DST recurrence
            // after the final tzif rule. For the moment, limit how far we check.
            // See https://github.com/dotnet/corefx/issues/17117
            TestHelper.IsRunningOnDotNetCoreUnix? 2037 : 2050;
#endif


        // TODO: Check what this does on Mono, both on Windows and Unix.

        [Test]
        [TestCaseSource(nameof(BclZones))]
        public void AreWindowsStyleRules(NamedWrapper<TimeZoneInfo> zoneWrapper)
        {
            var zone = zoneWrapper.Value;
            var expected = !TestHelper.IsRunningOnDotNetCoreUnix;
            var rules = zone.GetAdjustmentRules();
            if (rules is null || rules.Length == 0)
            {
                return;
            }
            Assert.AreEqual(expected, BclDateTimeZone.AreWindowsStyleRules(rules));
        }

        [Test]
        [TestCaseSource(nameof(BclZones))]
        [Category("BrokenOnMonoLinux")]
        public void AllZoneTransitions(NamedWrapper<TimeZoneInfo> windowsZoneWrapper)
        {
            // The Central Brazilian Standard Time zone is broken in .NET 6.
            // See https://github.com/dotnet/runtime/issues/61842
            if (windowsZoneWrapper.Value.Id == "Central Brazilian Standard Time")
            {
                return;
            }

            var windowsZone = windowsZoneWrapper.Value;
            var nodaZone = BclDateTimeZone.FromTimeZoneInfo(windowsZone);

            Instant instant = Instant.FromUtc(1800, 1, 1, 0, 0);
            Instant end = Instant.FromUtc(EndTestYearExclusive, 1, 1, 0, 0);

            while (instant < end)
            {
                ValidateZoneEquality(instant - Duration.Epsilon, nodaZone, windowsZone);
                ValidateZoneEquality(instant, nodaZone, windowsZone);
                instant = nodaZone.GetZoneInterval(instant).RawEnd;
            }
        }

        [Test]
        [TestCaseSource(nameof(BclZones))]
        public void DisplayName(NamedWrapper<TimeZoneInfo> windowsZoneWrapper)
        {
            var windowsZone = windowsZoneWrapper.Value;
            var nodaZone = BclDateTimeZone.FromTimeZoneInfo(windowsZone);
            Assert.AreEqual(windowsZone.DisplayName, nodaZone.DisplayName);
        }

        /// <summary>
        /// This test catches situations where the Noda Time representation doesn't have all the
        /// transitions it should; AllZoneTransitions may pass not spot times when we *should* have
        /// a transition, because it only uses the transitions it knows about. Instead, here we
        /// check each week between 1st January 1950 and 1st January 2050 (or 2037, in some cases).
        /// We use midnight UTC, but this is arbitrary. The choice of checking once a week is just
        /// practical - it's a relatively slow test, mostly because TimeZoneInfo is slow.
        /// </summary>
        [Test]
        [TestCaseSource(nameof(BclZones))]
        [Category("BrokenOnMonoLinux")]
        public void AllZonesEveryWeek(NamedWrapper<TimeZoneInfo> windowsZoneWrapper)
        {
            // The Central Brazilian Standard Time zone is broken in .NET 6.
            // See https://github.com/dotnet/runtime/issues/61842
            if (windowsZoneWrapper.Value.Id == "Central Brazilian Standard Time")
            {
                return;
            }

            ValidateZoneEveryWeek(windowsZoneWrapper.Value);
        }

        [Test]
        [TestCaseSource(nameof(BclZones))]
        public void AllZonesStartAndEndOfTime(NamedWrapper<TimeZoneInfo> windowsZoneWrapper)
        {
            var nodaZone = BclDateTimeZone.FromTimeZoneInfo(windowsZoneWrapper.Value);
            var firstInterval = nodaZone.GetZoneInterval(Instant.MinValue);
            Assert.IsFalse(firstInterval.HasStart);
            var lastInterval = nodaZone.GetZoneInterval(Instant.MaxValue);
            Assert.IsFalse(lastInterval.HasEnd);
        }

        private void ValidateZoneEveryWeek(TimeZoneInfo windowsZone)
        {
            var nodaZone = BclDateTimeZone.FromTimeZoneInfo(windowsZone);
            Instant instant = Instant.FromUtc(1950, 1, 1, 0, 0);
            Instant end = Instant.FromUtc(EndTestYearExclusive, 1, 1, 0, 0);

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
            // .NET Core on Unix loses data from rules provided to CreateCustomTimeZone :(
            // (It assumes the rules have been created from tzif files, which isn't the case here.)
            // See https://github.com/dotnet/corefx/issues/29912
            Ignore.When(TestHelper.IsRunningOnDotNetCoreUnix, ".NET Core on Unix mangles custom time zones");

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
            // each rule for quite a long time. One of those years is 1948, and the daylight transition is February
            // 29th. That then fails when we try to build a ZoneInterval at the end of that year.
            // See https://github.com/nodatime/nodatime/issues/743 for more details. We've simplified this to just
            // a single rule here...

            // Amusingly, trying to reproduce the test on Mono with a custom time zone causes Mono to throw -
            // quite possibly due to the same root cause that we're testing we've fixed in Noda Time.
            // See https://bugzilla.xamarin.com/attachment.cgi?id=21192&action=edit
            Ignore.When(TestHelper.IsRunningOnMono, "Mono throws an exception with awkward leap years");

            // .NET Core on Unix loses data from rules provided to CreateCustomTimeZone :(
            // (It assumes the rules have been created from tzif files, which isn't the case here.)
            Ignore.When(TestHelper.IsRunningOnDotNetCoreUnix, ".NET Core on Unix mangles custom time zones");

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

        [Test]
        public void FakeDaylightSavingTime()
        {
            // .NET Core on Unix loses data from rules provided to CreateCustomTimeZone :(
            // (It assumes the rules have been created from tzif files, which isn't the case here.)
            Ignore.When(TestHelper.IsRunningOnDotNetCoreUnix, ".NET Core on Unix mangles custom time zones");

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

        // See https://github.com/nodatime/nodatime/issues/1524 for the background
        // It would be nice to test earlier dates (e.g. 1967 and 1998) where transitions
        // actually occurred on the first of the "next month", but the Windows database has very different
        // data from TZDB in those cases.
        [Test]
        public void TransitionAtMidnight()
        {
            var bclZone = GetBclZoneOrIgnore("E. South America Standard Time");
            var nodaTzdbZone = DateTimeZoneProviders.Tzdb["America/Sao_Paulo"];
            var nodaBclZone = BclDateTimeZone.FromTimeZoneInfo(bclZone);

            // Brazil in 2012:
            // Fall back from -02:00 to -03:00 at midnight on February 26th
            var expectedFallBack = Instant.FromUtc(2012, 2, 26, 2, 0, 0);
            // Spring forward from -03:00 to -02:00 at midnight on October 21st
            var expectedSpringForward = Instant.FromUtc(2012, 10, 21, 3, 0, 0);
            // This is an arbitrary instant between the fall back and spring forward.
            var betweenTransitions = Instant.FromUtc(2012, 6, 1, 0, 0, 0);

            // Check that these transitions are as expected when we use TZDB.
            var nodaTzdbInterval = nodaTzdbZone.GetZoneInterval(betweenTransitions);
            Assert.AreEqual(expectedFallBack, nodaTzdbInterval.Start);
            Assert.AreEqual(expectedSpringForward, nodaTzdbInterval.End);

            // Check that the real BCL time zone behaves as reported in the issue: the transitions occur one millisecond early
            var expectedFallBackBclTransition = expectedFallBack - Duration.FromMilliseconds(1);
            Assert.AreEqual(TimeSpan.FromHours(-2), bclZone.GetUtcOffset(expectedFallBackBclTransition.ToDateTimeUtc() - TimeSpan.FromTicks(1)));
            Assert.AreEqual(TimeSpan.FromHours(-3), bclZone.GetUtcOffset(expectedFallBackBclTransition.ToDateTimeUtc()));

            var expectedSpringForwardBclTransition = expectedSpringForward - Duration.FromMilliseconds(1);
            Assert.AreEqual(TimeSpan.FromHours(-3), bclZone.GetUtcOffset(expectedSpringForwardBclTransition.ToDateTimeUtc() - TimeSpan.FromTicks(1)));
            Assert.AreEqual(TimeSpan.FromHours(-2), bclZone.GetUtcOffset(expectedSpringForwardBclTransition.ToDateTimeUtc()));

            // Assert that Noda Time accounts for the Windows time zone data weirdness, and corrects it to
            // a transition at midnight.
            var nodaBclInterval = nodaBclZone.GetZoneInterval(betweenTransitions);
            Assert.AreEqual(nodaTzdbInterval.Start, nodaBclInterval.Start);
            Assert.AreEqual(nodaTzdbInterval.End, nodaBclInterval.End);

            // Finally check the use case that was actually reported
            var actualStartOfDayAfterSpringForward = nodaBclZone.AtStartOfDay(new LocalDate(2012, 10, 21));
            var expectedStartOfDayAfterSpringForward = new LocalDateTime(2012, 10, 21, 1, 0, 0).InZoneStrictly(nodaBclZone);
            Assert.AreEqual(expectedStartOfDayAfterSpringForward, actualStartOfDayAfterSpringForward);
        }

        private void ValidateZoneEquality(Instant instant, DateTimeZone nodaZone, TimeZoneInfo windowsZone)
        {
            // The BCL is basically broken (up to and including .NET 6 at least) around its interpretation
            // of its own data around the new year. See http://codeblog.jonskeet.uk/2014/09/30/the-mysteries-of-bcl-time-zone-data/
            // for details. We're not trying to emulate this behaviour.
            // It's improved over time, but it's still broken in some places. It's not worth worrying about.
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

            // Some midnight transitions in the Noda Time representation are actually corrections for the
            // BCL data indicating 23:59:59.999 on the previous day. If we're testing around midnight,
            // allow the Windows data to be correct for either of those instants.
            var acceptableInstants = new List<Instant> { instant };
            var localTimeOfDay = instant.InZone(nodaZone).TimeOfDay;
            if ((localTimeOfDay == LocalTime.Midnight || localTimeOfDay == LocalTime.MaxValue) && instant > NodaConstants.BclEpoch)
            {
                acceptableInstants.Add(instant - Duration.FromMilliseconds(1));
            }

            var expectedOffsetAsTimeSpan = nodaOffset.ToTimeSpan();

            // Find an instant that at least has the right offset (so will pass the first assertion).
            var instantToTest = acceptableInstants.FirstOrDefault(candidate => windowsZone.GetUtcOffset(candidate.ToDateTimeUtc()) == expectedOffsetAsTimeSpan);
            // If the test is definitely going to fail, just use the original instant that was passed in.
            if (instantToTest == default)
            {
                instantToTest = instant;
            }

            var windowsOffset = windowsZone.GetUtcOffset(instantToTest.ToDateTimeUtc());
            Assert.AreEqual(windowsOffset, expectedOffsetAsTimeSpan, "Incorrect offset at {0} in interval {1}", instantToTest, interval);
            var bclDaylight = windowsZone.IsDaylightSavingTime(instantToTest.ToDateTimeUtc());
            Assert.AreEqual(bclDaylight, interval.Savings != Offset.Zero,
                "At {0}, BCL IsDaylightSavingTime={1}; Noda savings={2}",
                instant, bclDaylight, interval.Savings);
        }

        private TimeZoneInfo GetBclZoneOrIgnore(string systemTimeZoneId) =>
            TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(z => z.Id == systemTimeZoneId)
            ?? Ignore.Throw<TimeZoneInfo>($"Time zone {systemTimeZoneId} not found");
    }
}
