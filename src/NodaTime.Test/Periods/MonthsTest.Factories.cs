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
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Periods
{
    public partial class MonthsTest
    {
        [Test]
        public void StaticProperties_ReturnsCorrectValues()
        {
            Assert.AreEqual(0, Months.Zero.Value);
            Assert.AreEqual(1, Months.One.Value);
            Assert.AreEqual(2, Months.Two.Value);
            Assert.AreEqual(3, Months.Three.Value);
            Assert.AreEqual(4, Months.Four.Value);
            Assert.AreEqual(5, Months.Five.Value);
            Assert.AreEqual(6, Months.Six.Value);
            Assert.AreEqual(7, Months.Seven.Value);
            Assert.AreEqual(8, Months.Eight.Value);
            Assert.AreEqual(9, Months.Nine.Value);
            Assert.AreEqual(10, Months.Ten.Value);
            Assert.AreEqual(11, Months.Eleven.Value);
            Assert.AreEqual(12, Months.Twelve.Value);

            Assert.AreEqual(int.MinValue, Months.MinValue.Value);
            Assert.AreEqual(int.MaxValue, Months.MaxValue.Value);
        }

        [Test]
        public void StaticProperties_ReturnsCachedInstances()
        {
            Assert.AreSame(Months.Zero, Months.Zero);
            Assert.AreSame(Months.One, Months.One);
            Assert.AreSame(Months.Two, Months.Two);
            Assert.AreSame(Months.Three, Months.Three);
            Assert.AreSame(Months.Four, Months.Four);
            Assert.AreSame(Months.Five, Months.Five);
            Assert.AreSame(Months.Six, Months.Six);
            Assert.AreSame(Months.Seven, Months.Seven);
            Assert.AreSame(Months.Eight, Months.Eight);
            Assert.AreSame(Months.Nine, Months.Nine);
            Assert.AreSame(Months.Ten, Months.Ten);
            Assert.AreSame(Months.Eleven, Months.Eleven);
            Assert.AreSame(Months.Twelve, Months.Twelve);

            Assert.AreSame(Months.MinValue, Months.MinValue);
            Assert.AreSame(Months.MaxValue, Months.MaxValue);
        }

        [Test]
        public void From_ReturnsCachedInstancesUpTo12Value()
        {
            Assert.AreSame(Months.Zero, Months.From(0));
            Assert.AreSame(Months.One, Months.From(1));
            Assert.AreSame(Months.Two, Months.From(2));
            Assert.AreSame(Months.Three, Months.From(3));
            Assert.AreSame(Months.Four, Months.From(4));
            Assert.AreSame(Months.Five, Months.From(5));
            Assert.AreSame(Months.Six, Months.From(6));
            Assert.AreSame(Months.Seven, Months.From(7));
            Assert.AreSame(Months.Eight, Months.From(8));
            Assert.AreSame(Months.Nine, Months.From(9));
            Assert.AreSame(Months.Ten, Months.From(10));
            Assert.AreSame(Months.Eleven, Months.From(11));
            Assert.AreSame(Months.Twelve, Months.From(12));
            
            Assert.AreSame(Months.MinValue, Months.From(int.MinValue));
            Assert.AreSame(Months.MaxValue, Months.From(int.MaxValue));

            Assert.AreEqual(20, Months.From(20).Value);
        }
    }
}
