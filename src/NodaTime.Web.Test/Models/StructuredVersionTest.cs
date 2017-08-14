// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.using System;

using NodaTime.Web.Models;
using NUnit.Framework;
using System;

namespace NodaTime.Web.Test.Models
{
    public class StructuredVersionTest
    {
        [Test]
        [TestCase("1.0.0", "1.0.1")]
        [TestCase("1.0.0", "1.1.0")]
        [TestCase("1.1.0", "2.0.0")]
        [TestCase("1.0.0", "1.0.1-alpha01")]
        [TestCase("1.0.0-alpha01", "1.0.0-alpha02")]
        [TestCase("1.0.0-alpha01", "1.0.0-beta01")]
        [TestCase("1.0.0-alpha01", "1.0.0")]
        [TestCase("2.0.0", "10.0.0")]
        [TestCase("1.2.0", "1.10.0")]
        [TestCase("1.0.2", "1.0.10")]
        public void Compare(string earlier, string later)
        {
            StructuredVersion earlierVersion = new StructuredVersion(earlier);
            StructuredVersion laterVersion = new StructuredVersion(later);
            Assert.That(earlierVersion.CompareTo(laterVersion), Is.LessThan(0));
            Assert.That(laterVersion.CompareTo(earlierVersion), Is.GreaterThan(0));
            Assert.AreNotEqual(laterVersion, earlierVersion);
            Assert.AreNotEqual(laterVersion.GetHashCode(), earlierVersion.GetHashCode());
        }

        [Test]
        [TestCase("1.2.3")]
        [TestCase("1.2.3-alpha01")]
        public void Compare_Equal(string value)
        {
            StructuredVersion v1 = new StructuredVersion(value);
            StructuredVersion v2 = new StructuredVersion(value);
            Assert.AreEqual(0, v1.CompareTo(v2));
            Assert.AreEqual(v1, v2);
            Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
        }

        [Test]
        [TestCase("1.2.3", 1, 2, 3, null)]
        [TestCase("11.22.33-xyz", 11, 22, 33, "xyz")]
        public void Construction(string version, int major, int minor, int patch, string prerelease)
        {
            var structured = new StructuredVersion(version);
            Assert.AreEqual(major, structured.Major);
            Assert.AreEqual(minor, structured.Minor);
            Assert.AreEqual(patch, structured.Patch);
            Assert.AreEqual(prerelease, structured.Prerelease);
        }

        [Test]
        [TestCase("")]
        [TestCase("abc")]
        [TestCase("1")]
        [TestCase("1.0")]
        [TestCase("1.0.0+other")]
        [TestCase("1.0.0other")]
        [TestCase("1.0.0-")]
        [TestCase("1.0.9999999999999999")]
        [TestCase("x1.0.0y")]
        public void Construction_Invalid(string version)
        {
            Assert.Throws<ArgumentException>(() => new StructuredVersion(version));
        }
    }
}
