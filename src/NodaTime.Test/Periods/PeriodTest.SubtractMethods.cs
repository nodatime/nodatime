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

using System;
using NodaTime.Fields;
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Periods
{
    public partial class PeriodTest
    {
        [Test]
        public void SubtractYears_SubtractsValueFromYearsField()
        {
            var sut = Period.FromYears(3).SubtractYears(1);
            Assert.AreEqual(2, sut.Years);
        }

        [Test]
        public void SubtractYears_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromYears(3);
            var second = first.SubtractYears(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void SubtractMonths_SubtractsValueFromMonthsField()
        {
            var sut = Period.FromMonths(3).SubtractMonths(1);
            Assert.AreEqual(2, sut.Months);
        }

        [Test]
        public void SubtractMonths_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromMonths(3);
            var second = first.SubtractMonths(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void SubtractWeeks_SubtractsValueFromWeeksField()
        {
            var sut = Period.FromWeeks(3).SubtractWeeks(1);
            Assert.AreEqual(2, sut.Weeks);
        }

        [Test]
        public void SubtractWeeks_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromWeeks(3);
            var second = first.SubtractWeeks(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void SubtractDays_SubtractsValueFromDaysField()
        {
            var sut = Period.FromDays(3).SubtractDays(1);
            Assert.AreEqual(2, sut.Days);
        }

        [Test]
        public void SubtractDays_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromDays(3);
            var second = first.SubtractDays(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void SubtractHours_SubtractsValueFromHoursField()
        {
            var sut = Period.FromHours(3).SubtractHours(1);
            Assert.AreEqual(2, sut.Hours);
        }

        [Test]
        public void SubtractHours_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromHours(3);
            var second = first.SubtractHours(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void SubtractMinutes_SubtractsValueFromMinutesField()
        {
            var sut = Period.FromMinutes(3).SubtractMinutes(1);
            Assert.AreEqual(2, sut.Minutes);
        }

        [Test]
        public void SubtractMinutes_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromMinutes(3);
            var second = first.SubtractMinutes(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void SubtractSeconds_SubtractsValueFromSecondsField()
        {
            var sut = Period.FromSeconds(3).SubtractSeconds(1);
            Assert.AreEqual(2, sut.Seconds);
        }

        [Test]
        public void SubtractSeconds_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromSeconds(3);
            var second = first.SubtractSeconds(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void SubtractMilliseconds_SubtractsValueFromMillisecondsField()
        {
            var sut = Period.FromMilliseconds(3).SubtractMilliseconds(1);
            Assert.AreEqual(2, sut.Milliseconds);
        }

        [Test]
        public void SubtractMilliseconds_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromMilliseconds(3);
            var second = first.SubtractMilliseconds(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void SubtractMilliseconds_ThrowsUnsupported_OnYearsPeriodType()
        {
            var sut = new Period(6, 0, 0, 0, 0, 0, 0, 0, PeriodType.Years);
            Assert.Throws<NotSupportedException>(() => sut.SubtractMilliseconds(1));
        }

        [Test]
        public void SubtractMilliseconds_AddsZeroValue_OnYearsPeriodType()
        {
            var sut = new Period(6, 0, 0, 0, 0, 0, 0, 0, PeriodType.Years).SubtractMilliseconds(0);
            Assert.AreEqual(0, sut.Milliseconds);
        }

        [Test]
        public void SubtractTime_SubtractsTimesValue_OnStandardPeriodType()
        {
            var first = new Period(1, 2, 3, 4, 5, 6, 7, 8);
            var second = new Period(0, 0, 0, 0, 12, 11, 10, 9, PeriodType.Time);
            var result = first.Subtract(second);

            Assert.AreEqual(1, result.Years);
            Assert.AreEqual(2, result.Months);
            Assert.AreEqual(3, result.Weeks);
            Assert.AreEqual(4, result.Days);
            Assert.AreEqual(-7, result.Hours);
            Assert.AreEqual(-5, result.Minutes);
            Assert.AreEqual(-3, result.Seconds);
            Assert.AreEqual(-1, result.Milliseconds);
        }

        [Test]
        public void SubtractNull_ReturnsTheSameInstance_OnStandardPeriodType()
        {
            var first = new Period(1, 2, 3, 4, 5, 6, 7, 8);
            IPeriod second = null;
            var result = first.Subtract(second);

            Assert.AreEqual(1, result.Years);
            Assert.AreEqual(2, result.Months);
            Assert.AreEqual(3, result.Weeks);
            Assert.AreEqual(4, result.Days);
            Assert.AreEqual(5, result.Hours);
            Assert.AreEqual(6, result.Minutes);
            Assert.AreEqual(7, result.Seconds);
            Assert.AreEqual(8, result.Milliseconds);

            Assert.AreSame(first, result);
        }

        [Test]
        public void SubtractStandard_ThrowsUnsupported_OnTimePeriodType()
        {
            var first = new Period(0, 0, 0, 0, 9, 9, 9, 9, PeriodType.Time);
            var second = new Period(1, 2, 3, 4, 5, 6, 7, 8);

            Assert.Throws<NotSupportedException>(() => first.Subtract(second));
        }

    }
}
