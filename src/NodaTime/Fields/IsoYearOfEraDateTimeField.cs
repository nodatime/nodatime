#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Still need to do AddWrapField.
    /// </summary>
    internal class IsoYearOfEraDateTimeField : DecoratedDateTimeField
    {
        internal static readonly DateTimeFieldBase Instance = new IsoYearOfEraDateTimeField();

        private IsoYearOfEraDateTimeField() : base(GregorianCalendarSystem.Default.Fields.Year, DateTimeFieldType.YearOfEra)
        {
        }

        public override int GetValue(LocalInstant localInstant)
        {
            return Math.Abs(WrappedField.GetValue(localInstant));
        }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            return Math.Abs(WrappedField.GetValue(localInstant));
        }

        public override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return WrappedField.GetDifference(minuendInstant, subtrahendInstant);
        }

        public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return WrappedField.GetInt64Difference(minuendInstant, subtrahendInstant);
        }

        public override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, 0, GetMaximumValue());
            if (WrappedField.GetValue(localInstant) < 0)
            {
                value = -value;
            }
            return base.SetValue(localInstant, value);
        }

        public override long GetMinimumValue()
        {
            return 0;
        }

        public override long GetMaximumValue()
        {
            return WrappedField.GetMaximumValue();
        }

        public override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return WrappedField.RoundCeiling(localInstant);
        }

        public override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return WrappedField.RoundCeiling(localInstant);
        }

        public override Duration Remainder(LocalInstant localInstant)
        {
            return WrappedField.Remainder(localInstant);
        }
    }
}