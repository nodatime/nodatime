namespace NodaTime.Benchmarks.Timing
{
    /// <summary>
    /// Timer used to measure performance. Implementations
    /// may use wall time, CPU timing etc. Implementations
    /// don't need to be thread-safe.
    /// </summary>
    internal interface IBenchTimer
    {
        void Reset();
        Duration ElapsedTime { get; }
    }
}