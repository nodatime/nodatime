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
using NodaTime.Fields;
using NodaTime.Utility;

namespace NodaTime.Periods
{
    /// <summary>
    /// Controls a period implementation by specifying which duration fields are to be used.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The properties are effectively singletons - accessing the same property
    /// twice will return the same reference both times. New instances are created with the 
    /// </para>
    /// <para>
    /// Standard - years, months, weeks, days, hours, minutes, seconds, millis
    /// YearMonthDayTime - years, months, days, hours, minutes, seconds, millis
    /// YearMonthDay - years, months, days
    /// YearWeekDayTime - years, weeks, days, hours, minutes, seconds, millis
    /// YearWeekDay - years, weeks, days
    /// YearDayTime - years, days, hours, minutes, seconds, millis
    /// YearDay - years, days, hours
    /// DayTime - days, hours, minutes, seconds, millis
    /// Time - hours, minutes, seconds, millis
    /// plus one for each single type
    /// </para>
    /// <para>
    /// TODO: Consider ticks as well.
    /// </para>
    /// </remarks>
    public sealed class PeriodType : IEquatable<PeriodType>
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
            Millisecond
        }

        #region Static fields backing properties

        private static readonly PeriodType years = new PeriodType("Years", DurationFieldType.Years);
        private static readonly PeriodType months = new PeriodType("Months",  DurationFieldType.Months);
        private static readonly PeriodType weeks = new PeriodType("Weeks",  DurationFieldType.Weeks);
        private static readonly PeriodType days = new PeriodType("Days",  DurationFieldType.Days);
        private static readonly PeriodType hours = new PeriodType("Hours",  DurationFieldType.Hours);
        private static readonly PeriodType minutes = new PeriodType("Minutes",  DurationFieldType.Minutes);
        private static readonly PeriodType seconds = new PeriodType("Seconds",  DurationFieldType.Seconds);
        private static readonly PeriodType milliseconds = new PeriodType("Milliseconds",  DurationFieldType.Milliseconds);
        private static readonly PeriodType standard = new PeriodType("Standard", 
            DurationFieldType.Years, DurationFieldType.Months, DurationFieldType.Weeks, DurationFieldType.Days,
            DurationFieldType.Hours, DurationFieldType.Minutes,DurationFieldType.Seconds, DurationFieldType.Milliseconds);
        private static readonly PeriodType yearMonthDayTime = new PeriodType("YearMonthDayTime", 
            DurationFieldType.Years, DurationFieldType.Months, DurationFieldType.Days,
            DurationFieldType.Hours, DurationFieldType.Minutes, DurationFieldType.Seconds, DurationFieldType.Milliseconds);

        private static readonly PeriodType yearMonthDay = new PeriodType("YearMonthDay", DurationFieldType.Years, DurationFieldType.Months, DurationFieldType.Days);

        private static readonly PeriodType yearWeekDayTime = new PeriodType("YearWeekDayTime", 
            DurationFieldType.Years, DurationFieldType.Weeks, DurationFieldType.Days,
            DurationFieldType.Hours, DurationFieldType.Minutes,
            DurationFieldType.Seconds, DurationFieldType.Milliseconds);

        private static readonly PeriodType yearWeekDay = new PeriodType("YearWeekDay",
            DurationFieldType.Years, DurationFieldType.Weeks, DurationFieldType.Days);

        private static readonly PeriodType yearDayTime = new PeriodType("YearDayTime", 
            DurationFieldType.Years, DurationFieldType.Days,
            DurationFieldType.Hours, DurationFieldType.Minutes,
            DurationFieldType.Seconds, DurationFieldType.Milliseconds);

        private static readonly PeriodType yearDay = new PeriodType("YearDay", DurationFieldType.Years, DurationFieldType.Days);

        private static readonly PeriodType dayTime = new PeriodType("DayTime", 
            DurationFieldType.Days, DurationFieldType.Hours, DurationFieldType.Minutes,
            DurationFieldType.Seconds, DurationFieldType.Milliseconds);

        private static readonly PeriodType time = new PeriodType("Time", 
            DurationFieldType.Hours, DurationFieldType.Minutes,
            DurationFieldType.Seconds, DurationFieldType.Milliseconds);
        #endregion

        #region Static Properties

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

        /// <summary>
        /// Gets a type that defines all standard fields.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <listheader>
        /// <term>Field</term>
        /// <description>Summary</description>
        /// </listheader>
        /// <item>
        /// <term>Years</term>
        /// <description>Years</description>
        /// <term>Months</term>
        /// <description>Months</description>
        /// <term>Weeks</term>
        /// <description>Weeks</description>
        /// <term>Days</term>
        /// <description>Days</description>
        /// <term>Hours</term>
        /// <description>Hours</description>
        /// <term>Minutes</term>
        /// <description>Minutes</description>
        /// <term>Seconds</term>
        /// <description>Seconds</description>
        /// <term>Milliseconds</term>
        /// <description>Milliseconds</description>       
        /// </item>
        /// </list>
        /// </remarks>
        public static PeriodType Standard { get { return standard; } }

        /// <summary>
        /// Gets a type that defines all standard fields except weeks.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <listheader>
        /// <term>Field</term>
        /// <description>Summary</description>
        /// </listheader>
        /// <item>
        /// <term>Years</term>
        /// <description>Years</description>
        /// <term>Months</term>
        /// <description>Months</description>
        /// <term>Days</term>
        /// <description>Days</description>
        /// <term>Hours</term>
        /// <description>Hours</description>
        /// <term>Minutes</term>
        /// <description>Minutes</description>
        /// <term>Seconds</term>
        /// <description>Seconds</description>
        /// <term>Milliseconds</term>
        /// <description>Milliseconds</description>       
        /// </item>
        /// </list>
        /// </remarks>
        public static PeriodType YearMonthDayTime { get { return yearMonthDayTime; } }

        /// <summary>
        /// Gets a type that defines the year, month and day fields.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <listheader>
        /// <term>Field</term>
        /// <description>Summary</description>
        /// </listheader>
        /// <item>
        /// <term>Years</term>
        /// <description>Years</description>
        /// <term>Months</term>
        /// <description>Months</description>
        /// <term>Days</term>
        /// <description>Days</description>
        /// </item>
        /// </list>
        /// </remarks>
        public static PeriodType YearMonthDay { get { return yearMonthDay; } }

        /// <summary>
        /// Gets a type that defines all standard fields except months.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <listheader>
        /// <term>Field</term>
        /// <description>Summary</description>
        /// </listheader>
        /// <item>
        /// <term>Years</term>
        /// <description>Years</description>
        /// <term>Weeks</term>
        /// <description>Weeks</description>
        /// <term>Days</term>
        /// <description>Days</description>
        /// <term>Hours</term>
        /// <description>Hours</description>
        /// <term>Minutes</term>
        /// <description>Minutes</description>
        /// <term>Seconds</term>
        /// <description>Seconds</description>
        /// <term>Milliseconds</term>
        /// <description>Milliseconds</description>       
        /// </item>
        /// </list>
        /// </remarks>
        public static PeriodType YearWeekDayTime { get { return yearWeekDayTime; } }

        /// <summary>
        /// Gets a type that defines year, week and day fields.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <listheader>
        /// <term>Field</term>
        /// <description>Summary</description>
        /// </listheader>
        /// <item>
        /// <term>Years</term>
        /// <description>Years</description>
        /// <term>Weeks</term>
        /// <description>Weeks</description>
        /// <term>Days</term>
        /// <description>Days</description>
        /// </item>
        /// </list>
        /// </remarks>
        public static PeriodType YearWeekDay { get { return yearWeekDay; } }

        /// <summary>
        /// Gets a type that defines all standard fields except months and weeks.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <listheader>
        /// <term>Field</term>
        /// <description>Summary</description>
        /// </listheader>
        /// <item>
        /// <term>Years</term>
        /// <description>Years</description>
        /// <term>Days</term>
        /// <description>Days</description>
        /// <term>Hours</term>
        /// <description>Hours</description>
        /// <term>Minutes</term>
        /// <description>Minutes</description>
        /// <term>Seconds</term>
        /// <description>Seconds</description>
        /// <term>Milliseconds</term>
        /// <description>Milliseconds</description>       
        /// </item>
        /// </list>
        /// </remarks>
        public static PeriodType YearDayTime { get { return yearDayTime; } }
        
        /// <summary>
        /// Gets a type that defines the year and day fields.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <listheader>
        /// <term>Field</term>
        /// <description>Summary</description>
        /// </listheader>
        /// <item>
        /// <term>Years</term>
        /// <description>Years</description>
        /// <term>Days</term>
        /// <description>Days</description>
        /// </item>
        /// </list>
        /// </remarks>
        public static PeriodType YearDay { get { return yearDay; } }

        /// <summary>
        /// Gets a type that defines all standard fields from days downwards.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <listheader>
        /// <term>Field</term>
        /// <description>Summary</description>
        /// </listheader>
        /// <item>
        /// <term>Days</term>
        /// <description>Days</description>
        /// <term>Hours</term>
        /// <description>Hours</description>
        /// <term>Minutes</term>
        /// <description>Minutes</description>
        /// <term>Seconds</term>
        /// <description>Seconds</description>
        /// <term>Milliseconds</term>
        /// <description>Milliseconds</description>       
        /// </item>
        /// </list>
        /// </remarks>
        public static PeriodType DayTime { get { return dayTime; } }

        /// <summary>
        /// Gets a type that defines all standard time fields.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <listheader>
        /// <term>Field</term>
        /// <description>Summary</description>
        /// </listheader>
        /// <item>
        /// <term>Hours</term>
        /// <description>Hours</description>
        /// <term>Minutes</term>
        /// <description>Minutes</description>
        /// <term>Seconds</term>
        /// <description>Seconds</description>
        /// <term>Milliseconds</term>
        /// <description>Milliseconds</description>       
        /// </item>
        /// </list>
        /// </remarks>
        public static PeriodType Time { get { return time; } }
        #endregion

        private readonly string name;
        private readonly DurationFieldType[] fieldTypes;

        //the only one purpose of this member is to improve perfomance
        //of searching the index of the field for particular period type
        //otherwise, it would be looping through fieldTypes array
        private readonly int[] indices;

        private PeriodType(string name, params DurationFieldType[] fieldTypes)
            : this(name, fieldTypes, BuildIndices(fieldTypes))
        {
        }

        /// <summary>
        /// Builds a set of indexes from the given field types, to avoid having to specify them
        /// in all the static variable initializers. This doesn't have to be particularly fast,
        /// as it's not called often.
        /// </summary>
        private static int[] BuildIndices(DurationFieldType[] fieldTypes)
        {
            int[] ret = new[] { -1, -1, -1, -1, -1, -1, -1, -1 };
            for (int i = 0; i < fieldTypes.Length; i++)
            {
                switch (fieldTypes[i])
                {
                    case DurationFieldType.Days: ret[(int)Index.Day] = i; break;
                    case DurationFieldType.Hours: ret[(int)Index.Hour] = i; break;
                    case DurationFieldType.Milliseconds: ret[(int)Index.Millisecond] = i; break;
                    case DurationFieldType.Minutes: ret[(int)Index.Minute] = i; break;
                    case DurationFieldType.Months: ret[(int)Index.Month] = i; break;
                    case DurationFieldType.Seconds: ret[(int)Index.Second] = i; break;
                    case DurationFieldType.Weeks: ret[(int)Index.Week] = i; break;
                    case DurationFieldType.Years: ret[(int)Index.Year] = i; break;
                    default: throw new ArgumentException("Invalid duration field type for period type");
                }
            }
            return ret;
        }

        private PeriodType(string name, DurationFieldType[] fieldTypes, int[] indices)
        {
            this.name = name;
            this.fieldTypes = fieldTypes;
            this.indices = indices;
        }

        /// <summary>
        /// Gets the name of the period type.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the number of fields in the period type.
        /// </summary>
        public int Size
        {
            get { return fieldTypes.Length; }
        }

        /// <summary>
        /// Creates an array of the field types that this period type supports.
        /// This array is a copy; changes to it will not affect this period type.
        /// </summary>
        /// <remarks>
        /// The fields are returned largest to smallest, for example Hours, Minutes, Seconds.
        /// </remarks>
        /// <returns>The fields supported in an array that may be altered, largest to smallest</returns>
        public DurationFieldType[] GetFieldTypes()
        {
            return (DurationFieldType[])fieldTypes.Clone();
        }

        /// <summary>
        /// Gets the field type by index.
        /// </summary>
        /// <param name="index">the index to retrieve</param>
        /// <returns>the field type</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the index is invalid</exception>
        public DurationFieldType GetFieldType(int index)
        {
            return fieldTypes[index];
        }

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

        internal int GetRealIndex(PeriodType.Index index)
        {
            return indices[(int)index];
        }

        #region Masking

        private PeriodType WithFieldRemoved(PeriodType.Index index, string suffix)
        {
            int fieldIndex = GetRealIndex(index);
            if (fieldIndex == -1)
            {
                return this;
            }

            //construct new field types
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

            //construct new indices
            int[] newIndices = new int[8];
            int indicesIndex = (int)index;
            for (int i = 0; i < indicesIndex; i++)
            {
                newIndices[i] = indices[i];
            }
            newIndices[indicesIndex] = -1;
            for (int i = indicesIndex + 1; i < 8; i++)
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

        #endregion

        #region Equality

        public bool Equals(PeriodType other)
        {
            if (Object.ReferenceEquals(this, other))
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

        public override bool Equals(object obj)
        {
            return Equals(obj as PeriodType);
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            for (int i = 0; i < fieldTypes.Length; i++)
            {
                hash = HashCodeHelper.Hash(hash, fieldTypes[i]);
            }

            return hash;
        }

        public static bool operator ==(PeriodType left, PeriodType right)
        {
            return Object.Equals(left, right);
        }

        public static bool operator !=(PeriodType left, PeriodType right)
        {
            return !Object.Equals(left, right);
        }

        #endregion

        internal void UpdateIndexedField(int[] values, PeriodType.Index index, int newValue, bool add)
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

        public override string ToString()
        {
            return "PeriodType[" + Name + "]";
        }
    }
}