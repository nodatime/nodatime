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
        public static IDateTimeZoneProvider Tzdb { get { return TzdbHolder.TzdbImpl; } }

        // This class exists to force TZDB initialization to be lazy. We don't want using
        // DateTimeZoneProviders.Bcl to force a read/parse.
        private static class TzdbHolder
        {
            // See http://csharpindepth.com/Articles/General/BeforeFieldInit.aspx
            static TzdbHolder() {}
            internal static readonly DateTimeZoneCache TzdbImpl = new DateTimeZoneCache(TzdbDateTimeZoneSource.Default);
        }

#if !PCL
        /// <summary>
        /// Gets the TZDB time zone provider.
        /// This always returns the same value as the <see cref="Tzdb"/> property.
        /// </summary>
        /// <remarks>This method is not available in the PCL version, as it was made obsolete in Noda Time 1.1.</remarks>
        /// <seealso cref="Tzdb"/>
        [Obsolete("Use DateTimeZoneProviders.Tzdb instead")]
        public static IDateTimeZoneProvider Default { get { return Tzdb; } }

        private static readonly DateTimeZoneCache bclFactory = new DateTimeZoneCache(new BclDateTimeZoneSource());

        /// <summary>
        /// Gets a time zone provider which uses a <see cref="BclDateTimeZoneSource"/>.
        /// This property is not available on the PCL build of Noda Time.
        /// </summary>
        public static IDateTimeZoneProvider Bcl { get { return bclFactory; } }
#endif

        private static readonly object XmlSerializationProviderLock = new object();
        private static IDateTimeZoneProvider xmlSerializationProvider;

        /// <summary>
        /// The <see cref="IDateTimeZoneProvider"/> to use to interpret a time zone ID read as part of
        /// XML serialization.
        /// </summary>
        /// <remarks>
        /// This property defaults to <see cref="DateTimeZoneProviders.Tzdb"/>. The mere existence of
        /// this property is unfortunate, but XML serialization in .NET provides no way of configuring
        /// appropriate context. It is expected that any single application is unlikely to want to serialize
        /// <c>ZonedDateTime</c> values using different time zone providers.
        /// </remarks>
        public static IDateTimeZoneProvider XmlSerialization
        {
            get
            {
                lock (XmlSerializationProviderLock)
                {
                    return xmlSerializationProvider ?? (xmlSerializationProvider = DateTimeZoneProviders.Tzdb);
                }
            }
            set
            {
                lock (XmlSerializationProviderLock)
                {
                    xmlSerializationProvider = Preconditions.CheckNotNull(value, "value");
                }
            }
        }
    }
}
