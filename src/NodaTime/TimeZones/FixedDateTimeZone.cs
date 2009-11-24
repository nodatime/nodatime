
namespace NodaTime.TimeZones
{
    /// <summary>
    /// Basic <see cref="IDateTimeZone" /> implementation that has a fixed name key and offset.
    /// </summary>
    /// <remarks>
    /// This type is thread-safe and immutable.
    /// </remarks>
    public sealed class FixedDateTimeZone : IDateTimeZone
    {
        private readonly Duration offset;
        private readonly string id;

        /// <summary>
        /// Creates a new fixed time zone.
        /// </summary>
        /// <param name="id">The ID of the time zone.</param>
        /// <param name="offset">The offset from UTC. A positive duration indicates that the local time is later than UTC.</param>
        public FixedDateTimeZone(string id, Duration offset)
        {
            this.id = id;
            this.offset = offset;
        }

        public Instant? NextTransition(Instant instant)
        {
            return null;
        }

        public Instant? PreviousTransition(Instant instant)
        {
            return null;
        }

        public Duration GetOffsetFromUtc(Instant instant)
        {
            return offset;
        }

        public Duration GetOffsetFromLocal(LocalDateTime localTime)
        {
            return offset;
        }

        public string Id
        {
            get { return id; }
        }

        public bool IsFixed
        {
            get { return true; }
        }
    }
}
