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
            Assert.AreSame(Months.MinValue, Months.MinValue);
            Assert.AreSame(Months.MaxValue, Months.MaxValue);
        }

        [Test]
        public void From_ReturnsCachedInstancesUpTo3Value()
        {
            Assert.AreSame(Months.Zero, Months.From(0));
            Assert.AreSame(Months.One, Months.From(1));
            Assert.AreSame(Months.Two, Months.From(2));
            Assert.AreSame(Months.Three, Months.From(3));
            Assert.AreSame(Months.MinValue, Months.From(int.MinValue));
            Assert.AreSame(Months.MaxValue, Months.From(int.MaxValue));

            Assert.AreEqual(10, Months.From(10).Value);
        }
    }
}
