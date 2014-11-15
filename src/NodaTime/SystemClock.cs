// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Annotations;

namespace NodaTime
{
    /// <summary>
    /// Singleton implementation of <see cref="IClock"/> which reads the current system time.
    /// It is recommended that for anything other than throwaway code, this is only referenced
    /// in a single place in your code: where you provide a value to inject into the rest of
    /// your application, which should only depend on the interface.
    /// </summary>
    /// <threadsafety>This type has no state, and is thread-safe. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class SystemClock : IClock
    {
        /// <summary>
        /// The singleton instance of <see cref="SystemClock"/>.
        /// </summary>
        /// <value>The singleton instance of <see cref="SystemClock"/>.</value>
        public static SystemClock Instance { get; } = new SystemClock();

        /// <summary>
        /// Constructor present to prevent external construction.
        /// </summary>
        private SystemClock()
        {
        }

        /// <summary>
        /// Gets the current time as an <see cref="Instant"/>.
        /// </summary>
        /// <returns>The current time in ticks as an <see cref="Instant"/>.</returns>
        public Instant GetCurrentInstant() => NodaConstants.BclEpoch.PlusTicks(DateTime.UtcNow.Ticks);
    }
}
