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
using System;

namespace NodaTime.Test
{
    partial class OffsetTest
    {
        [Test]
        public void Zero()
        {
            Offset test = Offset.Zero;
            Assert.AreEqual(0, test.TotalMilliseconds);
        }

        [Test]
        public void FromMillis_Valid()
        {
            int length = 5 * NodaConstants.MillisecondsPerHour + 6 * NodaConstants.MillisecondsPerMinute +
                         7 * NodaConstants.MillisecondsPerSecond + 8;
            var test = Offset.FromMilliseconds(length);
            Assert.AreEqual(5, test.Hours);
            Assert.AreEqual(6, test.Minutes);
            Assert.AreEqual(7, test.Seconds);
            Assert.AreEqual(8, test.FractionalSeconds);
            Assert.AreEqual(length, test.TotalMilliseconds);
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
            Assert.AreEqual(-15 * NodaConstants.MillisecondsPerMinute, value.TotalMilliseconds);
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
            Assert.AreEqual(-15 * NodaConstants.MillisecondsPerHour, value.TotalMilliseconds);
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
            Assert.AreEqual(5 * NodaConstants.MillisecondsPerHour + 30 * NodaConstants.MillisecondsPerMinute, value.TotalMilliseconds);
        }

        [Test]
        public void Create_NoSign_Valid()
        {
            Offset value = Offset.Create(5, 4, 3, 200);
            Assert.IsFalse(value.IsNegative);
            Assert.AreEqual(5, value.Hours);
            Assert.AreEqual(4, value.Minutes);
            Assert.AreEqual(3, value.Seconds);
            Assert.AreEqual(200, value.FractionalSeconds);
        }

        [Test]
        public void Create_NoSign_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(-24, 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(24, 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(0, -1, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(0, 60, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(0, 0, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(0, 0, 60, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(0, 0, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(0, 0, 0, 1000));
        }

        [Test]
        public void Create_WithSign_Valid()
        {
            Offset value = Offset.Create(5, 4, 3, 200, false);
            Assert.IsFalse(value.IsNegative);
            Assert.AreEqual(5, value.Hours);
            Assert.AreEqual(4, value.Minutes);
            Assert.AreEqual(3, value.Seconds);
            Assert.AreEqual(200, value.FractionalSeconds);

            Offset value2 = Offset.Create(5, 4, 3, 200, true);
            Assert.IsTrue(value2.IsNegative);
            Assert.AreEqual(5, value2.Hours);
            Assert.AreEqual(4, value2.Minutes);
            Assert.AreEqual(3, value2.Seconds);
            Assert.AreEqual(200, value2.FractionalSeconds);

            Assert.AreEqual(-value.TotalMilliseconds, value2.TotalMilliseconds);
        }

        [Test]
        public void Create_WithSign_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(-24, 0, 0, 0, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(24, 0, 0, 0, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(0, -1, 0, 0, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(0, 60, 0, 0, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(0, 0, -1, 0, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(0, 0, 60, 0, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(0, 0, 0, -1, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.Create(0, 0, 0, 1000, false));
        }

    }
}
