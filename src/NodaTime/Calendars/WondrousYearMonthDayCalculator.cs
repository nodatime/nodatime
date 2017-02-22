// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;

namespace NodaTime.Calendars
{
    /// <summary>
    /// See <see cref="CalendarSystem.GetWondrousCalendar" /> for details. 
    /// </summary>
    internal sealed class WondrousYearMonthDayCalculator : YearMonthDayCalculator
    {
        private const int BeMaxYear = 9999;
        private const int BeMinYear = 1;
        private const int AverageDaysPer10Years = 3652; // Ideally 365.2425 per year...

        private const int UnixEpochDayAtStartOfYear1 = -45941; //TODO: confirm that this is 1844 March 21

        internal WondrousYearMonthDayCalculator()
            : base(BeMinYear,
                  BeMaxYear,
                  AverageDaysPer10Years,
                  UnixEpochDayAtStartOfYear1)
        {
        }

        protected override int GetDaysFromStartOfYearToStartOfMonth(int year, int month)
        {
            throw new NotImplementedException();
        }

        protected override int CalculateStartOfYearDays(int year)
        {
            throw new NotImplementedException();
        }

        internal override int GetMonthsInYear(int year)
        {
            throw new NotImplementedException();
        }

        internal override int GetDaysInMonth(int year, int month)
        {
            throw new NotImplementedException();
        }

        internal override bool IsLeapYear(int year)
        {
            throw new NotImplementedException();
        }

        internal override YearMonthDay AddMonths(YearMonthDay yearMonthDay, int months)
        {
            throw new NotImplementedException();
        }

        internal override YearMonthDay GetYearMonthDay(int year, int dayOfYear)
        {
            throw new NotImplementedException();
        }

        internal override int GetDaysInYear(int year)
        {
            throw new NotImplementedException();
        }

        internal override int MonthsBetween(YearMonthDay minuendDate, YearMonthDay subtrahendDate)
        {
            throw new NotImplementedException();
        }

        internal override YearMonthDay SetYear(YearMonthDay yearMonthDay, int year)
        {
            throw new NotImplementedException();
        }
    }
}
