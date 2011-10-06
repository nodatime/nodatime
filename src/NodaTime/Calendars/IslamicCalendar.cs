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

namespace NodaTime.Calendars
{
    /// <summary>
    /// Islamic (Hijri) calendar. See documentation for <see cref="CalendarSystem.GetIslamicCalendar"/> for
    /// more details.
    /// </summary>
    internal sealed class IslamicCalendar : BasicCalendarSystem
    {
        /// <summary>Singleton era field.</summary>
        private static readonly DateTimeField EraField = new BasicSingleEraDateTimeField(Era.AnnoHegirae);

        // TODO: Validate these.
        /// <summary>
        /// The highest year that can be fully supported. It's possible that
        /// a few years above this can be partially supported, but some calculations may overflow.
        /// </summary>
        public override int MaxYear { get { return 29226; } }

        const int RealMinYear = -29225;

        /// <summary>The lowest year that can be fully supported.</summary>
        public override int MinYear { get { return 1; } }

        /// <summary>Days in a pair of months, in days.</summary>
        private const int MonthPairLength = 59;

        /// <summary>The length of a long month, in days.</summary>
        private const int LongMonthLength = 30;

        /// <summary>The length of a short month, in days.</summary>
        private const int ShortMonthLength = 29;

        /// <summary>The length of a pair of months, in ticks./</summary>
        private const long TicksPerMonthPair = MonthPairLength * NodaConstants.TicksPerStandardDay;

        /// <summary>The length of a long month, in ticks./</summary>
        private const long TicksPerLongMonth = LongMonthLength * NodaConstants.TicksPerStandardDay;

        /// <summary>The ticks in a typical month.</summary>
        private const long TicksPerMonth = (long) (29.53056 * NodaConstants.TicksPerStandardDay);

        /// <summary>The typical number of ticks in a year.</summary>
        private const long TicksPerYear = (long) (354.36667 * NodaConstants.TicksPerStandardDay);

        /// <summary>The number of ticks in a non-leap year.</summary>
        private const long TicksPerNonLeapYear = 354 * NodaConstants.TicksPerStandardDay;

        /// <summary>The number of ticks in a leap year.</summary>
        private const long TicksPerLeapYear = 355 * NodaConstants.TicksPerStandardDay;

        /// <summary>The ticks for the civil (Friday) epoch of July 16th 622CE.</summary>
        private const long TicksAtCivilEpoch = -425215872000000000L;

        /// <summary>The ticks for the civil (Thursday) epoch of July 15th 622CE.</summary>
        private const long TicksAtAstronomicalEpoch = TicksAtCivilEpoch - NodaConstants.TicksPerStandardDay;

        /// <summary>The length of the cycle of leap years.</summary>
        private const int LeapYearCycleLength = 30;

        /// <summary>The number of ticks in leap cycle.</summary>
        private const long TicksPerLeapCycle = 19L * TicksPerNonLeapYear + 11 * TicksPerLeapYear;

        /// <summary>The pattern of leap years within a cycle, one bit per year, for this calendar.</summary>
        private readonly int leapYearPatternBits;
        /// <summary>The ticks at the start of the epoch for this calendar.</summary>
        private readonly long epochTicks;

        // TODO: Caching
        internal static CalendarSystem GetInstance(IslamicLeapYearPattern leapYearPattern, IslamicEpoch epoch)
        {
            int patternBits = GetLeapYearPatternBits(leapYearPattern);
            long epochTicks = GetEpochTicks(epoch);
            return new IslamicCalendar(patternBits, epochTicks);
        }

        private IslamicCalendar(int leapYearPatternBits, long epochTicks) : base("Hijri (Islamic)", 4, AssembleFields, new[] { Era.AnnoHegirae })
        {
            this.leapYearPatternBits = leapYearPatternBits;
            this.epochTicks = epochTicks;
        }

        private static void AssembleFields(FieldSet.Builder builder, CalendarSystem @this)
        {
            builder.Era = EraField;
            builder.MonthOfYear = new BasicMonthOfYearDateTimeField((BasicCalendarSystem) @this, 12);
            builder.Months = builder.MonthOfYear.DurationField;
        }

        internal override int GetYear(LocalInstant localInstant)
        {
            long ticksIslamic = localInstant.Ticks - epochTicks;
            long cycles = ticksIslamic / TicksPerLeapCycle;
            long cycleRemainder = ticksIslamic % TicksPerLeapCycle;

            int year = (int)((cycles * LeapYearCycleLength) + 1L);
            long yearTicks = IsLeapYear(year) ? TicksPerLeapYear : TicksPerNonLeapYear;
            while (cycleRemainder >= yearTicks)
            {
                cycleRemainder -= yearTicks;
                year++;
                yearTicks = IsLeapYear(year) ? TicksPerLeapYear : TicksPerNonLeapYear;
            }
            return year;
        }

        internal override LocalInstant SetYear(LocalInstant localInstant, int year)
        {
            // Optimized implementation of SetYear, due to fixed months.
            int thisYear = GetYear(localInstant);
            int dayOfYear = GetDayOfYear(localInstant, thisYear);
            long tickOfDay = GetTickOfDay(localInstant);

            if (dayOfYear > 354)
            {
                // Current year is leap, and day is leap.
                if (!IsLeapYear(year))
                {
                    // Moving to a non-leap year, leap day doesn't exist.
                    dayOfYear--;
                }
            }
            // TODO: Use GetYearTicks and just add the dayOfYear * ticks per day?
            long resultTicks = GetYearMonthDayTicks(year, 1, dayOfYear);
            resultTicks += tickOfDay;
            return new LocalInstant(resultTicks);
        }

        internal override long GetYearDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            int minuendYear = GetYear(minuendInstant);
            int subtrahendYear = GetYear(subtrahendInstant);

            // Inlined remainder to avoi duplicate calls to Get.
            long minuendRem = minuendInstant.Ticks - GetYearTicks(minuendYear);
            long subtrahendRem = subtrahendInstant.Ticks - GetYearTicks(subtrahendYear);

            int difference = minuendYear - subtrahendYear;
            if (minuendRem < subtrahendRem)
            {
                difference--;
            }
            return difference;
        }

        protected override long GetTotalTicksByYearMonth(int year, int month)
        {
            if (--month % 2 == 1)
            {
                month /= 2;
                return month * TicksPerMonthPair + TicksPerLongMonth;
            }
            else
            {
                month /= 2;
                return month * TicksPerMonthPair;
            }
        }

        internal override int GetDayOfMonth(LocalInstant localInstant)
        {
            int dayOfYear = GetDayOfYear(localInstant) - 1;
            if (dayOfYear == 354)
            {
                return 30;
            }
            return (dayOfYear % MonthPairLength) % LongMonthLength + 1; 
        }

        public override bool IsLeapYear(int year)
        {
            int key = 1 << (year % 30);
            return (leapYearPatternBits & key) > 0;
        }

        internal override int GetDaysInYearMax()
        {
            return 355;
        }

        internal override int GetDaysInYear(int year)
        {
            return IsLeapYear(year) ? 355 : 354;
        }

        public override int GetDaysInMonth(int year, int month)
        {
            if (month == 12 && IsLeapYear(year))
            {
                return LongMonthLength;
            }
            return (--month % 2 == 0 ? LongMonthLength : ShortMonthLength);
        }

        internal override int GetDaysInMonthMax()
        {
            return LongMonthLength;
        }

        internal override int GetDaysInMonthMax(int month)
        {
            if (month == 12)
            {
                return LongMonthLength;
            }
            return (--month % 2 == 0 ? LongMonthLength : ShortMonthLength);
        }

        protected internal override int GetMonthOfYear(LocalInstant localInstant, int year)
        {
            int doyZeroBased = (int)((localInstant.Ticks - GetYearTicks(year)) / NodaConstants.TicksPerStandardDay);
            if (doyZeroBased == 354)
            {
                return 12;
            }
            return ((doyZeroBased * 2) / MonthPairLength) + 1;
        }

        internal override long AverageTicksPerYear { get { return TicksPerYear; } }

        internal override long AverageTicksPerYearDividedByTwo { get { return TicksPerYear / 2; } }

        internal override long AverageTicksPerMonth { get { return TicksPerMonth; } }

        protected override LocalInstant CalculateStartOfYear(int year)
        {
            if (year > MaxYear)
            {
                throw new ArgumentOutOfRangeException("year");
            }
            if (year < RealMinYear)
            {
                throw new ArgumentOutOfRangeException("year");
            }
            // TODO: Clarify this comment, which is incorrect as far as I can tell...
            // Unix epoch is 1970-01-01 Gregorian which is 0622-07-16 Islamic.
            // 0001-01-01 Islamic is -42520809600000L
            // would prefer to calculate against year zero, but leap year
            // can be in that year so it doesn't work
            year--;
            long cycle = year / LeapYearCycleLength;
            long ticks = epochTicks + cycle * TicksPerLeapCycle;
            int cycleRemainder = (year % LeapYearCycleLength) + 1;

            for (int i = 1; i < cycleRemainder; i++)
            {
                ticks += (IsLeapYear(i) ? TicksPerLeapYear : TicksPerNonLeapYear);
            }

            return new LocalInstant(ticks);
        }

        // Epoch 1970-01-01 ISO = 1389-10-22 Islamic
        internal override long ApproxTicksAtEpochDividedByTwo { get { return -epochTicks / 2; } }

        /// <summary>
        /// Returns the pattern of leap years within a cycle, one bit per year, for the specified pattern.
        /// </summary>
        private static int GetLeapYearPatternBits(IslamicLeapYearPattern leapYearPattern){
            switch (leapYearPattern)
            {
                case IslamicLeapYearPattern.Base15:        return 623158436;
                case IslamicLeapYearPattern.Base16:        return 623191204;
                case IslamicLeapYearPattern.Indian:        return 690562340;
                case IslamicLeapYearPattern.HabashAlHasib: return 153692453;
                default: throw new ArgumentOutOfRangeException("leapYearPattern");
            }
        }

        /// <summary>
        /// Returns the LocalInstant ticks at the specified epoch.
        /// </summary>
        private static long GetEpochTicks(IslamicEpoch epoch)
        {
            switch (epoch)
            {
                case IslamicEpoch.Astronomical: return TicksAtAstronomicalEpoch;
                case IslamicEpoch.Civil:        return TicksAtCivilEpoch;
                default: throw new ArgumentOutOfRangeException("epoch");
            }
        }
    }
}
