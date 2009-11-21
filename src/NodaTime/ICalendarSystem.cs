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

using NodaTime.Fields;

namespace NodaTime
{
    /// <summary>
    /// A system of defining time in terms of years, months, days and so forth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The most commonly use calendar system in Noda Time is <see cref="IsoCalendarSystem" />,
    /// which is used as a default value in many overloaded methods and constructors.
    /// </para>
    /// <para>
    /// A calendar system has no specific time zone; a <see cref="Chronology" /> represents the union
    /// of a time zone with a calendar system.
    /// </para>
    /// <para>
    /// The members of this class are unlikely to be used directly by most users of the API.
    /// </para>
    public interface ICalendarSystem
    {
        LocalInstant GetLocalInstant(int year, int month, int day, int hour, 
                                     int minute, int second, int millisecond, int tickWithinMillisecond);

        DateTimeFieldSet DateTimeFields { get; }
    }
}
