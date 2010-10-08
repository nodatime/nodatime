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
    partial class DaysTest
    {
        [Test]
        public void StaticProperties_ReturnsCorrectValues()
        {
            Assert.AreEqual(0, Days.Zero.Value);
            Assert.AreEqual(1, Days.One.Value);
            Assert.AreEqual(2, Days.Two.Value);
            Assert.AreEqual(3, Days.Three.Value);
            Assert.AreEqual(4, Days.Four.Value);
            Assert.AreEqual(5, Days.Five.Value);
            Assert.AreEqual(6, Days.Six.Value);
            Assert.AreEqual(7, Days.Seven.Value);

            Assert.AreEqual(int.MinValue, Days.MinValue.Value);
            Assert.AreEqual(int.MaxValue, Days.MaxValue.Value);
        }

        [Test]
        public void StaticProperties_ReturnsCachedInstances()
        {
            Assert.AreSame(Days.Zero, Days.Zero);
            Assert.AreSame(Days.One, Days.One);
            Assert.AreSame(Days.Two, Days.Two);
            Assert.AreSame(Days.Three, Days.Three);
            Assert.AreSame(Days.Four, Days.Four);
            Assert.AreSame(Days.Five, Days.Five);
            Assert.AreSame(Days.Six, Days.Six);
            Assert.AreSame(Days.Seven, Days.Seven);

            Assert.AreSame(Days.MinValue, Days.MinValue);
            Assert.AreSame(Days.MaxValue, Days.MaxValue);
        }

        [Test]
        public void From_ReturnsCachedInstancesUpTo7Value()
        {
            Assert.AreSame(Days.Zero, Days.From(0));
            Assert.AreSame(Days.One, Days.From(1));
            Assert.AreSame(Days.Two, Days.From(2));
            Assert.AreSame(Days.Three, Days.From(3));
            Assert.AreSame(Days.Four, Days.From(4));
            Assert.AreSame(Days.Five, Days.From(5));
            Assert.AreSame(Days.Six, Days.From(6));
            Assert.AreSame(Days.Seven, Days.From(7));

            Assert.AreSame(Days.MinValue, Days.From(int.MinValue));
            Assert.AreSame(Days.MaxValue, Days.From(int.MaxValue));

            Assert.AreEqual(10, Days.From(10).Value);
        }
    }
}