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
using NodaTime.Calendars;

namespace NodaTime.Partials
{
    /// <summary>
    /// BasePartial is an abstract implementation of ReadablePartial that stores
    /// data in array and <code>Chronology</code> fields.
    /// </summary>
    public abstract class PartialBase : AbstractPartial
    {
        private readonly ICalendarSystem calendar;
        private readonly int[] values;

        protected PartialBase() { }

        /// <summary>
        /// Initializes a partial with specified time field values and chronology.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="chronology"></param>
        protected PartialBase(int[] values, ICalendarSystem calendar)
        {
            this.calendar = calendar;
            calendar.Validate(this, values);
            this.values = values;
        }

        /// <summary>
        /// Gets the calendar system of the partial which is never null.
        /// <para>
        /// The <see cref="ICalendarSystem"/> is the calculation engine behind the partial and
        /// provides conversion and validation of the fields in a particular calendar system.
        /// </para>
        /// </summary>
        public override ICalendarSystem Calendar
        {
            get { return calendar; }
        }

        /// <summary>
        /// Gets the value of the field at the specifed index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The value</returns>
        public override int GetValue(int index)
        {
            return values[index];
        }
    }
}