#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
    /// Original name: GregorianChronology
    /// </summary>
    public class GregorianCalendarSystem : BasicGJCalendarSystem
    {
        private const string GregorianName = "Gregorian";

        private const int DaysFrom0000To1970 = 719527;
        private const long AverageTicksPerGregorianYear = (long)(365.2425m * NodaConstants.TicksPerDay);

        // TODO: Consider making this public, but with a different name?
        // It roughly maps onto GregorianChronology.getInstanceUTC() except of course we don't have a time zone...
        internal static readonly GregorianCalendarSystem Default = new GregorianCalendarSystem(4);

        private GregorianCalendarSystem(int minDaysInFirstWeek) : base(GregorianName, null, minDaysInFirstWeek)
        {
        }

        protected override void AssembleFields(FieldSet.Builder builder)
        {
            // TODO: This pattern appears all over the place. It *may* not be necessary
            // now we've separated out all the time zone stuff.
            if (BaseCalendar == null)
            {
                base.AssembleFields(builder);
            }
        }

        public override long AverageTicksPerYear { get { return AverageTicksPerGregorianYear; } }
        public override long AverageTicksPerYearDividedByTwo { get { return AverageTicksPerGregorianYear / 2; } }
        public override long AverageTicksPerMonth { get { return (long)(365.2425m * NodaConstants.TicksPerDay / 12); } }
        public override long ApproxTicksAtEpochDividedByTwo { get { return (1970 * AverageTicksPerGregorianYear) / 2; } }
        // TODO: Check that this is still valid now we've moved to ticks. I suspect it's not... (divide by 10000?)
        public override int MinYear { get { return -27258; } }
        public override int MaxYear { get { return 31196; } }

        public static Chronology GetInstance(IDateTimeZone dateTimeZone)
        {
            throw new NotImplementedException();
        }

        protected override LocalInstant CalculateStartOfYear(int year)
        {
            if (year <= MinYear)
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

            return new LocalInstant((year * 365L + (leapYears - DaysFrom0000To1970)) * NodaConstants.TicksPerDay);
        }

        protected internal override bool IsLeapYear(int year)
        {
            return ((year & 3) == 0) && ((year % 100) != 0 || (year % 400) == 0);
        }
    }
}