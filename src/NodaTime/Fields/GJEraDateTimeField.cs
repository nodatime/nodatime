// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// Era field for Gregorian and Julian calendars, representing the BC and CE eras.
    /// </summary>
    internal sealed class GJEraDateTimeField : DateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;
        private const int BeforeCommonEraIndex = 0;
        private const int CommonEraIndex = 1;

        internal GJEraDateTimeField(BasicCalendarSystem calendarSystem) 
            : base(DateTimeFieldType.Era, UnsupportedPeriodField.Eras)
        {
            this.calendarSystem = calendarSystem;
        }

        #region Values
        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetYear(localInstant) <= 0 ? BeforeCommonEraIndex : CommonEraIndex;
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            Preconditions.CheckArgumentRange("value", value, BeforeCommonEraIndex, CommonEraIndex);

            int oldEra = GetValue(localInstant);
            if (oldEra == value)
            {
                return localInstant;
            }

            int yearOfEra = calendarSystem.Fields.YearOfEra.GetValue(localInstant);
            int newAbsoluteYear = value == 1 ? yearOfEra : 1 - yearOfEra;
            return calendarSystem.SetYear(localInstant, newAbsoluteYear);
        }
        #endregion

        #region Ranges
        internal override long GetMaximumValue()
        {
            return CommonEraIndex;
        }

        internal override long GetMinimumValue()
        {
            return BeforeCommonEraIndex;
        }
        #endregion

        #region Rounding

        #endregion
    }
}