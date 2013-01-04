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
using NodaTime;
using NodaTime.TimeZones;
using NodaTime.ZoneInfoCompiler.Tzdb;

namespace ZoneInfoCompiler.Test.Tzdb
{
    /// <summary>
    /// Tests for DateTimeZoneBuilder; currently only scant coverage based on bugs which have
    /// previously been found.
    /// </summary>
    [TestFixture]
    public class DateTimeZoneBuilderTest
    {
        [Test]
        public void FixedZone_Western()
        {
            var offset = Offset.FromHours(-5);
            var builder = new DateTimeZoneBuilder();
            builder.SetStandardOffset(offset);
            builder.SetFixedSavings("GMT+5", Offset.Zero);
            var zone = builder.ToDateTimeZone("GMT+5");
            FixedDateTimeZone fixedZone = (FixedDateTimeZone)zone;
            Assert.AreEqual(offset, fixedZone.Offset);
        }

        [Test]
        public void FixedZone_Eastern()
        {
            var offset = Offset.FromHours(5);
            var builder = new DateTimeZoneBuilder();
            builder.SetStandardOffset(offset);
            builder.SetFixedSavings("GMT-5", Offset.Zero);
            var zone = builder.ToDateTimeZone("GMT-5");
            FixedDateTimeZone fixedZone = (FixedDateTimeZone)zone;
            Assert.AreEqual(offset, fixedZone.Offset);
        }
    }
}
