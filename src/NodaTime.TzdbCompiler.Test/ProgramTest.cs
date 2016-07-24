// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones.Cldr;
using NUnit.Framework;

namespace NodaTime.TzdbCompiler.Test
{
    public class ProgramTest
    {
        [Test]
        public void MergeWindowsZones()
        {
            var originalZones = new WindowsZones("orig-version", "orig-tzdb-version", "orig-win-version",
                new[]
                {
                    new MapZone("ID1", "001", new[] { "A" }),
                    new MapZone("ID1", "T1-1", new[] { "B", "C" }),
                    new MapZone("ID1", "T1-2", new[] { "D" }),
                    new MapZone("ID2", "001", new[] { "E" }),
                    new MapZone("ID2", "T2-1", new[] { "F" })
                });

            var overrideZones = new WindowsZones("override-version", "", "",
                new[]
                {
                    // Replace ID1 / 001
                    new MapZone("ID1", "001", new[] { "A1" }),
                    // Delete ID1 / T1-1
                    new MapZone("ID1", "T1-1", new string[0]),
                    // (Leave ID1 / T1-2)
                    // Add ID1 / T1-3
                    new MapZone("ID1", "T1-3", new[] { "B1", "C1", "G1" }),
                    // (Leave ID2 / 001)
                    // Replace ID2 / T2-1
                    new MapZone("ID2", "T2-1", new[] { "H1" }),
                    // Add ID3 / 001
                    new MapZone("ID3", "001", new[] { "I1" })
                });
            var actual = Program.MergeWindowsZones(originalZones, overrideZones);
            var expected = new WindowsZones("override-version", "orig-tzdb-version", "orig-win-version",
                new[]
                {
                    new MapZone("ID1", "001", new[] { "A1" }),
                    new MapZone("ID1", "T1-2", new[] { "D" }),
                    new MapZone("ID1", "T1-3", new[] { "B1", "C1", "G1" }),
                    new MapZone("ID2", "001", new[] { "E" }),
                    new MapZone("ID2", "T2-1", new[] { "H1" }),
                    new MapZone("ID3", "001", new[] { "I1" })
                });

            // Could implement IEquatable<WindowsZones>, but it doesn't seem worth it right now.
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.TzdbVersion, actual.TzdbVersion);
            Assert.AreEqual(expected.WindowsVersion, actual.WindowsVersion);
            Assert.AreEqual(expected.MapZones, actual.MapZones);
        }
    }
}
