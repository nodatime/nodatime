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

using NodaTime.Fields;
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Periods
{
    public partial class PeriodTypeTest
    {
        [Test]
        public void WithYearsRemoves_MasksYears_InStandardPeriodType()
        {
            var sut = PeriodType.Standard.WithYearsRemoved();

            Assert.AreEqual(7, sut.Size);
            Assert.AreEqual(DurationFieldType.Months, sut.GetFieldType(0));
            Assert.AreEqual(DurationFieldType.Weeks, sut.GetFieldType(1));
            Assert.AreEqual(DurationFieldType.Days, sut.GetFieldType(2));
            Assert.AreEqual(DurationFieldType.Hours, sut.GetFieldType(3));
            Assert.AreEqual(DurationFieldType.Minutes, sut.GetFieldType(4));
            Assert.AreEqual(DurationFieldType.Seconds, sut.GetFieldType(5));
            Assert.AreEqual(DurationFieldType.Milliseconds, sut.GetFieldType(6));
            Assert.AreEqual("StandardNoYears", sut.Name);
            Assert.AreEqual("PeriodType[StandardNoYears]", sut.ToString());
        }

        [Test]
        public void WithMonthsRemoved_MasksMonths_InStandardPeriodType()
        {
            var sut = PeriodType.Standard.WithMonthsRemoved();

            Assert.AreEqual(7, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut.GetFieldType(0));
            Assert.AreEqual(DurationFieldType.Weeks, sut.GetFieldType(1));
            Assert.AreEqual(DurationFieldType.Days, sut.GetFieldType(2));
            Assert.AreEqual(DurationFieldType.Hours, sut.GetFieldType(3));
            Assert.AreEqual(DurationFieldType.Minutes, sut.GetFieldType(4));
            Assert.AreEqual(DurationFieldType.Seconds, sut.GetFieldType(5));
            Assert.AreEqual(DurationFieldType.Milliseconds, sut.GetFieldType(6));
            Assert.AreEqual("StandardNoMonths", sut.Name);
            Assert.AreEqual("PeriodType[StandardNoMonths]", sut.ToString());
        }

        [Test]
        public void WithWeeksRemoved_MasksWeeks_InStandardPeriodType()
        {
            var sut = PeriodType.Standard.WithWeeksRemoved();

            Assert.AreEqual(7, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut.GetFieldType(0));
            Assert.AreEqual(DurationFieldType.Months, sut.GetFieldType(1));
            Assert.AreEqual(DurationFieldType.Days, sut.GetFieldType(2));
            Assert.AreEqual(DurationFieldType.Hours, sut.GetFieldType(3));
            Assert.AreEqual(DurationFieldType.Minutes, sut.GetFieldType(4));
            Assert.AreEqual(DurationFieldType.Seconds, sut.GetFieldType(5));
            Assert.AreEqual(DurationFieldType.Milliseconds, sut.GetFieldType(6));
            Assert.AreEqual("StandardNoWeeks", sut.Name);
            Assert.AreEqual("PeriodType[StandardNoWeeks]", sut.ToString());
        }

        [Test]
        public void WithDaysRemoved_MasksDays_InStandardPeriodType()
        {
            var sut = PeriodType.Standard.WithDaysRemoved();

            Assert.AreEqual(7, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut.GetFieldType(0));
            Assert.AreEqual(DurationFieldType.Months, sut.GetFieldType(1));
            Assert.AreEqual(DurationFieldType.Weeks, sut.GetFieldType(2));
            Assert.AreEqual(DurationFieldType.Hours, sut.GetFieldType(3));
            Assert.AreEqual(DurationFieldType.Minutes, sut.GetFieldType(4));
            Assert.AreEqual(DurationFieldType.Seconds, sut.GetFieldType(5));
            Assert.AreEqual(DurationFieldType.Milliseconds, sut.GetFieldType(6));
            Assert.AreEqual("StandardNoDays", sut.Name);
            Assert.AreEqual("PeriodType[StandardNoDays]", sut.ToString());
        }

        [Test]
        public void WithHoursRemoved_MasksHours_InStandardPeriodType()
        {
            var sut = PeriodType.Standard.WithHoursRemoved();

            Assert.AreEqual(7, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut.GetFieldType(0));
            Assert.AreEqual(DurationFieldType.Months, sut.GetFieldType(1));
            Assert.AreEqual(DurationFieldType.Weeks, sut.GetFieldType(2));
            Assert.AreEqual(DurationFieldType.Days, sut.GetFieldType(3));
            Assert.AreEqual(DurationFieldType.Minutes, sut.GetFieldType(4));
            Assert.AreEqual(DurationFieldType.Seconds, sut.GetFieldType(5));
            Assert.AreEqual(DurationFieldType.Milliseconds, sut.GetFieldType(6));
            Assert.AreEqual("StandardNoHours", sut.Name);
            Assert.AreEqual("PeriodType[StandardNoHours]", sut.ToString());
        }

        [Test]
        public void WithMinutesRemoved_MasksMinutes_InStandardPeriodType()
        {
            var sut = PeriodType.Standard.WithMinutesRemoved();

            Assert.AreEqual(7, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut.GetFieldType(0));
            Assert.AreEqual(DurationFieldType.Months, sut.GetFieldType(1));
            Assert.AreEqual(DurationFieldType.Weeks, sut.GetFieldType(2));
            Assert.AreEqual(DurationFieldType.Days, sut.GetFieldType(3));
            Assert.AreEqual(DurationFieldType.Hours, sut.GetFieldType(4));
            Assert.AreEqual(DurationFieldType.Seconds, sut.GetFieldType(5));
            Assert.AreEqual(DurationFieldType.Milliseconds, sut.GetFieldType(6));
            Assert.AreEqual("StandardNoMinutes", sut.Name);
            Assert.AreEqual("PeriodType[StandardNoMinutes]", sut.ToString());
        }

        [Test]
        public void WithSecondsRemoved_MasksSeconds_InStandardPeriodType()
        {
            var sut = PeriodType.Standard.WithSecondsRemoved();

            Assert.AreEqual(7, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut.GetFieldType(0));
            Assert.AreEqual(DurationFieldType.Months, sut.GetFieldType(1));
            Assert.AreEqual(DurationFieldType.Weeks, sut.GetFieldType(2));
            Assert.AreEqual(DurationFieldType.Days, sut.GetFieldType(3));
            Assert.AreEqual(DurationFieldType.Hours, sut.GetFieldType(4));
            Assert.AreEqual(DurationFieldType.Minutes, sut.GetFieldType(5));
            Assert.AreEqual(DurationFieldType.Milliseconds, sut.GetFieldType(6));
            Assert.AreEqual("StandardNoSeconds", sut.Name);
            Assert.AreEqual("PeriodType[StandardNoSeconds]", sut.ToString());
        }

        [Test]
        public void WithMillisecondsRemoved_MasksMilliseconds_InStandardPeriodType()
        {
            var sut = PeriodType.Standard.WithMillisecondsRemoved();

            Assert.AreEqual(7, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut.GetFieldType(0));
            Assert.AreEqual(DurationFieldType.Months, sut.GetFieldType(1));
            Assert.AreEqual(DurationFieldType.Weeks, sut.GetFieldType(2));
            Assert.AreEqual(DurationFieldType.Days, sut.GetFieldType(3));
            Assert.AreEqual(DurationFieldType.Hours, sut.GetFieldType(4));
            Assert.AreEqual(DurationFieldType.Minutes, sut.GetFieldType(5));
            Assert.AreEqual(DurationFieldType.Seconds, sut.GetFieldType(6));
            Assert.AreEqual("StandardNoMilliseconds", sut.Name);
            Assert.AreEqual("PeriodType[StandardNoMilliseconds]", sut.ToString());
        }

        [Test]
        public void WithHoursMinutesSeconsRemoved_MasksHoursMinutesSeconds_InStandardPeriodType()
        {
            var sut = PeriodType.Standard
                                    .WithHoursRemoved()
                                    .WithMinutesRemoved()
                                    .WithSecondsRemoved();

            Assert.AreEqual(5, sut.Size);
            Assert.AreEqual(DurationFieldType.Years, sut.GetFieldType(0));
            Assert.AreEqual(DurationFieldType.Months, sut.GetFieldType(1));
            Assert.AreEqual(DurationFieldType.Weeks, sut.GetFieldType(2));
            Assert.AreEqual(DurationFieldType.Days, sut.GetFieldType(3));
            Assert.AreEqual(DurationFieldType.Milliseconds, sut.GetFieldType(4));
            Assert.AreEqual("StandardNoHoursNoMinutesNoSeconds", sut.Name);
            Assert.AreEqual("PeriodType[StandardNoHoursNoMinutesNoSeconds]", sut.ToString());
        }
    }
}
