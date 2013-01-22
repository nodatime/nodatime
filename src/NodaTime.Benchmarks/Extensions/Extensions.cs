namespace NodaTime.Benchmarks.Extensions
{
    internal static class BenchmarkingExtensions
    {
        /// <summary>
        /// This does absolutely nothing, but 
        /// allows us to consume a property value without having
        /// a useless local variable. It should end up being JITted
        /// away completely, so have no effect on the results.
        /// </summary>
        internal static void Consume<T>(this T value)
        {
        }
    }
}