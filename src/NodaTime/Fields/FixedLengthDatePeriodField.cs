// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Date period field for fixed-length periods (weeks and days).
    /// </summary>
    internal sealed class FixedLengthDatePeriodField : IDatePeriodField
    {
        private readonly int unitDays;

        internal FixedLengthDatePeriodField(int unitDays)
        {
            this.unitDays = unitDays;
        }

        public LocalDate Add(LocalDate localDate, int value)
        {
            if (value == 0)
            {
                return localDate;
            }
            int daysToAdd = value * unitDays;
            var calendar = localDate.Calendar;
            // If we know it will be in this year, next year, or the previous year...
            if (daysToAdd < 300 && daysToAdd > -300)
            {
                YearMonthDayCalculator calculator = calendar.YearMonthDayCalculator;
                YearMonthDay yearMonthDay = localDate.YearMonthDay;
                int year = yearMonthDay.Year;
                int month = yearMonthDay.Month;
                int day = yearMonthDay.Day;
                int newDayOfMonth = day + daysToAdd;
                if (1 <= newDayOfMonth && newDayOfMonth <= calculator.GetDaysInMonth(year, month))
                {
                    return new LocalDate(new YearMonthDayCalendar(year, month, newDayOfMonth, calendar.Ordinal));
                }
                int dayOfYear = calculator.GetDayOfYear(yearMonthDay);
                int newDayOfYear = dayOfYear + daysToAdd;

                if (newDayOfYear < 1)
                {
                    newDayOfYear += calculator.GetDaysInYear(year - 1);
                    year--;
                    if (year < calculator.MinYear)
                    {
                        throw new OverflowException("Date computation would underflow the minimum year of the calendar");
                    }
                }
                else
                {
                    int daysInYear = calculator.GetDaysInYear(year);
                    if (newDayOfYear > daysInYear)
                    {
                        newDayOfYear -= daysInYear;
                        year++;
                        if (year > calculator.MaxYear)
                        {
                            throw new OverflowException("Date computation would overflow the maximum year of the calendar");
                        }
                    }
                }
                return new LocalDate(calculator.GetYearMonthDay(year, newDayOfYear), calendar);
            }
            // LocalDate constructor will validate.
            int days = localDate.DaysSinceEpoch + daysToAdd;
            return new LocalDate(days, calendar);
        }

        public int Subtract(LocalDate minuendDate, LocalDate subtrahendDate)
        {
            // We already assume the calendars are the same.
            if (minuendDate.YearMonthDay == subtrahendDate.YearMonthDay)
            {
                return 0;
            }
            // Note: I've experimented with checking for the dates being in the same year and optimizing that.
            // It helps a little if they're in the same month, but just that test has a cost for other situations.
            // Being able to find the day of year if they're in the same year but different months doesn't help,
            // somewhat surprisingly.
            int minuendDays = minuendDate.DaysSinceEpoch;
            int subtrahendDays = subtrahendDate.DaysSinceEpoch;
            return (minuendDays - subtrahendDays) / unitDays;
        }
    }
}
