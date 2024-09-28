// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

#if NET8_0_OR_GREATER
using NodaTime.Annotations;
using NodaTime.Extensions;
using NodaTime.Utility;
#endif
using System;

namespace NodaTime
{
    /// <summary>
    /// Represents a clock which can return the current time as an <see cref="Instant" />.
    /// </summary>
    /// <remarks>
    /// <see cref="IClock"/> is intended for use anywhere you need to have access to the current time.
    /// Although it's not strictly incorrect to call <c>SystemClock.Instance.GetCurrentInstant()</c> directly,
    /// in the same way as you might call <see cref="DateTime.UtcNow"/>, it's strongly discouraged
    /// as a matter of style for production code. We recommend providing an instance of <see cref="IClock"/>
    /// to anything that needs it, which allows you to write tests using the fake clock in the NodaTime.Testing
    /// assembly (or your own implementation).
    /// </remarks>
    /// <seealso cref="SystemClock"/>
    /// <threadsafety>All implementations in Noda Time are thread-safe; custom implementations
    /// should be thread-safe too. See the thread safety section of the user guide for more information.
    /// </threadsafety>
    public interface IClock
    {
        /// <summary>
        /// Gets the current <see cref="Instant"/> on the time line according to this clock.
        /// </summary>
        /// <returns>The current instant on the time line according to this clock.</returns>
        Instant GetCurrentInstant();

#if NET6_0_OR_GREATER
        /// <summary>
        /// Convenience property for <see cref="SystemClock.Instance"/>.
        /// </summary>
        public static IClock System => SystemClock.Instance;
#endif

#if NET8_0_OR_GREATER
        /// <summary>
        /// Creates a clock which delegates requests for the current time to <see cref="TimeProvider.GetUtcNow"/>
        /// </summary>
        /// <param name="timeProvider">The time provider to delegate to. Must not be null.</param>
        /// <returns>A clock which delegates to the given time provider.</returns>
        public static IClock FromTimeProvider(TimeProvider timeProvider) => new TimeProviderClock(timeProvider);

        [Immutable]
        private sealed class TimeProviderClock : IClock
        {
            private readonly TimeProvider timeProvider;

            /// <summary>
            /// Creates an instance from the given time provider, delegating
            /// requests for the current time to <see cref="TimeProvider.GetUtcNow"/>
            /// </summary>
            /// <param name="timeProvider">The time provider to delegate to. Must not be null.</param>
            public TimeProviderClock(TimeProvider timeProvider) =>
                this.timeProvider = Preconditions.CheckNotNull(timeProvider, nameof(timeProvider));

            /// <inheritdoc/>
            public Instant GetCurrentInstant() => timeProvider.GetUtcNow().ToInstant();
        }
#endif
    }
}
