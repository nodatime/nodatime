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

namespace NodaTime.Calendars
{
    /// <summary>
    /// The epoch to use when constructing an Islamic calendar.
    /// </summary>
    /// <remarks>
    /// The Islamic, or Hijri, calendar can either be constructed
    /// starting on July 15th 622CE (in the Julian calendar) or on the following day.
    /// The former is the "astronomical" or "Thursday" epoch; the latter is the "civil" or "Friday" epoch.
    /// </remarks>
    /// <seealso cref="CalendarSystem.GetIslamicCalendar"/>
    public enum IslamicEpoch
    {
        /// <summary>
        /// Epoch beginning on July 15th 622CE (Julian).
        /// </summary>
        Astronomical = 1,
        /// <summary>
        /// Epoch beginning on July 16th 622CE (Julian).
        /// </summary>
        Civil = 2
    }
}
