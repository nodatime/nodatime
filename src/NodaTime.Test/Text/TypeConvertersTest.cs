// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NUnit.Framework;
using System;
using System.ComponentModel;

namespace NodaTime.Test.Text
{
    public class TypeConvertersTest
    {
        [Test]
        [TestCase(typeof(AnnualDate))]
        [TestCase(typeof(Duration))]
        [TestCase(typeof(Instant))]
        [TestCase(typeof(LocalDate))]
        [TestCase(typeof(LocalDateTime))]
        [TestCase(typeof(LocalTime))]
        [TestCase(typeof(Offset))]
        [TestCase(typeof(OffsetDate))]
        [TestCase(typeof(OffsetDateTime))]
        [TestCase(typeof(OffsetTime))]
        [TestCase(typeof(Period))]
        [TestCase(typeof(YearMonth))]
        [TestCase(typeof(ZonedDateTime))]
        public void HasConverter(Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);

            Assert.NotNull(converter);
            Assert.AreNotEqual(TypeDescriptor.GetConverter(typeof(object)), converter);

            Assert.True(converter.CanConvertFrom(typeof(string)));
            Assert.True(converter.CanConvertTo(typeof(string)));

            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(null!));
            Assert.Throws<UnparsableValueException>(() => converter.ConvertFrom(""));

            if (type.IsValueType)
            {
                Assert.DoesNotThrow(() => converter.ConvertToString(Activator.CreateInstance(type)));
            }
        }

        [Test]
        [TestCase(01, 01, "01-01")]
        [TestCase(12, 31, "12-31")]
        public void AnnualDate_Roundtrip(int month, int day, string text) =>
            AssertRoundtrip(text, new AnnualDate(month, day));

        [Test]
        [TestCase(0, 0, "0:00:00:00")]
        [TestCase(0, 1, "0:00:00:00.000000001")]
        [TestCase(1, 0, "1:00:00:00")]
        [TestCase(-1, 0, "-1:00:00:00")]
        public void Duration_Roundtrip(int days, long nanoOfDay, string text) =>
            AssertRoundtrip(text, new Duration(days, nanoOfDay));

        [Test]
        [TestCase(2018, 01, 01, 00, 00, 00, 000, "2018-01-01T00:00:00")]
        [TestCase(2018, 12, 31, 23, 59, 59, 999, "2018-12-31T23:59:59.999")]
        public void LocalDateTime_Roundtrip(int year, int month, int day, int hour, int minute, int second, int millisecond, string text) =>
            AssertRoundtrip(text, new LocalDateTime(year, month, day, hour, minute, second, millisecond));

        [Test]
        [TestCase(2018, 01, 01, "2018-01-01")]
        [TestCase(2018, 12, 31, "2018-12-31")]
        public void LocalDate_Roundtrip(int year, int month, int day, string text) =>
            AssertRoundtrip(text, new LocalDate(year, month, day));

        [Test]
        [TestCase(0, 0, "1970-01-01T00:00:00Z")]
        [TestCase(0, 1, "1970-01-01T00:00:00.000000001Z")]
        [TestCase(-1, 0, "1969-12-31T00:00:00Z")]
        public void Instant_Roundtrip(int days, long nanoOfDay, string text) =>
            AssertRoundtrip(text, new Instant(days, nanoOfDay));

        [Test]
        [TestCase(00, 00, 00, 000, "00:00:00")]
        [TestCase(00, 00, 00, 001, "00:00:00.001")]
        [TestCase(23, 59, 59, 999, "23:59:59.999")]
        public void LocalTime_Roundtrip(int hour, int minute, int second, int millisecond, string text) =>
            AssertRoundtrip(text, new LocalTime(hour, minute, second, millisecond));

        [Test]
        [TestCase(-25200, "-07")]
        [TestCase(0, "Z")]
        [TestCase(20700, "+05:45")]
        public void Offset_Roundtrip(int seconds, string text) =>
            AssertRoundtrip(text, new Offset(seconds));

        [Test]
        [TestCase(2019, 1, 1, -25200, "2019-01-01-07")]
        [TestCase(2018, 12, 31, 0, "2018-12-31Z")]
        [TestCase(2020, 2, 29, 20700, "2020-02-29+05:45")]
        public void OffsetDate_Roundtrip(int year, int month, int day, int seconds, string text) =>
            AssertRoundtrip(text, new OffsetDate(new LocalDate(year, month, day), new Offset(seconds)));

        [Test]
        [TestCase(2019, 1, 1, 4, 59, -25200, "2019-01-01T04:59:00-07")]
        [TestCase(2020, 2, 29, 23, 59, 0, "2020-02-29T23:59:00Z")]
        [TestCase(2018, 12, 31, 13, 30, 20700, "2018-12-31T13:30:00+05:45")]
        public void OffsetDateTime_Roundtrip(int year, int month, int day, int hour, int minute, int offsetSeconds, string text) =>
            AssertRoundtrip(text, new OffsetDateTime(new LocalDateTime(year, month, day, hour, minute), new Offset(offsetSeconds)));

        [Test]
        [TestCase(0, 0, 0, 0, -25200, "00:00:00-07")]
        [TestCase(0, 0, 0, 1, 0, "00:00:00.001Z")]
        [TestCase(23, 59, 59, 999, 20700, "23:59:59.999+05:45")]
        public void OffsetTime_Roundtrip(int hour, int minute, int second, int millisecond, int seconds, string text) =>
            AssertRoundtrip(text, new OffsetTime(new LocalTime(hour, minute, second, millisecond), new Offset(seconds)));

        [Test]
        [TestCase(00, 00, 00, 00, 00, 00, 00, 00, 00, 00, "P0D")]
        [TestCase(01, 01, 01, 01, 01, 01, 01, 01, 01, 01, "P1Y1M1W1DT1H1M1S1s1t1n")]
        public void Period_Roundtrip(int years, int months, int weeks, int days, long hours, long minutes, long seconds, long milliseconds, long ticks, long nanoseconds, string text) =>
            AssertRoundtrip(text, new Period(years, months, weeks, days, hours, minutes, seconds, milliseconds, ticks, nanoseconds));

        [Test]
        [TestCase(2019, 1, 1, 4, 59, "Europe/Paris", "2019-01-01T04:59:00 Europe/Paris (+01)")]
        [TestCase(2020, 2, 29, 23, 59, "America/Los_Angeles", "2020-02-29T23:59:00 America/Los_Angeles (-08)")]
        [TestCase(2018, 12, 31, 13, 30, "Asia/Kathmandu", "2018-12-31T13:30:00 Asia/Kathmandu (+05:45)")]
        public void ZonedDateTime_Roundtrip(int year, int month, int day, int hour, int minute, string zoneId, string text)
        {
            Assert.AreSame(DateTimeZoneProviders.Tzdb, TypeConverterSettings.DateTimeZoneProvider,
                "Expected no other test to change DateTimeZoneProviders.ForTypeConverter");
            var zone = DateTimeZoneProviders.Tzdb[zoneId];
            AssertRoundtrip(text, new LocalDateTime(year, month, day, hour, minute).InZoneStrictly(zone));
        }

        [Test]
        [TestCase(2020, 5, "2020-05")]
        [TestCase(1976, 6, "1976-06")]
        public void YearMonth_RoundTrip(int year, int month, string text) =>
            AssertRoundtrip(text, new YearMonth(year, month));

        private static void AssertRoundtrip<T>(string textEquivalent, T nodaValue)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            var valueFromConverter = (T)converter.ConvertFrom(textEquivalent)!;
            Assert.AreEqual(nodaValue, valueFromConverter);

            var textFromConverter = converter.ConvertTo(nodaValue, typeof(string));
            Assert.AreEqual(textEquivalent, textFromConverter);
        }
    }
}
