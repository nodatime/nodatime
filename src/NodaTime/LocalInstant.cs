namespace NodaTime
{
    /// <summary>
    /// Represents a local date and time without reference to a calendar system,
    /// as the number of ticks since the Unix epoch which would represent that time
    /// of the same date in UTC. This needs a better description, and possibly a better name
    /// at some point...
    /// </summary>
    public struct LocalInstant
    {
        public static readonly LocalInstant LocalUnixEpoch = new LocalInstant(0);

        private readonly long ticks;

        /// <summary>
        /// Ticks since the Unix epoch.
        /// </summary>
        public long Ticks { get { return ticks; } }

        public LocalInstant(long ticks)
        {
            this.ticks = ticks;
        }
    }
}
