using NodaTime.Fields;
using NUnit.Framework;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class ZeroIsMaxDateTimeFieldTest
    {
        private readonly ZeroIsMaxDateTimeField field = new ZeroIsMaxDateTimeField
            (new PreciseDateTimeField(DateTimeFieldType.HourOfDay,
                 PreciseDurationField.Hours, PreciseDurationField.Days),
             DateTimeFieldType.ClockHourOfDay);

        [Test]
        public void GetMinimum_AlwaysReturns1()
        {
            Assert.AreEqual(1, field.GetMinimumValue());
            Assert.AreEqual(1, field.GetMinimumValue(new LocalInstant(0)));
        }

        [Test]
        public void GetMaximum_AlwaysReturnsWrappedMaximumPlus1()
        {
            Assert.AreEqual(24, field.GetMaximumValue());
            Assert.AreEqual(24, field.GetMaximumValue(new LocalInstant(0)));
        }

        [Test]
        public void GetValue_ForZero_ReturnsMaximum()
        {
            Assert.AreEqual(24, field.GetValue(new LocalInstant(0)));
        }

        [Test]
        public void GetValue_ForNonZero_ReturnsOriginalValue()
        {
            Assert.AreEqual(1, field.GetValue(new LocalInstant(NodaConstants.TicksPerHour)));
        }

        [Test]
        public void TestSetValue_WithMaximumUsesZero()
        {
            Assert.AreEqual(0, field.SetValue(new LocalInstant(NodaConstants.TicksPerHour), 24).Ticks);
        }

        [Test]
        public void TestSetValue_WithNonMaximumPassesValueThrough()
        {
            Assert.AreEqual(NodaConstants.TicksPerHour * 2,
                field.SetValue(new LocalInstant(0), 2).Ticks);
        }
    }
}
