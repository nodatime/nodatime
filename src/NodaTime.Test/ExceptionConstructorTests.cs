// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NUnit.Framework;
using System;
using System.IO;
#if !NETCORE
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace NodaTime.Test
{
    /// <summary>
    /// Tests for exceptions - we don't typically call the serialization code anywhere else, for eaxmple.
    /// </summary>
    public class ExceptionConstructorTests
    {
        // We never actually use this constructor, but it's in the public API and it's harmless...
        [Test]
        public void InvalidPatternExceptionParameterlessConstructor()
        {
            var exception = new InvalidPatternException();
            Assert.AreEqual(new FormatException().Message, exception.Message);
        }

        // We never actually use this constructor, but it's in the public API and it's harmless...
        [Test]
        public void UnparsableValueExceptionParameterlessConstructor()
        {
            var exception = new UnparsableValueException();
            Assert.AreEqual(new FormatException().Message, exception.Message);
        }

        // Only two of our constructors actually have a private constructor for serialization.
        // Others have the serializable attribute without the constructor - which may or may not be bad,
        // but I'm not going to investigate it...
        [Test]
        [TestCase(typeof(InvalidPatternException))]
        [TestCase(typeof(UnparsableValueException))]
        public void BinaryFormat(Type type)
        {
#if !NETCORE
            var value = type.GetConstructor(new[] { typeof(string) }).Invoke(new object[] { "Message" });
            var stream = new MemoryStream();
            new BinaryFormatter().Serialize(stream, value);

            stream.Position = 0;
            var rehydrated = new BinaryFormatter().Deserialize(stream);
            Assert.IsInstanceOf(type, rehydrated);
#endif
        }
    }
}
