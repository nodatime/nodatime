// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Linq;
using NodaTime.Calendars;
using NodaTime.TimeZones;
using NUnit.Framework;
using System;

namespace NodaTime.Demo
{
    /// <summary>
    /// These are all examples taken from real Stack Overflow questions. Each contains
    /// a link to the original question. They may have had nothing to do with .NET in the original
    /// form, but the code demonstrates some aspect of solving the problem in Noda Time.
    /// </summary>
    [TestFixture]
    public class StackOverflowExamples
    {
        /// <summary>
        /// Why would iOS 6 not be able to parse a Julian date of 2475213?
        /// <see href="http://stackoverflow.com/questions/12922645"/>
        /// </summary>
        [Test]
        public void MysteryTimeZones()
        {
            var julianCalendar = CalendarSystem.GetJulianCalendar(4);
            var julianEpoch = new LocalDate(Era.BeforeCommon, 4713, 1, 1, julianCalendar);
            var sampleDate = julianEpoch.PlusDays(2475213);

            Console.WriteLine("Sample date in ISO calendar: {0}", sampleDate.WithCalendar(CalendarSystem.Iso));

            var zoneProvider = DateTimeZoneProviders.Tzdb;
            foreach (var id in zoneProvider.Ids)
            {
                var zone = zoneProvider[id];
                if (zone.AtStartOfDay(sampleDate).LocalDateTime.TimeOfDay != LocalTime.Midnight)
                {
                    Console.WriteLine(id);
                }
            }
        }

        /// <summary>
        /// What's the time zone of Sat, 27 Oct 2012 23:47:57 -0700?
        /// <see cref="http://stackoverflow.com/questions/13172341" />
        /// </summary>
        [Test]
        public void FindCandidateZones()
        {
            // Unfortunately we can't (yet - November 1st 2012) parse an OffsetDateTime. One day...
            OffsetDateTime odt = new OffsetDateTime(new LocalDateTime(2012, 10, 27, 23, 47, 57), Offset.FromHours(-7));
            var targetInstant = odt.ToInstant();
            var targetOffset = odt.Offset;

            var zoneProvider = DateTimeZoneProviders.Tzdb;
            foreach (var id in zoneProvider.Ids)
            {
                var zone = zoneProvider[id];
                if (zone.GetUtcOffset(targetInstant) == targetOffset)
                {
                    Console.WriteLine(id);
                }
            }
        }

        /// <summary>
        /// Why does "1927-12-31 23:54:08" minus "1927-12-31 23:54:07" give a difference of nearly 6 minutes?
        /// <see cref="http://stackoverflow.com/questions/6841333" />
        /// </summary>
        [Test]
        public void CuriousSubtraction()
        {
            var shanghai = DateTimeZoneProviders.Tzdb["Asia/Shanghai"];
            var localBefore = new LocalDateTime(1927, 12, 31, 23, 54, 07);
            var localAfter = localBefore.PlusSeconds(1);
            var instantBefore = localBefore.InZoneLeniently(shanghai).ToInstant();
            var instantAfter = localAfter.InZoneLeniently(shanghai).ToInstant();

            Assert.AreEqual(Duration.FromSeconds(353), instantAfter - instantBefore);

            // Now let's resolve them differently...
            var resolver = Resolvers.CreateMappingResolver(Resolvers.ReturnEarlier, Resolvers.ThrowWhenSkipped);
            instantBefore = localBefore.InZone(shanghai, resolver).ToInstant();
            instantAfter = localAfter.InZone(shanghai, resolver).ToInstant();
            Assert.AreEqual(Duration.FromSeconds(1), instantAfter - instantBefore);
        }

        /// <summary>
        /// How can I dedupe the IDs in TZDB? (There's more to the question, but this is one part of it...)
        /// <see cref="http://stackoverflow.com/questions/13321716"/>
        /// </summary>
        [Test]
        public void Aliases()
        {
            var source = TzdbDateTimeZoneSource.Default;
            Console.WriteLine("TZDB has {0} entries; {1} are canonical.", source.GetIds().Count(), source.Aliases.Count());
            Console.WriteLine("Aliases for Europe/London:");
            foreach (var alias in source.Aliases["Europe/London"])
            {
                Console.WriteLine("  {0}", alias);
            }
        }

    }
}
