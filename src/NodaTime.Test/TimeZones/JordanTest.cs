#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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

using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// As of 2002, Jordan switches to DST at the *end* of the last Thursday of March.
    /// This is denoted in the zoneinfo database using lastThu 24:00, which was invalid
    /// in our parser.
    /// </summary>
    [TestFixture]
    public class JordanTest
    {
        private static readonly DateTimeZone Jordan = DateTimeZoneProviders.Tzdb["Asia/Amman"];

        /// <summary>
        /// If all of these transitions are right, we're probably okay... in particular,
        /// checking the 2005 transition occurs on the 1st of April is important.
        /// </summary>
        [Test]
        public void Transitions2000To2010()
        {
            // These were fetched with Joda Time 1.6.2, which definitely uses the new rules.
            var expectedDates = new[]
            {
                new LocalDate(2000, 3, 30), // Thursday morning
                new LocalDate(2001, 3, 29), // Thursday morning
                new LocalDate(2002, 3, 29), // Friday morning from here onwards
                new LocalDate(2003, 3, 28),
                new LocalDate(2004, 3, 26),
                new LocalDate(2005, 4, 1),
                new LocalDate(2006, 3, 31),
                new LocalDate(2007, 3, 30),
                new LocalDate(2008, 3, 28),
                new LocalDate(2009, 3, 27),
                new LocalDate(2010, 3, 26)
            };

            for (int year = 2000; year <= 2010; year++)
            {
                LocalDate summer = new LocalDate(year, 6, 1);
                var intervalPair = Jordan.GetZoneIntervals(summer.AtMidnight().LocalInstant);
                Assert.AreEqual(1, intervalPair.MatchingIntervals);
                Assert.AreEqual(expectedDates[year - 2000], intervalPair.EarlyInterval.IsoLocalStart.Date);
            }
        }
    }
}