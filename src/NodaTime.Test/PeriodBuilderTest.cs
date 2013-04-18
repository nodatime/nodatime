// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class PeriodBuilderTest
    {
        [Test]
        public void Indexer_Getter_ValidUnits()
        {
            var builder = new PeriodBuilder
            {
                Months = 1,
                Weeks = 2,
                Days = 3,
                Hours = 4,
                Minutes = 5,
                Seconds = 6,
                Milliseconds = 7,
                Ticks = 8
            };
            Assert.AreEqual(0L, builder[PeriodUnits.Years]);
            Assert.AreEqual(1L, builder[PeriodUnits.Months]);
            Assert.AreEqual(2L, builder[PeriodUnits.Weeks]);
            Assert.AreEqual(3L, builder[PeriodUnits.Days]);
            Assert.AreEqual(4L, builder[PeriodUnits.Hours]);
            Assert.AreEqual(5L, builder[PeriodUnits.Minutes]);
            Assert.AreEqual(6L, builder[PeriodUnits.Seconds]);
            Assert.AreEqual(7L, builder[PeriodUnits.Milliseconds]);
            Assert.AreEqual(8L, builder[PeriodUnits.Ticks]);
        }

        [Test]
        public void Index_Getter_InvalidUnits()
        {
            var builder = new PeriodBuilder();
            Assert.Throws<ArgumentOutOfRangeException>(() => Call(builder[0]));
            Assert.Throws<ArgumentOutOfRangeException>(() => Call(builder[(PeriodUnits) (-1)]));
            Assert.Throws<ArgumentOutOfRangeException>(() => Call(builder[PeriodUnits.DateAndTime]));
        }

        [Test]
        public void Indexer_Setter_ValidUnits()
        {
            var builder = new PeriodBuilder();
            builder[PeriodUnits.Months] = 1;
            builder[PeriodUnits.Weeks] = 2;
            builder[PeriodUnits.Days] = 3;
            builder[PeriodUnits.Hours] = 4;
            builder[PeriodUnits.Minutes] = 5;
            builder[PeriodUnits.Seconds] = 6;
            builder[PeriodUnits.Milliseconds] = 7;
            builder[PeriodUnits.Ticks] = 8;
            var expected = new PeriodBuilder
            {
                Years = 0,
                Months = 1,
                Weeks = 2,
                Days = 3,
                Hours = 4,
                Minutes = 5,
                Seconds = 6,
                Milliseconds = 7,
                Ticks = 8
            }.Build();
            Assert.AreEqual(expected, builder.Build());
        }

        [Test]
        public void Index_Setter_InvalidUnits()
        {
            var builder = new PeriodBuilder();
            Assert.Throws<ArgumentOutOfRangeException>(() => builder[0] = 1L);
            Assert.Throws<ArgumentOutOfRangeException>(() => builder[(PeriodUnits)(-1)] = 1L);
            Assert.Throws<ArgumentOutOfRangeException>(() => builder[PeriodUnits.DateAndTime] = 1L);
        }

        [Test]
        public void Build_SingleUnit()
        {
            Period period = new PeriodBuilder { Hours = 10 }.Build();

            Period expected = Period.FromHours(10);
            Assert.AreEqual(expected, period);
        }

        [Test]
        public void Build_MultipleUnits()
        {
            Period period = new PeriodBuilder { Days = 5, Minutes = -10 }.Build();

            Period expected = Period.FromDays(5) + Period.FromMinutes(-10);
            Assert.AreEqual(expected, period);
        }

        [Test]
        public void Build_Zero()
        {
            Assert.AreEqual(Period.Zero, new PeriodBuilder().Build());
        }

        [Test]
        public void XmlSerialization()
        {
            var value = new PeriodBuilder { Years = 10, Days = 5, Hours = 3, Seconds = 20 };
            TestHelper.AssertXmlRoundtrip(value, "<value>P10Y5DT3H20S</value>", new BuilderEqualityComparer());
        }

        [Test]
        public void XmlSerialization_ExpectedUse()
        {
            var demo = new XmlSerializationDemo { Period = Period.FromMinutes(5) };
            var serializer = new XmlSerializer(typeof(XmlSerializationDemo));
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, demo);
                stream.Position = 0;
                var deserialized = (XmlSerializationDemo) serializer.Deserialize(stream);
                Assert.AreEqual(demo.Period, deserialized.Period);
            }
        }

        /// <summary>
        /// Sample class to show how to serialize classes which have Period properties.
        /// </summary>
        public class XmlSerializationDemo
        {
            /// <summary>
            /// Use this property!
            /// </summary>
            [XmlIgnore]
            public Period Period { get; set; }

            /// <summary>
            /// Don't use this property! It's only present for the purposes of XML serialization.
            /// We'd mark it as obsolete, but then the XML serializer would ignore it...
            /// </summary>
            [XmlElement("period")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public PeriodBuilder PeriodBuilder
            {
                get { return Period == null ? null : Period.ToBuilder(); }
                set { Period = value == null ? null : value.Build(); }
            }
        }

        private class BuilderEqualityComparer : IEqualityComparer<PeriodBuilder>
        {
            public bool Equals(PeriodBuilder x, PeriodBuilder y)
            {
                return x.Build().Equals(y.Build());
            }

            public int GetHashCode(PeriodBuilder obj)
            {
                return obj.Build().GetHashCode();
            }
        }

        private static void Call(object ignored) {}
    }
}
