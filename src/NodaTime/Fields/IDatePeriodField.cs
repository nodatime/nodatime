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
        /// of this field. Any fractional units are dropped from the result. Calling
        /// Subtract reverses the effect of calling Add, as far as possible.
        /// </summary>
        /// <remarks>
        /// The result is determined so as not to overshoot when added back: calling
        /// <see cref="Add"/> using <paramref name="subtrahendDate"/> and
        /// the result of this method will yield a value which is between <paramref name="subtrahendDate"/>
        /// and <paramref name="minuendDate"/>. (In a simpler world, it would exactly equal
        /// <paramref name="minuendDate"/>, but that's not always possible.)
        /// </remarks>
        /// <param name="minuendDate">The local date to subtract from</param>
        /// <param name="subtrahendDate">The local date to subtract from minuendDate</param>
        /// <returns>The difference in the units of this field</returns>
        int Subtract(LocalDate minuendDate, LocalDate subtrahendDate);
    }
}
