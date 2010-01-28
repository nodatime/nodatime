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
using NodaTime.Calendars;
using NodaTime.Fields;

namespace NodaTime.Partials
{
    /// <summary>
    /// Original name: Partial.
    /// </summary>
    public sealed class Partial : AbstractPartial
    {
        private readonly ICalendarSystem calendar;
        private readonly DateTimeFieldType[] types;
        private readonly int[] values;

        public Partial(DateTimeFieldType[] types, int[] values, ICalendarSystem calendar)
        {
            this.calendar = calendar;
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Length != types.Length)
            {
                throw new ArgumentException("Values array must be the same length as the types array");
            }
            if (types.Length == 0)
            {
                this.types = types;
                this.values = values;
                return;
            }
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == null)
                {
                    throw new ArgumentException("Types array must not contain null: index " + i);
                }
            }


            this.types = (DateTimeFieldType[])types.Clone();
            calendar.Validate(this, values);
            values = (int[])values.Clone();
        }

        /// <summary>
        /// Gets the number of fields in this partial.
        /// </summary>
        public override int Size
        {
            get { return types.Length; }
        }

        /// <summary>
        /// Gets the chronology of the partial which is never null.
        /// </summary>
        public override ICalendarSystem Calendar
        {
            get { return calendar; }
        }

        protected override DateTimeFieldBase GetField(int index, ICalendarSystem calendar)
        {
            throw new NotImplementedException();
        }

        public override int GetValue(int index)
        {
            throw new NotImplementedException();
        }
    }
}