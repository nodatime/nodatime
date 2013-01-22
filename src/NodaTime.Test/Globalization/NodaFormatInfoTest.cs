// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using NodaTime.Calendars;
using NodaTime.Globalization;

namespace NodaTime.Test.Globalization
{
    [TestFixture]
    public class NodaFormatInfoTest
    {
        private readonly CultureInfo enUs = CultureInfo.GetCultureInfo("en-US");
        private readonly CultureInfo enGb = CultureInfo.GetCultureInfo("en-GB");

        private sealed class EmptyFormatProvider : IFormatProvider
        {
            #region IFormatProvider Members
            public object GetFormat(Type formatType)
            {
                return null;
            }
            #endregion
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
            clone.DateTimeFormat.DateSeparator = "@@@";

            // Fool Noda Time into believing both are read-only, so it can use a cache...
            original = CultureInfo.ReadOnly(original);
            clone = CultureInfo.ReadOnly(clone);

            var nodaOriginal = NodaFormatInfo.GetFormatInfo(original);
            var nodaClone = NodaFormatInfo.GetFormatInfo(clone);
            Assert.AreEqual(original.DateTimeFormat.DateSeparator, nodaOriginal.DateSeparator);
            Assert.AreEqual(clone.DateTimeFormat.DateSeparator, nodaClone.DateSeparator);
        }

        [Test]
        public void TestConstructor()
        {
            var info = new NodaFormatInfo(enUs);
            Assert.AreSame(enUs, info.CultureInfo);
            Assert.NotNull(info.NumberFormat);
            Assert.NotNull(info.DateTimeFormat);
            Assert.AreEqual(".", info.DecimalSeparator);
            Assert.AreEqual("+", info.PositiveSign);
            Assert.AreEqual("-", info.NegativeSign);
            Assert.AreEqual(":", info.TimeSeparator);
            Assert.AreEqual("/", info.DateSeparator);
            Assert.IsFalse(info.IsReadOnly);
            Assert.IsInstanceOf<string>(info.OffsetPatternFull);
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
            info.DateTimeFormat = format;
            Assert.AreEqual(format, info.DateTimeFormat);
            Assert.Throws<ArgumentNullException>(() => info.DateTimeFormat = null);
            info.IsReadOnly = true;
            Assert.Throws<InvalidOperationException>(() => info.DateTimeFormat = format);
        }

        [Test]
        public void TestGetFormat()
        {
            using (CultureSaver.SetCultures(enGb, FailingCultureInfo.Instance))
            {
                var info = new NodaFormatInfo(enUs);
                Assert.AreSame(info, info.GetFormat(typeof(NodaFormatInfo)));

                var actualNfi = info.GetFormat(typeof(NumberFormatInfo));
                Assert.AreSame(enUs.NumberFormat, actualNfi);
                Assert.AreNotSame(Thread.CurrentThread.CurrentCulture.NumberFormat, actualNfi);

                var actualDtfi = info.GetFormat(typeof(DateTimeFormatInfo));
                Assert.AreSame(enUs.DateTimeFormat, actualDtfi);
                Assert.AreNotSame(Thread.CurrentThread.CurrentCulture.DateTimeFormat, actualDtfi);
            }
        }

        [Test]
        public void TestGetFormatInfo()
        {
            NodaFormatInfo.ClearCache();
            var info1 = NodaFormatInfo.GetFormatInfo(enUs);
            Assert.NotNull(info1);
            Assert.IsTrue(info1.IsReadOnly);

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
                Assert.AreSame(enGb.Name, actual.Name);
            }
        }

        [Test]
        public void TestGetInstance_IFormatProvider()
        {
            NodaFormatInfo.ClearCache();
            using (CultureSaver.SetCultures(enUs, FailingCultureInfo.Instance))
            {
                var provider = new EmptyFormatProvider();
                var actual = NodaFormatInfo.GetInstance(provider);
                Assert.AreSame(enUs.Name, actual.Name);
            }
        }

        [Test]
        public void TestGetInstance_NodaFormatInfo()
        {
            NodaFormatInfo.ClearCache();
            using (CultureSaver.SetCultures(enUs, FailingCultureInfo.Instance))
            {
                var info = new NodaFormatInfo(enGb);
                var actual = NodaFormatInfo.GetInstance(info);
                Assert.AreSame(info, actual);
            }
        }

        [Test]
        public void TestGetInstance_null()
        {
            NodaFormatInfo.ClearCache();
            using (CultureSaver.SetCultures(enUs, FailingCultureInfo.Instance))
            {
                var info = NodaFormatInfo.GetInstance(null);
                Assert.AreEqual(Thread.CurrentThread.CurrentCulture, info.CultureInfo);
            }
            using (CultureSaver.SetCultures(enGb, FailingCultureInfo.Instance))
            {
                var info = NodaFormatInfo.GetInstance(null);
                Assert.AreEqual(Thread.CurrentThread.CurrentCulture, info.CultureInfo);
            }
        }

        [Test]
        public void TestIsReadOnly()
        {
            var info = new NodaFormatInfo(enUs);
            Assert.IsFalse(info.IsReadOnly);
            info.IsReadOnly = false;
            Assert.IsFalse(info.IsReadOnly);
            info.IsReadOnly = false;
            Assert.IsFalse(info.IsReadOnly);
            info.IsReadOnly = true;
            Assert.IsTrue(info.IsReadOnly);
            info.IsReadOnly = true;
            Assert.IsTrue(info.IsReadOnly);
            Assert.Throws<InvalidOperationException>(() => info.IsReadOnly = false);
        }

        [Test]
        public void TestNumberFormat()
        {
            var format = NumberFormatInfo.InvariantInfo;
            var info = new NodaFormatInfo(enUs);
            Assert.AreNotEqual(format, info.NumberFormat);
            info.NumberFormat = format;
            Assert.AreEqual(format, info.NumberFormat);
            Assert.Throws<ArgumentNullException>(() => info.NumberFormat = null);
            info.IsReadOnly = true;
            Assert.Throws<InvalidOperationException>(() => info.NumberFormat = format);
        }

        [Test]
        public void TestOffsetPatternFull()
        {
            const string pattern = "This is a test";
            var info = new NodaFormatInfo(enUs);
            Assert.AreNotEqual(pattern, info.OffsetPatternFull);
            info.OffsetPatternFull = pattern;
            Assert.AreEqual(pattern, info.OffsetPatternFull);
            Assert.Throws<ArgumentNullException>(() => info.OffsetPatternFull = null);
            info.IsReadOnly = true;
            Assert.Throws<InvalidOperationException>(() => info.OffsetPatternFull = "abc");
        }

        [Test]
        public void TestOffsetPatternLong()
        {
            const string pattern = "This is a test";
            var info = new NodaFormatInfo(enUs);
            Assert.AreNotEqual(pattern, info.OffsetPatternLong);
            info.OffsetPatternLong = pattern;
            Assert.AreEqual(pattern, info.OffsetPatternLong);
            Assert.Throws<ArgumentNullException>(() => info.OffsetPatternLong = null);
            info.IsReadOnly = true;
            Assert.Throws<InvalidOperationException>(() => info.OffsetPatternLong = "abc");
        }

        [Test]
        public void TestOffsetPatternMedium()
        {
            const string pattern = "This is a test";
            var info = new NodaFormatInfo(enUs);
            Assert.AreNotEqual(pattern, info.OffsetPatternMedium);
            info.OffsetPatternMedium = pattern;
            Assert.AreEqual(pattern, info.OffsetPatternMedium);
            Assert.Throws<ArgumentNullException>(() => info.OffsetPatternMedium = null);
            info.IsReadOnly = true;
            Assert.Throws<InvalidOperationException>(() => info.OffsetPatternMedium = "abc");
        }

        [Test]
        public void TestOffsetPatternShort()
        {
            const string pattern = "This is a test";
            var info = new NodaFormatInfo(enUs);
            Assert.AreNotEqual(pattern, info.OffsetPatternShort);
            info.OffsetPatternShort = pattern;
            Assert.AreEqual(pattern, info.OffsetPatternShort);
            Assert.Throws<ArgumentNullException>(() => info.OffsetPatternShort = null);
            info.IsReadOnly = true;
            Assert.Throws<InvalidOperationException>(() => info.OffsetPatternShort = "abc");
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
    }
}
