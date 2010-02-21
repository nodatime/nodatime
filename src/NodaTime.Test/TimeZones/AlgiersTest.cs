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
    /// Algiers had DST until May 1st 1981, after which time it didn't have any - so
    /// we use that to test a time zone whose transitions run out. (When Algiers
    /// decided to stop using DST, it changed its standard offset to be what had previously
    /// been its DST offset, i.e. +1.)
    /// </summary>
    [TestFixture]
    public class AlgiersTest
    {
        private static readonly IDateTimeZone Algiers = DateTimeZones.ForId("Africa/Algiers");

        [Test]
        public void NextTransition_RunsOutOfTransitions()
        {
            Instant april1981 = new ZonedDateTime(1981, 4, 1, 0, 0, 0, DateTimeZones.Utc).ToInstant();
            Transition? lastTransition = Algiers.NextTransition(april1981);
            Assert.IsNotNull(lastTransition);

            Transition expected = new Transition(new ZonedDateTime(1981, 5, 1, 0, 0, 0, DateTimeZones.Utc).ToInstant(),
                Offset.Zero, Offset.ForHours(1));
            Assert.AreEqual(expected, lastTransition.Value);

            Assert.IsNull(Algiers.NextTransition(expected.Instant));
        }
    }
}
