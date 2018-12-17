// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.ComponentModel;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Text
{
    public class TypeConvertersTest
    {
        #region HasConverter

        [Test]
        public void AnnualDate_HasConverter() => HasConverter<AnnualDate>();

        [Test]
        public void Duration_HasConverter() => HasConverter<Duration>();

        [Test]
        public void LocalDateTime_HasConverter() => HasConverter<LocalDateTime>();

        [Test]
        public void LocalDate_HasConverter() => HasConverter<LocalDate>();

        [Test]
        public void Instant_HasConverter() => HasConverter<Instant>();

        [Test]
        public void LocalTime_HasConverter() => HasConverter<LocalTime>();

        [Test]
        public void Period_HasConverter() => HasConverter<Period>();

        #endregion

        #region CanRoundtrip

        [Test]
        [TestCase(01, 01, "01-01")]
        [TestCase(12, 31, "12-31")]
        public void AnnualDate_CanRoundtrip(int month, int day, string text)
            => CanRoundtrip(text, new AnnualDate(month, day));

        [Test]
        [TestCase(0, 0, "0:00:00:00")]
        [TestCase(0, 1, "0:00:00:00.000000001")]
        [TestCase(1, 0, "1:00:00:00")]
        [TestCase(-1, 0, "-1:00:00:00")]
        public void Duration_CanRoundtrip(int days, long nanoOfDay, string text)
            => CanRoundtrip(text, new Duration(days, nanoOfDay));

        [Test]
        [TestCase(2018, 01, 01, 00, 00, 00, 000, "2018-01-01T00:00:00")]
        [TestCase(2018, 12, 31, 23, 59, 59, 999, "2018-12-31T23:59:59.999")]
        public void LocalDateTime_CanRoundtrip(int year, int month, int day, int hour, int minute, int second, int millisecond, string text)
            => CanRoundtrip(text, new LocalDateTime(year, month, day, hour, minute, second, millisecond));

        [Test]
        [TestCase(2018, 01, 01, "2018-01-01")]
        [TestCase(2018, 12, 31, "2018-12-31")]
        public void LocalDate_CanRoundtrip(int year, int month, int day, string text)
            => CanRoundtrip(text, new LocalDate(year, month, day));

        [Test]
        [TestCase(0, 0, "1970-01-01T00:00:00Z")]
        [TestCase(0, 1, "1970-01-01T00:00:00.000000001Z")]
        [TestCase(-1, 0, "1969-12-31T00:00:00Z")]
        public void Instant_CanRoundtrip(int days, long nanoOfDay, string text)
            => CanRoundtrip(text, new Instant(days, nanoOfDay));

        [Test]
        [TestCase(00, 00, 00, 000, "00:00:00")]
        [TestCase(00, 00, 00, 001, "00:00:00.001")]
        [TestCase(23, 59, 59, 999, "23:59:59.999")]
        public void LocalTime_CanRoundtrip(int hour, int minute, int second, int millisecond, string text)
            => CanRoundtrip(text, new LocalTime(hour, minute, second, millisecond));

        [Test]
        [TestCase(00, 00, 00, 00, 00, 00, 00, 00, 00, 00, "P")]
        [TestCase(01, 01, 01, 01, 01, 01, 01, 01, 01, 01, "P1Y1M1W1DT1H1M1S1s1t1n")]
        public void Period_CanRoundtrip(int years, int months, int weeks, int days, long hours, long minutes, long seconds, long milliseconds, long ticks, long nanoseconds, string text)
            => CanRoundtrip(text, new Period(years, months, weeks, days, hours, minutes, seconds, milliseconds, ticks, nanoseconds));

        #endregion

        #region Helpers

        private static void CanRoundtrip<T>(string input, T expected)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));

            var parsed = converter.ConvertFrom(input);

            Assert.NotNull(parsed);
            Assert.AreEqual(expected, parsed);

            var serialized = converter.ConvertTo((T) parsed, typeof(string));

            Assert.NotNull(serialized);
            Assert.AreEqual(input, serialized);
        }

        private static void HasConverter<T>()
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));

            Assert.NotNull(converter);
            Assert.AreNotEqual(TypeDescriptor.GetConverter(typeof(object)), converter);

            Assert.True(converter.CanConvertFrom(typeof(string)));
            Assert.True(converter.CanConvertTo(typeof(string)));

            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(null));
            Assert.Throws<UnparsableValueException>(() => converter.ConvertFrom(""));

            if (default(T) is T value)
                Assert.DoesNotThrow(() => converter.ConvertToString(value));
        }

        #endregion
    }
}