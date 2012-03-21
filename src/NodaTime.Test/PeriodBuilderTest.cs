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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class PeriodBuilderTest
    {
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
            Assert.Throws<InvalidOperationException>(() => new PeriodBuilder().Build());
        }

        [Test]
        public void Equality()
        {
            PeriodBuilder equal1 = new PeriodBuilder { Hours = 10, Minutes = 1 };
            PeriodBuilder equal2 = new PeriodBuilder { Minutes = 1, Hours = 10, Ticks = null };
            PeriodBuilder unequal = new PeriodBuilder { Minutes = 1 };
            TestHelper.TestEqualsClass(equal1, equal2, unequal);
        }
    }
}
