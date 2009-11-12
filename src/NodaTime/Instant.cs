#region Copyright and license information

// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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

namespace NodaTime
{
    /// <summary>
    /// Represents an instant on the timeline, measured in milliseconds from the Unix epoch,
    /// which is typically described as January 1st 1970, midnight, UTC (ISO calendar).
    /// </summary>
    /// <remarks>
    /// The default value of this struct is the Unix epoch.
    /// This type is immutable and thread-safe.
    /// </remarks>
    public struct Instant
    {
        public static readonly Instant UnixEpoch = new Instant(0);

        private readonly long milliseconds;

        /// <summary>
        /// Milliseconds since the Unix epoch.
        /// </summary>
        public long Milliseconds { get { return milliseconds; } }

        public Instant(long milliseconds)
        {
            this.milliseconds = milliseconds;
        }

        /// <summary>
        /// Returns the difference between two instants as a duration.
        /// TODO: It *could* return an interval... but I think this is better.
        /// </summary>
        public static Duration operator -(Instant first, Instant second)
        {
            return new Duration(first.Milliseconds - second.Milliseconds);
        }
    }
}
