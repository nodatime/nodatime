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
using NodaTime.Calendars;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Defines an offset within a year. Can be applied to multiple years.
    /// </summary>
    /// <remarks>
    /// Immutable, thread safe
    /// </remarks>
    internal class ZoneYearOffset
        : IEquatable<ZoneYearOffset>
    {
        private readonly TransitionMode mode;
        private readonly int monthOfYear;
        private readonly int dayOfMonth;
        private readonly int dayOfWeek;
        private readonly bool advance;
        private readonly long tickOfDay;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneYearOffset"/> class.
        /// </summary>
        /// <param name="mode">The transition mode.</param>
        /// <param name="monthOfYear">The month year offset.</param>
        /// <param name="dayOfMonth">The day of month. 0 means not set. Negatives count from end of month.</param>
        /// <param name="dayOfWeek">The day of week. 0 menas not set.</param>
        /// <param name="advance">if set to <c>true</c> [advance].</param>
        /// <param name="tickOfDay">The tick within the day.</param>
        internal ZoneYearOffset(TransitionMode mode,
                                int monthOfYear,
                                int dayOfMonth,
                                int dayOfWeek,
                                bool advance,
                                Duration tickOfDay)
        {
            this.mode = mode;
            this.monthOfYear = monthOfYear;
            this.dayOfMonth = dayOfMonth;
            this.dayOfWeek = dayOfWeek;
            this.advance = advance;
            // TODO: Consider making tickOfDay a long instead
            this.tickOfDay = tickOfDay.Ticks;
        }

        /// <summary>
        /// Returns an <see cref="Instant"/> that represents the point in the given year that this
        /// object defines. If the exact point is not valid then the nearest point that matches the
        /// definition is returned.
        /// </summary>
        /// <param name="year">The year to calculate for.</param>
        /// <param name="standardOffset">The standard offset.</param>
        /// <param name="savings">The daylight savings adjustment.</param>
        /// <returns>The <see cref="Instant"/> of the point in the given year.</returns>
        internal Instant MakeInstant(int year, Offset standardOffset, Offset savings)
        {
            ICalendarSystem calendar = IsoCalendarSystem.Instance;
            LocalInstant instant = calendar.Fields.Year.SetValue(LocalInstant.LocalUnixEpoch, year);
            instant = calendar.Fields.MonthOfYear.SetValue(instant, this.monthOfYear);
            instant = calendar.Fields.TickOfDay.SetValue(instant, this.tickOfDay);
            instant = SetDayOfMonth(calendar, instant);
            instant = SetDayOfWeek(calendar, instant);

            Offset offset = GetOffset(standardOffset, savings);
            // Convert from local time to UTC.
            return instant - offset;
        }

        /// <summary>
        /// Returns the given instant adjusted one year forward taking into account leap years and other
        /// adjustments like day of week.
        /// </summary>
        /// <param name="instant">The instant to adjust.</param>
        /// <param name="standardOffset">The standard offset.</param>
        /// <param name="savings">The daylight savings adjustment.</param>
        /// <returns>The adjusted <see cref="LocalInstant"/>.</returns>
        internal Instant Next(Instant instant, Offset standardOffset, Offset savings)
        {
            return AdjustInstant(instant, standardOffset, savings, 1);
        }

        /// <summary>
        /// Returns the given instant adjusted one year backward taking into account leap years and other
        /// adjustments like day of week.
        /// </summary>
        /// <param name="instant">The instant to adjust.</param>
        /// <param name="standardOffset">The standard offset.</param>
        /// <param name="savings">The daylight savings adjustment.</param>
        /// <returns>The adjusted <see cref="LocalInstant"/>.</returns>
        internal Instant Previous(Instant instant, Offset standardOffset, Offset savings)
        {
            return AdjustInstant(instant, standardOffset, savings, -1);
        }

        /// <summary>
        /// Adjusts the instant one year in the given direction.
        /// </summary>
        /// <remarks>
        /// If there is an overflow/underflow in any operation performed in this method then <see
        /// cref="Instant.MinValue"/> or <see cref="Instant.MaxValue"/> will be returned depending
        /// on <paramref name="direction"/>.
        /// </remarks>
        /// <param name="instant">The instant to adjust.</param>
        /// <param name="standardOffset">The standard offset.</param>
        /// <param name="savings">The daylight savings adjustment.</param>
        /// <param name="direction">The direction to adjust. 1 for forward, -1 for backward.</param>
        /// <returns>The adjusted <see cref="Instant"/>.</returns>
        private Instant AdjustInstant(Instant instant, Offset standardOffset, Offset savings, int direction)
        {
            try
            {
                Offset offset = GetOffset(standardOffset, savings);

                // Convert from UTC to local time.
                LocalInstant localInstant = instant + offset;

                IsoCalendarSystem calendar = IsoCalendarSystem.Instance;
                LocalInstant newInstant = calendar.Fields.MonthOfYear.SetValue(localInstant, this.monthOfYear);
                // Be lenient with millisOfDay.
                newInstant = calendar.Fields.TickOfDay.SetValue(newInstant, this.tickOfDay);
                newInstant = SetDayOfMonthWithLeap(calendar, newInstant, direction);

                if (this.dayOfWeek == 0)
                {
                    if (newInstant >= localInstant)
                    {
                        newInstant = calendar.Fields.Year.Add(newInstant, direction);
                        newInstant = SetDayOfMonthWithLeap(calendar, newInstant, direction);
                    }
                }
                else
                {
                    newInstant = SetDayOfWeek(calendar, newInstant);
                    if (newInstant >= localInstant)
                    {
                        newInstant = calendar.Fields.Year.Add(newInstant, direction);
                        newInstant = calendar.Fields.MonthOfYear.SetValue(newInstant, this.monthOfYear);
                        newInstant = SetDayOfMonthWithLeap(calendar, newInstant, direction);
                        newInstant = SetDayOfWeek(calendar, newInstant);
                    }
                }
                // Convert from local time to UTC.
                return newInstant - offset;
            }
            catch (OverflowException)
            {
                return direction < 0 ? Instant.MinValue : Instant.MaxValue;
            }
        }

        /// <summary>
        /// Sets the day of month handling leap years.
        /// </summary>
        /// <remarks>
        /// If the day of the month is February 29 then the starting year is a leap year and we have
        /// to go forward or back to the next or previous leap year or February 29 will be an
        /// invalid date.
        /// </remarks>
        /// <param name="calendar">The calendar to use to set the values.</param>
        /// <param name="instant">The instant to adjust.</param>
        /// <returns>The adjusted <see cref="LocalInstant"/>.</returns>
        private LocalInstant SetDayOfMonthWithLeap(ICalendarSystem calendar, LocalInstant instant, int direction)
        {
            if (this.monthOfYear == 2 && this.dayOfMonth == 29)
            {
                while (calendar.Fields.Year.IsLeap(instant) == false)
                {
                    instant = calendar.Fields.Year.Add(instant, direction);
                }
            }
            instant = SetDayOfMonth(calendar, instant);
            return instant;
        }

        /// <summary>
        /// Sets the day of month of the given instant. If the day of the month is negative then sets the
        /// day from the end of the month.
        /// </summary>
        /// <param name="calendar">The calendar to use to set the values.</param>
        /// <param name="instant">The instant to adjust.</param>
        /// <returns>The adjusted <see cref="LocalInstant"/>.</returns>
        private LocalInstant SetDayOfMonth(ICalendarSystem calendar, LocalInstant instant)
        {
            if (this.dayOfMonth > 0)
            {
                instant = calendar.Fields.DayOfMonth.SetValue(instant, this.dayOfMonth);
            }
            else if (this.dayOfMonth < 0)
            {
                instant = calendar.Fields.DayOfMonth.SetValue(instant, 1);
                instant = calendar.Fields.MonthOfYear.Add(instant, 1);
                instant = calendar.Fields.DayOfMonth.Add(instant, this.dayOfMonth);
            }
            return instant;
        }

        /// <summary>
        /// Sets the day of week of the given instant.
        /// </summary>
        /// <remarks>
        /// This will move the current day of the week either forward or backward by up to one week.
        /// If the day of the week is already correct then nothing changes.
        /// </remarks>
        /// <param name="calendar">The calendar to use to set the values.</param>
        /// <param name="instant">The instant to adjust.</param>
        /// <returns>The adjusted <see cref="LocalInstant"/>.</returns>
        private LocalInstant SetDayOfWeek(ICalendarSystem calendar, LocalInstant instant)
        {
            if (this.dayOfWeek != 0)
            {
                int dayOfWeek = calendar.Fields.DayOfWeek.GetValue(instant);
                int daysToAdd = this.dayOfWeek - dayOfWeek;
                if (daysToAdd != 0)
                {
                    if (this.advance)
                    {
                        if (daysToAdd < 0)
                        {
                            daysToAdd += 7;
                        }
                    }
                    else
                    {
                        if (daysToAdd > 0)
                        {
                            daysToAdd -= 7;
                        }
                    }
                    instant = calendar.Fields.DayOfWeek.Add(instant, daysToAdd);
                }
            }
            return instant;
        }

        /// <summary>
        /// Returns the offset to use for this object's <see cref="TransitionMode"/>.
        /// </summary>
        /// <param name="standardOffset">The standard offset.</param>
        /// <param name="savings">The daylight savings adjustment.</param>
        /// <returns>The base time offset as a <see cref="Duration"/>.</returns>
        private Offset GetOffset(Offset standardOffset, Offset savings)
        {
            Offset offset;
            if (this.mode == TransitionMode.Wall)
            {
                offset = standardOffset + savings;
            }
            else if (this.mode == TransitionMode.Standard)
            {
                offset = standardOffset;
            }
            else
            {
                offset = Offset.Zero;
            }
            return offset;
        }

        #region Object overrides

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (obj is ZoneYearOffset)
            {
                return Equals((ZoneYearOffset)obj);
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, this.mode);
            hash = HashCodeHelper.Hash(hash, this.monthOfYear);
            hash = HashCodeHelper.Hash(hash, this.dayOfMonth);
            hash = HashCodeHelper.Hash(hash, this.dayOfWeek);
            hash = HashCodeHelper.Hash(hash, this.advance);
            hash = HashCodeHelper.Hash(hash, this.tickOfDay);
            return hash;
        }

        #endregion // Object overrides

        #region IEquatable<ZoneYearOffset> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ZoneYearOffset other)
        {
            if (other == null)
            {
                return false;
            }
            return
                this.mode == other.mode &&
                this.monthOfYear == other.monthOfYear &&
                this.dayOfMonth == other.dayOfMonth &&
                this.dayOfWeek == other.dayOfWeek &&
                this.advance == other.advance &&
                this.tickOfDay == other.tickOfDay;
        }

        #endregion
    }
}
