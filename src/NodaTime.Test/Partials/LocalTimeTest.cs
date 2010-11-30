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

using System;
using NodaTime.Partials;
using NUnit.Framework;

namespace NodaTime.Test.Partials
{
    [TestFixture]
    public class LocalTimeTest
    {
        [Test]
        public void Addition_WithPeriod()
        {
            LocalTime start = new LocalTime(3, 30, 0);
            Period2 period = Period2.FromHours(2) + Period2.FromSeconds(1);
            LocalTime expected = new LocalTime(5, 30, 1);
            Assert.AreEqual(expected, start + period);
        }

        [Test]
        public void Addition_WrapsAtMidnight()
        {
            LocalTime start = new LocalTime(22, 0, 0);
            Period2 period = Period2.FromHours(3);
            LocalTime expected = new LocalTime(1, 0, 0);
            Assert.AreEqual(expected, start + period);
        }

        [Test]
        public void Addition_WithNullPeriod_ThrowsArgumentNullException()
        {
            LocalTime date = new LocalTime(12, 0, 0);
            // Call to ToString just to make it a valid statement
            Assert.Throws<ArgumentNullException>(() => (date + (Period2)null).ToString());
        }

        [Test]
        public void Subtraction_WithPeriod()
        {
            LocalTime start = new LocalTime(5, 30, 1);
            Period2 period = Period2.FromHours(2) + Period2.FromSeconds(1);
            LocalTime expected = new LocalTime(3, 30, 0);
            Assert.AreEqual(expected, start - period);
        }

        [Test]
        public void Subtraction_WrapsAtMidnight()
        {
            LocalTime start = new LocalTime(1, 0, 0);
            Period2 period = Period2.FromHours(3);
            LocalTime expected = new LocalTime(22, 0, 0);
            Assert.AreEqual(expected, start - period);
        }

        [Test]
        public void Subtraction_WithNullPeriod_ThrowsArgumentNullException()
        {
            LocalTime date = new LocalTime(12, 0, 0);
            // Call to ToString just to make it a valid statement
            Assert.Throws<ArgumentNullException>(() => (date - (Period2)null).ToString());
        }
    }
}