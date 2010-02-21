#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
    /// Implements a calendar system that follows the rules of the ISO8601 standard,
    /// which is compatible with Gregorian for all modern dates. This class is a singleton.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When ISO does not define a field, but it can be determined (such as AM/PM) it is included.
    /// </para>
    /// <para>
    /// With the exception of century related fields, ISOChronology is exactly the
    /// same as <see cref="GregorianCalendarSystem" />. In this chronology, centuries and year
    /// of century are zero based. For all years, the century is determined by
    /// dropping the last two digits of the year, ignoring sign. The year of century
    /// is the value of the last two year digits.
    /// </para>
    /// </remarks>
    public sealed class IsoCalendarSystem : AssembledCalendarSystem
    {
        public static readonly IsoCalendarSystem Instance = new IsoCalendarSystem(GregorianCalendarSystem.Default);

        // We precompute useful values for each month between these years, as we anticipate most
        // dates will be in this range.
        private const int FirstOptimizedYear = 1900;
        private const int LastOptimizedYear = 2100;
        private static readonly long[] MonthStartTicks = new long[(LastOptimizedYear + 1 - FirstOptimizedYear) * 12 + 1];
        private static readonly int[] MonthLengths = new int[(LastOptimizedYear + 1 - FirstOptimizedYear) * 12 + 1];

        static IsoCalendarSystem()
        {
            for (int year = FirstOptimizedYear; year <= LastOptimizedYear; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    int yearMonthIndex = (year - FirstOptimizedYear) * 12 + month;
                    MonthStartTicks[yearMonthIndex] = GregorianCalendarSystem.Default.GetYearMonthTicks(year, month);
                    MonthLengths[yearMonthIndex] = GregorianCalendarSystem.Default.GetDaysInYearMonth(year, month);
                }
            }
        }

        private IsoCalendarSystem(ICalendarSystem baseSystem)
            : base(baseSystem)
        {
        }

        protected override void AssembleFields(FieldSet.Builder fields)
        {
            if (fields == null)
            {
                throw new ArgumentNullException("fields");
            }
            // Use zero based century and year of century.
            DividedDateTimeField centuryOfEra = new DividedDateTimeField
                (IsoYearOfEraDateTimeField.Instance, DateTimeFieldType.CenturyOfEra, 100);
            fields.CenturyOfEra = centuryOfEra;
            fields.YearOfCentury = new RemainderDateTimeField(centuryOfEra, DateTimeFieldType.YearOfCentury);
            fields.WeekYearOfCentury = new RemainderDateTimeField(centuryOfEra, DateTimeFieldType.WeekYearOfCentury);
            fields.Centuries = centuryOfEra.DurationField;
        }

        public override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute, int millisecondOfSecond, int tickOfMillisecond)
        {
            int yearMonthIndex = (year - FirstOptimizedYear) * 12 + monthOfYear;
            if (year < FirstOptimizedYear || year > LastOptimizedYear - 1 ||
                monthOfYear < 1 || monthOfYear > 12 ||
                dayOfMonth < 1 || dayOfMonth > MonthLengths[yearMonthIndex] ||
                hourOfDay < 0 || hourOfDay > 23 ||
                minuteOfHour < 0 || minuteOfHour > 59 ||
                secondOfMinute < 0 || secondOfMinute > 59 ||
                millisecondOfSecond < 0 || millisecondOfSecond > 999 ||
                tickOfMillisecond < 0 || tickOfMillisecond > NodaConstants.TicksPerMillisecond - 1)
            {
                // It may still be okay - let's take the long way to find out
                return base.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute, millisecondOfSecond, tickOfMillisecond);
            }
            return new LocalInstant(MonthStartTicks[yearMonthIndex] + 
                (dayOfMonth - 1) * NodaConstants.TicksPerDay +
                hourOfDay * NodaConstants.TicksPerHour + minuteOfHour * NodaConstants.TicksPerMinute +
                secondOfMinute * NodaConstants.TicksPerSecond + millisecondOfSecond * NodaConstants.TicksPerMillisecond + tickOfMillisecond);
        }

        public override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute)
        {
            int yearMonthIndex = (year - FirstOptimizedYear) * 12 + monthOfYear;
            if (year < FirstOptimizedYear || year > LastOptimizedYear - 1 ||
                monthOfYear < 1 || monthOfYear > 12 ||
                dayOfMonth < 1 || dayOfMonth > MonthLengths[yearMonthIndex] ||
                hourOfDay < 0 || hourOfDay > 23 ||
                minuteOfHour < 0 || minuteOfHour > 59 ||
                secondOfMinute < 0 || secondOfMinute > 59)
            {
                // It may still be okay - let's take the long way to find out
                return base.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute);
            }
            return new LocalInstant(MonthStartTicks[yearMonthIndex] +
                (dayOfMonth - 1) * NodaConstants.TicksPerDay +
                hourOfDay * NodaConstants.TicksPerHour + minuteOfHour * NodaConstants.TicksPerMinute +
                secondOfMinute * NodaConstants.TicksPerSecond);
        }

        public override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour)
        {
            int yearMonthIndex = (year - FirstOptimizedYear) * 12 + monthOfYear;
            if (year < FirstOptimizedYear || year > LastOptimizedYear - 1 ||
                monthOfYear < 1 || monthOfYear > 12 ||
                dayOfMonth < 1 || dayOfMonth > MonthLengths[yearMonthIndex] ||
                hourOfDay < 0 || hourOfDay > 23 ||
                minuteOfHour < 0 || minuteOfHour > 59)
            {
                // It may still be okay - let's take the long way to find out
                return base.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour);
            }
            return new LocalInstant(MonthStartTicks[yearMonthIndex] +
                (dayOfMonth - 1) * NodaConstants.TicksPerDay +
                hourOfDay * NodaConstants.TicksPerHour + minuteOfHour * NodaConstants.TicksPerMinute);
        }

        // TODO: Try overriding the GetLocalInstant methods to micro-optimise them (they will be called for almost every ZonedDateTime/LocalDateTime)
    }
}
