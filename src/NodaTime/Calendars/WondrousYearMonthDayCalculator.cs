// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NodaTime.Calendars
{
  /// <summary>
  ///   See <see cref="CalendarSystem.GetWondrousCalendar" /> for details.
  /// </summary>
  internal sealed class WondrousYearMonthDayCalculator : YearMonthDayCalculator
  {
    private const int BeMaxYear = 999; // 1 less than the table of offsets
    private const int BeMinYear = 1;
    private const int AverageDaysPer10Years = 3652; // Ideally 365.2425 per year...
    private const int DaysInMonth = 19;
    private const int FirstYearInNewModel = 173;

    private static Dictionary<int, int> _nawRuzOffsetFrom21;

    private const int UnixEpochDayAtStartOfYear1 = -45941; //TODO: confirm that this is 1844 March 21

    internal WondrousYearMonthDayCalculator()
      : base(BeMinYear,
        BeMaxYear,
        AverageDaysPer10Years,
        UnixEpochDayAtStartOfYear1)
    {
      if (_nawRuzOffsetFrom21 == null)
      {
        new WondrousCalendarStaticKnowledge().PepareExternalKnowledge(out _nawRuzOffsetFrom21);
      }
    }

    const int AyyamiHaMonthNum = -1;
    /// <summary>
    /// </summary>
    /// <remarks>
    ///   There are 19 months in a year. Between the 18th and 19th month are the Ayyam-i-Ha (days of Ha).
    ///   Options for counting months:
    ///   - treat Ayyam-i-Ha as month 0. This will cause problems if 0 is treated as unknown
    ///   - treat Ayyam-i-Ha as month 20. This will cause problems when months are sorted.
    ///   - treat Ayyam-i-Ha as month 19. This is confusing because the 19th month would be month 20.
    ///   - treat Ayyam-i-Ha as month -1. This might work...
    /// </remarks>
    /// <param name="month"></param>
    /// <returns></returns>
    private bool IsAyyamiHa(int month)
    {
      return month == AyyamiHaMonthNum;
    }

    protected override int GetDaysFromStartOfYearToStartOfMonth(int year, int month)
    {
      if (month >= 1 && month <= 18)
      {
        return DaysInMonth * (month - 1);
      }
      if (IsAyyamiHa(month))
      {
        return DaysInMonth * 19;
      }

      // month 19
      return (GregorianDateOfNawRuz(year + 1) - GregorianDateOfNawRuz(year)).Days - DaysInMonth;
    }

    /// <param name="year"></param>
    /// <remarks>Rely on Gregorian calendar for "days" in unix epoch</remarks>
    /// <remarks>Untested</remarks>
    /// <returns></returns>
    protected override int CalculateStartOfYearDays(int year)
    {
      var gregorianYear = year + 1843;

      var gregorianStart = new GregorianYearMonthDayCalculator().GetStartOfYearInDays(gregorianYear);

      var dayInMarch = GregorianDateOfNawRuz(year);

      return gregorianStart + dayInMarch.DayOfYear;
    }

    /// <param name="year"></param>
    /// <remarks>All years have 19 months</remarks>
    /// <remarks>Tested</remarks>
    /// <returns></returns>
    internal override int GetMonthsInYear(int year)
    {
      return 19;
    }

    internal override int GetDaysInMonth(int year, int month)
    {
      if (IsAyyamiHa(month))
      {
        return (GregorianDateOfNawRuz(year + 1) - GregorianDateOfNawRuz(year)).Days - 361;
      }
      return DaysInMonth;
    }

    internal override bool IsLeapYear(int year)
    {
      return (GregorianDateOfNawRuz(year + 1) - GregorianDateOfNawRuz(year)).Days > 365;
    }

    /// <param name="yearMonthDay"></param>
    /// <param name="months"></param>
    /// <remarks>Adding months around Ayyam-i-Ha is not well defined. Because they are "days outside of time", this
    /// implementation will skip over them. </remarks>
    /// <remarks>Untested, especially for months > 19 or negative</remarks>
    /// <returns></returns>
    internal override YearMonthDay AddMonths(YearMonthDay yearMonthDay, int months)
    {
      if (months == 0)
      {
        return yearMonthDay;
      }

      var thisMonth = yearMonthDay.Month;
      var thisYear = yearMonthDay.Year;

      if (thisMonth == AyyamiHaMonthNum)
      {
        // if moving forward, start from month 18
        // if moving back, start from month 19
        thisMonth = months > 0 ? 18 : 19;
      }
    
      // handle wrap around to future and to past
      var nextMonth = thisMonth + months;
      var nextYear = thisYear + nextMonth % 19;
      nextMonth = nextMonth / 19;

      CheckIfYearIsValid(nextYear);

      return new YearMonthDay(nextYear, nextMonth, yearMonthDay.Day);
    }

    bool CheckIfYearIsValid(int year, bool throwException = true)
    {
      var isOkay = year >= BeMinYear && year <= BeMaxYear;
      if (throwException)
      {
        throw new ArgumentOutOfRangeException("year", year, "Year outside of defined range");
      }
      return isOkay;
    }

    /// <param name="minuendDate"></param>
    /// <param name="subtrahendDate"></param>
    /// <remarks>Untested</remarks>
    /// <returns></returns>
    internal override int MonthsBetween(YearMonthDay minuendDate, YearMonthDay subtrahendDate)
    {
      return minuendDate.Year*19 + minuendDate.Month - (subtrahendDate.Year*19 + subtrahendDate.Month);
    }

    internal override YearMonthDay SetYear(YearMonthDay yearMonthDay, int year)
    {
      var month = yearMonthDay.Month;

      switch (month)
      {
        case AyyamiHaMonthNum:
        case 19:
          return new YearMonthDay();//TODO
          break;

        default:
          return new YearMonthDay(year, month, yearMonthDay.Day);
      }
    }


    internal override int GetDaysSinceEpoch(YearMonthDay yearMonthDay) =>
      // Just inline the arithmetic that would be done via various methods.
      GetStartOfYearInDays(yearMonthDay.Year)
      + (yearMonthDay.Month - 1) * DaysInMonth
      + (yearMonthDay.Day - 1);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="year"></param>
    /// <remarks>Can only know by looking at when next year starts.</remarks>
    /// <remarks>Untested</remarks>
    /// <returns></returns>
    internal override int GetDaysInYear(int year)
      => (int)Math.Floor((GregorianDateOfNawRuz(year + 1) - GregorianDateOfNawRuz(year)).TotalDays);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="year">Wondrous Year</param>
    /// <param name="dayOfYear"></param>
    /// <remarks>Untested</remarks>
    /// <returns></returns>
    internal override YearMonthDay GetYearMonthDay(int year, int dayOfYear)
    {
      var zeroBasedDayOfYear = dayOfYear - 1;
      var month = zeroBasedDayOfYear / DaysInMonth + 1;
      if (month <= 18)
      {
        var day = zeroBasedDayOfYear % DaysInMonth + 1;
        return new YearMonthDay(year, month, day);
      }

      // in Ayyam-i-Ha or Loftiness
      var firstOfLoftiness = DayOfYearOfStartOfMonth19(year);
      if (dayOfYear < firstOfLoftiness)
      {
        var dayBeforeAyyamiHa = 18 * DaysInMonth;
        var dayInAyyamiHa = dayOfYear - dayBeforeAyyamiHa;

        Debug.Assert(dayInAyyamiHa >= 1 && dayInAyyamiHa <= 5, "Calc error 1");

        return new YearMonthDay(year, AyyamiHaMonthNum, dayInAyyamiHa);
      }

      return new YearMonthDay(year, month, dayOfYear - firstOfLoftiness + 1);
    }

    /// <param name="bYear"></param>
    /// <remarks>The exact date of the equinox in Tehran. Look up in precalculated table.</remarks>
    /// <remarks>Untested</remarks>
    /// <returns></returns>
    private DateTime GregorianDateOfNawRuz(int bYear)
    {
      return new DateTime(1844 + (bYear - 1), 3, 21 + (bYear < FirstYearInNewModel ? 0 : _nawRuzOffsetFrom21[bYear]));
    }

    /// <param name="bYear"></param>
    /// <remarks>Check Naw-Ruz of next year, then subtract <see cref="DaysInMonth"/>.</remarks>
    /// <remarks>Untested</remarks>
    /// <returns></returns>
    private int DayOfYearOfStartOfMonth19(int bYear)
    {
      return GetDaysInYear(bYear) - DaysInMonth;
    }

  }
}

