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

#region usings
using System.Globalization;
using System.Threading;
using NodaTime.Globalization;
using NUnit.Framework;
#endregion

namespace NodaTime.Test.Globalization
{
    [TestFixture]
    public class NodaCultureInfoTest
    {
        [Test]
        public void TestClone()
        {
            var actual1 = NodaCultureInfo.InvariantCulture;
            var actual2 = actual1.Clone();
            Assert.AreNotSame(actual1, actual2);
            Assert.AreEqual(actual1, actual2);
        }

        [Test]
        public void TestGetFormat()
        {
            var info = new NodaCultureInfo("en-US");
            var nodaFormatInfo = info.GetFormat(typeof(NodaFormatInfo));
            Assert.NotNull(nodaFormatInfo, "GetFormat supoprts NodaFormatInfo");
            var numberFormatInfo = info.GetFormat(typeof(NumberFormatInfo));
            Assert.NotNull(numberFormatInfo, "NodaCultureInfo passes GetFormat() to underlying CultureInfo");
        }

        [Test]
        public void TestInvariantCulture()
        {
            var actual1 = NodaCultureInfo.InvariantCulture;
            Assert.NotNull(actual1, "Invariant #1 null");
            Assert.AreEqual(CultureInfo.InvariantCulture.LCID, actual1.LCID, "Invariant #1 LCID");

            var actual2 = NodaCultureInfo.InvariantCulture;
            Assert.NotNull(actual2, "Invariant #2 null");
            Assert.AreEqual(CultureInfo.InvariantCulture.LCID, actual2.LCID, "Invariant #2 LCID");

            Assert.AreSame(actual1, actual2, "Two invariants not the same");
        }

        [Test]
        public void TestNodaFormatInfo_get()
        {
            var actual = NodaCultureInfo.InvariantCulture.NodaFormatInfo;
            Assert.NotNull(actual);
        }

        [Test]
        public void TestNodaFormatInfo_set()
        {
            var info = new NodaCultureInfo("en-US");
            var original = info.NodaFormatInfo;
            Assert.NotNull(original);
            var newFormat = new NodaFormatInfo(Thread.CurrentThread.CurrentCulture);
            Assert.AreNotSame(original, newFormat);
            info.NodaFormatInfo = newFormat;
            Assert.AreSame(newFormat, info.NodaFormatInfo);
        }

        [Test]
        public void TestParent()
        {
            var info = new NodaCultureInfo("en-US");
            var actual = info.Parent;
            Assert.NotNull(actual);
            Assert.AreEqual("en", actual.Name);
        }

        [Test]
        public void TestToString()
        {
            const string name = "en-US";
            var info = new NodaCultureInfo(name);
            const string toString = "NodaCultureInfo: " + name;
            Assert.AreEqual(toString, info.ToString());
        }
    }
}
