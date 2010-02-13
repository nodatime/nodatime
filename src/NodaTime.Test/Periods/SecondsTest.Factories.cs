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
    partial class SecondsTest
    {
        [Test]
        public void StaticProperties_ReturnsCorrectValues()
        {
            Assert.AreEqual(0, Seconds.Zero.Value);
            Assert.AreEqual(1, Seconds.One.Value);
            Assert.AreEqual(2, Seconds.Two.Value);
            Assert.AreEqual(3, Seconds.Three.Value);

            Assert.AreEqual(int.MinValue, Seconds.MinValue.Value);
            Assert.AreEqual(int.MaxValue, Seconds.MaxValue.Value);
        }

        [Test]
        public void StaticProperties_ReturnsCachedInstances()
        {
            Assert.AreSame(Seconds.Zero, Seconds.Zero);
            Assert.AreSame(Seconds.One, Seconds.One);
            Assert.AreSame(Seconds.Two, Seconds.Two);
            Assert.AreSame(Seconds.Three, Seconds.Three);

            Assert.AreSame(Seconds.MinValue, Seconds.MinValue);
            Assert.AreSame(Seconds.MaxValue, Seconds.MaxValue);
        }

        [Test]
        public void From_ReturnsCachedInstancesUpTo3Value()
        {
            Assert.AreSame(Seconds.Zero, Seconds.From(0));
            Assert.AreSame(Seconds.One, Seconds.From(1));
            Assert.AreSame(Seconds.Two, Seconds.From(2));
            Assert.AreSame(Seconds.Three, Seconds.From(3));

            Assert.AreSame(Seconds.MinValue, Seconds.From(int.MinValue));
            Assert.AreSame(Seconds.MaxValue, Seconds.From(int.MaxValue));

            Assert.AreEqual(10, Seconds.From(10).Value);
        }
    }
}