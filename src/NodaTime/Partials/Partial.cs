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
using NodaTime.Periods;
using NodaTime.Utility;

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
            if (calendar == null)
            {
                throw new ArgumentNullException("calendar");
            }
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
            this.values = (int[])values.Clone();
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

        protected override IDateTimeField GetField(int index, ICalendarSystem calendar)
        {
            if (index < 0 || index >= types.Length)
                throw new ArgumentOutOfRangeException("index");
            return types[index].GetField(calendar);
        }

        public override int GetValue(int index)
        {
            if (index < 0 || index >= values.Length)
                throw new ArgumentOutOfRangeException("index");
            return values[index];
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            var asPartial = obj as IPartial;
            return asPartial != null && Equals(asPartial);
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, calendar);
            foreach (DateTimeFieldType type in types)
                hash = HashCodeHelper.Hash(hash, type);
            foreach (int value in values)
                hash = HashCodeHelper.Hash(hash, value);
            return hash;
        }

        public override bool Equals(IPartial other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (other.Calendar != Calendar)
                return false;
            if (Size != other.Size)
                return false;
            for (int i = 0; i < Size; i++)
            {
                if (types[i] != other.GetFieldType(i))
                    return false;
                if (values[i] != other.GetValue(i))
                    return false;
            }
            return true;
        }

        public override int CompareTo(IPartial other)
        {
            if (other == null)
                return 1;
            if (this == other)
                return 0;
            const string differentFieldMessage = "Cannot compare partials with different fields";
            if (Size != other.Size)
            {
                throw new InvalidOperationException(differentFieldMessage);
            }
            for (int i = 0; i < Size; i++)
            {
                if (types[i] != other.GetFieldType(i))
                    throw new InvalidOperationException(differentFieldMessage);
            }
            for (int i = 0; i < Size; i++)
            {
                if (values[i] > other.GetValue(i))
                    return 1;
                if (values[i] < other.GetValue(i))
                    return -1;
            }
            return 0;
        }

        public Partial WithCalendar(ICalendarSystem newCalendar)
        {
            if (calendar == newCalendar)
                return this;
            return new Partial(types, values, newCalendar);
        }

        public Partial With(DateTimeFieldType fieldType, int value)
        {
            if (fieldType == null)
                throw new ArgumentNullException("fieldType");
            int index = IndexOf(fieldType);
            if (index == -1)
            {
                var newTypes = new DateTimeFieldType[Size + 1];
                var newValues = new int[newTypes.Length];
                long fieldDurationTicks = fieldType.GetField(calendar).DurationField.UnitTicks;
                long fieldRangeTicks = fieldType.GetField(calendar).RangeDurationField.UnitTicks;
                int i;
                for(i = 0; i < types.Length; i++)
                {
                    long iDurationTicks = types[i].GetField(calendar).DurationField.UnitTicks;
                    if (fieldDurationTicks > iDurationTicks)
                        break;
                    if (fieldDurationTicks == iDurationTicks)
                    {
                        long iRangeTicks = types[i].GetField(calendar).RangeDurationField.UnitTicks;
                        if (fieldRangeTicks > iRangeTicks)
                            break;
                    }
                }
                Array.Copy(types, newTypes, i);
                Array.Copy(values, newValues, i);
                newTypes[i] = fieldType;
                newValues[i] = value;
                Array.Copy(types, i, newTypes, i + 1, types.Length - i);
                Array.Copy(values, i, newValues, i + 1, values.Length - i);
                return new Partial(newTypes, newValues, calendar);
            }
            else
            {
                return this.WithField(fieldType, value);
            }
        }

        public Partial Without(DateTimeFieldType fieldType)
        {
            if (fieldType == null)
                throw new ArgumentNullException("fieldType");
            int index = IndexOf(fieldType);
            if (index == -1)
                return this;

            var newTypes = new DateTimeFieldType[Size - 1];
            var newValues = new int[newTypes.Length];

            Array.Copy(types, newTypes, index);
            Array.Copy(values, newValues, index);
            Array.Copy(types, index + 1, newTypes, index, newTypes.Length - index);
            Array.Copy(values, index + 1, newValues, index, newValues.Length - index);
            return new Partial(newTypes, newValues, calendar);
        }

        public Partial WithField(DateTimeFieldType fieldType, int value)
        {
            if (fieldType == null)
                throw new ArgumentNullException("fieldType");
            int index = IndexOf(fieldType);
            if (index == -1)
                throw new ArgumentOutOfRangeException("fieldType");
            if (Get(fieldType) == value)
                return this;

            if (Get(fieldType) == value)
                return this;
            var newValues = (int[])values.Clone();
            newValues[index] = value;
            GetField(index).SetValue(this, index, newValues, value);
            return new Partial(types, newValues, calendar);
        }

        public Partial WithFieldAdded(DurationFieldType fieldType, int amount)
        {
            if (amount == 0)
                return this;
            int index = IndexOf(fieldType);
            int[] newValues = GetValues();
            newValues = GetField(index).Add(this, index, newValues, amount);
            return new Partial(types, newValues, calendar);
        }

        public Partial WithFieldAddWrapped(DurationFieldType fieldType, int amount)
        {
            if (amount == 0)
                return this;
            int index = IndexOf(fieldType);
            int[] newValues = GetValues();
            newValues = GetField(index).AddWrapPartial(this, index, newValues, amount);
            return new Partial(types, newValues, calendar);
        }

        public Partial WithPeriodAdded(IPeriod period, int factor)
        {
            if (period == null)
                throw new ArgumentNullException("period");
            if (factor == 0)
                return this;
            int[] newValues = GetValues();
            for (int i = 0; i < period.Size; i++)
            {
                var fieldType = period.GetFieldType(i);
                int index = IndexOf(fieldType);
                if(index != -1)
                {
                    newValues = GetField(index).Add(this, index, newValues, period.GetValue(i) * factor);
                }
            }
            return new Partial(types, newValues, calendar);
        }

        public Partial Plus(IPeriod period)
        {
            return WithPeriodAdded(period, 1);
        }

        public Partial Minus(IPeriod period)
        {
            return WithPeriodAdded(period, -1);
        }
    }
}