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
    partial class HoursTest
    {
        [Test]
        public void StaticProperties_ReturnsCorrectValues()
        {
            Assert.AreEqual(0, Hours.Zero.Value);
            Assert.AreEqual(1, Hours.One.Value);
            Assert.AreEqual(2, Hours.Two.Value);
            Assert.AreEqual(3, Hours.Three.Value);
            Assert.AreEqual(4, Hours.Four.Value);
            Assert.AreEqual(5, Hours.Five.Value);
            Assert.AreEqual(6, Hours.Six.Value);
            Assert.AreEqual(7, Hours.Seven.Value);
            Assert.AreEqual(8, Hours.Eight.Value);

            Assert.AreEqual(int.MinValue, Hours.MinValue.Value);
            Assert.AreEqual(int.MaxValue, Hours.MaxValue.Value);
        }

        [Test]
        public void StaticProperties_ReturnsCachedInstances()
        {
            Assert.AreSame(Hours.Zero, Hours.Zero);
            Assert.AreSame(Hours.One, Hours.One);
            Assert.AreSame(Hours.Two, Hours.Two);
            Assert.AreSame(Hours.Three, Hours.Three);
            Assert.AreSame(Hours.Four, Hours.Four);
            Assert.AreSame(Hours.Five, Hours.Five);
            Assert.AreSame(Hours.Six, Hours.Six);
            Assert.AreSame(Hours.Seven, Hours.Seven);

            Assert.AreSame(Hours.MinValue, Hours.MinValue);
            Assert.AreSame(Hours.MaxValue, Hours.MaxValue);
        }

        [Test]
        public void From_ReturnsCachedInstancesUpTo8Value()
        {
            Assert.AreSame(Hours.Zero, Hours.From(0));
            Assert.AreSame(Hours.One, Hours.From(1));
            Assert.AreSame(Hours.Two, Hours.From(2));
            Assert.AreSame(Hours.Three, Hours.From(3));
            Assert.AreSame(Hours.Four, Hours.From(4));
            Assert.AreSame(Hours.Five, Hours.From(5));
            Assert.AreSame(Hours.Six, Hours.From(6));
            Assert.AreSame(Hours.Seven, Hours.From(7));
            Assert.AreSame(Hours.Eight, Hours.From(8));

            Assert.AreSame(Hours.MinValue, Hours.From(int.MinValue));
            Assert.AreSame(Hours.MaxValue, Hours.From(int.MaxValue));

            Assert.AreEqual(10, Hours.From(10).Value);
        }
    }
}