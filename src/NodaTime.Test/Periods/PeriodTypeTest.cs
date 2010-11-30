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

using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Periods
{
    [TestFixture]
    public partial class PeriodTypeTest
    {
        [Test]
        public void HasDateFields_Various()
        {
            Assert.IsTrue(PeriodType.AllFields.HasDateFields);
            Assert.IsTrue(PeriodType.YearDay.HasDateFields);
            Assert.IsTrue(PeriodType.YearDay.WithDaysRemoved().HasDateFields);
            Assert.IsTrue(PeriodType.YearDayTime.HasDateFields);
            Assert.IsTrue(PeriodType.Years.HasDateFields);

            Assert.IsFalse(PeriodType.Hours.HasDateFields);
            Assert.IsFalse(PeriodType.Time.HasDateFields);
            Assert.IsFalse(PeriodType.Time.WithMinutesRemoved().HasDateFields);
            Assert.IsFalse(PeriodType.YearDayTime.WithYearsRemoved().WithDaysRemoved().HasDateFields);
        }

        [Test]
        public void HasTimeFields_Various()
        {
            Assert.IsTrue(PeriodType.AllFields.HasTimeFields);
            Assert.IsTrue(PeriodType.YearDayTime.HasTimeFields);
            Assert.IsTrue(PeriodType.Hours.HasTimeFields);
            Assert.IsTrue(PeriodType.Time.HasTimeFields);
            Assert.IsTrue(PeriodType.Time.WithMinutesRemoved().HasTimeFields);
            Assert.IsTrue(PeriodType.YearDayTime.WithYearsRemoved().WithDaysRemoved().HasTimeFields);

            Assert.IsFalse(PeriodType.Years.HasTimeFields);
            Assert.IsFalse(PeriodType.YearDay.HasTimeFields);
            Assert.IsFalse(PeriodType.YearDay.WithDaysRemoved().HasTimeFields);
        }
    }
}