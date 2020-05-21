// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using NodaTime.Utility;
using System;

namespace NodaTime
{
    /// <summary>
    /// Static access to date/time zone providers built into Noda Time and for global configuration where this is unavoidable.
    /// All properties are thread-safe, and the providers returned by the read-only properties cache their results.
    /// </summary>
    public static class DateTimeZoneProviders
    {
        /// <summary>
        /// Gets a time zone provider which uses a <see cref="TzdbDateTimeZoneSource"/>.
        /// The underlying source is <see cref="TzdbDateTimeZoneSource.Default"/>, which is initialized from
        /// resources within the NodaTime assembly.
        /// </summary>
        /// <value>A time zone provider using a <c>TzdbDateTimeZoneSource</c>.</value>
        public static IDateTimeZoneProvider Tzdb => TzdbHolder.TzdbImpl;

        // This class exists to force TZDB initialization to be lazy. We don't want using
        // DateTimeZoneProviders.Bcl to force a read/parse of TZDB data.
        private static class TzdbHolder
        {
            // See https://csharpindepth.com/Articles/BeforeFieldInit
            static TzdbHolder() {}
            internal static readonly DateTimeZoneCache TzdbImpl = new DateTimeZoneCache(TzdbDateTimeZoneSource.Default);
        }

        // As per TzDbHolder above, this exists to defer construction of a BCL provider until needed.
        // While BclDateTimeZoneSource itself is lightweight, DateTimeZoneCache still does a non-trivial amount of work
        // on initialisation.
        private static class BclHolder
        {
            static BclHolder() {}
            internal static readonly DateTimeZoneCache BclImpl = new DateTimeZoneCache(new BclDateTimeZoneSource());
        }

        /// <summary>
        /// Gets a time zone provider which uses a <see cref="BclDateTimeZoneSource"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// See note on <see cref="BclDateTimeZone"/> for details of some incompatibilities with the BCL.
        /// </para>
        /// <para>
        /// In Noda Time 1.x and 2.x, this property is only available on the .NET Framework builds of Noda Time, and not
        /// the PCL (Noda Time 1.x) or .NET Standard 1.3 (Noda Time 2.x) builds.
        /// </para>
        /// </remarks>
        /// <value>A time zone provider which uses a <c>BclDateTimeZoneSource</c>.</value>
        public static IDateTimeZoneProvider Bcl => BclHolder.BclImpl;

        /// <summary>
        /// Gets the <see cref="IDateTimeZoneProvider"/> to use to interpret a time zone ID read as part of
        /// XML serialization. This property is obsolete as of version 3.0; the functionality still exists
        /// in <see cref="Xml.XmlSerializationSettings.DateTimeZoneProvider"/>, which this property delegates
        /// to. (The behavior has not changed; this is purely an exercise in moving/renaming.)
        /// </summary>
        [Obsolete("This property exists primarily for binary backward compatibility. Please use NodaTime.Xml.XmlSerializationSettings.DateTimeZoneProvider instead.")]
        public static IDateTimeZoneProvider Serialization
        {
            get => Xml.XmlSerializationSettings.DateTimeZoneProvider;
            set => Xml.XmlSerializationSettings.DateTimeZoneProvider = value;
        }
    }
}
