using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Calculator for the Hebrew calendar, as described at http://en.wikipedia.org/wiki/Hebrew_calendar.
    /// This is a purely mathematical calculator, applied proleptically to the period where the real
    /// calendar was observational.
    /// </summary>
    internal sealed class HebrewYearMonthDayCalculator : YearMonthDayCalculator
    {
        internal HebrewYearMonthDayCalculator()
            : base(1, 30000 /* FIXME */, 365 * NodaConstants.TicksPerStandardDay /* FIXME */, 0L /* FIXME */, new[] { Era.AnnoMundi })
        {

        }

        /// <summary>
        /// Bit mask of all the years in the 19 year cycle which are leap years.
        /// </summary>
        const int LeapYearBits = (1 << 0) | (1 << 3) | (1 << 6) | (1 << 8) | (1 << 11) | (1 << 14) | (1 << 17);

        /// <summary>
        /// Returns whether or not the given year is a leap year - that is, one with 13 months. This is
        /// not quite the same as a leap year in (say) the Gregorian calendar system...
        /// </summary>
        internal override bool IsLeapYear(int year)
        {
            // 0-based value
 	        int yearOfCycle = year % 19;
            return (LeapYearBits & (1 << yearOfCycle)) != 0;
        }



        protected override long GetTicksFromStartOfYearToStartOfMonth(int year, int month)
        {
            throw new NotImplementedException();
        }

        protected override int CalculateStartOfYearDays(int year)
        {
            throw new NotImplementedException();
        }

        protected override int GetMonthOfYear(LocalInstant localInstant, int year)
        {
            throw new NotImplementedException();
        }

        protected override long GetTicksInYear(int year)
        {
            throw new NotImplementedException();
        }

        internal override int GetMaxMonth(int year)
        {
            return IsLeapYear(year) ? 13 : 12;
        }

        internal override LocalInstant SetYear(LocalInstant localInstant, int year)
        {
            throw new NotImplementedException();
        }

        internal override int GetDaysInMonth(int year, int month)
        {
            throw new NotImplementedException();
        }

        internal override LocalInstant AddMonths(LocalInstant localInstant, int months)
        {
            throw new NotImplementedException();
        }

        internal override int MonthsBetween(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            throw new NotImplementedException();
        }

        internal struct Molad
        {
            // Synodic month is 29 days, 12 hours, and 793 parts. (1080 parts = 1 hour)

            private readonly int year;
            internal int Year { get { return year; } }

            private readonly int month;
            internal int Month { get { return month; } }

            private readonly int day;
            internal int Day { get { return day; } }

            private readonly int hours;
            internal int Hours { get { return hours; } }

            private readonly int parts;
            internal int Parts { get { return parts; } }

            private readonly DayOfWeek dayOfWeek;
            internal DayOfWeek DayOfWeek { get { return dayOfWeek; } }

            internal Molad(int year, int month, int day, DayOfWeek dayOfWeek, int hours, int parts)
            {
                this.year = year;
                this.month = month;
                this.day = day;
                this.dayOfWeek = dayOfWeek;
                this.hours = hours;
                this.parts = parts;
            }


        }
    }
}
