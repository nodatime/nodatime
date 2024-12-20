﻿// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NodaTime.TzdbCompiler.Tzdb
{
    /// <summary>
    /// Extra support required for the PCL builds of Noda Time 1.x, which has a somewhat anaemic
    /// version of TimeZoneInfo. These aren't used in Noda Time 2.x, as netstandard1.3 supports
    /// the Id property.
    /// </summary>
    internal static class NameIdMappingSupport
    {
        /// <summary>
        /// Hard-coded map from StandardName to Id, where these are known to differ.
        /// The unit test for this member checks that the system we're running on doesn't
        /// introduce any new or different mappings, but allows this mapping to be a superset
        /// of the detected one.
        /// </summary>
        internal static ReadOnlyDictionary<string, string> StandardNameToIdMap =
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
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
                { "West Bank Gaza Standard Time", "West Bank Standard Time" },
                { "Novosibirsk Standard Time", "N. Central Asia Standard Time" },
                { "Türkiye Standard Time", "Turkey Standard Time" },
                // Difference here is just the case of "and"!
                { "Turks and Caicos Standard Time", "Turks And Caicos Standard Time" },
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
