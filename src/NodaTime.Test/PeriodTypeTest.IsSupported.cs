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
        public void IsSupported_Correct_ForDayTime()
        {
            var sut = PeriodType.DayTime;

            Assert.IsFalse(sut.IsSupported(PeriodFieldType.Eras));
            Assert.IsFalse(sut.IsSupported(PeriodFieldType.Centuries));
            Assert.IsFalse(sut.IsSupported(PeriodFieldType.WeekYears));
            Assert.IsFalse(sut.IsSupported(PeriodFieldType.Years));
            Assert.IsFalse(sut.IsSupported(PeriodFieldType.Months));
            Assert.IsFalse(sut.IsSupported(PeriodFieldType.Weeks));
            Assert.IsTrue(sut.IsSupported(PeriodFieldType.Days));
            Assert.IsFalse(sut.IsSupported(PeriodFieldType.HalfDays));
            Assert.IsTrue(sut.IsSupported(PeriodFieldType.Hours));
            Assert.IsTrue(sut.IsSupported(PeriodFieldType.Minutes));
            Assert.IsTrue(sut.IsSupported(PeriodFieldType.Seconds));
            Assert.IsTrue(sut.IsSupported(PeriodFieldType.Milliseconds));
            Assert.IsTrue(sut.IsSupported(PeriodFieldType.Ticks));
        }
    }
}