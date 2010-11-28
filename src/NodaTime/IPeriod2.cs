using System;
using System.Collections.Generic;
using System.Text;
using NodaTime.Periods;

namespace NodaTime
{
    // TODO: Consider whether this should derive from IEnumerable[DurationFieldValue] instead
    // of having a property for it.

    /// <summary>
    /// Represents a period of time expressed in human chronological terms: hours, days,
    /// weeks, months and so on.
    /// </summary>
    /// <remarks>
    /// Periods operate on calendar-related types such as
    /// <see cref="LocalDateTime" /> whereas <see cref="Duration"/> operates on instants
    /// on the time line. <see cref="ZonedDateTime" /> includes both concepts, so both
    /// durations and periods can be added to zoned date-time instances - although the
    /// results may not be the same. For example, adding a two hour period to a ZonedDateTime
    /// will always give a result has a local date-time which is two hours later, even if that
    /// means that three hours would have to actually pass in experienced time to arrive at
    /// that local date-time, due to changes in the UTC offset (e.g. for daylight savings).
    /// </remarks>
    public interface IPeriod2
    {
        IEnumerable<DurationFieldValue> FieldValues { get; }
    }
}
