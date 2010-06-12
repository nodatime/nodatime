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
    /// Represents a clock which can tell the current time as an <see cref="Instant"/>.
    /// </summary>
    public static class Clock
    {
        private static IClock current = SystemClock.Instance;

        /// <summary>
        /// Gets or sets the object that reports the current time. Replaceable for easier testing.
        /// </summary>
        /// <value>The clock object.</value>
        public static IClock Current
        {
            get { return current; }
            set
            {
                if (value == null)
                {
                    current = SystemClock.Instance;
                }
                else
                {
                    Current = value;
                }
            }
        }

        /// <summary>
        /// Gets the current time as an <see cref="Instant"/>.
        /// </summary>
        /// <value>The current time in ticks as an <see cref="Instant"/>.</value>
        public static Instant Now { get { return Current.Now; } }
    }
}