#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
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
            Preconditions.CheckArgumentRange("value", value, GetMinimumValue(), GetMaximumValueForSet(localInstant, value));
            return new LocalInstant(localInstant.Ticks + (value - GetInt64Value(localInstant)) * unitTicks);
        }
        #endregion

        #region Ranges
        internal override long GetMinimumValue()
        {
            return 0;
        }

        /// <summary>
        /// Called by <see cref="DateTimeField.SetValue(NodaTime.LocalInstant,long)" /> (and related overloads)
        /// to get the maximum allowed value. By default, returns GetMaximumValue(localInstant). Override to provide
        /// a faster implementation.
        /// </summary>
        protected long GetMaximumValueForSet(LocalInstant localInstant, long value)
        {
            return GetMaximumValue(localInstant);
        }
        #endregion

        #region Rounding
        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            long ticks = localInstant.Ticks;
            if (ticks >= 0)
            {
                return new LocalInstant(ticks - (ticks % unitTicks));
            }
            else
            {
                ticks++;
                return new LocalInstant(ticks - (ticks % unitTicks) - unitTicks);
            }
        }

        internal override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            long ticks = localInstant.Ticks;
            if (ticks > 0)
            {
                ticks--;
                return new LocalInstant(ticks - (ticks % unitTicks) + unitTicks);
            }
            else
            {
                return new LocalInstant(ticks - ticks % unitTicks);
            }
        }

        internal override Duration Remainder(LocalInstant localInstant)
        {
            long ticks = localInstant.Ticks;
            return new Duration(ticks >= 0 ? ticks % unitTicks : ((ticks + 1) % unitTicks) + unitTicks - 1);
        }
        #endregion
    }
}