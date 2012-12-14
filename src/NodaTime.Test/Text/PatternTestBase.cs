#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using NodaTime.Calendars;

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
