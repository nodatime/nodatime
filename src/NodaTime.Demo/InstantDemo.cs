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

using NUnit.Framework;

namespace NodaTime.Demo
{
    [TestFixture]
    public class InstantDemo
    {

        [Test]
        public void Construction()
        {
            // 10 million ticks = 1 second...
            Instant instant = new Instant(10000000);
            // Epoch is 1970 UTC
            // An instant isn't really "in" a time zone or calendar, but
            // it's convenient to consider UTC in the ISO-8601 calendar.
            Assert.AreEqual("1970-01-01T00:00:01Z", instant.ToString());
        }

        [Test]
        public void ConvenienceConstruction()
        {
            Instant instant = Instant.FromUtc(2010, 6, 9, 14, 15, 0);
            // Here's a number I prepared earlier...
            Assert.AreEqual(12760929000000000, instant.Ticks);
            // But it really is correct
            Assert.AreEqual("2010-06-09T14:15:00Z", instant.ToString());
        }
    }
}
