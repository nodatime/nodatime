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

using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Porting status: Need AddWrapField
    /// </summary>
    internal sealed class GJYearOfEraDateTimeField : DecoratedDateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal GJYearOfEraDateTimeField(DateTimeField yearField, BasicCalendarSystem calendarSystem) : base(yearField, DateTimeFieldType.YearOfEra)
        {
            this.calendarSystem = calendarSystem;
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            int year = WrappedField.GetValue(localInstant);
            return year <= 0 ? 1 - year : year;
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, 1, GetMaximumValue());
            if (calendarSystem.GetYear(localInstant) <= 0)
            {
                value = 1 - value;
            }
            return base.SetValue(localInstant, value);
        }

        internal override long GetMinimumValue()
        {
            return 1;
        }

        internal override long GetMaximumValue()
        {
            return WrappedField.GetMaximumValue();
        }

        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return WrappedField.RoundFloor(localInstant);
        }

        internal override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return WrappedField.RoundCeiling(localInstant);
        }

        internal override Duration Remainder(LocalInstant localInstant)
        {
            return WrappedField.Remainder(localInstant);
        }
    }
}