// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Calendars
{
    /// <summary>
    /// The ISO-8601 calendar is equivalent to the Gregorian calendar but the century
    /// and year-of-century are 0-based, and for negative centuries the year is treated as 0-based too.
    /// (This may be a bug in previous versions of Noda Time, but we should be backward compatible
    /// at least until we know for sure.)
    /// </summary>
    internal sealed class IsoYearMonthDayCalculator : GregorianYearMonthDayCalculator
    {
        internal static readonly IsoYearMonthDayCalculator IsoInstance = new IsoYearMonthDayCalculator();

        private IsoYearMonthDayCalculator()
        {
        }

        internal override int GetCenturyOfEra(LocalInstant localInstant)
        {
            return Math.Abs(GetYear(localInstant)) / 100;
        }

        internal override int GetYearOfCentury(LocalInstant localInstant)
        {
            return Math.Abs(GetYear(localInstant)) % 100;
        }
    }
}
