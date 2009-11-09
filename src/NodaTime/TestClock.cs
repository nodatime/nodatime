using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
