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
using NodaTime.Text;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class InstantTest
    {
        [Test, Category("Formatting"), Category("Format")]
        public void TestToString_InvalidFormat()
        {
            Assert.Throws<InvalidPatternException>(() => Instant.UnixEpoch.ToString("A"));
        }

        [Test, Category("Formatting"), Category("Format")]
        public void TestToString_MinValue()
        {
            TestToStringBase(Instant.MinValue, Instant.BeginningOfTimeLabel);
        }

        [Test, Category("Formatting"), Category("Format")]
        public void TestToString_MaxValue()
        {
            TestToStringBase(Instant.MaxValue, Instant.EndOfTimeLabel);
        }

        [Test, Category("Formatting"), Category("Format")]
        public void TestToString_UnixEpoch()
        {
            TestToStringBase(Instant.UnixEpoch, "1970-01-01T00:00:00Z");
        }

        [Test, Category("Formatting"), Category("Format")]
        public void TestToString_Padding()
        {
            TestToStringBase(Instant.FromUtc(1, 1, 1, 12, 34, 56), "0001-01-01T12:34:56Z");
        }

        private static void TestToStringBase(Instant value, string gvalue)
        {
            string actual = value.ToString();
            Assert.AreEqual(gvalue, actual);
            actual = value.ToString("G");
            Assert.AreEqual(gvalue, actual);
            actual = value.ToString("N");
            Assert.AreEqual(value.Ticks.ToString("N0"), actual);
            actual = value.ToString("N", CultureInfo.InvariantCulture);
            Assert.AreEqual(value.Ticks.ToString("N0", CultureInfo.InvariantCulture), actual);
            actual = value.ToString(CultureInfo.InvariantCulture);
            Assert.AreEqual(gvalue, actual);
            actual = value.ToString("D");
            Assert.AreEqual(value.Ticks.ToString("D"), actual);

            actual = string.Format("{0}", value);
            Assert.AreEqual(gvalue, actual);
            actual = string.Format("{0:G}", value);
            Assert.AreEqual(gvalue, actual);
            actual = string.Format("{0:N}", value);
            Assert.AreEqual(value.Ticks.ToString("N0"), actual);
            actual = string.Format("{0:D}", value);
            Assert.AreEqual(value.Ticks.ToString("D"), actual);
        }
    }
}
