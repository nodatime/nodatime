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
using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Provides the era component of any calendar with only a single era.
    /// This always returns a value of 0, as it's always the sole entry in the list of eras.
    /// </summary>
    internal class BasicSingleEraDateTimeField : DateTimeField
    {
        private readonly Era era;

        internal BasicSingleEraDateTimeField(Era era)
            : base(DateTimeFieldType.Era, UnsupportedPeriodField.Eras)
        {
            this.era = era;
        }

        internal override string Name { get { return era.Name; } }

        internal override int GetValue(LocalInstant localInstant)
        {
            return 0;
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return 0;
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, 0, 0);
            return localInstant;
        }

        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return LocalInstant.MinValue;
        }

        internal override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return LocalInstant.MaxValue;
        }

        internal override LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            return LocalInstant.MinValue;
        }

        internal override LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            return LocalInstant.MinValue;
        }

        internal override LocalInstant RoundHalfEven(LocalInstant localInstant)
        {
            return LocalInstant.MinValue;
        }

        // FIXME: Joda returns a null. Could return an unsupported field?
        internal override PeriodField RangePeriodField
        {
            get { throw new NotSupportedException(); }
        }

        internal override long GetMinimumValue()
        {
            return 0;
        }

        internal override long GetMaximumValue()
        {
            return 0;
        }
    }
}
