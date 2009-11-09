using System;

namespace NodaTime.Chronologies
{
    /// <summary>
    /// Original name: GregorianChronology
    /// 
    /// There are a lot more classes in the hierarchy between GregorianChronology and IChronology.
    /// </summary>
    public class GregorianChronology : IChronology
    {
        public static IChronology GetInstance(DateTimeZone dateTimeZone)
        {
            throw new NotImplementedException();
        }
    }
}