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

using NodaTime.Calendars;
using NodaTime.Fields;

namespace NodaTime
{
    /// <summary>
    /// Defines a partial time that does not support every datetime field, and is
    /// thus a local time.
    /// <para>
    /// An <code>IPartial</code> supports a subset of those fields on the chronology.
    /// It cannot be compared to a <code>Instant</code>, as it does not fully
    /// specify an instant in time. The time it does specify is a local time, and does
    /// not include a time zone.
    /// </para>
    /// <para>
    /// An <code>IPartial</code> can be converted to a <code>Instant</code>
    /// using the <code>ToDateTime</code> method. This works by providing a full base
    /// instant that can be used to 'fill in the gaps' and specify a time zone.
    /// </para>
    /// </summary>
    public interface IPartial
    {
        /// <summary>
        /// Gets the calendar system of the partial which is never null.
        /// <para>
        /// The <see cref="ICalendarSystem"/> is the calculation engine behind the partial and
        /// provides conversion and validation of the fields in a particular calendar system.
        /// </para>
        /// </summary>
        ICalendarSystem Calendar { get; }

        /// <summary>
        /// Gets the number of fields that this partial supports.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Checks whether the field type specified is supported by this partial.
        /// </summary>
        /// <param name="field">The field to check, may be null which returns false</param>
        /// <returns>True if the field is supported, false otherwise</returns>
        bool IsSupported(DateTimeFieldType fieldType);

        /// <summary>
        /// Gets the value of one of the fields.
        /// <para>
        /// The field type specified must be one of those that is supported by the partial.
        /// </para>
        /// </summary>
        /// <param name="fieldType">A DateTimeFieldType instance that is supported by this partial</param>
        /// <returns>The value of that field</returns>
        int Get(DateTimeFieldType fieldType);

        /// <summary>
        /// Gets the field type at the specified index.
        /// </summary>
        /// <param name="index">The index to retrieve</param>
        /// <returns>The field type at the specified index</returns>
        DateTimeFieldType GetFieldType(int index);

        /// <summary>
        /// Gets the field at the specified index.
        /// </summary>
        /// <param name="index">The index to retrieve</param>
        /// <returns>The field at the specified index</returns>
        DateTimeFieldBase GetField(int index);

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="index">The index to retrieve</param>
        /// <returns>The value of the field at the specified index</returns>
        int GetValue(int index);

        /// <summary>
        /// Converts this partial to a full datetime by resolving it against another
        /// datetime.
        /// <para>
        /// This method takes the specified datetime and sets the fields from this
        /// instant on top. The chronology from the base instant is used.
        /// </para>
        /// <para>
        /// For example, if this partial represents a time, then the result of this
        /// method will be the datetime from the specified base instant plus the
        /// time from this partial.
        /// </para>
        /// </summary>
        /// <param name="baseInstant">The instant that provides the missing fields, null means now</param>
        /// <returns>The combined datetime</returns>
        ZonedDateTime ToDateTime(Instant baseInstant);
    }
}
