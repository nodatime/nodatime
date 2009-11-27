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
    public sealed class FixedDateTimeZone : IDateTimeZone
    {
        private readonly Duration offset;
        private readonly string id;

        /// <summary>
        /// Creates a new fixed time zone.
        /// </summary>
        /// <param name="id">The ID of the time zone.</param>
        /// <param name="offset">The offset from UTC. A positive duration indicates that the local time is later than UTC.</param>
        public FixedDateTimeZone(string id, Duration offset)
        {
            this.id = id;
            this.offset = offset;
        }

        public Instant? NextTransition(Instant instant)
        {
            return null;
        }

        public Instant? PreviousTransition(Instant instant)
        {
            return null;
        }

        public Duration GetOffsetFromUtc(Instant instant)
        {
            return offset;
        }

        public Duration GetOffsetFromLocal(LocalDateTime localTime)
        {
            return offset;
        }

        public string Id
        {
            get { return id; }
        }

        public bool IsFixed
        {
            get { return true; }
        }
    }
}
