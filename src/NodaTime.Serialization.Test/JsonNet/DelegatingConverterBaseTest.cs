// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Newtonsoft.Json;
using NodaTime.Serialization.JsonNet;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Serialization.Test.JsonNet
{
    public class DelegatingConverterBaseTest
    {
        [Test]
        public void Serialize()
        {
            string expected = "{'ShortDate':'2017-02-20','LongDate':'20 February 2017'}"
                .Replace("'", "\"");
            var date = new LocalDate(2017, 2, 20);
            var entity = new Entity { ShortDate = date, LongDate = date };
            var actual = JsonConvert.SerializeObject(entity, Formatting.None);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Deserialize()
        {
            string json = "{'ShortDate':'2017-02-20','LongDate':'20 February 2017'}"
                .Replace("'", "\"");
            var expectedDate = new LocalDate(2017, 2, 20);
            var entity = JsonConvert.DeserializeObject<Entity>(json);
            Assert.AreEqual(expectedDate, entity.ShortDate);
            Assert.AreEqual(expectedDate, entity.LongDate);
        }

        public class Entity
        {
            [JsonConverter(typeof(ShortDateConverter))]
            public LocalDate ShortDate { get; set; }

            [JsonConverter(typeof(LongDateConverter))]
            public LocalDate LongDate { get; set; }
        }

        public class ShortDateConverter : DelegatingConverterBase
        {
            public ShortDateConverter() : base(NodaConverters.LocalDateConverter) { }
        }

        public class LongDateConverter : DelegatingConverterBase
        {
            // No need to create a new one of these each time...
            private static readonly JsonConverter converter =
                new NodaPatternConverter<LocalDate>(LocalDatePattern.CreateWithInvariantCulture("d MMMM yyyy"));

            public LongDateConverter() : base(converter)
            {
            }
        }
    }
}
