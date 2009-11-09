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

namespace NodaTime.Test
{
    /// <summary>
    /// Original name: TestInstant_Constructors
    /// </summary>
    public partial class InstantTest
    {
        [Test]
        public void TestCurrentTime()
        {
            using (TestClock.ReplaceCurrent(TestTimeNow))
            {
                var test = Instant.Now;
                Assert.Equals(IsoChronology.Utc, test.Chronology);
                Assert.Equals(TestTimeNow, test.Milliseconds);
            }
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