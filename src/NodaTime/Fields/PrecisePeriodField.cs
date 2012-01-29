#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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
    /// Period field class representing a field with a fixed unit length.
    /// </summary>
    internal sealed class PrecisePeriodField : PeriodField
    {
        internal static readonly PrecisePeriodField Milliseconds = new PrecisePeriodField(PeriodFieldType.Milliseconds, NodaConstants.TicksPerMillisecond);
        internal static readonly PrecisePeriodField Seconds = new PrecisePeriodField(PeriodFieldType.Seconds, NodaConstants.TicksPerSecond);
        internal static readonly PrecisePeriodField Minutes = new PrecisePeriodField(PeriodFieldType.Minutes, NodaConstants.TicksPerMinute);
        internal static readonly PrecisePeriodField Hours = new PrecisePeriodField(PeriodFieldType.Hours, NodaConstants.TicksPerHour);
        internal static readonly PrecisePeriodField HalfDays = new PrecisePeriodField(PeriodFieldType.HalfDays, NodaConstants.TicksPerStandardDay / 2);
        internal static readonly PrecisePeriodField Days = new PrecisePeriodField(PeriodFieldType.Days, NodaConstants.TicksPerStandardDay);
        internal static readonly PrecisePeriodField Weeks = new PrecisePeriodField(PeriodFieldType.Weeks, NodaConstants.TicksPerStandardWeek);

        internal PrecisePeriodField(PeriodFieldType type, long unitTicks) : base(type, unitTicks, true, true)
        {
        }

        internal override long GetInt64Value(Duration duration, LocalInstant localInstant)
        {
            return duration.Ticks / UnitTicks;
        }

        internal override Duration GetDuration(long value, LocalInstant localInstant)
        {
            return new Duration(value * UnitTicks);
        }

        internal override LocalInstant Add(LocalInstant localInstant, int value)
        {
            return new LocalInstant(localInstant.Ticks + value * UnitTicks);
        }

        internal override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return new LocalInstant(localInstant.Ticks + value * UnitTicks);
        }

        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return (minuendInstant.Ticks - subtrahendInstant.Ticks) / UnitTicks;
        }
    }
}