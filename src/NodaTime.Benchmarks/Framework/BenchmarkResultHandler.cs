// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Benchmarks.Framework
{
    /// <summary>
    /// Handler for benchmark results.
    /// </summary>
    /// <remarks>
    /// While we could have used events on BenchmarkRunner, it's likely that a bunch of these will be
    /// customized at the same time, so it makes more sense to make it a normal class.
    /// </remarks>
    internal abstract class BenchmarkResultHandler
    {
        /// <summary>
        /// Called at the very start of the set of tests.
        /// </summary>
        /// <param name="options">Options used in this test</param>
        internal virtual void HandleStartRun(BenchmarkOptions options)
        {
        }

        /// <summary>
        /// Called at the very end of the set of tests.
        /// </summary>
        internal virtual void HandleEndRun()
        {
        }

        /// <summary>
        /// Called at the start of benchmarks for a single type
        /// </summary>
        internal virtual void HandleStartType(Type type)
        {
        }

        /// <summary>
        /// Called at the end of benchmarks for a single type.
        /// </summary>
        internal virtual void HandleEndType()
        {
        }

        /// <summary>
        /// Called once for each test.
        /// </summary>
        /// <param name="result"></param>
        internal virtual void HandleResult(BenchmarkResult result)
        {
        }

        /// <summary>
        /// Called each time a type or method isn't tested unexpectedly.
        /// </summary>
        /// <param name="text"></param>
        internal virtual void HandleWarning(string text)
        {
        }
    }
}
