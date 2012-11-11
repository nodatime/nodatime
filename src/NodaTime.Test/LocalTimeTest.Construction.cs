#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
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
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalTimeTest
    {
        [Test]
        public void FromTicksSinceMidnight_Valid()
        {
            Assert.AreEqual(LocalTime.Midnight, LocalTime.FromTicksSinceMidnight(0));
            Assert.AreEqual(LocalTime.Midnight - Period.FromTicks(1), LocalTime.FromTicksSinceMidnight(NodaConstants.TicksPerStandardDay - 1));
        }

        [Test]
        public void FromTicksSinceMidnight_RangeChecks()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromTicksSinceMidnight(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromTicksSinceMidnight(NodaConstants.TicksPerStandardDay));
        }

        [Test]
        public void FromMillisecondsSinceMidnight_Valid()
        {
            Assert.AreEqual(LocalTime.Midnight, LocalTime.FromMillisecondsSinceMidnight(0));
            Assert.AreEqual(LocalTime.Midnight - Period.FromMillseconds(1), LocalTime.FromMillisecondsSinceMidnight(NodaConstants.MillisecondsPerStandardDay - 1));
        }

        [Test]
        public void FromMillisecondsSinceMidnight_RangeChecks()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromMillisecondsSinceMidnight(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromMillisecondsSinceMidnight(NodaConstants.MillisecondsPerStandardDay));
        }

        [Test]
        public void FromSecondsSinceMidnight_Valid()
        {
            Assert.AreEqual(LocalTime.Midnight, LocalTime.FromSecondsSinceMidnight(0));
            Assert.AreEqual(LocalTime.Midnight - Period.FromSeconds(1), LocalTime.FromSecondsSinceMidnight(NodaConstants.SecondsPerStandardDay - 1));
        }

        [Test]
        public void FromSecondsSinceMidnight_RangeChecks()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromSecondsSinceMidnight(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromSecondsSinceMidnight(NodaConstants.SecondsPerStandardDay));
        }
    }
}
