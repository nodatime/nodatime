using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodaTime.TimeZones;

namespace NodaTime.Experimental
{
    /// <summary>
    /// Representation of a time zone converted from a <see cref="TimeZoneInfo"/> from .NET.
    /// </summary>
    public sealed class BclTimeZone : DateTimeZone
    {
        private readonly TimeZoneInfo bclZone;
        private readonly List<AdjustmentInterval> intervals;
        private readonly ZoneInterval headInterval;

        /// <summary>
        /// Returns the original <see cref="TimeZoneInfo"/> from which this was created.
        /// </summary>
        public TimeZoneInfo OriginalZone { get { return bclZone; } }

        private BclTimeZone(TimeZoneInfo bclZone, Offset minOffset, Offset maxOffset)
            : base(bclZone.Id, bclZone.SupportsDaylightSavingTime, minOffset, maxOffset)
        {
        }

        public override ZoneInterval GetZoneInterval(Instant instant)
        {
            if (headInterval != null && headInterval.Contains(instant))
            {
                return headInterval;
            }
            // Avoid having to worry about Instant.MaxValue for the rest of the class.
            if (instant == Instant.MaxValue)
            {
                return intervals[intervals.Count - 1].GetZoneInterval(instant);
            }

            int lower = 0; // Inclusive
            int upper = intervals.Count; // Exclusive

            while (lower < upper)
            {
                int current = (lower + upper) / 2;
                var candidate = intervals[current];
                if (candidate.Start > instant)
                {
                    upper = current;
                }
                else if (candidate.End < instant)
                {
                    lower = current + 1;
                }
                else
                {
                    return candidate.GetZoneInterval(instant);
                }
            }
            throw new InvalidOperationException
                ("Instant " + instant + " did not exist in the range of adjustment intervals");
        }

        internal static BclTimeZone FromTimeZoneInfo(TimeZoneInfo bclZone)
        {
            Offset standardOffset = Offset.FromTimeSpan(bclZone.BaseUtcOffset);

            if (!bclZone.SupportsDaylightSavingTime)
            {
                return new BclTimeZone(bclZone, standardOffset, standardOffset);
            }
            var rules = bclZone.GetAdjustmentRules();
            var headInterval = ComputeHeadInterval(bclZone, rules[0]);
            Instant previousEnd = headInterval != null ? headInterval.End : Instant.MinValue;
            for (int i = 0; i < rules.Length; i++)
            {
                var rule = rules[i];
                ZoneRecurrence standard, daylight;
                GetRecurrences(bclZone, rule, out standard, out daylight);
                // Find the last valid transition
                var lastStandard = standard.Previous(Instant.MaxValue, standardOffset, daylight.Savings).Value;
                var lastDaylight = daylight.Previous(Instant.MaxValue, standardOffset, Offset.Zero).Value;
                Offset seamOffset = lastStandard.Instant > lastDaylight.Instant ? Offset.Zero : daylight.Savings;
                string seamName = lastStandard.Instant > lastDaylight.Instant ? standard.Name : daylight.Name;
                ZoneInterval seam;
                if (i == rules.Length - 1)
                {
                    seam = new ZoneInterval(seamName, Instant.Max(lastStandard.Instant, lastDaylight.Instant),
                }
            }
        }

        private static ZoneInterval ComputeHeadInterval(TimeZoneInfo zone, TimeZoneInfo.AdjustmentRule rule)
        {
            if (rule.DateStart.Year == 1)
            {
                return null;
            }
            ZoneRecurrence firstStandard, firstDaylight;
            GetRecurrences(zone, rule, out firstStandard, out firstDaylight);
            var standardOffset = Offset.FromTimeSpan(zone.BaseUtcOffset);
            Transition? firstTransition = firstDaylight.Next(Instant.MinValue,
                standardOffset, Offset.Zero);
            if (firstTransition == null)
            {
                throw new InvalidOperationException("Adjustment rule never has an effect");
            }
            return new ZoneInterval(zone.StandardName, Instant.MinValue,
                firstTransition.Value.Instant, standardOffset, Offset.Zero);
        }

        private static void GetRecurrences(TimeZoneInfo zone, TimeZoneInfo.AdjustmentRule rule,
            out ZoneRecurrence standardRecurrence, out ZoneRecurrence daylightRecurrence)
        {
            int startYear = rule.DateStart.Year == 1 ? int.MinValue : rule.DateStart.Year;
            int endYear = rule.DateStart.Year == 9999 ? int.MaxValue : rule.DateStart.Year;
            Offset daylightOffset = Offset.FromTimeSpan(rule.DaylightDelta);
            ZoneYearOffset daylightStart = ConvertTransition(rule.DaylightTransitionStart);
            ZoneYearOffset daylightEnd = ConvertTransition(rule.DaylightTransitionEnd);
            standardRecurrence = new ZoneRecurrence(zone.StandardName, Offset.Zero, daylightEnd, startYear, endYear);
            daylightRecurrence = new ZoneRecurrence(zone.DaylightName, daylightOffset, daylightStart, startYear, endYear);
        }

        private static ZoneYearOffset ConvertTransition(TimeZoneInfo.TransitionTime transitionTime)
        {
            // Easy case - fixed day of the month.
            if (transitionTime.IsFixedDateRule)
            {
                return new ZoneYearOffset(TransitionMode.Wall, transitionTime.Month, transitionTime.Day,
                    0, false, Offset.FromTimeSpan(transitionTime.TimeOfDay.TimeOfDay));
            }

            // Floating: 1st Sunday in March etc.
            IsoDayOfWeek dayOfWeek = BclConversions.ToIsoDayOfWeek(transitionTime.DayOfWeek);
            int dayOfMonth;
            bool advance;
            // "Last"
            if (transitionTime.Week == 5)
            {
                advance = false;
                dayOfMonth = -1;
            }
            else
            {
                advance = true;
                // Week 1 corresponds to ">=1"
                // Week 2 corresponds to ">=8" etc
                dayOfMonth = (transitionTime.Week * 7) - 6;
            }
            return new ZoneYearOffset(TransitionMode.Wall, transitionTime.Month, dayOfMonth,
                (int)dayOfWeek, advance, Offset.FromTimeSpan(transitionTime.TimeOfDay.TimeOfDay));
        }


        internal override void Write(DateTimeZoneWriter writer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Interval covered by an adjustment rule. The start instant is that of the
        /// first transition reported by this rule, and the seam covers the gap between
        /// two adjustment rules.
        /// </summary>
        private class AdjustmentInterval
        {
            private readonly Instant start;
            private readonly ZoneInterval seam;
            private readonly DaylightSavingsTimeZone adjustmentZone;

            internal Instant Start { get { return start; } }
            internal Instant End { get { return seam.End; } }

            internal AdjustmentInterval(Instant start,
                ZoneRecurrence standardRecurrence,
                ZoneRecurrence daylightRecurrence,
                Offset standardOffset, ZoneInterval seam)
            {
                this.start = start;
                this.seam = seam;
                this.adjustmentZone = new DaylightSavingsTimeZone("ignored", standardOffset,
                    standardRecurrence.ToInfinity(), daylightRecurrence.ToInfinity());
            }

            internal ZoneInterval GetZoneInterval(Instant instant)
            {
                if (seam.Contains(instant))
                {
                    return seam;
                }
                return adjustmentZone.GetZoneInterval(instant);
            }
        }
    }
}
