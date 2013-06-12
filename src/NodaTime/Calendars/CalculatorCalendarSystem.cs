// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodaTime.Calendars
{
    internal sealed class CalculatorCalendarSystem : CalendarSystem
    {
        private readonly YearMonthDayCalculator yearMonthDayCalculator;
        private readonly WeekYearCalculator weekYearCalculator;

        internal CalculatorCalendarSystem(
            string id,
            string name,
            YearMonthDayCalculator yearMonthDayCalculator,
            WeekYearCalculator weekYearCalculator)
            : base(id, name,
                   yearMonthDayCalculator.MinYear,
                   yearMonthDayCalculator.MaxYear,
                   (builder, @this) => AssembleFields(builder,
                                                      yearMonthDayCalculator,
                                                      weekYearCalculator),
                   yearMonthDayCalculator.Eras)
        {
            this.yearMonthDayCalculator = yearMonthDayCalculator;
            this.weekYearCalculator = weekYearCalculator;
        }

        private static void AssembleFields(FieldSet.Builder builder,
            YearMonthDayCalculator yearMonthDayCalculator,
            WeekYearCalculator weekYearCalculator)
        {            
        }

        public override int GetDaysInMonth(int year, int month)
        {
            return yearMonthDayCalculator.GetDaysInMonth(year, month);
        }

        public override bool IsLeapYear(int year)
        {
            return yearMonthDayCalculator.IsLeapYear(year);
        }

        public override int GetMaxMonth(int year)
        {
            return yearMonthDayCalculator.GetMaxMonth(year);
        }

        internal override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth,
                                                       long tickOfDay)
        {
            return yearMonthDayCalculator.GetLocalInstant(year, monthOfYear, dayOfMonth).PlusTicks(tickOfDay);
        }

        internal override LocalInstant GetLocalInstantFromWeekYearWeekAndDayOfWeek(int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek)
        {
            return weekYearCalculator.GetLocalInstant(weekYear, weekOfWeekYear, dayOfWeek);
        }

        internal override LocalInstant GetLocalInstant(Era era, int yearOfEra, int monthOfYear, int dayOfMonth)
        {
            return yearMonthDayCalculator.GetLocalInstant(era, yearOfEra, monthOfYear, dayOfMonth);
        }

        internal override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute, int millisecondOfSecond, int tickOfMillisecond)
        {
            LocalInstant date = yearMonthDayCalculator.GetLocalInstant(year, monthOfYear, dayOfMonth);
            long timeTicks = TimeOfDayCalculator.GetTicks(hourOfDay, minuteOfHour, secondOfMinute, millisecondOfSecond);
            return date.PlusTicks(timeTicks);
        }
    }
}
