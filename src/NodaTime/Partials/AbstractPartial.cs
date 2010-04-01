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
using NodaTime.Calendars;
using NodaTime.Fields;

namespace NodaTime.Partials
{
    /// <summary>
    /// AbstractPartial provides a standard base implementation of most methods
    /// in the IPartial interface.
    /// <para>
    /// Calculations on are performed using a <see cref="ICalendarSystem"/>.
    /// </para>
    /// </summary>
    public abstract class AbstractPartial: IPartial
    {
        /// <summary>
        /// Gets the calendar system of the partial which is never null.
        /// <para>
        /// The <see cref="ICalendarSystem"/> is the calculation engine behind the partial and
        /// provides conversion and validation of the fields in a particular calendar system.
        /// </para>
        /// </summary>
        public abstract ICalendarSystem Calendar { get; }

        /// <summary>
        /// Gets the number of fields that this partial supports.
        /// </summary>
        public abstract int Size { get; }

        /// <summary>
        /// Gets an array of the field types that this partial supports.
        /// The fields are returned largest to smallest, for example Hour, Minute, Second.
        /// </summary>
        /// <returns>The fields supported in an array that may be altered, largest to smallest</returns>
        public DateTimeFieldType[] GetFieldTypes()
        {
            DateTimeFieldType[] result = new DateTimeFieldType[Size];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = GetFieldType(i);
            }
            return result;
        }

        /// <summary>
        /// Gets the index of the specified field, or -1 if the field is unsupported.
        /// </summary>
        /// <param name="type">The type to check, may be null which returns -1</param>
        /// <returns>The index of the field, -1 if unsupported</returns>
        public int IndexOf(DateTimeFieldType type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            for (int i = 0; i < Size; i++)
            {
                if (GetFieldType(i) == type)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the index of the specified field, throwing an exception if the
        /// field is unsupported.
        /// </summary>
        /// <param name="type">The type to check, not null</param>
        /// <returns>The index of the field</returns>
        protected int IndexOfSupported(DateTimeFieldType type)
        {
            int index = IndexOf(type);
            if (index == -1)
            {
                throw new ArgumentOutOfRangeException("Field '" + type + "' is not supported");
            }
            return index;
        }

        /// <summary>
        /// Checks whether the field specified is supported by this partial.
        /// </summary>
        /// <param name="fieldType">The type to check, may be null which returns false</param>
        /// <returns>True if the field is supported. false otherwise</returns>
        public bool IsSupported(DateTimeFieldType fieldType)
        {
            return (IndexOf(fieldType) != -1);
        }

        /// <summary>
        /// Get the value of one of the fields of a datetime.
        /// <para>
        /// The field specified must be one of those that is supported by the partial.
        /// </para>
        /// </summary>
        /// <param name="fieldType">A DateTimeFieldType instance that is supported by this partial</param>
        /// <returns>Value of that field</returns>
        public int Get(DateTimeFieldType fieldType)
        {
            return GetValue(IndexOfSupported(fieldType));
        }

        /// <summary>
        /// Gets the field type at the specified index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The field type</returns>
        public DateTimeFieldType GetFieldType(int index)
        {
            return GetField(index, Calendar).FieldType;
        }

        /// <summary>
        /// Gets the field at the specifed index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The field</returns>
        public IDateTimeField GetField(int index)
        {
            return GetField(index, Calendar);
        }

        /// <summary>
        /// Gets an array of the fields that this partial supports.
        /// The fields are returned largest to smallest, for example Hour, Minute, Second.
        /// </summary>
        /// <returns>The fields supported in an array that may be altered, largest to smallest</returns>
        public IDateTimeField[] GetFields()
        {
            IDateTimeField[] result = new IDateTimeField[Size];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = GetField(i);
            }
            return result;
        }

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="index">The index to retrieve</param>
        /// <returns>The value of the field at the specified index</returns>
        public abstract int GetValue(int index);

        /// <summary>
        /// Gets an array of the value of each of the fields that this partial supports.
        /// <para>
        /// The fields are returned largest to smallest, for example Hour, Minute, Second.
        /// Each value corresponds to the same array index as <code>GetFields()</code>
        /// </para>
        /// </summary>
        /// <returns>The current values of each field in an array that may be altered, largest to smallest</returns>
        public int[] GetValues()
        {
            int[] result = new int[Size];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = GetValue(i);
            }
            return result;
        }

        /// <summary>
        /// Gets the field for a specific index in the calendar system specified.
        /// <para>
        /// This method must not use any instance variables.
        /// </para>
        /// </summary>
        /// <param name="index">The index to retrieve</param>
        /// <param name="calendar">The chronology to use</param>
        /// <returns>The field</returns>
        protected abstract IDateTimeField GetField(int index, ICalendarSystem calendar);

        /// <summary>
        /// Gets the index of the first fields to have the specified duration,
        /// or -1 if the field is unsupported.
        /// </summary>
        /// <param name="type">The type to check, may be null which returns -1</param>
        /// <returns>The index of the field, -1 if unsupported</returns>
        protected int IndexOf(DurationFieldType type)
        {
            int size = Size;
            for (int i = 0; i < size; i++)
            {
                if (GetFieldType(i).DurationFieldType == type)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the index of the first fields to have the specified duration,
        /// throwing an exception if the field is unsupported.
        /// </summary>
        /// <param name="type">The type to check, not null</param>
        /// <returns>Index of the field</returns>
        protected int IndexOfSupported(DurationFieldType type)
        {
            int index = IndexOf(type);
            if (index == -1)
            {
                throw new ArgumentException("Field '" + type + "' is not supported");
            }
            return index;
        }

        /// <summary>
        /// Resolves this partial against another complete instant to create a new
        /// full instant. The combination is performed using the chronology of the
        /// specified instant.
        /// <para>
        /// For example, if this partial represents a time, then the result of this
        /// method will be the datetime from the specified base instant plus the
        /// time from this partial.
        /// </para>
        /// </summary>
        /// <param name="baseInstant">The instant that provides the missing fields, null means now</param>
        /// <returns></returns>
        public ZonedDateTime ToDateTime(Instant baseInstant)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public abstract bool Equals(IPartial other);

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// The return value has the following meanings:
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <description>Meaning</description>
        /// </listheader>
        /// <item>
        /// <term>&lt; 0</term>
        /// <description>This object is less than the <paramref name="other"/> parameter.</description>
        /// </item>
        /// <item>
        /// <term>0</term>
        /// <description>This object is equal to <paramref name="other"/>.</description>
        /// </item>
        /// <item>
        /// <term>&gt; 0</term>
        /// <description>This object is greater than <paramref name="other"/>.</description>
        /// </item>
        /// </list>
        /// </returns>
        public abstract int CompareTo(IPartial other);
    }
}