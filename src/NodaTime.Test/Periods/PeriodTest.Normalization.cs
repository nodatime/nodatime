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
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Periods
{
    public partial class PeriodTest
    {
        #region ToStandardDuration

        [Test]
        public void ToStandardDuration_ThrowsNotSupprted_IfPeriodContainsYears()
        {
            var sut = Period.FromYears(1).AddDays(2);
            Assert.That(() => sut.ToStandardDuration(), Throws.InstanceOf<NotSupportedException>());
        }

        [Test]
        public void ToStandardDuration_ThrowsNotSupprted_IfPeriodContainsMonths()
        {
            var sut = Period.FromMonths(1).AddDays(2);
            Assert.That(() => sut.ToStandardDuration(), Throws.InstanceOf<NotSupportedException>());
        }

        [Test]
        public void ToStandardDuration_CalculatesDuration()
        {
            var sut = new Period(0, 0, 3, 4, 5, 6, 7, 8);
            var expectedDuration = Duration.FromStandardWeeks(3) 
                + Duration.FromStandardDays(4)
                + Duration.FromHours(5)
                + Duration.FromMinutes(6)
                + Duration.FromSeconds(7)
                + Duration.FromMilliseconds(8);

            Assert.That(sut.ToStandardDuration(), Is.EqualTo(expectedDuration));
        }

        #endregion

        #region ToStandardSeconds

        [Test]
        public void ToStandardSeconds_ThrowsNotSupprted_IfPeriodContainsYears()
        {
            var sut = Period.FromYears(1).AddDays(2);
            Assert.That(() => sut.ToStandardSeconds(), Throws.InstanceOf<NotSupportedException>());
        }

        [Test]
        public void ToStandardSeconds_ThrowsNotSupprted_IfPeriodContainsMonths()
        {
            var sut = Period.FromMonths(1).AddDays(2);
            Assert.That(() => sut.ToStandardSeconds(), Throws.InstanceOf<NotSupportedException>());
        }

        [Test]
        public void ToStandardSeconds_CalculatesSeconds()
        {
            var sut = new Period(0, 0, 3, 4, 5, 6, 7, 8);
            var secondsAmount = 3 * NodaConstants.SecondsPerWeek 
                                + 4 * NodaConstants.SecondsPerDay 
                                + 5 * NodaConstants.SecondsPerHour
                                + 6 * NodaConstants.SecondsPerMinute
                                + 7 
                                + 8 / NodaConstants.MillisecondsPerSecond;

            var seconds = Seconds.From(secondsAmount);

            Assert.That(sut.ToStandardSeconds(), Is.EqualTo(seconds));
        }

        #endregion

        #region ToStandardMinutes

        [Test]
        public void ToStandardMinutes_ThrowsNotSupprted_IfPeriodContainsYears()
        {
            var sut = Period.FromYears(1).AddDays(2);
            Assert.That(() => sut.ToStandardMinutes(), Throws.InstanceOf<NotSupportedException>());
        }

        [Test]
        public void ToStandardMinutes_ThrowsNotSupprted_IfPeriodContainsMonths()
        {
            var sut = Period.FromMonths(1).AddDays(2);
            Assert.That(() => sut.ToStandardMinutes(), Throws.InstanceOf<NotSupportedException>());
        }

        [Test]
        public void ToStandardMinutes_CalculatesMinutes()
        {
            var sut = new Period(0, 0, 3, 4, 5, 6, 7, 8);
            var minutesAmount = 3 * NodaConstants.MinutesPerWeek
                                + 4 * NodaConstants.MinutesPerDay
                                + 5 * NodaConstants.MinutesPerHour
                                + 6 
                                + 7 / NodaConstants.SecondsPerMinute
                                + 8 / NodaConstants.MillisecondsPerMinute;

            var minutes = Minutes.From(minutesAmount);

            Assert.That(sut.ToStandardMinutes(), Is.EqualTo(minutes));
        }

        #endregion

        #region ToStandardHours

        [Test]
        public void ToStandardHours_ThrowsNotSupprted_IfPeriodContainsYears()
        {
            var sut = Period.FromYears(1).AddDays(2);
            Assert.That(() => sut.ToStandardHours(), Throws.InstanceOf<NotSupportedException>());
        }

        [Test]
        public void ToStandardHours_ThrowsNotSupprted_IfPeriodContainsMonths()
        {
            var sut = Period.FromMonths(1).AddDays(2);
            Assert.That(() => sut.ToStandardHours(), Throws.InstanceOf<NotSupportedException>());
        }

        [Test]
        public void ToStandardHours_CalculatesHours()
        {
            var sut = new Period(0, 0, 3, 4, 5, 6, 7, 8);
            var hoursAmount = 3 * NodaConstants.HoursPerWeek
                                + 4 * NodaConstants.HoursPerDay
                                + 5 
                                + 6 / NodaConstants.MinutesPerHour
                                + 7 / NodaConstants.SecondsPerHour
                                + 8 / NodaConstants.MillisecondsPerHour;

            var hours = Hours.From(hoursAmount);

            Assert.That(sut.ToStandardHours(), Is.EqualTo(hours));
        }

        #endregion
    }
}
