using NodaTime.TimeZones;

namespace NodaTime
{
    /// <summary>
    /// Static access to date/time zone providers built into Noda Time. These are all thread-safe and caching.
    /// </summary>
    public static class DateTimeZoneProviders
    {
// We'll want to expose at least TZDB of course...
#if PCL
        public static IDateTimeZoneProvider Tzdb { get { return null; } }
        public static IDateTimeZoneProvider Default { get { return null; } }
#else
        private static readonly DateTimeZoneCache tzdbFactory = new DateTimeZoneCache(TzdbDateTimeZoneSource.Default);
        private static readonly DateTimeZoneCache bclFactory = new DateTimeZoneCache(new BclDateTimeZoneSource());

        /// <summary>
        /// Gets the default time zone provider, which is initialized from resources within the NodaTime assembly.
        /// </summary>
        /// <remarks>
        /// Currently this returns the same value as the <see cref="Tzdb"/> property.
        /// </remarks>
        public static IDateTimeZoneProvider Default { get { return Tzdb; } }

        /// <summary>
        /// Gets a time zone provider which uses a <see cref="TzdbDateTimeZoneSource"/>.
        /// The underlying source is <see cref="TzdbDateTimeZoneSource.Default"/>, which is initialized from
        /// resources within the NodaTime assembly.
        /// </summary>
        public static IDateTimeZoneProvider Tzdb { get { return tzdbFactory; } }

        /// <summary>
        /// Gets a time zone provider which uses a <see cref="BclDateTimeZoneSource"/>.
        /// </summary>
        public static IDateTimeZoneProvider Bcl { get { return bclFactory; } }
#endif
    }
}
