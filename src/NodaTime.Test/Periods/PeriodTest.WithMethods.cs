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
        public void WithYears_SetsYearsField()
        {
            var sut = Period.FromYears(6).WithYears(1);
            Assert.AreEqual(1, sut.Years);
        }

        [Test]
        public void WithMonths_SetsMonthsField()
        {
            var sut = Period.FromMonths(6).WithMonths(1);
            Assert.AreEqual(1, sut.Months);
        }

        [Test]
        public void WithWeeks_SetsWeeksField()
        {
            var sut = Period.FromWeeks(6).WithWeeks(1);
            Assert.AreEqual(1, sut.Weeks);
        }

        [Test]
        public void WithDays_SetsDaysField()
        {
            var sut = Period.FromDays(6).WithDays(1);
            Assert.AreEqual(1, sut.Days);
        }

        [Test]
        public void WithHours_SetsHoursField()
        {
            var sut = Period.FromHours(6).WithHours(1);
            Assert.AreEqual(1, sut.Hours);
        }

        [Test]
        public void WithMinutes_SetsMinutesField()
        {
            var sut = Period.FromMinutes(6).WithMinutes(1);
            Assert.AreEqual(1, sut.Minutes);
        }

        [Test]
        public void WithSeconds_SetsSecondsField()
        {
            var sut = Period.FromSeconds(6).WithSeconds(1);
            Assert.AreEqual(1, sut.Seconds);
        }

        [Test]
        public void WithMilliseconds_SetsMillisecondsField()
        {
            var sut = Period.FromMilliseconds(6).WithMilliseconds(1);
            Assert.AreEqual(1, sut.Milliseconds);
        }

        [Test]
        public void WithMilliseconds_ThrowsUnsupported_OnYearsPeriodType()
        {
            var sut = new Period(6, 0, 0, 0, 0, 0, 0, 0, PeriodType.Years);
            Assert.Throws<NotSupportedException>(() => sut.WithMilliseconds(1));
        }

        [Test]
        public void WithMilliseconds_SetsZeroValue_OnYearsPeriodType()
        {
            var sut = new Period(6, 0, 0, 0, 0, 0, 0, 0, PeriodType.Years).WithMilliseconds(0);
            Assert.AreEqual(0, sut.Milliseconds);
        }

        [Test]
        public void WithFieldYears_SetsYearsField_OnStandartPeriodType()
        {
            var sut = Period.FromYears(6).WithField(DurationFieldType.Years, 1);
            Assert.AreEqual(1, sut.Years);
        }

        [Test]
        public void WithFieldYears_ThrowsUnsupported_OnTimePeriodType()
        {
            var sut = new Period(0, 0, 0, 0, 5, 6, 7, 8, PeriodType.Time);
            Assert.Throws<NotSupportedException>(() => sut.WithField(DurationFieldType.Years, 1));
        }

        [Test]
        public void WithFieldYears_SetsZeroValue_OnTimePeriodType()
        {
            var sut = new Period(0, 0, 0, 0, 5, 6, 7, 8, PeriodType.Time);
            sut.WithField(DurationFieldType.Years, 0);
            Assert.AreEqual(0, sut.Years);
        }

        [Test]
        public void WithTime_SetsTimesValues_OnStandartPeriodType()
        {
            var first = new Period(1, 2, 3, 4, 5, 6, 7, 8);
            var second = new Period(0, 0, 0, 0, 9, 9, 9, 9, PeriodType.Time);
            var result = first.With(second);

            Assert.AreEqual(1, result.Years);
            Assert.AreEqual(2, result.Months);
            Assert.AreEqual(3, result.Weeks);
            Assert.AreEqual(4, result.Days);
            Assert.AreEqual(9, result.Hours);
            Assert.AreEqual(9, result.Minutes);
            Assert.AreEqual(9, result.Seconds);
            Assert.AreEqual(9, result.Milliseconds);
        }

        [Test]
        public void WithNull_ReturnsTheSameInstance_OnStandartPeriodType()
        {
            var first = new Period(1, 2, 3, 4, 5, 6, 7, 8);
            IPeriod second = null;
            var result = first.With(second);

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
        public void WithStandart_ThrowsUnsupported_OnTimePeriodType()
        {
            var first = new Period(0, 0, 0, 0, 9, 9, 9, 9, PeriodType.Time);
            var second = new Period(1, 2, 3, 4, 5, 6, 7, 8);

            Assert.Throws<NotSupportedException>(() => first.With(second));
        }
    }
}
