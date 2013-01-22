// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Calendars;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    internal sealed class IsoYearOfEraDateTimeField : DecoratedDateTimeField
    {
        internal static readonly DateTimeField Instance = new IsoYearOfEraDateTimeField();

        private IsoYearOfEraDateTimeField() : base(GregorianCalendarSystem.GetInstance(4).Fields.Year, DateTimeFieldType.YearOfEra)
        {
        }

        internal override int GetValue(LocalInstant localInstant)
        {
            return Math.Abs(WrappedField.GetValue(localInstant));
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return Math.Abs(WrappedField.GetValue(localInstant));
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            Preconditions.CheckArgumentRange("value", value, 0, GetMaximumValue());
            if (WrappedField.GetValue(localInstant) < 0)
            {
                value = -value;
            }
            return base.SetValue(localInstant, value);
        }

        internal override long GetMinimumValue()
        {
            return 0;
        }

        internal override long GetMaximumValue()
        {
            return WrappedField.GetMaximumValue();
        }

        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return WrappedField.RoundCeiling(localInstant);
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