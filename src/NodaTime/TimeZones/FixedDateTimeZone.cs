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
namespace NodaTime.TimeZones
{
    /// <summary>
    /// Basic <see cref="IDateTimeZone" /> implementation that has a fixed name key and offset.
    /// </summary>
    /// <remarks>
    /// This type is thread-safe and immutable.
    /// </remarks>
    public sealed class FixedDateTimeZone 
        : DateTimeZoneBase
    {
        /// <summary>
        /// Creates a new fixed time zone.
        /// </summary>
        /// <param name="id">The ID of the time zone.</param>
        /// <param name="offset">The <see cref="Offset"/> from UTC.</param>
        public FixedDateTimeZone(string id, Offset offset)
            : base(id, offset, true)
        {
        }

        /// <summary>
        /// Returns the transition occurring strictly after the specified instant,
        /// or null if there are no further transitions.
        /// </summary>
        /// <param name="instant">The instant after which to consider transitions.</param>
        /// <returns>
        /// The instant of the next transition, or null if there are no further transitions.
        /// </returns>
        public override Instant? NextTransition(Instant instant)
        {
            return null;
        }

        /// <summary>
        /// Returns the transition occurring strictly before the specified instant,
        /// or null if there are no earlier transitions.
        /// </summary>
        /// <param name="instant">The instant before which to consider transitions.</param>
        /// <returns>
        /// The instant of the previous transition, or null if there are no further transitions.
        /// </returns>
        public override Instant? PreviousTransition(Instant instant)
        {
            return null;
        }

        /// <summary>
        /// Returns the offset from local time to UTC, where a positive duration indicates that UTC is earlier
        /// than local time. In other words, UTC = local time - (offset from local).
        /// </summary>
        /// <param name="instant">The instant for which to calculate the offset.</param>
        /// <returns>The offset at the specified local time.</returns>
        public override Offset GetOffsetFromLocal(LocalInstant instant)
        {
            return this.standardOffset;
        }
    }
}
