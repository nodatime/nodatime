#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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

namespace NodaTime.Periods
{
    /// <summary>
    /// Controls a period implementation by specifying which duration fields are to be used.
    /// </summary>
    /// <remarks>
    /// TODO: Consider where we should really have ticks. It's not entirely clear.
    /// Defined values are:
    /// 
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
    /// </remarks>
    public sealed class PeriodType
    {
        #region Static Properties

        /// <summary>
        /// Gets a type that defines just the years field.
        /// </summary>
        public static PeriodType Years
        {
            get
            {
                return new PeriodType("Years"
                    ,
                    new DurationFieldType[] {DurationFieldType.Years}
                    );
            }
        }

        /// <summary>
        /// Gets a type that defines just the months field.
        /// </summary>
        public static PeriodType Months
        {
            get
            {
                return new PeriodType("Months"
                    ,
                    new DurationFieldType[] { DurationFieldType.Months }
                    );
            }
        }

        /// <summary>
        /// Gets a type that defines just the weeks field.
        /// </summary>
        public static PeriodType Weeks
        {
            get
            {
                return new PeriodType("Weeks"
                    ,
                    new DurationFieldType[] { DurationFieldType.Weeks }
                    );
            }
        }

        /// <summary>
        /// Gets a type that defines just the days field.
        /// </summary>
        public static PeriodType Days
        {
            get
            {
                return new PeriodType("Days"
                    ,
                    new DurationFieldType[] { DurationFieldType.Days }
                    );
            }
        }

        /// <summary>
        /// Gets a type that defines just the hours field.
        /// </summary>
        public static PeriodType Hours
        {
            get
            {
                return new PeriodType("Hours"
                    ,
                    new DurationFieldType[] { DurationFieldType.Hours }
                    );
            }
        }

        /// <summary>
        /// Gets a type that defines just the minutes field.
        /// </summary>
        public static PeriodType Minutes
        {
            get
            {
                return new PeriodType("Minutes"
                    ,
                    new DurationFieldType[] { DurationFieldType.Minutes }
                    );
            }
        }

        /// <summary>
        /// Gets a type that defines just the seconds field.
        /// </summary>
        public static PeriodType Seconds
        {
            get
            {
                return new PeriodType("Seconds"
                    ,
                    new DurationFieldType[] { DurationFieldType.Seconds }
                    );
            }
        }

        /// <summary>
        /// Gets a type that defines just the milliseconds field.
        /// </summary>
        public static PeriodType Milliseconds
        {
            get
            {
                return new PeriodType("Milliseconds"
                    ,
                    new DurationFieldType[] { DurationFieldType.Milliseconds }
                    );
            }
        }

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
        public static PeriodType Standart
        {
            get
            {
                return new PeriodType("Standard"
                    ,
                    new DurationFieldType[] {
                    DurationFieldType.Years, DurationFieldType.Months,
                    DurationFieldType.Weeks, DurationFieldType.Days,
                    DurationFieldType.Hours, DurationFieldType.Minutes,
                    DurationFieldType.Seconds, DurationFieldType.Milliseconds}
                    );                
            }
        }

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
        public static PeriodType YearMonthDayTime
        {
            get
            {
                return new PeriodType("YearMonthDayTime"
                    ,
                    new DurationFieldType[] {
                    DurationFieldType.Years, DurationFieldType.Months,
                    DurationFieldType.Days,
                    DurationFieldType.Hours, DurationFieldType.Minutes,
                    DurationFieldType.Seconds, DurationFieldType.Milliseconds}
                    );
            }
        }

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
        public static PeriodType YearMonthDay
        {
            get
            {
                return new PeriodType("YearMonthDay"
                    ,
                    new DurationFieldType[] {
                    DurationFieldType.Years, DurationFieldType.Months,
                    DurationFieldType.Days}
                    );
            }
        }

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
        public static PeriodType YearWeekDayTime
        {
            get
            {
                return new PeriodType("YearWeekDayTime"
                    ,
                    new DurationFieldType[] {
                    DurationFieldType.Years, DurationFieldType.Weeks, DurationFieldType.Days,
                    DurationFieldType.Hours, DurationFieldType.Minutes,
                    DurationFieldType.Seconds, DurationFieldType.Milliseconds}
                    );
            }
        }

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
        public static PeriodType YearWeekDay
        {
            get
            {
                return new PeriodType("YearWeekDay"
                    ,
                    new DurationFieldType[] {
                    DurationFieldType.Years, DurationFieldType.Weeks, DurationFieldType.Days}
                    );
            }
        }

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
        public static PeriodType YearDayTime
        {
            get
            {
                return new PeriodType("YearDayTime"
                    ,
                    new DurationFieldType[] {
                    DurationFieldType.Years, DurationFieldType.Days,
                    DurationFieldType.Hours, DurationFieldType.Minutes,
                    DurationFieldType.Seconds, DurationFieldType.Milliseconds}
                    );
            }
        }

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
        public static PeriodType YearDay
        {
            get
            {
                return new PeriodType("YearDay"
                    ,
                    new DurationFieldType[] {
                    DurationFieldType.Years, DurationFieldType.Days}
                    );
            }
        }

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
        public static PeriodType DayTime
        {
            get
            {
                return new PeriodType("DayTime"
                    ,
                    new DurationFieldType[] {
                    DurationFieldType.Days,
                    DurationFieldType.Hours, DurationFieldType.Minutes,
                    DurationFieldType.Seconds, DurationFieldType.Milliseconds}
                    );
            }
        }

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
        public static PeriodType Time
        {
            get
            {
                return new PeriodType("Time"
                    ,
                    new DurationFieldType[] {
                    DurationFieldType.Hours, DurationFieldType.Minutes,
                    DurationFieldType.Seconds, DurationFieldType.Milliseconds}
                    );
            }
        }

        #endregion

        private readonly string name;
        private readonly DurationFieldType[] fieldTypes;

        private PeriodType(string name, DurationFieldType[] fieldTypes)
        {
            this.name = name;
            this.fieldTypes = fieldTypes;
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
        /// <param name="fieldType">hte type to check, may be null which returns -1</param>
        /// <returns>The index or -1 if not supported</returns>
        public int IndexOf(DurationFieldType fieldType)
        {
            for (int i = 0, isize = Size; i < isize; i++)
            {
                if (fieldTypes[i] == fieldType)
                {
                    return i;
                }
            }
            return -1;
        }

        public override string ToString()
        {
            return "PeriodType[" + Name + "]";
        }
    }
}