// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Text.RegularExpressions;

namespace NodaTime.Tools.SetVersion
{
    internal sealed class ProjectVersion
    {
        private static readonly Regex Pattern = new Regex(@"^(\d+\.\d+)\.(\d)+(-.*)?$");

        /// <summary>
        /// The original input, e.g. 1.1.2-dev
        /// </summary>
        public string FullText { get; private set; }

        /// <summary>
        /// The dependency version, which is Major.Minor.0 for stable releases,
        /// and the full version text otherwise. (i.e. if there's a -foo part at the end,
        /// use the full text.)
        /// </summary>
        public string Dependency { get; private set; }

        /// <summary>
        /// The major/minor/patch version.
        /// So a full version of 1.1.2-dev would give 1.1.2.
        /// </summary>
        public string MajorMinorPatch { get; private set; }

        /// <summary>
        /// The major/minor version.
        /// So a full version of 1.1.2-dev would give 1.1.
        /// </summary>
        public string MajorMinor { get; private set; }

        internal ProjectVersion(string text)
        {
            Match match = Pattern.Match(text);
            if (!match.Success)
            {
                throw new ArgumentException("Failed to match pattern");
            }
            FullText = text;
            MajorMinor = match.Groups[1].Value;
            MajorMinorPatch = MajorMinor + "." + match.Groups[2];
            // For the first pre-release of any minor version, the dependency is just
            // the full version number; we have nothing else to depend on. For later
            // pre-releases and for stable versions, we can depend on Major.Minor.0.
            string patchBase = MajorMinor + ".0";
            if (FullText != MajorMinorPatch && patchBase == MajorMinorPatch)
            {
                Dependency = FullText;
            }
            else
            {
                Dependency = patchBase;
            }
        }
    }
}
