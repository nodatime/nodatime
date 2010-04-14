#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
    [TestFixture]
    public partial class PeriodTest
    {
        [Test]
        public void NamedValues_ForYearDayTimePeriod()
        {
            var sut = new Period(1, 0, 0, 4, 5, 6, 7, 8, PeriodType.YearDayTime);
            Assert.AreEqual(1, sut.Years);
            Assert.AreEqual(0, sut.Months);
            Assert.AreEqual(4, sut.Days);
            Assert.AreEqual(5, sut.Hours);
            Assert.AreEqual(6, sut.Minutes);
            Assert.AreEqual(7, sut.Seconds);
            Assert.AreEqual(8, sut.Milliseconds);
        }

        [Test]
        public void GetValue_ForYearDayTimePeriod()
        {
            Period sut = new Period(1, 0, 0, 4, 5, 6, 7, 8, PeriodType.YearDayTime);
            Assert.AreEqual(6, sut.Size);
            Assert.AreEqual(1, sut[0]);
            Assert.AreEqual(4, sut[1]);
            Assert.AreEqual(5, sut[2]);
            Assert.AreEqual(6, sut[3]);
            Assert.AreEqual(7, sut[4]);
            Assert.AreEqual(8, sut[5]);
        }

        [Test]
        public void GetValues_ForYearDayTimePeriod()
        {
            Period sut = new Period(1, 0, 0, 4, 5, 6, 7, 8, PeriodType.YearDayTime);
            var values  = sut.GetValues();

            Assert.AreEqual(6, values.Length);
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(4, values[1]);
            Assert.AreEqual(5, values[2]);
            Assert.AreEqual(6, values[3]);
            Assert.AreEqual(7, values[4]);
            Assert.AreEqual(8, values[5]);

            values[0] = 42;
            Assert.AreEqual(1, sut[0]);
        }

        [Test]
        public void GetFieldType_ForYearDayTimePeriod()
        {
            Period sut = new Period(1, 0, 0, 4, 5, 6, 7, 8, PeriodType.YearDayTime);
            Assert.AreEqual(DurationFieldType.Years, sut.GetFieldType(0));
            Assert.AreEqual(DurationFieldType.Days, sut.GetFieldType(1));
            Assert.AreEqual(DurationFieldType.Hours, sut.GetFieldType(2));
            Assert.AreEqual(DurationFieldType.Minutes, sut.GetFieldType(3));
            Assert.AreEqual(DurationFieldType.Seconds, sut.GetFieldType(4));
            Assert.AreEqual(DurationFieldType.Milliseconds, sut.GetFieldType(5));
        }

        [Test]
        public void GetFieldTypes_ForYearDayTimePeriod()
        {
            Period sut = new Period(1, 0, 0, 4, 5, 6, 7, 8, PeriodType.YearDayTime);
            var fieldTypes = sut.GetFieldTypes();

            Assert.AreEqual(DurationFieldType.Years, fieldTypes[0]);
            Assert.AreEqual(DurationFieldType.Days, fieldTypes[1]);
            Assert.AreEqual(DurationFieldType.Hours, fieldTypes[2]);
            Assert.AreEqual(DurationFieldType.Minutes, fieldTypes[3]);
            Assert.AreEqual(DurationFieldType.Seconds, fieldTypes[4]);
            Assert.AreEqual(DurationFieldType.Milliseconds, fieldTypes[5]);

            fieldTypes[0] = DurationFieldType.Eras;
            Assert.AreEqual(DurationFieldType.Years, sut.GetFieldType(0));
        }

        [Test]
        public void Get_ForYearDayTimePeriod()
        {
            Period sut = new Period(1, 0, 0, 4, 5, 6, 7, 8, PeriodType.YearDayTime);
            Assert.AreEqual(1, sut[DurationFieldType.Years]);
            Assert.AreEqual(4, sut[DurationFieldType.Days]);
            Assert.AreEqual(5, sut[DurationFieldType.Hours]);
            Assert.AreEqual(6, sut[DurationFieldType.Minutes]);
            Assert.AreEqual(7, sut[DurationFieldType.Seconds]);
            Assert.AreEqual(8, sut[DurationFieldType.Milliseconds]);
        }

        [Test]
        public void IsSupported_ForYearDayTimePeriod()
        {
            Period sut = new Period(1, 0, 0, 4, 5, 6, 7, 8, PeriodType.YearDayTime);
            Assert.IsFalse(sut.IsSupported(DurationFieldType.Eras));
            Assert.IsFalse(sut.IsSupported(DurationFieldType.Centuries));
            Assert.IsTrue(sut.IsSupported(DurationFieldType.Years));
            Assert.IsFalse(sut.IsSupported(DurationFieldType.WeekYears));
            Assert.IsFalse(sut.IsSupported(DurationFieldType.Months));
            Assert.IsTrue(sut.IsSupported(DurationFieldType.Days));
            Assert.IsFalse(sut.IsSupported(DurationFieldType.HalfDays));
            Assert.IsTrue(sut.IsSupported(DurationFieldType.Hours));
            Assert.IsTrue(sut.IsSupported(DurationFieldType.Minutes));
            Assert.IsTrue(sut.IsSupported(DurationFieldType.Seconds));
            Assert.IsTrue(sut.IsSupported(DurationFieldType.Milliseconds));
        }

    }
}
