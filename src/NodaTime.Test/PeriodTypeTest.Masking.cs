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
        public void WithYearsRemoves_MasksYears_InAllFieldsPeriodType()
        {
            var sut = PeriodType.AllFields.WithYearsRemoved();

            Assert.AreEqual(8, sut.Size);
            Assert.AreEqual(PeriodFieldType.Months, sut[0]);
            Assert.AreEqual(PeriodFieldType.Weeks, sut[1]);
            Assert.AreEqual(PeriodFieldType.Days, sut[2]);
            Assert.AreEqual(PeriodFieldType.Hours, sut[3]);
            Assert.AreEqual(PeriodFieldType.Minutes, sut[4]);
            Assert.AreEqual(PeriodFieldType.Seconds, sut[5]);
            Assert.AreEqual(PeriodFieldType.Milliseconds, sut[6]);
            Assert.AreEqual(PeriodFieldType.Ticks, sut[7]);
            Assert.AreEqual("AllNoYears", sut.Name);
            Assert.AreEqual("PeriodType[AllNoYears]", sut.ToString());
        }

        [Test]
        public void WithMonthsRemoved_MasksMonths_InAllFieldsPeriodType()
        {
            var sut = PeriodType.AllFields.WithMonthsRemoved();

            Assert.AreEqual(8, sut.Size);
            Assert.AreEqual(PeriodFieldType.Years, sut[0]);
            Assert.AreEqual(PeriodFieldType.Weeks, sut[1]);
            Assert.AreEqual(PeriodFieldType.Days, sut[2]);
            Assert.AreEqual(PeriodFieldType.Hours, sut[3]);
            Assert.AreEqual(PeriodFieldType.Minutes, sut[4]);
            Assert.AreEqual(PeriodFieldType.Seconds, sut[5]);
            Assert.AreEqual(PeriodFieldType.Milliseconds, sut[6]);
            Assert.AreEqual(PeriodFieldType.Ticks, sut[7]);
            Assert.AreEqual("AllNoMonths", sut.Name);
            Assert.AreEqual("PeriodType[AllNoMonths]", sut.ToString());
        }

        [Test]
        public void WithWeeksRemoved_MasksWeeks_InAllFieldsPeriodType()
        {
            var sut = PeriodType.AllFields.WithWeeksRemoved();

            Assert.AreEqual(8, sut.Size);
            Assert.AreEqual(PeriodFieldType.Years, sut[0]);
            Assert.AreEqual(PeriodFieldType.Months, sut[1]);
            Assert.AreEqual(PeriodFieldType.Days, sut[2]);
            Assert.AreEqual(PeriodFieldType.Hours, sut[3]);
            Assert.AreEqual(PeriodFieldType.Minutes, sut[4]);
            Assert.AreEqual(PeriodFieldType.Seconds, sut[5]);
            Assert.AreEqual(PeriodFieldType.Milliseconds, sut[6]);
            Assert.AreEqual(PeriodFieldType.Ticks, sut[7]);
            Assert.AreEqual("AllNoWeeks", sut.Name);
            Assert.AreEqual("PeriodType[AllNoWeeks]", sut.ToString());
        }

        [Test]
        public void WithDaysRemoved_MasksDays_InAllFieldsPeriodType()
        {
            var sut = PeriodType.AllFields.WithDaysRemoved();

            Assert.AreEqual(8, sut.Size);
            Assert.AreEqual(PeriodFieldType.Years, sut[0]);
            Assert.AreEqual(PeriodFieldType.Months, sut[1]);
            Assert.AreEqual(PeriodFieldType.Weeks, sut[2]);
            Assert.AreEqual(PeriodFieldType.Hours, sut[3]);
            Assert.AreEqual(PeriodFieldType.Minutes, sut[4]);
            Assert.AreEqual(PeriodFieldType.Seconds, sut[5]);
            Assert.AreEqual(PeriodFieldType.Milliseconds, sut[6]);
            Assert.AreEqual(PeriodFieldType.Ticks, sut[7]);
            Assert.AreEqual("AllNoDays", sut.Name);
            Assert.AreEqual("PeriodType[AllNoDays]", sut.ToString());
        }

        [Test]
        public void WithHoursRemoved_MasksHours_InAllFieldsPeriodType()
        {
            var sut = PeriodType.AllFields.WithHoursRemoved();

            Assert.AreEqual(8, sut.Size);
            Assert.AreEqual(PeriodFieldType.Years, sut[0]);
            Assert.AreEqual(PeriodFieldType.Months, sut[1]);
            Assert.AreEqual(PeriodFieldType.Weeks, sut[2]);
            Assert.AreEqual(PeriodFieldType.Days, sut[3]);
            Assert.AreEqual(PeriodFieldType.Minutes, sut[4]);
            Assert.AreEqual(PeriodFieldType.Seconds, sut[5]);
            Assert.AreEqual(PeriodFieldType.Milliseconds, sut[6]);
            Assert.AreEqual(PeriodFieldType.Ticks, sut[7]);
            Assert.AreEqual("AllNoHours", sut.Name);
            Assert.AreEqual("PeriodType[AllNoHours]", sut.ToString());
        }

        [Test]
        public void WithMinutesRemoved_MasksMinutes_InAllFieldsPeriodType()
        {
            var sut = PeriodType.AllFields.WithMinutesRemoved();

            Assert.AreEqual(8, sut.Size);
            Assert.AreEqual(PeriodFieldType.Years, sut[0]);
            Assert.AreEqual(PeriodFieldType.Months, sut[1]);
            Assert.AreEqual(PeriodFieldType.Weeks, sut[2]);
            Assert.AreEqual(PeriodFieldType.Days, sut[3]);
            Assert.AreEqual(PeriodFieldType.Hours, sut[4]);
            Assert.AreEqual(PeriodFieldType.Seconds, sut[5]);
            Assert.AreEqual(PeriodFieldType.Milliseconds, sut[6]);
            Assert.AreEqual(PeriodFieldType.Ticks, sut[7]);
            Assert.AreEqual("AllNoMinutes", sut.Name);
            Assert.AreEqual("PeriodType[AllNoMinutes]", sut.ToString());
        }

        [Test]
        public void WithSecondsRemoved_MasksSeconds_InAllFieldsPeriodType()
        {
            var sut = PeriodType.AllFields.WithSecondsRemoved();

            Assert.AreEqual(8, sut.Size);
            Assert.AreEqual(PeriodFieldType.Years, sut[0]);
            Assert.AreEqual(PeriodFieldType.Months, sut[1]);
            Assert.AreEqual(PeriodFieldType.Weeks, sut[2]);
            Assert.AreEqual(PeriodFieldType.Days, sut[3]);
            Assert.AreEqual(PeriodFieldType.Hours, sut[4]);
            Assert.AreEqual(PeriodFieldType.Minutes, sut[5]);
            Assert.AreEqual(PeriodFieldType.Milliseconds, sut[6]);
            Assert.AreEqual(PeriodFieldType.Ticks, sut[7]);
            Assert.AreEqual("AllNoSeconds", sut.Name);
            Assert.AreEqual("PeriodType[AllNoSeconds]", sut.ToString());
        }

        [Test]
        public void WithMillisecondsRemoved_MasksMilliseconds_InAllFieldsPeriodType()
        {
            var sut = PeriodType.AllFields.WithMillisecondsRemoved();

            Assert.AreEqual(8, sut.Size);
            Assert.AreEqual(PeriodFieldType.Years, sut[0]);
            Assert.AreEqual(PeriodFieldType.Months, sut[1]);
            Assert.AreEqual(PeriodFieldType.Weeks, sut[2]);
            Assert.AreEqual(PeriodFieldType.Days, sut[3]);
            Assert.AreEqual(PeriodFieldType.Hours, sut[4]);
            Assert.AreEqual(PeriodFieldType.Minutes, sut[5]);
            Assert.AreEqual(PeriodFieldType.Seconds, sut[6]);
            Assert.AreEqual(PeriodFieldType.Ticks, sut[7]);
            Assert.AreEqual("AllNoMilliseconds", sut.Name);
            Assert.AreEqual("PeriodType[AllNoMilliseconds]", sut.ToString());
        }

        [Test]
        public void WithHoursMinutesSeconsRemoved_MasksHoursMinutesSeconds_InAllFieldsPeriodType()
        {
            var sut = PeriodType.AllFields.WithHoursRemoved().WithMinutesRemoved().WithSecondsRemoved();

            Assert.AreEqual(6, sut.Size);
            Assert.AreEqual(PeriodFieldType.Years, sut[0]);
            Assert.AreEqual(PeriodFieldType.Months, sut[1]);
            Assert.AreEqual(PeriodFieldType.Weeks, sut[2]);
            Assert.AreEqual(PeriodFieldType.Days, sut[3]);
            Assert.AreEqual(PeriodFieldType.Milliseconds, sut[4]);
            Assert.AreEqual(PeriodFieldType.Ticks, sut[5]);
            Assert.AreEqual("AllNoHoursNoMinutesNoSeconds", sut.Name);
            Assert.AreEqual("PeriodType[AllNoHoursNoMinutesNoSeconds]", sut.ToString());
        }
    }
}