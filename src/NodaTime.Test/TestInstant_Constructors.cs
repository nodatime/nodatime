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
using System.Globalization;

using NodaTime.Chronologies;
using NodaTime.Converters;

using NUnit.Framework;

namespace NodaTime
{
    /// <summary>
    /// Original name: TestInstant_Constructors
    /// </summary>
    [TestFixture]
    public class TestInstant_Constructors
    {
        // This is not used, but it was in JodaTime. Maybe we should just delete it
        private static readonly DateTimeZone Paris = DateTimeZone.ForID("Europe/Paris");

        private static readonly DateTimeZone London = DateTimeZone.ForID("Europe/London");

        // 1970-06-09
        private const long TestTimeNow =
            (31L + 28L + 31L + 30L + 31L + 9L - 1L) * DateTimeConstants.MillisecondsPerDay;
        // 1970-04-05
        private const long TestTime1 =
            (31L + 28L + 31L + 6L - 1L) * DateTimeConstants.MillisecondsPerDay
            + 12L * DateTimeConstants.MillisecondsPerHour
            + 24L * DateTimeConstants.MillisecondsPerMinute;

        // 1971-05-06
        private const long TestTime2 =
            (365L + 31L + 28L + 31L + 30L + 7L - 1L) * DateTimeConstants.MillisecondsPerDay
            + 14L * DateTimeConstants.MillisecondsPerHour
            + 28L * DateTimeConstants.MillisecondsPerMinute;

        private DateTimeZone zone;
        // Was java.util.Locale, using CultureInfo
        private CultureInfo locale;

        [SetUp]
        public void SetUp()
        {
            DateTimeUtils.SetCurrentMillisFixed(TestTimeNow);
            zone = DateTimeZone.Default;
            locale = CultureInfo.CurrentCulture;
            DateTimeZone.Default = London;
            // TODO: TimeZone.setDefault(London.ToTimeZone());
            // TODO: Locale.setDefault(Locale.UK);
        }

        [TearDown]
        public void TearDown()
        {
            DateTimeUtils.SetCurrentMillisSystem();
            DateTimeZone.Default = zone;
            // TODO: TimeZone.setDefault(zone.toTimeZone());
            // TODO: Locale.setDefault(locale);
            zone = null;
        }

        [Test]
        public void TestConstructor()
        {
            var test = new Instant();
            Assert.Equals(IsoChronology.Utc, test.Chronology);
            Assert.Equals(TestTimeNow, test.Milliseconds);
        }

        [Test]
        public void TestConstructor_long1()
        {
            var test = new Instant(TestTime1);
            Assert.Equals(IsoChronology.Utc, test.Chronology);
            Assert.Equals(TestTime1, test.Milliseconds);
        }

        [Test]
        public void TestConstructor_long2()
        {
            var test = new Instant(TestTime2);
            Assert.Equals(IsoChronology.Utc, test.Chronology);
            Assert.Equals(TestTime2, test.Milliseconds);
        }

        [Test]
        public void TestConstructor_Object()
        {
            var date = new System.DateTime(TestTime1);
            var test = new Instant(date);
            Assert.Equals(IsoChronology.Utc, test.Chronology);
            Assert.Equals(TestTime1, test.Milliseconds);
        }

        [Test]
        public void TestConstructor_InvalidObject()
        {
            Assert.Throws<ArgumentException>(() => new Instant(new object()));
        }

        [Test]
        public void TestConstructor_NullObject()
        {
            var test = new Instant(null);
            Assert.Equals(IsoChronology.Utc, test.Chronology);
            Assert.Equals(TestTimeNow, test.Milliseconds);
        }

        [Test]
        public void TestConstructor_BadConverterObject()
        {
            try
            {
                ConverterManager.Instance.AddInstantConverter(MockZeroNullIntegerConverter.Instance);
                var test = new Instant((object) 0);
                Assert.Equals(IsoChronology.Utc, test.Chronology);
                Assert.Equals(0L, test.Milliseconds);
            }
            finally
            {
                ConverterManager.Instance.RemoveInstantConverter(MockZeroNullIntegerConverter.Instance);
            }
        }
    }
}