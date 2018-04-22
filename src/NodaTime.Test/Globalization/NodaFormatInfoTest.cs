// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using NodaTime.Calendars;
using NodaTime.Globalization;
using NodaTime.Test.Text;
using NUnit.Framework;

namespace NodaTime.Test.Globalization
{
    public class NodaFormatInfoTest
    {
        private static readonly CultureInfo enUs = Cultures.GetCultureInfo("en-US");
        private static readonly CultureInfo enGb = Cultures.GetCultureInfo("en-GB");

        // Just check we can actually build a NodaFormatInfo for every culture, outside
        // text-specific tests.
        [Test]
        [TestCaseSource(typeof(Cultures), nameof(Cultures.AllCultures))]
        public void ConvertCulture(CultureInfo culture)
        {
            NodaFormatInfo.GetFormatInfo(culture);
        }

        [Test]
        public void TestCachingWithReadOnly()
        {
            var original = new CultureInfo("en-US");
            // Use a read-only wrapper so that it gets cached
            var wrapper = CultureInfo.ReadOnly(original);

            var nodaWrapper1 = NodaFormatInfo.GetFormatInfo(wrapper);
            var nodaWrapper2 = NodaFormatInfo.GetFormatInfo(wrapper);
            Assert.AreSame(nodaWrapper1, nodaWrapper2);
        }

        [Test]
        public void TestCachingWithClonedCulture()
        {
            var original = new CultureInfo("en-US");
            var clone = (CultureInfo) original.Clone();
            Assert.AreEqual(original.Name, clone.Name);
            var dayNames = clone.DateTimeFormat.DayNames;
            dayNames[1] = "@@@";
            clone.DateTimeFormat.DayNames = dayNames;

            // Fool Noda Time into believing both are read-only, so it can use a cache...
            original = CultureInfo.ReadOnly(original);
            clone = CultureInfo.ReadOnly(clone);

            var nodaOriginal = NodaFormatInfo.GetFormatInfo(original);
            var nodaClone = NodaFormatInfo.GetFormatInfo(clone);
            Assert.AreEqual(original.DateTimeFormat.DayNames[1], nodaOriginal.LongDayNames[1]);
            Assert.AreEqual(clone.DateTimeFormat.DayNames[1], nodaClone.LongDayNames[1]);
            // Just check we made a difference...
            Assert.AreNotEqual(nodaOriginal.LongDayNames[1], nodaClone.LongDayNames[1]);
        }

        [Test]
        public void TestConstructor()
        {
            var info = new NodaFormatInfo(enUs);
            Assert.AreSame(enUs, info.CultureInfo);
            Assert.NotNull(info.DateTimeFormat);
            Assert.AreEqual(":", info.TimeSeparator);
            Assert.AreEqual("/", info.DateSeparator);
            Assert.IsInstanceOf<string>(info.OffsetPatternLong);
            Assert.IsInstanceOf<string>(info.OffsetPatternMedium);
            Assert.IsInstanceOf<string>(info.OffsetPatternShort);
            Assert.AreEqual("NodaFormatInfo[en-US]", info.ToString());
        }

        [Test]
        public void TestConstructor_null()
        {
            Assert.Throws<ArgumentNullException>(() => new NodaFormatInfo(null));
        }

        [Test]
        public void TestDateTimeFormat()
        {
            var format = DateTimeFormatInfo.InvariantInfo;
            var info = new NodaFormatInfo(enUs);
            Assert.AreNotEqual(format, info.DateTimeFormat);
        }

        [Test]
        public void TestGetFormatInfo()
        {
            NodaFormatInfo.ClearCache();
            var info1 = NodaFormatInfo.GetFormatInfo(enUs);
            Assert.NotNull(info1);

            var info2 = NodaFormatInfo.GetFormatInfo(enUs);
            Assert.AreSame(info1, info2);

            var info3 = NodaFormatInfo.GetFormatInfo(enGb);
            Assert.AreNotSame(info1, info3);
        }

        [Test]
        public void TestGetFormatInfo_null()
        {
            NodaFormatInfo.ClearCache();
            Assert.Throws<ArgumentNullException>(() => NodaFormatInfo.GetFormatInfo(null));
        }

        [Test]
        public void TestGetInstance_CultureInfo()
        {
            NodaFormatInfo.ClearCache();
            using (CultureSaver.SetCultures(enUs, FailingCultureInfo.Instance))
            {
                var actual = NodaFormatInfo.GetInstance(enGb);
                Assert.AreSame(enGb, actual.CultureInfo);
            }
        }

        [Test]
        public void TestGetInstance_UnusableType()
        {
            NodaFormatInfo.ClearCache();
            Assert.Throws<ArgumentException>(() => NodaFormatInfo.GetInstance(CultureInfo.InvariantCulture.NumberFormat));
        }

        [Test]
        public void TestGetInstance_DateTimeFormatInfo()
        {
            NodaFormatInfo.ClearCache();
            using (CultureSaver.SetCultures(enUs, FailingCultureInfo.Instance))
            {
                var info = NodaFormatInfo.GetInstance(enGb.DateTimeFormat);
                Assert.AreEqual(enGb.DateTimeFormat, info.DateTimeFormat);
                Assert.AreEqual(CultureInfo.InvariantCulture, info.CultureInfo);
            }
        }

        [Test]
        public void TestGetInstance_null()
        {
            NodaFormatInfo.ClearCache();
            using (CultureSaver.SetCultures(enUs, FailingCultureInfo.Instance))
            {
                var info = NodaFormatInfo.GetInstance(null);
                Assert.AreEqual(CultureInfo.CurrentCulture, info.CultureInfo);
            }
            using (CultureSaver.SetCultures(enGb, FailingCultureInfo.Instance))
            {
                var info = NodaFormatInfo.GetInstance(null);
                Assert.AreEqual(CultureInfo.CurrentCulture, info.CultureInfo);
            }
        }

        [Test]
        public void TestOffsetPatternLong()
        {
            const string pattern = "This is a test";
            var info = new NodaFormatInfo(enUs);
            Assert.AreNotEqual(pattern, info.OffsetPatternLong);
        }

        [Test]
        public void TestOffsetPatternMedium()
        {
            const string pattern = "This is a test";
            var info = new NodaFormatInfo(enUs);
            Assert.AreNotEqual(pattern, info.OffsetPatternMedium);
        }

        [Test]
        public void TestOffsetPatternShort()
        {
            const string pattern = "This is a test";
            var info = new NodaFormatInfo(enUs);
            Assert.AreNotEqual(pattern, info.OffsetPatternShort);
        }

        [Test]
        public void TestGetEraNames()
        {
            var info = NodaFormatInfo.GetFormatInfo(enUs);
            IList<string> names = info.GetEraNames(Era.BeforeCommon);
            CollectionAssert.AreEqual(new[] { "B.C.E.", "B.C.", "BCE", "BC" }, names);
        }

        [Test]
        public void TestGetEraNames_NoSuchEra()
        {
            var info = NodaFormatInfo.GetFormatInfo(enUs);
            Assert.AreEqual(0, info.GetEraNames(new Era("Ignored", "NonExistantResource")).Count);
        }

        [Test]
        public void TestEraGetNames_Null()
        {
            var info = NodaFormatInfo.GetFormatInfo(enUs);
            Assert.Throws<ArgumentNullException>(() => info.GetEraNames(null));
        }

        [Test]
        public void TestGetEraPrimaryName()
        {
            var info = NodaFormatInfo.GetFormatInfo(enUs);
            Assert.AreEqual("B.C.", info.GetEraPrimaryName(Era.BeforeCommon));
        }

        [Test]
        public void TestGetEraPrimaryName_NoSuchEra()
        {
            var info = NodaFormatInfo.GetFormatInfo(enUs);
            Assert.AreEqual("", info.GetEraPrimaryName(new Era("Ignored", "NonExistantResource")));
        }

        [Test]
        public void TestEraGetEraPrimaryName_Null()
        {
            var info = NodaFormatInfo.GetFormatInfo(enUs);
            Assert.Throws<ArgumentNullException>(() => info.GetEraPrimaryName(null));
        }

        [Test]
        public void TestIntegerGenitiveMonthNames()
        {
            // Emulate behavior of Mono 3.0.6
            var culture = (CultureInfo) CultureInfo.InvariantCulture.Clone();
            culture.DateTimeFormat.MonthGenitiveNames = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
            culture.DateTimeFormat.AbbreviatedMonthGenitiveNames = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
            var info = NodaFormatInfo.GetFormatInfo(culture);
            CollectionAssert.AreEqual(info.LongMonthGenitiveNames, info.LongMonthNames);
            CollectionAssert.AreEqual(info.ShortMonthGenitiveNames, info.ShortMonthNames);
        }
    }
}
