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
        public void AddYears_AddsValueToYearsField()
        {
            var sut = Period.FromYears(1).AddYears(1);
            Assert.AreEqual(2, sut.Years);
        }

        [Test]
        public void AddYears_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromYears(1);
            var second = first.AddYears(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void AddMonths_AddsValueToMonthsField()
        {
            var sut = Period.FromMonths(1).AddMonths(1);
            Assert.AreEqual(2, sut.Months);
        }

        [Test]
        public void AddMonths_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromMonths(1);
            var second = first.AddMonths(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void AddWeeks_AddsValueToWeeksField()
        {
            var sut = Period.FromWeeks(1).AddWeeks(1);
            Assert.AreEqual(2, sut.Weeks);
        }

        [Test]
        public void AddWeeks_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromWeeks(1);
            var second = first.AddWeeks(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void AddDays_AddsValueToDaysField()
        {
            var sut = Period.FromDays(1).AddDays(1);
            Assert.AreEqual(2, sut.Days);
        }

        [Test]
        public void AddDayso_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromDays(1);
            var second = first.AddDays(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void AddHours_AddsValueToHoursField()
        {
            var sut = Period.FromHours(1).AddHours(1);
            Assert.AreEqual(2, sut.Hours);
        }

        [Test]
        public void AddHours_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromHours(1);
            var second = first.AddHours(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void AddMinutes_AddsValueToMinutesField()
        {
            var sut = Period.FromMinutes(1).AddMinutes(1);
            Assert.AreEqual(2, sut.Minutes);
        }

        [Test]
        public void AddMinutes_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromMinutes(1);
            var second = first.AddMinutes(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void AddSeconds_AddsValueToSecondsField()
        {
            var sut = Period.FromSeconds(1).AddSeconds(1);
            Assert.AreEqual(2, sut.Seconds);
        }

        [Test]
        public void AddSeconds_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromSeconds(1);
            var second = first.AddSeconds(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void AddMilliseconds_AddsValueToMillisecondsField()
        {
            var sut = Period.FromMilliseconds(1).AddMilliseconds(1);
            Assert.AreEqual(2, sut.Milliseconds);
        }

        [Test]
        public void AddMilliseconds_ReturnsTheSamePeriod_IfZero()
        {
            var first = Period.FromMilliseconds(1);
            var second = first.AddMilliseconds(0);
            Assert.AreSame(first, second);
        }

        [Test]
        public void AddMilliseconds_ThrowsUnsupported_OnYearsPeriodType()
        {
            var sut = new Period(6, 0, 0, 0, 0, 0, 0, 0, PeriodType.Years);
            Assert.Throws<NotSupportedException>(() => sut.AddMilliseconds(1));
        }

        [Test]
        public void AddMilliseconds_AddsZeroValue_OnYearsPeriodType()
        {
            var sut = new Period(6, 0, 0, 0, 0, 0, 0, 0, PeriodType.Years).AddMilliseconds(0);
            Assert.AreEqual(0, sut.Milliseconds);
        }

        [Test]
        public void AddFieldYears_AddsValueToYearsField_OnStandartPeriodType()
        {
            var sut = Period.FromYears(1).AddField(DurationFieldType.Years, 1);
            Assert.AreEqual(2, sut.Years);
        }

        [Test]
        public void AddFieldYears_ThrowsUnsupported_OnTimePeriodType()
        {
            var sut = new Period(0, 0, 0, 0, 5, 6, 7, 8, PeriodType.Time);
            Assert.Throws<NotSupportedException>(() => sut.AddField(DurationFieldType.Years, 1));
        }

        [Test]
        public void AddFieldYears_AddsZeroValue_OnTimePeriodType()
        {
            var sut = new Period(0, 0, 0, 0, 5, 6, 7, 8, PeriodType.Time);
            sut.AddField(DurationFieldType.Years, 0);
            Assert.AreEqual(0, sut.Years);
        }

        [Test]
        public void AddFieldYears_ReturnsTheSamePeriod_OnTimePeriodTypeIfZero()
        {
            var first = new Period(0, 0, 0, 0, 5, 6, 7, 8, PeriodType.Time);
            var second = first.AddField(DurationFieldType.Years, 0);            
            Assert.AreSame(first, second);
        }

        [Test]
        public void AddYears_SetsYearsValue_OnStandartPeriodType()
        {
            var first = new Period(1, 2, 3, 4, 5, 6, 7, 8);
            var second = Period.FromYears(10);
            var result = first.Add(second);

            Assert.AreEqual(11, result.Years);
            Assert.AreEqual(2, result.Months);
            Assert.AreEqual(3, result.Weeks);
            Assert.AreEqual(4, result.Days);
            Assert.AreEqual(5, result.Hours);
            Assert.AreEqual(6, result.Minutes);
            Assert.AreEqual(7, result.Seconds);
            Assert.AreEqual(8, result.Milliseconds);
        }

        [Test]
        public void AddTime_AddsTimesValue_OnStandartPeriodType()
        {
            var first = new Period(1, 2, 3, 4, 5, 6, 7, 8);
            var second = new Period(0, 0, 0, 0, 9, 10, 11, 12, PeriodType.Time);
            var result = first.Add(second);

            Assert.AreEqual(1, result.Years);
            Assert.AreEqual(2, result.Months);
            Assert.AreEqual(3, result.Weeks);
            Assert.AreEqual(4, result.Days);
            Assert.AreEqual(14, result.Hours);
            Assert.AreEqual(16, result.Minutes);
            Assert.AreEqual(18, result.Seconds);
            Assert.AreEqual(20, result.Milliseconds);
        }

        [Test]
        public void AddNull_ReturnsTheSameInstance_OnStandartPeriodType()
        {
            var first = new Period(1, 2, 3, 4, 5, 6, 7, 8);
            IPeriod second = null;
            var result = first.Add(second);

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
        public void AddStandart_ThrowsUnsupported_OnTimePeriodType()
        {
            var first = new Period(0, 0, 0, 0, 9, 9, 9, 9, PeriodType.Time);
            var second = new Period(1, 2, 3, 4, 5, 6, 7, 8);

            Assert.Throws<NotSupportedException>(() => first.Add(second));
        }
    }
}
