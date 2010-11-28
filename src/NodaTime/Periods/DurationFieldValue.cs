using NodaTime.Fields;

namespace NodaTime.Periods
{
    // TODO: Implement IEquatable[T] etc? I'm not currently sure there's much point.

    /// <summary>
    /// A simple combination of a <see cref="DurationFieldType" /> and a 64-bit integer value.
    /// These are used when representing periods.
    /// </summary>
    public struct DurationFieldValue
    {
        private readonly DurationFieldType fieldType;
        public DurationFieldType FieldType { get { return fieldType; } }

        private readonly long value;
        public long Value { get { return value; } }

        public DurationFieldValue(DurationFieldType fieldType, long value)
        {
            this.fieldType = fieldType;
            this.value = value;
        }
    }
}
