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
        public void Years_ContainsValidFields()
        {
            var sut = PeriodType.Years;

            Assert.AreEqual(1, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut[0]);
            Assert.AreEqual("Years", sut.Name);
            Assert.AreEqual("PeriodType[Years]", sut.ToString());
        }

        [Test]
        public void Months_ContainsValidFields()
        {
            var sut = PeriodType.Months;

            Assert.AreEqual(1, sut.Size);
            Assert.AreEqual(DurationFieldType.Months, sut[0]);
            Assert.AreEqual("Months", sut.Name);
            Assert.AreEqual("PeriodType[Months]", sut.ToString());
        }

        [Test]
        public void Weeks_ContainsValidFields()
        {
            var sut = PeriodType.Weeks;

            Assert.AreEqual(1, sut.Size);
            Assert.AreEqual(DurationFieldType.Weeks, sut[0]);
            Assert.AreEqual("Weeks", sut.Name);
            Assert.AreEqual("PeriodType[Weeks]", sut.ToString());
        }

        [Test]
        public void Days_ContainsValidFields()
        {
            var sut = PeriodType.Days;

            Assert.AreEqual(1, sut.Size);
            Assert.AreEqual(DurationFieldType.Days, sut[0]);
            Assert.AreEqual("Days", sut.Name);
            Assert.AreEqual("PeriodType[Days]", sut.ToString());
        }

        [Test]
        public void Hours_ContainsValidFields()
        {
            var sut = PeriodType.Hours;

            Assert.AreEqual(1, sut.Size);
            Assert.AreEqual(DurationFieldType.Hours, sut[0]);
            Assert.AreEqual("Hours", sut.Name);
            Assert.AreEqual("PeriodType[Hours]", sut.ToString());
        }

        [Test]
        public void Minutes_ContainsValidFields()
        {
            var sut = PeriodType.Minutes;

            Assert.AreEqual(1, sut.Size);
            Assert.AreEqual(DurationFieldType.Minutes, sut[0]);
            Assert.AreEqual("Minutes", sut.Name);
            Assert.AreEqual("PeriodType[Minutes]", sut.ToString());
        }

        [Test]
        public void Seconds_ContainsValidFields()
        {
            var sut = PeriodType.Seconds;

            Assert.AreEqual(1, sut.Size);
            Assert.AreEqual(DurationFieldType.Seconds, sut[0]);
            Assert.AreEqual("Seconds", sut.Name);
            Assert.AreEqual("PeriodType[Seconds]", sut.ToString());
        }

        [Test]
        public void Milliseconds_ContainsValidFields()
        {
            var sut = PeriodType.Milliseconds;

            Assert.AreEqual(1, sut.Size);
            Assert.AreEqual(DurationFieldType.Milliseconds, sut[0]);
            Assert.AreEqual("Milliseconds", sut.Name);
            Assert.AreEqual("PeriodType[Milliseconds]", sut.ToString());
        }

        [Test]
        public void Ticks_ContainsValidFields()
        {
            var sut = PeriodType.Ticks;

            Assert.AreEqual(1, sut.Size);
            Assert.AreEqual(DurationFieldType.Ticks, sut[0]);
            Assert.AreEqual("Ticks", sut.Name);
            Assert.AreEqual("PeriodType[Ticks]", sut.ToString());
        }

        [Test]
        public void AllFields_ContainsValidFields()
        {
            var sut = PeriodType.AllFields;

            Assert.AreEqual(9, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut[0]);
            Assert.AreEqual(DurationFieldType.Months, sut[1]);
            Assert.AreEqual(DurationFieldType.Weeks, sut[2]);
            Assert.AreEqual(DurationFieldType.Days, sut[3]);
            Assert.AreEqual(DurationFieldType.Hours, sut[4]);
            Assert.AreEqual(DurationFieldType.Minutes, sut[5]);
            Assert.AreEqual(DurationFieldType.Seconds, sut[6]);
            Assert.AreEqual(DurationFieldType.Milliseconds, sut[7]);
            Assert.AreEqual(DurationFieldType.Ticks, sut[8]);
            Assert.AreEqual("All", sut.Name);
            Assert.AreEqual("PeriodType[All]", sut.ToString());
        }

        [Test]
        public void YearMonthDayTime_ContainsValidFields()
        {
            var sut = PeriodType.YearMonthDayTime;

            Assert.AreEqual(8, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut[0]);
            Assert.AreEqual(DurationFieldType.Months, sut[1]);
            Assert.AreEqual(DurationFieldType.Days, sut[2]);
            Assert.AreEqual(DurationFieldType.Hours, sut[3]);
            Assert.AreEqual(DurationFieldType.Minutes, sut[4]);
            Assert.AreEqual(DurationFieldType.Seconds, sut[5]);
            Assert.AreEqual(DurationFieldType.Milliseconds, sut[6]);
            Assert.AreEqual(DurationFieldType.Ticks, sut[7]);
            Assert.AreEqual("YearMonthDayTime", sut.Name);
            Assert.AreEqual("PeriodType[YearMonthDayTime]", sut.ToString());
        }

        [Test]
        public void YearMonthDay_ContainsValidFields()
        {
            var sut = PeriodType.YearMonthDay;

            Assert.AreEqual(3, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut[0]);
            Assert.AreEqual(DurationFieldType.Months, sut[1]);
            Assert.AreEqual(DurationFieldType.Days, sut[2]);
            Assert.AreEqual("YearMonthDay", sut.Name);
            Assert.AreEqual("PeriodType[YearMonthDay]", sut.ToString());
        }

        [Test]
        public void YearWeekDateTime_ContainsValidFields()
        {
            var sut = PeriodType.YearWeekDayTime;

            Assert.AreEqual(8, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut[0]);
            Assert.AreEqual(DurationFieldType.Weeks, sut[1]);
            Assert.AreEqual(DurationFieldType.Days, sut[2]);
            Assert.AreEqual(DurationFieldType.Hours, sut[3]);
            Assert.AreEqual(DurationFieldType.Minutes, sut[4]);
            Assert.AreEqual(DurationFieldType.Seconds, sut[5]);
            Assert.AreEqual(DurationFieldType.Milliseconds, sut[6]);
            Assert.AreEqual(DurationFieldType.Ticks, sut[7]);
            Assert.AreEqual("YearWeekDayTime", sut.Name);
            Assert.AreEqual("PeriodType[YearWeekDayTime]", sut.ToString());
        }

        [Test]
        public void YearWeekDay_ContainsValidFields()
        {
            var sut = PeriodType.YearWeekDay;

            Assert.AreEqual(3, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut[0]);
            Assert.AreEqual(DurationFieldType.Weeks, sut[1]);
            Assert.AreEqual(DurationFieldType.Days, sut[2]);
            Assert.AreEqual("YearWeekDay", sut.Name);
            Assert.AreEqual("PeriodType[YearWeekDay]", sut.ToString());
        }

        [Test]
        public void YearDayTime_ContainsValidFields()
        {
            var sut = PeriodType.YearDayTime;

            Assert.AreEqual(7, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut[0]);
            Assert.AreEqual(DurationFieldType.Days, sut[1]);
            Assert.AreEqual(DurationFieldType.Hours, sut[2]);
            Assert.AreEqual(DurationFieldType.Minutes, sut[3]);
            Assert.AreEqual(DurationFieldType.Seconds, sut[4]);
            Assert.AreEqual(DurationFieldType.Milliseconds, sut[5]);
            Assert.AreEqual(DurationFieldType.Ticks, sut[6]);
            Assert.AreEqual("YearDayTime", sut.Name);
            Assert.AreEqual("PeriodType[YearDayTime]", sut.ToString());
        }

        [Test]
        public void YearDay_ContainsValidFields()
        {
            var sut = PeriodType.YearDay;

            Assert.AreEqual(2, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut[0]);
            Assert.AreEqual(DurationFieldType.Days, sut[1]);
            Assert.AreEqual("YearDay", sut.Name);
            Assert.AreEqual("PeriodType[YearDay]", sut.ToString());
        }

        [Test]
        public void DayTime_ContainsValidFields()
        {
            var sut = PeriodType.DayTime;

            Assert.AreEqual(6, sut.Size);
            Assert.AreEqual(DurationFieldType.Days, sut[0]);
            Assert.AreEqual(DurationFieldType.Hours, sut[1]);
            Assert.AreEqual(DurationFieldType.Minutes, sut[2]);
            Assert.AreEqual(DurationFieldType.Seconds, sut[3]);
            Assert.AreEqual(DurationFieldType.Milliseconds, sut[4]);
            Assert.AreEqual(DurationFieldType.Ticks, sut[5]);
            Assert.AreEqual("DayTime", sut.Name);
            Assert.AreEqual("PeriodType[DayTime]", sut.ToString());
        }

        [Test]
        public void Time_ContainsValidFields()
        {
            var sut = PeriodType.Time;

            Assert.AreEqual(5, sut.Size);
            Assert.AreEqual(DurationFieldType.Hours, sut[0]);
            Assert.AreEqual(DurationFieldType.Minutes, sut[1]);
            Assert.AreEqual(DurationFieldType.Seconds, sut[2]);
            Assert.AreEqual(DurationFieldType.Milliseconds, sut[3]);
            Assert.AreEqual(DurationFieldType.Ticks, sut[4]);
            Assert.AreEqual("Time", sut.Name);
            Assert.AreEqual("PeriodType[Time]", sut.ToString());
        }
    }
}