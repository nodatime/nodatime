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
    /// Fixture for miscellaneous bug reports and oddities which don't really fit anywhere else.
    /// Quite often the cause of a problem is nowhere near the test code; it's still useful
    /// to have the original test which showed up the problem, as a small contribution
    /// to regression testing.
    /// </summary>
    [TestFixture]
    public class MiscellaneousBugReports
    {
        [Test]
        public void Niue()
        {
            DateTimeZone niue = DateTimeZones.ForId("Pacific/Niue");
            var offset = niue.GetOffsetFromUtc(new ZonedDateTime(2010, 1, 1, 0, 0, 0, niue).ToInstant());
            Assert.AreEqual(Offset.ForHours(-11), offset);
        }

        [Test]
        public void Kiritimati()
        {
            DateTimeZone kiritimati = DateTimeZones.ForId("Pacific/Kiritimati");
            var offset = kiritimati.GetOffsetFromUtc(new ZonedDateTime(2010, 1, 1, 0, 0, 0, kiritimati).ToInstant());
            Assert.AreEqual(Offset.ForHours(14), offset);
        }

        [Test]
        public void Pyongyang()
        {
            DateTimeZone pyongyang = DateTimeZones.ForId("Asia/Pyongyang");
            var offset = pyongyang.GetOffsetFromUtc(new ZonedDateTime(2010, 1, 1, 0, 0, 0, pyongyang).ToInstant());
            Assert.AreEqual(Offset.ForHours(9), offset);
        }
    }
}