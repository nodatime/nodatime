using System;

using NodaTime.Converters;
using NodaTime.Format;

namespace NodaTime
{
    public class MockZeroNullIntegerConverter : IInstantConverter
    {
        public static readonly IInstantConverter Instance = new MockZeroNullIntegerConverter();

        public long GetInstantMilliseconds(object obj, IChronology chrono)
        {
            return 0;
        }

        public long GetInstantMilliseconds(object obj, IChronology chrono, DateTimeFormatter parser)
        {
            return 0;
        }

        public IChronology GetChronology(object obj, DateTimeZone zone)
        {
            return null;
        }

        public IChronology GetChronology(object obj, IChronology chrono)
        {
            return null;
        }

        public Type GetSupportedType()
        {
            return typeof(int);
        }
    }
}