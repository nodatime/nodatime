using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    public static class TestHelper
    {
        private static void ValidateInput(object value, object equalValue, object unEqualValue, string unEqualName)
        {
            Assert.NotNull(value, "value cannot be null in TestObjectEquals() method");
            Assert.NotNull(equalValue, "equalValue cannot be null in TestObjectEquals() method");
            Assert.NotNull(unEqualValue, unEqualName + " cannot be null in TestObjectEquals() method");
            Assert.AreNotSame(value, equalValue, "value and equalValue MUST be different objects");
            Assert.AreNotSame(value, unEqualValue, unEqualName + " and value MUST be different objects");
        }

        public static void TestObjectEquals(object value, object equalValue, object unEqualValue)
        {
            ValidateInput(value, equalValue, unEqualValue, "unEqualValue");
            Assert.False(value.Equals(null), "value.Equals(null)");
            Assert.True(value.Equals(value), "value.Equals(value)");
            Assert.True(value.Equals(equalValue), "value.Equals(equalValue)");
            Assert.True(equalValue.Equals(value), "equalValue.Equals(value)");
            Assert.False(value.Equals(unEqualValue), "value.Equals(unEqualValue)");
            Assert.AreEqual(value.GetHashCode(), value.GetHashCode(), "GetHashCode() twice for same object");
            Assert.AreEqual(value.GetHashCode(), equalValue.GetHashCode(), "GetHashCode() for two different but equal objects");
        }

        public static void TestEqualsClass<T>(T value, T equalValue, T unEqualValue)
            where T : class, IEquatable<T>
        {
            TestObjectEquals(value, equalValue, unEqualValue);
            Assert.False(value.Equals((T)null), "value.Equals<T>(null)");
            Assert.True(value.Equals(value), "value.Equals<T>(value)");
            Assert.True(value.Equals(equalValue), "value.Equals<T>(equalValue)");
            Assert.True(equalValue.Equals(value), "equalValue.Equals<T>(value)");
            Assert.False(value.Equals(unEqualValue), "value.Equals<T>(unEqualValue)");
        }

        public static void TestEqualsStruct<T>(T value, T equalValue, T unEqualValue)
            where T : struct, IEquatable<T>
        {
            TestObjectEquals(value, equalValue, unEqualValue);
            Assert.True(value.Equals(value), "value.Equals<T>(value)");
            Assert.True(value.Equals(equalValue), "value.Equals<T>(equalValue)");
            Assert.True(equalValue.Equals(value), "equalValue.Equals<T>(value)");
            Assert.False(value.Equals(unEqualValue), "value.Equals<T>(unEqualValue)");
        }

        public static void TestCompareToClass<T>(T value, T equalValue, T greaterValue)
            where T : class, IComparable<T>
        {
            ValidateInput(value, equalValue, greaterValue, "greaterValue");
            Assert.True(value.CompareTo((T)null) > 0, "value.CompareTo<T>(null)");
            Assert.True(value.CompareTo(value) == 0, "value.CompareTo<T>(value)");
            Assert.True(value.CompareTo(equalValue) == 0, "value.CompareTo<T>(equalValue)");
            Assert.True(equalValue.CompareTo(value) == 0, "equalValue.CompareTo<T>(value)");
            Assert.True(value.CompareTo(greaterValue) < 0, "value.CompareTo<T>(greaterValue)");
            Assert.True(greaterValue.CompareTo(value) > 0, "greaterValue.CompareTo<T>(value)");
        }

        public static void TestCompareToStruct<T>(T value, T equalValue, T greaterValue)
            where T : struct, IComparable<T>
        {
            ValidateInput(value, equalValue, greaterValue, "greaterValue");
            Assert.True(value.CompareTo(value) == 0, "value.CompareTo<T>(value)");
            Assert.True(value.CompareTo(equalValue) == 0, "value.CompareTo<T>(equalValue)");
            Assert.True(equalValue.CompareTo(value) == 0, "equalValue.CompareTo<T>(value)");
            Assert.True(value.CompareTo(greaterValue) < 0, "value.CompareTo<T>(greaterValue)");
            Assert.True(greaterValue.CompareTo(value) > 0, "greaterValue.CompareTo<T>(value)");
        }

        public static void TestOperatorEquality<T>(T value, T equalValue, T unEqualValue)
        {
            ValidateInput(value, equalValue, unEqualValue, "unEqualValue");
            Type type = typeof(T);
            string name = type.Name;
            var equality = type.GetMethod("op_Equality", new Type[] { type, type });
            if (equality != null)
            {
                if (!type.IsValueType)
                {
                    Assert.False((bool)equality.Invoke(null, new object[] { null, null }), "null == null");
                    Assert.False((bool)equality.Invoke(null, new object[] { value, null }), "value == null");
                    Assert.False((bool)equality.Invoke(null, new object[] { null, value }), "null == value");
                }
                Assert.True((bool)equality.Invoke(null, new object[] { value, value }), "value == value");
                Assert.True((bool)equality.Invoke(null, new object[] { value, equalValue }), "value == equalValue");
                Assert.True((bool)equality.Invoke(null, new object[] { equalValue, value }), "equalValue == value");
                Assert.False((bool)equality.Invoke(null, new object[] { value, unEqualValue }), "value == unEqualValue");
            }
            var inequality = type.GetMethod("op_Inequality", new Type[] { type, type });
            if (inequality != null)
            {
                if (!type.IsValueType)
                {
                    Assert.True((bool)inequality.Invoke(null, new object[] { null, null }), "null != null");
                    Assert.True((bool)inequality.Invoke(null, new object[] { value, null }), "value != null");
                    Assert.True((bool)inequality.Invoke(null, new object[] { null, value }), "null != value");
                }
                Assert.False((bool)inequality.Invoke(null, new object[] { value, value }), "value != value");
                Assert.False((bool)inequality.Invoke(null, new object[] { value, equalValue }), "value != equalValue");
                Assert.False((bool)inequality.Invoke(null, new object[] { equalValue, value }), "equalValue != value");
                Assert.True((bool)inequality.Invoke(null, new object[] { value, unEqualValue }), "value != unEqualValue");
            }
        }

        public static void TestOperatorComparison<T>(T value, T equalValue, T greaterValue)
        {
            ValidateInput(value, equalValue, greaterValue, "greaterValue");
            Type type = typeof(T);
            var greaterThan = type.GetMethod("op_GreaterThan", new Type[] { type, type });
            if (greaterThan != null)
            {
                if (!type.IsValueType)
                {
                    Assert.True((bool)greaterThan.Invoke(null, new object[] { value, null }), "value > null");
                    Assert.False((bool)greaterThan.Invoke(null, new object[] { null, value }), "null > value");
                }
                Assert.False((bool)greaterThan.Invoke(null, new object[] { value, value }), "value > value");
                Assert.False((bool)greaterThan.Invoke(null, new object[] { value, equalValue }), "value > equalValue");
                Assert.False((bool)greaterThan.Invoke(null, new object[] { equalValue, value }), "equalValue > value");
                Assert.False((bool)greaterThan.Invoke(null, new object[] { value, greaterValue }), "value > greaterValue");
                Assert.True((bool)greaterThan.Invoke(null, new object[] { greaterValue, value }), "greaterValue > value");
            }
            var lessThan = type.GetMethod("op_LessThan", new Type[] { type, type });
            if (lessThan != null)
            {
                if (!type.IsValueType)
                {
                    Assert.False((bool)greaterThan.Invoke(null, new object[] { value, null }), "value < null");
                    Assert.True((bool)greaterThan.Invoke(null, new object[] { null, value }), "null < value");
                }
                Assert.False((bool)lessThan.Invoke(null, new object[] { value, value }), "value < value");
                Assert.False((bool)lessThan.Invoke(null, new object[] { value, equalValue }), "value < equalValue");
                Assert.False((bool)lessThan.Invoke(null, new object[] { equalValue, value }), "equalValue < value");
                Assert.True((bool)lessThan.Invoke(null, new object[] { value, greaterValue }), "value < greaterValue");
                Assert.False((bool)lessThan.Invoke(null, new object[] { greaterValue, value }), "greaterValue < value");
            }
        }

        public static void TestOperatorComparisonEquality<T>(T value, T equalValue, T greaterValue)
        {
            TestOperatorEquality(value, equalValue, greaterValue);
            TestOperatorComparison(value, equalValue, greaterValue);
            Type type = typeof(T);
            var greaterThanOrEqual = type.GetMethod("op_GreaterThanOrEqual", new Type[] { type, type });
            if (greaterThanOrEqual != null)
            {
                if (!type.IsValueType)
                {
                    Assert.True((bool)greaterThanOrEqual.Invoke(null, new object[] { value, null }), "value >= null");
                    Assert.False((bool)greaterThanOrEqual.Invoke(null, new object[] { null, value }), "null >= value");
                }
                Assert.True((bool)greaterThanOrEqual.Invoke(null, new object[] { value, value }), "value >= value");
                Assert.True((bool)greaterThanOrEqual.Invoke(null, new object[] { value, equalValue }), "value >= equalValue");
                Assert.True((bool)greaterThanOrEqual.Invoke(null, new object[] { equalValue, value }), "equalValue >= value");
                Assert.False((bool)greaterThanOrEqual.Invoke(null, new object[] { value, greaterValue }), "value >= greaterValue");
                Assert.True((bool)greaterThanOrEqual.Invoke(null, new object[] { greaterValue, value }), "greaterValue >= value");
            }
            var lessThanOrEqual = type.GetMethod("op_LessThanOrEqual", new Type[] { type, type });
            if (lessThanOrEqual != null)
            {
                if (!type.IsValueType)
                {
                    Assert.False((bool)greaterThanOrEqual.Invoke(null, new object[] { value, null }), "value <= null");
                    Assert.True((bool)greaterThanOrEqual.Invoke(null, new object[] { null, value }), "null <= value");
                }
                Assert.True((bool)lessThanOrEqual.Invoke(null, new object[] { value, value }), "value <= value");
                Assert.True((bool)lessThanOrEqual.Invoke(null, new object[] { value, equalValue }), "value <= equalValue");
                Assert.True((bool)lessThanOrEqual.Invoke(null, new object[] { equalValue, value }), "equalValue <= value");
                Assert.True((bool)lessThanOrEqual.Invoke(null, new object[] { value, greaterValue }), "value <= greaterValue");
                Assert.False((bool)lessThanOrEqual.Invoke(null, new object[] { greaterValue, value }), "greaterValue <= value");
            }
        }
    }
}
