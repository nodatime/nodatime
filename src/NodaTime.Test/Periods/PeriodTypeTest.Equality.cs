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
        public void Equals_StandardAndStandardNotTime()
        {
            TestHelper.TestEqualsClass(PeriodType.Standard, PeriodType.Standard, PeriodType.Time);
        }

        [Test]
        public void Equals_YearMonthDayTimeAndStandardWithWeeksRemovedNotYears()
        {
            TestHelper.TestEqualsClass(PeriodType.YearMonthDayTime, PeriodType.Standard.WithWeeksRemoved(), PeriodType.Years);
        }

    }
}
