#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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

namespace NodaTime
{
    /// <summary>
    /// Singleton implementation of <see cref="IClock"/> which reads the current system time.
    /// It is recommended that for anything other than throwaway code, this is only referenced
    /// in a single place in your code: where you provide a value to inject into the rest of
    /// your application, which should only depend on the interface.
    /// </summary>
    /// <threadsafety>This type has no state, and is thread-safe. See the thread safety section of the user guide for more information.</threadsafety>
    public sealed class SystemClock : IClock
    {
        /// <summary>
        /// The singleton instance of <see cref="SystemClock"/>.
        /// </summary>
        /// <value>The singleton instance of <see cref="SystemClock"/>.</value>
        public static readonly SystemClock Instance = new SystemClock();

        /// <summary>
        /// Constructor present to prevent external construction.
        /// </summary>
        private SystemClock()
        {
        }

        /// <summary>
        /// Gets the current time as an <see cref="Instant"/>.
        /// </summary>
        /// <value>The current time in ticks as an <see cref="Instant"/>.</value>
        public Instant Now { get { return NodaConstants.BclEpoch.PlusTicks(DateTime.UtcNow.Ticks); } }
    }
}