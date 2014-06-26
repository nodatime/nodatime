// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    public partial class IsoCalendarSystemTest
    {
        // These tests assume that if the method doesn't throw, it's doing the right thing - this
        // is all tested elsewhere.
        [Test]
        public void ValidateYearMonthDay_AllValues_ValidValuesDoesntThrow()
        {
            Iso.ValidateYearMonthDay(20, 2, 20);
        }

        [Test]
        public void ValidateYearMonthDay_InvalidYear_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Iso.ValidateYearMonthDay(50000, 2, 20));
        }

        [Test]
        public void GetLocalInstant_InvalidMonth_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Iso.ValidateYearMonthDay(2010, 13, 20));
        }

        [Test]
        public void GetLocalInstant_29thOfFebruaryInNonLeapYear_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Iso.ValidateYearMonthDay(2010, 2, 29));
        }

        [Test]
        public void GetLocalInstant_29thOfFebruaryInLeapYear_DoesntThrow()
        {
            Iso.ValidateYearMonthDay(2012, 2, 29);
        }
    }
}