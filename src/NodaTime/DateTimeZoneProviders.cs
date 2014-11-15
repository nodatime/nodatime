// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.TimeZones;
using NodaTime.Utility;

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
            // See http://csharpindepth.com/Articles/General/BeforeFieldInit.aspx
            static TzdbHolder() {}
            internal static readonly DateTimeZoneCache TzdbImpl = new DateTimeZoneCache(TzdbDateTimeZoneSource.Default);
        }

#if !PCL
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
        /// This property is not available on the PCL build of Noda Time.
        /// </summary>
        /// <remarks>
        /// <para>See note on <see cref="BclDateTimeZone"/> for details of some incompatibilities with the BCL.</para>
        /// </remarks>
        /// <value>A time zone provider which uses a <c>BclDateTimeZoneSource</c>.</value>
        public static IDateTimeZoneProvider Bcl => BclHolder.BclImpl;
#endif

        private static readonly object SerializationProviderLock = new object();
        private static IDateTimeZoneProvider serializationProvider;

        /// <summary>
        /// Gets the <see cref="IDateTimeZoneProvider"/> to use to interpret a time zone ID read as part of
        /// XML or binary serialization.
        /// </summary>
        /// <remarks>
        /// This property defaults to <see cref="DateTimeZoneProviders.Tzdb"/>. The mere existence of
        /// this property is unfortunate, but XML and binary serialization in .NET provide no simple way of configuring
        /// appropriate context. It is expected that any single application is unlikely to want to serialize
        /// <c>ZonedDateTime</c> values using different time zone providers.
        /// </remarks>
        /// <value>The <c>IDateTimeZoneProvider</c> to use to interpret a time zone ID read as part of
        /// XML or binary serialization.</value>
        public static IDateTimeZoneProvider Serialization
        {
            get
            {
                lock (SerializationProviderLock)
                {
                    return serializationProvider ?? (serializationProvider = Tzdb);
                }
            }
            set
            {
                lock (SerializationProviderLock)
                {
                    serializationProvider = Preconditions.CheckNotNull(value, "value");
                }
            }
        }
    }
}
