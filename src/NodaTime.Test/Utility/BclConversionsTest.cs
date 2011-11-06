#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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
using NodaTime.Utility;

namespace NodaTime.Test.Utility
{
    [TestFixture]
    public class BclConversionsTest
    {
        // This tests both directions for all valid values.
        // Alternatively, we could have checked names...
        // it doesn't matter much.
        [TestCase(DayOfWeek.Sunday, IsoDayOfWeek.Sunday)]
        [TestCase(DayOfWeek.Monday, IsoDayOfWeek.Monday)]
        [TestCase(DayOfWeek.Tuesday, IsoDayOfWeek.Tuesday)]
        [TestCase(DayOfWeek.Wednesday, IsoDayOfWeek.Wednesday)]
        [TestCase(DayOfWeek.Thursday, IsoDayOfWeek.Thursday)]
        [TestCase(DayOfWeek.Friday, IsoDayOfWeek.Friday)]
        [TestCase(DayOfWeek.Saturday, IsoDayOfWeek.Saturday)]
        public void DayOfWeek_BothWaysValid(DayOfWeek bcl, IsoDayOfWeek noda)
        {
            Assert.AreEqual(bcl, BclConversions.ToDayOfWeek(noda));
            Assert.AreEqual(noda, BclConversions.ToIsoDayOfWeek(bcl));
        }

        [TestCase(0)]
        [TestCase(8)]
        public void ToDayOfWeek_InvalidValues(IsoDayOfWeek noda)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => BclConversions.ToDayOfWeek(noda));
        }

        [TestCase(-1)]
        [TestCase(7)]
        public void ToIsoDayOfWeek_InvalidValues(DayOfWeek bcl)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => BclConversions.ToIsoDayOfWeek(bcl));
        }
    }
}
