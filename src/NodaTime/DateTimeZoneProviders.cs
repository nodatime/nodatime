using NodaTime.TimeZones;

namespace NodaTime
{
    /// <summary>
    /// Known NodaTime date/time zone providers.
    /// </summary>
    public static class DateTimeZoneProviders
    {
        private static readonly DateTimeZoneCache tzdbFactory = new DateTimeZoneCache(new TzdbDateTimeZoneSource("NodaTime.TimeZones.Tzdb"));
        private static readonly DateTimeZoneCache bclFactory = new DateTimeZoneCache(new BclDateTimeZoneSource());

        /// <summary>
        /// Gets the default time zone provider, which is initialized from resources within the NodaTime assembly.
        /// </summary>
        public static IDateTimeZoneProvider Default { get { return Tzdb; } }

        /// <summary>
        /// Gets a time zone provider which uses the <see cref="TzdbDateTimeZoneSource"/>.
        /// </summary>
        public static IDateTimeZoneProvider Tzdb { get { return tzdbFactory; } }

        /// <summary>
        /// Gets a time zone provider which uses the <see cref="BclDateTimeZoneSource"/>.
        /// </summary>
        public static IDateTimeZoneProvider Bcl { get { return bclFactory; } }
    }
}
