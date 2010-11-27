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
    public partial class YearsTest
    {
        [Test]
        public void StaticProperties_ReturnsCorrectValues()
        {
            Assert.AreEqual(0, Years.Zero.Value);
            Assert.AreEqual(1, Years.One.Value);
            Assert.AreEqual(2, Years.Two.Value);
            Assert.AreEqual(3, Years.Three.Value);
            Assert.AreEqual(int.MinValue, Years.MinValue.Value);
            Assert.AreEqual(int.MaxValue, Years.MaxValue.Value);
        }

        [Test]
        public void StaticProperties_ReturnsCachedInstances()
        {
            Assert.AreSame(Years.Zero, Years.Zero);
            Assert.AreSame(Years.One, Years.One);
            Assert.AreSame(Years.Two, Years.Two);
            Assert.AreSame(Years.Three, Years.Three);
            Assert.AreSame(Years.MinValue, Years.MinValue);
            Assert.AreSame(Years.MaxValue, Years.MaxValue);
        }

        [Test]
        public void From_ReturnsCachedInstancesUpTo3Value()
        {
            Assert.AreSame(Years.Zero, Years.From(0));
            Assert.AreSame(Years.One, Years.From(1));
            Assert.AreSame(Years.Two, Years.From(2));
            Assert.AreSame(Years.Three, Years.From(3));
            Assert.AreSame(Years.MinValue, Years.From(int.MinValue));
            Assert.AreSame(Years.MaxValue, Years.From(int.MaxValue));

            Assert.AreEqual(10, Years.From(10).Value);
        }
    }
}