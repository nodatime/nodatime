// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NodaTime.Annotations;
#if !NETCORE
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace NodaTime.Test
{
    /// <summary>
    /// Provides methods to help run tests for some of the system interfaces and object support.
    /// </summary>
    public static class TestHelper
    {
        public static readonly bool IsRunningOnMono = Type.GetType("Mono.Runtime") != null;

        /// <summary>
        /// Does nothing other than let us prove method or constructor calls don't throw.
        /// </summary>
        internal static void Consume<T>(this T ignored)
        {
        }

        /// <summary>
        /// Asserts that calling the specified delegate with the specified value throws ArgumentException.
        /// </summary>
        internal static void AssertInvalid<TArg, TOut>(Func<TArg, TOut> func, TArg arg)
        {
            Assert.Throws<ArgumentException>(() => func(arg));
        }

        /// <summary>
        /// Asserts that calling the specified delegate with the specified values throws ArgumentException.
        /// </summary>
        internal static void AssertInvalid<TArg1, TArg2, TOut>(Func<TArg1, TArg2, TOut> func, TArg1 arg1, TArg2 arg2)
        {
            Assert.Throws<ArgumentException>(() => func(arg1, arg2));
        }

        /// <summary>
        /// Asserts that calling the specified delegate with the specified value throws ArgumentNullException.
        /// </summary>
        internal static void AssertArgumentNull<TArg, TOut>(Func<TArg, TOut> func, TArg arg)
        {
            Assert.Throws<ArgumentNullException>(() => func(arg));
        }

        /// <summary>
        /// Asserts that calling the specified delegate with the specified value throws ArgumentOutOfRangeException.
        /// </summary>
        internal static void AssertOutOfRange<TArg, TOut>(Func<TArg, TOut> func, TArg arg)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => func(arg));
        }

        /// <summary>
        /// Asserts that calling the specified delegate with the specified value doesn't throw an exception.
        /// </summary>
        internal static void AssertValid<TArg, TOut>(Func<TArg, TOut> func, TArg arg)
        {
            func(arg);
        }

        /// <summary>
        /// Asserts that calling the specified delegate with the specified values throws ArgumentOutOfRangeException.
        /// </summary>
        internal static void AssertOutOfRange<TArg1, TArg2, TOut>(Func<TArg1, TArg2, TOut> func, TArg1 arg1, TArg2 arg2)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => func(arg1, arg2));
        }

        /// <summary>
        /// Asserts that calling the specified delegate with the specified values throws ArgumentNullException.
        /// </summary>
        internal static void AssertArgumentNull<TArg1, TArg2, TOut>(Func<TArg1, TArg2, TOut> func, TArg1 arg1, TArg2 arg2)
        {
            Assert.Throws<ArgumentNullException>(() => func(arg1, arg2));
        }

        /// <summary>
        /// Asserts that calling the specified delegate with the specified values doesn't throw an exception.
        /// </summary>
        internal static void AssertValid<TArg1, TArg2, TOut>(Func<TArg1, TArg2, TOut> func, TArg1 arg1, TArg2 arg2)
        {
            func(arg1, arg2);
        }

        /// <summary>
        /// Asserts that calling the specified delegate with the specified values throws ArgumentOutOfRangeException.
        /// </summary>
        internal static void AssertOutOfRange<TArg1, TArg2, TArg3, TOut>(Func<TArg1, TArg2, TArg3, TOut> func, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => func(arg1, arg2, arg3));
        }

        /// <summary>
        /// Asserts that calling the specified delegate with the specified values throws ArgumentNullException.
        /// </summary>
        internal static void AssertArgumentNull<TArg1, TArg2, TArg3, TOut>(Func<TArg1, TArg2, TArg3, TOut> func, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            Assert.Throws<ArgumentNullException>(() => func(arg1, arg2, arg3));
        }

        /// <summary>
        /// Asserts that calling the specified delegate with the specified values doesn't throw an exception.
        /// </summary>
        internal static void AssertValid<TArg1, TArg2, TArg3, TOut>(Func<TArg1, TArg2, TArg3, TOut> func, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            func(arg1, arg2, arg3);
        }

        /// <summary>
        /// Asserts that the given operation throws one of InvalidOperationException, ArgumentException (including
        /// ArgumentOutOfRangeException) or OverflowException. (It's hard to always be consistent bearing in mind
        /// one method calling another.)
        /// </summary>
        internal static void AssertOverflow<TArg1, TOut>(Func<TArg1, TOut> func, TArg1 arg1)
        {
            AssertOverflow(() => func(arg1));
        }

        /// <summary>
        /// Typically used to report a list of items (e.g. reflection members) that fail a condition, one per line.
        /// </summary>
        internal static void AssertNoFailures<T>(IEnumerable<T> failures, Func<T, string> failureFormatter)
        {
            var failureList = failures.ToList();
            if (failureList.Count == 0)
            {
                return;
            }
            var newLine = Environment.NewLine;
            var message = $"Failures: {failureList.Count}{newLine}{string.Join(newLine, failureList.Select(failureFormatter))}";
            Assert.Fail(message);
        }

        internal static void AssertNoFailures<T>(IEnumerable<T> failures, Func<T, string> failureFormatter, TestExemptionCategory category)
            where T : MemberInfo =>
            AssertNoFailures(failures.Where(member => !IsExempt(member, category)), failureFormatter);

        private static bool IsExempt(MemberInfo member, TestExemptionCategory category) =>
            member.GetCustomAttributes(typeof(TestExemptionAttribute), false)
                .Cast<TestExemptionAttribute>()
                .Any(e => e.Category == category);

        /// <summary>
        /// Asserts that the given operation throws one of InvalidOperationException, ArgumentException (including
        /// ArgumentOutOfRangeException) or OverflowException. (It's hard to always be consistent bearing in mind
        /// one method calling another.)
        /// </summary>
        internal static void AssertOverflow(Action action)
        {
            try
            {
                action();
                Assert.Fail("Expected OverflowException, ArgumentException, ArgumentOutOfRangeException or InvalidOperationException");
            }
            catch (OverflowException)
            {
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.GetType() == typeof(ArgumentException) || e.GetType() == typeof(ArgumentOutOfRangeException),
                    "Exception should not be a subtype of ArgumentException, other than ArgumentOutOfRangeException");
            }
            catch (InvalidOperationException)
            {
            }
        }

        public static void TestComparerStruct<T>(IComparer<T> comparer, T value, T equalValue, T greaterValue) where T : struct
        {
            Assert.AreEqual(0, comparer.Compare(value, equalValue));
            Assert.AreEqual(1, Math.Sign(comparer.Compare(greaterValue, value)));
            Assert.AreEqual(-1, Math.Sign(comparer.Compare(value, greaterValue)));
        }

        /// <summary>
        ///   Tests the <see cref="IComparable{T}.CompareTo" /> method for reference objects.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="greaterValue">The values greater than the base value, in ascending order.</param>
        public static void TestCompareToClass<T>(T value, T equalValue, params T[] greaterValues) where T : class, IComparable<T>
        {
            ValidateInput(value, equalValue, greaterValues, "greaterValue");
            Assert.Greater(value.CompareTo(null), 0, "value.CompareTo<T>(null)");
            Assert.AreEqual(value.CompareTo(value), 0, "value.CompareTo<T>(value)");
            Assert.AreEqual(value.CompareTo(equalValue), 0, "value.CompareTo<T>(equalValue)");
            Assert.AreEqual(equalValue.CompareTo(value), 0, "equalValue.CompareTo<T>(value)");
            foreach (var greaterValue in greaterValues)
            {
                Assert.Less(value.CompareTo(greaterValue), 0, "value.CompareTo<T>(greaterValue)");
                Assert.Greater(greaterValue.CompareTo(value), 0, "greaterValue.CompareTo<T>(value)");
                // Now move up to the next pair...
                value = greaterValue;
            }
        }

        /// <summary>
        /// Tests the <see cref="IComparable{T}.CompareTo" /> method for value objects.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="greaterValue">The values greater than the base value, in ascending order.</param>
        public static void TestCompareToStruct<T>(T value, T equalValue, params T[] greaterValues) where T : struct, IComparable<T>
        {
            Assert.AreEqual(value.CompareTo(value), 0, "value.CompareTo(value)");
            Assert.AreEqual(value.CompareTo(equalValue), 0, "value.CompareTo(equalValue)");
            Assert.AreEqual(equalValue.CompareTo(value), 0, "equalValue.CompareTo(value)");
            foreach (var greaterValue in greaterValues)
            {
                Assert.Less(value.CompareTo(greaterValue), 0, "value.CompareTo(greaterValue)");
                Assert.Greater(greaterValue.CompareTo(value), 0, "greaterValue.CompareTo(value)");
                // Now move up to the next pair...
                value = greaterValue;
            }
        }

        /// <summary>
        /// Tests the <see cref="IComparable.CompareTo" /> method - note that this is the non-generic interface.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="greaterValue">The values greater than the base value, in ascending order.</param>
        public static void TestNonGenericCompareTo<T>(T value, T equalValue, params T[] greaterValues) where T : IComparable
        {
            // Just type the values as plain IComparable for simplicity
            IComparable value2 = value;
            IComparable equalValue2 = equalValue;

            ValidateInput(value2, equalValue2, greaterValues, "greaterValues");
            Assert.Greater(value2.CompareTo(null), 0, "value.CompareTo(null)");
            Assert.AreEqual(value2.CompareTo(value2), 0, "value.CompareTo(value)");
            Assert.AreEqual(value2.CompareTo(equalValue2), 0, "value.CompareTo(equalValue)");
            Assert.AreEqual(equalValue2.CompareTo(value2), 0, "equalValue.CompareTo(value)");

            foreach (IComparable greaterValue in greaterValues)
            {
                Assert.Less(value2.CompareTo(greaterValue), 0, "value.CompareTo(greaterValue)");
                Assert.Greater(greaterValue.CompareTo(value2), 0, "greaterValue.CompareTo(value)");
                // Now move up to the next pair...
                value2 = greaterValue;
            }
            Assert.Throws<ArgumentException>(() => value2.CompareTo(new object()));
        }

        /// <summary>
        /// Tests the IEquatable.Equals method for reference objects. Also tests the
        /// object equals method.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="unequalValue">Values not equal to the base value.</param>
        public static void TestEqualsClass<T>(T value, T equalValue, params T[] unequalValues) where T : class, IEquatable<T>
        {
            TestObjectEquals(value, equalValue, unequalValues);
            Assert.False(value.Equals(null), "value.Equals<T>(null)");
            Assert.True(value.Equals(value), "value.Equals<T>(value)");
            Assert.True(value.Equals(equalValue), "value.Equals<T>(equalValue)");
            Assert.True(equalValue.Equals(value), "equalValue.Equals<T>(value)");
            foreach (var unequal in unequalValues)
            {
                Assert.False(value.Equals(unequal), "value.Equals<T>(unequalValue)");
            }
        }

        /// <summary>
        /// Tests the IEquatable.Equals method for value objects. Also tests the
        /// object equals method.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="unequalValue">The value not equal to the base value.</param>
        public static void TestEqualsStruct<T>(T value, T equalValue, params T[] unequalValues) where T : struct, IEquatable<T>
        {
            var unequalArray = unequalValues.Cast<object>().ToArray();
            TestObjectEquals(value, equalValue, unequalArray);
            Assert.True(value.Equals(value), "value.Equals<T>(value)");
            Assert.True(value.Equals(equalValue), "value.Equals<T>(equalValue)");
            Assert.True(equalValue.Equals(value), "equalValue.Equals<T>(value)");
            foreach (var unequalValue in unequalValues)
            {
                Assert.False(value.Equals(unequalValue), "value.Equals<T>(unequalValue)");
            }
        }

        /// <summary>
        /// Tests the Object.Equals method.
        /// </summary>
        /// <remarks>
        /// It takes two equal values, and then an array of values which should not be equal to the first argument.
        /// </remarks>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="unequalValue">The value not equal to the base value.</param>
        public static void TestObjectEquals(object value, object equalValue, params object[] unequalValues)
        {
            ValidateInput(value, equalValue, unequalValues, "unequalValue");
            Assert.False(value.Equals(null), "value.Equals(null)");
            Assert.True(value.Equals(value), "value.Equals(value)");
            Assert.True(value.Equals(equalValue), "value.Equals(equalValue)");
            Assert.True(equalValue.Equals(value), "equalValue.Equals(value)");
            foreach (var unequalValue in unequalValues)
            {
                Assert.False(value.Equals(unequalValue), "value.Equals(unequalValue)");
            }
            Assert.AreEqual(value.GetHashCode(), value.GetHashCode(), "GetHashCode() twice for same object");
            Assert.AreEqual(value.GetHashCode(), equalValue.GetHashCode(), "GetHashCode() for two different but equal objects");
        }

        /// <summary>
        /// Tests the less than (&lt;) and greater than (&gt;) operators if they exist on the object.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="greaterValue">The values greater than the base value, in ascending order.</param>
        public static void TestOperatorComparison<T>(T value, T equalValue, params T[] greaterValues)
        {
            ValidateInput(value, equalValue, greaterValues, "greaterValue");
            Type type = typeof(T);
            var greaterThan = type.GetMethod("op_GreaterThan", new[] { type, type });
            var lessThan = type.GetMethod("op_LessThan", new[] { type, type });

            // Comparisons only involving equal values
            if (greaterThan != null)
            {
                if (!type.GetTypeInfo().IsValueType)
                {
                    Assert.True((bool) greaterThan.Invoke(null, new object[] { value, null }), "value > null");
                    Assert.False((bool) greaterThan.Invoke(null, new object[] { null, value }), "null > value");
                }
                Assert.False((bool) greaterThan.Invoke(null, new object[] { value, value }), "value > value");
                Assert.False((bool) greaterThan.Invoke(null, new object[] { value, equalValue }), "value > equalValue");
                Assert.False((bool) greaterThan.Invoke(null, new object[] { equalValue, value }), "equalValue > value");
            }
            if (lessThan != null)
            {
                if (!type.GetTypeInfo().IsValueType)
                {
                    Assert.False((bool) lessThan.Invoke(null, new object[] { value, null }), "value < null");
                    Assert.True((bool) lessThan.Invoke(null, new object[] { null, value }), "null < value");
                }
                Assert.False((bool) lessThan.Invoke(null, new object[] { value, value }), "value < value");
                Assert.False((bool) lessThan.Invoke(null, new object[] { value, equalValue }), "value < equalValue");
                Assert.False((bool) lessThan.Invoke(null, new object[] { equalValue, value }), "equalValue < value");
            }

            // Then comparisons involving the greater values
            foreach (var greaterValue in greaterValues)
            {
                if (greaterThan != null)
                {
                    Assert.False((bool) greaterThan.Invoke(null, new object[] { value, greaterValue }), "value > greaterValue");
                    Assert.True((bool) greaterThan.Invoke(null, new object[] { greaterValue, value }), "greaterValue > value");
                }
                if (lessThan != null)
                {
                    Assert.True((bool) lessThan.Invoke(null, new object[] { value, greaterValue }), "value < greaterValue");
                    Assert.False((bool) lessThan.Invoke(null, new object[] { greaterValue, value }), "greaterValue < value");
                }
                // Now move up to the next pair...
                value = greaterValue;
            }
        }

        /// <summary>
        /// Tests the equality (==), inequality (!=), less than (&lt;), greater than (&gt;), less than or equals (&lt;=),
        /// and greater than or equals (&gt;=) operators if they exist on the object.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="greaterValue">The values greater than the base value, in ascending order.</param>
        public static void TestOperatorComparisonEquality<T>(T value, T equalValue, params T[] greaterValues)
        {
            foreach (var greaterValue in greaterValues)
            {
                TestOperatorEquality(value, equalValue, greaterValue);
            }
            TestOperatorComparison(value, equalValue, greaterValues);
            Type type = typeof(T);
            var greaterThanOrEqual = type.GetMethod("op_GreaterThanOrEqual", new[] { type, type });
            var lessThanOrEqual = type.GetMethod("op_LessThanOrEqual", new[] { type, type });

            // First the comparisons with equal values
            if (greaterThanOrEqual != null)
            {
                if (!type.GetTypeInfo().IsValueType)
                {
                    Assert.True((bool) greaterThanOrEqual.Invoke(null, new object[] { value, null }), "value >= null");
                    Assert.False((bool) greaterThanOrEqual.Invoke(null, new object[] { null, value }), "null >= value");
                }
                Assert.True((bool) greaterThanOrEqual.Invoke(null, new object[] { value, value }), "value >= value");
                Assert.True((bool) greaterThanOrEqual.Invoke(null, new object[] { value, equalValue }), "value >= equalValue");
                Assert.True((bool) greaterThanOrEqual.Invoke(null, new object[] { equalValue, value }), "equalValue >= value");
            }
            if (lessThanOrEqual != null)
            {
                if (!type.GetTypeInfo().IsValueType)
                {
                    Assert.False((bool) lessThanOrEqual.Invoke(null, new object[] { value, null }), "value <= null");
                    Assert.True((bool) lessThanOrEqual.Invoke(null, new object[] { null, value }), "null <= value");
                }
                Assert.True((bool) lessThanOrEqual.Invoke(null, new object[] { value, value }), "value <= value");
                Assert.True((bool) lessThanOrEqual.Invoke(null, new object[] { value, equalValue }), "value <= equalValue");
                Assert.True((bool) lessThanOrEqual.Invoke(null, new object[] { equalValue, value }), "equalValue <= value");
            }

            // Now the "greater than" values
            foreach (var greaterValue in greaterValues)
            {
                if (greaterThanOrEqual != null)
                {
                    Assert.False((bool) greaterThanOrEqual.Invoke(null, new object[] { value, greaterValue }), "value >= greaterValue");
                    Assert.True((bool) greaterThanOrEqual.Invoke(null, new object[] { greaterValue, value }), "greaterValue >= value");
                }
                if (lessThanOrEqual != null)
                {
                    Assert.True((bool) lessThanOrEqual.Invoke(null, new object[] { value, greaterValue }), "value <= greaterValue");
                    Assert.False((bool) lessThanOrEqual.Invoke(null, new object[] { greaterValue, value }), "greaterValue <= value");
                }
                // Now move up to the next pair...
                value = greaterValue;
            }
        }

        /// <summary>
        ///   Tests the equality and inequality operators (==, !=) if they exist on the object.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="unequalValue">The value not equal to the base value.</param>
        public static void TestOperatorEquality<T>(T value, T equalValue, T unequalValue)
        {
            ValidateInput(value, equalValue, unequalValue, "unequalValue");
            Type type = typeof(T);
            var equality = type.GetMethod("op_Equality", new[] { type, type });
            if (equality != null)
            {
                if (!type.GetTypeInfo().IsValueType)
                {
                    Assert.True((bool)equality.Invoke(null, new object[] { null, null }), "null == null");
                    Assert.False((bool)equality.Invoke(null, new object[] { value, null }), "value == null");
                    Assert.False((bool)equality.Invoke(null, new object[] { null, value }), "null == value");
                }
                Assert.True((bool)equality.Invoke(null, new object[] { value, value }), "value == value");
                Assert.True((bool)equality.Invoke(null, new object[] { value, equalValue }), "value == equalValue");
                Assert.True((bool)equality.Invoke(null, new object[] { equalValue, value }), "equalValue == value");
                Assert.False((bool)equality.Invoke(null, new object[] { value, unequalValue }), "value == unequalValue");
            }
            var inequality = type.GetMethod("op_Inequality", new[] { type, type });
            if (inequality != null)
            {
                if (!type.GetTypeInfo().IsValueType)
                {
                    Assert.False((bool)inequality.Invoke(null, new object[] { null, null }), "null != null");
                    Assert.True((bool)inequality.Invoke(null, new object[] { value, null }), "value != null");
                    Assert.True((bool)inequality.Invoke(null, new object[] { null, value }), "null != value");
                }
                Assert.False((bool)inequality.Invoke(null, new object[] { value, value }), "value != value");
                Assert.False((bool)inequality.Invoke(null, new object[] { value, equalValue }), "value != equalValue");
                Assert.False((bool)inequality.Invoke(null, new object[] { equalValue, value }), "equalValue != value");
                Assert.True((bool)inequality.Invoke(null, new object[] { value, unequalValue }), "value != unequalValue");
            }
        }
        
        /// <summary>
        /// Validates that the given value can be serialized, and that deserializing the same data
        /// returns an equal value.
        /// </summary>
        /// <remarks>
        /// This method is effectively ignored in the .NET Core tests - that means all the binary serialization tests 
        /// do nothing on .NET Core, but it's simpler than conditionalizing each one of them.
        /// </remarks>
        internal static void AssertBinaryRoundtrip<T>(T value)
        {
            // Can't use [Conditional("!NETCORE")] as ConditionalAttribute is only positive.
            // This approach seems to confuse the build system less, too.
#if !NETCORE
            var stream = new MemoryStream();
            new BinaryFormatter().Serialize(stream, value);

            stream.Position = 0;
            var rehydrated = (T)new BinaryFormatter().Deserialize(stream);
            Assert.AreEqual(value, rehydrated);
#endif
        }

#if !NETCORE
        internal static void AssertBinaryDeserializationFailure<T>(Type expectedExceptionType, Action<SerializationInfo> fillInfoAction)
        {
            var info = new SerializationInfo(typeof(T), new FormatterConverter());
            fillInfoAction(info);
            var ctor = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                new Type[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);
            Assert.NotNull(ctor);
            var context = new StreamingContext();
            var exception = Assert.Throws<TargetInvocationException>(() => ctor.Invoke(new object[] { info, context }));
            Assert.IsInstanceOf(expectedExceptionType, exception.InnerException);
        }
#endif

        /// <summary>
        /// Validates that a value can be serialized to the expected XML, deserialized to an equal
        /// value, and that a direct call of ReadXml on a value with an XmlReader initially positioned
        /// at an element will read to the end of that element.
        /// </summary>
        internal static void AssertXmlRoundtrip<T>(T value, string expectedXml, IEqualityComparer<T> comparer = null)
            where T : IXmlSerializable, new()
        {
            comparer = comparer ?? EqualityComparer<T>.Default;

            // Just include this here to simply cover everything that's doing XML serialization.
            Assert.Null(value.GetSchema());

            XmlSerializer serializer = new XmlSerializer(typeof(SerializationHelper<T>));
            var helper = new SerializationHelper<T> { Value = value, Before = 100, After = 200 };
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, helper);
                stream.Position = 0;
                var result = (SerializationHelper<T>) serializer.Deserialize(stream);
                Assert.IsTrue(comparer.Equals(result.Value, value), "Expected " + value + "; was " + result.Value);
                // Validate the rest of the object deserialization is still okay.
                Assert.AreEqual(100, result.Before);
                Assert.AreEqual(200, result.After);
                
                stream.Position = 0;
                var element = XElement.Load(stream).Element("value");
                Assert.AreEqual(element.ToString(), expectedXml);
            }
            AssertReadXmlConsumesElement<T>(expectedXml);
        }

        internal static void AssertParsableXml<T>(T expectedValue, string validXml)  where T : IXmlSerializable, new()
        {
            AssertParsableXml(expectedValue, validXml, EqualityComparer<T>.Default);
        }

        internal static void AssertParsableXml<T>(T expectedValue, string validXml, IEqualityComparer<T> comparer)
            where T : IXmlSerializable, new()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializationHelper<T>));
            // Serialize any value, just so we can replace the <value> element.
            var helper = new SerializationHelper<T> { Value = new T(), Before = 100, After = 200 };
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, helper);
                stream.Position = 0;
                var doc = XElement.Load(stream);
                doc.Element("value").ReplaceWith(XElement.Parse(validXml));
                var result = (SerializationHelper<T>) serializer.Deserialize(doc.CreateReader());
                Assert.IsTrue(comparer.Equals(result.Value, expectedValue), "Expected " + expectedValue + "; was " + result.Value);
                // Validate the rest of the object deserialization is still okay.
                Assert.AreEqual(100, result.Before);
                Assert.AreEqual(200, result.After);
            }
            AssertReadXmlConsumesElement<T>(validXml);
        }

        private static void AssertReadXmlConsumesElement<T>(string validXml) where T : IXmlSerializable, new()
        {
            XElement element = XElement.Parse(validXml);
            var root = new XElement("root", element, new XElement("after"));
            var reader = root.CreateReader();
            reader.ReadToDescendant(element.Name.LocalName);
            var value = new T();
            value.ReadXml(reader);
            Assert.AreEqual(XmlNodeType.Element, reader.NodeType);
            Assert.AreEqual("after", reader.Name);
        }

        internal static void AssertXmlInvalid<T>(string invalidXml, Type expectedExceptionType) where T : new()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializationHelper<T>));
            // Serialize any value, just so we can replace the <value> element.
            var helper = new SerializationHelper<T> { Value = new T() };
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, helper);
                stream.Position = 0;
                var doc = XElement.Load(stream);
                doc.Element("value").ReplaceWith(XElement.Parse(invalidXml));
                // Sometimes exceptions are wrapped in InvalidOperationException, sometimes they're not. It's not
                // always easy to predict. (.NET always does; old Mono never does; new Mono sometimes does - I think.)
                // Odd that I can't just specify "well it throws something, I'll check the details later". Oh well.
                var exception = Assert.Throws(Is.InstanceOf(typeof(Exception)), () => serializer.Deserialize(doc.CreateReader()));
                if (exception is InvalidOperationException)
                {
                    exception = exception.InnerException;
                }
                Assert.IsInstanceOf(expectedExceptionType, exception);                    
            }
        }

        /// <summary>
        /// Validates that the input parameters to the test methods are valid.
        /// </summary>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="unequalValues">The values not equal to the base value.</param>
        /// <param name="unequalName">The name to use in "not equal value" error messages.</param>
        private static void ValidateInput(object value, object equalValue, object[] unequalValues, string unequalName)
        {
            Assert.NotNull(value, "value cannot be null in TestObjectEquals() method");
            Assert.NotNull(equalValue, "equalValue cannot be null in TestObjectEquals() method");
            Assert.AreNotSame(value, equalValue, "value and equalValue MUST be different objects");
            foreach (var unequalValue in unequalValues)
            {
                Assert.NotNull(unequalValue, unequalName + " cannot be null in TestObjectEquals() method");
                Assert.AreNotSame(value, unequalValue, unequalName + " and value MUST be different objects");
            }
        }

        /// <summary>
        /// Validates that the input parameters to the test methods are valid.
        /// </summary>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="unequalValue">The value not equal to the base value.</param>
        /// <param name="unequalName">The name to use in "not equal value" error messages.</param>
        private static void ValidateInput(object value, object equalValue, object unequalValue, string unequalName)
        {
            ValidateInput(value, equalValue, new object[] { unequalValue }, unequalName);
        }
    }

    /// <summary>
    /// Class used in XML serialization tests. This would be nested within TestHelper,
    /// but some environments don't like serializing nested types within static types.
    /// </summary>
    /// <typeparam name="T">Type of value to serialize</typeparam>
    public class SerializationHelper<T>
    {
        [XmlElement("before")]
        public int Before { get; set; }

        [XmlElement("value")]
        public T Value { get; set; }

        [XmlElement("after")]
        public int After { get; set; }
    }
}
