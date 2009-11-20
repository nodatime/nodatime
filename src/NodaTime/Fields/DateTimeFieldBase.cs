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

namespace NodaTime.Fields
{
    public abstract class DateTimeFieldBase : IDateTimeField
    {
        internal readonly static int DateTimeFieldTypeLength = Enum.GetValues(typeof(DateTimeFieldType)).Length;

        private readonly DateTimeFieldType fieldType;
        
        protected DateTimeFieldBase(DateTimeFieldType fieldType)
        {
            if (!IsTypeValid(fieldType))
            {
                throw new ArgumentOutOfRangeException("fieldType");
            }
            this.fieldType = fieldType;
        }

        public DateTimeFieldType FieldType { get { return fieldType; } }

        /// <summary>
        /// Defaults to fields being supported
        /// </summary>
        public virtual bool IsSupported { get { return true; } }

        public virtual int GetValue(LocalInstant localInstant)
        {
            return checked((int) GetInt64Value(localInstant));
        }

        public virtual LocalInstant Add(LocalInstant localInstant, int value)
        {
            return DurationField.Add(localInstant, value);
        }

        public virtual LocalInstant Add(LocalInstant localInstant, long value)
        {
            return DurationField.Add(localInstant, value);
        }

        public int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return DurationField.GetDifference(minuendInstant, subtrahendInstant);
        }

        public long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return DurationField.GetInt64Difference(minuendInstant, subtrahendInstant);
        }

        /// <summary>
        /// Defaults to non-leap.
        /// </summary>
        public virtual bool IsLeap(LocalInstant localInstant)
        {
            return false;
        }

        /// <summary>
        /// Defaults to 0.
        /// </summary>
        public virtual int GetLeapAmount(LocalInstant localInstant)
        {
            return 0;
        }

        /// <summary>
        /// Defaults to null, i.e. no leap duration field.
        /// </summary>
        public virtual DurationField LeapDurationField { get { return null; } }

        /// <summary>
        /// Defaults to the absolute maximum for the field.
        /// </summary>
        public virtual long GetMaximumValue(LocalInstant localInstant)
        {
            return GetMaximumValue();
        }

        /// <summary>
        /// Defaults to the absolute minimum for the field.
        /// </summary>
        public virtual long GetMinimumValue(LocalInstant localInstant)
        {
            return GetMinimumValue();
        }

        public abstract LocalInstant RoundFloor(LocalInstant localInstant);

        public virtual LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            LocalInstant newInstant = RoundFloor(localInstant);
            if (newInstant.Ticks != localInstant.Ticks)
            {
                newInstant = Add(newInstant, 1);
            }
            return newInstant;
        }

        public virtual LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            LocalInstant floor = RoundFloor(localInstant);
            LocalInstant ceiling = RoundCeiling(localInstant);

            long diffFromFloor = localInstant.Ticks - floor.Ticks;
            long diffToCeiling = ceiling.Ticks - localInstant.Ticks;

             // Closer to the floor, or halfway - round floor
            return diffFromFloor <= diffToCeiling ? floor : ceiling;
        }
        
        public virtual LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            LocalInstant floor = RoundFloor(localInstant);
            LocalInstant ceiling = RoundCeiling(localInstant);

            long diffFromFloor = localInstant.Ticks - floor.Ticks;
            long diffToCeiling = ceiling.Ticks - localInstant.Ticks;

             // Closer to the ceiling, or halfway - round ceiling
            return diffToCeiling <= diffFromFloor ? ceiling : floor;
        }

        public virtual LocalInstant RoundHalfEven(LocalInstant localInstant)
        {
            LocalInstant floor = RoundFloor(localInstant);
            LocalInstant ceiling = RoundCeiling(localInstant);

            long diffFromFloor = localInstant.Ticks - floor.Ticks;
            long diffToCeiling = ceiling.Ticks - localInstant.Ticks;

            // Closer to the floor - round floor
            if (diffFromFloor < diffToCeiling)
            {
                return floor;
            }
            // Closer to the ceiling - round ceiling
            else if (diffToCeiling < diffFromFloor)
            {
                return ceiling;
            }
            else
            {
                // Round to the instant that makes this field even. If both values
                // make this field even (unlikely), favor the ceiling.
                return (GetInt64Value(ceiling) & 1) == 0 ? ceiling : floor;
            }
        }

        public virtual Duration Remainder(LocalInstant localInstant)
        {
            // TODO: Improve this in terms of readability when we've got operators on local instants
            return new Duration(localInstant.Ticks - RoundFloor(localInstant).Ticks);
        }

        public abstract bool IsLenient { get; }
        public abstract long GetInt64Value(LocalInstant localInstant);
        public abstract LocalInstant SetValue(LocalInstant localInstant, long value);
        public abstract DurationField DurationField { get; }
        public abstract DurationField RangeDurationField { get; }
        public abstract long GetMaximumValue();
        public abstract long GetMinimumValue();


        public static bool IsTypeValid(DateTimeFieldType type)
        {
            return type >= 0 && type <= DateTimeFieldType.TickOfDay;
        }
    }
}
