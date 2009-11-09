using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodaTime
{
    /// <summary>
    /// Represents a clock which can tell the current time, expressed in milliseconds since
    /// the Unix epoch.
    /// </summary>
    public interface IClock
    {
        long Now { get; }
    }
}
