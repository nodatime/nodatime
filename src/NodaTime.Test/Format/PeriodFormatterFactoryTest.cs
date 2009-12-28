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
using NodaTime.Format;
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    [TestFixture]
    public partial class PeriodFormatterFactoryTest
    {

        [Test]
        public void DefaultFormatter_Print_StandartPeriodWithTimeValues()
        {
            var p = new Period(0, 0, 0, 1, 5, 6, 7, 8);
            Assert.AreEqual("1 day, 5 hours, 6 minutes, 7 seconds and 8 milliseconds", 
                PeriodFormatterFactory.Default.Print(p));
        }

        [Test]
        public void DefaultFormatter_Print_StandartPeriodWithDaysValue()
        {
            var p = Period.FromDays(2);
            Assert.AreEqual("2 days", PeriodFormatterFactory.Default.Print(p));
        }

        [Test]
        public void DefaultFormatter_Print_StandartPeriodWithDaysAndHoursValues()
        {
            var p = Period.FromDays(2).WithHours(5);
            Assert.AreEqual("2 days and 5 hours", PeriodFormatterFactory.Default.Print(p));
        }

        //[Test]
        //public void DefaultFormatter_Parse_StandartPeriodWithDaysValue()
        //{
        //    var p = Period.FromDays(2);
        //    Assert.AreEqual(p, PeriodFormatterFactory.Default.Parse("2 days"));
        //}

        //[Test]
        //public void DefaultFormatter_Parse_StandartPeriodWithDaysAndHoursValues()
        //{
        //    var p = Period.FromDays(2).WithHours(5);
        //    Assert.AreEqual(p, PeriodFormatterFactory.Default.Parse("2 days and 5 hours"));
        //}
    }
}
