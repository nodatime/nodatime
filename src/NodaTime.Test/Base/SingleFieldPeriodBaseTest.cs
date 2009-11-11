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

using NUnit.Framework;

namespace NodaTime.Test.Base
{
    [TestFixture]
    public partial class SingleFieldPeriodBaseTest
    {
        // test in 2002/03 as time zones are more well known
        // (before the late 90's they were all over the place)
        private static readonly DateTimeZone Paris = DateTimeZone.ForID("Europe/Paris");

        #region Note: was testFactory_between_RInstant

        [Test]
        public void Between_StaticIInstant()
        {
            DateTime start = new DateTime(2006, 6, 9, 12, 0, 0, 0, Paris);
            DateTime end1 = new DateTime(2006, 6, 12, 12, 0, 0, 0, Paris);
            DateTime end2 = new DateTime(2006, 6, 15, 18, 0, 0, 0, Paris);

            Assert.AreEqual(3, Single.SBetween(start, end1, DurationFieldType.Days));
            Assert.AreEqual(0, Single.SBetween(start, start, DurationFieldType.Days));
            Assert.AreEqual(0, Single.SBetween(end1, end1, DurationFieldType.Days));
            Assert.AreEqual(-3, Single.SBetween(end1, start, DurationFieldType.Days));
            Assert.AreEqual(6, Single.SBetween(start, end2, DurationFieldType.Days));
        }

        [Test, ExpectedException("System.ArgumentException")]
        public void Between_StaticIInstant_ThrowsArgumentException1()
        {
            DateTime start = new DateTime(2006, 6, 9, 12, 0, 0, 0, Paris);

            Single.SBetween(start, null, DurationFieldType.Days);
        }

        [Test, ExpectedException("System.ArgumentException")]
        public void Between_StaticIInstant_ThrowsArgumentException2()
        {
            DateTime end1 = new DateTime(2006, 6, 12, 12, 0, 0, 0, Paris);

            Single.SBetween(null, end1, DurationFieldType.Days);
        }

        [Test, ExpectedException("System.ArgumentException")]
        public void Between_StaticIInstant_ThrowsArgumentException3()
        {
            Single.SBetween(null, null, DurationFieldType.Days);
        }

        #endregion

        #region Note: was testFactory_between_RPatial

        [Test]
        public void Between_StaticIPartial()
        {
            LocalDate start = new LocalDate(2006, 6, 9);
            LocalDate end1 = new LocalDate(2006, 6, 12);
            YearMonthDay end2 = new YearMonthDay(2006, 6, 15);
            Single zero = new Single(0);

            Assert.AreEqual(3, Single.SBetween(start, end1, zero));
            Assert.AreEqual(0, Single.SBetween(start, start, zero));
            Assert.AreEqual(0, Single.SBetween(end1, end1, zero));
            Assert.AreEqual(-3, Single.SBetween(end1, start, zero));
            Assert.AreEqual(6, Single.SBetween(start, end2, zero));
        }

        [Test]
        public void Between_StaticIPartial_ThrowsArgumentException1()
        {
            LocalDate start = new LocalDate(2006, 6, 9);
            Single zero = new Single(0);

            Single.SBetween(start, null, zero);
        }

        [Test]
        public void Between_StaticIPartial_ThrowsArgumentException2()
        {
            LocalDate end1 = new LocalDate(2006, 6, 12);
            Single zero = new Single(0);

            Single.SBetween(null, end1, zero);
        }

        [Test]
        public void Between_StaticIPartial_ThrowsArgumentException3()
        {
            Single zero = new Single(0);

            Single.SBetween(null, null, zero);
        }

        [Test]
        public void Between_StaticIPartial_ThrowsArgumentException4()
        {
            LocalDate start = new LocalDate(2006, 6, 9);
            Single zero = new Single(0);

            Single.SBetween(start, new LocalTime(), zero);
        }

        [Test]
        public void Between_StaticIPartial_ThrowsArgumentException5()
        {
            Single zero = new Single(0);

            Single.SBetween(new Partial(DateTimeFieldType.DayOfWeek, 2), new Partial(DateTimeFieldType.DayOfMonth, 3), zero);
        }

        [Test]
        public void Between_StaticIPartial_ThrowsArgumentException6()
        {
            Single zero = new Single(0);
            Partial p = new Partial(
                new DateTimeFieldType[] { DateTimeFieldType.Year, DateTimeFieldType.HourOfDay }, 
                new int[] { 1, 2 });

            Single.SBetween(p, p, zero);
        }

        #endregion
    }
}
