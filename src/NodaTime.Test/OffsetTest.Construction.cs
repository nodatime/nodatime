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
using NUnit.Framework;

namespace NodaTime.Test
{
    partial class OffsetTest
    {
        [Test]
        public void Zero()
        {
            Offset test = Offset.Zero;
            Assert.AreEqual(0, test.Milliseconds);
        }

        [Test]
        public void FromMillis_Valid()
        {
            var test = Offset.FromMilliseconds(12345);
            Assert.AreEqual(12345, test.Milliseconds);
        }

        [Test]
        public void FromMilliseconds_Invalid()
        {
            int millis = 24 * NodaConstants.MillisecondsPerHour;
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromMilliseconds(millis));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromMilliseconds(-millis));
        }

        [Test]
        public void FromTicks_Valid()
        {
            Offset value = Offset.FromTicks(-15 * NodaConstants.TicksPerMinute);
            Assert.AreEqual(-15 * NodaConstants.MillisecondsPerMinute, value.Milliseconds);
        }
        
        [Test]
        public void FromTicks_Invalid()
        {
            long ticks = 24 * NodaConstants.TicksPerHour;
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromTicks(ticks));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromTicks(-ticks));
        }

        [Test]
        public void FromHours_Valid()
        {
            Offset value = Offset.FromHours(-15);
            Assert.AreEqual(-15 * NodaConstants.MillisecondsPerHour, value.Milliseconds);
        }

        [Test]
        public void FromHours_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromHours(24));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromHours(-24));
        }

        [Test]
        public void FromHoursAndMinutes_Valid()
        {
            Offset value = Offset.FromHoursAndMinutes(5, 30);
            Assert.AreEqual(5 * NodaConstants.MillisecondsPerHour + 30 * NodaConstants.MillisecondsPerMinute, value.Milliseconds);
        }
    }
}
