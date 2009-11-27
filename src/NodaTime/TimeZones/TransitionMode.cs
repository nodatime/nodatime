
namespace NodaTime.TimeZones
{
    /// <summary>
    /// Specifies how transitions are calculated.
    /// </summary>
    public enum TransitionMode
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
