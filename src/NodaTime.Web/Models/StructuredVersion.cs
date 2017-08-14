// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.using System;

using System;
using System.Text.RegularExpressions;

namespace NodaTime.Web.Models
{
    /// <summary>
    /// A structured version number: major/minor/patch/pre-release
    /// </summary>
    public class StructuredVersion : IComparable<StructuredVersion>, IEquatable<StructuredVersion>
    {
        // Allow up to 7 digits per part. That's enough for anything sensible, and will avoid overflow when parsing.
        private static readonly Regex VersionPattern = new Regex(@"^(?<major>\d{1,7})\.(?<minor>\d{1,7})\.(?<patch>\d{1,7})(?:-(?<pre>.+))?$");

        private readonly string text;
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        /// <summary>
        /// Prerelease part of the version, if any, e.g. "beta01". Null for GA versions.
        /// </summary>
        public string Prerelease { get; }

        public override string ToString() => text;

        public StructuredVersion(string version)
        {
            text = version; // TODO: Normalize?
            var match = VersionPattern.Match(version);
            if (!match.Success)
            {
                throw new ArgumentException($"String '{version}' isn't a valid version", nameof(version));
            }
            Major = int.Parse(match.Groups["major"].Value);
            Minor = int.Parse(match.Groups["minor"].Value);
            Patch = int.Parse(match.Groups["patch"].Value);
            Prerelease = match.Groups["pre"].Value;
            if (Prerelease == "")
            {
                Prerelease = null;
            }
        }

        public int CompareTo(StructuredVersion other) =>
            other == null ? -1
            : Major != other.Major ? Major.CompareTo(other.Major)
            : Minor != other.Minor ? Minor.CompareTo(other.Minor)
            : Patch != other.Patch ? Patch.CompareTo(other.Patch)
            : ComparePrereleases(Prerelease, other.Prerelease);

        // A null prerelease needs to come *before* a non-null one
        private static int ComparePrereleases(string x, string y) =>
            x == y ? 0
            : x == null ? 1
            : y == null ? -1
            : string.CompareOrdinal(x, y);

        public bool Equals(StructuredVersion other) => other != null &&
            other.Major == Major &&
            other.Minor == Minor &&
            other.Patch == Patch &&
            other.Prerelease == Prerelease;

        public override bool Equals(object obj) => Equals(obj as StructuredVersion);

        public override int GetHashCode() => (Major * 100 + Minor * 10 + Patch) ^ (Prerelease?.GetHashCode() ?? 0);
    }
}
