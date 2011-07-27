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
using System;
using System.Globalization;
using NodaTime.Format;
using NUnit.Framework;
#endregion

namespace NodaTime.Test
{
    [TestFixture]
    public partial class InstantTest
    {
        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestParse_BadValue()
        {
            Assert.Catch<FormatException>(() => Instant.Parse("ads"));
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestParse_D()
        {
            var actual = Instant.Parse(threeMillion.Ticks.ToString("D"));
            Assert.AreEqual(threeMillion, actual);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestParse_G()
        {
            var actual = Instant.Parse("1970-01-01T00:00:00Z");
            Assert.AreEqual(Instant.UnixEpoch, actual);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestParse_N()
        {
            var actual = Instant.Parse(threeMillion.Ticks.ToString("N0"));
            Assert.AreEqual(threeMillion, actual);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestParse_N_extraSpace()
        {
            Assert.Catch<FormatException>(() => Instant.Parse(" " + threeMillion.Ticks.ToString("N0")));
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestParse_N_frFR()
        {
            var frFr = new CultureInfo("fr-FR");
            var actual = Instant.Parse(threeMillion.Ticks.ToString("N0", frFr), frFr);
            Assert.AreEqual(threeMillion, actual);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestParse_N_leadingSpace_Flaged()
        {
            var actual = Instant.Parse(" " + threeMillion.Ticks.ToString("N0"), null, DateTimeParseStyles.AllowLeadingWhite);
            Assert.AreEqual(threeMillion, actual);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestParse_N_trailingSpace_Flaged()
        {
            var actual = Instant.Parse(threeMillion.Ticks.ToString("N0") + " ", null, DateTimeParseStyles.AllowTrailingWhite);
            Assert.AreEqual(threeMillion, actual);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestParse_null()
        {
            Assert.Throws<ArgumentNullException>(() => Instant.Parse(null));
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParseExact_D()
        {
            Instant result;
            Assert.IsTrue(Instant.TryParseExact(Int64.MinValue.ToString("D"), "d", null, DateTimeParseStyles.None, out result));
            Assert.AreEqual(Instant.MinValue, result);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParseExact_FormatListEmpty()
        {
            Instant result;
            Assert.IsFalse(Instant.TryParseExact("0", new string[] { }, null, DateTimeParseStyles.None, out result));
            Assert.AreEqual(Instant.MinValue, result);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParseExact_G()
        {
            Instant result;
            Assert.IsTrue(Instant.TryParseExact("1970-01-01T00:00:00Z", "g", null, DateTimeParseStyles.None, out result));
            Assert.AreEqual(Instant.UnixEpoch, result);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParseExact_G_BOT()
        {
            Instant result;
            Assert.IsTrue(Instant.TryParseExact("bot", "g", null, DateTimeParseStyles.None, out result));
            Assert.AreEqual(Instant.MinValue, result);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParseExact_G_EOT()
        {
            Instant result;
            Assert.IsTrue(Instant.TryParseExact("eot", "g", null, DateTimeParseStyles.None, out result));
            Assert.AreEqual(Instant.MaxValue, result);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParseExact_InvalidFormat()
        {
            Instant result;
            Assert.IsFalse(Instant.TryParseExact("0", "Q", null, DateTimeParseStyles.None, out result));
            Assert.AreEqual(Instant.MinValue, result);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParseExact_N()
        {
            Instant result;
            Assert.IsTrue(Instant.TryParseExact(threeMillion.Ticks.ToString("N0"), "n", null, DateTimeParseStyles.None, out result));
            Assert.AreEqual(threeMillion, result);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParseExact_NG()
        {
            Instant result;
            Assert.IsTrue(Instant.TryParseExact("1970-01-01T00:00:00Z", new[] { "n", "g" }, null, DateTimeParseStyles.None, out result));
            Assert.AreEqual(Instant.UnixEpoch, result);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParseExact_N_BadString()
        {
            Instant result;
            Assert.IsFalse(Instant.TryParseExact("asdf", "n", null, DateTimeParseStyles.None, out result));
            Assert.AreEqual(Instant.MinValue, result);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParseExact_N_fr()
        {
            var frFr = new CultureInfo("fr-FR");
            Instant result;
            Assert.IsTrue(Instant.TryParseExact(threeMillion.Ticks.ToString("N0", frFr), "n", frFr, DateTimeParseStyles.None, out result));
            Assert.AreEqual(threeMillion, result);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParseExact_N_null()
        {
            Instant result;
            Assert.Throws<ArgumentNullException>(() => Instant.TryParseExact(null, "n", null, DateTimeParseStyles.None, out result));
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParseExact_NullFormat()
        {
            Instant result;
            Assert.Throws<ArgumentNullException>(() => Instant.TryParseExact("0", (string)null, null, DateTimeParseStyles.None, out result));
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParseExact_NullFormatList()
        {
            Instant result;
            Assert.Throws<ArgumentNullException>(() => Instant.TryParseExact("0", (string[])null, null, DateTimeParseStyles.None, out result));
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParseExact_ValidValue_WrongFormat()
        {
            Instant result;
            Assert.IsFalse(Instant.TryParseExact("1970-01-01T00:00:00Z", "n", null, DateTimeParseStyles.None, out result));
            Assert.AreEqual(Instant.MinValue, result);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParse_BadValue()
        {
            Instant result;
            Assert.IsFalse(Instant.TryParse("ads", null, DateTimeParseStyles.None, out result));
            Assert.AreEqual(Instant.MinValue, result);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParse_D()
        {
            Instant result;
            Assert.IsTrue(Instant.TryParse(threeMillion.Ticks.ToString("D"), null, DateTimeParseStyles.None, out result));
            Assert.AreEqual(threeMillion, result);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParse_G()
        {
            Instant result;
            Assert.IsTrue(Instant.TryParse("1970-01-01T00:00:00Z", null, DateTimeParseStyles.None, out result));
            Assert.AreEqual(Instant.UnixEpoch, result);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParse_N()
        {
            Instant result;
            Assert.IsTrue(Instant.TryParse(threeMillion.Ticks.ToString("N0"), null, DateTimeParseStyles.None, out result));
            Assert.AreEqual(threeMillion, result);
        }

        [Test]
        [Category("Formating")]
        [Category("Parse")]
        public void TestTryParse_null()
        {
            Instant result;
            Assert.Throws<ArgumentNullException>(() => Instant.TryParse(null, null, DateTimeParseStyles.None, out result));
        }
    }
}