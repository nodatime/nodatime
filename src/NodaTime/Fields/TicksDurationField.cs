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

namespace NodaTime.Fields
{
    /// <summary>
    /// Singleton duration field for a fixed duration of 1 tick.
    /// </summary>
    internal sealed class TicksDurationField : DurationField
    {
        private static readonly TicksDurationField instance = new TicksDurationField();

        public static TicksDurationField Instance { get { return instance; } }

        private TicksDurationField() : base(DurationFieldType.Ticks)
        {
        }

        internal override bool IsSupported { get { return true; } }

        internal override bool IsPrecise { get { return true; } }

        internal override long UnitTicks { get { return 1; } }

        internal override int GetValue(Duration duration)
        {
            return (int)duration.Ticks;
        }

        internal override long GetInt64Value(Duration duration)
        {
            return duration.Ticks;
        }

        internal override int GetValue(Duration duration, LocalInstant localInstant)
        {
            return (int)duration.Ticks;
        }

        internal override long GetInt64Value(Duration duration, LocalInstant localInstant)
        {
            return duration.Ticks;
        }

        internal override Duration GetDuration(long value)
        {
            return new Duration(value);
        }

        internal override Duration GetDuration(long value, LocalInstant localInstant)
        {
            return new Duration(value);
        }

        internal override LocalInstant Add(LocalInstant localInstant, int value)
        {
            return new LocalInstant(localInstant.Ticks + value);
        }

        internal override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return new LocalInstant(localInstant.Ticks + value);
        }

        internal override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return (int)(minuendInstant.Ticks - subtrahendInstant.Ticks);
        }

        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return minuendInstant.Ticks - subtrahendInstant.Ticks;
        }
    }
}