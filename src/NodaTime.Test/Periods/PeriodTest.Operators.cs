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
    public partial class PeriodTest
    {
        [Test]
        [Ignore("IsoCalendar is not implemented yet")]
        public void OperatorPlus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(Period.Zero, (Period.Zero + Period.Zero), "0 + 0");
            Assert.AreEqual(Period.FromWeeks(5), (Period.FromWeeks(5) + Period.Zero), "5 weeks + 0");
            Assert.AreEqual(Period.FromWeeks(5), (Period.Zero + Period.FromWeeks(5)), "0 + 5 weeks");
        }

        [Test]
        public void OperatorPlus_Nulls_ChangesNothing()
        {
            Assert.IsNull((Period)null + null, "null + null");

            Assert.AreEqual(Period.FromYears(3), (Period.FromYears(3) + null), "3 years + null");
            Assert.AreEqual(Period.FromYears(3), (null + Period.FromYears(3)), "null + 3 years");
        }

        [Test]
        public void OperatorPlus_NonZero()
        {
            Assert.AreEqual(new Period(2,10,0,0), Period.FromMinutes(10) + Period.FromHours(2), "10 minutes + 1 hour");
        }
    }
}
