// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NodaTime.Serialization.JsonNet;

namespace NodaTime.Serialization.Test.JsonNet
{
    /// <summary>
    /// Tests for the converters exposed in NodaConverters.
    /// </summary>
    [TestFixture]
    public class NodaConvertersTest
    {
        [Test]
        public void OffsetConverter_Serialize()
        {
            var offset = Offset.FromHoursAndMinutes(5, 30);

            var json = JsonConvert.SerializeObject(offset, Formatting.None, NodaConverters.OffsetConverter);

            string expectedJson = "\"+05:30\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void OffsetConverter_Deserialize()
        {
            string json = "\"+05:30\"";

            var offset = JsonConvert.DeserializeObject<Offset>(json, NodaConverters.OffsetConverter);

            var expectedOffset = Offset.FromHoursAndMinutes(5, 30);
            Assert.AreEqual(expectedOffset, offset);
        }

        [Test]
        public void InstantConverter_Serialize()
        {
            var instant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var json = JsonConvert.SerializeObject(instant, Formatting.None, NodaConverters.InstantConverter);
            string expectedJson = "\"2012-01-02T03:04:05Z\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void InstantConverter_Deserialize()
        {
            string json = "\"2012-01-02T03:04:05Z\"";

            var instant = JsonConvert.DeserializeObject<Instant>(json, NodaConverters.InstantConverter);

            var expectedInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            Assert.AreEqual(expectedInstant, instant);
        }

        [Test]
        public void InstantConverter_EquivalentToIsoDateTimeConverter()
        {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var instant = Instant.FromDateTimeUtc(dateTime);
            var jsonDateTime = JsonConvert.SerializeObject(dateTime, new IsoDateTimeConverter());
            var jsonInstant = JsonConvert.SerializeObject(instant, Formatting.None, NodaConverters.InstantConverter);
            Assert.AreEqual(jsonDateTime, jsonInstant);
        }

        [Test]
        public void LocalDateConverter_Serialize()
        {
            var localDate = new LocalDate(2012, 1, 2, CalendarSystem.Iso);

            var json = JsonConvert.SerializeObject(localDate, Formatting.None, NodaConverters.LocalDateConverter);

            string expectedJson = "\"2012-01-02\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void LocalDateConverter_Deserialize()
        {
            string json = "\"2012-01-02\"";

            var localDate = JsonConvert.DeserializeObject<LocalDate>(json, NodaConverters.LocalDateConverter);

            var expectedLocalDate = new LocalDate(2012, 1, 2, CalendarSystem.Iso);
            Assert.AreEqual(expectedLocalDate, localDate);
        }

        [Test]
        public void LocalDateConverter_SerializeNonIso_Throws()
        {
            var localDate = new LocalDate(2012, 1, 2, CalendarSystem.GetCopticCalendar(4));

            Assert.Throws<ArgumentException>(() => JsonConvert.SerializeObject(localDate, Formatting.None, NodaConverters.LocalDateConverter));
        }

        [Test]
        public void LocalDateTimeConverter_Serialize()
        {
            var localDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, 6, 7, CalendarSystem.Iso);

            var json = JsonConvert.SerializeObject(localDateTime, Formatting.None, NodaConverters.LocalDateTimeConverter);

            string expectedJson = "\"2012-01-02T03:04:05.0060007\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void LocalDateTimeConverter_Deserialize()
        {
            string json = "\"2012-01-02T03:04:05.0060007\"";

            var localDateTime = JsonConvert.DeserializeObject<LocalDateTime>(json, NodaConverters.LocalDateTimeConverter);

            var expectedLocalDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, 6, 7, CalendarSystem.Iso);
            Assert.AreEqual(expectedLocalDateTime, localDateTime);
        }

        [Test]
        public void LocalDateTimeConverter_EquivalentToIsoDateTimeConverter()
        {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, 6, DateTimeKind.Unspecified);
            var localDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, 6, CalendarSystem.Iso);

            var jsonDateTime = JsonConvert.SerializeObject(dateTime, new IsoDateTimeConverter());
            var jsonLocalDateTime = JsonConvert.SerializeObject(localDateTime, Formatting.None, NodaConverters.LocalDateTimeConverter);

            Assert.AreEqual(jsonDateTime, jsonLocalDateTime);
        }

        [Test]
        public void LocalDateTimeConverter_SerializeNonIso_Throws()
        {
            var localDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, CalendarSystem.GetCopticCalendar(4));

            Assert.Throws<ArgumentException>(() => JsonConvert.SerializeObject(localDateTime, Formatting.None, NodaConverters.LocalDateTimeConverter));
        }

        [Test]
        public void LocalTimeConverter_Serialize()
        {
            var localTime = new LocalTime(1, 2, 3, 4, 5);
            var json = JsonConvert.SerializeObject(localTime, Formatting.None, NodaConverters.LocalTimeConverter);
            string expectedJson = "\"01:02:03.0040005\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void LocalTimeConverter_Deserialize()
        {
            string json = "\"01:02:03.0040005\"";
            var localTime = JsonConvert.DeserializeObject<LocalTime>(json, NodaConverters.LocalTimeConverter);
            var expectedLocalTime = new LocalTime(1, 2, 3, 4, 5);
            Assert.AreEqual(expectedLocalTime, localTime);
        }

        [Test]
        public void RoundtripPeriodConverter_Serialize()
        {
            var period = Period.FromDays(2) + Period.FromHours(3) + Period.FromMinutes(90);
            var json = JsonConvert.SerializeObject(period, Formatting.None, NodaConverters.RoundtripPeriodConverter);
            string expectedJson = "\"P2DT3H90M\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void PeriodConverter_Deserialize()
        {
            string json = "\"P2DT3H90M\"";
            var period = JsonConvert.DeserializeObject<Period>(json, NodaConverters.RoundtripPeriodConverter);
            var expectedPeriod = Period.FromDays(2) + Period.FromHours(3) + Period.FromMinutes(90);
            Assert.AreEqual(expectedPeriod, period);
        }

        [Test]
        public void NormalizingIsoPeriodConverter_Serialize()
        {
            var period = Period.FromDays(2) + Period.FromHours(3) + Period.FromMinutes(90);
            var json = JsonConvert.SerializeObject(period, Formatting.None, NodaConverters.NormalizingIsoPeriodConverter);
            string expectedJson = "\"P2DT4H30M\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void NormalizingIsoPeriodConverter_Deserialize()
        {
            string json = "\"P2DT4H30M\"";
            var period = JsonConvert.DeserializeObject<Period>(json, NodaConverters.NormalizingIsoPeriodConverter);
            var expectedPeriod = Period.FromDays(2) + Period.FromHours(4) + Period.FromMinutes(30);
            Assert.AreEqual(expectedPeriod, period);
        }
    }
}
