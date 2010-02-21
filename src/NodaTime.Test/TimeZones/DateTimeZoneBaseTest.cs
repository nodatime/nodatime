#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Tests within this class test the functionality within DateTimeZoneBase, even though it
    /// tests it via concrete implementations.
    /// </summary>
    [TestFixture]
    public class DateTimeZoneBaseTest
    {
        private static readonly IDateTimeZone Paris = DateTimeZones.ForId("Europe/Paris");
        private static readonly IDateTimeZone LosAngeles = DateTimeZones.ForId("America/Los_Angeles");
        private static readonly IDateTimeZone NewZealand = DateTimeZones.ForId("NZ"); // FIXME!

        // Paris goes from +1 to +2 on March 28th 2010 at 2am wall time
        [Test]
        public void GetOffsetFromLocal_ParisSpringTransition()
        {
            LocalDateTime before = new LocalDateTime(2010, 3, 28, 1, 30);
            LocalDateTime impossible = new LocalDateTime(2010, 3, 28, 2, 30);
            LocalDateTime atTransition = new LocalDateTime(2010, 3, 28, 3, 0);
            LocalDateTime after = new LocalDateTime(2010, 3, 28, 3, 30);
            AssertOffset(1, before, Paris);
            AssertImpossible(impossible, Paris);
            AssertOffset(2, atTransition, Paris);
            AssertOffset(2, after, Paris);
        }

        // Paris goes from +2 to +1 on October 31st 2010 at 3am wall time
        [Test]
        public void GetOffsetFromLocal_ParisFallTransition()
        {
            LocalDateTime before = new LocalDateTime(2010, 10, 31, 1, 30);
            LocalDateTime atTransition = new LocalDateTime(2010, 10, 31, 2, 0);
            LocalDateTime ambiguous = new LocalDateTime(2010, 10, 31, 2, 30);
            LocalDateTime after = new LocalDateTime(2010, 10, 31, 3, 30);
            AssertOffset(2, before, Paris);
            AssertOffset(1, ambiguous, Paris);
            AssertOffset(1, atTransition, Paris);
            AssertOffset(1, after, Paris);
        }

        // Los Angeles goes from -8 to -7 on March 14th 2010 at 2am wall time
        [Test]
        public void GetOffsetFromLocal_LosAngelesSpringTransition()
        {
            LocalDateTime before = new LocalDateTime(2010, 3, 14, 1, 30);
            LocalDateTime impossible = new LocalDateTime(2010, 3, 14, 2, 30);
            LocalDateTime atTransition = new LocalDateTime(2010, 3, 14, 3, 0);
            LocalDateTime after = new LocalDateTime(2010, 3, 14, 3, 30);
            AssertOffset(-8, before, LosAngeles);
            AssertImpossible(impossible, LosAngeles);
            AssertOffset(-7, atTransition, LosAngeles);
            AssertOffset(-7, after, LosAngeles);
        }

        // Los Angeles goes from -7 to -8 on November 7th 2010 at 2am wall time
        [Test]
        public void GetOffsetFromLocal_LosAngelesFallTransition()
        {
            LocalDateTime before = new LocalDateTime(2010, 11, 7, 0, 30);
            LocalDateTime atTransition = new LocalDateTime(2010, 11, 7, 1, 0);
            LocalDateTime ambiguous = new LocalDateTime(2010, 11, 7, 1, 30);
            LocalDateTime after = new LocalDateTime(2010, 11, 7, 2, 30);
            AssertOffset(-7, before, LosAngeles);
            AssertOffset(-8, atTransition, LosAngeles);
            AssertOffset(-8, ambiguous, LosAngeles);
            AssertOffset(-8, after, LosAngeles);
        }

        // New Zealand goes from +13 to +12 on April 4th 2010 at 3am wall time
        [Test]
        public void GetOffsetFromLocal_NewZealandFallTransition()
        {
            LocalDateTime before = new LocalDateTime(2010, 4, 4, 1, 30);
            LocalDateTime atTransition = new LocalDateTime(2010, 4, 4, 2, 0);
            LocalDateTime ambiguous = new LocalDateTime(2010, 4, 4, 2, 30);
            LocalDateTime after = new LocalDateTime(2010, 4, 4, 3, 30);
            AssertOffset(+13, before, NewZealand);
            AssertOffset(+12, atTransition, NewZealand);
            AssertOffset(+12, ambiguous, NewZealand);
            AssertOffset(+12, after, NewZealand);
        }

        // New Zealand goes from +12 to +13 on September 26th 2010 at 2am wall time
        [Test]
        public void GetOffsetFromLocal_NewZealandSpringTransition()
        {
            LocalDateTime before = new LocalDateTime(2010, 9, 26, 1, 30);
            LocalDateTime impossible = new LocalDateTime(2010, 9, 26, 2, 30);
            LocalDateTime atTransition = new LocalDateTime(2010, 9, 26, 3, 0);
            LocalDateTime after = new LocalDateTime(2010, 9, 26, 3, 30);
            AssertOffset(+12, before, NewZealand);
            AssertImpossible(impossible, NewZealand);
            AssertOffset(+13, atTransition, NewZealand);
            AssertOffset(+13, after, NewZealand);
        }

        private static void AssertImpossible(LocalDateTime localTime, IDateTimeZone zone)
        {
            Assert.Throws<SkippedTimeException>(() => zone.GetOffsetFromLocal(localTime.LocalInstant));
        }

        private static void AssertOffset(int expectedHours, LocalDateTime localTime, IDateTimeZone zone)
        {
            Offset offset = zone.GetOffsetFromLocal(localTime.LocalInstant);
            int actualHours = offset.Milliseconds / NodaConstants.MillisecondsPerHour;
            Assert.AreEqual(expectedHours, actualHours);
        }
    }
}
