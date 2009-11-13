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

namespace NodaTime.Test
{
    // Test is commented out as little of it makes sense at the moment. We may or may not want some of it :)
    /*
    /// <summary>
    /// Original name: TestInstant_Assert.AreEquals
    /// </summary>
    public partial class InstantTest
    {
        /// <summary>
        /// TODO: Rename this file to InstantTest.Construction or something to allow for things like this.
        /// </summary>
        [Test]
        public void NowProperty_ReturnsCurrentTime()
        {
            using (TestClock.ReplaceCurrent(TestTimeNow))
            {
                var test = Instant.Now;
                Assert.Equals(IsoChronology.Utc, test.Chronology);
                Assert.Equals(TestTimeNow, test.Milliseconds);
            }
        }

        [Test]
        public void ConstructFrom_Long()
        {
            var test = new Instant(TestTime1);
            Assert.Equals(IsoChronology.Utc, test.Chronology);
            Assert.Equals(TestTime1, test.Milliseconds);
            test = new Instant(TestTime2);
            Assert.Equals(IsoChronology.Utc, test.Chronology);
            Assert.Equals(TestTime2, test.Milliseconds);
        }

        [Test]
        public void ConstructFrom_Object()
        {
            // TODO: Make this the right value (will currently be in ticks from the wrong starting point!)
            var date = new System.DateTime(TestTime1);
            var test = new Instant(date);
            Assert.Equals(IsoChronology.Utc, test.Chronology);
            Assert.Equals(TestTime1, test.Milliseconds);
        }

        [Test]
        public void ConstructFrom_InvalidObject()
        {
            Assert.Throws<ArgumentException>(() => new Instant(new object()));
        }

        /// <summary>
        /// TODO: Decide if we want to support this
        /// </summary>
        [Test]
        public void ConstructFrom_NullObjectReference_UsesSystemTime()
        {
            using (TestClock.ReplaceCurrent(TestTimeNow))
            {
                var test = new Instant(null);
                Assert.Equals(IsoChronology.Utc, test.Chronology);
                Assert.Equals(TestTimeNow, test.Milliseconds);
            }
        }

        [Test]
        public void ConstructFrom_BadConverterObject()
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
    }*/
}