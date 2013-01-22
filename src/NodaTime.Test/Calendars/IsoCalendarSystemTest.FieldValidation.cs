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
        public void GetLocalInstant_AllValues_ValidValuesDoesntThrow()
        {
            Iso.GetLocalInstant(2010, 2, 20, 21, 57, 30, 250, 1234);
        }

        [Test]
        public void GetLocalInstant_InvalidYear_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Iso.GetLocalInstant(50000, 2, 20, 21, 57, 30, 250, 1234));
        }

        [Test]
        public void GetLocalInstant_InvalidMonth_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Iso.GetLocalInstant(2010, 13, 20, 21, 57, 30, 250, 1234));
        }

        [Test]
        public void GetLocalInstant_29thOfFebruaryInNonLeapYear_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Iso.GetLocalInstant(2010, 2, 29, 21, 57, 30, 250, 1234));
        }

        [Test]
        public void GetLocalInstant_29thOfFebruaryInLeapYear_DoesntThrow()
        {
            Iso.GetLocalInstant(2012, 2, 29, 21, 57, 30, 250, 1234);
        }

        [Test]
        public void GetLocalInstant_InvalidHour_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Iso.GetLocalInstant(2010, 2, 20, 24, 57, 30, 250, 1234));
        }

        [Test]
        public void GetLocalInstant_InvalidMinute_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Iso.GetLocalInstant(2010, 2, 20, 21, 60, 30, 250, 1234));
        }

        [Test]
        public void GetLocalInstant_InvalidSecond_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Iso.GetLocalInstant(2010, 2, 20, 21, 57, 60, 250, 1234));
        }

        [Test]
        public void GetLocalInstant_InvalidMillisecond_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Iso.GetLocalInstant(2010, 2, 20, 21, 57, 30, 1000, 1234));
        }

        [Test]
        public void GetLocalInstant_InvalidTickOfMillisecond_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Iso.GetLocalInstant(2010, 2, 20, 21, 57, 30, 250, 10000));
        }
    }
}