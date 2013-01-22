namespace NodaTime.TimeZones
{
    /// <summary>
    /// The core part of a DateTimeZone: mapping an Instant to an Interval.
    /// Separating this out into an interface allows for flexible caching.
    /// </summary>
    // TODO: Investigate whether a delegate would be faster.
    internal interface IZoneIntervalMap
    {
        ZoneInterval GetZoneInterval(Instant instant);
    }
}
