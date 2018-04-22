// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones.Cldr;
using NodaTime.TzdbCompiler.Tzdb;
using NUnit.Framework;
using System.Xml.Linq;

namespace NodaTime.TzdbCompiler.Test.Tzdb
{
    public class CldrWindowsZonesParserTest
    {
        [Test]
        public void Versions_Present()
        {
            string xml = @"
<supplementalData>
  <version number='$Revision: rev-version $'/>
  <generation date='$Date: 2012-01-14 03:54:32 -0600 (Sat, 14 Jan 2012) $' />
  <windowsZones>
    <mapTimezones otherVersion='win-version' typeVersion='tzdb-version'>
    </mapTimezones>
  </windowsZones>
</supplementalData>
    ";
            var doc = XDocument.Parse(xml);
            var parsed = CldrWindowsZonesParser.Parse(doc);
            Assert.AreEqual("rev-version", parsed.Version);
            Assert.AreEqual("win-version", parsed.WindowsVersion);
            Assert.AreEqual("tzdb-version", parsed.TzdbVersion);
        }

        [Test]
        public void Versions_Absent()
        {
            string xml = @"
<supplementalData>
  <generation date='$Date: 2012-01-14 03:54:32 -0600 (Sat, 14 Jan 2012) $' />
  <windowsZones>
    <mapTimezones>
    </mapTimezones>
  </windowsZones>
</supplementalData>
    ";
            var doc = XDocument.Parse(xml);
            var parsed = CldrWindowsZonesParser.Parse(doc);
            Assert.AreEqual("", parsed.Version);
            Assert.AreEqual("", parsed.WindowsVersion);
            Assert.AreEqual("", parsed.TzdbVersion);
        }

        [Test]
        public void MapZones()
        {
            string xml = @"
<supplementalData>
  <windowsZones>
    <mapTimezones>
      <mapZone other='X' territory='t0' type='' />
      <mapZone other='X' territory='t1' type='A' />
      <mapZone other='X' territory='t2' type='B C' />
    </mapTimezones>
  </windowsZones>
</supplementalData>
    ";
            var doc = XDocument.Parse(xml);
            var parsed = CldrWindowsZonesParser.Parse(doc);
            var mapZones = parsed.MapZones;
            Assert.AreEqual(3, mapZones.Count);
            Assert.AreEqual(new MapZone("X", "t0", new string[0]), mapZones[0]);
            Assert.AreEqual(new MapZone("X", "t1", new[] { "A" }), mapZones[1]);
            Assert.AreEqual(new MapZone("X", "t2", new[] { "B", "C" }), mapZones[2]);
        }
    }
}
