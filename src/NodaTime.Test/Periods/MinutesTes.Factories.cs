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
    partial class MinutesTest
    {
        [Test]
        public void StaticProperties_ReturnsCorrectValues()
        {
            Assert.AreEqual(0, Minutes.Zero.Value);
            Assert.AreEqual(1, Minutes.One.Value);
            Assert.AreEqual(2, Minutes.Two.Value);
            Assert.AreEqual(3, Minutes.Three.Value);

            Assert.AreEqual(int.MinValue, Minutes.MinValue.Value);
            Assert.AreEqual(int.MaxValue, Minutes.MaxValue.Value);
        }

        [Test]
        public void StaticProperties_ReturnsCachedInstances()
        {
            Assert.AreSame(Minutes.Zero, Minutes.Zero);
            Assert.AreSame(Minutes.One, Minutes.One);
            Assert.AreSame(Minutes.Two, Minutes.Two);
            Assert.AreSame(Minutes.Three, Minutes.Three);

            Assert.AreSame(Minutes.MinValue, Minutes.MinValue);
            Assert.AreSame(Minutes.MaxValue, Minutes.MaxValue);
        }

        [Test]
        public void From_ReturnsCachedInstancesUpTo3Value()
        {
            Assert.AreSame(Minutes.Zero, Minutes.From(0));
            Assert.AreSame(Minutes.One, Minutes.From(1));
            Assert.AreSame(Minutes.Two, Minutes.From(2));
            Assert.AreSame(Minutes.Three, Minutes.From(3));

            Assert.AreSame(Minutes.MinValue, Minutes.From(int.MinValue));
            Assert.AreSame(Minutes.MaxValue, Minutes.From(int.MaxValue));

            Assert.AreEqual(10, Minutes.From(10).Value);
        }
    }
}