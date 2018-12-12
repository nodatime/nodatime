// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.ComponentModel;
using JetBrains.Annotations;
using NUnit.Framework;

namespace NodaTime.Test.Text
{
    /// <summary>
    /// Base class for all the type converter tests.
    /// </summary>
    public abstract class TypeConverterBaseTestBase<T>
    {
        [NotNull] static readonly TypeConverter TypeConverter = TypeDescriptor.GetConverter(typeof(T));

        [Test]
        [TestCaseSource("RoundtripData")]
        public void Roundtrip(string input)
        {
            Assert.True(TypeConverter.CanConvertFrom(typeof(string)));
            Assert.True(TypeConverter.CanConvertTo(typeof(string)));

            var parsed = TypeConverter.ConvertFrom(input);

            Assert.NotNull(parsed);
            Assert.IsInstanceOf<T>(parsed);

            var serialized = TypeConverter.ConvertTo((T) parsed, typeof(string));

            Assert.NotNull(serialized);
            Assert.IsInstanceOf<string>(serialized);

            Assert.AreEqual(input, serialized);
        }
    }
}