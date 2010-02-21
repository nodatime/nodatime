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
namespace NodaTime.Clocks
{
    /// <summary>
    /// Singleton implementation of <see cref="IClock"/> which reads the current system time.
    /// </summary>
    public class SystemClock
        : IClock
    {
        private static readonly SystemClock instance = new SystemClock();

        /// <summary>
        /// Gets the singleton instance of <see cref="SystemClock"/>.
        /// </summary>
        /// <value>The singleton instance of <see cref="SystemClock"/>.</value>
        public static SystemClock Instance { get { return instance; } }

        // TODO: We'll want to do better than this, but it'll do for now.
        // private static readonly System.DateTime UnixEpoch = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly long UnixEpochTicks = (new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).Ticks;

        #region IClock Members

        /// <summary>
        /// Gets the current time as an <see cref="Instant"/>.
        /// </summary>
        /// <value>The current time in ticks as an <see cref="Instant"/>.</value>
        public Instant Now
        {
            get { return new Instant(System.DateTime.UtcNow.Ticks - UnixEpochTicks); }
        }

        #endregion
    }
}
