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
    public partial class WeeksTest
    {
        [Test]
        public void StaticProperties_ReturnsCorrectValues()
        {
            Assert.AreEqual(0, Weeks.Zero.Value);
            Assert.AreEqual(1, Weeks.One.Value);
            Assert.AreEqual(2, Weeks.Two.Value);
            Assert.AreEqual(3, Weeks.Three.Value);
            Assert.AreEqual(int.MinValue, Weeks.MinValue.Value);
            Assert.AreEqual(int.MaxValue, Weeks.MaxValue.Value);
        }

        [Test]
        public void StaticProperties_ReturnsCachedInstances()
        {
            Assert.AreSame(Weeks.Zero, Weeks.Zero);
            Assert.AreSame(Weeks.One, Weeks.One);
            Assert.AreSame(Weeks.Two, Weeks.Two);
            Assert.AreSame(Weeks.Three, Weeks.Three);
            Assert.AreSame(Weeks.MinValue, Weeks.MinValue);
            Assert.AreSame(Weeks.MaxValue, Weeks.MaxValue);
        }

        [Test]
        public void From_ReturnsCachedInstancesUpTo3Value()
        {
            Assert.AreSame(Weeks.Zero, Weeks.From(0));
            Assert.AreSame(Weeks.One, Weeks.From(1));
            Assert.AreSame(Weeks.Two, Weeks.From(2));
            Assert.AreSame(Weeks.Three, Weeks.From(3));
            Assert.AreSame(Weeks.MinValue, Weeks.From(int.MinValue));
            Assert.AreSame(Weeks.MaxValue, Weeks.From(int.MaxValue));

            Assert.AreEqual(10, Weeks.From(10).Value);
            Assert.AreNotSame(Weeks.From(10), Weeks.From(10));
        }
    }
}
