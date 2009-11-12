
namespace NodaTime
{
    /// <summary>
    /// A date and time with an associated chronology - or to look at it
    /// a different way, a LocalDateTime plus a time zone.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Some seemingly valid values do not represent a valid instant due to
    /// the local time moving forward during a daylight saving transition, thus "skipping"
    /// the given value. Other values occur twice (due to local time moving backward),
    /// in which case a ZonedDateTime will always represent the later of the two
    /// possible times, when converting it to an instant.
    /// </para>
    /// <para>
    /// A value constructed with "new ZonedDateTime()" will represent the Unix epoch
    /// in the ISO calendar system in the UTC time zone. That is the only situation in which
    /// the chronology is assumed rather than specified.
    /// </para>
    /// </remarks>
    public struct ZonedDateTime
    {
        private Instant instant;
        private Chronology chronology;

        public ZonedDateTime(LocalDateTime localDateTime, DateTimeZone zone)
        {
            instant = Instant.UnixEpoch;
            chronology = null;
        }

        public ZonedDateTime(Instant instant, Chronology chronology)
        {
            this.instant = Instant.UnixEpoch;
            this.chronology = null;
        }

        public ZonedDateTime(int year, int month, int day,
                             int hour, int minute, int second,
                             DateTimeZone zone)
        {
            instant = Instant.UnixEpoch;
            chronology = null;
        }

        public ZonedDateTime(int year, int month, int day,
                             int hour, int minute, int second,
                             Chronology chronology)
        {
            instant = Instant.UnixEpoch;
            this.chronology = null;
        }

        public ZonedDateTime(int year, int month, int day,
                             int hour, int minute, int second,
                             int millisecond, DateTimeZone zone)
        {
            instant = Instant.UnixEpoch;
            chronology = null;
        }

        public ZonedDateTime(int year, int month, int day,
                             int hour, int minute, int second,
                             int millisecond, Chronology chronology)
        {
            instant = Instant.UnixEpoch;
            this.chronology = null;
        }

        /// <summary>
        /// Converts this value to the instant it represents on the time line.
        /// If two instants are represented by the same set of values, the later
        /// instant is returned.
        /// </summary>
        /// <remarks>
        /// Conceptually this is a conversion (which is why it's not a property) but
        /// in reality the conversion is done at the point of construction.
        /// </remarks>
        public Instant ToInstant()
        {
            return instant;
        }

        /// <summary>
        /// Returns the chronology associated with this date and time.
        /// </summary>
        public Chronology Chronology { get { return chronology ?? Chronology.IsoUtc; } }

        public DateTimeZone Zone { get { return Chronology.Zone; } }
    }
}
