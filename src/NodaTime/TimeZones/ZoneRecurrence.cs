// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Text;
using NodaTime.Calendars;
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
        private readonly LocalInstant maxLocalInstant;
        private readonly LocalInstant minLocalInstant;
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

            Preconditions.CheckArgument(fromYear == int.MinValue || (fromYear >= -9998 && fromYear <= 9999), "fromYear",
                "fromYear must be in the range [-9998, 9999] or Int32.MinValue");
            Preconditions.CheckArgument(toYear == int.MaxValue || (toYear >= -9998 && toYear <= 9999), "toYear",
                "toYear must be in the range [-9998, 9999] or Int32.MaxValue");
            this.name = name;
            this.savings = savings;
            this.yearOffset = yearOffset;
            this.fromYear = fromYear;
            this.toYear = toYear;
            this.minLocalInstant = fromYear == int.MinValue ? LocalInstant.BeforeMinValue : new LocalDateTime(fromYear, 1, 1, 0, 0).ToLocalInstant();
            this.maxLocalInstant = toYear == int.MaxValue ? LocalInstant.AfterMaxValue : new LocalDateTime(toYear, 12, 31, 0, 0).PlusNanoseconds(NodaConstants.NanosecondsPerDay - 1).ToLocalInstant();
        }

        public string Name { get { return name; } }

        public Offset Savings { get { return savings; } }

        public ZoneYearOffset YearOffset { get { return yearOffset; } }

        public int FromYear { get { return fromYear; } }

        public int ToYear { get { return toYear; } }

        public bool IsInfinite { get { return toYear == Int32.MaxValue; } }

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
            return savings == other.savings && fromYear == other.fromYear && toYear == other.toYear && name == other.name && yearOffset.Equals(other.yearOffset);
        }
        #endregion

        /// <summary>
        /// Returns the first transition which occurs strictly after the given instant.
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
            Offset ruleOffset = yearOffset.GetRuleOffset(standardOffset, previousSavings);
            Offset newOffset = standardOffset + Savings;

            LocalInstant safeLocal = instant.SafePlus(ruleOffset);
            int targetYear;
            if (safeLocal < minLocalInstant)
            {
                // Asked for a transition after some point before our first year: crop to first year.
                targetYear = fromYear;
            }
            else if (safeLocal > maxLocalInstant)
            {
                // Asked for a transition after our end point: there isn't one.
                return null;
            }
            else if (!safeLocal.IsValid)
            {
                if (safeLocal == LocalInstant.BeforeMinValue)
                {
                    // We've been asked to find the next transition after some point which is a valid instant, but is before the
                    // start of valid local time after applying the rule offset. For example, passing Instant.MinValue for a rule which says
                    // "transition uses wall time, which is UTC-5". Proceed as if we'd been asked for something in -9998.
                    // I *think* that works...
                    targetYear = GregorianYearMonthDayCalculator.MinGregorianYear;
                }
                else
                {
                    // We've been asked to find the next transition after some point which is a valid instant, but is after the
                    // end of valid local time after applying the rule offset. It's possible that the next transition *would*
                    // be representable as an instant (e.g. 1am Jan 1st 10000 with an offset of +5) but it's reasonable to
                    // just return an infinite transition.
                    return new Transition(Instant.AfterMaxValue, newOffset);
                }
            }
            else
            {
                // Simple case: we were asked for a "normal" value in the range of years for which this recurrence is valid.
                int ignoredDayOfYear;
                targetYear = CalendarSystem.Iso.YearMonthDayCalculator.GetYear(safeLocal.DaysSinceEpoch, out ignoredDayOfYear);
            }

            LocalInstant transition = yearOffset.GetOccurrenceForYear(targetYear).ToLocalInstant();

            Instant safeTransition = transition.SafeMinus(ruleOffset);
            if (safeTransition > instant)
            {
                return new Transition(safeTransition, newOffset);
            }

            // We've got a transition earlier than we were asked for. Try next year.
            targetYear++;
            if (targetYear > toYear)
            {
                return null;
            }
            // Handle infinite transitions
            if (targetYear > GregorianYearMonthDayCalculator.MaxGregorianYear)
            {
                return new Transition(Instant.AfterMaxValue, newOffset);
            }
            // It's fine for this to be "end of time", and it can't be "start of time" because we're at least finding a transition in -9997.
            safeTransition = yearOffset.GetOccurrenceForYear(targetYear).ToLocalInstant().SafeMinus(ruleOffset);
            return new Transition(safeTransition, newOffset);
        }

        /// <summary>
        /// Returns the last transition which occurs before or on the given instant.
        /// </summary>
        /// <param name="instant">The <see cref="Instant"/> lower bound for the next trasnition.</param>
        /// <param name="standardOffset">The <see cref="Offset"/> standard offset.</param>
        /// <param name="previousSavings">The <see cref="Offset"/> savings adjustment at the given Instant.</param>
        /// <returns>The previous transition, or null if there is no previous transition.</returns>
        internal Transition? PreviousOrSame(Instant instant, Offset standardOffset, Offset previousSavings)
        {
            Offset ruleOffset = yearOffset.GetRuleOffset(standardOffset, previousSavings);
            Offset newOffset = standardOffset + Savings;

            LocalInstant safeLocal = instant.SafePlus(ruleOffset);
            int targetYear;
            if (safeLocal > maxLocalInstant)
            {
                // Asked for a transition before some point after our last year: crop to last year.
                targetYear = toYear;
            }
            else if (safeLocal < minLocalInstant)
            {
                // Asked for a transition before our first year: there isn't one.
                return null;
            }
            else if (!safeLocal.IsValid)
            {
                if (safeLocal == LocalInstant.BeforeMinValue)
                {
                    // We've been asked to find the next transition before some point which is a valid instant, but is before the
                    // start of valid local time after applying the rule offset.  It's possible that the next transition *would*
                    // be representable as an instant (e.g. 1pm Dec 31st -9999 with an offset of -5) but it's reasonable to
                    // just return an infinite transition.
                    return new Transition(Instant.BeforeMinValue, newOffset);
                }
                else
                {
                    // We've been asked to find the next transition before some point which is a valid instant, but is after the
                    // end of valid local time after applying the rule offset. For example, passing Instant.MaxValue for a rule which says
                    // "transition uses wall time, which is UTC+5". Proceed as if we'd been asked for something in 9999.
                    // I *think* that works...
                    targetYear = GregorianYearMonthDayCalculator.MaxGregorianYear;
                }
            }
            else
            {
                // Simple case: we were asked for a "normal" value in the range of years for which this recurrence is valid.
                int ignoredDayOfYear;
                targetYear = CalendarSystem.Iso.YearMonthDayCalculator.GetYear(safeLocal.DaysSinceEpoch, out ignoredDayOfYear);
            }

            LocalInstant transition = yearOffset.GetOccurrenceForYear(targetYear).ToLocalInstant();

            Instant safeTransition = transition.SafeMinus(ruleOffset);
            if (safeTransition <= instant)
            {
                return new Transition(safeTransition, newOffset);
            }

            // We've got a transition later than we were asked for. Try next year.
            targetYear--;
            if (targetYear < fromYear)
            {
                return null;
            }
            // Handle infinite transitions
            if (targetYear < GregorianYearMonthDayCalculator.MinGregorianYear)
            {
                return new Transition(Instant.BeforeMinValue, newOffset);
            }
            // It's fine for this to be "start of time", and it can't be "end of time" because we're at latest finding a transition in 9998.
            safeTransition = yearOffset.GetOccurrenceForYear(targetYear).ToLocalInstant().SafeMinus(ruleOffset);
            return new Transition(safeTransition, newOffset);
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
        /// Piggy-backs onto PreviousOrSame, but fails with a descriptive InvalidOperationException if there's no such transition.
        /// </summary>
        internal Transition PreviousOrSameOrFail(Instant instant, Offset standardOffset, Offset previousSavings)
        {
            Transition? previous = PreviousOrSame(instant, standardOffset, previousSavings);
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
            builder.Append(" [").Append(fromYear).Append("-").Append(toYear).Append("]");
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
            return IsInfinite ? this : new ZoneRecurrence(name, savings, yearOffset, int.MinValue, int.MaxValue);
        }
    }
}
