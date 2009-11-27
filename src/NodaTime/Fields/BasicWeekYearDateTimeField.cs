using System;
using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Porting status: Needs AddWrapField.
    /// </summary>
    internal sealed class BasicWeekYearDateTimeField : ImpreciseDateTimeField
    {
        private static readonly Duration Week53Ticks = Duration.StandardWeeks(52);
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicWeekYearDateTimeField(BasicCalendarSystem calendarSystem)
            : base(DateTimeFieldType.WeekYear, calendarSystem.AverageTicksPerYear)
        {
            this.calendarSystem = calendarSystem;
        }

        public override bool IsLenient { get { return false; } }

        public override int GetValue(LocalInstant localInstant)
        {
 	         return calendarSystem.GetWeekYear(localInstant);
        }

        public override long GetInt64Value(LocalInstant localInstant)
        {
 	         return calendarSystem.GetWeekYear(localInstant);
        }

        public override LocalInstant Add(LocalInstant localInstant, int value)
        {
            return value == 0 ? localInstant : SetValue(localInstant, GetValue(localInstant) + value);
        }

        public override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return Add(localInstant, (int)value);
        }

        public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            // TODO: use operators when we can :)
            if (minuendInstant < subtrahendInstant)
            {
                return -GetInt64Difference(subtrahendInstant, minuendInstant);
            }
            int minuendWeekYear = GetValue(minuendInstant);
            int subtrahendWeekYear = GetValue(subtrahendInstant);

            Duration minuendRemainder = Remainder(minuendInstant);
            Duration subtrahendRemainder = Remainder(subtrahendInstant);

            // Balance leap weekyear differences on remainders.
            if (subtrahendRemainder >= Week53Ticks && calendarSystem.GetWeeksInYear(minuendWeekYear) <= 52)
            {
                subtrahendRemainder -= DateTimeConstants.Durations.OneWeek;
            }

            int difference = minuendWeekYear - subtrahendWeekYear;
            if (minuendRemainder < subtrahendRemainder)
            {
                difference--;
            }
            return difference;
        }

        public override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            int year = (int) value;
            // TODO: Check this. In the Java it uses Math.abs, but I'm not convinced that's correct...
            FieldUtils.VerifyValueBounds(this, year, calendarSystem.MinYear, calendarSystem.MaxYear);

            // Do nothing if no real change is requested
            int thisWeekYear = GetValue(localInstant);
            if (thisWeekYear == year)
            {
                return localInstant;
            }

            // Calculate the day of week (to be preserved)
            int thisDow = calendarSystem.GetDayOfWeek(localInstant);

            // Calculate the maximum weeks in the target year
            int weeksInFromYear = calendarSystem.GetWeeksInYear(thisWeekYear);
            int weeksInToYear = calendarSystem.GetWeeksInYear(year);
            // TODO: Check this. Doesn't look right, but mirrors the Java code
            int maxOutWeeks = Math.Min(weeksInToYear, weeksInFromYear);

            // Get the current week of the year. This will be preserved in
            // the output unless it is greater than the maximum possible
            // for the target weekyear.  In that case it is adjusted
            // to the maximum possible.
            int setToWeek = Math.Min(maxOutWeeks, calendarSystem.GetWeekOfWeekYear(localInstant));
            
            // Get a working copy of the current date-time. This can be a convenience for debugging
            LocalInstant workInstant = localInstant;

            // Attempt to get closer to the proper weekyear.
            // Note - we cannot currently call ourself, so we just call
            // set for the year. This at least gets us close.
            workInstant = calendarSystem.SetYear(workInstant, year);

            // Calculate the weekyear number for the approximation
            // (which might or might not be equal to the year just set)
            int workWeekYear = GetValue(workInstant);

            // At most we are off by one year, which can be "fixed" by adding or subtracting a week
            if (workWeekYear < year)
            {
                workInstant += DateTimeConstants.Durations.OneWeek;
            } 
            else if (workWeekYear > year)
            {
                workInstant -= DateTimeConstants.Durations.OneWeek;
            }

            // Set the proper week in the current weekyear

            // BEGIN: possible set WeekOfWeekyear logic.
            int currentWeekYearWeek = calendarSystem.GetWeekOfWeekYear(workInstant);
            workInstant += Duration.StandardWeeks(setToWeek - currentWeekYearWeek);
            // END: possible set WeekOfWeekyear logic.

            // Reset DayOfWeek to previous value.
            // Note: This works fine, but it ideally shouldn't invoke other
            // fields from within a field.
            workInstant = calendarSystem.Fields.DayOfWeek.SetValue(workInstant, thisDow);

            // Done!
            return workInstant;
        }

        public override DurationField  RangeDurationField { get { return null; } }

        public override bool IsLeap(LocalInstant localInstant)
        {
 	        return GetLeapAmount(localInstant) > 0;
        }

        public override int  GetLeapAmount(LocalInstant localInstant)
        {
 	        return calendarSystem.GetWeeksInYear(calendarSystem.GetWeekYear(localInstant)) - 52;
        }

        public override DurationField LeapDurationField { get { return calendarSystem.Fields.Weeks; } }

        public override long GetMinimumValue()
        {
            return calendarSystem.MinYear;
        }

        public override long GetMaximumValue()
        {
            return calendarSystem.MaxYear;
        }

        public override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            // Note: This works fine, but it ideally shouldn't invoke other
            // fields from within a field.
            localInstant = calendarSystem.Fields.WeekOfWeekYear.RoundFloor(localInstant);
            int wow = calendarSystem.GetWeekOfWeekYear(localInstant);
            if (wow > 1)
            {
                localInstant -= Duration.StandardWeeks(wow - 1);
            }
            return localInstant;
        }

        public override Duration Remainder(LocalInstant localInstant)
        {
            return localInstant - RoundFloor(localInstant);
        }
    }
}
