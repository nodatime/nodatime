// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.TimeZones.IO;
using NodaTime.Utility;
using JetBrains.Annotations;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides a basic daylight savings time zone. A DST time zone has a simple recurrence
    /// where an extra offset is applied between two dates of a year.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: This class *accepts* recurrences which start from a particular year
    /// rather than being infinite back to the start of time, but *treats* them as if
    /// they were infinite. This makes various calculations easier, but this zone should
    /// only be used as part of a PrecalculatedDateTimeZone which will only ask it for
    /// values within the right portion of the timeline.
    /// </remarks>
    internal sealed class DaylightSavingsDateTimeZone : DateTimeZone
    {
        private readonly Offset standardOffset;
        private readonly ZoneRecurrence standardRecurrence;
        private readonly ZoneRecurrence dstRecurrence;

        /// <summary>
        /// Initializes a new instance of the <see cref="DaylightSavingsDateTimeZone"/> class.
        /// </summary>
        /// <remarks>
        /// At least one of the recurrences (it doesn't matter which) must be a "standard", i.e. not have any savings
        /// applied. The other may still not have any savings (e.g. for America/Resolute) or (for BCL compatibility) may
        /// even have negative daylight savings.
        /// </remarks>
        /// <param name="id">The id.</param>
        /// <param name="standardOffset">The standard offset.</param>
        /// <param name="startRecurrence">The start recurrence.</param>
        /// <param name="endRecurrence">The end recurrence.</param>
        internal DaylightSavingsDateTimeZone(String id, Offset standardOffset, ZoneRecurrence startRecurrence, ZoneRecurrence endRecurrence)
            : base(id, false,
            standardOffset + Offset.Min(startRecurrence.Savings, endRecurrence.Savings),
            standardOffset + Offset.Max(startRecurrence.Savings, endRecurrence.Savings))
        {
            this.standardOffset = standardOffset;
            // Treat the recurrences as if they extended to the start of time.
            startRecurrence = startRecurrence.ToStartOfTime();
            endRecurrence = endRecurrence.ToStartOfTime();
            Preconditions.CheckArgument(startRecurrence.IsInfinite, nameof(startRecurrence), "Start recurrence must extend to the end of time");
            Preconditions.CheckArgument(endRecurrence.IsInfinite, nameof(endRecurrence), "End recurrence must extend to the end of time");
            var dst = startRecurrence;
            var standard = endRecurrence;
            if (startRecurrence.Savings == Offset.Zero)
            {
                dst = endRecurrence;
                standard = startRecurrence;
            }
            Preconditions.CheckArgument(standard.Savings == Offset.Zero, nameof(startRecurrence), "At least one recurrence must not have savings applied");
            dstRecurrence = dst;
            standardRecurrence = standard;
        }

        protected override bool EqualsImpl(DateTimeZone other)
        {
            DaylightSavingsDateTimeZone otherZone = (DaylightSavingsDateTimeZone)other;
            return Id == otherZone.Id && 
                standardOffset == otherZone.standardOffset && 
                dstRecurrence.Equals(otherZone.dstRecurrence) &&
                standardRecurrence.Equals(otherZone.standardRecurrence);
        }

        public override int GetHashCode()
        {
            int hashCode = HashCodeHelper.Initialize();
            hashCode = HashCodeHelper.Hash(hashCode, Id);
            hashCode = HashCodeHelper.Hash(hashCode, standardOffset);
            hashCode = HashCodeHelper.Hash(hashCode, dstRecurrence);
            hashCode = HashCodeHelper.Hash(hashCode, standardRecurrence);
            return hashCode;
        }

        /// <summary>
        /// Gets the zone interval for the given instant.
        /// </summary>
        /// <param name="instant">The Instant to test.</param>
        /// <returns>The ZoneInterval in effect at the given instant.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The instant falls outside the bounds
        /// of the recurrence rules of the zone.</exception>
        public override ZoneInterval GetZoneInterval(Instant instant)
        {
            ZoneRecurrence recurrence;
            var next = NextTransition(instant, out recurrence);
            // Now we know the recurrence we're in, we can work out when we went into it. (We'll never have
            // two transitions into the same recurrence in a row.)
            Offset previousSavings = ReferenceEquals(recurrence, standardRecurrence) ? dstRecurrence.Savings : Offset.Zero;
            var previous = recurrence.PreviousOrSameOrFail(instant, standardOffset, previousSavings);
            return new ZoneInterval(recurrence.Name, previous.Instant, next.Instant, standardOffset + recurrence.Savings, recurrence.Savings);
        }

        /// <summary>
        /// Returns the transition occurring strictly after the specified instant. The <paramref name="recurrence"/>
        /// parameter will be populated with the recurrence the transition goes *from*.
        /// </summary>
        /// <param name="instant">The instant after which to consider transitions.</param>
        /// <param name="recurrence">Receives the savings offset for the transition.</param>
        private Transition NextTransition(Instant instant, out ZoneRecurrence recurrence)
        {
            // Both recurrences are infinite, so they'll both have previous transitions (possibly at int.MinValue).
            Transition dstTransition = dstRecurrence.NextOrFail(instant, standardOffset, Offset.Zero);
            Transition standardTransition = standardRecurrence.NextOrFail(instant, standardOffset, dstRecurrence.Savings);
            var standardTransitionInstant = standardTransition.Instant;
            var dstTransitionInstant = dstTransition.Instant;
            if (standardTransitionInstant < dstTransitionInstant)
            {
                // Next transition is from DST to standard.
                recurrence = dstRecurrence;
                return standardTransition;
            }
            else if (standardTransitionInstant > dstTransitionInstant)
            {
                // Next transition is from standard to DST.
                recurrence = standardRecurrence;
                return dstTransition;
            }
            else
            {
                // Okay, the transitions happen at the same time. If they're not at infinity, we're stumped.
                if (standardTransitionInstant.IsValid)
                {
                    throw new InvalidOperationException("Zone recurrence rules have identical transitions. This time zone is broken.");
                }
                // Okay, the two transitions must be to the end of time. Find which recurrence has the later *previous* transition...
                var previousDstTransition = dstRecurrence.PreviousOrSameOrFail(instant, standardOffset, Offset.Zero);
                var previousStandardTransition = standardRecurrence.PreviousOrSameOrFail(instant, standardOffset, dstRecurrence.Savings);
                // No point in checking for equality here... they can't go back from the end of time to the start...
                if (previousDstTransition.Instant > previousStandardTransition.Instant)
                {
                    // The previous transition is from standard to DST. Therefore the next one is from DST to standard.
                    recurrence = dstRecurrence;
                    return standardTransition;
                }
                else
                {
                    // The previous transition is from standard to DST. Therefore the next one is from DST to standard.
                    recurrence = standardRecurrence;
                    return dstTransition;
                }
            }
        }

        /// <summary>
        /// Returns the offset from UTC, where a positive duration indicates that local time is later
        /// than UTC. In other words, local time = UTC + offset.
        /// </summary>
        /// <param name="instant">The instant for which to calculate the offset.</param>
        /// <returns>
        /// The offset from UTC at the specified instant.
        /// </returns>
        public override Offset GetUtcOffset(Instant instant)
        {
            ZoneRecurrence recurrence;
            NextTransition(instant, out recurrence);
            return recurrence.Savings + standardOffset;
        }
        
        /// <summary>
        /// Writes the time zone to the specified writer.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        internal void Write([NotNull] IDateTimeZoneWriter writer)
        {
            // We don't need everything a recurrence can supply: we know that both recurrences should be
            // infinite, and that only the DST recurrence should have savings.
            Preconditions.CheckNotNull(writer, nameof(writer));
            writer.WriteOffset(standardOffset);
            writer.WriteString(standardRecurrence.Name);
            standardRecurrence.YearOffset.Write(writer);
            writer.WriteString(dstRecurrence.Name);
            dstRecurrence.YearOffset.Write(writer);
            writer.WriteOffset(dstRecurrence.Savings);
        }

        internal static DaylightSavingsDateTimeZone Read([NotNull] IDateTimeZoneReader reader, string id)
        {
            Preconditions.CheckNotNull(reader, nameof(reader));
            Offset standardOffset = reader.ReadOffset();
            string standardName = reader.ReadString();
            ZoneYearOffset standardYearOffset = ZoneYearOffset.Read(reader);
            string daylightName = reader.ReadString();
            ZoneYearOffset daylightYearOffset = ZoneYearOffset.Read(reader);
            Offset savings = reader.ReadOffset();
            ZoneRecurrence standardRecurrence = new ZoneRecurrence(standardName, Offset.Zero, standardYearOffset, int.MinValue, int.MaxValue);
            ZoneRecurrence dstRecurrence = new ZoneRecurrence(daylightName, savings, daylightYearOffset, int.MinValue, int.MaxValue);
            return new DaylightSavingsDateTimeZone(id, standardOffset, standardRecurrence, dstRecurrence);
        }
    }
}
