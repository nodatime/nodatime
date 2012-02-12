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

namespace NodaTime.Fields
{
    /// <summary>
    /// Period field for years in a basic calendar system with a fixed number of months of varying lengths.
    /// </summary>
    internal sealed class BasicYearPeriodField : VaryiableLengthPeriodField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicYearPeriodField(BasicCalendarSystem calendarSystem) : base(PeriodFieldType.Years, calendarSystem.AverageTicksPerYear)
        {
            this.calendarSystem = calendarSystem;
        }

        internal override LocalInstant Add(LocalInstant localInstant, int value)
        {
           return calendarSystem.Fields.Year.SetValue(localInstant, calendarSystem.GetYear(localInstant) + value);
        }

        internal override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return Add(localInstant, (int) value);
        }

        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return minuendInstant < subtrahendInstant
                ? -calendarSystem.GetYearDifference(subtrahendInstant, minuendInstant)
                : calendarSystem.GetYearDifference(minuendInstant, subtrahendInstant);
        }
    }
}
