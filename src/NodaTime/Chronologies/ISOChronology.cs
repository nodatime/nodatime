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

namespace NodaTime.Chronologies
{
    /// <summary>
    /// Original name: ISOChronology
    /// </summary>
    public sealed class IsoChronology : AssembledChronology
    {
        public static IsoChronology Utc
        {
            get { throw new NotImplementedException(); }
        }

        public static IsoChronology GetInstance()
        {
            throw new NotImplementedException();
        }

        public DateTimeField Era
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField CenturyOfEra
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField YearOfCentury
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField YearOfEra
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField Year
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField DayOfMonth
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField MonthOfYear
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField Weekyear
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField WeekOfWeekyear
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField DayOfWeek
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField DayOfYear
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField HalfdayOfDay
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField HourOfHalfday
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField ClockhourOfDay
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField ClockhourOfHalfday
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField HourOfDay
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField MinuteOfHour
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField MinuteOfDay
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField SecondOfMinute
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField SecondOfDay
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField MillisecondsOfSecond
        {
            get { throw new NotImplementedException(); }
        }
        public DateTimeField MillisecondsOfDay
        {
            get { throw new NotImplementedException(); }
        }

        public static IsoChronology GetInstance(DateTimeZone dateTimeZone)
        {
            throw new NotImplementedException();
        }
    }
}
