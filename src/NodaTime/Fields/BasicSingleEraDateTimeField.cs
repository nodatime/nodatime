// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Calendars;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// Provides the era component of any calendar with only a single era.
    /// This always returns a value of 0, as it's always the sole entry in the list of eras.
    /// </summary>
    internal sealed class BasicSingleEraDateTimeField : DateTimeField
    {
        private readonly Era era;

        internal BasicSingleEraDateTimeField(Era era)
            : base(DateTimeFieldType.Era, UnsupportedPeriodField.Eras)
        {
            this.era = era;
        }

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
            Preconditions.CheckArgumentRange("value", value, 0, 0);
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
