// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.IO;
using NodaTime.Utility;
using NUnit.Framework;
using Newtonsoft.Json;
using NodaTime.Serialization.JsonNet;

namespace NodaTime.Serialization.Test.JsonNet
{
    [TestFixture]
    public class NodaConverterBaseTest
    {
        [Test]
        public void Serialize_NonNullValue()
        {
            var converter = new TestConverter();

            JsonConvert.SerializeObject(5, Formatting.None, converter);
        }

        [Test]
        public void Serialize_NullValue()
        {
            var converter = new TestConverter();

            JsonConvert.SerializeObject(null, Formatting.None, converter);
        }

        [Test]
        public void Deserialize_NullableType_NullValue()
        {
            var converter = new TestConverter();

            Assert.IsNull(JsonConvert.DeserializeObject<int?>("null", converter));
            Assert.IsNull(JsonConvert.DeserializeObject<int?>("\"\"", converter));
        }

        [Test]
        public void Deserialize_ReferenceType_NullValue()
        {
            var converter = new TestStringConverter();

            Assert.IsNull(JsonConvert.DeserializeObject<string>("null", converter));
            Assert.IsNull(JsonConvert.DeserializeObject<string>("\"\"", converter));
        }

        [Test]
        public void Deserialize_NullableType_NonNullValue()
        {
            var converter = new TestConverter();

            Assert.AreEqual(5, JsonConvert.DeserializeObject<int?>("\"5\"", converter));
        }

        [Test]
        public void Deserialize_NonNullableType_NullValue()
        {
            var converter = new TestConverter();

            Assert.Throws<InvalidNodaDataException>(() => JsonConvert.DeserializeObject<int>("null", converter));
            Assert.Throws<InvalidNodaDataException>(() => JsonConvert.DeserializeObject<int>("\"\"", converter));
        }

        [Test]
        public void Deserialize_NonNullableType_NonNullValue()
        {
            var converter = new TestConverter();

            Assert.AreEqual(5, JsonConvert.DeserializeObject<int>("\"5\"", converter));
        }

        [Test]
        public void CanConvert_ValidValues()
        {
            var converter = new TestConverter();

            Assert.IsTrue(converter.CanConvert(typeof(int)));
            Assert.IsTrue(converter.CanConvert(typeof(int?)));
        }

        [Test]
        public void CanConvert_InvalidValues()
        {
            var converter = new TestConverter();

            Assert.IsFalse(converter.CanConvert(typeof(uint)));
        }

        [Test]
        public void CanConvert_Inheritance()
        {
            var converter = new TestInheritanceConverter();

            Assert.IsTrue(converter.CanConvert(typeof(MemoryStream)));
        }

        private class TestConverter : NodaConverterBase<int>
        {
            protected override int ReadJsonImpl(JsonReader reader, JsonSerializer serializer)
            {
                return int.Parse(reader.Value.ToString());
            }

            protected override void WriteJsonImpl(JsonWriter writer, int value, JsonSerializer serializer)
            {
                writer.WriteValue(value.ToString());
            }
        }

        private class TestStringConverter : NodaConverterBase<string>
        {
            protected override string ReadJsonImpl(JsonReader reader, JsonSerializer serializer)
            {
                return reader.Value.ToString();
            }

            protected override void WriteJsonImpl(JsonWriter writer, string value, JsonSerializer serializer)
            {
                writer.WriteValue(value);
            }
        }

        /// <summary>
        /// Just use for CanConvert testing...
        /// </summary>
        private class TestInheritanceConverter : NodaConverterBase<Stream>
        {
            protected override Stream ReadJsonImpl(JsonReader reader, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            protected override void WriteJsonImpl(JsonWriter writer, Stream value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
