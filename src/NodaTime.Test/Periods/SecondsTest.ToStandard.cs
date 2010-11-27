#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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

using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Periods
{
    public partial class SecondsTest
    {
        [Test]
        public void ToStandardWeeks()
        {
            var sut = Seconds.From(2 * 7 * 24 * 60 * 60);
            var expected = Weeks.From(2);
            Assert.That(sut.ToStandardWeeks(), Is.EqualTo(expected));
        }

        [Test]
        public void ToStandardDays()
        {
            var sut = Seconds.From(2 * 24 * 60 * 60);
            var expected = Days.From(2);
            Assert.That(sut.ToStandardDays(), Is.EqualTo(expected));
        }

        [Test]
        public void ToStandardHours()
        {
            var sut = Seconds.From(2 * 60 * 60);
            var expected = Hours.From(2);
            Assert.That(sut.ToStandardHours(), Is.EqualTo(expected));
        }

        [Test]
        public void ToStandardMinutes()
        {
            var sut = Seconds.From(2 * 60);
            var expected = Minutes.From(2);
            Assert.That(sut.ToStandardMinutes(), Is.EqualTo(expected));
        }

        [Test]
        public void ToStandardDuration()
        {
            var sut = Seconds.From(2);
            var expected = new Duration(2 * NodaConstants.MillisecondsPerSecond);
            Assert.That(sut.ToStandardDuration(), Is.EqualTo(expected));
        }
    }
}