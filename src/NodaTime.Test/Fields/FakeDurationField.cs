
using NodaTime.Fields;

namespace NodaTime.Test.Fields
{
    /// <summary>
    /// Class allowing simple construction of fields for testing constructors of other fields.
    /// </summary>
    internal class FakeDurationField : DurationFieldBase
    {
        private readonly long unitTicks;
        private readonly bool precise;

        internal FakeDurationField(long unitTicks, bool precise) : base(DurationFieldType.Seconds)
        {
            this.unitTicks = unitTicks;
            this.precise = precise;
        }

        public override bool IsPrecise { get { return precise; } }

        public override long UnitTicks { get { return unitTicks; } }

        public override long GetInt64Value(Duration duration, LocalInstant localInstant)
        {
            return 0;
        }

        public override Duration GetDuration(long value, LocalInstant localInstant)
        {
            return new Duration(0);
        }

        public override LocalInstant Add(LocalInstant localInstant, int value)
        {
            return new LocalInstant();
        }

        public override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return new LocalInstant();
        }

        public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return 0;
        }
    }
}
