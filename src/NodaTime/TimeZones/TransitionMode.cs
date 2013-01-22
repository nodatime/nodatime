namespace NodaTime.TimeZones
{
    /// <summary>
    /// Specifies how transitions are calculated. Whether relative to UTC, the time zones standard
    /// offset, or the wall (or daylight savings) offset.
    /// </summary>
    internal enum TransitionMode
    {
        /// <summary>
        /// Calculate transitions against UTC.
        /// </summary>
        Utc,

        /// <summary>
        /// Calculate transitions against wall offset.
        /// </summary>
        Wall,

        /// <summary>
        /// Calculate transitions against standard offset.
        /// </summary>
        Standard
    }
}