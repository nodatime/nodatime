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
    internal class UnsupportedDurationField : DurationField
    {
        private static readonly UnsupportedDurationField[] Cache = Array.ConvertAll
            ((DurationFieldType[]) Enum.GetValues(typeof(DurationFieldType)),
             type => new UnsupportedDurationField(type));

        // Convenience fields
        public static readonly UnsupportedDurationField Eras = Cache[(int) DurationFieldType.Eras];
        public static readonly UnsupportedDurationField Centuries = Cache[(int) DurationFieldType.Centuries];
        public static readonly UnsupportedDurationField WeekYears = Cache[(int) DurationFieldType.WeekYears];
        public static readonly UnsupportedDurationField Years = Cache[(int) DurationFieldType.Years];
        public static readonly UnsupportedDurationField Months = Cache[(int) DurationFieldType.Months];
        public static readonly UnsupportedDurationField Weeks = Cache[(int) DurationFieldType.Weeks];
        public static readonly UnsupportedDurationField Days = Cache[(int) DurationFieldType.Days];
        public static readonly UnsupportedDurationField HalfDays = Cache[(int) DurationFieldType.HalfDays];
        public static readonly UnsupportedDurationField Hours = Cache[(int) DurationFieldType.Hours];
        public static readonly UnsupportedDurationField Minutes = Cache[(int) DurationFieldType.Minutes];
        public static readonly UnsupportedDurationField Seconds = Cache[(int) DurationFieldType.Seconds];
        public static readonly UnsupportedDurationField Milliseconds = Cache[(int) DurationFieldType.Milliseconds];
        public static readonly UnsupportedDurationField Ticks = Cache[(int) DurationFieldType.Ticks];

        private readonly DurationFieldType fieldType;

        private UnsupportedDurationField(DurationFieldType fieldType)
        {
            this.fieldType = fieldType;
        }

        public static UnsupportedDurationField ForFieldType(DurationFieldType fieldType)
        {
            if (!DurationFieldBase.IsTypeValid(fieldType))
            {
                throw new ArgumentOutOfRangeException("fieldType");
            }
            return Cache[(int) fieldType];
        }

        public override DurationFieldType FieldType { get { return FieldType; } }

        public override bool IsSupported { get { return false; } }

        public override bool IsPrecise { get { return true; } }

        public override long UnitTicks { get { return 0; } }

        public override int GetValue(Duration duration)
        {
            throw new NotSupportedException();
        }

        public override long GetInt64Value(Duration duration)
        {
            throw new NotSupportedException();
        }

        public override int GetValue(Duration duration, LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public override long GetInt64Value(Duration duration, LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public override Duration GetDuration(long value)
        {
            throw new NotSupportedException();
        }

        public override Duration GetDuration(long value, LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public override LocalInstant Add(LocalInstant localInstant, int value)
        {
            throw new NotSupportedException();
        }

        public override LocalInstant Add(LocalInstant localInstant, long value)
        {
            throw new NotSupportedException();
        }

        public override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            throw new NotSupportedException();
        }

        public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            throw new NotSupportedException();
        }
    }
}
