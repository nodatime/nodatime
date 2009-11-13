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

namespace NodaTime
{
    /// <summary>
    /// A date and time in a particular calendar system.
    /// </summary>
    /// <remarks><para>A LocalDateTime value does not
    /// represent an instant on the time line, mostly because it has no associated
    /// time zone: "November 12th 2009 7pm, ISO calendar" occurred at different instants
    /// for different people around the world.
    /// </para>
    /// <para>
    /// This type defaults to using the IsoCalendarSystem unless a different calendar
    /// system is specified.
    /// </para>
    /// </summary>
    public struct LocalDateTime
    {/*
        public LocalDateTime(int year, int month, int day,
                             int minute, int hour, int second,
                             ICalendarSystem calendar)
        {
        }

        public LocalDateTime(int year, int month, int day,
                             int minute, int hour, int second, int millisecond,
                             ICalendarSystem calendar)
        {
        }

        public LocalDateTime(int year, int month, int day,
                             int minute, int hour, int second)
        {
        }

        public LocalDateTime(int year, int month, int day,
                             int minute, int hour, int second, int millisecond)
        {
        }
        */
    }
}
