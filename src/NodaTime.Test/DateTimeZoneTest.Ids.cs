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

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Test
{
    /// <summary>
    /// Tests for code in DateTimeZone and code which will be moving out
    /// of DateTimeZones into DateTimeZone over time.
    /// </summary>
    [TestFixture]
    public partial class DateTimeZoneTest
    {
        [Test(Description = "Test for issue 7 in bug tracker")]
        public void IterateOverIds()
        {
            // According to bug, this would go bang
            int count = DateTimeZone.Ids.Count();

            Assert.IsTrue(count > 1);
            int utcCount = DateTimeZone.Ids.Count(id => id == DateTimeZone.UtcId);
            Assert.AreEqual(1, utcCount);
        }

        [Test]
        public void UtcIsNotNull()
        {
            Assert.IsNotNull(DateTimeZone.Utc);
        }

        [Test]
        public void ForId_nullId()
        {
            Assert.Throws<ArgumentNullException>(() => DateTimeZone.ForId(null));
        }

        [Test]
        public void ForId_UtcId()
        {
            Assert.AreEqual(DateTimeZone.Utc, DateTimeZone.ForId(DateTimeZone.UtcId));
        }

        [Test]
        public void ForId_InvalidId()
        {
            Assert.Throws<TimeZoneNotFoundException>(() => DateTimeZone.ForId("not a known id"));
        }

        [Test]
        public void ForId_AmericaLosAngeles()
        {
            const string americaLosAngeles = "America/Los_Angeles";
            var actual = DateTimeZone.ForId(americaLosAngeles);
            Assert.IsNotNull(actual);
            Assert.AreNotEqual(DateTimeZone.Utc, actual);
            Assert.AreEqual(americaLosAngeles, actual.Id);
        }

        [Test]
        public void Ids_All()
        {
            var actual = DateTimeZone.Ids;
            var actualCount = actual.Count();
            Assert.IsTrue(actualCount > 1);
            var utc = actual.Single(id => id == DateTimeZone.UtcId);
            Assert.AreEqual(DateTimeZone.UtcId, utc);
        }

        /// <summary>
        /// Simply tests that every ID in the built-in database can be fetched. This is also
        /// helpful for diagnostic debugging when we want to check that some potential
        /// invariant holds for all time zones...
        /// </summary>
        [Test]
        public void ForId_AllIds()
        {
            foreach (string id in DateTimeZone.Ids)
            {
                Assert.IsNotNull(DateTimeZone.ForId(id));
            }
        }

        [Test]
        public void For_Id_FixedOffset()
        {
            string id = "UTC+05:30";
            DateTimeZone zone = DateTimeZone.ForId(id);
            Assert.AreEqual(DateTimeZone.ForOffset(Offset.FromHoursAndMinutes(5, 30)), zone);
            Assert.AreEqual(id, zone.Id);
        }

        [Test]
        public void For_Id_FixedOffset_NonCanonicalId()
        {
            string id = "UTC+05:00:00";
            DateTimeZone zone = DateTimeZone.ForId(id);
            Assert.AreEqual(zone, DateTimeZone.ForOffset(Offset.FromHours(5)));
            Assert.AreEqual("UTC+05", zone.Id);
        }

        [Test]
        public void For_Id_InvalidFixedOffset()
        {
            Assert.Throws<TimeZoneNotFoundException>(() => DateTimeZone.ForId("UTC+5Months"));
        }
    }
}
