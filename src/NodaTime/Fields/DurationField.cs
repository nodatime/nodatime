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

namespace NodaTime
{
    /// <summary>
    /// Original name: DurationField.
    /// Note: Can't easily copy the tests for this until we've got a real DurationField.
    /// Note: The fact that this is an abstract class and IDateTimeField is an interface is slightly irksome. Suggestions welcome.
    /// </summary>
    public abstract class DurationField
    {
        public abstract bool IsSupported { get; }

        public abstract bool IsPrecise { get; }

        public abstract long UnitTicks { get; }
        public abstract DurationFieldType FieldType { get; }

        public abstract int GetValue(Duration duration);

        public abstract long GetInt64Value(Duration duration);

        public abstract int GetValue(Duration duration, LocalInstant localInstant);

        public abstract long GetInt64Value(Duration duration, LocalInstant localInstant);

        public abstract Duration GetDuration(long value);

        public abstract Duration GetDuration(long value, LocalInstant localInstant);

        public abstract LocalInstant Add(LocalInstant localInstant, int value);

        public abstract LocalInstant Add(LocalInstant localInstant, long value);

        public LocalInstant Subtract(LocalInstant localInstant, int value)
        {
            if (value == int.MinValue)
            {
                return Subtract(localInstant, (long)value);
            }
            return Add(localInstant, -value);
        }

        public LocalInstant Subtract(LocalInstant instant, long value)
        {
            if (value == long.MinValue)
            {
                throw new ArithmeticException("Int64.MinValue cannot be negated");
            }
            return Add(instant, -value);
        }

        public abstract int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant);

        public abstract long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant);
    }
}
