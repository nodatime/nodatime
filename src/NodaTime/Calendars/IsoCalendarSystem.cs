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

namespace NodaTime.Calendars
{
    /// <summary>
    /// Original name: ISOChronology
    /// </summary>
    public sealed class IsoCalendarSystem : AssembledCalendarSystem
    {
        /// <summary>
        /// Returns the IsoChronology with the system default time zone.
        /// </summary>
        public static IsoCalendarSystem SystemDefault
        {
            get { throw new NotImplementedException(); }
        }

        public static IsoCalendarSystem GetInstance(IDateTimeZone dateTimeZone)
        {
            throw new NotImplementedException();
        }

        // TODO: Not sure what we'll really need here yet.
        private IsoCalendarSystem(ICalendarSystem baseSystem) : base(baseSystem, baseSystem.Fields)
        {
        }
    }
}
