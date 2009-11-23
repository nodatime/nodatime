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
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class DurationTest
    {
        Duration zero = new Duration(0L);
        Duration one = new Duration(1L);
        Duration onePrime = new Duration(1L);
        Duration threeMillion = new Duration(3000000L);
        Duration negativeFiftyMillion = new Duration(-50000000L);
        Duration minimum = new Duration(Int64.MinValue);
        Duration maximum = new Duration(Int64.MaxValue);

        [Test]
        public void OperatorPlus()
        {
            Assert.AreEqual(0L, (zero + zero).Ticks);
            Assert.AreEqual(1L, (one + zero).Ticks);
            Assert.AreEqual(1L, (zero + one).Ticks);
            Assert.AreEqual(3000001L, (threeMillion + one).Ticks);
        }

        [Test]
        public void OperatorMinus()
        {
            Assert.AreEqual(0L, (zero - zero).Ticks);
            Assert.AreEqual(1L, (one - zero).Ticks);
            Assert.AreEqual(-1L, (zero - one).Ticks);
            Assert.AreEqual(2999999L, (threeMillion - one).Ticks);
        }

        [Test]
        public void CompareTo()
        {
            Assert.True(one.CompareTo(one) == 0, "1 < 1 (same object)");
            Assert.True(one.CompareTo(onePrime) == 0, "1 < 1 (different objects)");
            Assert.True(one.CompareTo(negativeFiftyMillion) > 0, "1 < -50,000,000");
            Assert.True(one.CompareTo(threeMillion) < 0, "1 < 3,000,000");
            Assert.True(minimum.CompareTo(maximum) < 0, "MinValue < MaxValue");
            Assert.True(maximum.CompareTo(minimum) > 0, "MaxValue > MinValue");
        }

        [Test]
        public void IEquatableEquals()
        {
            Assert.True(one.Equals(one), "1 == 1 (same object)");
            Assert.True(one.Equals(onePrime), "1 == 1 (different objects)");
            Assert.False(one.Equals(threeMillion), "1 == 3,000,000");
            Assert.False(one.Equals(negativeFiftyMillion), "1 == -50,000,000");
            Assert.False(minimum.Equals(maximum), "MinValue == MaxValue");
        }

        [Test]
        public void ObjectEquals()
        {
            Object oOne = one;
            Object oOnePrime = onePrime;
            Object oThreeMillion = threeMillion;
            Object oNegativeFiftyMillion = negativeFiftyMillion;
            Object oMinimum = minimum;
            Object oMaximum = maximum;

            Assert.False(oOne.Equals(null), "1 == null");
            Assert.True(oOne.Equals(oOne), "1 == 1 (same object)");
            Assert.True(oOne.Equals(oOnePrime), "1 == 1 (different objects)");
            Assert.False(oOne.Equals(oThreeMillion), "1 == 3,000,000");
            Assert.False(oOne.Equals(oNegativeFiftyMillion), "1 == -50,000,000");
            Assert.False(oMinimum.Equals(oMaximum), "MinValue == MaxValue");
        }

        [Test]
        public void OperatorEquals()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.True(one == one, "1 == 1 (same object)");
#pragma warning restore 1718
            Assert.True(one == onePrime, "1 == 1 (different objects)");
            Assert.False(one == threeMillion, "1 == 3,000,000");
            Assert.False(one == negativeFiftyMillion, "1 == -50,000,000");
            Assert.False(minimum == maximum, "MinValue == MaxValue");
        }

        [Test]
        public void OperatorNotEquals()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.False(one != one, "1 != 1 (same object)");
#pragma warning restore 1718
            Assert.False(one != onePrime, "1 != 1 (different objects)");
            Assert.True(one != threeMillion, "1 != 3,000,000");
            Assert.True(one != negativeFiftyMillion, "1 != -50,000,000");
            Assert.True(minimum != maximum, "MinValue != MaxValue");
        }

        [Test]
        public void OperatorLessThan()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.False(one < one, "1 < 1 (same object)");
#pragma warning restore 1718
            Assert.False(one < onePrime, "1 < 1 (different objects)");
            Assert.True(one < threeMillion, "1 < 3,000,000");
            Assert.False(one < negativeFiftyMillion, "1 < -50,000,000");
            Assert.True(minimum < maximum, "MinValue < MaxValue");
        }

        [Test]
        public void OperatorLessThanEqualTo()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.True(one <= one, "1 <= 1 (same object)");
#pragma warning restore 1718
            Assert.True(one <= onePrime, "1 <= 1 (different objects)");
            Assert.True(one <= threeMillion, "1 <= 3,000,000");
            Assert.False(one <= negativeFiftyMillion, "1 <= -50,000,000");
            Assert.True(minimum <= maximum, "MinValue <= MaxValue");
        }

        [Test]
        public void OperatorGreaterThan()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.False(one > one, "1 > 1 (same object)");
#pragma warning restore 1718
            Assert.False(one > onePrime, "1 > 1 (different objects)");
            Assert.False(one > threeMillion, "1 > 3,000,000");
            Assert.True(one > negativeFiftyMillion, "1 > -50,000,000");
            Assert.False(minimum > maximum, "MinValue > MaxValue");
        }

        [Test]
        public void OperatorGreaterThanEqualTo()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.True(one >= one, "1 >= 1 (same object)");
#pragma warning restore 1718
            Assert.True(one >= onePrime, "1 >= 1 (different objects)");
            Assert.False(one >= threeMillion, "1 >= 3,000,000");
            Assert.True(one >= negativeFiftyMillion, "1 >= -50,000,000");
            Assert.False(minimum >= maximum, "MinValue >= MaxValue");
        }
    }
}
