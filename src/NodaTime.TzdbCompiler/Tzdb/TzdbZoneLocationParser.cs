// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using NodaTime.Utility;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.TzdbCompiler.Tzdb
{
    /// <summary>
    /// Separate class for parsing zone location data from zone.tab and iso3166.tab.
    /// Stateless, simply static methods - only separated from TzdbZoneInfoCompiler to make the
    /// organization a little cleaner.
    /// </summary>
    internal static class TzdbZoneLocationParser
    {
        internal static TzdbZoneLocation ParseLocation(string line, Dictionary<string, string> countryMapping)
        {
            string[] bits = line.Split('\t');
            Preconditions.CheckArgument(bits.Length == 3 || bits.Length == 4, nameof(line), "Line must have 3 or 4 tab-separated values");
            string countryCode = bits[0];
            string countryName = countryMapping[countryCode];
            int[] latLong = ParseCoordinates(bits[1]);
            string zoneId = bits[2];
            string comment = bits.Length == 4 ? bits[3] : "";
            return new TzdbZoneLocation(latLong[0], latLong[1], countryName, countryCode, zoneId, comment);
        }

        internal static TzdbZone1970Location ParseEnhancedLocation(string line, Dictionary<string, TzdbZone1970Location.Country> countryMapping)
        {
            string[] bits = line.Split('\t');
            Preconditions.CheckArgument(bits.Length == 3 || bits.Length == 4, nameof(line), "Line must have 3 or 4 tab-separated values");
            string countryCodes = bits[0];
            var countries = countryCodes.Split(',').Select(code => countryMapping[code]).ToList();
            int[] latLong = ParseCoordinates(bits[1]);
            string zoneId = bits[2];
            string comment = bits.Length == 4 ? bits[3] : "";
            return new TzdbZone1970Location(latLong[0], latLong[1], countries, zoneId, comment);
        }

        // Internal for testing
        /// <summary>
        /// Parses a string such as "-7750+16636" or "+484531-0913718" into a pair of Int32
        /// values: the latitude and longitude of the coordinates, in seconds.
        /// </summary>
        internal static int[] ParseCoordinates(string text)
        {
            Preconditions.CheckArgument(text.Length == 11 || text.Length == 15, "point", "Invalid coordinates");
            int latDegrees;
            int latMinutes;
            int latSeconds = 0;
            int latSign;
            int longDegrees;
            int longMinutes;
            int longSeconds = 0;
            int longSign;

            if (text.Length == 11 /* +-DDMM+-DDDMM */)
            {
                latSign = text[0] == '-' ? -1 : 1;
                latDegrees = int.Parse(text.Substring(1, 2));
                latMinutes = int.Parse(text.Substring(3, 2));
                longSign = text[5] == '-' ? -1 : 1;
                longDegrees = int.Parse(text.Substring(6, 3));
                longMinutes = int.Parse(text.Substring(9, 2));
            }
            else /* +-DDMMSS+-DDDMMSS */
            {
                latSign = text[0] == '-' ? -1 : 1;
                latDegrees = int.Parse(text.Substring(1, 2));
                latMinutes = int.Parse(text.Substring(3, 2));
                latSeconds = int.Parse(text.Substring(5, 2));
                longSign = text[7] == '-' ? -1 : 1;
                longDegrees = int.Parse(text.Substring(8, 3));
                longMinutes = int.Parse(text.Substring(11, 2));
                longSeconds = int.Parse(text.Substring(13, 2));
            }
            return new[] {
                latSign * (latDegrees * 3600 + latMinutes * 60 + latSeconds),
                longSign * (longDegrees * 3600 + longMinutes * 60 + longSeconds)
            };
        }
    }
}
