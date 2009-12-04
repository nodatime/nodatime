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
using System.Text;
using NodaTime.Fields;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Defines an offset within a year. Can be applied to multiple years.
    /// </summary>
    /// <remarks>
    /// Immutable, thread safe
    /// </remarks>
    public class ZoneYearOffset
        : IEquatable<ZoneYearOffset>
    {
        /// <summary>
        /// An offset that specifies the beginning of the year.
        /// </summary>
        public static readonly ZoneYearOffset StartOfYear = new ZoneYearOffset(TransitionMode.Wall, 1, 1, 0, true, Offset.Zero);

        private readonly TransitionMode mode;
        private readonly int monthOfYear;
        private readonly int dayOfMonth;
        private readonly int dayOfWeek;
        private readonly bool advance;
        private readonly Offset tickOfDay;

        // TODO: find a better home for these two arrays

        /// <summary>
        /// The months of the year names as they appear in the TZDB zone files. They are
        /// always the short name in US English. Extra blank name at the beginning helps
        /// to make the indexes to come out right.
        /// </summary>
        private static readonly string[] Months = { 
                                             "",
                                             "Jan",
                                             "Feb",
                                             "Mar",
                                             "Apr",
                                             "May",
                                             "Jun",
                                             "Jul",
                                             "Aug",
                                             "Sep",
                                             "Oct",
                                             "Nov",
                                             "Dec"
                                         };

        /// <summary>
        /// The days of the week names as they appear in the TZDB zone files. They are
        /// always the short name in US English.
        /// </summary>
        private static readonly string[] DaysOfWeek = {
                                                         "",
                                                         "Mon",
                                                         "Tue",
                                                         "Wed",
                                                         "Thu",
                                                         "Fri",
                                                         "Sat",
                                                         "Sun"
                                                     };
        /// <summary>
        /// Gets the method by which offsets are added to Instants to get LocalInstants.
        /// </summary>
        public TransitionMode Mode { get { return this.mode; } }

        /// <summary>
        /// Gets the month of year the rule starts.
        /// </summary>
        public int MonthOfYear { get { return this.monthOfYear; } }

        /// <summary>
        /// Gets the day of month this rule starts.
        /// </summary>
        public int DayOfMonth { get { return this.dayOfMonth; } }

        /// <summary>
        /// Gets the day of week this rule starts.
        /// </summary>
        /// <value>The integer day of week (1=Mon, 2=Tue, etc.). 0 means not set.</value>
        public int DayOfWeek { get { return this.dayOfWeek; } }

        /// <summary>
        /// Gets a value indicating whether [advance day of week].
        /// </summary>
        public bool AdvanceDayOfWeek { get { return this.advance; } }

        /// <summary>
        /// Gets the tick of day when the rule takes effect.
        /// </summary>
        public Offset TickOfDay { get { return this.tickOfDay; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneYearOffset"/> class.
        /// </summary>
        /// <param name="mode">The transition mode.</param>
        /// <param name="monthOfYear">The month year offset.</param>
        /// <param name="dayOfMonth">The day of month. 0 means not set. Negatives count from end of month.</param>
        /// <param name="dayOfWeek">The day of week. 0 menas not set.</param>
        /// <param name="advance">if set to <c>true</c> [advance].</param>
        /// <param name="tickOfDay">The tick within the day.</param>
        public ZoneYearOffset(TransitionMode mode,
                                int monthOfYear,
                                int dayOfMonth,
                                int dayOfWeek,
                                bool advance,
                                Offset tickOfDay)
        {
            FieldUtils.VerifyFieldValue(IsoCalendarSystem.Instance.Fields.MonthOfYear, "monthOfYear", monthOfYear);
            FieldUtils.VerifyFieldValue(IsoCalendarSystem.Instance.Fields.DayOfMonth, "dayOfMonth", dayOfMonth, true);
            if (dayOfWeek != 0)
            {
                FieldUtils.VerifyFieldValue(IsoCalendarSystem.Instance.Fields.DayOfWeek, "dayOfWeek", dayOfWeek);
            }
            FieldUtils.VerifyFieldValue(IsoCalendarSystem.Instance.Fields.TickOfDay, "tickOfDay", tickOfDay.Ticks);

            this.mode = mode;
            this.monthOfYear = monthOfYear;
            this.dayOfMonth = dayOfMonth;
            this.dayOfWeek = dayOfWeek;
            this.advance = advance;
            this.tickOfDay = tickOfDay;
        }

        /// <summary>
        /// Normalizes the transition mode characater.
        /// </summary>
        /// <param name="c">The character to normalize.</param>
        /// <returns>The <see cref="TransitionMode"/>.</returns>
        public static TransitionMode NormalizeModeCharacter(char c)
        {
            switch (c)
            {
                case 's':
                case 'S':
                    return TransitionMode.Standard;
                case 'u':
                case 'U':
                case 'g':
                case 'G':
                case 'z':
                case 'Z':
                    return TransitionMode.Utc;
                case 'w':
                case 'W':
                default:
                    return TransitionMode.Wall;
            }
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
            instant = calendar.Fields.TickOfDay.SetValue(instant, this.tickOfDay.Ticks);
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
        /// Writes this object to the given <see cref="DateTimeZoneWriter"/>.
        /// </summary>
        /// <param name="writer">Where to send the output.</param>
        internal void Write(DateTimeZoneWriter writer)
        {
            writer.WriteInt8((byte)Mode);
            writer.WriteInt8((byte)MonthOfYear);
            writer.WriteInt8((byte)DayOfMonth);
            writer.WriteInt8((byte)DayOfWeek);
            writer.WriteBoolean(AdvanceDayOfWeek);
            writer.WriteTicks(TickOfDay.Ticks);
        }

        public static ZoneYearOffset Read(DateTimeZoneReader reader)
        {
            TransitionMode mode = (TransitionMode)reader.ReadInt8();
            int monthOfYear = reader.ReadInt8();
            int dayOfMonth = reader.ReadInt8();
            int dayOfWeek = reader.ReadInt8();
            bool advance = reader.ReadBoolean();
            long ticks = reader.ReadTicks();
            return new ZoneYearOffset(mode, monthOfYear, dayOfMonth, dayOfWeek, advance, new Offset(ticks));
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
                newInstant = calendar.Fields.TickOfDay.SetValue(newInstant, this.tickOfDay.Ticks);
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

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Months[MonthOfYear]).Append(" ");
            if (DayOfMonth == -1)
            {
                builder.Append("last").Append(DaysOfWeek[DayOfWeek]).Append(" ");
            }
            else if (DayOfWeek == 0)
            {
                builder.Append(DayOfMonth).Append(" ");
            }
            else
            {
                builder.Append(DaysOfWeek[DayOfWeek]);
                if (AdvanceDayOfWeek)
                {
                    builder.Append(">=");
                }
                else
                {
                    builder.Append("<=");
                }
                builder.Append(DayOfMonth).Append(" ");
            }
            builder.Append(TickOfDay);
            switch (Mode)
            {
                case TransitionMode.Standard:
                    builder.Append("s");
                    break;
                case TransitionMode.Utc:
                    builder.Append("u");
                    break;
                case TransitionMode.Wall:
                    break;
            }
            return builder.ToString();
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
