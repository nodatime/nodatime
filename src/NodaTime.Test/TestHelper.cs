#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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
using NUnit.Framework;

namespace NodaTime.Test
{
    /// <summary>
    ///   Provides methods to help run tests for some of the system interfaces and object support.
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        ///   Tests the <see cref="IComparable.CompareTo" /> method for reference objects.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="greaterValue">The value greater than the base value..</param>
        public static void TestCompareToClass<T>(T value, T equalValue, T greaterValue) where T : class, IComparable<T>
        {
            ValidateInput(value, equalValue, greaterValue, "greaterValue");
            Assert.Greater(value.CompareTo(null), 0, "value.CompareTo<T>(null)");
            Assert.AreEqual(value.CompareTo(value), 0, "value.CompareTo<T>(value)");
            Assert.AreEqual(value.CompareTo(equalValue), 0, "value.CompareTo<T>(equalValue)");
            Assert.AreEqual(equalValue.CompareTo(value), 0, "equalValue.CompareTo<T>(value)");
            Assert.Less(value.CompareTo(greaterValue), 0, "value.CompareTo<T>(greaterValue)");
            Assert.Greater(greaterValue.CompareTo(value), 0, "greaterValue.CompareTo<T>(value)");
        }

        /// <summary>
        ///   Tests the <see cref="IComparable.CompareTo" /> method for value objects.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="greaterValue">The value greater than the base value..</param>
        public static void TestCompareToStruct<T>(T value, T equalValue, T greaterValue) where T : struct, IComparable<T>
        {
            ValidateInput(value, equalValue, greaterValue, "greaterValue");
            Assert.AreEqual(value.CompareTo(value), 0, "value.CompareTo<T>(value)");
            Assert.AreEqual(value.CompareTo(equalValue), 0, "value.CompareTo<T>(equalValue)");
            Assert.AreEqual(equalValue.CompareTo(value), 0, "equalValue.CompareTo<T>(value)");
            Assert.Less(value.CompareTo(greaterValue), 0, "value.CompareTo<T>(greaterValue)");
            Assert.Greater(greaterValue.CompareTo(value), 0, "greaterValue.CompareTo<T>(value)");
        }

        /// <summary>
        ///   Tests the <see cref="M:IEquatable.Equals" /> method for reference objects. Also tests the
        ///   object equals method.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="unEqualValue">The value not equal to the base value.</param>
        public static void TestEqualsClass<T>(T value, T equalValue, T unEqualValue) where T : class, IEquatable<T>
        {
            TestObjectEquals(value, equalValue, unEqualValue);
            Assert.False(value.Equals(null), "value.Equals<T>(null)");
            Assert.True(value.Equals(value), "value.Equals<T>(value)");
            Assert.True(value.Equals(equalValue), "value.Equals<T>(equalValue)");
            Assert.True(equalValue.Equals(value), "equalValue.Equals<T>(value)");
            Assert.False(value.Equals(unEqualValue), "value.Equals<T>(unEqualValue)");
        }

        /// <summary>
        ///   Tests the <see cref="M:IEquatable.Equals" /> method for value objects. Also tests the
        ///   object equals method.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="unEqualValue">The value not equal to the base value.</param>
        public static void TestEqualsStruct<T>(T value, T equalValue, T unEqualValue) where T : struct, IEquatable<T>
        {
            TestObjectEquals(value, equalValue, unEqualValue);
            Assert.True(value.Equals(value), "value.Equals<T>(value)");
            Assert.True(value.Equals(equalValue), "value.Equals<T>(equalValue)");
            Assert.True(equalValue.Equals(value), "equalValue.Equals<T>(value)");
            Assert.False(value.Equals(unEqualValue), "value.Equals<T>(unEqualValue)");
        }

        /// <summary>
        ///   Tests the <see cref="M:System.Object.Equals" /> method.
        /// </summary>
        /// <remarks>
        ///   It takes three, non-null values: a value,
        ///   a value that is equal to but not the same object, and a value that is not equal to the base value.
        /// </remarks>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="unEqualValue">The value not equal to the base value.</param>
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

        /// <summary>
        ///   Tests the less than (^lt;) and greater than (&gt;) operators if they exist on the object.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="greaterValue">The value greater than the base value..</param>
        public static void TestOperatorComparison<T>(T value, T equalValue, T greaterValue)
        {
            ValidateInput(value, equalValue, greaterValue, "greaterValue");
            Type type = typeof(T);
            var greaterThan = type.GetMethod("op_GreaterThan", new[] { type, type });
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
            var lessThan = type.GetMethod("op_LessThan", new[] { type, type });
            if (lessThan != null)
            {
                if (!type.IsValueType)
                {
                    Assert.False((bool)lessThan.Invoke(null, new object[] { value, null }), "value < null");
                    Assert.True((bool)lessThan.Invoke(null, new object[] { null, value }), "null < value");
                }
                Assert.False((bool)lessThan.Invoke(null, new object[] { value, value }), "value < value");
                Assert.False((bool)lessThan.Invoke(null, new object[] { value, equalValue }), "value < equalValue");
                Assert.False((bool)lessThan.Invoke(null, new object[] { equalValue, value }), "equalValue < value");
                Assert.True((bool)lessThan.Invoke(null, new object[] { value, greaterValue }), "value < greaterValue");
                Assert.False((bool)lessThan.Invoke(null, new object[] { greaterValue, value }), "greaterValue < value");
            }
        }

        /// <summary>
        ///   Tests the equality (==), inequality (!=), less than (^lt;), greater than (&gt;), less than or equals (&lt;=),
        ///   and freater than of equals (&gt;=) operators if they exist on the object.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="greaterValue">The value greater than the base value..</param>
        public static void TestOperatorComparisonEquality<T>(T value, T equalValue, T greaterValue)
        {
            TestOperatorEquality(value, equalValue, greaterValue);
            TestOperatorComparison(value, equalValue, greaterValue);
            Type type = typeof(T);
            var greaterThanOrEqual = type.GetMethod("op_GreaterThanOrEqual", new[] { type, type });
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
            var lessThanOrEqual = type.GetMethod("op_LessThanOrEqual", new[] { type, type });
            if (lessThanOrEqual != null)
            {
                if (!type.IsValueType)
                {
                    Assert.False((bool)lessThanOrEqual.Invoke(null, new object[] { value, null }), "value <= null");
                    Assert.True((bool)lessThanOrEqual.Invoke(null, new object[] { null, value }), "null <= value");
                }
                Assert.True((bool)lessThanOrEqual.Invoke(null, new object[] { value, value }), "value <= value");
                Assert.True((bool)lessThanOrEqual.Invoke(null, new object[] { value, equalValue }), "value <= equalValue");
                Assert.True((bool)lessThanOrEqual.Invoke(null, new object[] { equalValue, value }), "equalValue <= value");
                Assert.True((bool)lessThanOrEqual.Invoke(null, new object[] { value, greaterValue }), "value <= greaterValue");
                Assert.False((bool)lessThanOrEqual.Invoke(null, new object[] { greaterValue, value }), "greaterValue <= value");
            }
        }

        /// <summary>
        ///   Tests the equality and inequality operators (==, !=) if they exist on the object.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="unEqualValue">The value not equal to the base value.</param>
        public static void TestOperatorEquality<T>(T value, T equalValue, T unEqualValue)
        {
            ValidateInput(value, equalValue, unEqualValue, "unEqualValue");
            Type type = typeof(T);
            string name = type.Name;
            var equality = type.GetMethod("op_Equality", new[] { type, type });
            if (equality != null)
            {
                if (!type.IsValueType)
                {
                    Assert.True((bool)equality.Invoke(null, new object[] { null, null }), "null == null");
                    Assert.False((bool)equality.Invoke(null, new object[] { value, null }), "value == null");
                    Assert.False((bool)equality.Invoke(null, new object[] { null, value }), "null == value");
                }
                Assert.True((bool)equality.Invoke(null, new object[] { value, value }), "value == value");
                Assert.True((bool)equality.Invoke(null, new object[] { value, equalValue }), "value == equalValue");
                Assert.True((bool)equality.Invoke(null, new object[] { equalValue, value }), "equalValue == value");
                Assert.False((bool)equality.Invoke(null, new object[] { value, unEqualValue }), "value == unEqualValue");
            }
            var inequality = type.GetMethod("op_Inequality", new[] { type, type });
            if (inequality != null)
            {
                if (!type.IsValueType)
                {
                    Assert.False((bool)inequality.Invoke(null, new object[] { null, null }), "null != null");
                    Assert.True((bool)inequality.Invoke(null, new object[] { value, null }), "value != null");
                    Assert.True((bool)inequality.Invoke(null, new object[] { null, value }), "null != value");
                }
                Assert.False((bool)inequality.Invoke(null, new object[] { value, value }), "value != value");
                Assert.False((bool)inequality.Invoke(null, new object[] { value, equalValue }), "value != equalValue");
                Assert.False((bool)inequality.Invoke(null, new object[] { equalValue, value }), "equalValue != value");
                Assert.True((bool)inequality.Invoke(null, new object[] { value, unEqualValue }), "value != unEqualValue");
            }
        }

        /// <summary>
        /// Asserts that the given character is equal to the expected value.
        /// </summary>
        /// <remarks>
        /// The NUnit <c>Assert.AreEqual()</c> method compares characters as integers and displays the
        /// mismatches as integers. This displays the differences as characters making the interpretation
        /// of the error message much easier.
        /// </remarks>
        /// <param name="expected">The expected character value.</param>
        /// <param name="actual">The actual character value.</param>
        internal static void AssertCharEqual(char expected, char actual)
        {
            AssertCharEqual(expected, actual, null);
        }

        /// <summary>
        /// Asserts that the given character is equal to the expected value.
        /// </summary>
        /// <remarks>
        /// The NUnit <c>Assert.AreEqual()</c> method compares characters as integers and displays the
        /// mismatches as integers. This displays the differences as characters making the interpretation
        /// of the error message much easier.
        /// </remarks>
        /// <param name="expected">The expected character value.</param>
        /// <param name="actual">The actual character value.</param>
        /// <param name="message">The message to display.</param>
        internal static void AssertCharEqual(char expected, char actual, string message)
        {
            if (expected != actual)
            {
                if (message == null)
                {
                    message = "";
                }
                else
                {
                    message = message + "\n";
                }
                Assert.Fail("{0}  Expected: '{1}'\n  But was:  '{2}'", message, expected, actual);
            }
        }

        /// <summary>
        ///   Validates that the input parameters to the test methods are valid.
        /// </summary>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="unEqualValue">The value not equal to the base value.</param>
        /// <param name="unEqualName">GetName of the not equal value error messages..</param>
        private static void ValidateInput(object value, object equalValue, object unEqualValue, string unEqualName)
        {
            Assert.NotNull(value, "value cannot be null in TestObjectEquals() method");
            Assert.NotNull(equalValue, "equalValue cannot be null in TestObjectEquals() method");
            Assert.NotNull(unEqualValue, unEqualName + " cannot be null in TestObjectEquals() method");
            Assert.AreNotSame(value, equalValue, "value and equalValue MUST be different objects");
            Assert.AreNotSame(value, unEqualValue, unEqualName + " and value MUST be different objects");
        }
    }
}