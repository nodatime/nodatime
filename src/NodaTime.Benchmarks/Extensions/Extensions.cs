// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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