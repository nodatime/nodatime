// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using NUnit.Framework;

namespace NodaTime.Test
{
    /// <summary>
    /// Provides methods to help run tests for some of the system interfaces and object support.
    /// </summary>
    public static class TestHelper
    {
        public static readonly bool IsRunningOnMono = Type.GetType("Mono.Runtime") != null;

        /// <summary>
        ///   Tests the <see cref="IComparable{T}.CompareTo" /> method for reference objects.
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
        /// Tests the <see cref="IComparable{T}.CompareTo" /> method for value objects.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="greaterValue">The value greater than the base value..</param>
        public static void TestCompareToStruct<T>(T value, T equalValue, T greaterValue) where T : struct, IComparable<T>
        {
            ValidateInput(value, equalValue, greaterValue, "greaterValue");
            Assert.AreEqual(value.CompareTo(value), 0, "value.CompareTo(value)");
            Assert.AreEqual(value.CompareTo(equalValue), 0, "value.CompareTo(equalValue)");
            Assert.AreEqual(equalValue.CompareTo(value), 0, "equalValue.CompareTo(value)");
            Assert.Less(value.CompareTo(greaterValue), 0, "value.CompareTo(greaterValue)");
            Assert.Greater(greaterValue.CompareTo(value), 0, "greaterValue.CompareTo(value)");
        }

        /// <summary>
        /// Tests the <see cref="IComparable.CompareTo" /> method - note that this is the non-generic interface.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The base value.</param>
        /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
        /// <param name="greaterValue">The value greater than the base value..</param>
        public static void TestNonGenericCompareTo<T>(T value, T equalValue, T greaterValue) where T : IComparable
        {
            // Just type the values as plain IComparable for simplicity
            IComparable value2 = value;
            IComparable equalValue2 = equalValue;
            IComparable greaterValue2 = greaterValue;

            ValidateInput(value2, equalValue2, greaterValue2, "greaterValue");
            Assert.Greater(value2.CompareTo(null), 0, "value.CompareTo<T>(null)");
            Assert.AreEqual(value2.CompareTo(value2), 0, "value.CompareTo<T>(value)");
            Assert.AreEqual(value2.CompareTo(equalValue2), 0, "value.CompareTo<T>(equalValue)");
            Assert.AreEqual(equalValue2.CompareTo(value2), 0, "equalValue.CompareTo<T>(value)");
            Assert.Less(value2.CompareTo(greaterValue2), 0, "value.CompareTo<T>(greaterValue)");
            Assert.Greater(greaterValue2.CompareTo(value2), 0, "greaterValue.CompareTo<T>(value)");
            Assert.Throws<ArgumentException>(() => value2.CompareTo(new object()));
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

        internal static void AssertXmlRoundtrip<T>(T value, string expectedXml) where T : IXmlSerializable, new()
        {
            AssertXmlRoundtrip(value, expectedXml, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Validates that a value can be serialized to the expected XML, deserialized to an equal
        /// value, and that a direct call of ReadXml on a value with an XmlReader initially positioned
        /// at an element will read to the end of that element.
        /// </summary>
        internal static void AssertXmlRoundtrip<T>(T value, string expectedXml, IEqualityComparer<T> comparer)
            where T : IXmlSerializable, new()
        {
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
                // .NET wraps any exceptions in InvalidOperationException; Mono doesn't.
                if (IsRunningOnMono)
                {
                    Assert.Throws(expectedExceptionType, () => serializer.Deserialize(doc.CreateReader()));
                }
                else
                {
                    var exception = Assert.Throws<InvalidOperationException>(() => serializer.Deserialize(doc.CreateReader()));
                    Assert.IsInstanceOf(expectedExceptionType, exception.InnerException);                    
                }
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
