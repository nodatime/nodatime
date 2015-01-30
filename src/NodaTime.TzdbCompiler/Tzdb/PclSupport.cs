// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;
using System.Collections.Generic;

namespace NodaTime.TzdbCompiler.Tzdb
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
                { "Russia TZ 1 Standard Time", "Kaliningrad Standard Time" },
                { "Russia TZ 2 Standard Time", "Russian Standard Time" },
                { "Russia TZ 4 Standard Time", "Ekaterinburg Standard Time" },
                { "Russia TZ 6 Standard Time", "North Asia Standard Time" },
                { "Russia TZ 7 Standard Time", "North Asia East Standard Time" },
                { "Russia TZ 8 Standard Time", "Yakutsk Standard Time" },
                { "Russia TZ 9 Standard Time", "Vladivostok Standard Time" },
                { "Cabo Verde Standard Time", "Cape Verde Standard Time" },
                // The following name/ID mappings give an ID which then isn't present in CLDR
                // (at least in v26 data); these zones will have to be mapped by transitions
                // in the PCL.
                // { "Russia TZ 3 Standard Time", "Russia Time Zone 3" },
                // { "Russia TZ 5 Standard Time", "N.Central Asia Standard Time" },
                // { "Russia TZ 10 Standard Time", "Russia Time Zone 10" },
                // { "Russia TZ 11 Standard Time", "Russia Time Zone 11" },
    });
    }
}
