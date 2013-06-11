// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// Fixed length date/time field, which has a fixed length unit period field.
    /// </summary>
    /// <remarks>
    /// </remarks>
    internal abstract class FixedLengthPeriodDateTimeField : DateTimeField
    {
        /// <summary>
        /// The fractional unit in ticks
        /// </summary>
        private readonly long unitTicks;

        protected FixedLengthPeriodDateTimeField(DateTimeFieldType fieldType, PeriodField unitField)
            : base(fieldType, unitField)
        {
            Preconditions.CheckArgument(unitField.IsFixedLength, "unitField", "Unit period field must have a fixed length");
            Preconditions.CheckArgument(unitField.IsSupported, "unitField", "Unit period field must be supported");
            unitTicks = unitField.UnitTicks;
        }

        internal long UnitTicks { get { return unitTicks; } }

        #region Values
        /// <summary>
        /// Sets a value in the milliseconds supplied.
        /// <para>
        /// The value of this field will be set.
        /// If the value is invalid, an exception if thrown.
        /// </para>
        /// <para>
        /// If setting this field would make other fields invalid, then those fields
        /// may be changed. For example if the current date is the 31st January, and
        /// the month is set to February, the day would be invalid. Instead, the day
        /// would be changed to the closest value - the 28th/29th February as appropriate.
        /// </para>
        /// </summary>
        /// <param name="localInstant">The local instant to set in</param>
        /// <param name="value">The value to set, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            // TODO: There's no need to fetch the actual maximum value, in cases where we know we're trying to set a value
            // which is always in range (e.g. 28 for day-of-year). Consider optimizing this.
            Preconditions.CheckArgumentRange("value", value, GetMinimumValue(), GetMaximumValue(localInstant));
            return new LocalInstant(localInstant.Ticks + (value - GetInt64Value(localInstant)) * unitTicks);
        }
        #endregion

        #region Ranges
        internal override long GetMinimumValue()
        {
            return 0;
        }
        #endregion

        #region Rounding

        #endregion
    }
}