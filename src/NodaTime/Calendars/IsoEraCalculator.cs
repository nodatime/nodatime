// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Calendars
{
    /// <summary>
    /// An era calculator for the ISO calendar - just like a normal Gregorian/Julian one,
    /// except for the century handling. If issue 318 ends up removing century support, this class
    /// can die.
    /// </summary>
    internal sealed class IsoEraCalculator : GJEraCalculator
    {
        internal IsoEraCalculator(YearMonthDayCalculator yearMonthDayCalculator) : base(yearMonthDayCalculator)
        {
        }

        internal override int GetCenturyOfEra(YearMonthDay yearMonthDay)
        {
            return Math.Abs(yearMonthDay.Year) / 100;
        }

        internal override int GetYearOfCentury(YearMonthDay yearMonthDay)
        {
            return Math.Abs(yearMonthDay.Year) % 100;
        }
    }
}
