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
    public abstract class DurationFieldBase : DurationField
    {
        private readonly DurationFieldType fieldType;

        protected DurationFieldBase(DurationFieldType fieldType)
        {
            if (!IsTypeValid(fieldType))
            {
                throw new ArgumentOutOfRangeException("fieldType");
            }
            this.fieldType = fieldType;
        }
        
        public override DurationFieldType FieldType { get { return fieldType; } }

        /// <summary>
        /// Fields derived from this class are always supported.
        /// </summary>
        public override bool IsSupported { get { return true; } }

        public override int GetValue(Duration duration)
        {
 	        return (int) GetInt64Value(duration);
        }

        public override long GetInt64Value(Duration duration)
        {
 	        return duration.Ticks / UnitTicks;
        }

        public override int GetValue(Duration duration, LocalInstant localInstant)
        {
 	        return (int) GetInt64Value(duration, localInstant);
        }

        public override Duration GetDuration(long value)
        {
            return new Duration(value * UnitTicks);
        }

        public override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return (int) GetInt64Difference(minuendInstant, subtrahendInstant);
        }

        public static bool IsTypeValid(DurationFieldType type)
        {
            return type >= 0 && type <= DurationFieldType.Milliseconds;
        }
    }
}
