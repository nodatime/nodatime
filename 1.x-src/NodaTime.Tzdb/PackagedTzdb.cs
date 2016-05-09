// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime;
using NodaTime.TimeZones;
using System.IO;
using System.Reflection;

namespace NodaTime.Tzdb
{
    /// <summary>
    /// Holder class for the TZDB data in the <c>NodaTime.Tzdb</c>
    /// package, which is updated when IANA ships a new data release.
    /// </summary>
    public class PackagedTzdb
    {
        /// <summary>
        /// Gets a <see cref="TzdbDataTimeZoneSource" /> for the packaged TZDB data.
        /// This can be used for TZDB-specific operations such as inspecting location data.
        /// </summary>
        /// <value>The time zone source data packaged in <c>NodaTime.Tzdb</c>.</value>        
        public static TzdbDateTimeZoneSource Source { get; } = LoadDefaultDataSource();
        
        /// <summary>
        /// Gets an <see cref="IDateTimeZoneProvider" /> for the packaged TZDB data.
        /// </summary>
        /// <value>The time zone provider for the data packaged in <c>NodaTime.Tzdb</c>.</value>
        public static IDateTimeZoneProvider Provider { get; } = new DateTimeZoneCache(Source);

        private static TzdbDateTimeZoneSource LoadDefaultDataSource()
        {
            var assembly = typeof(PackagedTzdb).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("NodaTime.Tzdb.Tzdb.nzd"))
            {
                return TzdbDateTimeZoneSource.FromStream(stream);
            }
        }
    }
}