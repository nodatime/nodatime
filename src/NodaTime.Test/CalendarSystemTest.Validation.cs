// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test
{
    // Tests for validation of public methods on CalendarSystem. These typically use the ISO calendar, just for simplicity.
    partial class CalendarSystemTest
    {
        private static readonly CalendarSystem Iso = CalendarSystem.Iso;

        [Test, TestCase(-9998), TestCase(9999)]
        public void GetMonthsInYear_Valid(int year)
        {
            TestHelper.AssertValid(Iso.GetMonthsInYear, year);
        }

        [Test, TestCase(-9999), TestCase(10000)]
        public void GetMonthsInYear_Invalid(int year)
        {
            TestHelper.AssertOutOfRange(Iso.GetMonthsInYear, year);
        }

        [Test, TestCase(-9998, 1), TestCase(9999, 12)]
        public void GetDaysInMonth_Valid(int year, int month)
        {
            TestHelper.AssertValid(Iso.GetDaysInMonth, year, month);
        }

        [Test, TestCase(-9999, 1), TestCase(1, 0), TestCase(1, 13), TestCase(10000, 1)]
        public void GetDaysInMonth_Invalid(int year, int month)
        {
            TestHelper.AssertOutOfRange(Iso.GetDaysInMonth, year, month);
        }

        [Test]
        public void GetDaysInMonth_Hebrew()
        {
            TestHelper.AssertValid(CalendarSystem.HebrewCivil.GetDaysInMonth, 5402, 13); // Leap year
            TestHelper.AssertOutOfRange(CalendarSystem.HebrewCivil.GetDaysInMonth, 5401, 13); // Not a leap year
        }

        [Test, TestCase(-9998), TestCase(9999)]
        public void IsLeapYear_Valid(int year)
        {
            TestHelper.AssertValid(Iso.IsLeapYear, year);
        }

        [Test, TestCase(-9999), TestCase(10000)]
        public void IsLeapYear_Invalid(int year)
        {
            TestHelper.AssertOutOfRange(Iso.IsLeapYear, year);
        }

        [Test, TestCase(1), TestCase(9999)]
        public void GetAbsoluteYear_ValidCe(int year)
        {
            TestHelper.AssertValid(Iso.GetAbsoluteYear, year, Era.Common);
        }

        [Test, TestCase(1), TestCase(9999)]
        public void GetAbsoluteYear_ValidBce(int year)
        {
            TestHelper.AssertValid(Iso.GetAbsoluteYear, year, Era.BeforeCommon);
        }

        [Test, TestCase(0), TestCase(10000)]
        public void GetAbsoluteYear_InvalidCe(int year)
        {
            TestHelper.AssertOutOfRange(Iso.GetAbsoluteYear, year, Era.Common);
        }

        [Test, TestCase(0), TestCase(10000)]
        public void GetAbsoluteYear_InvalidBce(int year)
        {
            TestHelper.AssertOutOfRange(Iso.GetAbsoluteYear, year, Era.BeforeCommon);
        }

        [Test]
        public void GetAbsoluteYear_InvalidEra()
        {
            TestHelper.AssertInvalid(Iso.GetAbsoluteYear, 1, Era.AnnoPersico);
        }

        [Test]
        public void GetAbsoluteYear_NullEra()
        {
            TestHelper.AssertArgumentNull(Iso.GetAbsoluteYear, 1, (Era) null!);
        }

        [Test]
        public void GetMinYearOfEra_NullEra()
        {
            TestHelper.AssertArgumentNull(Iso.GetMinYearOfEra, (Era) null!);
        }

        [Test]
        public void GetMinYearOfEra_InvalidEra()
        {
            TestHelper.AssertInvalid(Iso.GetMinYearOfEra, Era.AnnoPersico);
        }

        [Test]
        public void GetMaxYearOfEra_NullEra()
        {
            TestHelper.AssertArgumentNull(Iso.GetMaxYearOfEra, (Era) null!);
        }

        [Test]
        public void GetMaxYearOfEra_InvalidEra()
        {
            TestHelper.AssertInvalid(Iso.GetMaxYearOfEra, Era.AnnoPersico);
        }
    }
}
