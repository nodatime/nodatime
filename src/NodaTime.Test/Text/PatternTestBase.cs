// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Globalization;
using NodaTime.Calendars;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Text
{
    /// <summary>
    /// Base class for all the pattern tests (when we've migrated OffsetPattern off FormattingTestSupport).
    /// Derived classes should have internal static fields with the names listed in the TestCaseSource
    /// attributes here: InvalidPatternData, ParseFailureData, ParseData, FormatData. Any field
    /// which is missing cause that test to be "not runnable" for that concrete subclass.
    /// If a test isn't appropriate (e.g. there's no configurable pattern) just provide a property with
    /// an array containing a null value - that will be ignored.
    /// </summary>
    public abstract class PatternTestBase<T>
    {
        [Test]
        [TestCaseSource("InvalidPatternData")]
        public void InvalidPatterns(PatternTestData<T> data)
        {
            data?.TestInvalidPattern();
        }

        [Test]
        [TestCaseSource("ParseFailureData")]
        public void ParseFailures(PatternTestData<T> data)
        {
            data?.TestParseFailure();
        }

        [Test]
        [TestCaseSource("ParseData")]
        public void Parse(PatternTestData<T> data)
        {
            data?.TestParse();
        }

        [Test]
        [TestCaseSource("FormatData")]
        public void Format(PatternTestData<T> data)
        {
            data?.TestFormat();
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
            // Yes, this is horrible... but the specific calendars aren't available to test
            // against in the PCL
            switch (bcl.GetType().Name)
            {
                case "GregorianCalendar": return CalendarSystem.Iso;
                case "HijriCalendar": return CalendarSystem.IslamicBcl;
                case "HebrewCalendar": return CalendarSystem.HebrewCivil;
                case "PersianCalendar": return bcl.IsLeapYear(1) ? CalendarSystem.PersianSimple : CalendarSystem.PersianAstronomical;
                case "UmAlQuraCalendar": return CalendarSystem.UmAlQura;
                case "JulianCalendar":
                    return CalendarSystem.Julian;
                default:
                    // No idea - we can't test with this calendar...
                    return null;
            }
        }
    }
}
