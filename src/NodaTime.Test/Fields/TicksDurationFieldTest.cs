using System;
using NodaTime.Fields;
using NUnit.Framework;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class TicksDurationFieldTest
    {
        [Test]
        public void FieldType()
        {
            Assert.AreEqual(DurationFieldType.Ticks, TicksDurationField.Instance.FieldType);
        }

        public void IsSupported()
        {
            Assert.IsTrue(TicksDurationField.Instance.IsSupported);
        }

        public void IsPrecise()
        {
            Assert.IsTrue(TicksDurationField.Instance.IsPrecise);
        }

        [Test]
        public void UnitTicks()
        {
            Assert.AreEqual(1, TicksDurationField.Instance.UnitTicks);
        }

        [Test]
        public void GetInt64Value()
        {
            Assert.AreEqual(0L, TicksDurationField.Instance.GetInt64Value(new Duration(0L)));
            Assert.AreEqual(1234L, TicksDurationField.Instance.GetInt64Value(new Duration(1234L)));
            Assert.AreEqual(-1234L, TicksDurationField.Instance.GetInt64Value(new Duration(-1234L)));
            Assert.AreEqual(int.MaxValue + 1L, TicksDurationField.Instance.GetInt64Value(new Duration(int.MaxValue + 1L)));
        }

        [Test]
        public void GetInt32Value()
        {
            Assert.AreEqual(0, TicksDurationField.Instance.GetValue(new Duration(0L)));
            Assert.AreEqual(1234, TicksDurationField.Instance.GetValue(new Duration(1234L)));
            Assert.AreEqual(-1234, TicksDurationField.Instance.GetValue(new Duration(-1234L)));
        }

        [Test]
        public void GetInt64Value_WithLocalInstant()
        {
            LocalInstant when = new LocalInstant(56789L);
            Assert.AreEqual(0L, TicksDurationField.Instance.GetInt64Value(new Duration(0L), when));
            Assert.AreEqual(1234L, TicksDurationField.Instance.GetInt64Value(new Duration(1234L), when));
            Assert.AreEqual(-1234L, TicksDurationField.Instance.GetInt64Value(new Duration(-1234L), when));
            Assert.AreEqual(int.MaxValue + 1L, TicksDurationField.Instance.GetInt64Value(new Duration(int.MaxValue + 1L), when));
        }

        [Test]
        public void GetInt32Value_Overflows()
        {
            Assert.Throws<OverflowException>(() => TicksDurationField.Instance.GetValue(new Duration(int.MaxValue + 1L)));
        }

        [Test]
        public void GetTicks()
        {
            Assert.AreEqual(0L, TicksDurationField.Instance.Get);
        }


        //-----------------------------------------------------------------------
        public void test_getMillis_int()
        {
            assertEquals(0, MillisDurationField.INSTANCE.getMillis(0));
            assertEquals(1234, MillisDurationField.INSTANCE.getMillis(1234));
            assertEquals(-1234, MillisDurationField.INSTANCE.getMillis(-1234));
        }

        public void test_getMillis_long()
        {
            assertEquals(0L, MillisDurationField.INSTANCE.getMillis(0L));
            assertEquals(1234L, MillisDurationField.INSTANCE.getMillis(1234L));
            assertEquals(-1234L, MillisDurationField.INSTANCE.getMillis(-1234L));
        }

        public void test_getMillis_int_long()
        {
            assertEquals(0, MillisDurationField.INSTANCE.getMillis(0, 567L));
            assertEquals(1234, MillisDurationField.INSTANCE.getMillis(1234, 567L));
            assertEquals(-1234, MillisDurationField.INSTANCE.getMillis(-1234, 567L));
        }

        public void test_getMillis_long_long()
        {
            assertEquals(0L, MillisDurationField.INSTANCE.getMillis(0L, 567L));
            assertEquals(1234L, MillisDurationField.INSTANCE.getMillis(1234L, 567L));
            assertEquals(-1234L, MillisDurationField.INSTANCE.getMillis(-1234L, 567L));
        }

        //-----------------------------------------------------------------------
        public void test_add_long_int()
        {
            assertEquals(567L, MillisDurationField.INSTANCE.add(567L, 0));
            assertEquals(567L + 1234L, MillisDurationField.INSTANCE.add(567L, 1234));
            assertEquals(567L - 1234L, MillisDurationField.INSTANCE.add(567L, -1234));
            try
            {
                MillisDurationField.INSTANCE.add(Long.MAX_VALUE, 1);
                fail();
            }
            catch (ArithmeticException ex) { }
        }

        public void test_add_long_long()
        {
            assertEquals(567L, MillisDurationField.INSTANCE.add(567L, 0L));
            assertEquals(567L + 1234L, MillisDurationField.INSTANCE.add(567L, 1234L));
            assertEquals(567L - 1234L, MillisDurationField.INSTANCE.add(567L, -1234L));
            try
            {
                MillisDurationField.INSTANCE.add(Long.MAX_VALUE, 1L);
                fail();
            }
            catch (ArithmeticException ex) { }
        }

        //-----------------------------------------------------------------------
        public void test_getDifference_long_int()
        {
            assertEquals(567, MillisDurationField.INSTANCE.getDifference(567L, 0L));
            assertEquals(567 - 1234, MillisDurationField.INSTANCE.getDifference(567L, 1234L));
            assertEquals(567 + 1234, MillisDurationField.INSTANCE.getDifference(567L, -1234L));
            try
            {
                MillisDurationField.INSTANCE.getDifference(Long.MAX_VALUE, 1L);
                fail();
            }
            catch (ArithmeticException ex) { }
        }

        public void test_getDifferenceAsLong_long_long()
        {
            assertEquals(567L, MillisDurationField.INSTANCE.getDifferenceAsLong(567L, 0L));
            assertEquals(567L - 1234L, MillisDurationField.INSTANCE.getDifferenceAsLong(567L, 1234L));
            assertEquals(567L + 1234L, MillisDurationField.INSTANCE.getDifferenceAsLong(567L, -1234L));
            try
            {
                MillisDurationField.INSTANCE.getDifferenceAsLong(Long.MAX_VALUE, -1L);
                fail();
            }
            catch (ArithmeticException ex) { }
        }

    }
}
