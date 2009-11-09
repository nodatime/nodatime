using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodaTime
{
    public class SystemClock : IClock
    {
        /// <summary>
        /// We'll want to do better than this, but it'll do for now.
        /// </summary>
        private static readonly System.DateTime UnixEpoch = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public long Now
        {
            get { return (System.DateTime.UtcNow.Ticks - UnixEpoch.Ticks) / TimeSpan.TicksPerMillisecond; }
        }
    }
}
