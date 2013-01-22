namespace NodaTime.Fields
{
    /// <summary>
    /// Base class for variable length period fields - i.e. where the duration of a single value depends on when the value occurs.
    /// (For example, months and years.) Derived classes need only override Add and GetInt64Difference.
    /// </summary>
    internal abstract class VaryiableLengthPeriodField : PeriodField
    {
        internal VaryiableLengthPeriodField(PeriodFieldType fieldType, long averageUnitTicks)
            : base(fieldType, averageUnitTicks, false, true)
        {
        }

        internal override long GetInt64Value(Duration duration, LocalInstant localInstant)
        {
            return GetInt64Difference(localInstant + duration, localInstant);
        }

        internal override Duration GetDuration(long value, LocalInstant localInstant)
        {
            return Add(localInstant, value) - localInstant;
        }
    }
}
