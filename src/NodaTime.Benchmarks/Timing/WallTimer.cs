using System.Diagnostics;

namespace NodaTime.Benchmarks.Timing
{
    /// <summary>
    /// Timer using the built-in stopwatch class; measures wall-clock time.
    /// </summary>
    internal class WallTimer : IBenchTimer
    {
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();

        public void Reset()
        {
            stopwatch.Reset();
            stopwatch.Start();
        }

        public Duration ElapsedTime { get { return new Duration(stopwatch.Elapsed.Ticks); } }
    }
}