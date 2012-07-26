#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
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
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class PeriodBuilderTest
    {
        [Test]
        public void Indexer_Getter_ValidUnits()
        {
            var builder = new PeriodBuilder
            {
                Years = null,
                Months = 1,
                Weeks = 2,
                Days = 3,
                Hours = 4,
                Minutes = 5,
                Seconds = 6,
                Milliseconds = 7,
                Ticks = 8
            };
            Assert.IsFalse(builder[PeriodUnits.Years].HasValue);
            Assert.AreEqual(1L, builder[PeriodUnits.Months]);
            Assert.AreEqual(2L, builder[PeriodUnits.Weeks]);
            Assert.AreEqual(3L, builder[PeriodUnits.Days]);
            Assert.AreEqual(4L, builder[PeriodUnits.Hours]);
            Assert.AreEqual(5L, builder[PeriodUnits.Minutes]);
            Assert.AreEqual(6L, builder[PeriodUnits.Seconds]);
            Assert.AreEqual(7L, builder[PeriodUnits.Milliseconds]);
            Assert.AreEqual(8L, builder[PeriodUnits.Ticks]);
        }

        [Test]
        public void Index_Getter_InvalidUnits()
        {
            var builder = new PeriodBuilder();
            Assert.Throws<ArgumentOutOfRangeException>(() => builder[0].GetValueOrDefault());
            Assert.Throws<ArgumentOutOfRangeException>(() => builder[(PeriodUnits) (-1)].GetValueOrDefault());
            Assert.Throws<ArgumentOutOfRangeException>(() => builder[PeriodUnits.DateAndTime].GetValueOrDefault());
        }

        [Test]
        public void Indexer_Setter_ValidUnits()
        {
            var builder = new PeriodBuilder();
            builder[PeriodUnits.Years] = null;
            builder[PeriodUnits.Months] = 1;
            builder[PeriodUnits.Weeks] = 2;
            builder[PeriodUnits.Days] = 3;
            builder[PeriodUnits.Hours] = 4;
            builder[PeriodUnits.Minutes] = 5;
            builder[PeriodUnits.Seconds] = 6;
            builder[PeriodUnits.Milliseconds] = 7;
            builder[PeriodUnits.Ticks] = 8;
            var expected = new PeriodBuilder
            {
                Years = null,
                Months = 1,
                Weeks = 2,
                Days = 3,
                Hours = 4,
                Minutes = 5,
                Seconds = 6,
                Milliseconds = 7,
                Ticks = 8
            }.Build();
            Assert.AreEqual(expected, builder.Build());
        }

        [Test]
        public void Index_Setter_InvalidUnits()
        {
            var builder = new PeriodBuilder();
            Assert.Throws<ArgumentOutOfRangeException>(() => builder[0] = 1L);
            Assert.Throws<ArgumentOutOfRangeException>(() => builder[(PeriodUnits)(-1)] = 1L);
            Assert.Throws<ArgumentOutOfRangeException>(() => builder[PeriodUnits.DateAndTime] = 1L);
        }

        [Test]
        public void Build_SingleUnit()
        {
            Period period = new PeriodBuilder { Hours = 10 }.Build();

            Period expected = Period.FromHours(10);
            Assert.AreEqual(expected, period);
            Assert.AreEqual(expected.Units, period.Units);
        }

        [Test]
        public void Build_MultipleUnits()
        {
            Period period = new PeriodBuilder { Days = 5, Minutes = -10 }.Build();

            Period expected = Period.FromDays(5) + Period.FromMinutes(-10);
            Assert.AreEqual(expected, period);
            Assert.AreEqual(expected.Units, period.Units);
        }

        [Test]
        public void Build_Empty()
        {
            Assert.AreEqual(Period.Empty, new PeriodBuilder().Build());
        }
    }
}
