using System;

namespace NodaTime.Benchmarks.Timing
{
    /// <summary>
    /// Attribute applied to any method which should be benchmarked.
    /// The method must be parameterless, and its containing class
    /// must have a parameterless constructor. The constructor will
    /// be called just once, before all the tests are run - typically
    /// any initialization will just be for readonly fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class BenchmarkAttribute : Attribute
    {
    }
}