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
        public static IsoCalendarSystem Utc
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the IsoChronology with the system default time zone.
        /// </summary>
        public static IsoCalendarSystem SystemDefault
        {
            get { throw new NotImplementedException(); }
        }

        public override IDateTimeField Era
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField CenturyOfEra
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField YearOfCentury
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField YearOfEra
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField Year
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField DayOfMonth
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField MonthOfYear
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField Weekyear
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField WeekOfWeekyear
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField DayOfWeek
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField DayOfYear
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField HalfdayOfDay
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField HourOfHalfday
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField ClockhourOfDay
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField ClockhourOfHalfday
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField HourOfDay
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField MinuteOfHour
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField MinuteOfDay
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField SecondOfMinute
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField SecondOfDay
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField MillisecondsOfSecond
        {
            get { throw new NotImplementedException(); }
        }
        public override IDateTimeField MillisecondsOfDay
        {
            get { throw new NotImplementedException(); }
        }

        public static IsoCalendarSystem GetInstance(IDateTimeZone dateTimeZone)
        {
            throw new NotImplementedException();
        }
    }
}
