namespace NodaTime.Converters
{
    /// <summary>
    /// Original name: InstantConverter
    /// </summary>
    public interface IInstantConverter : IConverter
    {
        /// <summary>
        /// Original name: getChronology
        /// </summary>
        IChronology GetChronology(object obj, DateTimeZone zone);

        /// <summary>
        /// Original name: getChronology
        /// </summary>
        IChronology GetChronology(object obj, IChronology chrono);

        /// <summary>
        /// Original name: getInstantMillis
        /// </summary>
        long GetInstantMilliseconds(object obj, IChronology chrono);
    }
}