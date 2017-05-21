// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Fields
{
    /// <summary>
    /// General representation of the difference between two dates in a particular time unit,
    /// such as "days" or "months".
    /// </summary>
    internal interface IDatePeriodField
    {
        /// <summary>
        /// Adds a duration value (which may be negative) to the date. This may not
        /// be reversible; for example, adding a month to January 30th will result in
        /// February 28th or February 29th.
        /// </summary>
        /// <param name="localDate">The local date to add to</param>
        /// <param name="value">The value to add, in the units of the field</param>
        /// <returns>The updated local date</returns>
        LocalDate Add(LocalDate localDate, int value);

        /// <summary>
        /// Computes the difference between two local dates, as measured in the units
        /// of this field, rounding towards zero. This rounding means that
        /// unit.Add(start, unit.UnitsBetween(start, end)) always ends up with a date
        /// between start and end. (Ideally equal to end, but importantly, it never overshoots.)
        /// </summary>
        /// <param name="start">The start date</param>
        /// <param name="end">The end date</param>
        /// <returns>The difference in the units of this field</returns>
        int UnitsBetween(LocalDate start, LocalDate end);
    }
}
