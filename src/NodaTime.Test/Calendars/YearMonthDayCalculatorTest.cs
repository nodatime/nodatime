// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NodaTime.Calendars;

namespace NodaTime.Test.Calendars
{
    [TestFixture]
    public class YearMonthDayCalculatorTest
    {
        private static readonly TestCaseData[] NonIslamicCalculators = {
            new TestCaseData(new GregorianYearMonthDayCalculator()).SetName("Gregorian"),
            new TestCaseData(new IsoYearMonthDayCalculator()).SetName("ISO"),
            new TestCaseData(new CopticYearMonthDayCalculator()).SetName("Coptic"),
            new TestCaseData(new JulianYearMonthDayCalculator()).SetName("Julian"),
        };

        private static readonly TestCaseData[] IslamicCalculators =
            (from epoch in Enum.GetValues(typeof(IslamicEpoch)).Cast<IslamicEpoch>()
             from leapYearPattern in Enum.GetValues(typeof(IslamicLeapYearPattern)).Cast<IslamicLeapYearPattern>()
             let calculator = new IslamicYearMonthDayCalculator(leapYearPattern, epoch)
             select new TestCaseData(calculator).SetName(string.Format("Islamic: {0}, {1}", epoch, leapYearPattern)))
             .ToArray();

#pragma warning disable 0414 // Used by tests via reflection - do not remove!
        private static IEnumerable<TestCaseData> AllCalculators = NonIslamicCalculators.Concat(IslamicCalculators);
#pragma warning restore 0414

        // Note for tests using TestCaseSource:
        // We can't make the parameter of type YearMonthDayCalculator, because that's internal.
        // We can't make the method internal, as then it isn't a test. Casting is all we've got.

        [Test]
        [TestCaseSource("AllCalculators")]
        public void ValidateYear1Ticks(object calculatorAsObject)
        {
            var calculator = (YearMonthDayCalculator) calculatorAsObject;
            Assert.AreEqual(calculator.GetYearTicks(1), calculator.TicksAtStartOfYear1);
        }

        [Test]
        [TestCaseSource("AllCalculators")]
        public void GetYearConsistentWithGetYearTicks(object calculatorAsObject)
        {
            var calculator = (YearMonthDayCalculator)calculatorAsObject;
            for (int year = calculator.MinYear; year <= calculator.MaxYear; year++)
            {
                long startOfYearTicks = calculator.GetYearTicks(year);
                Assert.AreEqual(year, calculator.GetYear(new LocalInstant(startOfYearTicks)), "Start of year {0}", year);
                Assert.AreEqual(year - 1, calculator.GetYear(new LocalInstant(startOfYearTicks - 1)), "End of year {0}", year - 1);
            }
        }
    }
}
