using System;
using NodaTime.Calendars;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// Provides time calculations for the week of the weekyear component of time.
    /// </summary>
    internal sealed class BasicWeekYearDateTimeField : DateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicWeekYearDateTimeField(BasicCalendarSystem calendarSystem)
            : base(DateTimeFieldType.WeekYear, new BasicWeekYearPeriodField(calendarSystem))
        {
            this.calendarSystem = calendarSystem;
        }

        /// <summary>
        /// Always returns null(not supported)
        /// </summary>
        internal override PeriodField RangePeriodField { get { return null; } }

        #region Values
        /// <summary>
        /// Get the Year of a week based year component of the specified local instant.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The year extracted from the input.</returns>
        internal override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetWeekYear(localInstant);
        }

        /// <summary>
        /// Get the Year of a week based year component of the specified local instant.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The year extracted from the input.</returns>
        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetWeekYear(localInstant);
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            int year = (int)value;
            // TODO(Post-V1): Check this. In the Java it uses Math.abs, but I'm not convinced that's correct...
            Preconditions.CheckArgumentRange("value", year, calendarSystem.MinYear, calendarSystem.MaxYear);

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
            // TODO(Post-V1): Check this. Doesn't look right, but mirrors the Java code
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
                workInstant += Duration.OneStandardWeek;
            }
            else if (workWeekYear > year)
            {
                workInstant -= Duration.OneStandardWeek;
            }

            // Set the proper week in the current weekyear

            // BEGIN: possible set WeekOfWeekyear logic.
            int currentWeekYearWeek = calendarSystem.GetWeekOfWeekYear(workInstant);
            workInstant += Duration.FromStandardWeeks(setToWeek - currentWeekYearWeek);
            // END: possible set WeekOfWeekyear logic.

            // Reset DayOfWeek to previous value.
            // Note: This works fine, but it ideally shouldn't invoke other
            // fields from within a field.
            workInstant = calendarSystem.Fields.DayOfWeek.SetValue(workInstant, thisDow);

            // Done!
            return workInstant;
        }
        #endregion

        #region Leap
        internal override bool IsLeap(LocalInstant localInstant)
        {
            return GetLeapAmount(localInstant) > 0;
        }

        internal override int GetLeapAmount(LocalInstant localInstant)
        {
            return calendarSystem.GetWeeksInYear(calendarSystem.GetWeekYear(localInstant)) - 52;
        }

        internal override PeriodField LeapPeriodField { get { return calendarSystem.Fields.Weeks; } }
        #endregion

        #region Ranges
        internal override long GetMinimumValue()
        {
            return calendarSystem.MinYear;
        }

        internal override long GetMaximumValue()
        {
            return calendarSystem.MaxYear;
        }
        #endregion

        #region Rounding
        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            // Note: This works fine, but it ideally shouldn't invoke other
            // fields from within a field.
            localInstant = calendarSystem.Fields.WeekOfWeekYear.RoundFloor(localInstant);
            int wow = calendarSystem.GetWeekOfWeekYear(localInstant);
            if (wow > 1)
            {
                localInstant -= Duration.FromStandardWeeks(wow - 1);
            }
            return localInstant;
        }
        #endregion
    }
}