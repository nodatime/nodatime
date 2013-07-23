// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using NodaTime.Calendars;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    /// <summary>
    /// Base class for all the pattern tests (when we've migrated OffsetPattern off FormattingTestSupport).
    /// Derived classes should have internal static fields with the names listed in the TestCaseSource
    /// attributes here: InvalidPatternData, ParseFailureData, ParseData, FormatData. Any field
    /// which is missing cause that test to be "not runnable" for that concrete subclass.
    /// </summary>
    public abstract class PatternTestBase<T>
    {
        [Test]
        [TestCaseSource("InvalidPatternData")]
        public void InvalidPatterns(PatternTestData<T> data)
        {
            data.TestInvalidPattern();
        }

        [Test]
        [TestCaseSource("ParseFailureData")]
        public void ParseFailures(PatternTestData<T> data)
        {
            data.TestParseFailure();
        }

        [Test]
        [TestCaseSource("ParseData")]
        public void Parse(PatternTestData<T> data)
        {
            data.TestParse();
        }

        [Test]
        [TestCaseSource("FormatData")]
        public void Format(PatternTestData<T> data)
        {
            data.TestFormat();
        }

        protected void AssertRoundTrip(T value, IPattern<T> pattern)
        {
            string text = pattern.Format(value);
            var parseResult = pattern.Parse(text);
            Assert.AreEqual(value, parseResult.Value);            
        }

        /// <summary>
        /// Tries to work out a roughly-matching calendar system for the given BCL calendar.
        /// This is needed where we're testing whether days of the week match - even if we can
        /// get day/month/year values to match without getting the calendar right, the calendar
        /// affects the day of week.
        /// </summary>
        internal static CalendarSystem CalendarSystemForCalendar(Calendar bcl)
        {
            if (bcl is GregorianCalendar)
            {
                return CalendarSystem.Iso;
            }
            if (bcl is HijriCalendar)
            {
                return CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Astronomical);
            }
            // This is *not* a general rule. Noda Time simply doesn't support this calendar, which requires
            // table-based data. However, using the Islamic calendar with the civil epoch gives the same date for
            // our sample, which is good enough for now...
            if (bcl is UmAlQuraCalendar)
            {
                // ... On Mono, this actually might not be good enough. So let's just punt on it - the Mono
                // implementation of UmAlQuraCalendar currently differs from the Windows one, but may get fixed
                // at some point. Let's just abort the test.
                if (TestHelper.IsRunningOnMono)
                {
                    return null;
                }
                return CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Civil);
            }
            if (bcl is JulianCalendar)
            {
                return CalendarSystem.GetJulianCalendar(4);
            }
            // No idea - we can't test with this calendar...
            return null;
        }
    }
}
