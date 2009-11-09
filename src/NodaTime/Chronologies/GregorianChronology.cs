using System;

namespace NodaTime.Chronologies
{
    /// <summary>
    /// Original name: GregorianChronology
    /// 
    /// There are a lot more classes in the hierarchy between GregorianChronology and IChronology.
    /// </summary>
    public class GregorianChronology : BasicGJChronology
    {
        public static IChronology GetInstance(DateTimeZone dateTimeZone)
        {
            throw new NotImplementedException();
        }

        public override DateTimeField Era
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField CenturyOfEra
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField YearOfCentury
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField YearOfEra
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField Year
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField DayOfMonth
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField MonthOfYear
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField Weekyear
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField WeekOfWeekyear
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField DayOfWeek
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField DayOfYear
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField HalfdayOfDay
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField HourOfHalfday
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField ClockhourOfDay
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField ClockhourOfHalfday
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField HourOfDay
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField MinuteOfHour
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField MinuteOfDay
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField SecondOfMinute
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField SecondOfDay
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField MillisecondsOfSecond
        {
            get { throw new NotImplementedException(); }
        }
        public override DateTimeField MillisecondsOfDay
        {
            get { throw new NotImplementedException(); }
        }
    }
}