#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;

namespace NodaTime.Clocks
{
    /// <summary>
    /// Provides an implementation of <see cref="IClock"/> that always returns a known, fixed
    /// value.
    /// </summary>
    /// <remarks>
    /// This is used for testing where having a known value simplfies the tests.
    /// </remarks>
    public sealed class FixedClock
        : IClock, IDisposable
    {
        /// <summary>
        /// The original <see cref="IClock"/>.
        /// </summary>
        private readonly IClock original;

        /// <summary>
        /// The <see cref="Instant"/> this clock is fixed to.
        /// </summary>
        private readonly Instant instant;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedClock"/> class.
        /// </summary>
        /// <param name="original">The original <see cref="IClock"/>.</param>
        /// <param name="instant">The known, fixed <see cref="Instant"/>.</param>
        private FixedClock(IClock original, Instant instant)
        {
            this.original = original;
            this.instant = instant;
        }

        /// <summary>
        /// Replaces the system clock with a fixed clock that always returns the same value.
        /// </summary>
        /// <param name="instant">The fixed <see cref="Instant"/> to set the clock to.</param>
        /// <returns>The newly created <see cref="FixedClock"/>.</returns>
        /// <example>
        ///     using (FixedClock.ReplaceClock(new Instant(1234567L)))
        ///     {
        ///         // The system clock is fixed here
        ///     }
        ///     // The system clock is restored here
        /// </example>
        public static FixedClock ReplaceClock(Instant instant)
        {
            return new FixedClock(Clock.Current, instant);
        }

        #region IClock Members
        /// <summary>
        /// Gets the current time as an <see cref="Instant"/>.
        /// </summary>
        /// <value>The current time in ticks as an <see cref="Instant"/>.</value>
        public Instant Now { get { return instant; } }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        /// <remarks>
        /// This resets the global clock to the value it had when this object was created.
        /// </remarks>
        public void Dispose()
        {
            Clock.Current = original;
        }
        #endregion
    }
}