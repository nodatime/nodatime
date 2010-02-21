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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Tests for the Paris time zone. This exercises functionality within various classes.
    /// Paris varies between +1 (standard) and +2 (DST); transitions occur at 2am or 3am wall time,
    /// which is always 1am UTC.
    /// 2009 fall transition: October 25th
    /// 2010 spring transition: March 28th
    /// 2010 fall transition: October 31st
    /// 2011 spring transition: March 27th
    /// </summary>
    [TestFixture]
    public class ParisTest
    {
        // Make sure we deal with the uncached time zone
        private static readonly IDateTimeZone Paris = DateTimeZones.ForId("Europe/Paris").Uncached();

        private static readonly Offset StandardOffset = Offset.ForHours(1);
        private static readonly Offset DaylightOffset = Offset.ForHours(2);

        private static readonly Transition Fall2009Transition = new Transition
            (new ZonedDateTime(2009, 10, 25, 1, 0, 0, DateTimeZones.Utc).ToInstant(), DaylightOffset, StandardOffset);
        private static readonly Transition Spring2010Transition = new Transition
            (new ZonedDateTime(2010, 3, 28, 1, 0, 0, DateTimeZones.Utc).ToInstant(), StandardOffset, DaylightOffset);
        private static readonly Transition Fall2010Transition = new Transition
            (new ZonedDateTime(2010, 10, 31, 1, 0, 0, DateTimeZones.Utc).ToInstant(), DaylightOffset, StandardOffset);
        private static readonly Transition Spring2011Transition = new Transition
            (new ZonedDateTime(2011, 3, 27, 1, 0, 0, DateTimeZones.Utc).ToInstant(), StandardOffset, DaylightOffset);

        private static readonly Instant Summer2010 = new ZonedDateTime(2010, 6, 19, 0, 0, 0, DateTimeZones.Utc).ToInstant();
        private static readonly Instant Winter2010 = new ZonedDateTime(2010, 12, 1, 0, 0, 0, DateTimeZones.Utc).ToInstant();
        // Until 1911, Paris was 9 minutes and 21 seconds off UTC.
        private static readonly Offset InitialOffset = Offset.Create(0, 9, 21);

        [Test]
        public void PreviousTransition_OnTransitionInstant()
        {
            Transition actual = Paris.ValidatePreviousTransition(Spring2010Transition.Instant);
            Assert.AreEqual(Fall2009Transition, actual);
            actual = Paris.PreviousTransition(Fall2010Transition.Instant).Value;
            Assert.AreEqual(Spring2010Transition, actual);
        }

        [Test]
        public void PreviousTransition_NotNearTransition()
        {
            Transition actual = Paris.ValidatePreviousTransition(Summer2010);
            Assert.AreEqual(Spring2010Transition, actual);
            actual = Paris.ValidatePreviousTransition(Winter2010);
            Assert.AreEqual(Fall2010Transition, actual);
        }

        [Test]
        public void NextTransition_OnTransitionInstant()
        {
            Transition actual = Paris.ValidateNextTransition(Spring2010Transition.Instant);
            Assert.AreEqual(Fall2010Transition, actual);
            actual = Paris.ValidateNextTransition(Fall2010Transition.Instant);
            Assert.AreEqual(Spring2011Transition, actual);
        }

        [Test]
        public void NextTransition_NotNearTransition()
        {
            Transition actual = Paris.ValidateNextTransition(Summer2010);
            Assert.AreEqual(Fall2010Transition, actual);
            actual = Paris.ValidateNextTransition(Winter2010);
            Assert.AreEqual(Spring2011Transition, actual);
        }

        [Test]
        public void FirstTransitions()
        {
            // Paris had a name change in 1891, and then moved from +0:09:21 to UTC in 1911
            Instant nameChangeInstant = new ZonedDateTime(1891, 3, 14, 23, 51, 39, DateTimeZones.Utc).ToInstant();
            Instant utcChangeInstant = new ZonedDateTime(1911, 3, 10, 23, 51, 39, DateTimeZones.Utc).ToInstant();

            Transition? first = Paris.ValidateNextTransition(Instant.MinValue);
            Transition expected = new Transition(nameChangeInstant, InitialOffset, InitialOffset);
            Assert.AreEqual(expected, first.Value);
            Assert.IsNull(Paris.PreviousTransition(nameChangeInstant));

            Transition? second = Paris.ValidateNextTransition(nameChangeInstant);
            expected = new Transition(utcChangeInstant, InitialOffset, Offset.Zero);
            Assert.AreEqual(expected, second.Value);
            Assert.AreEqual(first.Value, Paris.PreviousTransition(utcChangeInstant));
        }
    }
}
