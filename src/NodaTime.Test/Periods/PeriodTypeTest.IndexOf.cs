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
using NodaTime.Fields;

namespace NodaTime.Test.Periods
{
    public partial class PeriodTypeTest
    {
        [Test]
        public void IndexOf_ReturnsCorrectIndex_ForDayTime()
        {
            var sut = PeriodType.DayTime;

            Assert.AreEqual(-1, sut.IndexOf(DurationFieldType.Eras));
            Assert.AreEqual(-1, sut.IndexOf(DurationFieldType.Centuries));
            Assert.AreEqual(-1, sut.IndexOf(DurationFieldType.WeekYears));
            Assert.AreEqual(-1, sut.IndexOf(DurationFieldType.Years));
            Assert.AreEqual(-1, sut.IndexOf(DurationFieldType.Months));
            Assert.AreEqual(-1, sut.IndexOf(DurationFieldType.Weeks));
            Assert.AreEqual(0, sut.IndexOf(DurationFieldType.Days));
            Assert.AreEqual(-1, sut.IndexOf(DurationFieldType.HalfDays));
            Assert.AreEqual(1, sut.IndexOf(DurationFieldType.Hours));
            Assert.AreEqual(2, sut.IndexOf(DurationFieldType.Minutes));
            Assert.AreEqual(3, sut.IndexOf(DurationFieldType.Seconds));
            Assert.AreEqual(4, sut.IndexOf(DurationFieldType.Milliseconds));
            Assert.AreEqual(-1, sut.IndexOf(DurationFieldType.Ticks));
        }

    }
}
