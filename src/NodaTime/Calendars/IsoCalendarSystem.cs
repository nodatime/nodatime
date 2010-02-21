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
using NodaTime.Fields;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Implements a calendar system that follows the rules of the ISO8601 standard,
    /// which is compatible with Gregorian for all modern dates. This class is a singleton.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When ISO does not define a field, but it can be determined (such as AM/PM) it is included.
    /// </para>
    /// <para>
    /// With the exception of century related fields, ISOChronology is exactly the
    /// same as <see cref="GregorianCalendarSystem" />. In this chronology, centuries and year
    /// of century are zero based. For all years, the century is determined by
    /// dropping the last two digits of the year, ignoring sign. The year of century
    /// is the value of the last two year digits.
    /// </para>
    /// </remarks>
    public sealed class IsoCalendarSystem : AssembledCalendarSystem
    {
        public static readonly IsoCalendarSystem Instance = new IsoCalendarSystem(GregorianCalendarSystem.Default);

        private IsoCalendarSystem(ICalendarSystem baseSystem) : base(baseSystem)
        {
        }

        protected override void AssembleFields(FieldSet.Builder fields)
        {
            if (fields == null)
            {
                throw new ArgumentNullException("fields");
            }
            // Use zero based century and year of century.
            DividedDateTimeField centuryOfEra = new DividedDateTimeField
                (IsoYearOfEraDateTimeField.Instance, DateTimeFieldType.CenturyOfEra, 100);
            fields.CenturyOfEra = centuryOfEra;
            fields.YearOfCentury = new RemainderDateTimeField(centuryOfEra, DateTimeFieldType.YearOfCentury);
            fields.WeekYearOfCentury = new RemainderDateTimeField(centuryOfEra, DateTimeFieldType.WeekYearOfCentury);
            fields.Centuries = centuryOfEra.DurationField;
        }

        // TODO: Try overriding the GetLocalInstant methods to micro-optimise them (they will be called for almost every ZonedDateTime/LocalDateTime)
    }
}
