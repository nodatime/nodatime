// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Calendars
{
    /// <summary>
    /// Conversions between civil and ecclesiastical month numbers in the Hebrew calendar system.
    /// </summary>
    public static class HebrewMonthConverter
    {
        /// <summary>
        /// Given a civil month number and a year in which it occurs, this method returns
        /// the equivalent ecclesiastical month number.
        /// </summary>
        /// <remarks>
        /// No validation is performed in this method: an input month number of 13 in a non-leap-year
        /// will return a result of 7.
        /// </remarks>
        /// <param name="year">Year during which the month occurs.</param>
        /// <param name="month">Civil month number.</param>
        /// <returns>The ecclesiastical month number.</returns>
        public static int CivilToEcclesiastical(int year, int month)
        {
            if (month < 7)
            {
                return month + 6;
            }
            bool leapYear = HebrewEcclesiasticalCalculator.IsLeapYear(year);
            if (month == 7) // Adar II (13) or Nisan (1) depending on whether it's a leap year.
            {
                return leapYear ? 13 : 1;
            }
            return leapYear ? month - 7 : month - 6;
        }

        /// <summary>
        /// Given an ecclesiastical month number and a year in which it occurs, this method returns
        /// the equivalent ecclesiastical month number.
        /// </summary>
        /// <remarks>
        /// No validation is performed in this method: an input month number of 13 in a non-leap-year
        /// will return a result of 7.
        /// </remarks>
        /// <param name="year">Year during which the month occurs.</param>
        /// <param name="month">Civil month number.</param>
        /// <returns>The ecclesiastical month number.</returns>
        public static int EcclesiasticalToCivil(int year, int month)
        {
            if (month >= 7)
            {
                return month - 6;
            }
            return HebrewEcclesiasticalCalculator.IsLeapYear(year) ? month + 7 : month + 6;
        }
    }
}
