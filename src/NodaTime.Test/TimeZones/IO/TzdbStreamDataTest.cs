// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones.Cldr;
using NodaTime.TimeZones.IO;
using NodaTime.Utility;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace NodaTime.Test.TimeZones.IO
{
    public class TzdbStreamDataTest
    {
        [Test]
        public void Minimal()
        {
            var data = new TzdbStreamData(CreateMinimalBuilder());
            Assert.NotNull(data.TzdbVersion);
        }

        [Test]
        public void MissingStringPool()
        {
            var builder = CreateMinimalBuilder();
            builder.stringPool = null;
            Assert.Throws<InvalidNodaDataException>(() => new TzdbStreamData(builder));
        }

        [Test]
        public void MissingTzbdAliasMap()
        {
            var builder = CreateMinimalBuilder();
            builder.tzdbIdMap = null;
            Assert.Throws<InvalidNodaDataException>(() => new TzdbStreamData(builder));
        }

        [Test]
        public void MissingTzdbVersion()
        {
            var builder = CreateMinimalBuilder();
            builder.tzdbVersion = null;
            Assert.Throws<InvalidNodaDataException>(() => new TzdbStreamData(builder));
        }

        [Test]
        public void MissingWindowsMapping()
        {
            var builder = CreateMinimalBuilder();
            builder.windowsMapping = null;
            Assert.Throws<InvalidNodaDataException>(() => new TzdbStreamData(builder));
        }

        [Test]
        public void InvalidVersion()
        {
            // It's hard to create a stream that's valid apart from the version, so we'll just
            // give one with an invalid version and check that it looks like the right message.
            var stream = new MemoryStream(new byte[] { 0, 0, 0, 1 });
            var exception = Assert.Throws<InvalidNodaDataException>(() => TzdbStreamData.FromStream(stream));
            Assert.IsTrue(exception.Message.Contains("version"));
        }

        private static TzdbStreamData.Builder CreateMinimalBuilder() =>
            new TzdbStreamData.Builder
            {
                stringPool = new List<string>(),
                tzdbIdMap = new Dictionary<string, string>(),
                tzdbVersion = "tzdb-version",
                windowsMapping = new WindowsZones("cldr-version", "tzdb-version", "windows-version", new MapZone[0]),
            };
    }
}
