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

using NodaTime.Fields;
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Periods
{
    public partial class PeriodTest
    {


        [Test]
        public void ConstructorDuration_SplitIntoFields_Precize()
        {
            var length = new Duration(5 * NodaConstants.TicksPerHour +
                                        6 * NodaConstants.TicksPerMinute +
                                        7 * NodaConstants.TicksPerSecond + 
                                        8 *NodaConstants.TicksPerMillisecond);
            var sut = Period.From(length);
            Assert.AreEqual(0, sut.Years);
            Assert.AreEqual(0, sut.Months);
            Assert.AreEqual(0, sut.Days);
            Assert.AreEqual(5, sut.Hours);
            Assert.AreEqual(6, sut.Minutes);
            Assert.AreEqual(7, sut.Seconds);
            Assert.AreEqual(8, sut.Milliseconds);
        }

        [Test]
        [Ignore("bug")]
        public void ConstructorDuration_SplitIntoFields_Unprecise()
        {
            var length = new Duration(4 * NodaConstants.TicksPerDay + 
                                        5 * NodaConstants.TicksPerHour +
                                        6 * NodaConstants.TicksPerMinute +
                                        7 * NodaConstants.TicksPerSecond +
                                        8 * NodaConstants.TicksPerMillisecond);
            var sut = Period.From(length);
            Assert.AreEqual(0, sut.Years);
            Assert.AreEqual(0, sut.Months);
            Assert.AreEqual(0, sut.Days);
            Assert.AreEqual(4 * 24 + 5, sut.Hours);
            Assert.AreEqual(6, sut.Minutes);
            Assert.AreEqual(7, sut.Seconds);
            Assert.AreEqual(8, sut.Milliseconds);
        }

        [Test]
        public void ConstructorDuration_InitPeriodTypeToStandard()
        {
            var sut = Period.From(new Duration(0));
            Assert.AreEqual(PeriodType.Standard, sut.PeriodType);
        }

    }
}
