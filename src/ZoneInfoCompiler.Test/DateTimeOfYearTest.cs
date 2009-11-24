#region Copyright and license information
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
using System.Collections;
using System.Collections.Generic;
using NodaTime.ZoneInfoCompiler;
using NUnit.Framework;
using NodaTime.ZoneInfoCompiler.Tzdb;

namespace ZoneInfoCompiler.Test
{    
    /// <summary>
    /// This is a test class for containing all of the DateTimeOfYear unit tests.
    ///</summary>
    [TestFixture]
    public class DateTimeOfYearTest
    {
#if qwe
        [Test]
        public void DateTimeOfYearTestConstructor_noArgs()
        {
            DateTimeOfYear actual = new DateTimeOfYear();
            Validate(actual, 1, 1, 0, false, 0, 'w');
        }

        [Test]
        public void DateTimeOfYearTestConstructor_null_exception()
        {
            IEnumerator<string> en = null;
            Assert.Throws(typeof(ArgumentNullException), () => new DateTimeOfYear(en));
        }

        [Test]
        public void DateTimeOfYearTestConstructor_empty()
        {
            List<string> values = new List<string>() { };
            IEnumerator<string> en = values.GetEnumerator();
            DateTimeOfYear actual = new DateTimeOfYear(en);
            Validate(actual, 1, 1, 0, false, 0, 'w');
        }

        [Test]
        public void DateTimeOfYearTestConstructor_invalidMonth_exception()
        {
            List<string> values = new List<string>() { "Abc" };
            IEnumerator<string> en = values.GetEnumerator();
            Assert.Throws(typeof(ArgumentException), () => new DateTimeOfYear(en));
        }

        [Test]
        public void DateTimeOfYearTestConstructor_month()
        {
            List<string> values = new List<string>() { "Jun" };
            IEnumerator<string> en = values.GetEnumerator();
            DateTimeOfYear actual = new DateTimeOfYear(en);
            Validate(actual, 6, 1, 0, false, 0, 'w');
        }

        [Test]
        public void DateTimeOfYearTestConstructor_fullWithGE()
        {
            List<string> values = new List<string>() { "Mar", "Sun>=8", "2:00", "1:30", "s" };
            IEnumerator<string> en = values.GetEnumerator();
            DateTimeOfYear actual = new DateTimeOfYear(en);
            Validate(actual, 3, 8, 0, true, 7200000, 's');
        }

        /// <summary>
        /// Validates the specified actual.
        /// </summary>
        /// <param name="actual">The actual.</param>
        /// <param name="monthOfYear">The month of year.</param>
        /// <param name="dayOfMonth">The day of month.</param>
        /// <param name="dayOfWeek">The day of week.</param>
        /// <param name="advance">if set to <c>true</c> [advance].</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="zone">The zone.</param>
        private void Validate(DateTimeOfYear actual, int monthOfYear, int dayOfMonth, int dayOfWeek, bool advance, int millisecond, char zone)
        {
            Assert.AreEqual(monthOfYear, actual.MonthOfYear, "MonthOfYear");
            Assert.AreEqual(dayOfMonth, actual.DayOfMonth, "DayOfMonth");
            Assert.AreEqual(dayOfWeek, actual.DayOfWeek, "DayOfWeek");
            Assert.AreEqual(advance, actual.AdvanceDayOfWeek, "AdvanceDayOfWeek");
            Assert.AreEqual(millisecond, actual.MillisecondOfDay, "MillisecondOfDay");
            Assert.AreEqual(zone, actual.ZoneChar, "ZoneChar not the expected value of '" + zone + "': '" + actual.ZoneChar + "'");
        }
#endif
    }
}
