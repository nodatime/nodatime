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

using NodaTime.Clocks;

namespace NodaTime
{
    /// <summary>
    /// Represents a clock which can tell the current time as an <see cref = "Instant" />.
    /// TODO: I'm still not really happy with this... and we've made more things internal than we
    /// need (e.g. ClockBase). Will ponder...
    /// </summary>
    public static class Clock
    {
        private static ClockBase current = SystemClock.Instance;

        /// <summary>
        /// Gets or sets the object that reports the current time. This can be replaced for
        /// testing purposes.
        /// </summary>
        /// <value>The clock object.</value>
        internal static ClockBase Current { get { return current; } set { current = value ?? SystemClock.Instance; } }

        /// <summary>
        ///   Gets the current time as an <see cref = "Instant" />.
        /// </summary>
        /// <value>The current time in ticks as an <see cref = "Instant" />.</value>
        public static Instant Now { get { return Current.Now; } }
    }
}