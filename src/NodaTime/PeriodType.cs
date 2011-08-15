#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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
using System.Collections;
using System.Collections.Generic;
using NodaTime.Fields;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// Controls a period implementation by specifying which duration fields are to be used.
    /// </summary>
    /// <remarks>
    /// The properties are effectively singletons - accessing the same property
    /// twice will return the same reference both times. New instances are created with the
    /// WithXXXRemoved methods.
    /// TODO: Consider implementing operator- and operator+?
    /// </remarks>
    public sealed class PeriodType : IEquatable<PeriodType>, IEnumerable<DurationFieldType>
    {
        internal enum Index
        {
            Year,
            Month,
            Week,
            Day,
            Hour,
            Minute,
            Second,
            Millisecond,
            Tick
        }

        private const int IndexCount = 9;

        #region Static fields backing properties
        private static readonly PeriodType years = new PeriodType("Years", DurationFieldType.Years);
        private static readonly PeriodType months = new PeriodType("Months", DurationFieldType.Months);
        private static readonly PeriodType weeks = new PeriodType("Weeks", DurationFieldType.Weeks);
        private static readonly PeriodType days = new PeriodType("Days", DurationFieldType.Days);
        private static readonly PeriodType hours = new PeriodType("Hours", DurationFieldType.Hours);
        private static readonly PeriodType minutes = new PeriodType("Minutes", DurationFieldType.Minutes);
        private static readonly PeriodType seconds = new PeriodType("Seconds", DurationFieldType.Seconds);
        private static readonly PeriodType milliseconds = new PeriodType("Milliseconds", DurationFieldType.Milliseconds);
        private static readonly PeriodType ticks = new PeriodType("Ticks", DurationFieldType.Ticks);

        private static readonly PeriodType allFields = new PeriodType("All", DurationFieldType.Years, DurationFieldType.Months, DurationFieldType.Weeks,
                                                                     DurationFieldType.Days, DurationFieldType.Hours, DurationFieldType.Minutes,
                                                                     DurationFieldType.Seconds, DurationFieldType.Milliseconds, DurationFieldType.Ticks);

        private static readonly PeriodType yearMonthDayTime = new PeriodType("YearMonthDayTime", DurationFieldType.Years, DurationFieldType.Months,
                                                                             DurationFieldType.Days, DurationFieldType.Hours, DurationFieldType.Minutes,
                                                                             DurationFieldType.Seconds, DurationFieldType.Milliseconds, DurationFieldType.Ticks);

        private static readonly PeriodType yearMonthDay = new PeriodType("YearMonthDay", DurationFieldType.Years, DurationFieldType.Months,
                                                                         DurationFieldType.Days);

        private static readonly PeriodType yearWeekDayTime = new PeriodType("YearWeekDayTime", DurationFieldType.Years, DurationFieldType.Weeks,
                                                                            DurationFieldType.Days, DurationFieldType.Hours, DurationFieldType.Minutes,
                                                                            DurationFieldType.Seconds, DurationFieldType.Milliseconds, DurationFieldType.Ticks);

        private static readonly PeriodType yearWeekDay = new PeriodType("YearWeekDay", DurationFieldType.Years, DurationFieldType.Weeks, DurationFieldType.Days);

        private static readonly PeriodType yearDayTime = new PeriodType("YearDayTime", DurationFieldType.Years, DurationFieldType.Days, DurationFieldType.Hours,
                                                                        DurationFieldType.Minutes, DurationFieldType.Seconds, DurationFieldType.Milliseconds,
                                                                        DurationFieldType.Ticks);

        private static readonly PeriodType yearDay = new PeriodType("YearDay", DurationFieldType.Years, DurationFieldType.Days);

        private static readonly PeriodType dayTime = new PeriodType("DayTime", DurationFieldType.Days, DurationFieldType.Hours, DurationFieldType.Minutes,
                                                                    DurationFieldType.Seconds, DurationFieldType.Milliseconds, DurationFieldType.Ticks);

        private static readonly PeriodType time = new PeriodType("Time", DurationFieldType.Hours, DurationFieldType.Minutes, DurationFieldType.Seconds,
                                                                 DurationFieldType.Milliseconds, DurationFieldType.Ticks);
        #endregion

        #region Static properties
        /// <summary>Gets a type that defines just the years field.</summary>
        public static PeriodType Years { get { return years; } }

        /// <summary>Gets a type that defines just the months field.</summary>
        public static PeriodType Months { get { return months; } }

        /// <summary>Gets a type that defines just the weeks field.</summary>
        public static PeriodType Weeks { get { return weeks; } }

        /// <summary>Gets a type that defines just the days field.</summary>
        public static PeriodType Days { get { return days; } }

        /// <summary>Gets a type that defines just the hours field.</summary>
        public static PeriodType Hours { get { return hours; } }

        /// <summary>Gets a type that defines just the minutes field.</summary>
        public static PeriodType Minutes { get { return minutes; } }

        /// <summary>Gets a type that defines just the seconds field.</summary>
        public static PeriodType Seconds { get { return seconds; } }

        /// <summary>Gets a type that defines just the milliseconds field.</summary>
        public static PeriodType Milliseconds { get { return milliseconds; } }

        /// <summary>Gets a type that defines just the ticks field.</summary>
        public static PeriodType Ticks{ get { return ticks; } }

        /// <summary>
        /// Gets a type that defines all fields.
        /// </summary>
        public static PeriodType AllFields { get { return allFields; } }

        /// <summary>
        /// Gets a type that defines all standard fields except weeks.
        /// </summary>
        public static PeriodType YearMonthDayTime { get { return yearMonthDayTime; } }

        /// <summary>
        /// Gets a type that defines the year, month and day fields.
        /// </summary>
        public static PeriodType YearMonthDay { get { return yearMonthDay; } }

        /// <summary>
        /// Gets a type that defines all standard fields except months.
        /// </summary>
        public static PeriodType YearWeekDayTime { get { return yearWeekDayTime; } }

        /// <summary>
        /// Gets a type that defines year, week and day fields.
        /// </summary>
        public static PeriodType YearWeekDay { get { return yearWeekDay; } }

        /// <summary>
        /// Gets a type that defines all standard fields except months and weeks.
        /// </summary>
        public static PeriodType YearDayTime { get { return yearDayTime; } }

        /// <summary>
        /// Gets a type that defines the year and day fields.
        /// </summary>
        public static PeriodType YearDay { get { return yearDay; } }

        /// <summary>
        /// Gets a type that defines all standard fields from days downwards.
        /// </summary>
        public static PeriodType DayTime { get { return dayTime; } }

        /// <summary>
        /// Gets a type that defines all standard time fields.
        /// </summary>
        public static PeriodType Time { get { return time; } }
        #endregion

        private readonly string name;
        private readonly DurationFieldType[] fieldTypes;
        private readonly bool hasTimeFields;
        private readonly bool hasDateFields;

        // The sole purpose of this member is to improve perfomance
        // of searching the index of the field for particular period type.
        // Otherwise, it would be looping through fieldTypes array.
        private readonly int[] indices;

        private PeriodType(string name, params DurationFieldType[] fieldTypes) : this(name, fieldTypes, BuildIndices(fieldTypes))
        {
        }

        /// <summary>
        /// Builds a set of indexes from the given field types, to avoid having to specify them
        /// in all the static variable initializers. This doesn't have to be particularly fast,
        /// as it's not called often.
        /// </summary>
        private static int[] BuildIndices(DurationFieldType[] fieldTypes)
        {
            int[] ret = new[] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            for (int i = 0; i < fieldTypes.Length; i++)
            {
                switch (fieldTypes[i])
                {
                    case DurationFieldType.Days:
                        ret[(int)Index.Day] = i;
                        break;
                    case DurationFieldType.Hours:
                        ret[(int)Index.Hour] = i;
                        break;
                    case DurationFieldType.Milliseconds:
                        ret[(int)Index.Millisecond] = i;
                        break;
                    case DurationFieldType.Minutes:
                        ret[(int)Index.Minute] = i;
                        break;
                    case DurationFieldType.Months:
                        ret[(int)Index.Month] = i;
                        break;
                    case DurationFieldType.Seconds:
                        ret[(int)Index.Second] = i;
                        break;
                    case DurationFieldType.Ticks:
                        ret[(int)Index.Tick] = i;
                        break;
                    case DurationFieldType.Weeks:
                        ret[(int)Index.Week] = i;
                        break;
                    case DurationFieldType.Years:
                        ret[(int)Index.Year] = i;
                        break;
                    default:
                        throw new ArgumentException("Invalid duration field type for period type");
                }
            }
            return ret;
        }

        private PeriodType(string name, DurationFieldType[] fieldTypes, int[] indices)
        {
            this.name = name;
            this.fieldTypes = fieldTypes;
            this.indices = indices;
            hasTimeFields = IsSupported(DurationFieldType.Hours) || IsSupported(DurationFieldType.Minutes) ||
                            IsSupported(DurationFieldType.Seconds) || IsSupported(DurationFieldType.Milliseconds) ||
                            IsSupported(DurationFieldType.Ticks);
            hasDateFields = IsSupported(DurationFieldType.Years) || IsSupported(DurationFieldType.Months) ||
                            IsSupported(DurationFieldType.Weeks) || IsSupported(DurationFieldType.Days);
        }

        /// <summary>
        /// Gets the name of the period type.
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// Gets the number of fields in the period type.
        /// </summary>
        public int Size { get { return fieldTypes.Length; } }

        /// <summary>
        /// Returns whether or not this period type supports any duration fields smaller than a day.
        /// </summary>
        public bool HasTimeFields { get { return hasTimeFields; } }

        /// <summary>
        /// Returns whether or not this period type supports any duration fields larger than an hour.
        /// </summary>
        public bool HasDateFields { get { return hasDateFields; } }
        
        /// <summary>
        /// Gets the index of the field in this period.
        /// </summary>
        /// <param name="fieldType">The type to check; may be null which returns -1</param>
        /// <returns>The index or -1 if not supported</returns>
        public int IndexOf(DurationFieldType fieldType)
        {
            return Array.IndexOf(fieldTypes, fieldType);
        }

        /// <summary>
        /// Checks whether the field specified is supported by this period.
        /// </summary>
        /// <param name="fieldType">The type to check, may be null which returns false</param>
        /// <returns>True if the field is supported, false otherwise</returns>
        public bool IsSupported(DurationFieldType fieldType)
        {
            return (IndexOf(fieldType) >= 0);
        }

        internal int GetRealIndex(Index index)
        {
            return indices[(int)index];
        }

        #region Masking
        private PeriodType WithFieldRemoved(Index index, string suffix)
        {
            int fieldIndex = GetRealIndex(index);
            if (fieldIndex == -1)
            {
                return this;
            }

            // Construct new field types
            DurationFieldType[] newFieldTypes = new DurationFieldType[Size - 1];
            for (int i = 0; i < fieldTypes.Length; i++)
            {
                if (i < fieldIndex)
                {
                    newFieldTypes[i] = fieldTypes[i];
                }
                else if (i > fieldIndex)
                {
                    newFieldTypes[i - 1] = fieldTypes[i];
                }
            }

            // Construct new indices
            int[] newIndices = new int[IndexCount];
            int indicesIndex = (int)index;
            for (int i = 0; i < indicesIndex; i++)
            {
                newIndices[i] = indices[i];
            }
            newIndices[indicesIndex] = -1;
            for (int i = indicesIndex + 1; i < newIndices.Length; i++)
            {
                newIndices[i] = Math.Max(indices[i] - 1, -1);
            }

            return new PeriodType(Name + suffix, newFieldTypes, newIndices);
        }

        /// <summary>
        /// Returns a version of this PeriodType instance that does not support years.
        /// </summary>
        /// <returns>A new period type that supports the original set of fields except years</returns>
        public PeriodType WithYearsRemoved()
        {
            return WithFieldRemoved(Index.Year, "NoYears");
        }

        /// <summary>
        /// Returns a version of this PeriodType instance that does not support months.
        /// </summary>
        /// <returns>A new period type that supports the original set of fields except months</returns>
        public PeriodType WithMonthsRemoved()
        {
            return WithFieldRemoved(Index.Month, "NoMonths");
        }

        /// <summary>
        /// Returns a version of this PeriodType instance that does not support weeks.
        /// </summary>
        /// <returns>A new period type that supports the original set of fields except weeks</returns>
        public PeriodType WithWeeksRemoved()
        {
            return WithFieldRemoved(Index.Week, "NoWeeks");
        }

        /// <summary>
        /// Returns a version of this PeriodType instance that does not support days.
        /// </summary>
        /// <returns>A new period type that supports the original set of fields except days</returns>
        public PeriodType WithDaysRemoved()
        {
            return WithFieldRemoved(Index.Day, "NoDays");
        }

        /// <summary>
        /// Returns a version of this PeriodType instance that does not support hours.
        /// </summary>
        /// <returns>A new period type that supports the original set of fields except hours</returns>
        public PeriodType WithHoursRemoved()
        {
            return WithFieldRemoved(Index.Hour, "NoHours");
        }

        /// <summary>
        /// Returns a version of this PeriodType instance that does not support minutes.
        /// </summary>
        /// <returns>A new period type that supports the original set of fields except minutes</returns>
        public PeriodType WithMinutesRemoved()
        {
            return WithFieldRemoved(Index.Minute, "NoMinutes");
        }

        /// <summary>
        /// Returns a version of this PeriodType instance that does not support seconds.
        /// </summary>
        /// <returns>A new period type that supports the original set of fields except seconds</returns>
        public PeriodType WithSecondsRemoved()
        {
            return WithFieldRemoved(Index.Second, "NoSeconds");
        }

        /// <summary>
        /// Returns a version of this PeriodType instance that does not support milliseconds.
        /// </summary>
        /// <returns>A new period type that supports the original set of fields except milliseconds</returns>
        public PeriodType WithMillisecondsRemoved()
        {
            return WithFieldRemoved(Index.Millisecond, "NoMilliseconds");
        }

        /// <summary>
        /// Returns a version of this PeriodType instance that does not support ticks.
        /// </summary>
        /// <returns>A new period type that supports the original set of fields except ticks</returns>
        public PeriodType WithTicksRemoved()
        {
            return WithFieldRemoved(Index.Tick, "NoTicks");
        }
        #endregion

        #region Equality
        /// <summary>
        /// Compares this object with another <see cref="PeriodType"/> for equality.
        /// </summary>
        /// <returns>True if <paramref name="other"/> consists of the same field types as this period type; False otherwise.</returns>
        public bool Equals(PeriodType other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            if (fieldTypes.Length != other.fieldTypes.Length)
            {
                return false;
            }

            // Check for elements equality
            for (int i = 0; i < fieldTypes.Length; i++)
            {
                if (fieldTypes[i] != other.fieldTypes[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares this object with another for equality.
        /// </summary>
        /// <returns>True if <paramref name="obj"/> is a <see cref="PeriodType"/> consisting of the same field types; False otherwise.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as PeriodType);
        }

        /// <summary>
        /// Returns the hash code for this period type, based on the field types within it.
        /// </summary>
        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            for (int i = 0; i < fieldTypes.Length; i++)
            {
                hash = HashCodeHelper.Hash(hash, fieldTypes[i]);
            }

            return hash;
        }

        /// <summary>
        /// Compares two period types for equality.
        /// </summary>
        public static bool operator ==(PeriodType left, PeriodType right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two period types for inequality.
        /// </summary>
        public static bool operator !=(PeriodType left, PeriodType right)
        {
            return !Equals(left, right);
        }
        #endregion

        internal void UpdateIndexedField(int[] values, Index index, int newValue, bool add)
        {
            int realIndex = GetRealIndex(index);
            Update(values, realIndex, newValue, add);
        }

        internal void UpdateAnyField(int[] values, DurationFieldType fieldType, int newValue, bool add)
        {
            int index = IndexOf(fieldType);
            Update(values, index, newValue, add);
        }

        private static void Update(int[] values, int index, int newValue, bool add)
        {
            if (index == -1)
            {
                if (newValue != 0)
                {
                    throw new NotSupportedException("Field is not supported");
                }
            }
            else
            {
                values[index] = newValue + (add ? values[index] : 0);
            }
        }

        /// <summary>
        /// Returns a text representation of this period type.
        /// </summary>
        public override string ToString()
        {
            return "PeriodType[" + Name + "]";
        }

        /// <summary>
        /// Returns an iterator over the field types within this period.
        /// </summary>
        public IEnumerator<DurationFieldType> GetEnumerator()
        {
            return ((IEnumerable<DurationFieldType>)fieldTypes).GetEnumerator();
        }

        /// <summary>
        /// Returns the field type at the given index, which must be between 0 (inclusive) and <see cref="Size"/> (exclusive).
        /// </summary>
        public DurationFieldType this[int index]
        {
            get { return fieldTypes[index]; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}