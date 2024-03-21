// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    public class YearMonthDayCalculatorTest
    {
        // Here the term "Islamic" only refers to whether the implementation is IslamicYearMonthDayCalculator,
        // not whether the calendar itself is based on Islamic scripture.
        private static readonly NamedWrapper<YearMonthDayCalculator>[] NonIslamicCalculators =
        {
            new NamedWrapper<YearMonthDayCalculator>(new GregorianYearMonthDayCalculator(), "Gregorian"),
            new NamedWrapper<YearMonthDayCalculator>(new CopticYearMonthDayCalculator(), "Coptic"),
            new NamedWrapper<YearMonthDayCalculator>(new JulianYearMonthDayCalculator(), "Julian"),
            new NamedWrapper<YearMonthDayCalculator>(new HebrewYearMonthDayCalculator(HebrewMonthNumbering.Civil), "HebrewCivil"),
            new NamedWrapper<YearMonthDayCalculator>(new HebrewYearMonthDayCalculator(HebrewMonthNumbering.Scriptural), "HebrewScriptural"),
            new NamedWrapper<YearMonthDayCalculator>(new PersianYearMonthDayCalculator.Simple(), "PersianSimple"),
            new NamedWrapper<YearMonthDayCalculator>(new PersianYearMonthDayCalculator.Arithmetic(), "PersianArithmetic"),
            new NamedWrapper<YearMonthDayCalculator>(new PersianYearMonthDayCalculator.Astronomical(), "PersianAstronomical"),
            new NamedWrapper<YearMonthDayCalculator>(new UmAlQuraYearMonthDayCalculator(), "UmAlQura)"),
        };

        private static readonly NamedWrapper<YearMonthDayCalculator>[] IslamicCalculators =
            (from epoch in Enum.GetValues(typeof(IslamicEpoch)).Cast<IslamicEpoch>()
             from leapYearPattern in Enum.GetValues(typeof(IslamicLeapYearPattern)).Cast<IslamicLeapYearPattern>()
             let calculator = new IslamicYearMonthDayCalculator(leapYearPattern, epoch)
             select new NamedWrapper<YearMonthDayCalculator>(calculator, $"Islamic;{epoch};{leapYearPattern}"))
             .ToArray();

        private static readonly IEnumerable<NamedWrapper<YearMonthDayCalculator>> AllCalculators = NonIslamicCalculators.Concat(IslamicCalculators);

        // Note for tests:
        // We can't make the parameter of type YearMonthDayCalculator, because that's internal.
        // We can't make the method internal, as then it isn't a test. Casting is all we've got.
        [Test]
        [TestCaseSource(nameof(AllCalculators))]
        public void ValidateStartOfYear1Days(object calculatorWrapper)
        {
            var calculator = ((NamedWrapper<YearMonthDayCalculator>) calculatorWrapper).Value;
            // Some calendars (e.g. Um Al Qura) don't support year 1, so the DaysAtStartOfYear1
            // is somewhat theoretical. (It's still used in such calendars, but only to get a guess
            // as to a year number given a day number.)
            if (calculator.MinYear > 1 || calculator.MaxYear < 0)
            {
                return;
            }
            Assert.AreEqual(calculator.GetStartOfYearInDays(1), calculator.DaysAtStartOfYear1);
        }

        [Test]
        [TestCaseSource(nameof(AllCalculators))]
        public void GetYearConsistentWithGetYearDays(object calculatorWrapper)
        {
            var calculator = ((NamedWrapper<YearMonthDayCalculator>) calculatorWrapper).Value;
            for (int year = calculator.MinYear; year <= calculator.MaxYear; year++)
            {
                int startOfYearDays = calculator.GetStartOfYearInDays(year);
                Assert.AreEqual(year, calculator.GetYear(startOfYearDays, out int dayOfYear), "Start of year {0}", year);
                Assert.AreEqual(0, dayOfYear); // Zero-based...
                Assert.AreEqual(year - 1, calculator.GetYear(startOfYearDays - 1, out dayOfYear), "End of year {0}", year - 1);
                Assert.AreEqual(calculator.GetDaysInYear(year - 1) - 1, dayOfYear);
            }
        }
    }
}
