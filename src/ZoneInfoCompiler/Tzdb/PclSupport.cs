// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;
using System.Collections.Generic;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    /// Extra support required for the PCL, which has a somewhat anaemic
    /// version of TimeZoneInfo.
    /// </summary>
    internal static class PclSupport
    {
        /// <summary>
        /// Hard-coded map from StandardName to Id, where these are known to differ.
        /// The unit test for this member checks that the system we're running on doesn't
        /// introduce any new or different mappings, but allows this mapping to be a superset
        /// of the detected one.
        /// </summary>
        internal static NodaReadOnlyDictionary<string, string> StandardNameToIdMap =
            new NodaReadOnlyDictionary<string, string>(new Dictionary<string, string>
            {
                { "Coordinated Universal Time", "UTC" },
                { "Co-ordinated Universal Time", "UTC" },
                { "Jerusalem Standard Time", "Israel Standard Time" },
                { "Malay Peninsula Standard Time" , "Singapore Standard Time" },
            });
    }
}
