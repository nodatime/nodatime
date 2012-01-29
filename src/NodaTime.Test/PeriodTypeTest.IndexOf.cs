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
using NodaTime.Fields;

namespace NodaTime.Test
{
    public partial class PeriodTypeTest
    {
        [Test]
        public void IndexOf_ReturnsCorrectIndex_ForDayTime()
        {
            var sut = PeriodType.DayTime;

            Assert.AreEqual(-1, sut.IndexOf(PeriodFieldType.Eras));
            Assert.AreEqual(-1, sut.IndexOf(PeriodFieldType.Centuries));
            Assert.AreEqual(-1, sut.IndexOf(PeriodFieldType.WeekYears));
            Assert.AreEqual(-1, sut.IndexOf(PeriodFieldType.Years));
            Assert.AreEqual(-1, sut.IndexOf(PeriodFieldType.Months));
            Assert.AreEqual(-1, sut.IndexOf(PeriodFieldType.Weeks));
            Assert.AreEqual(0, sut.IndexOf(PeriodFieldType.Days));
            Assert.AreEqual(-1, sut.IndexOf(PeriodFieldType.HalfDays));
            Assert.AreEqual(1, sut.IndexOf(PeriodFieldType.Hours));
            Assert.AreEqual(2, sut.IndexOf(PeriodFieldType.Minutes));
            Assert.AreEqual(3, sut.IndexOf(PeriodFieldType.Seconds));
            Assert.AreEqual(4, sut.IndexOf(PeriodFieldType.Milliseconds));
            Assert.AreEqual(5, sut.IndexOf(PeriodFieldType.Ticks));
        }
    }
}