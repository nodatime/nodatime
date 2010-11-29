using System;
using System.Collections.Generic;
using NodaTime.Fields;
using System.Collections;
using NodaTime.Periods;

namespace NodaTime
{
    /// <summary>
    /// Represents a period of time expressed in human chronological terms: hours, days,
    /// weeks, months and so on. All implementations in Noda Time are immutable, and return fields
    /// in descending size order: hours before minutes, for example.
    /// </summary>
    /// <remarks>
    /// Periods operate on calendar-related types such as
    /// <see cref="LocalDateTime" /> whereas <see cref="Duration"/> operates on instants
    /// on the time line. <see cref="ZonedDateTime" /> includes both concepts, so both
    /// durations and periods can be added to zoned date-time instances - although the
    /// results may not be the same. For example, adding a two hour period to a ZonedDateTime
    /// will always give a result has a local date-time which is two hours later, even if that
    /// means that three hours would have to actually pass in experienced time to arrive at
    /// that local date-time, due to changes in the UTC offset (e.g. for daylight savings).
    /// </remarks>
    public sealed class Period2 : IEnumerable<DurationFieldValue>
    {
        private const int DurationFieldTypeCount = ((int) DurationFieldType.Ticks) + 1;

        private readonly PeriodType periodType;
        private readonly long[] values;

        /// <summary>
        /// Creates a new period with the given array without copying it. The array contents must
        /// not be changed after the value has been constructed - which is why this method is private.
        /// </summary>
        /// <param name="periodType">Type of this period, describing which fields are present</param>
        /// <param name="values">Values for each field in the period type</param>
        private Period2(PeriodType periodType, long[] values)
        {
            this.values = values;
            this.periodType = periodType;
        }

        private static Period2 CreateSingleFieldPeriod(PeriodType periodType, long value)
        {
            long[] values = { value };
            return new Period2(periodType, values);
        }

        public static Period2 Years(long years)
        {
            return CreateSingleFieldPeriod(PeriodType.Years, years);
        }

        public static Period2 Months(long months)
        {
            return CreateSingleFieldPeriod(PeriodType.Months, months);
        }

        public static Period2 Days(long days)
        {
            return CreateSingleFieldPeriod(PeriodType.Days, days);
        }

        public static Period2 Hours(long hours)
        {
            return CreateSingleFieldPeriod(PeriodType.Hours, hours);
        }

        public static Period2 Minutes(long minutes)
        {
            return CreateSingleFieldPeriod(PeriodType.Minutes, minutes);
        }

        public static Period2 Seconds(long seconds)
        {
            return CreateSingleFieldPeriod(PeriodType.Seconds, seconds);
        }

        public static Period2 Millseconds(long milliseconds)
        {
            return CreateSingleFieldPeriod(PeriodType.Milliseconds, milliseconds);
        }

        public static Period2 Ticks(long ticks)
        {
            return CreateSingleFieldPeriod(PeriodType.Ticks, ticks);
        }

        public static Period2 operator +(Period2 left, Period2 right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            PeriodType all = PeriodType.AllFields;
            long[] newValues = new long[all.Size];
            // TODO: Make this a lot faster :)
            for (int i = 0; i < all.Size; i++)
            {
                DurationFieldType fieldType = all[i];
                newValues[i] = left[fieldType] + right[fieldType];
            }
            return new Period2(PeriodType.AllFields, newValues);
        }

        public IEnumerator<DurationFieldValue> GetEnumerator()
        {
            for (int i = 0; i < values.Length; i++)
            {
                yield return new DurationFieldValue(periodType[i], values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns the value of the given field within this period. If the period does not contain
        /// the given field, 0 is returned.
        /// </summary>
        public long this[DurationFieldType fieldType]
        {
            get
            {
                int index = periodType.IndexOf(fieldType);
                return index == -1 ? 0 : values[index];
            }
        }
    }
}
