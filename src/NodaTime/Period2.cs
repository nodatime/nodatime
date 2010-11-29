using System;
using System.Collections.Generic;
using NodaTime.Fields;
using System.Collections;

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

        private readonly long[] values;

        /// <summary>
        /// Creates a new period with the given array without copying it. The array contents must
        /// not be changed after the value has been constructed - which is why this method is private.
        /// </summary>
        /// <param name="values"></param>
        private Period2(long[] values)
        {
            this.values = values;
        }

        private static Period2 CreateSingleFieldPeriod(DurationFieldType fieldType, long value)
        {
            long[] values = new long[DurationFieldTypeCount];
            values[(int)fieldType] = value;
            return new Period2(values);
        }

        public static Period2 Years(long years)
        {
            return CreateSingleFieldPeriod(DurationFieldType.Years, years);
        }

        public static Period2 Months(long months)
        {
            return CreateSingleFieldPeriod(DurationFieldType.Months, months);
        }

        public static Period2 Days(long days)
        {
            return CreateSingleFieldPeriod(DurationFieldType.Days, days);
        }

        public static Period2 Hours(long hours)
        {
            return CreateSingleFieldPeriod(DurationFieldType.Hours, hours);
        }

        public static Period2 Minutes(long minutes)
        {
            return CreateSingleFieldPeriod(DurationFieldType.Minutes, minutes);
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
            long[] newValues = new long[DurationFieldTypeCount];
            for (int i = 0; i < newValues.Length; i++)
            {
                newValues[i] = left.values[i] + right.values[i];
            }
            return new Period2(newValues);
        }

        public IEnumerator<DurationFieldValue> GetEnumerator()
        {
            for (long i = 0; i < values.Length; i++)
            {
                yield return new DurationFieldValue((DurationFieldType)i, values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
