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

namespace NodaTime.Demo
{
    [TestFixture]
    internal class ZonedDateTimeDemo
    {
        private static readonly DateTimeZone Dublin = DateTimeZone.ForId("Europe/Dublin");

        [Test]
        public void Construction()
        {
            ZonedDateTime dt = Dublin.AtStrictly(new LocalDateTime(2010, 6, 9, 15, 15, 0));

            Assert.AreEqual(15, dt.Hour);
            Assert.AreEqual(2010, dt.Year);
            // Not 21... we're not in the Gregorian calendar!
            Assert.AreEqual(20, dt.CenturyOfEra);

            Instant instant = Instant.FromUtc(2010, 6, 9, 14, 15, 0);
            Assert.AreEqual(instant, dt.ToInstant());
        }

        [Test]
        public void Ambiguity()
        {
            Assert.Throws<AmbiguousTimeException>(() => Dublin.AtStrictly(new LocalDateTime(2010, 10, 31, 1, 15, 0)));
        }

        [Test]
        public void Impossibility()
        {
            Assert.Throws<SkippedTimeException>(() => Dublin.AtStrictly(new LocalDateTime(2010, 3, 28, 1, 15, 0)));
        }
    }
}