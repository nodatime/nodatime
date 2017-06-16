// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// Extension methods for <see cref="IClock"/> to enable migration to Noda Time 2.0.
    /// </summary>
    public static class ClockExtensions
    {
        /// <summary>
        /// Gets the current <see cref="Instant"/> on the time line according to <paramref name="clock"/>.
        /// </summary>
        /// <remarks>This extension method in 1.4 is an instance method in 2.0, replacing the
        /// <see cref="IClock.Now"/> property.</remarks>
        /// <returns>The current instant on the time line according to this clock.</returns>
        public static Instant GetCurrentInstant(this IClock clock) =>
            Preconditions.CheckNotNull(clock, nameof(clock)).Now;
    }
}
