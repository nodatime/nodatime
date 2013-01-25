// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Text;
using NodaTime.TimeZones.IO;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Extends <see cref="ZoneYearOffset"/> with a name and savings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This represents a recurring transition from or to a daylight savings time. The name is the
    /// name of the time zone during this period (e.g. PST or PDT). The savings is usually 0 or the
    /// daylight offset. This is also used to support some of the tricky transitions that occurred
    /// before that calendars were "standardized."
    /// </para>
    /// <para>
    /// Immutable, thread safe.
    /// </para>
    /// </remarks>
    internal sealed class ZoneRecurrence : IEquatable<ZoneRecurrence>
    {
        private readonly int fromYear;
        private readonly string name;
        private readonly Offset savings;
        private readonly int toYear;
        private readonly ZoneYearOffset yearOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneRecurrence"/> class.
        /// </summary>
        /// <param name="name">The name of the time zone period e.g. PST.</param>
        /// <param name="savings">The savings for this period.</param>
        /// <param name="yearOffset">The year offset of when this period starts in a year.</param>
        /// <param name="fromYear">The first year in which this recurrence is valid</param>
        /// <param name="toYear">The last year in which this recurrence is valid</param>
        public ZoneRecurrence(String name, Offset savings, ZoneYearOffset yearOffset, int fromYear, int toYear)
        {
            Preconditions.CheckNotNull(name, "name");
            Preconditions.CheckNotNull(yearOffset, "yearOffset");
            this.name = name;
            this.savings = savings;
            this.yearOffset = yearOffset;
            this.fromYear = fromYear;
            this.toYear = toYear;
        }

        public string Name { get { return name; } }

        public Offset Savings { get { return savings; } }

        public ZoneYearOffset YearOffset { get { return yearOffset; } }

        public int FromYear { get { return fromYear; } }

        public int ToYear { get { return toYear; } }

        public bool IsInfinite { get { return ToYear == Int32.MaxValue; } }

        /// <summary>
        /// Returns a new recurrence which has the same values as this, but a different name.
        /// </summary>
        internal ZoneRecurrence WithName(string newName)
        {
            return new ZoneRecurrence(newName, savings, yearOffset, fromYear, toYear);
        }

        #region IEquatable<ZoneRecurrence> Members
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(ZoneRecurrence other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return savings == other.savings && fromYear == other.fromYear && toYear == other.toYear && name == other.name && yearOffset == other.yearOffset;
        }
        #endregion

        #region Operator overloads
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ZoneRecurrence left, ZoneRecurrence right)
        {
            return ReferenceEquals(null, left) ? ReferenceEquals(null, right) : left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ZoneRecurrence left, ZoneRecurrence right)
        {
            return !(left == right);
        }
        #endregion

        /// <summary>
        /// Returns the given instant adjusted one year forward taking into account leap years and other
        /// adjustments like day of week.
        /// </summary>
        /// <remarks>
        /// If the given instant is before the starting year, the year of the given instant is
        /// adjusted to the beginning of the starting year. The first transition after the
        /// adjusted instant is determined. If the next adjustment is after the ending year, this
        /// method returns null; otherwise the next transition is returned.
        /// </remarks>
        /// <param name="instant">The <see cref="Instant"/> lower bound for the next transition.</param>
        /// <param name="standardOffset">The <see cref="Offset"/> standard offset.</param>
        /// <param name="previousSavings">The <see cref="Offset"/> savings adjustment at the given Instant.</param>
        /// <returns>The next transition, or null if there is no next transition.</returns>
        internal Transition? Next(Instant instant, Offset standardOffset, Offset previousSavings)
        {
            CalendarSystem calendar = CalendarSystem.Iso;

            Offset wallOffset = standardOffset + previousSavings;

            int year = instant == Instant.MinValue ? Int32.MinValue : calendar.Fields.Year.GetValue(instant.Plus(wallOffset));

            if (year < fromYear)
            {
                // First advance instant to start of from year.
                instant = calendar.Fields.Year.SetValue(LocalInstant.LocalUnixEpoch, fromYear).Minus(wallOffset);
                // Back off one tick to account for next recurrence being exactly at the beginning
                // of the year.
                instant = instant - Duration.Epsilon;
            }

            Instant next = yearOffset.Next(instant, standardOffset, previousSavings);

            if (next >= instant)
            {
                year = calendar.Fields.Year.GetValue(next.Plus(wallOffset));
                if (year > toYear)
                {
                    return null;
                }
            }

            return new Transition(next, wallOffset, standardOffset + Savings);
        }

        /// <summary>
        /// Returns the given instant adjusted one year backward taking into account leap years and other
        /// adjustments like day of week.
        /// </summary>
        /// <param name="instant">The <see cref="Instant"/> lower bound for the next trasnition.</param>
        /// <param name="standardOffset">The <see cref="Offset"/> standard offset.</param>
        /// <param name="previousSavings">The <see cref="Offset"/> savings adjustment at the given Instant.</param>
        /// <returns>The previous transition, or null if there is no previous transition.</returns>
        internal Transition? Previous(Instant instant, Offset standardOffset, Offset previousSavings)
        {
            CalendarSystem calendar = CalendarSystem.Iso;

            Offset wallOffset = standardOffset + previousSavings;

            int year = instant == Instant.MaxValue ? Int32.MaxValue : calendar.Fields.Year.GetValue(instant.Plus(wallOffset));

            if (year > toYear)
            {
                // First pull instant back to the start of the year after toYear
                instant = calendar.Fields.Year.SetValue(LocalInstant.LocalUnixEpoch, toYear + 1).Minus(wallOffset);
            }

            Instant previous = yearOffset.Previous(instant, standardOffset, previousSavings);

            if (previous <= instant)
            {
                year = calendar.Fields.Year.GetValue(previous.Plus(wallOffset));
                if (year < fromYear)
                {
                    return null;
                }
            }

            return new Transition(previous, wallOffset, standardOffset + Savings);
        }

        /// <summary>
        /// Piggy-backs onto Next, but fails with an InvalidOperationException if there's no such transition.
        /// </summary>
        internal Transition NextOrFail(Instant instant, Offset standardOffset, Offset previousSavings)
        {
            Transition? next = Next(instant, standardOffset, previousSavings);
            if (next == null)
            {
                throw new InvalidOperationException(
                    string.Format("Noda Time bug or bad data: Expected a transition later than {0}; standard offset = {1}; previousSavings = {2}; recurrence = {3}",
                        instant, standardOffset, previousSavings, this));
            }
            return next.Value;
        }

        /// <summary>
        /// Piggy-backs onto Previous, but fails with a descriptive InvalidOperationException if there's no such transition.
        /// </summary>
        internal Transition PreviousOrFail(Instant instant, Offset standardOffset, Offset previousSavings)
        {
            Transition? previous = Previous(instant, standardOffset, previousSavings);
            if (previous == null)
            {
                throw new InvalidOperationException(
                    string.Format("Noda Time bug or bad data: Expected a transition earlier than {0}; standard offset = {1}; previousSavings = {2}; recurrence = {3}",
                        instant, standardOffset, previousSavings, this));
            }
            return previous.Value;
        }

        /// <summary>
        /// Writes this object to the given <see cref="DateTimeZoneWriter"/>.
        /// </summary>
        /// <param name="writer">Where to send the output.</param>
        internal void Write(IDateTimeZoneWriter writer)
        {
            writer.WriteString(Name);
            writer.WriteOffset(Savings);
            YearOffset.Write(writer);
            // We'll never have time zones with recurrences between the beginning of time and 0AD,
            // so we can treat anything negative as 0, and go to the beginning of time when reading.
            writer.WriteCount(Math.Max(fromYear, 0));
            writer.WriteCount(toYear);
        }

        internal void WriteLegacy(LegacyDateTimeZoneWriter writer)
        {
            writer.WriteString(Name);
            writer.WriteOffset(Savings);
            YearOffset.WriteLegacy(writer);
            // We'll never have time zones with recurrences between the beginning of time and 0AD,
            // so we can treat anything negative as 0, and go to the beginning of time when reading.
            writer.WriteCount(Math.Max(fromYear, 0));
            writer.WriteCount(toYear);
        }

        /// <summary>
        /// Reads a recurrence from the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The recurrence read from the reader.</returns>
        public static ZoneRecurrence Read(IDateTimeZoneReader reader)
        {
            Preconditions.CheckNotNull(reader, "reader");
            string name = reader.ReadString();
            Offset savings = reader.ReadOffset();
            ZoneYearOffset yearOffset = ZoneYearOffset.Read(reader);
            int fromYear = reader.ReadCount();
            if (fromYear == 0)
            {
                fromYear = int.MinValue;
            }
            int toYear = reader.ReadCount();
            return new ZoneRecurrence(name, savings, yearOffset, fromYear, toYear);
        }

        internal static ZoneRecurrence ReadLegacy(LegacyDateTimeZoneReader reader)
        {
            Preconditions.CheckNotNull(reader, "reader");
            string name = reader.ReadString();
            Offset savings = reader.ReadOffset();
            ZoneYearOffset yearOffset = ZoneYearOffset.ReadLegacy(reader);
            int fromYear = reader.ReadCount();
            if (fromYear == 0)
            {
                fromYear = int.MinValue;
            }
            int toYear = reader.ReadCount();
            return new ZoneRecurrence(name, savings, yearOffset, fromYear, toYear);
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
        public override bool Equals(object obj)
        {
            return Equals(obj as ZoneRecurrence);
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
            hash = HashCodeHelper.Hash(hash, savings);
            hash = HashCodeHelper.Hash(hash, name);
            hash = HashCodeHelper.Hash(hash, yearOffset);
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
            builder.Append(Name);
            builder.Append(" ").Append(Savings);
            builder.Append(" ").Append(YearOffset);
            builder.Append(" [").Append(FromYear).Append("-").Append(ToYear).Append("]");
            return builder.ToString();
        }
        #endregion // Object overrides

        /// <summary>
        /// Returns either "this" (if this zone recurrence already has a from year of int.MinValue)
        /// or a new zone recurrence which is identical but with a from year of int.MinValue.
        /// </summary>
        internal ZoneRecurrence ToStartOfTime()
        {
            return fromYear == int.MinValue ? this : new ZoneRecurrence(name, savings, yearOffset, int.MinValue, toYear);
        }

        /// <summary>
        /// Returns either "this" (if this zone recurrence is already infinite)
        /// or a new zone recurrence which is identical but with a from year of int.MinValue
        /// and an end year of int.MaxValue.
        /// </summary>
        internal ZoneRecurrence ToInfinity()
        {
            return this.IsInfinite ? this : new ZoneRecurrence(name, savings, yearOffset, int.MinValue, int.MaxValue);
        }
    }
}
