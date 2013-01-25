// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Calendars
{
    /// <summary>
    /// The epoch to use when constructing an Islamic calendar.
    /// </summary>
    /// <remarks>
    /// The Islamic, or Hijri, calendar can either be constructed
    /// starting on July 15th 622CE (in the Julian calendar) or on the following day.
    /// The former is the "astronomical" or "Thursday" epoch; the latter is the "civil" or "Friday" epoch.
    /// </remarks>
    /// <seealso cref="CalendarSystem.GetIslamicCalendar"/>
    public enum IslamicEpoch
    {
        /// <summary>
        /// Epoch beginning on July 15th 622CE (Julian).
        /// </summary>
        Astronomical = 1,
        /// <summary>
        /// Epoch beginning on July 16th 622CE (Julian).
        /// </summary>
        Civil = 2
    }
}
