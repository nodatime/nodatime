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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using NodaTime.Fields;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Defines an offset within a year as an expression that can be used to reference multiple
    /// years.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A year offset defines a way of determining an offset into a year based on certain criteria.
    /// The most basic is the month of the year and the day of the month. If only these two are
    /// supplied then the offset is always the sae day of each year. The only exception is if the
    /// day is February 29th, then it only refers to those years that have a February 29th.
    /// </para>
    /// <para>
    /// If the day of the week is specified then the offset determined byt the month and day are
    /// adjusted to the nearest day that falls on the given day of the week. If then month and day
    /// fall on that day of the week then nothing changes. Otherwise the offset is moved forward or
    /// backward up to 6 days to make the day fall on the correct day of the week. The direction the
    /// offset is moved is determined by the <see cref="AdvanceDayOfWeek"/> property.
    /// </para>
    /// <para>
    /// Finally the <see cref="Mode"/> property deterines whether the <see cref="TickOfDay"/> value
    /// is added to the calculated offset to generate an offset within the day.
    /// </para>
    /// <para>
    /// Immutable, thread safe
    /// </para>
    /// </remarks>
    internal class ZoneYearOffset : IEquatable<ZoneYearOffset>
    {
        /// <summary>
        /// An offset that specifies the beginning of the year.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "ZoneYearOffset is immutable")] public static readonly ZoneYearOffset StartOfYear = new ZoneYearOffset(TransitionMode.Wall, 1, 1, 0, true, Offset.Zero);

        // TODO: find a better home for these two arrays

        /// <summary>
        /// The months of the year names as they appear in the TZDB zone files. They are
        /// always the short name in US English. Extra blank name at the beginning helps
        /// to make the indexes to come out right.
        /// </summary>
        private static readonly string[] Months = { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        /// <summary>
        /// The days of the week names as they appear in the TZDB zone files. They are
        /// always the short name in US English.
        /// </summary>
        private static readonly string[] DaysOfWeek = { "", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

        private readonly bool advance;
        private readonly int dayOfMonth;
        private readonly int dayOfWeek;
        private readonly TransitionMode mode;
        private readonly int monthOfYear;
        private readonly Offset tickOfDay;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneYearOffset"/> class.
        /// </summary>
        /// <param name="mode">The transition mode.</param>
        /// <param name="monthOfYear">The month year offset.</param>
        /// <param name="dayOfMonth">The day of month. 0 means not set. Negatives count from end of month.</param>
        /// <param name="dayOfWeek">The day of week. 0 menas not set.</param>
        /// <param name="advance">if set to <c>true</c> [advance].</param>
        /// <param name="tickOfDay">The tick within the day.</param>
        public ZoneYearOffset(TransitionMode mode, int monthOfYear, int dayOfMonth, int dayOfWeek, bool advance, Offset tickOfDay)
        {
            FieldUtils.VerifyFieldValue(CalendarSystem.Iso.Fields.MonthOfYear, "monthOfYear", monthOfYear);
            FieldUtils.VerifyFieldValue(CalendarSystem.Iso.Fields.DayOfMonth, "dayOfMonth", dayOfMonth, true);
            if (dayOfWeek != 0)
            {
                FieldUtils.VerifyFieldValue(CalendarSystem.Iso.Fields.DayOfWeek, "dayOfWeek", dayOfWeek);
            }
            FieldUtils.VerifyFieldValue(CalendarSystem.Iso.Fields.TickOfDay, "tickOfDay", tickOfDay.Ticks);

            this.mode = mode;
            this.monthOfYear = monthOfYear;
            this.dayOfMonth = dayOfMonth;
            this.dayOfWeek = dayOfWeek;
            this.advance = advance;
            this.tickOfDay = tickOfDay;
        }

        /// <summary>
        /// Gets the method by which offsets are added to Instants to get LocalInstants.
        /// </summary>
        public TransitionMode Mode { get { return mode; } }

        /// <summary>
        /// Gets the month of year the rule starts.
        /// </summary>
        public int MonthOfYear { get { return monthOfYear; } }

        /// <summary>
        /// Gets the day of month this rule starts.
        /// </summary>
        public int DayOfMonth { get { return dayOfMonth; } }

        /// <summary>
        /// Gets the day of week this rule starts.
        /// </summary>
        /// <value>The integer day of week (1=Mon, 2=Tue, etc.). 0 means not set.</value>
        public int DayOfWeek { get { return dayOfWeek; } }

        /// <summary>
        /// Gets a value indicating whether [advance day of week].
        /// </summary>
        public bool AdvanceDayOfWeek { get { return advance; } }

        /// <summary>
        /// Gets the tick of day when the rule takes effect.
        /// </summary>
        public Offset TickOfDay { get { return tickOfDay; } }

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
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return mode == other.mode && monthOfYear == other.monthOfYear && dayOfMonth == other.dayOfMonth && dayOfWeek == other.dayOfWeek &&
                   advance == other.advance && tickOfDay == other.tickOfDay;
        }
        #endregion

        /// <summary>
        /// Normalizes the transition mode characater.
        /// </summary>
        /// <param name="modeCharacter">The character to normalize.</param>
        /// <returns>The <see cref="TransitionMode"/>.</returns>
        public static TransitionMode NormalizeModeCharacter(char modeCharacter)
        {
            switch (modeCharacter)
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
            CalendarSystem calendar = CalendarSystem.Iso;
            LocalInstant instant = calendar.Fields.Year.SetValue(LocalInstant.LocalUnixEpoch, year);
            instant = calendar.Fields.MonthOfYear.SetValue(instant, monthOfYear);
            instant = calendar.Fields.TickOfDay.SetValue(instant, tickOfDay.Ticks);
            instant = SetDayOfMonth(calendar, instant);
            instant = SetDayOfWeek(calendar, instant);

            Offset offset = GetOffset(standardOffset, savings);
            // Convert from local time to UTC.
            return instant.Minus(offset);
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
        /// Writes this object to the given <see cref="DateTimeZoneCompressionWriter"/>.
        /// </summary>
        /// <param name="writer">Where to send the output.</param>
        internal void Write(DateTimeZoneWriter writer)
        {
            writer.WriteEnum((int)Mode);
            writer.WriteInteger(MonthOfYear);
            // Day or month can range from -(max value) to max value so if we add max value it will
            // force it into the positive range
            writer.WriteInteger(DayOfMonth);
            writer.WriteInteger(DayOfWeek);
            writer.WriteBoolean(AdvanceDayOfWeek);
            writer.WriteOffset(TickOfDay);
        }

        public static ZoneYearOffset Read(DateTimeZoneReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            var mode = (TransitionMode)reader.ReadEnum();
            int monthOfYear = reader.ReadInteger();
            // Day or month can range from -(max value) to max value so we added max value so it will
            // force it into the positive range
            int dayOfMonth = reader.ReadInteger();
            int dayOfWeek = reader.ReadInteger();
            bool advance = reader.ReadBoolean();
            var ticksOfDay = reader.ReadOffset();
            return new ZoneYearOffset(mode, monthOfYear, dayOfMonth, dayOfWeek, advance, ticksOfDay);
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
                LocalInstant localInstant = instant.Plus(offset);

                CalendarSystem calendar = CalendarSystem.Iso;
                LocalInstant newInstant = calendar.Fields.MonthOfYear.SetValue(localInstant, monthOfYear);
                // Be lenient with tick of day.
                newInstant = calendar.Fields.TickOfDay.SetValue(newInstant, tickOfDay.Ticks);
                newInstant = SetDayOfMonthWithLeap(calendar, newInstant, direction);

                int signDirection = Math.Sign(direction);
                if (dayOfWeek == 0)
                {
                    int signDifference = Math.Sign((localInstant - newInstant).Ticks);
                    if (signDifference == 0 || signDirection == signDifference)
                    {
                        newInstant = calendar.Fields.Year.Add(newInstant, direction);
                        newInstant = SetDayOfMonthWithLeap(calendar, newInstant, direction);
                    }
                }
                else
                {
                    newInstant = SetDayOfWeek(calendar, newInstant);
                    int signDifference = Math.Sign((localInstant - newInstant).Ticks);
                    if (signDifference == 0 || signDirection == signDifference)
                    {
                        newInstant = calendar.Fields.Year.Add(newInstant, direction);
                        newInstant = calendar.Fields.MonthOfYear.SetValue(newInstant, monthOfYear);
                        newInstant = SetDayOfMonthWithLeap(calendar, newInstant, direction);
                        newInstant = SetDayOfWeek(calendar, newInstant);
                    }
                }
                // Convert from local time to UTC.
                return newInstant.Minus(offset);
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
        /// <param name="direction"></param>
        /// <returns>The adjusted <see cref="LocalInstant"/>.</returns>
        private LocalInstant SetDayOfMonthWithLeap(CalendarSystem calendar, LocalInstant instant, int direction)
        {
            if (monthOfYear == 2 && dayOfMonth == 29)
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
        private LocalInstant SetDayOfMonth(CalendarSystem calendar, LocalInstant instant)
        {
            if (dayOfMonth > 0)
            {
                instant = calendar.Fields.DayOfMonth.SetValue(instant, dayOfMonth);
            }
            else if (dayOfMonth < 0)
            {
                instant = calendar.Fields.DayOfMonth.SetValue(instant, 1);
                instant = calendar.Fields.MonthOfYear.Add(instant, 1);
                instant = calendar.Fields.DayOfMonth.Add(instant, dayOfMonth);
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
        private LocalInstant SetDayOfWeek(CalendarSystem calendar, LocalInstant instant)
        {
            if (dayOfWeek != 0)
            {
                int dayOfWeekOfInstant = calendar.Fields.DayOfWeek.GetValue(instant);
                int daysToAdd = dayOfWeek - dayOfWeekOfInstant;
                if (daysToAdd != 0)
                {
                    if (advance)
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
            switch (mode)
            {
                case TransitionMode.Wall:
                    offset = standardOffset + savings;
                    break;
                case TransitionMode.Standard:
                    offset = standardOffset;
                    break;
                default:
                    offset = Offset.Zero;
                    break;
            }
            return offset;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ZoneYearOffset left, ZoneYearOffset right)
        {
            return ReferenceEquals(null, left) ? ReferenceEquals(null, right) : left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ZoneYearOffset left, ZoneYearOffset right)
        {
            return !(left == right);
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
            return Equals(obj as ZoneYearOffset);
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
            hash = HashCodeHelper.Hash(hash, mode);
            hash = HashCodeHelper.Hash(hash, monthOfYear);
            hash = HashCodeHelper.Hash(hash, dayOfMonth);
            hash = HashCodeHelper.Hash(hash, dayOfWeek);
            hash = HashCodeHelper.Hash(hash, advance);
            hash = HashCodeHelper.Hash(hash, tickOfDay);
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
            var builder = new StringBuilder();
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
                builder.Append(AdvanceDayOfWeek ? ">=" : "<=");
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
    }
}