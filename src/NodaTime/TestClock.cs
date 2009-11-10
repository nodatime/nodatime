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

using System;

namespace NodaTime
{
    /// <summary>
    /// Clock designed specifically for testing. Automatically replaces the "current"
    /// clock, and remembers it. When this clock is disposed, the "current" clock is
    /// set back to the original one. This clock's value can be reset at will.
    /// </summary>
    public sealed class TestClock : IDisposable
    {
        /// <summary>
        /// The original "current" clock.
        /// </summary>
        private readonly IClock original;

        private TestClock(IClock original, long now)
        {
            this.original = original;
            Now = now;
        }

        public long Now { get; set; }

        public IClock Original { get { return original; } }

        /// <summary>
        /// Replaces the current clock, and sets its own time
        /// to be the current time of that original clock. The returned
        /// clock is the new current clock; when it's disposed, it will
        /// replace itself with the original one.
        /// </summary>
        public static TestClock ReplaceCurrent()
        {
            return ReplaceCurrent(DateTimeUtils.Clock.Now);
        }

        /// <summary>
        /// Replaces the current clock, and sets its own time
        /// to be the current time of that original clock.
        /// </summary>
        public static TestClock ReplaceCurrent(long time)
        {
            return new TestClock(DateTimeUtils.Clock, time);
        }

        public void Dispose()
        {
            DateTimeUtils.Clock = original;
        }
    }
}
