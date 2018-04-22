// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;
using System;
using System.Diagnostics.CodeAnalysis;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Base class for the three variants of the Persian (Solar Hijri) calendar.
    /// Concrete subclasses are nested to allow different start dates and leap year calculations.
    /// </summary>
    /// <remarks>
    /// The constructor uses IsLeapYear to precompute lots of data; it is therefore important that
    /// the implementation of IsLeapYear in subclasses uses no instance fields.
    /// </remarks>
    internal abstract class PersianYearMonthDayCalculator : RegularYearMonthDayCalculator
    {
        private const int DaysPerNonLeapYear = (31 * 6) + (30 * 5) + 29;
        private const int DaysPerLeapYear = DaysPerNonLeapYear + 1;
        // Approximation; it'll be pretty close in all variants.
        private const int AverageDaysPer10Years = (DaysPerNonLeapYear * 25 + DaysPerLeapYear * 8) * 10 / 33;
        internal const int MaxPersianYear = 9377;

        private static readonly int[] TotalDaysByMonth;

        private readonly int[] startOfYearInDaysCache;

        static PersianYearMonthDayCalculator()
        {
            int days = 0;
            TotalDaysByMonth = new int[13];
            for (int i = 1; i <= 12; i++)
            {
                TotalDaysByMonth[i] = days;
                int daysInMonth = i <= 6 ? 31 : 30;
                // This doesn't take account of leap years, but that doesn't matter - because
                // it's not used on the last iteration, and leap years only affect the final month
                // in the Persian calendar.
                days += daysInMonth;
            }
        }

        private PersianYearMonthDayCalculator(int daysAtStartOfYear1)
            : base(1, MaxPersianYear, 12, AverageDaysPer10Years, daysAtStartOfYear1)
        {
            startOfYearInDaysCache = new int[MaxYear + 2];
            int startOfYear = DaysAtStartOfYear1 - GetDaysInYear(0);
            for (int year = 0; year <= MaxYear + 1; year++)
            {
                startOfYearInDaysCache[year] = startOfYear;
                startOfYear += GetDaysInYear(year);
            }
        }

        protected sealed override int GetDaysFromStartOfYearToStartOfMonth(int year, int month) => TotalDaysByMonth[month];

        internal sealed override int GetStartOfYearInDays(int year)
        {
            Preconditions.DebugCheckArgumentRange(nameof(year), year, MinYear - 1, MaxYear + 1);
            return startOfYearInDaysCache[year];
        }

#if NET45
        [ExcludeFromCodeCoverage]
#endif
        protected sealed override int CalculateStartOfYearDays(int year)
        {
            // This would only be called from GetStartOfYearInDays, which is overridden.
            throw new NotImplementedException();
        }

        internal sealed override YearMonthDay GetYearMonthDay(int year, int dayOfYear)
        {
            int dayOfYearZeroBased = dayOfYear - 1;
            int month;
            int day;
            if (dayOfYear == DaysPerLeapYear)
            {
                // Last day of a leap year.
                month = 12;
                day = 30;
            }
            else if (dayOfYearZeroBased < 6 * 31)
            {
                // In the first 6 months, all of which are 31 days long.
                month = dayOfYearZeroBased / 31 + 1;
                day = (dayOfYearZeroBased % 31) + 1;
            }
            else
            {
                // Last 6 months (other than last day of leap year).
                // Work out where we are within that 6 month block, then use simple arithmetic.
                int dayOfSecondHalf = dayOfYearZeroBased - 6 * 31;
                month = dayOfSecondHalf / 30 + 7;
                day = (dayOfSecondHalf % 30) + 1;
            }
            return new YearMonthDay(year, month, day);
        }

        internal sealed override int GetDaysInMonth(int year, int month) =>
            month < 7 ? 31
                : month < 12 ? 30
                : IsLeapYear(year) ? 30 : 29;

        internal sealed override int GetDaysInYear(int year) => IsLeapYear(year) ? DaysPerLeapYear : DaysPerNonLeapYear;

        /// <summary>
        /// Persian calendar using the simple 33-year cycle of 1, 5, 9, 13, 17, 22, 26, or 30.
        /// This corresponds to System.Globalization.PersianCalendar before .NET 4.6.
        /// </summary>
        internal class Simple : PersianYearMonthDayCalculator
        {
            // This is a long because we're notionally handling 33 bits. The top bit is
            // false anyway, but IsLeapYear shifts a long for simplicity, so let's be consistent with that.
            private const long LeapYearPatternBits = (1L << 1) | (1L << 5) | (1L << 9) | (1L << 13)
                | (1L << 17) | (1L << 22) | (1L << 26) | (1L << 30);
            private const int LeapYearCycleLength = 33;
            private const int DaysPerLeapCycle = DaysPerNonLeapYear * 25 + DaysPerLeapYear * 8;

            /// <summary>The ticks for the epoch of March 21st 622CE.</summary>
            private const int DaysAtStartOfYear1Constant = -492268;


            internal Simple() : base(DaysAtStartOfYear1Constant)
            {
            }

            /// <summary>
            /// Leap year condition using the simple 33-year cycle of 1, 5, 9, 13, 17, 22, 26, or 30.
            /// This corresponds to System.Globalization.PersianCalendar before .NET 4.6.
            /// </summary>
            internal override bool IsLeapYear(int year)
            {
                // Handle negative years in order to make calculations near the start of the calendar work cleanly.
                int yearOfCycle = year >= 0 ? year % LeapYearCycleLength
                                            : (year % LeapYearCycleLength) + LeapYearCycleLength;
                // Note the shift of 1L rather than 1, to avoid issues where shifting by 32
                // would get us back to 1.
                long key = 1L << yearOfCycle;
                return (LeapYearPatternBits & key) > 0;
            }
        }

        /// <summary>
        /// Persian calendar based on Birashk's subcycle/cycle/grand cycle scheme.
        /// </summary>
        internal class Arithmetic : PersianYearMonthDayCalculator
        {
            internal Arithmetic() : base(-492267)
            {
            }

            internal override bool IsLeapYear(int year)
            {
                // Offset the cycles for easier arithmetic.
                int offsetYear = year > 0 ? year - 474 : year - 473;
                int cycleYear = (offsetYear % 2820) + 474;
                return ((cycleYear + 38) * 31) % 128 < 31;
            }
        }

        /// <summary>
        /// Persian calendar based on stored BCL 4.6 information (avoids complex arithmetic for
        /// midday in Tehran).
        /// </summary>
        internal class Astronomical : PersianYearMonthDayCalculator
        {
            // Ugly, but the simplest way of embedding a big chunk of binary data...
            private static readonly byte[] AstronomicalLeapYearBits = Convert.FromBase64String(
                "ICIiIkJERESEiIiICBEREREiIiJCREREhIiIiAgRERERIiIiIkRERISIiIiIEBERESEiIiJEREREiIiI" +
                "iBAREREhIiIiQkRERISIiIgIERERESIiIkJERESEiIiICBEREREiIiIiRERERIiIiIgQERERISIiIkJE" +
                "RESEiIiICBEREREiIiIiREREhIiIiAgRERERISIiIkRERESIiIiIEBERESEiIiJCREREhIiIiAgRERER" +
                "IiIiIkRERISIiIgIERERESEiIiJEREREiIiIiBAREREhIiIiQkRERISIiIgIERERESIiIiJEREREiIiI" +
                "iBAREREhIiIiQkRERIiIiIgQERERISIiIiJERESEiIiICBEREREiIiIiRERERIiIiIgQERERISIiIkJE" +
                "RESEiIiICBEREREiIiIiRERERIiIiAgRERERIiIiIkJERESEiIiIEBERESEiIiJCRERERIiIiAgRERER" +
                "IiIiIkRERESIiIiIEBERESEiIiJCREREhIiIiAgREREhIiIiIkRERESIiIiIEBERESIiIiJEREREiIiI" +
                "iBAREREhIiIiQkRERISIiIgIERERESIiIiJEREREiIiIiBAREREiIiIiRERERIiIiIgQERERISIiIkJE" +
                "RESEiIiICBERESEiIiJCREREhIiIiAgRERERIiIiIkRERESIiIiIEBERESIiIiJEREREiIiIiBAREREh" +
                "IiIiQkRERISIiIgIERERISIiIkJERESEiIiICBEREREiIiIiRERERIiIiAgRERERIiIiIkRERESIiIgI" +
                "ERERESIiIiJEREREiIiIiBAREREhIiIiQkRERIiIiIgQERERISIiIkJERESIiIiIEBERESEiIiJCRERE" +
                "iIiIiBAREREhIiIiQkRERIiIiIgQERERISIiIkRERESIiIiIEBERESIiIiJEREREiIiIiBAREREiIiIi" +
                "RERERIiIiAgRERERIiIiIkRERISIiIgIERERESIiIkJERESEiIiIEBERESEiIiJEREREiIiIiBAREREi" +
                "IiIiRERERIiIiAgRERERIiIiIkRERISIiIgQERERISIiIkJERESIiIiIEBERESEiIiJERESEiIiICBER" +
                "ESEiIiJCREREhIiIiBAREREhIiIiREREhIiIiAgRERERIiIiQkRERIiIiIgQERERIiIiIkRERISIiIgQ" +
                "ERERISIiIkRERESIiIgIERERISIiIkJERESIiIiIEBERESIiIkJERESIiIiIEBERESIiIiJERESEiIiI" +
                "EBERESEiIiJERESEiIiICBERESEiIiJEREREiIiICBERESEiIiJERESEiIiICBERESEiIiJEREREiIiI" +
                "CBERESEiIiJERESEiIiICBERESEiIiJERESEiIiICBERESEiIiJERESEiIiIEBERESIiIiJERESEiIiI" +
                "EBERESIiIkJERESIiIgIERERISIiIkRERESIiIgIERERISIiIkRERISIiIgQERERIiIiQkRERIiIiAgR" +
                "EREhIiIiREREhIiIiBAREREiIiJCREREiIiICBERESEC"
            );

            internal Astronomical() : base(-492267)
            {
            }

            // 8 years per byte.
            internal override bool IsLeapYear(int year) => (AstronomicalLeapYearBits[year >> 3] & (1 << (year & 7))) != 0;
        }
    }
}
