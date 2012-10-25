#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using NodaTime.Fields;
using NodaTime.Utility;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Implements a pure proleptic Gregorian calendar system, which defines every
    /// fourth year as leap, unless the year is divisible by 100 and not by 400.
    /// This improves upon the Julian calendar leap year rule.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Although the Gregorian calendar did not exist before 1582 CE, this
    /// calendar assumes it did, thus it is proleptic. This implementation also
    /// fixes the start of the year at January 1, and defines the year zero.
    /// </para>
    /// <para>
    /// With the exception of century related fields, the ISO calendar system is exactly the
    /// same as the Gregorian one, so they share the same implementation - the ISO instance
    /// is constructed with a slightly different field assembler.
    /// </para>
    /// <para>
    /// Instances of this class are exposed via <see cref="CalendarSystem.GetGregorianCalendar"/>
    /// and <see cref="CalendarSystem.Iso"/>
    /// </para>
    /// </remarks>
    internal sealed class GregorianCalendarSystem : BasicGJCalendarSystem
    {
        private const string GregorianName = "Gregorian";
        private const string IsoName = "ISO";

        // We precompute useful values for each month between these years, as we anticipate most
        // dates will be in this range.
        private const int FirstOptimizedYear = 1900;
        private const int LastOptimizedYear = 2100;
        private static readonly long[] MonthStartTicks = new long[(LastOptimizedYear + 1 - FirstOptimizedYear) * 12 + 1];
        private static readonly int[] MonthLengths = new int[(LastOptimizedYear + 1 - FirstOptimizedYear) * 12 + 1];
        private static readonly long[] YearStartTicks = new long[LastOptimizedYear + 1 - FirstOptimizedYear];

        private const int DaysFrom0000To1970 = 719527;
        private const long AverageTicksPerGregorianYear = (long)(365.2425m * NodaConstants.TicksPerStandardDay);

        private static readonly GregorianCalendarSystem[] GregorianCache;

        static GregorianCalendarSystem()
        {
            GregorianCache = new GregorianCalendarSystem[7];
            for (int i = 0; i < 7; i++)
            {
                GregorianCache[i] = new GregorianCalendarSystem(GregorianName, i + 1, null);
            }

            var sample = GregorianCache[0];            
            for (int year = FirstOptimizedYear; year <= LastOptimizedYear; year++)
            {
                YearStartTicks[year - FirstOptimizedYear] = sample.CalculateStartOfYear(year).Ticks;
                for (int month = 1; month <= 12; month++)
                {
                    int yearMonthIndex = (year - FirstOptimizedYear) * 12 + month;
                    MonthStartTicks[yearMonthIndex] = sample.GetYearMonthTicks(year, month);
                    MonthLengths[yearMonthIndex] = sample.GetDaysInMonth(year, month);
                }
            }
        }

        /// <summary>
        /// Returns the instance of the Gregorian calendar system with the given number of days in the week.
        /// </summary>
        /// <param name="minDaysInFirstWeek">The minimum number of days at the start of the year to consider it
        /// a week in that year as opposed to at the end of the previous year.</param>
        internal static GregorianCalendarSystem GetInstance(int minDaysInFirstWeek)
        {
            if (minDaysInFirstWeek < 1 || minDaysInFirstWeek > 7)
            {
                throw new ArgumentOutOfRangeException("minDaysInFirstWeek", "Minimum days in first week must be between 1 and 7 inclusive");
            }
            return GregorianCache[minDaysInFirstWeek - 1];
        }

        private GregorianCalendarSystem(string name, int minDaysInFirstWeek, FieldAssembler fieldAssembler)
            : base(name, minDaysInFirstWeek, -27256, 31196, fieldAssembler)
        {
        }

        internal override long AverageTicksPerYear { get { return AverageTicksPerGregorianYear; } }
        internal override long AverageTicksPerYearDividedByTwo { get { return AverageTicksPerGregorianYear / 2; } }
        internal override long AverageTicksPerMonth { get { return (long)(365.2425m * NodaConstants.TicksPerStandardDay / 12); } }
        internal override long ApproxTicksAtEpochDividedByTwo { get { return (1970 * AverageTicksPerGregorianYear) / 2; } }

        internal override long GetYearTicks(int year)
        {
            if (year < FirstOptimizedYear || year > LastOptimizedYear)
            {
                return base.GetYearTicks(year);
            }
            return YearStartTicks[year - FirstOptimizedYear];
        }

        internal override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute,
                                                       int millisecondOfSecond, int tickOfMillisecond)
        {
            int yearMonthIndex = (year - FirstOptimizedYear) * 12 + monthOfYear;
            if (year < FirstOptimizedYear || year > LastOptimizedYear - 1 || monthOfYear < 1 || monthOfYear > 12 || dayOfMonth < 1 ||
                dayOfMonth > MonthLengths[yearMonthIndex] || hourOfDay < 0 || hourOfDay > 23 || minuteOfHour < 0 || minuteOfHour > 59 || secondOfMinute < 0 ||
                secondOfMinute > 59 || millisecondOfSecond < 0 || millisecondOfSecond > 999 || tickOfMillisecond < 0 ||
                tickOfMillisecond > NodaConstants.TicksPerMillisecond - 1)
            {
                // It may still be okay - let's take the long way to find out
                return base.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute, millisecondOfSecond, tickOfMillisecond);
            }
            // This is guaranteed not to overflow, as we've already validated the arguments
            return new LocalInstant(unchecked(
                MonthStartTicks[yearMonthIndex] + (dayOfMonth - 1) * NodaConstants.TicksPerStandardDay + hourOfDay * NodaConstants.TicksPerHour +
                minuteOfHour * NodaConstants.TicksPerMinute + secondOfMinute * NodaConstants.TicksPerSecond +
                millisecondOfSecond * NodaConstants.TicksPerMillisecond + tickOfMillisecond));
        }

        internal override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute)
        {
            int yearMonthIndex = (year - FirstOptimizedYear) * 12 + monthOfYear;
            if (year < FirstOptimizedYear || year > LastOptimizedYear - 1 || monthOfYear < 1 || monthOfYear > 12 || dayOfMonth < 1 ||
                dayOfMonth > MonthLengths[yearMonthIndex] || hourOfDay < 0 || hourOfDay > 23 || minuteOfHour < 0 || minuteOfHour > 59 || secondOfMinute < 0 ||
                secondOfMinute > 59)
            {
                // It may still be okay - let's take the long way to find out
                return base.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute);
            }
            // This is guaranteed not to overflow, as we've already validated the arguments
            return new LocalInstant(unchecked(
                MonthStartTicks[yearMonthIndex] + (dayOfMonth - 1) * NodaConstants.TicksPerStandardDay + hourOfDay * NodaConstants.TicksPerHour +
                minuteOfHour * NodaConstants.TicksPerMinute + secondOfMinute * NodaConstants.TicksPerSecond));
        }

        internal override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour)
        {
            int yearMonthIndex = (year - FirstOptimizedYear) * 12 + monthOfYear;
            if (year < FirstOptimizedYear || year > LastOptimizedYear - 1 || monthOfYear < 1 || monthOfYear > 12 || dayOfMonth < 1 ||
                dayOfMonth > MonthLengths[yearMonthIndex] || hourOfDay < 0 || hourOfDay > 23 || minuteOfHour < 0 || minuteOfHour > 59)
            {
                // It may still be okay - let's take the long way to find out
                return base.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour);
            }
            // This is guaranteed not to overflow, as we've already validated the arguments
            return new LocalInstant(unchecked(
                MonthStartTicks[yearMonthIndex] + (dayOfMonth - 1) * NodaConstants.TicksPerStandardDay + hourOfDay * NodaConstants.TicksPerHour +
                minuteOfHour * NodaConstants.TicksPerMinute));
        }

        internal override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, long tickOfDay)
        {
            int yearMonthIndex = (year - FirstOptimizedYear) * 12 + monthOfYear;
            if (year < FirstOptimizedYear || year > LastOptimizedYear - 1 || monthOfYear < 1 || monthOfYear > 12 || dayOfMonth < 1 ||
                dayOfMonth > MonthLengths[yearMonthIndex] || tickOfDay < 0 || tickOfDay >= NodaConstants.TicksPerStandardDay)
            {
                return base.GetLocalInstant(year, monthOfYear, dayOfMonth, tickOfDay);
            }
            // This is guaranteed not to overflow, as we've already validated the arguments
            return new LocalInstant(unchecked(MonthStartTicks[yearMonthIndex] + (dayOfMonth - 1) * NodaConstants.TicksPerStandardDay + tickOfDay));
        }

        protected override LocalInstant CalculateStartOfYear(int year)
        {
            if (year < MinYear)
            {
                return LocalInstant.MinValue;
            }
            if (year > MaxYear)
            {
                return LocalInstant.MaxValue;
            }

            // Initial value is just temporary.
            int leapYears = year / 100;
            if (year < 0)
            {
                // Add 3 before shifting right since /4 and >>2 behave differently
                // on negative numbers. When the expression is written as
                // (year / 4) - (year / 100) + (year / 400),
                // it works for both positive and negative values, except this optimization
                // eliminates two divisions.
                leapYears = ((year + 3) >> 2) - leapYears + ((leapYears + 3) >> 2) - 1;
            }
            else
            {
                leapYears = (year >> 2) - leapYears + (leapYears >> 2);
                if (IsLeapYear(year))
                {
                    leapYears--;
                }
            }

            return new LocalInstant((year * 365L + (leapYears - DaysFrom0000To1970)) * NodaConstants.TicksPerStandardDay);
        }

        public override bool IsLeapYear(int year)
        {
            return ((year & 3) == 0) && ((year % 100) != 0 || (year % 400) == 0);
        }

        /// <summary>
        /// Separate class to ensure that GregorianCalendarSystem can be properly initialized first.
        /// We need that to be valid so that IsoYearOfEraDateTimeField.Instance can be initialized appropriately. Ick.
        /// </summary>
        internal static class IsoHelper
        {
            internal static readonly GregorianCalendarSystem Instance = new GregorianCalendarSystem(IsoName, 4, AssembleIsoFields);

            /// <summary>
            /// Field assembly used solely for the ISO calendar variation.
            /// </summary>
            private static void AssembleIsoFields(FieldSet.Builder builder, CalendarSystem @this)
            {
                // Use zero based century and year of century.
                DividedDateTimeField centuryOfEra = new DividedDateTimeField(IsoYearOfEraDateTimeField.Instance, DateTimeFieldType.CenturyOfEra, 100);
                builder.CenturyOfEra = centuryOfEra;
                builder.YearOfCentury = new RemainderDateTimeField(centuryOfEra, DateTimeFieldType.YearOfCentury);
                builder.WeekYearOfCentury = new RemainderDateTimeField(centuryOfEra, DateTimeFieldType.WeekYearOfCentury);
                builder.Centuries = centuryOfEra.PeriodField;
            }
        }
    }
}
