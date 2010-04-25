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
        public void Zero_InitZeroValues()
        {
            var sut = Period.Zero;
            Assert.AreEqual(0, sut.Years);
            Assert.AreEqual(0, sut.Months);
            Assert.AreEqual(0, sut.Days);
            Assert.AreEqual(0, sut.Hours);
            Assert.AreEqual(0, sut.Minutes);
            Assert.AreEqual(0, sut.Seconds);
            Assert.AreEqual(0, sut.Milliseconds);
        }

        [Test]
        public void Years_ConstructsCorrectFields()
        {
            var sut = Period.FromYears(6);
            Assert.AreEqual(PeriodType.Standard, sut.PeriodType);
            Assert.AreEqual(6, sut.Years);
            Assert.AreEqual(0, sut.Months);
            Assert.AreEqual(0, sut.Weeks);
            Assert.AreEqual(0, sut.Days);
            Assert.AreEqual(0, sut.Hours);
            Assert.AreEqual(0, sut.Minutes);
            Assert.AreEqual(0, sut.Seconds);
            Assert.AreEqual(0, sut.Milliseconds);
        }

        [Test]
        public void Months_ConstructsCorrectFields()
        {
            var sut = Period.FromMonths(6);
            Assert.AreEqual(PeriodType.Standard, sut.PeriodType);
            Assert.AreEqual(0, sut.Years);
            Assert.AreEqual(6, sut.Months);
            Assert.AreEqual(0, sut.Weeks);
            Assert.AreEqual(0, sut.Days);
            Assert.AreEqual(0, sut.Hours);
            Assert.AreEqual(0, sut.Minutes);
            Assert.AreEqual(0, sut.Seconds);
            Assert.AreEqual(0, sut.Milliseconds);
        }

        [Test]
        public void Weeks_ConstructsCorrectFields()
        {
            var sut = Period.FromWeeks(6);
            Assert.AreEqual(PeriodType.Standard, sut.PeriodType);
            Assert.AreEqual(0, sut.Years);
            Assert.AreEqual(0, sut.Months);
            Assert.AreEqual(6, sut.Weeks);
            Assert.AreEqual(0, sut.Days);
            Assert.AreEqual(0, sut.Hours);
            Assert.AreEqual(0, sut.Minutes);
            Assert.AreEqual(0, sut.Seconds);
            Assert.AreEqual(0, sut.Milliseconds);
        }

        [Test]
        public void Days_ConstructsCorrectFields()
        {
            var sut = Period.FromDays(6);
            Assert.AreEqual(PeriodType.Standard, sut.PeriodType);
            Assert.AreEqual(0, sut.Years);
            Assert.AreEqual(0, sut.Months);
            Assert.AreEqual(0, sut.Weeks);
            Assert.AreEqual(6, sut.Days);
            Assert.AreEqual(0, sut.Hours);
            Assert.AreEqual(0, sut.Minutes);
            Assert.AreEqual(0, sut.Seconds);
            Assert.AreEqual(0, sut.Milliseconds);
        }

        [Test]
        public void Hours_ConstructsCorrectFields()
        {
            var sut = Period.FromHours(6);
            Assert.AreEqual(PeriodType.Standard, sut.PeriodType);
            Assert.AreEqual(0, sut.Years);
            Assert.AreEqual(0, sut.Months);
            Assert.AreEqual(0, sut.Weeks);
            Assert.AreEqual(0, sut.Days);
            Assert.AreEqual(6, sut.Hours);
            Assert.AreEqual(0, sut.Minutes);
            Assert.AreEqual(0, sut.Seconds);
            Assert.AreEqual(0, sut.Milliseconds);
        }

        [Test]
        public void Minutes_ConstructsCorrectFields()
        {
            var sut = Period.FromMinutes(6);
            Assert.AreEqual(PeriodType.Standard, sut.PeriodType);
            Assert.AreEqual(0, sut.Years);
            Assert.AreEqual(0, sut.Months);
            Assert.AreEqual(0, sut.Weeks);
            Assert.AreEqual(0, sut.Days);
            Assert.AreEqual(0, sut.Hours);
            Assert.AreEqual(6, sut.Minutes);
            Assert.AreEqual(0, sut.Seconds);
            Assert.AreEqual(0, sut.Milliseconds);
        }

        [Test]
        public void Seconds_ConstructsCorrectFields()
        {
            var sut = Period.FromSeconds(6);
            Assert.AreEqual(PeriodType.Standard, sut.PeriodType);
            Assert.AreEqual(0, sut.Years);
            Assert.AreEqual(0, sut.Months);
            Assert.AreEqual(0, sut.Weeks);
            Assert.AreEqual(0, sut.Days);
            Assert.AreEqual(0, sut.Hours);
            Assert.AreEqual(0, sut.Minutes);
            Assert.AreEqual(6, sut.Seconds);
            Assert.AreEqual(0, sut.Milliseconds);
        }

        [Test]
        public void Milliseconds_ConstructsCorrectFields()
        {
            var sut = Period.FromMilliseconds(6);
            Assert.AreEqual(PeriodType.Standard, sut.PeriodType);
            Assert.AreEqual(0, sut.Years);
            Assert.AreEqual(0, sut.Months);
            Assert.AreEqual(0, sut.Weeks);
            Assert.AreEqual(0, sut.Days);
            Assert.AreEqual(0, sut.Hours);
            Assert.AreEqual(0, sut.Minutes);
            Assert.AreEqual(0, sut.Seconds);
            Assert.AreEqual(6, sut.Milliseconds);
        }
    }
}
