// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime
{
    /// <summary>
    /// Represents a clock which can return the current time as an <see cref="Instant" />.
    /// </summary>
    /// <remarks>
    /// <see cref="IClock"/> is intended for use anywhere you need to have access to the current time.
    /// Although it's not strictly incorrect to call <c>SystemClock.Instance.Now</c> directly,
    /// in the same way as you might call <see cref="DateTime.UtcNow"/>, it's strongly discouraged
    /// as a matter of style for production code. We recommend providing an instance of <see cref="IClock"/>
    /// to anything that needs it, which allows you to write tests using the stub clock in the NodaTime.Testing
    /// assembly (or your own implementation).
    /// </remarks>
    /// <seealso cref="SystemClock"/>
    /// <seealso cref="T:NodaTime.Testing.StubClock"/>
    /// <threadsafety>All implementations in Noda Time are thread-safe; custom implementations
    /// should be thread-safe too. See the thread safety section of the user guide for more information.
    /// </threadsafety>
    public interface IClock
    {
        /// <summary>
        /// Gets the current <see cref="Instant"/> on the time line according to this clock.
        /// </summary>
        Instant Now { get; }
    }
}
