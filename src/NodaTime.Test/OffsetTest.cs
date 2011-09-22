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
using System.Globalization;
using NUnit.Framework;
using NodaTime.Globalization;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class OffsetTest
    {
        private static readonly Offset ThreeHours = Offset.Create(3, 0, 0, 0);
        private static readonly Offset NegativeThreeHours = Offset.Create(3, 0, 0, 0, true);
        private static readonly Offset NegativeTwelveHours = Offset.Create(12, 0, 0, 0, true);

        [Test]
        public void Max()
        {
            Offset x = Offset.FromMilliseconds(100);
            Offset y = Offset.FromMilliseconds(200);
            Assert.AreEqual(y, Offset.Max(x, y));
            Assert.AreEqual(y, Offset.Max(y, x));
            Assert.AreEqual(x, Offset.Max(x, Offset.MinValue));
            Assert.AreEqual(x, Offset.Max(Offset.MinValue, x));
            Assert.AreEqual(Offset.MaxValue, Offset.Max(Offset.MaxValue, x));
            Assert.AreEqual(Offset.MaxValue, Offset.Max(x, Offset.MaxValue));
        }

        [Test]
        public void Min()
        {
            Offset x = Offset.FromMilliseconds(100);
            Offset y = Offset.FromMilliseconds(200);
            Assert.AreEqual(x, Offset.Min(x, y));
            Assert.AreEqual(x, Offset.Min(y, x));
            Assert.AreEqual(Offset.MinValue, Offset.Min(x, Offset.MinValue));
            Assert.AreEqual(Offset.MinValue, Offset.Min(Offset.MinValue, x));
            Assert.AreEqual(x, Offset.Min(Offset.MaxValue, x));
            Assert.AreEqual(x, Offset.Min(x, Offset.MaxValue));
        }

        [Test]
        public void ToTimeSpan()
        {
            TimeSpan ts = Offset.FromMilliseconds(1234).ToTimeSpan();
            Assert.AreEqual(ts, TimeSpan.FromMilliseconds(1234));
        }

        [Test]
        public void Tmp()
        {
            Offset.ParseExact(".6", ".FFF", NodaFormatInfo.InvariantInfo);
        }
    }
}
