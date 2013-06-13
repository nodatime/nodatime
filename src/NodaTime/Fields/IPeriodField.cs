// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Fields
{
    /// <summary>
    /// General representation of the difference between two LocalInstant values in a particular unit,
    /// such as "months" or "hours". This is effectively a vector type: it doesn't make sense to ask
    /// a period field for its value at a particular local instant; instead, a number of units can be
    /// added to an existing local instant, and you can request the difference between two local instants
    /// in terms of that unit.
    /// </summary>
    internal interface IPeriodField
    {
        /// <summary>
        /// Adds a duration value (which may be negative) to the instant. This may not
        /// be reversible; for example, adding a month to January 30th will result in
        /// February 28th or February 29th.
        /// </summary>
        /// <param name="localInstant">The local instant to add to</param>
        /// <param name="value">The value to add, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        LocalInstant Add(LocalInstant localInstant, long value);

        /// <summary>
        /// Computes the difference between two local instants, as measured in the units
        /// of this field. Any fractional units are dropped from the result. Calling
        /// Subtract reverses the effect of calling Add, as far as possible.
        /// </summary>
        /// <remarks>
        /// The result is determined so as not to overshoot when added back: calling
        /// <see cref="Add"/> using <paramref name="subtrahendInstant"/> and
        /// the result of this method will yield a value which is between <paramref name="subtrahendInstant"/>
        /// and <paramref name="minuendInstant"/>. (In a simpler world, it would exactly equal
        /// <paramref name="minuendInstant"/>, but that's not always possible.)
        /// </remarks>
        /// <param name="minuendInstant">The local instant to subtract from</param>
        /// <param name="subtrahendInstant">The local instant to subtract from minuendInstant</param>
        /// <returns>The difference in the units of this field</returns>
        long Subtract(LocalInstant minuendInstant, LocalInstant subtrahendInstant);
    }
}
